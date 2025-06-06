﻿using SchoolWebApp.Core.Entities.Class;
using SchoolWebApp.Core.Entities.Enums;
using SchoolWebApp.Core.Entities.Identity;
using SchoolWebApp.Core.Entities.Settings;
using System.ComponentModel.DataAnnotations;

namespace SchoolWebApp.Core.Entities.Shared
{
    public class Person : Base
    {
        public string? StaffImageAsBase64 { get; set; }

        [Required(ErrorMessage = "Enter the full name")]
        [Display(Name = "Full name")]
        [StringLength(255)]
        public required string FullName { get; set; }

        [Required(ErrorMessage = "Enter the unique personal identifiable number")]
        [Display(Name = "Unique Personal Identifiable Number")]
        [StringLength(255)]
        public required string UPI { get; set; }

        [Display(Name = "Date of birth")]
        public DateTime? DateOfBirth { get; set; }

        [StringLength(255)]
        public string? Address { get; set; }

        [Display(Name = "Phone number")]
        [StringLength(255)]
        public string? PhoneNumber { get; set; }

        [Display(Name = "Email address")]
        [StringLength(255)]
        [RegularExpression(@"(?:[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*|""(?:[\x01-\x08\x0b\x0c\x0e-\x1f\x21\x23-\x5b\x5d-\x7f]|\\[\x01-\x09\x0b\x0c\x0e-\x7f])*"")@(?:(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?|\[(?:(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.){3}(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?|[a-z0-9-]*[a-z0-9]:(?:[\x01-\x08\x0b\x0c\x0e-\x1f\x21-\x5a\x53-\x7f]|\\[\x01-\x09\x0b\x0c\x0e-\x7f])+)\])",
           ErrorMessage = "The email address is not entered in a correct format")]
        public string? Email { get; set; }

        public Status Status { get; set; }
        public string? OtherDetails { get; set; }

        public int NationalityId { get; set; }
        public Nationality? Nationality { get; set; }
        public int ReligionId { get; set; }
        public Religion? Religion { get; set; }
        public int GenderId { get; set; }
        public Gender? Gender { get; set; }
        public List<SchoolClassLeaders> SchoolClassLeaders { get; set; } = new();
        public List<AppUser> AppUsers { get; set; } = new();
    }
}
