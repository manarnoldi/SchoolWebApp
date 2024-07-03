using SchoolWebApp.Core.DTOs.Settings.Designation;
using SchoolWebApp.Core.DTOs.Settings.EmploymentType;
using SchoolWebApp.Core.DTOs.Settings.StaffCategory;
using System.ComponentModel.DataAnnotations;

namespace SchoolWebApp.Core.DTOs.Staff.StaffDetails
{
    public class CreateStaffDetailDto: PersonDto
    {
        [Display(Name = "School identity number")]
        [StringLength(255)]
        public string? IdNumber { get; set; }
        public string? NhifNo { get; set; }
        public string? NssfNo { get; set; }
        public string? KraPinNo { get; set; }
        public DateTime? EmploymentDate { get; set; }
        public DateTime? EndofEmploymentDate { get; set; }
        public bool CurrentlyEmployed { get; set; }

        public int StaffCategoryId { get; set; }
        public StaffCategoryDto? StaffCategory { get; set; }
        public int DesignationId { get; set; }
        public DesignationDto? Designation { get; set; }
        public int EmploymentTypeId { get; set; }
        public EmploymentTypeDto? EmploymentType { get; set; }
    }
}
