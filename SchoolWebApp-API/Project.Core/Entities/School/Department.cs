using SchoolWebApp.Core.Entities.Academics;
using SchoolWebApp.Core.Entities.Shared;
using SchoolWebApp.Core.Entities.Staff;
using System.ComponentModel.DataAnnotations;

namespace SchoolWebApp.Core.Entities.School
{
    public class Department : Base
    {

        [Required(ErrorMessage = "Enter the department name.")]
        [StringLength(500)]
        [Display(Name = "Department name")]
        public required string Name { get; set; }

        [Required(ErrorMessage = "Enter the department code.")]
        [Display(Name = "Department code")]
        [StringLength(255)]
        public required string Code { get; set; }
        public string? Description { get; set; }

        public int? StaffDetailsId { get; set; }
        public StaffDetails? StaffDetails { get; set; }

        public List<Subject> Subjects { get; set; } = new();
    }
}
