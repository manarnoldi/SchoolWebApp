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
        // Tier I and Tier II are stored as computed, not just their sum: the
        // payslip prints them as separate lines, and the ceilings/rates they
        // derive from can change after a period is closed.
        public decimal NssfTier1 { get; set; }
        public decimal NssfTier2 { get; set; }
        public decimal NssfEmployee { get; set; }

        // The PAYE working, kept so the payslip can show how taxable income was
        // reached rather than just the answer.
        /// <summary>Gross pay less any earnings marked non-taxable.</summary>
        public decimal TaxablePay { get; set; }
        /// <summary>NSSF plus retirement contributions, as allowed after the limits.</summary>
        public decimal RetirementRelief { get; set; }
        /// <summary>Other deductions ticked tax deductible, after their own caps.</summary>
        public decimal OtherTaxDeductible { get; set; }
        public decimal TaxableIncome { get; set; }
        public decimal GrossTax { get; set; }
        public decimal PersonalRelief { get; set; }
        public decimal InsuranceRelief { get; set; }
        public decimal Paye { get; set; }
        public decimal Shif { get; set; }
        public decimal Ahl { get; set; }
        public decimal NssfEmployer { get; set; }

        /// <summary>
        /// The employer's matching Affordable Housing Levy. An employer cost, not a
        /// deduction: it is never shown on the payslip, and is kept here only so the
        /// payroll journal can charge it to salary expense and credit the levy payable.
        /// </summary>
        public decimal AhlEmployer { get; set; }

        // Other deductions
        public decimal OtherDeductions { get; set; }
        public decimal LoanDeductions { get; set; }

        // Net
        public decimal TotalDeductions { get; set; }
        /// <summary>
        /// What rounding net pay to the configured unit added (or took off), so the
        /// payslip still adds up: gross - deductions + rounding = net.
        /// </summary>
        public decimal RoundingAdjustment { get; set; }
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

        // Set only on loan instalment rows. Processing reduces the loan balance,
        // so re-processing a period has to know which loan each instalment came
        // from in order to add it back before rebuilding the payslips.
        public int? LoanAdvanceId { get; set; }
        public LoanAdvance? LoanAdvance { get; set; }
    }
}
