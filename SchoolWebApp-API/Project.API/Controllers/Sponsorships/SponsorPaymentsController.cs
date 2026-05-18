using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Project.Infrastructure.Data;
using SchoolWebApp.Core.DTOs.Sponsorships;
using SchoolWebApp.Core.Entities.Finance;
using SchoolWebApp.Core.Entities.Settings;
using SchoolWebApp.Core.Entities.Sponsorships;

namespace SchoolWebApp.API.Controllers.Sponsorships
{
    [Authorize]
    [Route("api/sponsorPayments")]
    [ApiController]
    public class SponsorPaymentsController : ControllerBase
    {
        private readonly ApplicationDbContext _db;
        private readonly IMapper _mapper;
        public SponsorPaymentsController(ApplicationDbContext db, IMapper mapper) { _db = db; _mapper = mapper; }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var list = await _db.SponsorPayments
                .Include(p => p.Sponsor)
                .Include(p => p.BankAccount)
                .OrderByDescending(p => p.PaymentDate)
                .ToListAsync();
            return Ok(_mapper.Map<List<SponsorPaymentDto>>(list));
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateSponsorPaymentDto model)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var sponsor = await _db.Sponsors.FirstOrDefaultAsync(s => s.Id == model.SponsorId);
            if (sponsor == null) return BadRequest(new { message = "Sponsor not found." });
            if (model.Amount <= 0) return BadRequest(new { message = "Amount must be positive." });

            var refNumber = string.IsNullOrWhiteSpace(model.ReferenceNumber)
                ? $"SP-{DateTime.Now:yyyyMMddHHmmss}"
                : model.ReferenceNumber;

            var item = new SponsorPayment
            {
                SponsorId = model.SponsorId,
                ReferenceNumber = refNumber,
                PaymentDate = model.PaymentDate,
                Amount = model.Amount,
                PaymentMethod = model.PaymentMethod,
                TransactionReference = model.TransactionReference,
                BankAccountId = model.BankAccountId,
                Description = model.Description
            };
            _db.SponsorPayments.Add(item);
            await _db.SaveChangesAsync();

            // Post journal: Debit Bank/Cash, Credit Sponsor Receivable
            await PostSponsorPaymentJournal(item, sponsor);

            return Ok(new { id = item.Id });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var item = await _db.SponsorPayments.FirstOrDefaultAsync(p => p.Id == id);
            if (item == null) return NotFound();
            // Also remove the auto-posted journal
            var autoRef = $"SP-JNL-{item.ReferenceNumber}";
            var journal = await _db.JournalEntries.Include(j => j.Lines).FirstOrDefaultAsync(j => j.ReferenceNumber == autoRef);
            if (journal != null)
            {
                _db.JournalLines.RemoveRange(journal.Lines);
                _db.JournalEntries.Remove(journal);
            }
            _db.SponsorPayments.Remove(item);
            await _db.SaveChangesAsync();
            return Ok();
        }

        private async Task<int?> GetSettingAccountId(string key)
        {
            var s = await _db.GlobalSettings.FirstOrDefaultAsync(x => x.Module == "Finance" && x.SettingKey == key);
            return int.TryParse(s?.SettingValue, out var id) ? id : (int?)null;
        }

        private async Task PostSponsorPaymentJournal(SponsorPayment payment, Sponsor sponsor)
        {
            int? bankAccountId = payment.BankAccountId ?? await GetSettingAccountId("CashAccountId");
            if (bankAccountId == null) return;
            // If the sponsor has a dedicated AR account, use it; otherwise fall back to the general debtors account.
            int? receivableAccountId = sponsor.ReceivableAccountId ?? await GetSettingAccountId("DebtorsAccountId");
            if (receivableAccountId == null) return;

            var journal = new JournalEntry
            {
                ReferenceNumber = $"SP-JNL-{payment.ReferenceNumber}",
                EntryDate = payment.PaymentDate,
                Description = $"Sponsor payment received: {sponsor.Name} ({payment.ReferenceNumber})",
                IsPosted = true,
                Status = JournalEntryStatus.Approved,
                Lines = new List<JournalLine>
                {
                    new JournalLine { AccountId = bankAccountId.Value, Debit = payment.Amount, Credit = 0,
                        Description = $"Sponsor payment received - {sponsor.Name}" },
                    new JournalLine { AccountId = receivableAccountId.Value, Debit = 0, Credit = payment.Amount,
                        Description = $"Clear sponsor receivable - {sponsor.Name}" }
                }
            };
            _db.JournalEntries.Add(journal);
            await _db.SaveChangesAsync();
        }
    }
}
