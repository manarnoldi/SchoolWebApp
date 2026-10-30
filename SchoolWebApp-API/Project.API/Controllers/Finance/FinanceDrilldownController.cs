using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Project.Infrastructure.Data;
using SchoolWebApp.Core.Entities.Finance;
using SchoolWebApp.API.Licensing;
using SchoolWebApp.API.Security;

namespace SchoolWebApp.API.Controllers.Finance
{
    // The transactions behind a figure on a finance report: an account's ledger, the expenses
    // behind a budget line, and any single document (invoice, receipt, expense, journal entry,
    // sponsor payment) laid out the same way, so the reports can drill down from any of them.
    [Route("api/financeDrilldown")]
    [ApiController]
    [Authorize]
    [RequiresModule(AppModules.Finance)]
    [RequiresMenu(MenuGroup.Finance)]
    public class FinanceDrilldownController : ControllerBase
    {
        private readonly ApplicationDbContext _db;
        public FinanceDrilldownController(ApplicationDbContext db) { _db = db; }

        // A drill target: a document to open, or an account's ledger.
        private static object Doc(string type, int id) => new { kind = "document", type, id };
        private static object Ledger(int accountId) => new { kind = "ledger", accountId };

        // An enum value as words: PartiallyPaid -> "Partially paid", Mpesa -> "M-Pesa".
        public static string Label(Enum value) => value.ToString() == "Mpesa" ? "M-Pesa"
            : System.Text.RegularExpressions.Regex.Replace(value.ToString(), "(?<=[a-z])([A-Z])", m => " " + m.Value.ToLower());

        private static bool DebitNatural(AccountType t) => t == AccountType.Asset || t == AccountType.Expense;

        // GET api/financeDrilldown/ledger?accountId=4&from=2026-01-01&to=2026-12-31
        [HttpGet("ledger")]
        public async Task<IActionResult> AccountLedger(int accountId, DateTime from, DateTime to)
        {
            var account = await _db.Accounts.AsNoTracking().FirstOrDefaultAsync(a => a.Id == accountId);
            if (account == null) return NotFound(new { message = "Account not found." });
            var sign = DebitNatural(account.AccountType) ? 1 : -1;
            var toEnd = to.Date.AddDays(1).AddTicks(-1);

            var posted = _db.JournalLines.AsNoTracking().Where(l => l.AccountId == accountId && l.JournalEntry!.IsPosted);
            // Balance accounts carry a balance in from before the period; income and expense start at nil.
            decimal opening = 0;
            if (account.AccountType is AccountType.Asset or AccountType.Liability or AccountType.Equity)
                opening = sign * (await posted.Where(l => l.JournalEntry!.EntryDate < from.Date).SumAsync(l => (decimal?)(l.Debit - l.Credit)) ?? 0);

            var lines = await posted.Where(l => l.JournalEntry!.EntryDate >= from.Date && l.JournalEntry.EntryDate <= toEnd)
                .OrderBy(l => l.JournalEntry!.EntryDate).ThenBy(l => l.JournalEntryId)
                .Select(l => new { l.JournalEntryId, l.JournalEntry!.EntryDate, l.JournalEntry.ReferenceNumber,
                    EntryDescription = l.JournalEntry.Description, l.Description, l.Debit, l.Credit })
                .ToListAsync();
            var sources = await ResolveSources(lines.Select(l => l.ReferenceNumber));

            var balance = opening;
            var rows = lines.Select(l =>
            {
                balance += sign * (l.Debit - l.Credit);
                var src = sources.GetValueOrDefault(l.ReferenceNumber);
                return new
                {
                    date = l.EntryDate,
                    reference = src?.Reference ?? l.ReferenceNumber,
                    party = src?.Party,
                    description = l.Description ?? l.EntryDescription,
                    debit = l.Debit,
                    credit = l.Credit,
                    balance,
                    drill = src != null ? Doc(src.Type, src.Id) : Doc("journal", l.JournalEntryId)
                };
            }).ToList();

            return Ok(new
            {
                account = new { id = account.Id, code = account.Code, name = account.Name, type = account.AccountType.ToString() },
                from, to, opening,
                totalDebit = lines.Sum(l => l.Debit),
                totalCredit = lines.Sum(l => l.Credit),
                closing = balance,
                lines = rows
            });
        }

