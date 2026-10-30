using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Project.Infrastructure.Data;
using SchoolWebApp.Core.DTOs.Sponsorships;
using SchoolWebApp.Core.Entities.Finance;
using SchoolWebApp.Core.Entities.Sponsorships;
using SchoolWebApp.API.Licensing;
using SchoolWebApp.API.Security;
using SchoolWebApp.API.Services;

namespace SchoolWebApp.API.Controllers.Sponsorships
{
    /// <summary>
    /// Money received from sponsors. Each payment is applied to the sponsored part of its students' invoices
    /// (one receipt per invoice, on the student); what is left is held as the sponsor's credit until more
    /// pledges fall due. Journal: Debit Bank, Credit Student Debtors (applied) and Sponsor Advances (held).
    /// </summary>
    [Authorize]
    [Route("api/sponsorPayments")]
    [ApiController]
    [RequiresModule(AppModules.Finance)]
    [RequiresMenu(MenuGroup.Finance)]
    public class SponsorPaymentsController : ControllerBase
    {
        private readonly ApplicationDbContext _db;
        private readonly IMapper _mapper;
        private readonly SponsorAllocator _allocator;
        public SponsorPaymentsController(ApplicationDbContext db, IMapper mapper, SponsorAllocator allocator)
        {
            _db = db; _mapper = mapper; _allocator = allocator;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var list = await _db.SponsorPayments
                .Include(p => p.Sponsor)
                .Include(p => p.BankAccount)
                .OrderByDescending(p => p.PaymentDate)
                .ToListAsync();
            var applied = await _db.Payments.Where(p => p.SponsorPaymentId != null)
                .GroupBy(p => p.SponsorPaymentId!.Value)
                .Select(g => new { Id = g.Key, Amount = g.Sum(p => p.Amount), Invoices = g.Select(p => p.StudentInvoiceId).Distinct().Count() })
                .ToDictionaryAsync(a => a.Id);
            var dtos = _mapper.Map<List<SponsorPaymentDto>>(list);
            foreach (var d in dtos)
            {
                if (applied.TryGetValue(d.Id, out var a)) { d.Applied = a.Amount; d.InvoiceCount = a.Invoices; }
                d.Held = d.Amount - d.Applied;
            }
            return Ok(dtos);
        }

        // GET api/sponsorPayments/proposal?sponsorId=1&amount=118000
        // The sponsor's pledges still to be paid, oldest first, with the amount spread over them - for the bursar
        // to review (and change) before saving the payment.
        [HttpGet("proposal")]
        public async Task<IActionResult> Proposal(int sponsorId, decimal amount)
        {
            var shares = await _allocator.OpenShares(sponsorId);
            var proposed = SponsorAllocator.Propose(shares, amount).ToDictionary(a => a.InvoiceId, a => a.Amount);
            var held = await HeldCredit(sponsorId);
            return Ok(new
            {
                totalDue = shares.Sum(s => s.Due),
                heldCredit = held,
                lines = shares.Select(s => new
                {
                    studentInvoiceId = s.InvoiceId, invoiceNumber = s.InvoiceNumber, invoiceDate = s.InvoiceDate,
                    studentId = s.StudentId, studentUPI = s.Upi, studentName = s.StudentName, term = s.Term,
                    pledged = s.Pledged, sponsorPaid = s.SponsorPaid, due = s.Due,
                    amount = proposed.GetValueOrDefault(s.InvoiceId)
                })
            });
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateSponsorPaymentDto model)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var sponsor = await _db.Sponsors.FirstOrDefaultAsync(s => s.Id == model.SponsorId);
            if (sponsor == null) return BadRequest(new { message = "Sponsor not found." });
            if (model.Amount <= 0) return BadRequest(new { message = "Amount must be positive." });

            var allocations = model.Allocations != null
                ? model.Allocations.Where(a => a.Amount > 0).Select(a => new SponsorAllocator.Allocation(a.StudentInvoiceId, a.Amount)).ToList()
                : SponsorAllocator.Propose(await _allocator.OpenShares(model.SponsorId), model.Amount);
            var toApply = allocations.Sum(a => a.Amount);
            if (toApply > model.Amount + 0.005m)
                return BadRequest(new { message = $"The amounts given to students ({toApply:N2}) are more than the payment ({model.Amount:N2})." });

            int? bank = model.BankAccountId ?? await _allocator.SettingAccountId("CashAccountId");
            int? debtors = await _allocator.SettingAccountId("DebtorsAccountId");
            int? advances = await _allocator.SettingAccountId("SponsorAdvancesAccountId");
            if (bank == null || debtors == null)
                return BadRequest(new { message = "Set the cash/bank and student debtors accounts under Settings > Global Settings > Finance first." });
            if (model.Amount - toApply > 0.005m && advances == null)
                return BadRequest(new { message = "Part of this payment would be held as the sponsor's credit. Set the Sponsor Advances account under Settings > Global Settings > Finance first, or give all of it to students." });

