using System.ComponentModel.DataAnnotations;

namespace SchoolWebApp.Core.DTOs.Payroll
{
    // --- Earning Type ---
    public class CreateEarningTypeDto
    {
        [Required, StringLength(255)] public string Name { get; set; } = string.Empty;
        [Required, StringLength(50)] public string Code { get; set; } = string.Empty;
        public bool IsTaxable { get; set; } = true;
        public bool IsActive { get; set; } = true;
        /// <summary>0 = fixed amount, 1 = percentage of basic salary.</summary>
        public int CalculationMethod { get; set; }
        /// <summary>The fixed amount or the percentage, per CalculationMethod.</summary>
        public decimal? DefaultValue { get; set; }
        /// <summary>Apply to every employee without assigning it to each one.</summary>
        public bool AppliesToAll { get; set; }
        /// <summary>Payroll accounts for this itself; it is never applied as a configured line.</summary>
        public bool IsSystemComputed { get; set; }
        [StringLength(500)] public string? Description { get; set; }
    }
    public class EarningTypeDto : CreateEarningTypeDto { public int Id { get; set; } }

    // --- Deduction Type ---
    public class CreateDeductionTypeDto
    {
        [Required, StringLength(255)] public string Name { get; set; } = string.Empty;
        [Required, StringLength(50)] public string Code { get; set; } = string.Empty;
        public bool IsStatutory { get; set; }
        public bool IsActive { get; set; } = true;
        /// <summary>0 = fixed amount, 1 = percentage of gross pay.</summary>
        public int CalculationMethod { get; set; }
        /// <summary>The fixed amount or the percentage, per CalculationMethod.</summary>
        public decimal? DefaultValue { get; set; }
        /// <summary>Apply to every employee without assigning it to each one.</summary>
        public bool AppliesToAll { get; set; }
        /// <summary>Payroll computes this itself; it is never applied as a configured line.</summary>
        public bool IsSystemComputed { get; set; }
        /// <summary>Allowable against taxable income, so taken off gross before PAYE.</summary>
        public bool IsTaxDeductible { get; set; }
        /// <summary>Monthly ceiling on the deductible amount; null means uncapped.</summary>
        public decimal? TaxDeductibleCap { get; set; }
        /// <summary>Pooled with NSSF under the retirement relief limits.</summary>
        public bool IsRetirementContribution { get; set; }
        /// <summary>Liability account credited when payroll posts to the GL; null uses the general one.</summary>
        public int? LiabilityAccountId { get; set; }
        [StringLength(500)] public string? Description { get; set; }
    }
    public class DeductionTypeDto : CreateDeductionTypeDto { public int Id { get; set; } }

    // --- NSSF Band ---
    public class CreateNssfBandDto
    {
        [Required, StringLength(255)] public string Name { get; set; } = string.Empty;
        public int Tier { get; set; }
        public decimal LowerLimit { get; set; }
        public decimal UpperLimit { get; set; }
        public decimal Rate { get; set; }
        public DateTime EffectiveDate { get; set; }
        public bool IsActive { get; set; } = true;
        [StringLength(500)] public string? Description { get; set; }
    }
    public class NssfBandDto : CreateNssfBandDto { public int Id { get; set; } }

    // --- Payroll Relief ---
    public class CreatePayrollReliefDto
    {
        [Required, StringLength(255)] public string Name { get; set; } = string.Empty;
        [Required, StringLength(50)] public string Code { get; set; } = string.Empty;
        /// <summary>0 = fixed amount, 1 = percentage of a deduction.</summary>
        public int Basis { get; set; }
        public int? DeductionTypeId { get; set; }
        public decimal Value { get; set; }
        public decimal? MonthlyCap { get; set; }
        public bool AppliesToAll { get; set; }
        public bool IsActive { get; set; } = true;
        [StringLength(500)] public string? Description { get; set; }
    }
    public class PayrollReliefDto : CreatePayrollReliefDto
    {
        public int Id { get; set; }
        public string? DeductionTypeName { get; set; }
    }

    // --- Tax Band ---
    public class CreateTaxBandDto
    {
        [StringLength(255)] public string? Description { get; set; }
        public decimal LowerLimit { get; set; }
        public decimal UpperLimit { get; set; }
        public decimal Rate { get; set; }
        public DateTime EffectiveDate { get; set; }
        public bool IsActive { get; set; } = true;
    }
    public class TaxBandDto : CreateTaxBandDto { public int Id { get; set; } }

    // --- Payroll Setting ---
    public class CreatePayrollSettingDto
    {
        [Required, StringLength(100)] public string Key { get; set; } = string.Empty;
        [Required, StringLength(255)] public string Name { get; set; } = string.Empty;
        public decimal Value { get; set; }
        [StringLength(50)] public string? Category { get; set; }
        [StringLength(500)] public string? Description { get; set; }
        public DateTime EffectiveDate { get; set; }
        public bool IsActive { get; set; } = true;
    }
    public class PayrollSettingDto : CreatePayrollSettingDto { public int Id { get; set; } }