        // GET api/financeDrilldown/budgetLine?academicYearId=8&departmentId=17&accountId=29
        // The approved expenses a department charged to its budget lines for one account.
        [HttpGet("budgetLine")]
        public async Task<IActionResult> BudgetLineExpenses(int academicYearId, int departmentId, int accountId)
        {
            var lineIds = await _db.BudgetLines.AsNoTracking()
                .Where(l => l.AccountId == accountId && l.Budget!.DepartmentId == departmentId && l.Budget.AcademicYearId == academicYearId && l.Budget.IsActive)
                .Select(l => l.Id).ToListAsync();
            var rows = await _db.ExpenseLines.AsNoTracking()
                .Where(l => l.BudgetLineId != null && lineIds.Contains(l.BudgetLineId.Value) && l.Expense!.Status == ExpenseStatus.Approved)
                .OrderBy(l => l.Expense!.ExpenseDate)
                .Select(l => new
                {
                    date = l.Expense!.ExpenseDate,
                    reference = l.Expense.ReferenceNumber,
                    category = l.ExpenseCategory!.Name,
                    vendor = l.Vendor,
                    description = l.Description ?? l.Expense.Description,
                    amount = l.Amount,
                    expenseId = l.ExpenseId
                }).ToListAsync();
            var account = await _db.Accounts.AsNoTracking().Where(a => a.Id == accountId).Select(a => new { a.Code, a.Name }).FirstOrDefaultAsync();
            var department = await _db.Departments.AsNoTracking().Where(d => d.Id == departmentId).Select(d => d.Name).FirstOrDefaultAsync();
            return Ok(new
            {
                department,
                account = account == null ? null : $"{account.Code} - {account.Name}",
                total = rows.Sum(r => r.amount),
                lines = rows.Select(r => new { r.date, r.reference, r.category, r.vendor, r.description, r.amount, drill = Doc("expense", r.expenseId) })
            });
        }

        // GET api/financeDrilldown/document?type=invoice&id=12
        // type: invoice | payment | expense | journal | sponsorPayment
        [HttpGet("document")]
        public async Task<IActionResult> Document(string type, int id)
        {
            object? doc = type switch
            {
                "invoice" => await InvoiceDoc(id),
                "payment" => await PaymentDoc(id),
                "expense" => await ExpenseDoc(id),
                "journal" => await JournalDoc(id),
                "sponsorPayment" => await SponsorPaymentDoc(id),
                _ => null
            };
            return doc == null ? NotFound(new { message = "Document not found." }) : Ok(doc);
        }

        // ---------- Documents, all in one shape: fields, then sections of rows ----------

        private static object Field(string label, object? value, object? drill = null) => new { label, value, drill };
        private static object Section(string title, string[] columns, IEnumerable<object> rows, object[]? footer = null) =>
            new { title, columns, rows = rows.ToList(), footer };
        private static object Row(object?[] cells, object? drill = null) => new { cells, drill };

        private async Task<object?> InvoiceDoc(int id)
        {
            var inv = await _db.StudentInvoices.AsNoTracking()
                .Include(i => i.Student).Include(i => i.AcademicYear).Include(i => i.Session)
                .Include(i => i.Items).ThenInclude(it => it.FeeCategory)
                .Include(i => i.Items).ThenInclude(it => it.Sponsorship).ThenInclude(s => s!.Sponsor)
                .Include(i => i.Payments)
                .FirstOrDefaultAsync(i => i.Id == id);
            if (inv == null) return null;
            var journal = await JournalFor($"INV-JNL-{inv.InvoiceNumber}");
            var balance = inv.TotalAmount - inv.PaidAmount - inv.DiscountAmount;
            return new
            {
                kind = "Invoice", reference = inv.InvoiceNumber, date = inv.InvoiceDate, status = Label(inv.Status),
                fields = new[]
                {
                    Field("Student", $"{inv.Student?.UPI} - {inv.Student?.FullName}"),
                    Field("Year / Term", $"{inv.AcademicYear?.Name} / {inv.Session?.SessionName}"),
                    Field("Due date", inv.DueDate),
                    Field("Total", inv.TotalAmount), Field("Paid", inv.PaidAmount), Field("Balance", balance),
                    Field("Description", inv.Description), Field("Recorded by", inv.CreatedBy),
                    Field("Accounting entry", journal?.ReferenceNumber, journal == null ? null : Doc("journal", journal.Id))
                },
                sections = new[]
                {
                    Section("Items", new[] { "Fee category", "Description", "Amount", "Discount", "Sponsor", "Paid" },
                        inv.Items.Select(it => Row(new object?[] { it.FeeCategory?.Name, it.Description, it.Amount, it.Discount,
                            it.Sponsorship?.Sponsor?.Name, it.PaidAmount }))),
                    Section("Payments", new[] { "Receipt", "Date", "Type", "Method", "Amount" },
                        inv.Payments.OrderBy(p => p.PaymentDate).Select(p => Row(new object?[] { p.ReceiptNumber, p.PaymentDate,
                            Label(p.PaymentType), Label(p.PaymentMethod), p.Amount }, Doc("payment", p.Id))))
                }
            };
        }

