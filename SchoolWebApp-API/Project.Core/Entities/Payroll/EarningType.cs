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

        [StringLength(500)]
        public string? Description { get; set; }
    }
}
