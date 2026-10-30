using Microsoft.EntityFrameworkCore;
using Project.Infrastructure.Data;
using SchoolWebApp.Core.Entities.Finance;
using SchoolWebApp.Core.Entities.Sponsorships;

namespace SchoolWebApp.API.Services
{
    /// <summary>
    /// Applies a sponsor's money to its students' invoices. A student is invoiced the whole fee and stays
    /// responsible for it; a sponsorship pledges part of each covered fee line (SponsoredAmount). When the
    /// sponsor pays, the money becomes receipts on each student's invoice, against the pledged lines
    /// (SponsorPaidAmount), oldest invoice first unless the bursar chose otherwise. Whatever is not applied is
    /// held as the sponsor's credit (in the Sponsor Advances account) and applied when more pledges fall due.
    /// </summary>
    public class SponsorAllocator
    {
        private readonly ApplicationDbContext _db;
        public SponsorAllocator(ApplicationDbContext db) { _db = db; }

        // A sponsor's pledge on one invoice that is still to be paid.
        public record OpenShare(int InvoiceId, string InvoiceNumber, DateTime InvoiceDate, int StudentId, string? Upi,
            string? StudentName, string? Term, decimal Pledged, decimal SponsorPaid, decimal Due);

        public record Allocation(int InvoiceId, decimal Amount);

        // What a line can still take from the sponsor: its pledge not yet paid, but never more than the line still owes
        // (the family may already have paid part of the sponsored fee).
        private static decimal LineDue(StudentInvoiceItem it) =>
            Math.Max(0, Math.Min(it.SponsoredAmount - it.SponsorPaidAmount, it.Amount - it.Discount - it.PaidAmount));

        /// <summary>The sponsor's pledges still to be paid, oldest invoice first.</summary>
        public async Task<List<OpenShare>> OpenShares(int sponsorId)
        {
            var invoices = await _db.StudentInvoices.AsNoTracking()
                .Include(i => i.Student).Include(i => i.Session)
                .Include(i => i.Items).ThenInclude(it => it.Sponsorship)
                .Where(i => i.Status != InvoiceStatus.Cancelled
                    && i.Items.Any(it => it.Sponsorship!.SponsorId == sponsorId && it.SponsoredAmount > it.SponsorPaidAmount))
                .OrderBy(i => i.InvoiceDate).ThenBy(i => i.Id)
                .ToListAsync();
            return invoices.Select(i =>
            {
                var lines = i.Items.Where(it => it.Sponsorship?.SponsorId == sponsorId).ToList();
                return new OpenShare(i.Id, i.InvoiceNumber, i.InvoiceDate, i.StudentId, i.Student?.UPI, i.Student?.FullName,
                    i.Session?.SessionName, lines.Sum(l => l.SponsoredAmount), lines.Sum(l => l.SponsorPaidAmount), lines.Sum(LineDue));
            }).Where(s => s.Due > 0).ToList();
        }

        /// <summary>An amount spread over the open pledges, oldest first.</summary>
        public static List<Allocation> Propose(IEnumerable<OpenShare> shares, decimal amount)
        {
            var result = new List<Allocation>();
            foreach (var s in shares)
            {
                if (amount <= 0) break;
                var take = Math.Min(amount, s.Due);
                result.Add(new Allocation(s.InvoiceId, take));
                amount -= take;
            }
            return result;
        }

        /// <summary>
        /// Puts part of a sponsor payment on students' invoices: one receipt per invoice, spread over that sponsor's
        /// pledged lines. Each amount must be within what the invoice still has pledged. Saves; returns the total
        /// applied. The caller posts the journal.
        /// </summary>
        public async Task<decimal> Apply(SponsorPayment payment, string sponsorName, IEnumerable<Allocation> allocations, DateTime date)
        {
            var wanted = allocations.Where(a => a.Amount > 0).GroupBy(a => a.InvoiceId)
                .ToDictionary(g => g.Key, g => g.Sum(a => a.Amount));
            if (wanted.Count == 0) return 0;

            var invoices = await _db.StudentInvoices
                .Include(i => i.Items).ThenInclude(it => it.Sponsorship)
                .Include(i => i.Items).ThenInclude(it => it.FeeCategory)
                .Where(i => wanted.Keys.Contains(i.Id))
                .ToListAsync();
            var seq = await _db.Payments.CountAsync(p => p.SponsorPaymentId == payment.Id);
            decimal applied = 0;

            foreach (var inv in invoices.OrderBy(i => i.InvoiceDate).ThenBy(i => i.Id))
            {
                var lines = inv.Items.Where(it => it.Sponsorship?.SponsorId == payment.SponsorId)
                    .OrderBy(it => it.FeeCategory?.Rank ?? 999).ThenBy(it => it.Id).ToList();
                var due = lines.Sum(LineDue);
                var amount = wanted[inv.Id];
                if (amount > due + 0.005m)
                    throw new InvalidOperationException($"{inv.InvoiceNumber} has only {due:N2} pledged by the sponsor still to pay, not {amount:N2}.");

                var receipt = new Payment
                {
                    ReceiptNumber = $"{payment.ReferenceNumber}-{++seq:00}",
                    PaymentType = PaymentType.Receipt,
                    ApprovalStatus = PaymentApprovalStatus.Approved,
                    StudentId = inv.StudentId,
                    StudentInvoiceId = inv.Id,
                    PaymentDate = date,
                    Amount = amount,
                    PaymentMethod = payment.PaymentMethod,
                    TransactionReference = payment.TransactionReference,
                    BankAccountId = payment.BankAccountId,
                    Description = $"Paid by {sponsorName} ({payment.ReferenceNumber})",
                    SponsorPaymentId = payment.Id
                };
                var left = amount;
                foreach (var it in lines)
                {
                    if (left <= 0) break;
                    var take = Math.Min(left, LineDue(it));
                    if (take <= 0) continue;
                    it.PaidAmount += take;
                    it.SponsorPaidAmount += take;
                    receipt.Allocations.Add(new PaymentAllocation { StudentInvoiceItemId = it.Id, Amount = take });
                    left -= take;
                }
                inv.PaidAmount += amount;
                inv.Status = Status(inv);
                _db.Payments.Add(receipt);
                applied += amount;
            }
            await _db.SaveChangesAsync();
            return applied;
        }

