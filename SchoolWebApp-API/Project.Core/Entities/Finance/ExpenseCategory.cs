using SchoolWebApp.Core.Entities.Shared;
using System.ComponentModel.DataAnnotations;

namespace SchoolWebApp.Core.Entities.Finance
{
    public class ExpenseCategory : Base
    {
        [Required]
        [StringLength(255)]
        public required string Name { get; set; }

        [StringLength(500)]
        public string? Description { get; set; }

        public int Rank { get; set; }

        public int? ExpenseAccountId { get; set; }
        public Account? ExpenseAccount { get; set; }

        public bool IsActive { get; set; } = true;
    }
}
