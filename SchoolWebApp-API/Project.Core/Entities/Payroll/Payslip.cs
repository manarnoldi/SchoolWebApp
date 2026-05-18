using SchoolWebApp.Core.Entities.Shared;
using SchoolWebApp.Core.Entities.Staff;

namespace SchoolWebApp.Core.Entities.Payroll
{
    public class Payslip : Base
    {
        public int PayrollPeriodId { get; set; }
        public PayrollPeriod? PayrollPeriod { get; set; }

        public int StaffDetailsId { get; set; }
        public StaffDetails? StaffDetails { get; set; }

        // Earnings breakdown
        public decimal BasicSalary { get; set; }
        public decimal HouseAllowance { get; set; }
        public decimal TransportAllowance { get; set; }
        public decimal OtherAllowances { get; set; }
        public decimal GrossPay { get; set; }

        // Statutory deductions (Kenyan)
        public decimal NssfEmployee { get; set; }
        public decimal TaxableIncome { get; set; }
        public decimal GrossTax { get; set; }
        public decimal PersonalRelief { get; set; }
        public decimal InsuranceRelief { get; set; }
        public decimal Paye { get; set; }
        public decimal Shif { get; set; }
        public decimal Ahl { get; set; }
        public decimal NssfEmployer { get; set; }

        // Other deductions
        public decimal OtherDeductions { get; set; }
        public decimal LoanDeductions { get; set; }

        // Net
        public decimal TotalDeductions { get; set; }
        public decimal NetPay { get; set; }

        public List<PayslipEarning> Earnings { get; set; } = new();
        public List<PayslipDeduction> Deductions { get; set; } = new();
    }

    public class PayslipEarning : Base
    {
        public int PayslipId { get; set; }
        public Payslip? Payslip { get; set; }

        public int EarningTypeId { get; set; }
        public EarningType? EarningType { get; set; }

        public decimal Amount { get; set; }
    }

    public class PayslipDeduction : Base
    {
        public int PayslipId { get; set; }
        public Payslip? Payslip { get; set; }

        public int DeductionTypeId { get; set; }
        public DeductionType? DeductionType { get; set; }

        public decimal Amount { get; set; }
    }
}
