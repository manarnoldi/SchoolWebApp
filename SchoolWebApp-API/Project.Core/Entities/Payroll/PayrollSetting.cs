using SchoolWebApp.Core.Entities.Shared;
using System.ComponentModel.DataAnnotations;

namespace SchoolWebApp.Core.Entities.Payroll
{
    public class PayrollSetting : Base
    {
        [Required, StringLength(100)]
        public string Key { get; set; } = string.Empty;

        [Required, StringLength(255)]
        public string Name { get; set; } = string.Empty;

        public decimal Value { get; set; }

        [StringLength(50)]
        public string? Category { get; set; }

        [StringLength(500)]
        public string? Description { get; set; }

        public DateTime EffectiveDate { get; set; }
        public bool IsActive { get; set; } = true;
    }
}
