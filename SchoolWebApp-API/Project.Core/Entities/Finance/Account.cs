using SchoolWebApp.Core.Entities.Shared;
using System.ComponentModel.DataAnnotations;

namespace SchoolWebApp.Core.Entities.Finance
{
    public class Account : Base
    {
        [Required]
        [StringLength(20)]
        public required string Code { get; set; }

        [Required]
        [StringLength(255)]
        public required string Name { get; set; }

        public AccountType AccountType { get; set; }

        public int? ParentAccountId { get; set; }
        public Account? ParentAccount { get; set; }

        [StringLength(500)]
        public string? Description { get; set; }

        public bool IsActive { get; set; } = true;

        public List<JournalLine> JournalLines { get; set; } = new();
    }
}
