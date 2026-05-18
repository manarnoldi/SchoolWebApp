using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolWebApp.Core.DTOs.Payroll;
using SchoolWebApp.Core.Entities.Enums;
using SchoolWebApp.Core.Entities.Payroll;
using SchoolWebApp.Core.Entities.Finance;
using SchoolWebApp.Core.Interfaces.IRepositories;
using System.Globalization;

namespace SchoolWebApp.API.Controllers.Payroll
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class PayrollPeriodsController : ControllerBase
    {
        private readonly ILogger<PayrollPeriodsController> _logger;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public PayrollPeriodsController(ILogger<PayrollPeriodsController> logger, IUnitOfWork unitOfWork, IMapper mapper)
        {
            _logger = logger; _unitOfWork = unitOfWork; _mapper = mapper;
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

            // --- Load reference data ---
            var settings = (await _unitOfWork.PayrollSettings.Find(s => s.IsActive)).ToList();
            var taxBands = (await _unitOfWork.TaxBands.Find(t => t.IsActive))
                .OrderBy(t => t.LowerLimit).ToList();
            var earningTypes = (await _unitOfWork.EarningTypes.Find(e => e.IsActive)).ToList();
            var deductionTypes = (await _unitOfWork.DeductionTypes.Find(d => d.IsActive)).ToList();

            // Helper to read a payroll setting by key
            decimal Setting(string key) => settings.FirstOrDefault(s => s.Key == key)?.Value ?? 0m;

            decimal nssfTier1Ceiling = Setting("NssfTier1Ceiling");
            decimal nssfTier1Rate = Setting("NssfTier1Rate") / 100m;
            decimal nssfTier2Ceiling = Setting("NssfTier2Ceiling");
            decimal nssfTier2Rate = Setting("NssfTier2Rate") / 100m;
            decimal personalRelief = Setting("PersonalRelief");
            decimal shifRate = Setting("ShifRate") / 100m;
            decimal ahlRate = Setting("AhlRate") / 100m;
            decimal insuranceReliefCap = Setting("InsuranceReliefCap");

            // Lookup helpers for earning/deduction types by code
            EarningType? EarningByCode(string code) => earningTypes.FirstOrDefault(e => e.Code == code);
            DeductionType? DeductionByCode(string code) => deductionTypes.FirstOrDefault(d => d.Code == code);

            // --- If re-processing, delete existing payslips ---
            if (period.Payslips.Any())
            {
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

            // --- Get active staff ---
            var activeStaff = await _unitOfWork.StaffDetails.SearchForStaff(null, null, Status.Active);

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

                // Sum earning items from salary structure
                decimal salaryEarningItems = salary.Items
                    .Where(i => i.EarningTypeId != null)
                    .Sum(i => i.Amount);

                decimal grossPay = basicSalary + houseAllowance + transportAllowance
                    + otherAllowances + salaryEarningItems;

                // b. NSSF Employee (tiered)
                decimal nssfTier1 = Math.Min(grossPay, nssfTier1Ceiling) * nssfTier1Rate;
                decimal nssfTier2 = Math.Max(0, Math.Min(grossPay, nssfTier2Ceiling) - nssfTier1Ceiling) * nssfTier2Rate;
                decimal nssfEmployee = Math.Round(nssfTier1 + nssfTier2, 2);

                // c. Taxable Income
                decimal taxableIncome = grossPay - nssfEmployee;

                // d. Gross Tax (PAYE) - progressive tax bands
                decimal grossTax = 0m;
                foreach (var band in taxBands)
                {
                    if (taxableIncome <= band.LowerLimit) break;
                    decimal taxableInBand = Math.Min(taxableIncome, band.UpperLimit) - band.LowerLimit;
                    if (taxableInBand > 0)
                        grossTax += taxableInBand * (band.Rate / 100m);
                }
                grossTax = Math.Round(grossTax, 2);

                // e. Personal Relief
                decimal empPersonalRelief = personalRelief;

                // f. Insurance Relief (SHIF-based)
                decimal shifAmount = Math.Round(grossPay * shifRate, 2);
                decimal insuranceRelief = 0m;
                if (shifAmount > 0)
                {
                    insuranceRelief = Math.Min(shifAmount * 0.15m, insuranceReliefCap);
                    insuranceRelief = Math.Round(insuranceRelief, 2);
                }

                // g. Net PAYE
                decimal paye = Math.Max(0, grossTax - empPersonalRelief - insuranceRelief);
                paye = Math.Round(paye, 2);

                // h. SHIF (already computed above)

                // i. AHL (Affordable Housing Levy)
                decimal ahl = Math.Round(grossPay * ahlRate, 2);

                // j. NSSF Employer (matches employee)
                decimal nssfEmployer = nssfEmployee;

                // k. Other Deductions from salary structure
                decimal otherDeductions = salary.Items
                    .Where(i => i.DeductionTypeId != null)
                    .Sum(i => i.Amount);

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

                // n. Net Pay
                decimal netPay = grossPay - totalDeductions;

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
                    NssfEmployee = nssfEmployee,
                    TaxableIncome = taxableIncome,
                    GrossTax = grossTax,
                    PersonalRelief = empPersonalRelief,
                    InsuranceRelief = insuranceRelief,
                    Paye = paye,
                    Shif = shifAmount,
                    Ahl = ahl,
                    NssfEmployer = nssfEmployer,
                    OtherDeductions = otherDeductions,
                    LoanDeductions = loanDeductions,
                    TotalDeductions = totalDeductions,
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

                // Salary structure earning items
                foreach (var item in salary.Items.Where(i => i.EarningTypeId != null))
                {
                    _unitOfWork.PayslipEarnings.Create(new PayslipEarning
                    { PayslipId = payslip.Id, EarningTypeId = item.EarningTypeId!.Value, Amount = item.Amount });
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

                // Salary structure deduction items
                foreach (var item in salary.Items.Where(i => i.DeductionTypeId != null))
                {
                    _unitOfWork.PayslipDeductions.Create(new PayslipDeduction
                    { PayslipId = payslip.Id, DeductionTypeId = item.DeductionTypeId!.Value, Amount = item.Amount });
                }

                // Loan deduction records
                var loanType = DeductionByCode("LOAN");
                foreach (var (loan, amount) in loanDeductionDetails)
                {
                    if (loanType != null)
                        _unitOfWork.PayslipDeductions.Create(new PayslipDeduction
                        { PayslipId = payslip.Id, DeductionTypeId = loanType.Id, Amount = amount });

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
        [HttpPost("{id}/approve")]
        public async Task<IActionResult> Approve(int id)
        {
            var period = await _unitOfWork.PayrollPeriods.GetById(id);
            if (period == null) return NotFound();

            if (period.Status != PayrollPeriodStatus.Processed)
                return BadRequest("Only processed payroll periods can be approved.");

            period.Status = PayrollPeriodStatus.Approved;
            period.ApprovedDate = DateTime.UtcNow;
            _unitOfWork.PayrollPeriods.Update(period);
            await _unitOfWork.SaveChangesAsync();

            await AutoPostPayrollJournal(period);

            return Ok(new { message = "Payroll period approved and posted to GL." });
        }

        private async Task<int?> GetSettingAccountId(string key)
        {
            var settings = await _unitOfWork.Repository<SchoolWebApp.Core.Entities.Settings.GlobalSetting>()
                .Find(s => s.Module == "Finance" && s.SettingKey == key);
            var val = settings.FirstOrDefault()?.SettingValue;
            return int.TryParse(val, out var id) ? id : null;
        }

        private async Task AutoPostPayrollJournal(PayrollPeriod period)
        {
            var fullPeriod = await _unitOfWork.PayrollPeriods.GetByIdWithPayslips(period.Id);
            if (fullPeriod == null || fullPeriod.Payslips.Count == 0) return;

            var salaryExpId = await GetSettingAccountId("SalaryExpenseAccountId");
            var cashId = await GetSettingAccountId("CashAccountId");
            var payeId = await GetSettingAccountId("PayeAccountId");
            var nssfId = await GetSettingAccountId("NssfAccountId");
            var shifId = await GetSettingAccountId("ShifAccountId");
            var ahlId = await GetSettingAccountId("AhlAccountId");

            if (salaryExpId == null || cashId == null) return;

            var totalGross = fullPeriod.Payslips.Sum(p => p.GrossPay);
            var totalNssfEe = fullPeriod.Payslips.Sum(p => p.NssfEmployee);
            var totalNssfEr = fullPeriod.Payslips.Sum(p => p.NssfEmployer);
            var totalPaye = fullPeriod.Payslips.Sum(p => p.Paye);
            var totalShif = fullPeriod.Payslips.Sum(p => p.Shif);
            var totalAhl = fullPeriod.Payslips.Sum(p => p.Ahl);
            var totalNet = fullPeriod.Payslips.Sum(p => p.NetPay);

            var journal = new JournalEntry
            {
                ReferenceNumber = $"PAY-JNL-{fullPeriod.Name?.Replace(" ", "-") ?? $"{fullPeriod.Month}-{fullPeriod.Year}"}",
                EntryDate = fullPeriod.ApprovedDate ?? DateTime.Now,
                Description = $"Auto-posted: Payroll {fullPeriod.Name}",
                IsPosted = true,
                Status = JournalEntryStatus.Approved,
                Lines = new List<JournalLine>()
            };

            // Debit: Salary Expense (gross + employer NSSF)
            journal.Lines.Add(new JournalLine
            {
                AccountId = salaryExpId.Value,
                Debit = totalGross + totalNssfEr,
                Credit = 0,
                Description = $"Salary expense - {fullPeriod.Name}"
            });

            // Credit: Cash/Bank (net pay)
            journal.Lines.Add(new JournalLine
            {
                AccountId = cashId.Value,
                Debit = 0,
                Credit = totalNet,
                Description = $"Net salaries paid - {fullPeriod.Name}"
            });

            // Credit: PAYE liability
            if (totalPaye > 0 && payeId != null)
                journal.Lines.Add(new JournalLine
                {
                    AccountId = payeId.Value,
                    Debit = 0, Credit = totalPaye,
                    Description = $"PAYE payable - {fullPeriod.Name}"
                });

            // Credit: NSSF liability (employee + employer)
            if ((totalNssfEe + totalNssfEr) > 0 && nssfId != null)
                journal.Lines.Add(new JournalLine
                {
                    AccountId = nssfId.Value,
                    Debit = 0, Credit = totalNssfEe + totalNssfEr,
                    Description = $"NSSF payable - {fullPeriod.Name}"
                });

            // Credit: SHIF liability
            if (totalShif > 0 && shifId != null)
                journal.Lines.Add(new JournalLine
                {
                    AccountId = shifId.Value,
                    Debit = 0, Credit = totalShif,
                    Description = $"SHIF payable - {fullPeriod.Name}"
                });

            // Credit: AHL liability
            if (totalAhl > 0 && ahlId != null)
                journal.Lines.Add(new JournalLine
                {
                    AccountId = ahlId.Value,
                    Debit = 0, Credit = totalAhl,
                    Description = $"AHL payable - {fullPeriod.Name}"
                });

            _unitOfWork.JournalEntries.Create(journal);
            await _unitOfWork.SaveChangesAsync();
        }

        // DELETE api/payrollPeriods/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var period = await _unitOfWork.PayrollPeriods.GetByIdWithPayslips(id);
            if (period == null) return NotFound();

            if (period.Status != PayrollPeriodStatus.Draft)
                return BadRequest("Only draft payroll periods can be deleted.");

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
