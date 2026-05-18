using SchoolWebApp.Core.Entities.Finance;
using System.ComponentModel.DataAnnotations;

namespace SchoolWebApp.Core.DTOs.Finance.Account
{
    public class CreateAccountDto
    {
        [Required]
        [StringLength(20)]
        public required string Code { get; set; }

        [Required]
        [StringLength(255)]
        public required string Name { get; set; }

        public AccountType AccountType { get; set; }

        public int? ParentAccountId { get; set; }

        [StringLength(500)]
        public string? Description { get; set; }

        public bool IsActive { get; set; } = true;
    }
}
