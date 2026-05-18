using Microsoft.AspNetCore.Identity;
using SchoolWebApp.Core.Entities.Shared;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

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

        // Optional link to the underlying Person row (Staff / Student / Parent
        // via the TPH `Person` table). Lets the system answer "who is this
        // user in real life?" and keeps the picker -> save roundtrip
        // traceable. Nullable so existing logins without a linked person
        // remain valid.
        public int? PersonId { get; set; }
        [ForeignKey(nameof(PersonId))]
        public Person? Person { get; set; }

        // Forces a password change on next login. Set true when an admin
        // creates the account or resets the password; cleared by the user
        // once they change it themselves via /api/auth/change-password.
        public bool MustChangePassword { get; set; } = true;
    }
}
