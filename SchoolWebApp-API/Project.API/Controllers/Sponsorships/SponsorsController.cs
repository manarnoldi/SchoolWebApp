using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Project.Infrastructure.Data;
using SchoolWebApp.Core.DTOs.Sponsorships;
using SchoolWebApp.Core.Entities.Sponsorships;

namespace SchoolWebApp.API.Controllers.Sponsorships
{
    [Authorize]
    [Route("api/sponsors")]
    [ApiController]
    public class SponsorsController : ControllerBase
    {
        private readonly ApplicationDbContext _db;
        private readonly IMapper _mapper;
        public SponsorsController(ApplicationDbContext db, IMapper mapper) { _db = db; _mapper = mapper; }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var list = await _db.Sponsors
                .Include(s => s.ReceivableAccount)
                .OrderBy(s => s.Name)
                .ToListAsync();
            return Ok(_mapper.Map<List<SponsorDto>>(list));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var item = await _db.Sponsors.Include(s => s.ReceivableAccount).FirstOrDefaultAsync(s => s.Id == id);
            if (item == null) return NotFound();
            return Ok(_mapper.Map<SponsorDto>(item));
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateSponsorDto model)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var item = _mapper.Map<Sponsor>(model);
            _db.Sponsors.Add(item);
            await _db.SaveChangesAsync();
            return Ok(_mapper.Map<SponsorDto>(item));
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] CreateSponsorDto model)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var item = await _db.Sponsors.FirstOrDefaultAsync(s => s.Id == id);
            if (item == null) return NotFound();
            item.Name = model.Name;
            item.Description = model.Description;
            item.SponsorType = model.SponsorType;
            item.ContactName = model.ContactName;
            item.Email = model.Email;
            item.Phone = model.Phone;
            item.Address = model.Address;
            item.ReceivableAccountId = model.ReceivableAccountId;
            item.IsActive = model.IsActive;
            await _db.SaveChangesAsync();
            return Ok(_mapper.Map<SponsorDto>(item));
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var item = await _db.Sponsors.FindAsync(id);
            if (item == null) return NotFound();
            var inUse = await _db.Sponsorships.AnyAsync(s => s.SponsorId == id)
                || await _db.SponsorPayments.AnyAsync(p => p.SponsorId == id);
            if (inUse) return BadRequest(new { message = "Sponsor has sponsorships or payments and cannot be deleted." });
            _db.Sponsors.Remove(item);
            await _db.SaveChangesAsync();
            return Ok();
        }

        // GET /api/sponsors/{id}/statement — ledger of sponsor receivable
        [HttpGet("{id}/statement")]
        public async Task<IActionResult> Statement(int id, DateTime? from, DateTime? to)
        {
            var sponsor = await _db.Sponsors.Include(s => s.ReceivableAccount).FirstOrDefaultAsync(s => s.Id == id);
            if (sponsor == null) return NotFound();

            // Invoice-driven AR: each sponsored invoice item's Discount is a charge on the sponsor.
            var chargesQuery = _db.StudentInvoiceItems
                .Include(i => i.StudentInvoice).ThenInclude(inv => inv!.Student)
                .Where(i => i.SponsorshipId != null && i.Sponsorship!.SponsorId == id);
            if (from.HasValue) chargesQuery = chargesQuery.Where(i => i.StudentInvoice!.InvoiceDate >= from.Value);
            if (to.HasValue) chargesQuery = chargesQuery.Where(i => i.StudentInvoice!.InvoiceDate <= to.Value);
            var charges = await chargesQuery.ToListAsync();

            var paymentsQuery = _db.SponsorPayments.Where(p => p.SponsorId == id);
            if (from.HasValue) paymentsQuery = paymentsQuery.Where(p => p.PaymentDate >= from.Value);
            if (to.HasValue) paymentsQuery = paymentsQuery.Where(p => p.PaymentDate <= to.Value);
            var payments = await paymentsQuery.ToListAsync();

            var rows = charges.Select(c => new {
                date = c.StudentInvoice!.InvoiceDate,
                type = "Charge",
                reference = c.StudentInvoice.InvoiceNumber,
                description = $"{c.StudentInvoice.Student?.FullName} - {c.FeeCategory?.Name}",
                debit = c.Discount,
                credit = 0m
            }).Concat(payments.Select(p => new {
                date = p.PaymentDate,
                type = "Payment",
                reference = p.ReferenceNumber,
                description = p.Description ?? "",
                debit = 0m,
                credit = p.Amount
            })).OrderBy(r => r.date).ToList();

            decimal running = 0;
            var ledger = rows.Select(r => {
                running += r.debit - r.credit;
                return new { r.date, r.type, r.reference, r.description, r.debit, r.credit, balance = running };
            }).ToList();

            return Ok(new {
                sponsor = new { sponsor.Id, sponsor.Name },
                fromDate = from,
                toDate = to,
                totalCharges = rows.Sum(r => r.debit),
                totalPayments = rows.Sum(r => r.credit),
                balance = running,
                entries = ledger
            });
        }
    }
}