    // --- Employee Salary ---
    public class CreateEmployeeSalaryDto
    {
        public int StaffDetailsId { get; set; }
        public decimal BasicSalary { get; set; }
        public decimal HouseAllowance { get; set; }
        public decimal TransportAllowance { get; set; }
        public decimal OtherAllowances { get; set; }
        public DateTime EffectiveDate { get; set; }
        public bool IsActive { get; set; } = true;
        [StringLength(500)] public string? Notes { get; set; }
        public List<EmployeeSalaryItemDto> Items { get; set; } = new();
    }
    public class EmployeeSalaryDto : CreateEmployeeSalaryDto
    {
        public int Id { get; set; }
        public string? StaffName { get; set; }
        public string? StaffUpi { get; set; }
        public decimal TotalEarnings { get; set; }
    }
    public class EmployeeSalaryItemDto
    {
        public int? Id { get; set; }
        public int? EarningTypeId { get; set; }
        public string? EarningTypeName { get; set; }
        public int? DeductionTypeId { get; set; }
        public string? DeductionTypeName { get; set; }
        public decimal Amount { get; set; }
    }

    // --- Loan / Advance ---
    public class CreateLoanAdvanceDto
    {
        public int StaffDetailsId { get; set; }
        [Required, StringLength(255)] public string Description { get; set; } = string.Empty;
        public decimal PrincipalAmount { get; set; }
        public decimal MonthlyDeduction { get; set; }
        public decimal Balance { get; set; }
        public DateTime IssueDate { get; set; }
        public int Status { get; set; } = 1;
        [StringLength(500)] public string? Notes { get; set; }
    }
    public class LoanAdvanceDto : CreateLoanAdvanceDto
    {
        public int Id { get; set; }
        public string? StaffName { get; set; }
    }

    // --- Payroll Period ---
    public class CreatePayrollPeriodDto
    {
        public int Month { get; set; }
        public int Year { get; set; }
        [StringLength(100)] public string? Name { get; set; }
    }
    public class PayrollPeriodDto
    {
        public int Id { get; set; }
        public int Month { get; set; }
        public int Year { get; set; }
        public string? Name { get; set; }
        public int Status { get; set; }
        public string? StatusLabel { get; set; }
        public DateTime? ProcessedDate { get; set; }
        public DateTime? ApprovedDate { get; set; }
        public DateTime? PostedDate { get; set; }
        public int PayslipCount { get; set; }
        public decimal TotalGross { get; set; }
        public decimal TotalNet { get; set; }
        public decimal TotalPaye { get; set; }
        public decimal TotalNssf { get; set; }
        public decimal TotalShif { get; set; }
    }

    // --- Payslip ---
    public class PayslipDto
    {
        public int Id { get; set; }
        public int PayrollPeriodId { get; set; }
        public string? PeriodName { get; set; }
        public int StaffDetailsId { get; set; }
        public string? StaffName { get; set; }
        public string? StaffUpi { get; set; }
        public string? KraPin { get; set; }
        public string? NssfNumber { get; set; }
        public string? IdNumber { get; set; }
        public string? ShaNumber { get; set; }
        /// <summary>
        /// True where the staff member has since been taken off the payroll. The
        /// payslip already exists for this period, but a re-run will drop them.
        /// </summary>
        public bool ExcludeFromPayroll { get; set; }
        public string? DesignationName { get; set; }
        public string? DepartmentName { get; set; }
        public string? BankName { get; set; }
        public string? BankAccountNumber { get; set; }

        public decimal BasicSalary { get; set; }
        public decimal HouseAllowance { get; set; }
        public decimal TransportAllowance { get; set; }
        public decimal OtherAllowances { get; set; }
        public decimal GrossPay { get; set; }
        public decimal NssfTier1 { get; set; }
        public decimal NssfTier2 { get; set; }
        public decimal NssfEmployee { get; set; }
        public decimal TaxablePay { get; set; }
        public decimal RetirementRelief { get; set; }
        public decimal OtherTaxDeductible { get; set; }
        public decimal TaxableIncome { get; set; }
        public decimal GrossTax { get; set; }
        public decimal PersonalRelief { get; set; }
        public decimal InsuranceRelief { get; set; }
        public decimal Paye { get; set; }
        public decimal Shif { get; set; }
        public decimal Ahl { get; set; }
        public decimal NssfEmployer { get; set; }
        // The employer's matching Housing Levy. Never shown on a payslip - it is an
        // employer cost - but reported for remittance with the employee share.
        public decimal AhlEmployer { get; set; }
        public decimal OtherDeductions { get; set; }
        public decimal LoanDeductions { get; set; }
        public decimal TotalDeductions { get; set; }
        public decimal RoundingAdjustment { get; set; }
        public decimal NetPay { get; set; }

        public List<PayslipLineDto> Earnings { get; set; } = new();
        public List<PayslipLineDto> Deductions { get; set; } = new();
    }
    public class PayslipLineDto
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Code { get; set; }
        public decimal Amount { get; set; }
        /// <summary>Earning lines only: false where the earning is left out of taxable pay.</summary>
        public bool IsTaxable { get; set; } = true;
    }
}
