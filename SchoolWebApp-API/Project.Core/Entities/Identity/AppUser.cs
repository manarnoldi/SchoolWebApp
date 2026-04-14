using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace SchoolWebApp.Core.Entities.Identity
{
    public class AppUser : IdentityUser<int>
    {
        [Required]
        public required string FirstName { get; set; }
        [Required]
        public required string LastName { get; set; }
        public DateTime? Created { get; set; }

        [StringLength(255)]
        public string? CreatedBy { get; set; }
        public DateTime? Modified { get; set; }

        [StringLength(255)]
        public string? ModifiedBy { get; set; }
    }
}
