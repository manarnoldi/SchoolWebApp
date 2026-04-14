using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace SchoolWebApp.Core.Entities.Identity
{
    public class AppRole : IdentityRole<int>
    {
        public DateTime? Created { get; set; }

        [StringLength(255)]
        public string? CreatedBy { get; set; }
        public DateTime? Modified { get; set; }

        [StringLength(255)]
        public string? ModifiedBy { get; set; }
    }
}
