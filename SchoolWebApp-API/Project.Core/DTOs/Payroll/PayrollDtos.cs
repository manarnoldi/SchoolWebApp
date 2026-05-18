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
        [StringLength(500)] public string? Description { get; set; }
    }
    public class DeductionTypeDto : CreateDeductionTypeDto { public int Id { get; set; } }

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
        public int StaffDetailsId { get; set; }
        public string? StaffName { get; set; }
        public string? StaffUpi { get; set; }
        public string? KraPin { get; set; }
        public string? NssfNumber { get; set; }
        public string? DesignationName { get; set; }
        public string? DepartmentName { get; set; }
        public string? BankName { get; set; }
        public string? BankAccountNumber { get; set; }

        public decimal BasicSalary { get; set; }
        public decimal HouseAllowance { get; set; }
        public decimal TransportAllowance { get; set; }
        public decimal OtherAllowances { get; set; }
        public decimal GrossPay { get; set; }
        public decimal NssfEmployee { get; set; }
        public decimal TaxableIncome { get; set; }
        public decimal GrossTax { get; set; }
        public decimal PersonalRelief { get; set; }
        public decimal InsuranceRelief { get; set; }
        public decimal Paye { get; set; }
        public decimal Shif { get; set; }
        public decimal Ahl { get; set; }
        public decimal NssfEmployer { get; set; }
        public decimal OtherDeductions { get; set; }
        public decimal LoanDeductions { get; set; }
        public decimal TotalDeductions { get; set; }
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
    }
}
