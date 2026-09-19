using SchoolWebApp.Core.Entities.Shared;
using System.ComponentModel.DataAnnotations;

namespace SchoolWebApp.Core.Entities.Payroll
{
    public class EarningType : Base
    {
        [Required, StringLength(255)]
        public string Name { get; set; } = string.Empty;

        [Required, StringLength(50)]
        public string Code { get; set; } = string.Empty;

        public bool IsTaxable { get; set; } = true;
        public bool IsActive { get; set; } = true;

        /// <summary>
        /// How the amount is worked out when the earning is applied from the type
        /// rather than entered per employee.
        /// </summary>
        public PayrollCalculationMethod CalculationMethod { get; set; } = PayrollCalculationMethod.FixedAmount;

        /// <summary>
        /// The fixed amount, or the percentage of BASIC salary, depending on
        /// CalculationMethod. Basic rather than gross, because an earning taken as
        /// a percentage of gross would be defined in terms of itself.
        /// </summary>
        public decimal? DefaultValue { get; set; }

        /// <summary>
        /// Apply to every employee on the payroll without assigning it to each one.
        /// An employee's own line for this earning takes precedence.
        /// </summary>
        public bool AppliesToAll { get; set; }

        /// <summary>
        /// Payroll already accounts for this from the salary structure's own columns
        /// (basic, house, transport). Such a type is never applied as a configured
        /// line, or it would be counted twice.
        /// </summary>
        public bool IsSystemComputed { get; set; }

        [StringLength(500)]
        public string? Description { get; set; }
    }
}