            var payment = new SponsorPayment
            {
                SponsorId = model.SponsorId,
                ReferenceNumber = string.IsNullOrWhiteSpace(model.ReferenceNumber) ? $"SP-{DateTime.Now:yyyyMMddHHmmss}" : model.ReferenceNumber,
                PaymentDate = model.PaymentDate,
                Amount = model.Amount,
                PaymentMethod = model.PaymentMethod,
                TransactionReference = model.TransactionReference,
                BankAccountId = model.BankAccountId,
                Description = model.Description
            };

            await using var tx = await _db.Database.BeginTransactionAsync();
            _db.SponsorPayments.Add(payment);
            await _db.SaveChangesAsync();
            decimal applied;
            try { applied = await _allocator.Apply(payment, sponsor.Name, allocations, payment.PaymentDate); }
            catch (InvalidOperationException e) { return BadRequest(new { message = e.Message }); }

            var held = payment.Amount - applied;
            var lines = new List<JournalLine>
            {
                new JournalLine { AccountId = bank.Value, Debit = payment.Amount, Credit = 0, Description = $"Sponsor payment received - {sponsor.Name}" }
            };
            if (applied > 0)
                lines.Add(new JournalLine { AccountId = debtors.Value, Debit = 0, Credit = applied, Description = $"Student fees paid by {sponsor.Name}" });
            if (held > 0)
                lines.Add(new JournalLine { AccountId = advances!.Value, Debit = 0, Credit = held, Description = $"Held as {sponsor.Name}'s credit" });
            _db.JournalEntries.Add(new JournalEntry
            {
                ReferenceNumber = $"SP-JNL-{payment.ReferenceNumber}",
                EntryDate = payment.PaymentDate,
                Description = $"Sponsor payment received: {sponsor.Name} ({payment.ReferenceNumber})",
                IsPosted = true,
                Status = JournalEntryStatus.Approved,
                Lines = lines
            });
            await _db.SaveChangesAsync();
            await tx.CommitAsync();

            return Ok(new { id = payment.Id, applied, held });
        }

        // POST api/sponsorPayments/applyCredit?sponsorId=1 - puts the sponsor's held credit on its pledges now due.
        [HttpPost("applyCredit")]
        public async Task<IActionResult> ApplyCredit(int sponsorId)
        {
            if (await _allocator.SettingAccountId("SponsorAdvancesAccountId") == null)
                return BadRequest(new { message = "Set the Sponsor Advances account under Settings > Global Settings > Finance first." });
            var applied = await _allocator.ApplyHeldCredit(sponsorId);
            return Ok(new { applied, heldCredit = await HeldCredit(sponsorId) });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var item = await _db.SponsorPayments.FirstOrDefaultAsync(p => p.Id == id);
            if (item == null) return NotFound();
            if (await _db.Payments.AnyAsync(p => p.OriginalPayment != null && p.OriginalPayment.SponsorPaymentId == id))
                return BadRequest(new { message = "A credit or debit note has been raised against one of this payment's receipts; deal with it first." });

            await using var tx = await _db.Database.BeginTransactionAsync();
            // Off the students' invoices first, then its journals: the receipt journal and any for credit applied later.
            await _allocator.Unapply(item);
            var refs = new[] { $"SP-JNL-{item.ReferenceNumber}" };
            var journals = await _db.JournalEntries.Include(j => j.Lines)
                .Where(j => refs.Contains(j.ReferenceNumber) || j.ReferenceNumber.StartsWith($"SPA-JNL-{item.ReferenceNumber}-"))
                .ToListAsync();
            foreach (var j in journals) { _db.JournalLines.RemoveRange(j.Lines); _db.JournalEntries.Remove(j); }
            _db.SponsorPayments.Remove(item);
            await _db.SaveChangesAsync();
            await tx.CommitAsync();
            return Ok();
        }

        private async Task<decimal> HeldCredit(int sponsorId)
        {
            var paid = await _db.SponsorPayments.Where(p => p.SponsorId == sponsorId).SumAsync(p => (decimal?)p.Amount) ?? 0;
            var applied = await _db.Payments.Where(p => p.SponsorPayment != null && p.SponsorPayment.SponsorId == sponsorId)
                .SumAsync(p => (decimal?)p.Amount) ?? 0;
            return paid - applied;
        }
    }
}