        private async Task<object?> PaymentDoc(int id)
        {
            var p = await _db.Payments.AsNoTracking()
                .Include(x => x.Student).Include(x => x.StudentInvoice).Include(x => x.BankAccount).Include(x => x.OriginalPayment)
                .Include(x => x.Allocations).ThenInclude(a => a.StudentInvoiceItem).ThenInclude(i => i!.FeeCategory)
                .FirstOrDefaultAsync(x => x.Id == id);
            if (p == null) return null;
            var prefix = p.PaymentType == PaymentType.Receipt ? "PAY-JNL-" : "CN-JNL-";
            var journal = await JournalFor(prefix + p.ReceiptNumber);
            var kind = p.PaymentType switch { PaymentType.CreditNote => "Credit note", PaymentType.DebitNote => "Debit note", _ => "Receipt" };
            return new
            {
                kind, reference = p.ReceiptNumber, date = p.PaymentDate, status = Label(p.ApprovalStatus),
                fields = new[]
                {
                    Field("Student", $"{p.Student?.UPI} - {p.Student?.FullName}"),
                    Field("Invoice", p.StudentInvoice?.InvoiceNumber, p.StudentInvoiceId == null ? null : Doc("invoice", p.StudentInvoiceId.Value)),
                    Field("Original receipt", p.OriginalPayment?.ReceiptNumber, p.OriginalPaymentId == null ? null : Doc("payment", p.OriginalPaymentId.Value)),
                    Field("Amount", p.Amount), Field("Method", Label(p.PaymentMethod)),
                    Field("Transaction ref", p.TransactionReference), Field("Received into", p.BankAccount?.Name),
                    Field("Description", p.Reason ?? p.Description), Field("Received by", p.CreatedBy),
                    Field("Accounting entry", journal?.ReferenceNumber, journal == null ? null : Doc("journal", journal.Id))
                },
                sections = new[]
                {
                    Section("Applied to", new[] { "Fee category", "Amount" },
                        p.Allocations.Select(a => Row(new object?[] { a.StudentInvoiceItem?.FeeCategory?.Name, a.Amount })))
                }
            };
        }

        private async Task<object?> ExpenseDoc(int id)
        {
            var e = await _db.Expenses.AsNoTracking()
                .Include(x => x.PaidFromAccount)
                .Include(x => x.Lines).ThenInclude(l => l.ExpenseCategory)
                .Include(x => x.Lines).ThenInclude(l => l.BudgetLine).ThenInclude(b => b!.Budget)
                .FirstOrDefaultAsync(x => x.Id == id);
            if (e == null) return null;
            var journal = await JournalFor($"EXP-JNL-{e.ReferenceNumber}");
            var approval = await _db.ApprovalRequests.AsNoTracking()
                .Include(r => r.Actions).ThenInclude(a => a.AssignedTo)
                .Include(r => r.Actions).ThenInclude(a => a.ActionedBy)
                .Include(r => r.SubmittedBy)
                .FirstOrDefaultAsync(r => r.EntityType == "Expense" && r.EntityId == id);
            var sections = new List<object>
            {
                Section("Lines", new[] { "Category", "Vendor", "Description", "Budget", "Amount" },
                    e.Lines.Select(l => Row(new object?[] { l.ExpenseCategory?.Name, l.Vendor, l.Description, l.BudgetLine?.Budget?.Name, l.Amount })),
                    new object?[] { "Total", null, null, null, e.Lines.Sum(l => l.Amount) })
            };
            if (approval != null)
                sections.Add(Section("Approval", new[] { "Step", "Approver", "Status", "Date", "Comment" },
                    approval.Actions.OrderBy(a => a.StepRank).Select(a => Row(new object?[] { a.StepName,
                        UserName(a.ActionedBy ?? a.AssignedTo), Label(a.Status), a.ActionedAt, a.Comment }))));
            return new
            {
                kind = "Expense", reference = e.ReferenceNumber, date = e.ExpenseDate, status = Label(e.Status),
                fields = new[]
                {
                    Field("Description", e.Description), Field("Method", Label(e.PaymentMethod)),
                    Field("Transaction ref", e.TransactionReference), Field("Paid from", e.PaidFromAccount?.Name),
                    Field("Recorded by", e.CreatedBy), Field("Submitted by", UserName(approval?.SubmittedBy)),
                    Field("Accounting entry", journal?.ReferenceNumber, journal == null ? null : Doc("journal", journal.Id))
                },
                sections
            };
        }

