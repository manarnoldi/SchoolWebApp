using SchoolWebApp.Core.Entities.Shared;
using System.ComponentModel.DataAnnotations;

namespace SchoolWebApp.Core.Entities.Payroll
{
    public class DeductionType : Base
    {
        [Required, StringLength(255)]
        public string Name { get; set; } = string.Empty;

        [Required, StringLength(50)]
        public string Code { get; set; } = string.Empty;

        public bool IsStatutory { get; set; }
        public bool IsActive { get; set; } = true;

        /// <summary>
        /// How the amount is worked out when the deduction is applied from the type
        /// rather than entered per employee.
        /// </summary>
        public PayrollCalculationMethod CalculationMethod { get; set; } = PayrollCalculationMethod.FixedAmount;

        /// <summary>
        /// The fixed amount, or the percentage of gross pay, depending on
        /// CalculationMethod. Ignored when the employee has their own line.
        /// </summary>
        public decimal? DefaultValue { get; set; }

        /// <summary>
        /// Apply to every employee on the payroll without assigning it to each one.
        /// An employee who has their own line for this deduction uses that line's
        /// amount instead, which is how an individual gets a different figure.
        /// </summary>
        public bool AppliesToAll { get; set; }

        /// <summary>
        /// Payroll computes this one itself from the statutory rules and rates
        /// (PAYE, NSSF, SHIF, AHL). Such a type is never applied as a configured
        /// line, or it would be counted twice.
        /// </summary>
        public bool IsSystemComputed { get; set; }

        /// <summary>
        /// The contribution is an allowable deduction against taxable income, so it
        /// is taken off gross pay before PAYE is worked out (pension, for example).
        /// Set from the Deduction Types screen so a change in the law needs no code
        /// change.
        /// </summary>
        public bool IsTaxDeductible { get; set; }

        /// <summary>
        /// Monthly ceiling on how much of this deduction may be taken off taxable
        /// income. Null means uncapped. Only consulted when IsTaxDeductible is set.
        /// </summary>
        public decimal? TaxDeductibleCap { get; set; }

        /// <summary>
        /// A contribution to a registered retirement scheme (pension, provident
        /// fund). These are not relieved one type at a time: they are pooled with
        /// the employee's NSSF and the pool is allowed at the lowest of the actual
        /// total, a percentage of basic pay and a monthly cap - the
        /// RetirementReliefPercent and RetirementReliefCap payroll settings. A type
        /// marked here is handled by that pool and never also by IsTaxDeductible.
        /// </summary>
        public bool IsRetirementContribution { get; set; }

        /// <summary>
        /// The liability account the deducted money is credited to when an approved
        /// payroll is posted to the GL - the pension scheme payable, the SACCO
        /// payable, and so on, since each is owed to a different party. Null falls
        /// back to the general PayrollDeductionsAccountId finance setting. Not used
        /// for system-computed statutory deductions, which have their own settings.
        /// </summary>
        public int? LiabilityAccountId { get; set; }
        public Finance.Account? LiabilityAccount { get; set; }

        // A deduction that attracts a relief is named by the relief itself - see
        // PayrollRelief.DeductionTypeId - so there is no flag for it here. One link,
        // held in one place.

        [StringLength(500)]
        public string? Description { get; set; }
    }
}
