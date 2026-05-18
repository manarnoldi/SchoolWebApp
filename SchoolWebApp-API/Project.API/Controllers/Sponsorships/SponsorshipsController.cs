using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Project.Infrastructure.Data;
using SchoolWebApp.Core.DTOs.Sponsorships;
using SchoolWebApp.Core.Entities.Finance;
using SchoolWebApp.Core.Entities.Sponsorships;

namespace SchoolWebApp.API.Controllers.Sponsorships
{
    [Authorize]
    [Route("api/sponsorships")]
    [ApiController]
    public class SponsorshipsController : ControllerBase
    {
        private readonly ApplicationDbContext _db;
        private readonly IMapper _mapper;
        public SponsorshipsController(ApplicationDbContext db, IMapper mapper) { _db = db; _mapper = mapper; }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var list = await _db.Sponsorships
                .Include(s => s.Sponsor)
                .Include(s => s.Student)
                .Include(s => s.SchoolClass).ThenInclude(c => c!.LearningLevel)
                .Include(s => s.AcademicYear)
                .Include(s => s.Session)
                .Include(s => s.FeeCategories)
                .OrderByDescending(s => s.StartDate)
                .ToListAsync();
            return Ok(_mapper.Map<List<SponsorshipDto>>(list));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var item = await _db.Sponsorships
                .Include(s => s.Sponsor)
                .Include(s => s.Student)
                .Include(s => s.SchoolClass).ThenInclude(c => c!.LearningLevel)
                .Include(s => s.AcademicYear)
                .Include(s => s.Session)
                .Include(s => s.FeeCategories)
                .FirstOrDefaultAsync(s => s.Id == id);
            if (item == null) return NotFound();
            return Ok(_mapper.Map<SponsorshipDto>(item));
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateSponsorshipDto model)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            if (model.StudentId == null && model.SchoolClassId == null)
                return BadRequest(new { message = "Either a student or a school class must be selected." });

