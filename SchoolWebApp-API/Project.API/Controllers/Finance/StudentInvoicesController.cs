using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolWebApp.Core.DTOs.Finance.StudentInvoice;
using SchoolWebApp.Core.Entities.Finance;
using SchoolWebApp.Core.Entities.Sponsorships;
using SchoolWebApp.Core.Interfaces.IRepositories;
using SchoolWebApp.API.Licensing;
using SchoolWebApp.API.Security;
using SchoolWebApp.API.Services;

namespace SchoolWebApp.API.Controllers.Finance
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    [RequiresModule(AppModules.Finance)]
    [RequiresMenu(MenuGroup.Finance)]
    public class StudentInvoicesController : ControllerBase
    {
        private readonly ILogger<StudentInvoicesController> _logger;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly SponsorAllocator _allocator;

        public StudentInvoicesController(ILogger<StudentInvoicesController> logger, IUnitOfWork unitOfWork, IMapper mapper, SponsorAllocator allocator)
        {
            _logger = logger; _unitOfWork = unitOfWork; _mapper = mapper; _allocator = allocator;
        }

        // New pledges on invoices: any credit those sponsors hold goes onto them straight away.
        private async Task ApplySponsorCredit(IEnumerable<StudentInvoiceItem> items)
        {
            var sponsorshipIds = items.Where(i => i.SponsorshipId != null && i.SponsoredAmount > 0).Select(i => i.SponsorshipId!.Value).Distinct().ToList();
            if (sponsorshipIds.Count == 0) return;
            var sponsorIds = (await _unitOfWork.Repository<Sponsorship>().Find(s => sponsorshipIds.Contains(s.Id)))
                .Select(s => s.SponsorId).Distinct().ToList();
            foreach (var id in sponsorIds) await _allocator.ApplyHeldCredit(id);
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var items = await _unitOfWork.StudentInvoices.Find(includeProperties: "Student,AcademicYear,Session,Items.FeeCategory");
            return Ok(_mapper.Map<List<StudentInvoiceDto>>(items));
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var item = await _unitOfWork.StudentInvoices.GetByIdWithDetails(id);
            if (item == null) return NotFound();
            return Ok(_mapper.Map<StudentInvoiceDto>(item));
        }

        [HttpGet("byStudentId/{studentId}")]
        public async Task<IActionResult> GetByStudentId(int studentId)
        {
            var items = await _unitOfWork.StudentInvoices.GetByStudentId(studentId);
            return Ok(_mapper.Map<List<StudentInvoiceDto>>(items));
        }

        [HttpGet("byAcademicYearId/{yearId}")]
        public async Task<IActionResult> GetByYear(int yearId)
        {
            var items = await _unitOfWork.StudentInvoices.GetByAcademicYearId(yearId);
            return Ok(_mapper.Map<List<StudentInvoiceDto>>(items));
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateStudentInvoiceDto model)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var invoice = _mapper.Map<StudentInvoice>(model);
            if (string.IsNullOrEmpty(invoice.InvoiceNumber))
                invoice.InvoiceNumber = $"INV-{DateTime.Now:yyyyMMddHHmmssfff}";
            invoice.TotalAmount = model.Items.Sum(i => i.Amount - i.Discount);
            foreach (var it in invoice.Items) { it.SponsorPaidAmount = 0; if (it.SponsorshipId == null) it.SponsoredAmount = 0; }
            _unitOfWork.StudentInvoices.Create(invoice);
            await _unitOfWork.SaveChangesAsync();

            await AutoPostInvoiceJournal(invoice);
            await ApplySponsorCredit(invoice.Items);

            return Ok(_mapper.Map<StudentInvoiceDto>(invoice));
        }

        [HttpPost("bulk")]
        public async Task<IActionResult> BulkCreate(BulkInvoiceDto model)
        {
            var feeStructure = await _unitOfWork.FeeStructures.GetByIdWithItems(model.FeeStructureId);
            if (feeStructure == null) return NotFound(new { message = "Fee structure not found." });

            // A student entered twice in the class is invoiced once.
            var studentClasses = (await _unitOfWork.StudentClasses.GetBySchoolClassId(model.SchoolClassId, Core.Entities.Enums.Status.Active))
                .GroupBy(sc => sc.StudentId).Select(g => g.First()).ToList();
            if (studentClasses.Count == 0) return BadRequest(new { message = "No active students in class." });

            // Pre-fetch all active sponsorships relevant to this year/session/class. We filter per-student
            // inside the loop by StudentId OR SchoolClassId membership.
            var allActiveSponsorships = (await _unitOfWork.Repository<Sponsorship>().Find(s =>
                    s.Status == SponsorshipStatus.Active
                    && s.AcademicYearId == feeStructure.AcademicYearId
                    && (s.SessionId == null || feeStructure.SessionId == null || s.SessionId == feeStructure.SessionId)
                    && s.StartDate <= model.InvoiceDate
                    && (s.EndDate == null || s.EndDate >= model.InvoiceDate),
                includeProperties: "FeeCategories,Sponsor"))
                .ToList();

            var createdCount = 0;
            var createdInvoices = new List<StudentInvoice>();
            var studentsWithSponsorships = 0;
            var sponsoredLineCount = 0;
            decimal totalSponsored = 0m;
            var perSponsorTotals = new Dictionary<string, decimal>();

            foreach (var sc in studentClasses)
            {
                // Applicable sponsorships: directly on student OR on the class they're in.
                var applicable = allActiveSponsorships
                    .Where(s => s.StudentId == sc.StudentId || s.SchoolClassId == sc.SchoolClassId)
                    .ToList();

                var invoice = new StudentInvoice
                {
                    InvoiceNumber = $"INV-{DateTime.Now:yyyyMMddHHmmssfff}-{sc.StudentId}",
                    StudentId = sc.StudentId,
                    AcademicYearId = feeStructure.AcademicYearId,
                    SessionId = feeStructure.SessionId,
                    InvoiceDate = model.InvoiceDate,
                    DueDate = model.DueDate,
                    Description = model.Description ?? feeStructure.Name,
                    Status = InvoiceStatus.Unpaid
                };

                // FixedAmount sponsorships have a cap per invoice that needs to be consumed across items.
                var fixedRemaining = applicable
                    .Where(s => s.CoverageType == SponsorshipCoverageType.FixedAmount)
                    .ToDictionary(s => s.Id, s => s.FixedAmount);

                var studentGotSponsorship = false;

                foreach (var fi in feeStructure.Items.Where(i => i.IsMandatory))
                {
                    var item = new StudentInvoiceItem
                    {
                        FeeCategoryId = fi.FeeCategoryId,
                        Amount = fi.Amount,
                        Discount = 0
                    };

                    // Find the first sponsorship that covers this fee category. It pledges its share of the fee: the
                    // student is still invoiced the whole fee, and stays responsible for it if the sponsor does not pay.
                    // Empty FeeCategories list = covers all categories.
                    var matching = applicable.FirstOrDefault(s =>
                        s.FeeCategories.Count == 0 || s.FeeCategories.Any(fc => fc.FeeCategoryId == fi.FeeCategoryId));

                    if (matching != null)
                    {
                        decimal coveredAmount = 0;
                        switch (matching.CoverageType)
                        {
                            case SponsorshipCoverageType.FullCoverage:
                                coveredAmount = fi.Amount;
                                break;
                            case SponsorshipCoverageType.Percentage:
                                coveredAmount = Math.Round(fi.Amount * (matching.Percentage / 100m), 2);
                                break;
                            case SponsorshipCoverageType.FixedAmount:
                                var rem = fixedRemaining.TryGetValue(matching.Id, out var r) ? r : 0;
                                coveredAmount = Math.Min(rem, fi.Amount);
                                fixedRemaining[matching.Id] = rem - coveredAmount;
                                break;
                        }

                        if (coveredAmount > 0)
                        {
                            item.SponsoredAmount = coveredAmount;
                            item.SponsorshipId = matching.Id;
                            sponsoredLineCount++;
                            totalSponsored += coveredAmount;
                            var key = matching.Sponsor?.Name ?? $"Sponsor #{matching.SponsorId}";
                            if (!perSponsorTotals.ContainsKey(key)) perSponsorTotals[key] = 0;
                            perSponsorTotals[key] += coveredAmount;
                            studentGotSponsorship = true;
                        }
                    }

                    invoice.Items.Add(item);
                }

                invoice.TotalAmount = invoice.Items.Sum(i => i.Amount - i.Discount);
                _unitOfWork.StudentInvoices.Create(invoice);
                createdInvoices.Add(invoice);
                createdCount++;
                if (studentGotSponsorship) studentsWithSponsorships++;
            }
            await _unitOfWork.SaveChangesAsync();

            foreach (var inv in createdInvoices)
                await AutoPostInvoiceJournal(inv);
            await ApplySponsorCredit(createdInvoices.SelectMany(i => i.Items));

            return Ok(new
            {
                message = $"{createdCount} invoice(s) created.",
                count = createdCount,
                studentsWithSponsorships,
                sponsoredLineCount,
                totalSponsored,
                perSponsor = perSponsorTotals.Select(kv => new { sponsor = kv.Key, amount = kv.Value }).ToList()
            });
        }

        [HttpGet("postMissingJournals")]
        public async Task<IActionResult> PostMissingJournals()
        {
            var allInvoices = await _unitOfWork.StudentInvoices.Find(includeProperties: "Items.FeeCategory");
            var existingJournals = await _unitOfWork.JournalEntries.Find();
            var existingRefs = new HashSet<string>(existingJournals.Select(j => j.ReferenceNumber));

            int posted = 0;
            foreach (var inv in allInvoices)
            {
                var refNumber = $"INV-JNL-{inv.InvoiceNumber}";
                if (existingRefs.Contains(refNumber)) continue;
                await AutoPostInvoiceJournal(inv);
                posted++;
            }
            return Ok(new { message = $"{posted} missing invoice journal(s) posted.", count = posted });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, CreateStudentInvoiceDto model)
        {
            var existing = await _unitOfWork.StudentInvoices.GetByIdWithDetails(id);
            if (existing == null) return NotFound();

            // Lines are changed in place, so what has been paid on each (and by which sponsor) stays with it. A line
            // with money on it cannot be dropped, and a pledge cannot fall below what the sponsor has already paid.
            var keep = model.Items.Where(i => i.Id != null).Select(i => i.Id!.Value).ToHashSet();
            foreach (var old in existing.Items.Where(i => !keep.Contains(i.Id)).ToList())
            {
                if (old.PaidAmount > 0)
                    return BadRequest(new { message = "A fee line that has been paid cannot be removed from the invoice." });
                _unitOfWork.StudentInvoiceItems.Delete(old);
            }
            foreach (var item in model.Items)
            {
                var line = item.Id != null ? existing.Items.FirstOrDefault(i => i.Id == item.Id) : null;
                var pledged = item.SponsorshipId != null ? item.SponsoredAmount : 0;
                if (line != null && pledged < line.SponsorPaidAmount)
                    return BadRequest(new { message = $"The sponsor has already paid {line.SponsorPaidAmount:N2} on a line; its pledge cannot be less." });
                if (line == null)
                {
                    _unitOfWork.StudentInvoiceItems.Create(new StudentInvoiceItem
                    {
                        StudentInvoiceId = existing.Id,
                        FeeCategoryId = item.FeeCategoryId,
                        Amount = item.Amount,
                        Discount = item.Discount,
                        SponsorshipId = item.SponsorshipId,
                        SponsoredAmount = pledged,
                        Description = item.Description
                    });
                    continue;
                }
                line.FeeCategoryId = item.FeeCategoryId;
                line.Amount = item.Amount;
                line.Discount = item.Discount;
                line.SponsorshipId = item.SponsorshipId;
                line.SponsoredAmount = pledged;
                line.Description = item.Description;
            }
            await _unitOfWork.SaveChangesAsync();

            // Recalculate total
            existing.TotalAmount = model.Items.Sum(i => i.Amount - i.Discount);
            existing.Description = model.Description ?? existing.Description;
            var balance = existing.TotalAmount - existing.PaidAmount - existing.DiscountAmount;
            if (balance <= 0 && existing.PaidAmount > 0) existing.Status = InvoiceStatus.Paid;
            else if (existing.PaidAmount > 0) existing.Status = InvoiceStatus.PartiallyPaid;
            else existing.Status = InvoiceStatus.Unpaid;

            _unitOfWork.StudentInvoices.Update(existing);
            await _unitOfWork.SaveChangesAsync();

            // Re-post journal (delete old, create new)
            var refPrefix = $"INV-JNL-{existing.InvoiceNumber}";
            var journals = await _unitOfWork.JournalEntries.Find(j => j.ReferenceNumber.StartsWith(refPrefix));
            foreach (var je in journals)
            {
                var full = await _unitOfWork.JournalEntries.GetByIdWithLines(je.Id);
                if (full != null)
                {
                    foreach (var line in full.Lines.ToList())
                        _unitOfWork.JournalLines.Delete(line);
                    _unitOfWork.JournalEntries.Delete(full);
                }
            }
            await _unitOfWork.SaveChangesAsync();
            await AutoPostInvoiceJournal(existing);
            await ApplySponsorCredit(existing.Items);

            return Ok(_mapper.Map<StudentInvoiceDto>(await _unitOfWork.StudentInvoices.GetByIdWithDetails(id)));
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var item = await _unitOfWork.StudentInvoices.GetByIdWithDetails(id);
            if (item == null) return NotFound();

            // Reverse the auto-posted journal if it exists
            var refPrefix = $"INV-JNL-{item.InvoiceNumber}";
            var journals = await _unitOfWork.JournalEntries.Find(
                j => j.ReferenceNumber.StartsWith(refPrefix));
            foreach (var je in journals)
            {
                var full = await _unitOfWork.JournalEntries.GetByIdWithLines(je.Id);
                if (full != null)
                {
                    foreach (var line in full.Lines.ToList())
                        _unitOfWork.JournalLines.Delete(line);
                    _unitOfWork.JournalEntries.Delete(full);
                }
            }

            _unitOfWork.StudentInvoices.Delete(item);
            await _unitOfWork.SaveChangesAsync();
            return Ok();
        }

        private async Task<int?> GetSettingAccountId(string key)
        {
            var settings = await _unitOfWork.Repository<SchoolWebApp.Core.Entities.Settings.GlobalSetting>()
                .Find(s => s.Module == "Finance" && s.SettingKey == key);
            var val = settings.FirstOrDefault()?.SettingValue;
            return int.TryParse(val, out var id) ? id : null;
        }

        /// <summary>
        /// Reads a true/false Finance setting. Anything other than an explicit
        /// "false" counts as on, so auto-posting is the default when unset.
        /// </summary>
        private async Task<bool> IsFinanceSettingOn(string key)
        {
            var settings = await _unitOfWork.Repository<SchoolWebApp.Core.Entities.Settings.GlobalSetting>()
                .Find(s => s.Module == "Finance" && s.SettingKey == key);
            var val = settings.FirstOrDefault()?.SettingValue;
            return !string.Equals(val?.Trim(), "false", StringComparison.OrdinalIgnoreCase);
        }

        private async Task AutoPostInvoiceJournal(StudentInvoice invoice)
        {
            // The Auto-Post Invoice Journal switch on the Finance settings page. It was
            // shown there but never read, so switching it off did nothing.
            if (!await IsFinanceSettingOn("AutoPostInvoiceJournal")) return;

            var debtorsAccountId = await GetSettingAccountId("DebtorsAccountId");
            if (debtorsAccountId == null) return; // Skip auto-posting if no Debtors account configured

            // Load fee categories to get income accounts for each item
            var items = invoice.Items.Any()
                ? invoice.Items
                : (await _unitOfWork.StudentInvoices.GetByIdWithDetails(invoice.Id))?.Items ?? new List<StudentInvoiceItem>();

            // Debit Student Debtors with what the student is invoiced (every fee less any discount) and credit each fee's
            // income account the same. A sponsor's pledge is not a separate debt: the student owes the whole invoice
            // until the sponsor (or the family) pays it.
            var studentDebitTotal = 0m;
            var creditByIncomeAccount = new Dictionary<int, decimal>();

            foreach (var item in items)
            {
                var cat = await _unitOfWork.FeeCategories.GetById(item.FeeCategoryId);
                if (cat?.IncomeAccountId == null) continue;
                var charged = item.Amount - item.Discount;
                studentDebitTotal += charged;
                if (!creditByIncomeAccount.ContainsKey(cat.IncomeAccountId.Value))
                    creditByIncomeAccount[cat.IncomeAccountId.Value] = 0;
                creditByIncomeAccount[cat.IncomeAccountId.Value] += charged;
            }

            if (creditByIncomeAccount.Count == 0 || studentDebitTotal <= 0) return;

            var journal = new JournalEntry
            {
                ReferenceNumber = $"INV-JNL-{invoice.InvoiceNumber}",
                EntryDate = invoice.InvoiceDate,
                Description = $"Auto-posted: Invoice {invoice.InvoiceNumber}",
                IsPosted = true,
                Status = JournalEntryStatus.Approved,
                Lines = new List<JournalLine>()
            };

            // Debit Debtors for what the student is invoiced
            if (studentDebitTotal > 0)
            {
                journal.Lines.Add(new JournalLine
                {
                    AccountId = debtorsAccountId.Value,
                    Debit = studentDebitTotal,
                    Credit = 0,
                    Description = $"Student debtors - {invoice.InvoiceNumber}"
                });
            }

            // Credit each income account
            foreach (var kvp in creditByIncomeAccount)
            {
                journal.Lines.Add(new JournalLine
                {
                    AccountId = kvp.Key,
                    Debit = 0,
                    Credit = kvp.Value,
                    Description = $"Fee income - {invoice.InvoiceNumber}"
                });
            }

            _unitOfWork.JournalEntries.Create(journal);
            await _unitOfWork.SaveChangesAsync();
        }
    }
}
