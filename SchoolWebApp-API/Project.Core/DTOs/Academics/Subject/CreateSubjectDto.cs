using SchoolWebApp.Core.DTOs.Academics.SubjectGroup;
using SchoolWebApp.Core.DTOs.School.Department;
using SchoolWebApp.Core.DTOs.Staff.StaffDetails;
using SchoolWebApp.Core.Entities.School;
using SchoolWebApp.Core.Entities.Staff;
using System.ComponentModel.DataAnnotations;

namespace SchoolWebApp.Core.DTOs.Academics.Subject
{
    public class CreateSubjectDto
    {
        [Required(ErrorMessage = "Enter the subject code")]
        [Display(Name = "Subject code")]
        [StringLength(255)]
        public required string Code { get; set; }

        [Required(ErrorMessage = "Enter the subject name")]
        [Display(Name = "Subject name")]
        [StringLength(255)]
        public required string Name { get; set; }

        [Required(ErrorMessage = "Enter the subject abbreviation")]
        [Display(Name = "Subject abbreviation")]
        [StringLength(255)]
        public required string Abbr { get; set; }
        public int NumOfLessons { get; set; }
        public string? Description { get; set; }
        public bool Optional { get; set; }
        public int Rank { get; set; }
        public int SubjectGroupId { get; set; }
        public SubjectGroupDto? SubjectGroup { get; set; }
        public int DepartmentId { get; set; }
        public DepartmentDto? Department { get; set; }
        public int? StaffDetailsId { get; set; }
        public StaffDetailDto? StaffDetails { get; set; }
    }
}
