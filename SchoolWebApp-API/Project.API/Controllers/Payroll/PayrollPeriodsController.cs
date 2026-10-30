using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolWebApp.Core.DTOs.Payroll;
using SchoolWebApp.Core.Entities.Enums;
using SchoolWebApp.Core.Entities.Payroll;
using SchoolWebApp.Core.Entities.Finance;
using SchoolWebApp.Core.Interfaces.IRepositories;
using Microsoft.EntityFrameworkCore;
using Project.Infrastructure.Data;
using SchoolWebApp.API.Services;
using System.Globalization;
using SchoolWebApp.API.Licensing;
using SchoolWebApp.API.Security;

namespace SchoolWebApp.API.Controllers.Payroll
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    [RequiresModule(AppModules.Payroll)]
    [RequiresMenu(MenuGroup.Payroll)]
    public class PayrollPeriodsController : ControllerBase
    {
        private readonly ILogger<PayrollPeriodsController> _logger;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly PayrollApprovalService _approval;
        private readonly ApplicationDbContext _db;

        public PayrollPeriodsController(ILogger<PayrollPeriodsController> logger, IUnitOfWork unitOfWork, IMapper mapper,
            PayrollApprovalService approval, ApplicationDbContext db)
        {
            _logger = logger; _unitOfWork = unitOfWork; _mapper = mapper; _approval = approval; _db = db;
        }

        // GET api/payrollPeriods
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var periods = await _unitOfWork.PayrollPeriods.GetAllWithPayslips();
            var result = periods.Select(p => new PayrollPeriodDto
            {
                Id = p.Id,
                Month = p.Month,
                Year = p.Year,
                Name = p.Name,
                Status = (int)p.Status,
                StatusLabel = p.Status.ToString(),
                ProcessedDate = p.ProcessedDate,
                ApprovedDate = p.ApprovedDate,
                PostedDate = p.PostedDate,
                PayslipCount = p.Payslips.Count,
                TotalGross = p.Payslips.Sum(s => s.GrossPay),
                TotalNet = p.Payslips.Sum(s => s.NetPay),
                TotalPaye = p.Payslips.Sum(s => s.Paye),
                TotalNssf = p.Payslips.Sum(s => s.NssfEmployee),
                TotalShif = p.Payslips.Sum(s => s.Shif)
            }).ToList();
            return Ok(result);
        }

        // GET api/payrollPeriods/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var p = await _unitOfWork.PayrollPeriods.GetByIdWithPayslips(id);
            if (p == null) return NotFound();
            var dto = new PayrollPeriodDto
            {
                Id = p.Id,
                Month = p.Month,
                Year = p.Year,
                Name = p.Name,
                Status = (int)p.Status,
                StatusLabel = p.Status.ToString(),
                ProcessedDate = p.ProcessedDate,
                ApprovedDate = p.ApprovedDate,
                PostedDate = p.PostedDate,
                PayslipCount = p.Payslips.Count,
                TotalGross = p.Payslips.Sum(s => s.GrossPay),
                TotalNet = p.Payslips.Sum(s => s.NetPay),
                TotalPaye = p.Payslips.Sum(s => s.Paye),
                TotalNssf = p.Payslips.Sum(s => s.NssfEmployee),
                TotalShif = p.Payslips.Sum(s => s.Shif)
            };
            return Ok(dto);
        }

        // GET api/payrollPeriods/5/payslips
        [HttpGet("{id}/payslips")]
        public async Task<IActionResult> GetPayslips(int id)
        {
            var period = await _unitOfWork.PayrollPeriods.GetById(id);
            if (period == null) return NotFound();
            var payslips = await _unitOfWork.Payslips.GetByPeriodId(id);
            return Ok(_mapper.Map<List<PayslipDto>>(payslips));
        }

        // GET api/payrollPeriods/payslip/5
        [HttpGet("payslip/{id}")]
        public async Task<IActionResult> GetPayslipById(int id)
        {
            var payslip = await _unitOfWork.Payslips.GetByIdFull(id);
            if (payslip == null) return NotFound();
            return Ok(_mapper.Map<PayslipDto>(payslip));
        }

        // POST api/payrollPeriods
        [HttpPost]
        public async Task<IActionResult> Create(CreatePayrollPeriodDto model)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            // Check for duplicate period
            var existing = await _unitOfWork.PayrollPeriods
                .Find(p => p.Month == model.Month && p.Year == model.Year);
            if (existing.Any())
                return BadRequest($"A payroll period for {CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(model.Month)} {model.Year} already exists.");

            var period = _mapper.Map<PayrollPeriod>(model);
            // Auto-generate Name as "Month Year"
            period.Name = $"{CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(model.Month)} {model.Year}";
            period.Status = PayrollPeriodStatus.Draft;

            _unitOfWork.PayrollPeriods.Create(period);
            await _unitOfWork.SaveChangesAsync();
            return Ok(_mapper.Map<PayrollPeriodDto>(period));
        }

        // POST api/payrollPeriods/5/process
        [HttpPost("{id}/process")]
        public async Task<IActionResult> Process(int id)
        {
            var period = await _unitOfWork.PayrollPeriods.GetByIdWithPayslips(id);
            if (period == null) return NotFound();

            if (period.Status == PayrollPeriodStatus.Approved || period.Status == PayrollPeriodStatus.Posted)
                return BadRequest("Cannot process an approved or posted payroll period.");
            var locked = await _approval.LockedReasonAsync(id);
            if (locked != null) return BadRequest(locked);

            // --- Load reference data ---
            var settings = (await _unitOfWork.PayrollSettings.Find(s => s.IsActive)).ToList();
            var taxBands = (await _unitOfWork.TaxBands.Find(t => t.IsActive))
                .OrderBy(t => t.LowerLimit).ToList();
            var earningTypes = (await _unitOfWork.EarningTypes.Find(e => e.IsActive)).ToList();
            var deductionTypes = (await _unitOfWork.DeductionTypes.Find(d => d.IsActive)).ToList();

            // Helper to read a payroll setting by key
            decimal Setting(string key) => settings.FirstOrDefault(s => s.Key == key)?.Value ?? 0m;

            decimal shifRate = Setting("ShifRate") / 100m;
            decimal shifMinimum = Setting("ShifMinimum");
            decimal ahlRate = Setting("AhlRate") / 100m;
            // The employer's matching levy - an employer cost for the journal, never
            // a payslip deduction. Its own setting so the two rates can differ.
            decimal ahlEmployerRate = Setting("AhlEmployerRate") / 100m;

            // Retirement relief: NSSF and retirement-scheme contributions are pooled
            // and allowed at the lowest of the actual total, this percentage of basic
            // pay, and this monthly cap. A limit set to 0 is treated as not applied.
            decimal retirementReliefPercent = Setting("RetirementReliefPercent") / 100m;
            decimal retirementReliefCap = Setting("RetirementReliefCap");

            // SHIF rounding is set by the SHA rules, so it stays with the statutory
            // rates: 1 rounds to the nearest shilling, 0 keeps the cents.
            decimal shifRoundingUnit = Setting("ShifRoundingUnit");

            static decimal RoundTo(decimal value, decimal unit) => unit <= 0
                ? Math.Round(value, 2, MidpointRounding.AwayFromZero)
                : Math.Round(value / unit, 0, MidpointRounding.AwayFromZero) * unit;

            // Net pay rounding is the school's own policy - Global Settings > Payroll.
            var (netPayRoundingUnit, netPayRoundingMethod) = await GetNetPayRounding();

            static decimal RoundNetPay(decimal value, decimal unit, string method)
            {
                if (unit <= 0) return Math.Round(value, 2, MidpointRounding.AwayFromZero);
                return method switch
                {
                    "down" => Math.Floor(value / unit) * unit,
                    "up" => Math.Ceiling(value / unit) * unit,
                    _ => Math.Round(value / unit, 0, MidpointRounding.AwayFromZero) * unit
                };
            }
            // Note: no NSSF, personal relief or insurance relief settings are read
            // here any more. NSSF tiers are dated rows in NssfBands, reliefs are rows
            // in PayrollReliefs, and a deductible cap sits on the deduction type - so
            // each can change without touching this code.

            // NSSF limits move most Februaries, so the tiers are dated. Use the most
            // recent set in force for the period being run: re-processing an old
            // period then still uses that period's limits, not whatever is current.
            var periodStart = new DateTime(period.Year, period.Month, 1);
            var candidateBands = (await _unitOfWork.Repository<NssfBand>()
                .Find(b => b.IsActive && b.EffectiveDate <= periodStart)).ToList();
            if (candidateBands.Count == 0)
                return BadRequest($"No NSSF bands are in force for {period.Name}. Set them up under Settings > Dropdowns > Payroll Settings > NSSF Bands before processing.");
            var latestNssfDate = candidateBands.Max(b => b.EffectiveDate);
            var nssfBands = candidateBands
                .Where(b => b.EffectiveDate == latestNssfDate)
                .OrderBy(b => b.Tier)
                .ToList();

            // Reliefs come off the tax itself once the bands have been applied.
            var reliefs = (await _unitOfWork.Repository<PayrollRelief>()
                .Find(r => r.IsActive)).ToList();

            // Lookup helpers for earning/deduction types by code
            EarningType? EarningByCode(string code) => earningTypes.FirstOrDefault(e => e.Code == code);
            DeductionType? DeductionByCode(string code) => deductionTypes.FirstOrDefault(d => d.Code == code);

            // How a deduction is treated for tax is configured on the deduction type
            // itself, not hard-coded against a code here, so a change in the law is a
            // settings change. Resolved once rather than per employee.
            // Retirement contributions go through the pooled retirement relief below,
            // so they are kept out of the per-type tax-deductible set even if ticked -
            // otherwise the same contribution would come off taxable income twice.
            var retirementTypeIds = deductionTypes
                .Where(d => d.IsRetirementContribution).Select(d => d.Id).ToHashSet();
            var taxDeductible = deductionTypes
                .Where(d => d.IsTaxDeductible && !d.IsRetirementContribution)
                .ToDictionary(d => d.Id, d => d.TaxDeductibleCap);

            // Earnings marked non-taxable stay in gross and net pay but are left out
            // of taxable pay. An unknown type is treated as taxable - the safe side.
            var nonTaxableEarningTypeIds = earningTypes
                .Where(e => !e.IsTaxable).Select(e => e.Id).ToHashSet();
            // Types applied to everyone without being assigned per employee (NITA,
            // staff welfare, and the like). System-computed types are excluded: the
            // engine already works PAYE, NSSF, SHIF and AHL out from the statutory
            // rules, and applying them again here would double them.
            var autoEarningTypes = earningTypes
                .Where(e => e.AppliesToAll && !e.IsSystemComputed).ToList();
            var autoDeductionTypes = deductionTypes
                .Where(d => d.AppliesToAll && !d.IsSystemComputed).ToList();

            // A system-computed type (basic, house, transport; PAYE, NSSF, SHIF, AHL)
            // entered as a line on a salary structure would be counted a second time
            // on top of the engine's own figure, so such lines are ignored.
            var systemEarningTypeIds = earningTypes
                .Where(e => e.IsSystemComputed).Select(e => e.Id).ToHashSet();
            var systemDeductionTypeIds = deductionTypes
                .Where(d => d.IsSystemComputed).Select(d => d.Id).ToHashSet();

            // --- If re-processing, unwind the previous run before rebuilding ---
            if (period.Payslips.Any())
            {
                // Processing reduces loan balances, so the instalments taken by the
                // run being discarded have to be credited back first. Without this a
                // re-run would deduct the same instalment twice: the payslip would
                // still read one instalment but the loan would have paid down two.
                foreach (var d in period.Payslips.SelectMany(s => s.Deductions)
                             .Where(d => d.LoanAdvanceId != null && d.Amount > 0))
                {
                    var loan = await _unitOfWork.LoanAdvances.GetById(d.LoanAdvanceId!.Value);
                    if (loan == null) continue;

                    loan.Balance += d.Amount;
                    // The discarded run may have closed the loan off. Restoring a
                    // balance reopens it, but a loan cancelled by hand stays cancelled.
                    if (loan.Status == LoanStatus.FullyPaid && loan.Balance > 0)
                        loan.Status = LoanStatus.Active;
                    _unitOfWork.LoanAdvances.Update(loan);
                }

                foreach (var existingSlip in period.Payslips)
                {
                    // Delete earnings and deductions first
                    foreach (var e in existingSlip.Earnings)
                        _unitOfWork.PayslipEarnings.Delete(e);
                    foreach (var d in existingSlip.Deductions)
                        _unitOfWork.PayslipDeductions.Delete(d);
                    _unitOfWork.Payslips.Delete(existingSlip);
                }
                await _unitOfWork.SaveChangesAsync();
            }

            // --- Get active staff who are actually on the payroll ---
            // People kept on the staff list but paid outside payroll (suppliers,
            // contractors) are flagged ExcludeFromPayroll and never get a payslip.
            // Someone who joins later in the year is not paid for the months before they started.
            var periodEnd = periodStart.AddMonths(1).AddDays(-1);
            var activeStaff = (await _unitOfWork.StaffDetails.SearchForStaff(null, null, Status.Active))
                .Where(s => !s.ExcludeFromPayroll && (s.EmploymentDate == null || s.EmploymentDate.Value.Date <= periodEnd))
                .ToList();

            // --- Process each employee ---
            foreach (var staff in activeStaff)
            {
                var salary = await _unitOfWork.EmployeeSalaries.GetActiveByStaffId(staff.Id);
                if (salary == null) continue; // Skip staff without active salary structure

                // a. Gross Pay
                decimal basicSalary = salary.BasicSalary;
                decimal houseAllowance = salary.HouseAllowance;
                decimal transportAllowance = salary.TransportAllowance;
                decimal otherAllowances = salary.OtherAllowances;

                // Earning lines that apply to this employee: the ones on their own
                // salary structure, plus any type marked "applies to all" that they
                // have no line of their own for. Their own line always wins, which
                // is how one person gets a different figure from everyone else.
                var earningLines = salary.Items
                    .Where(i => i.EarningTypeId != null && !systemEarningTypeIds.Contains(i.EarningTypeId.Value))
                    .Select(i => (TypeId: i.EarningTypeId!.Value, Amount: i.Amount))
                    .ToList();
                var ownEarningTypes = earningLines.Select(l => l.TypeId).ToHashSet();
                foreach (var type in autoEarningTypes.Where(t => !ownEarningTypes.Contains(t.Id)))
                {
                    // A percentage earning is a percentage of BASIC - taking it off
                    // gross would define the earning in terms of itself.
                    decimal amount = type.CalculationMethod == PayrollCalculationMethod.Percentage
                        ? Math.Round(basicSalary * (type.DefaultValue ?? 0m) / 100m, 2)
                        : (type.DefaultValue ?? 0m);
                    if (amount > 0) earningLines.Add((type.Id, amount));
                }

                decimal salaryEarningItems = earningLines.Sum(l => l.Amount);

                decimal grossPay = basicSalary + houseAllowance + transportAllowance
                    + otherAllowances + salaryEarningItems;

                // Deduction lines that apply, resolved the same way. Percentages are
                // of gross, so this has to follow the gross calculation above.
                var deductionLines = salary.Items
                    .Where(i => i.DeductionTypeId != null && !systemDeductionTypeIds.Contains(i.DeductionTypeId.Value))
                    .Select(i => (TypeId: i.DeductionTypeId!.Value, Amount: i.Amount))
                    .ToList();
                var ownDeductionTypes = deductionLines.Select(l => l.TypeId).ToHashSet();
                foreach (var type in autoDeductionTypes.Where(t => !ownDeductionTypes.Contains(t.Id)))
                {
                    decimal amount = type.CalculationMethod == PayrollCalculationMethod.Percentage
                        ? Math.Round(grossPay * (type.DefaultValue ?? 0m) / 100m, 2)
                        : (type.DefaultValue ?? 0m);
                    if (amount > 0) deductionLines.Add((type.Id, amount));
                }

                // b. NSSF Employee - every tier in the set in force for this period.
                var tierAmounts = nssfBands
                    .Select(b => Math.Round(
                        Math.Max(0, Math.Min(grossPay, b.UpperLimit) - b.LowerLimit) * (b.Rate / 100m), 2))
                    .ToList();
                // The payslip prints Tier I and Tier II on their own lines; any
                // further tiers still count towards the total.
                decimal nssfTier1 = tierAmounts.ElementAtOrDefault(0);
                decimal nssfTier2 = tierAmounts.ElementAtOrDefault(1);
                decimal nssfEmployee = Math.Round(tierAmounts.Sum(), 2);

                // c. SHIF - 2.75% of gross, rounded to the configured unit (whole
                // shillings), subject to a statutory monthly minimum.
                decimal shifAmount = Math.Max(RoundTo(grossPay * shifRate, shifRoundingUnit), shifMinimum);

                // d. AHL (Affordable Housing Levy) - 1.5% of gross.
                decimal ahl = Math.Round(grossPay * ahlRate, 2, MidpointRounding.AwayFromZero);

                // e. Taxable pay - gross less any earnings marked non-taxable. Those
                // still count towards gross and net pay; they are just not taxed.
                decimal nonTaxableEarnings = earningLines
                    .Where(l => nonTaxableEarningTypeIds.Contains(l.TypeId))
                    .Sum(l => l.Amount);
                decimal taxablePay = grossPay - nonTaxableEarnings;

                // f. Retirement relief. NSSF and retirement-scheme contributions are
                // one pool, not separate allowances: the pool is allowed at the lowest
                // of the actual total, a percentage of basic pay, and a monthly cap.
                // Deducting NSSF in full and then capping pension on its own - as this
                // used to - allows more than the law does whenever the two together
                // exceed the cap.
                decimal retirementContributions = nssfEmployee + deductionLines
                    .Where(l => retirementTypeIds.Contains(l.TypeId))
                    .Sum(l => l.Amount);
                decimal retirementRelief = retirementContributions;
                if (retirementReliefPercent > 0)
                    retirementRelief = Math.Min(retirementRelief, basicSalary * retirementReliefPercent);
                if (retirementReliefCap > 0)
                    retirementRelief = Math.Min(retirementRelief, retirementReliefCap);
                retirementRelief = Math.Round(retirementRelief, 2, MidpointRounding.AwayFromZero);

                // g. Any other deduction ticked tax deductible (a post-retirement
                // medical fund, say), each capped by its own type.
                decimal otherTaxDeductible = deductionLines
                    .Where(l => taxDeductible.ContainsKey(l.TypeId))
                    .GroupBy(l => l.TypeId)
                    .Sum(g =>
                    {
                        decimal contributed = g.Sum(l => l.Amount);
                        decimal? cap = taxDeductible[g.Key];
                        return cap.HasValue ? Math.Min(contributed, cap.Value) : contributed;
                    });

                // h. Taxable Income.
                // The Tax Laws (Amendment) Act 2024 amended s.15(2) of the Income Tax
                // Act so that SHIF and the Affordable Housing Levy are allowable
                // DEDUCTIONS against taxable income rather than reliefs against tax.
                decimal taxableIncome = Math.Max(0,
                    taxablePay - retirementRelief - shifAmount - ahl - otherTaxDeductible);

                // g. Gross Tax (PAYE) - progressive tax bands
                decimal grossTax = 0m;
                foreach (var band in taxBands)
                {
                    if (taxableIncome <= band.LowerLimit) break;
                    decimal taxableInBand = Math.Min(taxableIncome, band.UpperLimit) - band.LowerLimit;
                    if (taxableInBand > 0)
                        grossTax += taxableInBand * (band.Rate / 100m);
                }
                grossTax = Math.Round(grossTax, 2, MidpointRounding.AwayFromZero);

                // h/i. Reliefs - configured rows, not hard-coded rules. A fixed
                // relief is a flat figure (personal relief); a percentage relief is
                // worked out from what the employee paid towards a named deduction
                // (insurance relief on premiums). Each is capped on its own.
                decimal empPersonalRelief = 0m;   // fixed reliefs, shown as "Personal Relief"
                decimal insuranceRelief = 0m;     // reliefs derived from a deduction
                foreach (var relief in reliefs)
                {
                    decimal amount;
                    if (relief.Basis == ReliefBasis.PercentageOfDeduction)
                    {
                        decimal basis = relief.DeductionTypeId == null ? 0m : deductionLines
                            .Where(l => l.TypeId == relief.DeductionTypeId.Value)
                            .Sum(l => l.Amount);
                        if (basis <= 0) continue;
                        amount = basis * relief.Value / 100m;
                    }
                    else
                    {
                        // A fixed relief tied to a deduction is only given to people
                        // who actually pay it; otherwise it goes to everyone.
                        if (!relief.AppliesToAll)
                        {
                            if (relief.DeductionTypeId == null) continue;
                            if (!deductionLines.Any(l => l.TypeId == relief.DeductionTypeId.Value)) continue;
                        }
                        amount = relief.Value;
                    }

                    if (relief.MonthlyCap.HasValue) amount = Math.Min(amount, relief.MonthlyCap.Value);
                    amount = Math.Round(amount, 2);

                    if (relief.Basis == ReliefBasis.PercentageOfDeduction) insuranceRelief += amount;
                    else empPersonalRelief += amount;
                }

                // j. Net PAYE
                decimal paye = Math.Max(0, grossTax - empPersonalRelief - insuranceRelief);
                paye = Math.Round(paye, 2);

                // j. Employer contributions - costs of employment, not deductions. Neither
                // touches the employee's pay or appears on the payslip; both go only to
                // the journal. NSSF is matched; the Housing Levy is its own rate on gross.
                decimal nssfEmployer = nssfEmployee;
                decimal ahlEmployer = Math.Round(grossPay * ahlEmployerRate, 2, MidpointRounding.AwayFromZero);

                // k. Other Deductions - salary structure lines plus applies-to-all types
                decimal otherDeductions = deductionLines.Sum(l => l.Amount);

                // l. Loan Deductions
                decimal loanDeductions = 0m;
                var activeLoans = await _unitOfWork.LoanAdvances.GetActiveByStaffId(staff.Id);
                var loanDeductionDetails = new List<(LoanAdvance Loan, decimal Amount)>();
                foreach (var loan in activeLoans)
                {
                    decimal deduction = Math.Min(loan.MonthlyDeduction, loan.Balance);
                    if (deduction > 0)
                    {
                        loanDeductions += deduction;
                        loanDeductionDetails.Add((loan, deduction));
                    }
                }

                // m. Total Deductions
                decimal totalDeductions = paye + nssfEmployee + shifAmount + ahl + otherDeductions + loanDeductions;

                // n. Net Pay, rounded as the school's policy says (Global Settings >
                // Payroll). The difference is kept as its own figure so the payslip
                // still adds up: gross - deductions + rounding = net.
                decimal unroundedNet = grossPay - totalDeductions;
                decimal netPay = RoundNetPay(unroundedNet, netPayRoundingUnit, netPayRoundingMethod);
                decimal roundingAdjustment = netPay - unroundedNet;

                // --- Create Payslip ---
                var payslip = new Payslip
                {
                    PayrollPeriodId = period.Id,
                    StaffDetailsId = staff.Id,
                    BasicSalary = basicSalary,
                    HouseAllowance = houseAllowance,
                    TransportAllowance = transportAllowance,
                    OtherAllowances = otherAllowances,
                    GrossPay = grossPay,
                    NssfTier1 = Math.Round(nssfTier1, 2),
                    NssfTier2 = Math.Round(nssfTier2, 2),
                    NssfEmployee = nssfEmployee,
                    TaxablePay = taxablePay,
                    RetirementRelief = retirementRelief,
                    OtherTaxDeductible = otherTaxDeductible,
                    TaxableIncome = taxableIncome,
                    GrossTax = grossTax,
                    PersonalRelief = empPersonalRelief,
                    InsuranceRelief = insuranceRelief,
                    Paye = paye,
                    Shif = shifAmount,
                    Ahl = ahl,
                    NssfEmployer = nssfEmployer,
                    AhlEmployer = ahlEmployer,
                    OtherDeductions = otherDeductions,
                    LoanDeductions = loanDeductions,
                    TotalDeductions = totalDeductions,
                    RoundingAdjustment = roundingAdjustment,
                    NetPay = netPay
                };
                _unitOfWork.Payslips.Create(payslip);
                await _unitOfWork.SaveChangesAsync(); // Save to get payslip Id

                // --- Create PayslipEarning records ---
                var basicType = EarningByCode("BASIC");
                if (basicType != null)
                    _unitOfWork.PayslipEarnings.Create(new PayslipEarning
                    { PayslipId = payslip.Id, EarningTypeId = basicType.Id, Amount = basicSalary });

                var hseType = EarningByCode("HSEALL");
                if (hseType != null && houseAllowance > 0)
                    _unitOfWork.PayslipEarnings.Create(new PayslipEarning
                    { PayslipId = payslip.Id, EarningTypeId = hseType.Id, Amount = houseAllowance });

                var trnType = EarningByCode("TRNALL");
                if (trnType != null && transportAllowance > 0)
                    _unitOfWork.PayslipEarnings.Create(new PayslipEarning
                    { PayslipId = payslip.Id, EarningTypeId = trnType.Id, Amount = transportAllowance });

                // Earning lines - salary structure plus applies-to-all types
                foreach (var line in earningLines)
                {
                    _unitOfWork.PayslipEarnings.Create(new PayslipEarning
                    { PayslipId = payslip.Id, EarningTypeId = line.TypeId, Amount = line.Amount });
                }

                // --- Create PayslipDeduction records ---
                var payeType = DeductionByCode("PAYE");
                if (payeType != null && paye > 0)
                    _unitOfWork.PayslipDeductions.Create(new PayslipDeduction
                    { PayslipId = payslip.Id, DeductionTypeId = payeType.Id, Amount = paye });

                var nssfType = DeductionByCode("NSSF");
                if (nssfType != null && nssfEmployee > 0)
                    _unitOfWork.PayslipDeductions.Create(new PayslipDeduction
                    { PayslipId = payslip.Id, DeductionTypeId = nssfType.Id, Amount = nssfEmployee });

                var shifType = DeductionByCode("SHIF");
                if (shifType != null && shifAmount > 0)
                    _unitOfWork.PayslipDeductions.Create(new PayslipDeduction
                    { PayslipId = payslip.Id, DeductionTypeId = shifType.Id, Amount = shifAmount });

                var ahlType = DeductionByCode("AHL");
                if (ahlType != null && ahl > 0)
                    _unitOfWork.PayslipDeductions.Create(new PayslipDeduction
                    { PayslipId = payslip.Id, DeductionTypeId = ahlType.Id, Amount = ahl });

                // Deduction lines - salary structure plus applies-to-all types
                foreach (var line in deductionLines)
                {
                    _unitOfWork.PayslipDeductions.Create(new PayslipDeduction
                    { PayslipId = payslip.Id, DeductionTypeId = line.TypeId, Amount = line.Amount });
                }

                // Loan deduction records
                var loanType = DeductionByCode("LOAN");
                foreach (var (loan, amount) in loanDeductionDetails)
                {
                    if (loanType != null)
                        _unitOfWork.PayslipDeductions.Create(new PayslipDeduction
                        {
                            PayslipId = payslip.Id,
                            DeductionTypeId = loanType.Id,
                            Amount = amount,
                            // Recorded so a re-run can credit this instalment back.
                            LoanAdvanceId = loan.Id
                        });

                    // Reduce loan balance
                    loan.Balance -= amount;
                    if (loan.Balance <= 0)
                    {
                        loan.Balance = 0;
                        loan.Status = LoanStatus.FullyPaid;
                    }
                    _unitOfWork.LoanAdvances.Update(loan);
                }
            }

            // Update period status
            period.Status = PayrollPeriodStatus.Processed;
            period.ProcessedDate = DateTime.UtcNow;
            _unitOfWork.PayrollPeriods.Update(period);
            await _unitOfWork.SaveChangesAsync();

            return Ok(new { message = "Payroll processed successfully.", periodId = period.Id });
        }

        // POST api/payrollPeriods/5/approve
        // A single-click approval, for a school that has switched the payroll approval workflow off. With the
        // workflow on, a payroll is submitted for approval instead and its final approver approves it.
        [HttpPost("{id}/approve")]
        public async Task<IActionResult> Approve(int id)
        {
            if (await _approval.WorkflowActiveAsync())
                return BadRequest("Payroll is approved through its approval workflow: submit it for approval instead.");
            var (ok, message) = await _approval.ApproveAsync(id, null);
            return ok ? Ok(new { message }) : BadRequest(message);
        }

        // GET api/payrollPeriods/p9?year=2026&staffId=5
        /// <summary>
        /// The P9A tax deduction card: each employee's pay, deductions and PAYE month by month for the year,
        /// in KRA's columns, from approved payroll only. Without staffId, everyone paid that year.
        /// </summary>
        [HttpGet("p9")]
        public async Task<IActionResult> P9(int year, int? staffId)
        {
            var settings = (await _unitOfWork.PayrollSettings.Find(s => s.IsActive)).ToList();
            decimal Setting(string key) => settings.FirstOrDefault(s => s.Key == key)?.Value ?? 0m;
            var percent = Setting("RetirementReliefPercent");
            var cap = Setting("RetirementReliefCap");
            var retirementTypeIds = (await _unitOfWork.DeductionTypes.Find(d => d.IsRetirementContribution))
                .Select(d => d.Id).ToHashSet();

            var slips = await _db.Payslips.AsNoTracking()
                .Include(p => p.PayrollPeriod).Include(p => p.StaffDetails).Include(p => p.Deductions)
                .Where(p => p.PayrollPeriod!.Year == year
                            && (p.PayrollPeriod.Status == PayrollPeriodStatus.Approved || p.PayrollPeriod.Status == PayrollPeriodStatus.Posted)
                            && (staffId == null || p.StaffDetailsId == staffId))
                .ToListAsync();

            var employees = slips.GroupBy(p => p.StaffDetailsId).Select(g =>
            {
                var staff = g.First().StaffDetails!;
                var rows = g.OrderBy(p => p.PayrollPeriod!.Month).Select(p => new
                {
                    month = p.PayrollPeriod!.Month,
                    basic = p.BasicSalary,
                    // Taxable earnings other than basic pay: allowances and benefits.
                    benefits = p.TaxablePay - p.BasicSalary,
                    quarters = 0m,
                    gross = p.TaxablePay,
                    // E: retirement contributions - the limit as a share of basic, what was paid, the fixed cap.
                    e1 = Math.Round(p.BasicSalary * percent / 100m, 2),
                    e2 = p.NssfEmployee + p.Deductions.Where(d => retirementTypeIds.Contains(d.DeductionTypeId)).Sum(d => d.Amount),
                    e3 = cap,
                    ownerOccupiedInterest = 0m,
                    retirement = p.RetirementRelief,
                    ahl = p.Ahl,
                    shif = p.Shif,
                    otherDeductible = p.OtherTaxDeductible,
                    chargeable = p.TaxableIncome,
                    taxCharged = p.GrossTax,
                    personalRelief = p.PersonalRelief,
                    insuranceRelief = p.InsuranceRelief,
                    paye = p.Paye
                }).ToList();
                return new
                {
                    staffId = staff.Id,
                    name = staff.FullName,
                    staffNo = staff.UPI,
                    kraPin = staff.KraPinNo,
                    idNumber = staff.IdNumber,
                    rows
                };
            }).OrderBy(e => e.name).ToList();

            return Ok(new { year, employees });
        }

        /// <summary>
        /// Net pay rounding from Global Settings > Payroll. The unit is 0 (keep cents),
        /// 1 (whole shillings), 5, 10, 50 or 100; the method is nearest, down or up.
        /// Missing or unreadable values fall back to whole shillings, nearest - the
        /// same defaults the settings page shows.
        /// </summary>
        private async Task<(decimal Unit, string Method)> GetNetPayRounding()
        {
            var settings = (await _unitOfWork.Repository<SchoolWebApp.Core.Entities.Settings.GlobalSetting>()
                .Find(s => s.Module == "Payroll")).ToList();
            string? Value(string key) => settings.FirstOrDefault(s => s.SettingKey == key)?.SettingValue?.Trim();

            var unit = decimal.TryParse(Value("NetPayRoundingUnit"), System.Globalization.NumberStyles.Number,
                System.Globalization.CultureInfo.InvariantCulture, out var u) && u >= 0 ? u : 1m;
            var method = (Value("NetPayRoundingMethod") ?? "").ToLowerInvariant();
            if (method != "down" && method != "up") method = "nearest";
            return (unit, method);
        }

        // DELETE api/payrollPeriods/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var period = await _unitOfWork.PayrollPeriods.GetByIdWithPayslips(id);
            if (period == null) return NotFound();

            if (period.Status != PayrollPeriodStatus.Draft)
                return BadRequest("Only draft payroll periods can be deleted.");
            var locked = await _approval.LockedReasonAsync(id);
            if (locked != null) return BadRequest(locked);

            // Delete any payslips and their line items
            foreach (var payslip in period.Payslips)
            {
                foreach (var e in payslip.Earnings)
                    _unitOfWork.PayslipEarnings.Delete(e);
                foreach (var d in payslip.Deductions)
                    _unitOfWork.PayslipDeductions.Delete(d);
                _unitOfWork.Payslips.Delete(payslip);
            }

            _unitOfWork.PayrollPeriods.Delete(period);
            await _unitOfWork.SaveChangesAsync();
            return Ok();
        }
    }
}
