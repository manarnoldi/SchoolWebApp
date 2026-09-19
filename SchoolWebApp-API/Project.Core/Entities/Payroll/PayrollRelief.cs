using SchoolWebApp.Core.Entities.Shared;
using System.ComponentModel.DataAnnotations;

namespace SchoolWebApp.Core.Entities.Payroll
{
    /// <summary>How the relief's value is arrived at.</summary>
    public enum ReliefBasis
    {
        /// <summary>A flat monthly figure, such as personal relief.</summary>
        FixedAmount = 0,

        /// <summary>
        /// A percentage of what the employee paid towards a particular deduction,
        /// such as insurance relief at 15% of premiums.
        /// </summary>
        PercentageOfDeduction = 1
    }

    /// <summary>
    /// A relief comes off the tax itself, after the bands have been applied - unlike
    /// an allowable deduction, which comes off income before the bands.
    ///
    /// Reliefs are rows rather than code so a new one, or a change to an existing
    /// one, is data entry. This follows the legacy SchoolSoft tblPayrollReliefs,
    /// where each relief named the deduction it was computed from.
    /// </summary>
    public class PayrollRelief : Base
    {
        [Required, StringLength(255)]
        public string Name { get; set; } = string.Empty;

        [Required, StringLength(50)]
        public string Code { get; set; } = string.Empty;

        public ReliefBasis Basis { get; set; } = ReliefBasis.FixedAmount;

        /// <summary>
        /// The deduction the relief is worked out from. Required when Basis is
        /// PercentageOfDeduction; for a fixed relief it optionally restricts the
        /// relief to employees who actually pay that deduction.
        /// </summary>
        public int? DeductionTypeId { get; set; }
        public DeductionType? DeductionType { get; set; }

        /// <summary>The flat amount, or the percentage, depending on Basis.</summary>
        public decimal Value { get; set; }

        /// <summary>Monthly ceiling on the relief. Null means uncapped.</summary>
        public decimal? MonthlyCap { get; set; }

        /// <summary>
        /// Give the relief to every employee. Only meaningful for a fixed amount -
        /// a percentage relief can only apply where the deduction it is based on
        /// exists.
        /// </summary>
        public bool AppliesToAll { get; set; }

        public bool IsActive { get; set; } = true;

        [StringLength(500)]
        public string? Description { get; set; }
    }
}
