using SchoolWebApp.Core.Entities.Shared;
using System.ComponentModel.DataAnnotations;

namespace SchoolWebApp.Core.Entities.Finance
{
    public class FeeStructureItem : Base
    {
        public int FeeStructureId { get; set; }
        public FeeStructure? FeeStructure { get; set; }

        public int FeeCategoryId { get; set; }
        public FeeCategory? FeeCategory { get; set; }

        [Range(0, double.MaxValue)]
        public decimal Amount { get; set; }

        public bool IsMandatory { get; set; } = true;
    }
}
