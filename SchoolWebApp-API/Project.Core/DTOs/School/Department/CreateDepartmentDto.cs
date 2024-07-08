using SchoolWebApp.Core.DTOs.Staff.StaffDetails;
using System.ComponentModel.DataAnnotations;

namespace SchoolWebApp.Core.DTOs.School.Department
{
    public class CreateDepartmentDto
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

        public int StaffDetailsId { get; set; }
        public StaffDetailDto? StaffDetails { get; set; }
    }
}