        private async Task<object?> SponsorPaymentDoc(int id)
        {
            var p = await _db.SponsorPayments.AsNoTracking().Include(x => x.Sponsor).Include(x => x.BankAccount).FirstOrDefaultAsync(x => x.Id == id);
            if (p == null) return null;
            var journal = await _db.JournalEntries.AsNoTracking().Include(j => j.Lines).ThenInclude(l => l.Account)
                .FirstOrDefaultAsync(j => j.ReferenceNumber == $"SP-JNL-{p.ReferenceNumber}");

            // A sponsor payment is not allocated to invoices, so show what the sponsor carries for the
            // academic year it was paid in: its share of each student's invoices that year.
            var year = await _db.AcademicYears.AsNoTracking()
                .FirstOrDefaultAsync(y => y.StartDate <= p.PaymentDate && y.EndDate >= p.PaymentDate);
            var yearId = year?.Id ?? 0;
            var covered = await _db.StudentInvoiceItems.AsNoTracking()
                .Where(i => i.Sponsorship!.SponsorId == p.SponsorId && i.Discount > 0 && i.StudentInvoice!.AcademicYearId == yearId)
                .GroupBy(i => new { i.StudentInvoice!.StudentId, i.StudentInvoice.Student!.UPI, i.StudentInvoice.Student.FullName })
                .Select(g => new { g.Key.StudentId, g.Key.UPI, g.Key.FullName, Invoices = g.Select(i => i.StudentInvoiceId).Distinct().Count(), Amount = g.Sum(i => i.Discount) })
                .OrderBy(g => g.FullName).ToListAsync();
            var sponsorPaid = year == null ? 0 : await _db.SponsorPayments.AsNoTracking()
                .Where(s => s.SponsorId == p.SponsorId && s.PaymentDate >= year.StartDate && s.PaymentDate <= year.EndDate).SumAsync(s => s.Amount);

            var sections = new List<object>();
            if (journal != null)
                sections.Add(Section("Accounting entry", new[] { "Account", "Description", "Debit", "Credit" },
                    journal.Lines.Select(l => Row(new object?[] { $"{l.Account?.Code} - {l.Account?.Name}", l.Description,
                        l.Debit == 0 ? null : l.Debit, l.Credit == 0 ? null : l.Credit }, Ledger(l.AccountId))),
                    new object?[] { "Total", null, journal.Lines.Sum(l => l.Debit), journal.Lines.Sum(l => l.Credit) }));
            if (year != null)
                sections.Add(Section($"Students covered by {p.Sponsor?.Name} in {year.Name}", new[] { "UPI", "Student", "Invoices", "Sponsor's share" },
                    // The count goes as text so it is not shown as money (3.00).
                    covered.Select(c => Row(new object?[] { c.UPI, c.FullName, c.Invoices.ToString(), c.Amount },
                        new { kind = "statement", studentId = c.StudentId, academicYearId = year.Id, label = year.Name })),
                    new object?[] { "Total", null, null, covered.Sum(c => c.Amount) }));

            return new
            {
                kind = "Sponsor payment", reference = p.ReferenceNumber, date = p.PaymentDate, status = (string?)null,
                fields = new[]
                {
                    Field("Sponsor", p.Sponsor?.Name), Field("Amount", p.Amount), Field("Method", Label(p.PaymentMethod)),
                    Field("Transaction ref", p.TransactionReference), Field("Received into", p.BankAccount?.Name),
                    Field("Description", p.Description), Field("Recorded by", p.CreatedBy),
                    Field("Accounting entry", journal?.ReferenceNumber, journal == null ? null : Doc("journal", journal.Id)),
                    Field($"Sponsor's share {year?.Name}", year == null ? null : covered.Sum(c => c.Amount)),
                    Field($"Paid by sponsor {year?.Name}", year == null ? null : sponsorPaid),
                    Field("Sponsor's balance", year == null ? null : covered.Sum(c => c.Amount) - sponsorPaid)
                },
                sections
            };
        }

