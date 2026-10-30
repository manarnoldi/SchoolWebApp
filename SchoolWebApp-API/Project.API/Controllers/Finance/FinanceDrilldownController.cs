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
            var sponsorShare = inv.Items.Where(it => it.SponsorshipId != null).Sum(it => it.Discount);
            return new
            {
                kind = "Invoice", reference = inv.InvoiceNumber, date = inv.InvoiceDate, status = Label(inv.Status),
                fields = new[]
                {
                    Field("Student", $"{inv.Student?.UPI} - {inv.Student?.FullName}"),
                    Field("Year / Term", $"{inv.AcademicYear?.Name} / {inv.Session?.SessionName}"),
                    Field("Due date", inv.DueDate),
                    Field("Sponsor's share", sponsorShare > 0 ? sponsorShare : null),
                    Field(sponsorShare > 0 ? "Total for the family" : "Total", inv.TotalAmount),
                    Field(sponsorShare > 0 ? "Paid by the family" : "Paid", inv.PaidAmount),
                    Field(sponsorShare > 0 ? "Family's balance" : "Balance", balance),
                    Field("Description", inv.Description), Field("Recorded by", inv.CreatedBy),
                    Field("Accounting entry", journal?.ReferenceNumber, journal == null ? null : Doc("journal", journal.Id))
                },
                sections = new[]
                {
                    // Who pays what: the sponsor (or a discount) takes part of each fee, the family the rest.
                    Section("Items", new[] { "Fee category", "Description", "Fee", "Sponsor / discount", "Sponsor", "Family pays", "Paid by family" },
                        inv.Items.Select(it => Row(new object?[] { it.FeeCategory?.Name, it.Description, it.Amount,
                            it.Discount == 0 ? null : it.Discount, it.Sponsorship?.Sponsor?.Name, it.Amount - it.Discount, it.PaidAmount })),
                        new object?[] { "Total", null, inv.Items.Sum(it => it.Amount), inv.Items.Sum(it => it.Discount), null,
                            inv.Items.Sum(it => it.Amount - it.Discount), inv.Items.Sum(it => it.PaidAmount) }),
                    Section("Payments", new[] { "Receipt", "Date", "Type", "Method", "Amount" },
                        inv.Payments.OrderBy(p => p.PaymentDate).Select(p => Row(new object?[] { p.ReceiptNumber, p.PaymentDate,
                            Label(p.PaymentType), Label(p.PaymentMethod), p.Amount }, Doc("payment", p.Id)))),
                    journal == null ? null : JournalSection(journal)
                }.Where(s => s != null)
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
            var inv = p.StudentInvoice;

            // Where the invoice stood once this receipt was in: everything received on it up to and
            // including this one. Only worked out when the invoice has nothing but receipts, since a
            // credit or debit note moves the figures in ways a running sum of receipts would not show.
            var fields = new List<object> { Field(p.PaymentType == PaymentType.Receipt ? "Amount received" : "Amount", p.Amount) };
            if (inv != null)
            {
                var onInvoice = await _db.Payments.AsNoTracking().Where(x => x.StudentInvoiceId == inv.Id)
                    .Select(x => new { x.Id, x.PaymentDate, x.PaymentType, x.Amount }).ToListAsync();
                var payable = inv.TotalAmount - inv.DiscountAmount;
                if (p.PaymentType == PaymentType.Receipt && onInvoice.All(x => x.PaymentType == PaymentType.Receipt))
                {
                    var before = onInvoice.Where(x => x.PaymentDate < p.PaymentDate || (x.PaymentDate == p.PaymentDate && x.Id < p.Id)).Sum(x => x.Amount);
                    fields.Add(Field("Invoice total", payable));
                    fields.Add(Field("Paid before this receipt", before));
                    fields.Add(Field("Invoice balance after it", payable - before - p.Amount));
                }
                else
                {
                    fields.Add(Field("Invoice total", payable));
                    fields.Add(Field("Paid on the invoice", inv.PaidAmount));
                    fields.Add(Field("Invoice balance today", payable - inv.PaidAmount));
                }
            }
            fields.AddRange(new[]
            {
                Field(p.PaymentType == PaymentType.Receipt ? "Received from" : "Student", $"{p.Student?.UPI} - {p.Student?.FullName}",
                    new { kind = "statement", studentId = p.StudentId, academicYearId = inv?.AcademicYearId }),
                Field("For invoice", inv?.InvoiceNumber, p.StudentInvoiceId == null ? null : Doc("invoice", p.StudentInvoiceId.Value)),
                Field("Original receipt", p.OriginalPayment?.ReceiptNumber, p.OriginalPaymentId == null ? null : Doc("payment", p.OriginalPaymentId.Value)),
                Field("Method", Label(p.PaymentMethod)), Field("Transaction ref", p.TransactionReference),
                Field("Paid into", p.BankAccount == null ? null : $"{p.BankAccount.Code} - {p.BankAccount.Name}",
                    p.BankAccountId == null ? null : Ledger(p.BankAccountId.Value)),
                Field("Description", p.Reason ?? p.Description), Field("Received by", p.CreatedBy)
            });

            var sections = new List<object>
            {
                Section(p.PaymentType == PaymentType.Receipt ? "What this receipt paid for" : "Applied to",
                    new[] { "Fee category", "Description", "Charged", "Paid by this receipt" },
                    p.Allocations.Select(a => Row(new object?[] { a.StudentInvoiceItem?.FeeCategory?.Name, a.StudentInvoiceItem?.Description,
                        a.StudentInvoiceItem == null ? null : a.StudentInvoiceItem.Amount - a.StudentInvoiceItem.Discount, a.Amount })),
                    new object?[] { "Total", null, null, p.Allocations.Sum(a => a.Amount) })
            };
            if (journal != null) sections.Add(JournalSection(journal));
            return new { kind, reference = p.ReceiptNumber, date = p.PaymentDate, status = Label(p.ApprovalStatus), fields, sections };
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
            if (journal != null) sections.Add(JournalSection(journal));
            return new
            {
                kind = "Expense", reference = e.ReferenceNumber, date = e.ExpenseDate, status = Label(e.Status),
                fields = new[]
                {
                    Field("Amount", e.Lines.Sum(l => l.Amount)),
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
            var journal = await JournalFor($"SP-JNL-{p.ReferenceNumber}");

            // A sponsor payment is recorded against the sponsor, not against invoices. To show what this one
            // paid for, the sponsor's payments are applied to its share of invoices oldest first, and this
            // payment's part of that is listed, invoice by invoice.
            var shares = await _db.StudentInvoiceItems.AsNoTracking()
                .Where(i => i.Sponsorship!.SponsorId == p.SponsorId && i.Discount > 0)
                .GroupBy(i => new { i.StudentInvoiceId, i.StudentInvoice!.InvoiceNumber, i.StudentInvoice.InvoiceDate,
                    i.StudentInvoice.Student!.UPI, i.StudentInvoice.Student.FullName, Term = i.StudentInvoice.Session!.SessionName })
                .Select(g => new { g.Key.StudentInvoiceId, g.Key.InvoiceNumber, g.Key.InvoiceDate, g.Key.UPI, g.Key.FullName, g.Key.Term, Share = g.Sum(i => i.Discount) })
                .ToListAsync();
            shares = shares.OrderBy(s => s.InvoiceDate).ThenBy(s => s.StudentInvoiceId).ToList();
            var paidBefore = await _db.SponsorPayments.AsNoTracking()
                .Where(s => s.SponsorId == p.SponsorId && (s.PaymentDate < p.PaymentDate || (s.PaymentDate == p.PaymentDate && s.Id < p.Id)))
                .SumAsync(s => s.Amount);

            // Earlier payments use up the oldest shares; this one carries on from where they stopped.
            var applied = new List<object>();
            decimal skip = paidBefore, left = p.Amount, appliedTotal = 0;
            foreach (var s in shares)
            {
                if (left <= 0) break;
                var open = s.Share;
                var used = Math.Min(skip, open);
                skip -= used; open -= used;
                if (open <= 0) continue;
                var take = Math.Min(left, open);
                left -= take; appliedTotal += take;
                applied.Add(Row(new object?[] { s.InvoiceDate, s.InvoiceNumber, $"{s.UPI} - {s.FullName}", s.Term, s.Share, take,
                    open - take == 0 ? null : open - take }, Doc("invoice", s.StudentInvoiceId)));
            }

            var sections = new List<object>
            {
                Section("Invoices this payment settles", new[] { "Invoice date", "Invoice", "Student", "Term", "Sponsor's share", "Paid by this payment", "Still due from sponsor" },
                    applied, new object?[] { "Total", null, null, null, null, appliedTotal, null })
            };
            if (journal != null) sections.Add(JournalSection(journal));

            return new
            {
                kind = "Sponsor payment", reference = p.ReferenceNumber, date = p.PaymentDate, status = (string?)null,
                note = "A sponsor payment is not tied to invoices when it is recorded, so the sponsor's payments are applied to its share of student invoices, oldest first. "
                    + "Each row is the part of this payment that went to that invoice; open a row to see the invoice and the sponsor's share on it.",
                fields = new[]
                {
                    Field("Amount", p.Amount),
                    Field("Applied to invoices", appliedTotal),
                    Field("Paid in advance", left > 0 ? left : null),
                    Field("Sponsor", p.Sponsor?.Name), Field("Method", Label(p.PaymentMethod)),
                    Field("Transaction ref", p.TransactionReference), Field("Received into", p.BankAccount?.Name),
                    Field("Description", p.Description), Field("Recorded by", p.CreatedBy)
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
            _db.JournalEntries.AsNoTracking().Include(j => j.Lines).ThenInclude(l => l.Account)
                .FirstOrDefaultAsync(j => j.ReferenceNumber == reference);

        // The double entry a document posted, each line opening its account's ledger.
        private static object JournalSection(JournalEntry j) =>
            Section($"Accounting entry {j.ReferenceNumber}", new[] { "Account", "Description", "Debit", "Credit" },
                j.Lines.Select(l => Row(new object?[] { $"{l.Account?.Code} - {l.Account?.Name}", l.Description,
                    l.Debit == 0 ? null : l.Debit, l.Credit == 0 ? null : l.Credit }, Ledger(l.AccountId))),
                new object?[] { "Total", null, j.Lines.Sum(l => l.Debit), j.Lines.Sum(l => l.Credit) });

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
