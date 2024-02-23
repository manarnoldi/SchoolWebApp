using System.ComponentModel.DataAnnotations;

namespace SchoolWebApp.Core.DTOs.Staff.StaffDetails
{
    public class CreateStaffDetailDto: PersonDto
    {
        [Display(Name = "School identity number")]
        [StringLength(255)]
        public string? IdNumber { get; set; }

        public int StaffCategoryId { get; set; }
        public int DesignationId { get; set; }
        public int EmploymentTypeId { get; set; }
    }
}