        private async Task<object?> JournalDoc(int id)
        {
            var j = await _db.JournalEntries.AsNoTracking().Include(x => x.Lines).ThenInclude(l => l.Account).FirstOrDefaultAsync(x => x.Id == id);
            if (j == null) return null;
            var src = (await ResolveSources(new[] { j.ReferenceNumber })).GetValueOrDefault(j.ReferenceNumber);
            return new
            {
                kind = "Journal entry", reference = j.ReferenceNumber, date = j.EntryDate, status = Label(j.Status),
                fields = new[]
                {
                    Field("Description", j.Description), Field("Posted", j.IsPosted ? "Yes" : "No"), Field("Recorded by", j.CreatedBy),
                    Field("Source document", src == null ? null : $"{src.Reference}{(src.Party == null ? "" : " - " + src.Party)}",
                        src == null ? null : Doc(src.Type, src.Id))
                },
                sections = new[]
                {
                    Section("Lines", new[] { "Account", "Description", "Debit", "Credit" },
                        j.Lines.Select(l => Row(new object?[] { $"{l.Account?.Code} - {l.Account?.Name}", l.Description, l.Debit, l.Credit }, Ledger(l.AccountId))),
                        new object?[] { "Total", null, j.Lines.Sum(l => l.Debit), j.Lines.Sum(l => l.Credit) })
                }
            };
        }

        private Task<JournalEntry?> JournalFor(string reference) =>
            _db.JournalEntries.AsNoTracking().FirstOrDefaultAsync(j => j.ReferenceNumber == reference);

        private static string? UserName(Core.Entities.Identity.AppUser? u) =>
            u == null ? null : (($"{u.FirstName} {u.LastName}").Trim() is { Length: > 0 } n ? n : u.UserName);

        // ---------- Which document a journal entry was posted from ----------

        private record Source(string Type, int Id, string Reference, string? Party);

        // Auto-posted journals carry their document's number after a prefix: INV-JNL-<invoice>,
        // PAY-JNL- / CN-JNL-<receipt>, EXP-JNL-<expense>, SP-JNL-<sponsor payment>.
        private async Task<Dictionary<string, Source>> ResolveSources(IEnumerable<string> references)
        {
            var refs = references.Distinct().ToList();
            List<string> After(string prefix) => refs.Where(r => r.StartsWith(prefix)).Select(r => r[prefix.Length..]).ToList();
            var result = new Dictionary<string, Source>();

            var inv = After("INV-JNL-");
            if (inv.Count > 0)
                foreach (var i in await _db.StudentInvoices.AsNoTracking().Where(i => inv.Contains(i.InvoiceNumber))
                             .Select(i => new { i.Id, i.InvoiceNumber, i.Student!.FullName }).ToListAsync())
                    result[$"INV-JNL-{i.InvoiceNumber}"] = new Source("invoice", i.Id, i.InvoiceNumber, i.FullName);

            var receipts = After("PAY-JNL-").Concat(After("CN-JNL-")).ToList();
            if (receipts.Count > 0)
                foreach (var p in await _db.Payments.AsNoTracking().Where(p => receipts.Contains(p.ReceiptNumber))
                             .Select(p => new { p.Id, p.ReceiptNumber, p.PaymentType, p.Student!.FullName }).ToListAsync())
                    result[(p.PaymentType == PaymentType.Receipt ? "PAY-JNL-" : "CN-JNL-") + p.ReceiptNumber] =
                        new Source("payment", p.Id, p.ReceiptNumber, p.FullName);

            var exp = After("EXP-JNL-");
            if (exp.Count > 0)
                foreach (var e in await _db.Expenses.AsNoTracking().Where(e => exp.Contains(e.ReferenceNumber))
                             .Select(e => new { e.Id, e.ReferenceNumber, Vendor = e.Lines.Select(l => l.Vendor).FirstOrDefault(), e.Description }).ToListAsync())
                    result[$"EXP-JNL-{e.ReferenceNumber}"] = new Source("expense", e.Id, e.ReferenceNumber, e.Vendor ?? e.Description);

            var sp = After("SP-JNL-");
            if (sp.Count > 0)
                foreach (var s in await _db.SponsorPayments.AsNoTracking().Where(s => sp.Contains(s.ReferenceNumber))
                             .Select(s => new { s.Id, s.ReferenceNumber, s.Sponsor!.Name }).ToListAsync())
                    result[$"SP-JNL-{s.ReferenceNumber}"] = new Source("sponsorPayment", s.Id, s.ReferenceNumber, s.Name);

            return result;
        }
    }
}