        public static InvoiceStatus Status(StudentInvoice inv)
        {
            var balance = inv.TotalAmount - inv.PaidAmount - inv.DiscountAmount;
            return balance <= 0 && inv.PaidAmount > 0 ? InvoiceStatus.Paid
                : inv.PaidAmount > 0 ? InvoiceStatus.PartiallyPaid : InvoiceStatus.Unpaid;
        }

        /// <summary>What of a sponsor payment is not yet on any invoice.</summary>
        public async Task<decimal> Held(SponsorPayment payment) =>
            payment.Amount - (await _db.Payments.Where(p => p.SponsorPaymentId == payment.Id).SumAsync(p => (decimal?)p.Amount) ?? 0);

        /// <summary>
        /// Applies a sponsor's held credit to its pledges now due, oldest payment and oldest invoice first, posting
        /// Debit Sponsor Advances / Credit Student Debtors for each payment drawn on. Returns the total applied.
        /// </summary>
        public async Task<decimal> ApplyHeldCredit(int sponsorId)
        {
            var advances = await SettingAccountId("SponsorAdvancesAccountId");
            var debtors = await SettingAccountId("DebtorsAccountId");
            if (advances == null || debtors == null) return 0;
            var sponsor = await _db.Sponsors.AsNoTracking().FirstOrDefaultAsync(s => s.Id == sponsorId);
            if (sponsor == null) return 0;

            decimal total = 0;
            var payments = await _db.SponsorPayments.Where(p => p.SponsorId == sponsorId)
                .OrderBy(p => p.PaymentDate).ThenBy(p => p.Id).ToListAsync();
            foreach (var p in payments)
            {
                var held = await Held(p);
                if (held <= 0) continue;
                var shares = await OpenShares(sponsorId);
                if (shares.Count == 0) break;
                var date = DateTime.Today;
                var applied = await Apply(p, sponsor.Name, Propose(shares, held), date);
                if (applied <= 0) continue;
                total += applied;
                _db.JournalEntries.Add(new JournalEntry
                {
                    ReferenceNumber = $"SPA-JNL-{p.ReferenceNumber}-{DateTime.Now:yyyyMMddHHmmss}",
                    EntryDate = date,
                    Description = $"Sponsor credit applied: {sponsor.Name} ({p.ReferenceNumber})",
                    IsPosted = true,
                    Status = JournalEntryStatus.Approved,
                    Lines = new List<JournalLine>
                    {
                        new JournalLine { AccountId = advances.Value, Debit = applied, Credit = 0, Description = $"Sponsor credit used - {sponsor.Name}" },
                        new JournalLine { AccountId = debtors.Value, Debit = 0, Credit = applied, Description = $"Student fees paid by {sponsor.Name}" }
                    }
                });
                await _db.SaveChangesAsync();
            }
            return total;
        }

        /// <summary>
        /// Takes a sponsor payment back off the invoices: its receipts are removed and the invoices and lines they
        /// paid are restored. Saves. Used when the sponsor payment itself is deleted.
        /// </summary>
        public async Task Unapply(SponsorPayment payment)
        {
            var receipts = await _db.Payments.Include(p => p.Allocations)
                .Where(p => p.SponsorPaymentId == payment.Id).ToListAsync();
            var invoiceIds = receipts.Where(r => r.StudentInvoiceId != null).Select(r => r.StudentInvoiceId!.Value).Distinct().ToList();
            var invoices = await _db.StudentInvoices.Include(i => i.Items).Where(i => invoiceIds.Contains(i.Id)).ToListAsync();
            foreach (var r in receipts)
            {
                var inv = invoices.FirstOrDefault(i => i.Id == r.StudentInvoiceId);
                foreach (var a in r.Allocations)
                {
                    var it = inv?.Items.FirstOrDefault(x => x.Id == a.StudentInvoiceItemId);
                    if (it == null) continue;
                    it.PaidAmount -= a.Amount;
                    it.SponsorPaidAmount -= a.Amount;
                }
                if (inv != null) { inv.PaidAmount -= r.Amount; inv.Status = Status(inv); }
                _db.PaymentAllocations.RemoveRange(r.Allocations);
                _db.Payments.Remove(r);
            }
            await _db.SaveChangesAsync();
        }

        public async Task<int?> SettingAccountId(string key)
        {
            var s = await _db.GlobalSettings.AsNoTracking().FirstOrDefaultAsync(x => x.Module == "Finance" && x.SettingKey == key);
            return int.TryParse(s?.SettingValue, out var id) ? id : null;
        }
    }
}
