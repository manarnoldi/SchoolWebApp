using SchoolWebApp.Core.Entities.Shared;
using System.ComponentModel.DataAnnotations;

namespace SchoolWebApp.Core.Entities.Finance
{
    public class FeeCategory : Base
    {
        [Required]
        [StringLength(255)]
        public required string Name { get; set; }

        [StringLength(500)]
        public string? Description { get; set; }

        public int Rank { get; set; }

        public int? IncomeAccountId { get; set; }
        public Account? IncomeAccount { get; set; }

        public bool IsActive { get; set; } = true;
    }
}
