using SchoolWebApp.Core.Entities.Shared;
using System.ComponentModel.DataAnnotations;

namespace SchoolWebApp.Core.Entities.Payroll
{
    public class TaxBand : Base
    {
        [StringLength(255)]
        public string? Description { get; set; }

        public decimal LowerLimit { get; set; }
        public decimal UpperLimit { get; set; }
        public decimal Rate { get; set; }
        public DateTime EffectiveDate { get; set; }
        public bool IsActive { get; set; } = true;
    }
}
