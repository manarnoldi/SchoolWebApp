using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchoolWebApp.Core.Entities.Identity
{
    public class AppRole : IdentityRole
    {
        public DateTime? Created { get; set; }

        [StringLength(255)]
        public string? CreatedBy { get; set; }
        public DateTime? Modified { get; set; }

        [StringLength(255)]
        public string? ModifiedBy { get; set; }
    }
}