            var item = new Sponsorship
            {
                SponsorId = model.SponsorId,
                StudentId = model.StudentId,
                SchoolClassId = model.SchoolClassId,
                AcademicYearId = model.AcademicYearId,
                SessionId = model.SessionId,
                CoverageType = model.CoverageType,
                FixedAmount = model.FixedAmount,
                Percentage = model.Percentage,
                StartDate = model.StartDate,
                EndDate = model.EndDate,
                Notes = model.Notes,
                Status = model.Status,
                FeeCategories = (model.FeeCategoryIds ?? new List<int>())
                    .Select(fcId => new SponsorshipFeeCategory { FeeCategoryId = fcId }).ToList()
            };
            _db.Sponsorships.Add(item);
            await _db.SaveChangesAsync();
            return Ok(new { id = item.Id });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] CreateSponsorshipDto model)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var item = await _db.Sponsorships.Include(s => s.FeeCategories).FirstOrDefaultAsync(s => s.Id == id);
            if (item == null) return NotFound();
            if (model.StudentId == null && model.SchoolClassId == null)
                return BadRequest(new { message = "Either a student or a school class must be selected." });

            item.SponsorId = model.SponsorId;
            item.StudentId = model.StudentId;
            item.SchoolClassId = model.SchoolClassId;
            item.AcademicYearId = model.AcademicYearId;
            item.SessionId = model.SessionId;
            item.CoverageType = model.CoverageType;
            item.FixedAmount = model.FixedAmount;
            item.Percentage = model.Percentage;
            item.StartDate = model.StartDate;
            item.EndDate = model.EndDate;
            item.Notes = model.Notes;
            item.Status = model.Status;

            _db.SponsorshipFeeCategories.RemoveRange(item.FeeCategories);
            item.FeeCategories = (model.FeeCategoryIds ?? new List<int>())
                .Select(fcId => new SponsorshipFeeCategory { SponsorshipId = id, FeeCategoryId = fcId }).ToList();

            await _db.SaveChangesAsync();
            return Ok();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var item = await _db.Sponsorships.Include(s => s.FeeCategories).FirstOrDefaultAsync(s => s.Id == id);
            if (item == null) return NotFound();
            var used = await _db.StudentInvoiceItems.AnyAsync(i => i.SponsorshipId == id);
            if (used) return BadRequest(new { message = "This sponsorship has been applied to invoices and cannot be deleted. Mark it Ended instead." });
            _db.SponsorshipFeeCategories.RemoveRange(item.FeeCategories);
            _db.Sponsorships.Remove(item);
            await _db.SaveChangesAsync();
            return Ok();
        }

        /// <summary>
        /// Find active sponsorships that apply to a given student on a given date.
        /// Returns a list sorted by coverage weight — useful for the invoice form to auto-pre-fill.
        /// </summary>
        [HttpGet("byStudent/{studentId}")]
        public async Task<IActionResult> ByStudent(int studentId, int? academicYearId, int? sessionId, DateTime? onDate)
        {
            var day = onDate ?? DateTime.Today;

            // Student's class membership for the year (for class-wide sponsorships).
            var classIds = new List<int>();
            if (academicYearId.HasValue)
            {
                classIds = await _db.StudentClasses
                    .Where(sc => sc.StudentId == studentId && sc.SchoolClass != null && sc.SchoolClass.AcademicYearId == academicYearId.Value)
                    .Select(sc => sc.SchoolClassId)
                    .ToListAsync();
            }

            var list = await _db.Sponsorships
                .Include(s => s.Sponsor)
                .Include(s => s.FeeCategories)
                .Where(s => s.Status == SponsorshipStatus.Active
                    && s.StartDate <= day && (s.EndDate == null || s.EndDate >= day)
                    && (!academicYearId.HasValue || s.AcademicYearId == academicYearId.Value)
                    && (!sessionId.HasValue || s.SessionId == null || s.SessionId == sessionId.Value)
                    && (s.StudentId == studentId || (s.SchoolClassId != null && classIds.Contains(s.SchoolClassId.Value))))
                .ToListAsync();

            return Ok(_mapper.Map<List<SponsorshipDto>>(list));
        }

        /// <summary>
        /// Retroactively apply this sponsorship to existing open invoices that match its scope
        /// (student or class, year, session, covered fee categories). Posts an adjustment journal
        /// per invoice — Debit Sponsor Receivable / Credit Student Debtors — transferring the AR
        /// from the student to the sponsor. Skips fully-paid invoices and items already sponsored.
        /// </summary>
        [HttpPost("{id}/applyToExisting")]
        public async Task<IActionResult> ApplyToExisting(int id)
        {
            var sponsorship = await _db.Sponsorships
                .Include(s => s.Sponsor)
                .Include(s => s.FeeCategories)
                .FirstOrDefaultAsync(s => s.Id == id);
            if (sponsorship == null) return NotFound();
            if (sponsorship.Status != SponsorshipStatus.Active)
                return BadRequest(new { message = "Only Active sponsorships can be applied." });

            // Resolve target student ids: direct OR class members.
            var studentIds = new HashSet<int>();
            if (sponsorship.StudentId.HasValue) studentIds.Add(sponsorship.StudentId.Value);
            if (sponsorship.SchoolClassId.HasValue)
            {
                var classMembers = await _db.StudentClasses
                    .Where(sc => sc.SchoolClassId == sponsorship.SchoolClassId.Value)
                    .Select(sc => sc.StudentId)
                    .ToListAsync();
                foreach (var sid in classMembers) studentIds.Add(sid);
            }
            if (studentIds.Count == 0)
                return BadRequest(new { message = "Sponsorship has no beneficiary configured." });

            // Pre-load main Debtors account from settings.
            var debtorsAccountIdSetting = await _db.GlobalSettings
                .FirstOrDefaultAsync(s => s.Module == "Finance" && s.SettingKey == "DebtorsAccountId");
            int debtorsAccountId = int.TryParse(debtorsAccountIdSetting?.SettingValue, out var dx) ? dx : 0;

            // Sponsor receivable account: dedicated one if set, otherwise fall back to main debtors.
            int sponsorAccountId = sponsorship.Sponsor?.ReceivableAccountId ?? debtorsAccountId;
            if (sponsorAccountId == 0 || debtorsAccountId == 0)
                return BadRequest(new { message = "Debtors / Sponsor receivable account is not configured." });

            // Load candidate invoices.
            var invoices = await _db.StudentInvoices
                .Include(i => i.Items)
                .Where(i => studentIds.Contains(i.StudentId)
                    && i.AcademicYearId == sponsorship.AcademicYearId
                    && (sponsorship.SessionId == null || i.SessionId == null || i.SessionId == sponsorship.SessionId)
                    && i.InvoiceDate >= sponsorship.StartDate
                    && (sponsorship.EndDate == null || i.InvoiceDate <= sponsorship.EndDate)
                    && i.Status != InvoiceStatus.Paid
                    && i.Status != InvoiceStatus.Cancelled)
                .ToListAsync();

            var coveredCategories = sponsorship.FeeCategories.Select(fc => fc.FeeCategoryId).ToHashSet();
            bool coversAll = coveredCategories.Count == 0;

            int appliedInvoices = 0, appliedItems = 0;
            int skippedFullyPaidInvoices = 0, skippedAlreadySponsoredItems = 0, skippedNoCoverageItems = 0;
            decimal totalApplied = 0m;

            foreach (var inv in invoices)
            {
                // Per-invoice cap for FixedAmount sponsorships.
                decimal fixedRemaining = sponsorship.CoverageType == SponsorshipCoverageType.FixedAmount
                    ? sponsorship.FixedAmount : 0m;

                decimal invoiceApplied = 0m;

                foreach (var item in inv.Items)
                {
                    if (item.SponsorshipId.HasValue) { skippedAlreadySponsoredItems++; continue; }
                    if (!coversAll && !coveredCategories.Contains(item.FeeCategoryId)) { skippedNoCoverageItems++; continue; }

                    decimal openOnItem = item.Amount - item.Discount - item.PaidAmount;
                    if (openOnItem <= 0) continue;

                    decimal cover = 0m;
                    switch (sponsorship.CoverageType)
                    {
                        case SponsorshipCoverageType.FullCoverage:
                            cover = openOnItem;
                            break;
                        case SponsorshipCoverageType.Percentage:
                            cover = Math.Round(openOnItem * (sponsorship.Percentage / 100m), 2);
                            break;
                        case SponsorshipCoverageType.FixedAmount:
                            cover = Math.Min(fixedRemaining, openOnItem);
                            fixedRemaining -= cover;
                            break;
                    }
                    if (cover <= 0) continue;

                    item.Discount += cover;
                    item.SponsorshipId = sponsorship.Id;
                    invoiceApplied += cover;
                    appliedItems++;
                }

                if (invoiceApplied > 0)
                {
                    inv.TotalAmount = Math.Max(0, inv.TotalAmount - invoiceApplied);
                    var balance = inv.TotalAmount - inv.PaidAmount - inv.DiscountAmount;
                    if (balance <= 0 && inv.PaidAmount > 0) inv.Status = InvoiceStatus.Paid;
                    else if (inv.PaidAmount > 0) inv.Status = InvoiceStatus.PartiallyPaid;
                    else if (balance <= 0) inv.Status = InvoiceStatus.Paid;

                    // Adjustment journal: transfer AR from main Debtors to Sponsor Receivable.
                    var journal = new JournalEntry
                    {
                        ReferenceNumber = $"SP-ADJ-{inv.InvoiceNumber}-{sponsorship.Id}",
                        EntryDate = DateTime.Today,
                        Description = $"Sponsorship retro-applied: {sponsorship.Sponsor?.Name} on {inv.InvoiceNumber}",
                        IsPosted = true,
                        Status = JournalEntryStatus.Approved,
                        Lines = new List<JournalLine>
                        {
                            new JournalLine { AccountId = sponsorAccountId, Debit = invoiceApplied, Credit = 0,
                                Description = $"Sponsor receivable - {sponsorship.Sponsor?.Name} ({inv.InvoiceNumber})" },
                            new JournalLine { AccountId = debtorsAccountId, Debit = 0, Credit = invoiceApplied,
                                Description = $"Reduce student debtors - {inv.InvoiceNumber}" }
                        }
                    };
                    _db.JournalEntries.Add(journal);

                    totalApplied += invoiceApplied;
                    appliedInvoices++;
                }
                else
                {
                    if (inv.Status == InvoiceStatus.Paid) skippedFullyPaidInvoices++;
                }
            }

            await _db.SaveChangesAsync();

            return Ok(new
            {
                message = $"Sponsorship applied to {appliedInvoices} invoice(s).",
                appliedInvoices,
                appliedItems,
                totalApplied,
                skippedFullyPaidInvoices,
                skippedAlreadySponsoredItems,
                skippedNoCoverageItems
            });
        }
    }
}
