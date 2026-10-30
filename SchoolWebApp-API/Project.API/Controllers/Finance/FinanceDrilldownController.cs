using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Project.Infrastructure.Data;
using SchoolWebApp.Core.Entities.Finance;
using SchoolWebApp.Core.Entities.Sponsorships;
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
                "sponsor" => await SponsorDoc(id),
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
                .Include(i => i.Payments).ThenInclude(p => p.SponsorPayment).ThenInclude(sp => sp!.Sponsor)
                .FirstOrDefaultAsync(i => i.Id == id);
            if (inv == null) return null;
            var journal = await JournalFor($"INV-JNL-{inv.InvoiceNumber}");
            var balance = inv.TotalAmount - inv.PaidAmount - inv.DiscountAmount;
            var pledged = inv.Items.Sum(it => it.SponsoredAmount);
            var bySponsor = inv.Items.Sum(it => it.SponsorPaidAmount);
            var awaiting = inv.Items.Sum(it => Math.Max(0, Math.Min(it.SponsoredAmount - it.SponsorPaidAmount, it.Amount - it.Discount - it.PaidAmount)));
            var sponsors = string.Join(", ", inv.Items.Where(it => it.SponsoredAmount > 0).Select(it => it.Sponsorship?.Sponsor?.Name).Where(n => n != null).Distinct());
            // The student is invoiced the whole fee and owes all of it; a sponsor's pledge says who is expected to pay
            // part of it. The tiles split what has come in by who paid it.
            return new
            {
                kind = "Invoice", reference = inv.InvoiceNumber, date = inv.InvoiceDate, status = Label(inv.Status),
                note = pledged > 0
                    ? $"{sponsors} pledged {pledged:N2} of this invoice. The student stays responsible for the whole of it: what a sponsor does not pay is still owed by the family."
                    : null,
                fields = new[]
                {
                    Field("Invoiced", inv.TotalAmount - inv.DiscountAmount),
                    Field("Pledged by sponsor", pledged > 0 ? pledged : null),
                    Field("Paid by sponsor", pledged > 0 ? bySponsor : null),
                    Field(pledged > 0 ? "Paid by family" : "Paid", inv.PaidAmount - bySponsor),
                    Field("Balance", balance),
                    Field("Student", $"{inv.Student?.UPI} - {inv.Student?.FullName}",
                        new { kind = "statement", studentId = inv.StudentId, academicYearId = inv.AcademicYearId }),
                    Field("Year / Term", $"{inv.AcademicYear?.Name} / {inv.Session?.SessionName}"),
                    Field("Due date", inv.DueDate),
                    Field("Awaiting sponsor", awaiting > 0 ? $"{awaiting:N2} of the balance" : null),
                    Field("Description", inv.Description), Field("Recorded by", inv.CreatedBy),
                    Field("Accounting entry", journal?.ReferenceNumber, journal == null ? null : Doc("journal", journal.Id))
                },
                sections = new[]
                {
                    // Each fee: any discount, what a sponsor pledged and has paid, what the family has paid, and what is left.
                    Section("Items", new[] { "Fee", "Amount", "Discount", "Sponsor", "Pledged", "Paid by sponsor", "Paid by family", "Balance" },
                        inv.Items.Select(it => Row(new object?[] { it.FeeCategory?.Name, it.Amount, it.Discount == 0 ? null : it.Discount,
                            it.SponsoredAmount > 0 ? it.Sponsorship?.Sponsor?.Name : null, it.SponsoredAmount == 0 ? null : it.SponsoredAmount,
                            it.SponsoredAmount == 0 ? null : it.SponsorPaidAmount, it.PaidAmount - it.SponsorPaidAmount,
                            it.Amount - it.Discount - it.PaidAmount })),
                        new object?[] { "Total", inv.Items.Sum(it => it.Amount), inv.Items.Sum(it => it.Discount), null, pledged, bySponsor,
                            inv.Items.Sum(it => it.PaidAmount - it.SponsorPaidAmount), inv.Items.Sum(it => it.Amount - it.Discount - it.PaidAmount) }),
                    Section("Payments", new[] { "Receipt", "Date", "From", "Type", "Method", "Amount" },
                        inv.Payments.OrderBy(p => p.PaymentDate).Select(p => Row(new object?[] { p.ReceiptNumber, p.PaymentDate,
                            p.SponsorPayment?.Sponsor?.Name ?? "Family", Label(p.PaymentType), Label(p.PaymentMethod), p.Amount }, Doc("payment", p.Id)))),
                    journal == null ? null : JournalSection(journal)
                }.Where(x => x != null)
            };
        }

        private async Task<object?> PaymentDoc(int id)
        {
            var p = await _db.Payments.AsNoTracking()
                .Include(x => x.Student).Include(x => x.StudentInvoice).Include(x => x.BankAccount).Include(x => x.OriginalPayment)
                .Include(x => x.Allocations).ThenInclude(a => a.StudentInvoiceItem).ThenInclude(i => i!.FeeCategory)
                .Include(x => x.SponsorPayment).ThenInclude(sp => sp!.Sponsor)
                .FirstOrDefaultAsync(x => x.Id == id);
            if (p == null) return null;
            // A sponsor's money on the invoice is posted by the sponsor payment's journal, not one of its own.
            var sponsorPay = p.SponsorPayment;
            var prefix = p.PaymentType == PaymentType.Receipt ? "PAY-JNL-" : "CN-JNL-";
            var journal = sponsorPay != null ? await JournalFor($"SP-JNL-{sponsorPay.ReferenceNumber}") : await JournalFor(prefix + p.ReceiptNumber);
            var kind = p.PaymentType switch { PaymentType.CreditNote => "Credit note", PaymentType.DebitNote => "Debit note", _ => sponsorPay != null ? "Sponsor receipt" : "Receipt" };
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
                Field("Paid by", sponsorPay?.Sponsor?.Name, sponsorPay == null ? null : Doc("sponsorPayment", sponsorPay.Id)),
                Field("Sponsor payment", sponsorPay?.ReferenceNumber, sponsorPay == null ? null : Doc("sponsorPayment", sponsorPay.Id)),
                Field(p.PaymentType == PaymentType.Receipt && sponsorPay == null ? "Received from" : "Student", $"{p.Student?.UPI} - {p.Student?.FullName}",
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

            // Where the money went: a receipt on each student's invoice, against the fees the sponsor pledged.
            var receipts = await _db.Payments.AsNoTracking()
                .Include(r => r.Student).Include(r => r.StudentInvoice).ThenInclude(i => i!.Session)
                .Where(r => r.SponsorPaymentId == p.Id)
                .OrderBy(r => r.StudentInvoice!.InvoiceDate).ThenBy(r => r.Id)
                .ToListAsync();
            var applied = receipts.Sum(r => r.Amount);
            var held = p.Amount - applied;
            var later = await _db.JournalEntries.AsNoTracking().Include(j => j.Lines).ThenInclude(l => l.Account)
                .Where(j => j.ReferenceNumber.StartsWith($"SPA-JNL-{p.ReferenceNumber}-")).ToListAsync();

            var sections = new List<object>
            {
                Section("Applied to students' invoices", new[] { "Date", "Receipt", "Student", "Invoice", "Term", "Amount" },
                    receipts.Select(r => Row(new object?[] { r.PaymentDate, r.ReceiptNumber, $"{r.Student?.UPI} - {r.Student?.FullName}",
                        r.StudentInvoice?.InvoiceNumber, r.StudentInvoice?.Session?.SessionName, r.Amount }, Doc("payment", r.Id))),
                    new object?[] { "Total", null, null, null, null, applied })
            };
            if (journal != null) sections.Add(JournalSection(journal));
            foreach (var j in later) sections.Add(JournalSection(j));

            return new
            {
                kind = "Sponsor payment", reference = p.ReferenceNumber, date = p.PaymentDate, status = (string?)null,
                note = held > 0
                    ? $"{held:N2} of this payment is held as {p.Sponsor?.Name}'s credit and goes onto its students' invoices as the sponsor's next pledges fall due."
                    : "All of this payment is on the students' invoices. Open a row for the receipt on the student.",
                fields = new[]
                {
                    Field("Amount", p.Amount),
                    Field("Applied to students", applied),
                    Field("Held as credit", held > 0 ? held : null),
                    Field("Sponsor", p.Sponsor?.Name, Doc("sponsor", p.SponsorId)),
                    Field("Students", receipts.Select(r => r.StudentId).Distinct().Count().ToString()),
                    Field("Method", Label(p.PaymentMethod)),
                    Field("Transaction ref", p.TransactionReference), Field("Received into", p.BankAccount?.Name),
                    Field("Description", p.Description), Field("Recorded by", p.CreatedBy)
                },
                sections
            };
        }

        // A sponsor's account: what it pledged on its students' invoices, what it has paid onto them, the credit it
        // holds, its payments, and its students one by one.
        private async Task<object?> SponsorDoc(int id)
        {
            var sp = await _db.Sponsors.AsNoTracking().FirstOrDefaultAsync(s => s.Id == id);
            if (sp == null) return null;
            var pledges = await _db.StudentInvoiceItems.AsNoTracking()
                .Where(i => i.Sponsorship!.SponsorId == id && i.SponsoredAmount > 0)
                .Select(i => new { i.StudentInvoice!.StudentId, i.StudentInvoice.Student!.UPI, i.StudentInvoice.Student.FullName,
                    i.StudentInvoice.AcademicYearId, i.SponsoredAmount, i.SponsorPaidAmount,
                    Open = i.Amount - i.Discount - i.PaidAmount })
                .ToListAsync();
            var payments = await _db.SponsorPayments.AsNoTracking().Where(p => p.SponsorId == id).OrderBy(p => p.PaymentDate).ToListAsync();
            var appliedBy = await _db.Payments.AsNoTracking().Where(r => r.SponsorPayment != null && r.SponsorPayment.SponsorId == id)
                .GroupBy(r => r.SponsorPaymentId!.Value).Select(g => new { Id = g.Key, Amount = g.Sum(r => r.Amount) })
                .ToDictionaryAsync(a => a.Id, a => a.Amount);

            var pledged = pledges.Sum(x => x.SponsoredAmount);
            var paidOn = pledges.Sum(x => x.SponsorPaidAmount);
            var received = payments.Sum(p => p.Amount);
            var held = received - appliedBy.Values.Sum();
            var outstanding = pledges.Sum(x => Math.Max(0, Math.Min(x.SponsoredAmount - x.SponsorPaidAmount, x.Open)));

            var sponsorships = await _db.Sponsorships.AsNoTracking().Where(s => s.SponsorId == id)
                .Select(s => new { s.StudentId, Upi = s.Student!.UPI, Student = s.Student.FullName, Class = s.SchoolClass!.Name,
                    Year = s.AcademicYear!.Name, s.AcademicYearId, Term = s.Session!.SessionName, s.CoverageType, s.Percentage, s.FixedAmount, s.Status,
                    Fees = s.FeeCategories.Select(f => f.FeeCategory!.Name).ToList() })
                .ToListAsync();
            // What a sponsorship pays, in words: how much of which fees.
            string Covers(SponsorshipCoverageType type, decimal pct, decimal fixedAmount, List<string> fees)
            {
                var what = fees.Count == 0 ? "all fees" : string.Join(", ", fees);
                return type == SponsorshipCoverageType.FixedAmount ? $"Up to {fixedAmount:N2} per invoice of {what}"
                    : type == SponsorshipCoverageType.FullCoverage || pct >= 100 ? $"All of {what}" : $"{pct:0.##}% of {what}";
            }

            return new
            {
                kind = "Sponsor", reference = sp.Name, date = (DateTime?)null, status = sp.IsActive ? "Active" : "Inactive",
                note = "Pledged is the part of its students' fees the sponsor has promised. Each payment goes onto those students' invoices; what is not yet needed is held as credit. "
                    + "Students stay responsible for any pledge the sponsor does not pay.",
                fields = new[]
                {
                    Field("Pledged", pledged),
                    Field("Paid onto invoices", paidOn),
                    Field("Pledges outstanding", outstanding),
                    Field("Held as credit", held > 0 ? held : null),
                    Field("Received in all", received.ToString("N2")),
                    Field("Type", Label(sp.SponsorType)), Field("Contact", sp.ContactName), Field("Email", sp.Email), Field("Phone", sp.Phone)
                },
                sections = new[]
                {
                    Section("Payments received", new[] { "Date", "Reference", "Description", "Amount", "On invoices", "Held" },
                        payments.Select(p => Row(new object?[] { p.PaymentDate, p.ReferenceNumber, p.Description, p.Amount,
                            appliedBy.GetValueOrDefault(p.Id), p.Amount - appliedBy.GetValueOrDefault(p.Id) is var h && h > 0 ? h : null },
                            Doc("sponsorPayment", p.Id))),
                        new object?[] { "Total", null, null, received, appliedBy.Values.Sum(), held > 0 ? held : null }),
                    Section("Students", new[] { "Student", "Pledged", "Paid by sponsor", "Outstanding" },
                        pledges.GroupBy(x => new { x.StudentId, x.UPI, x.FullName }).OrderBy(g => g.Key.FullName)
                            .Select(g => Row(new object?[] { $"{g.Key.UPI} - {g.Key.FullName}", g.Sum(x => x.SponsoredAmount), g.Sum(x => x.SponsorPaidAmount),
                                g.Sum(x => Math.Max(0, Math.Min(x.SponsoredAmount - x.SponsorPaidAmount, x.Open))) },
                                new { kind = "statement", studentId = g.Key.StudentId, academicYearId = g.First().AcademicYearId }))),
                    Section("Sponsorships", new[] { "Beneficiary", "Year", "Term", "Covers", "Status" },
                        sponsorships.OrderBy(s => s.Student ?? s.Class).Select(s => Row(new object?[] {
                            s.StudentId == null ? $"Class {s.Class}" : $"{s.Upi} - {s.Student}", s.Year, s.Term ?? "All terms",
                            Covers(s.CoverageType, s.Percentage, s.FixedAmount, s.Fees), Label(s.Status) })))
                }
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

            // SPA-JNL-<sponsor payment>-<time>: that payment's held credit applied to invoices later.
            var spa = refs.Where(r => r.StartsWith("SPA-JNL-") && r.LastIndexOf('-') > "SPA-JNL-".Length)
                .ToDictionary(r => r, r => r["SPA-JNL-".Length..r.LastIndexOf('-')]);
            if (spa.Count > 0)
            {
                var spaRefs = spa.Values.Distinct().ToList();
                var found = await _db.SponsorPayments.AsNoTracking().Where(s => spaRefs.Contains(s.ReferenceNumber))
                    .Select(s => new { s.Id, s.ReferenceNumber, s.Sponsor!.Name }).ToListAsync();
                foreach (var kv in spa)
                    if (found.FirstOrDefault(s => s.ReferenceNumber == kv.Value) is { } s2)
                        result[kv.Key] = new Source("sponsorPayment", s2.Id, s2.ReferenceNumber, s2.Name);
            }

            return result;
        }
    }
}
