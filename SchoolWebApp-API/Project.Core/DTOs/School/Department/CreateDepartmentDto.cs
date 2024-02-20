using System.ComponentModel.DataAnnotations;

namespace SchoolWebApp.Core.DTOs.School.Department
{
    public class CreateDepartmentDto
    {
        [Required(ErrorMessage = "Enter the department name.")]
        [StringLength(500)]
        [Display(Name = "Department name")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Enter the department code.")]
        [Display(Name = "Department code")]
        [StringLength(255)]
        public string Code { get; set; }
        public string Description { get; set; }

        public int StaffDetailsId { get; set; }
    }
}
