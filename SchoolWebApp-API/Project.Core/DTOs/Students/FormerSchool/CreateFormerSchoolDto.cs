using SchoolWebApp.Core.DTOs.Academics.Curriculum;
using SchoolWebApp.Core.DTOs.School.EducationLevel;
using SchoolWebApp.Core.DTOs.Students.Student;
using SchoolWebApp.Core.Entities.Academics;
using SchoolWebApp.Core.Entities.School;
using System.ComponentModel.DataAnnotations;

namespace SchoolWebApp.Core.DTOs.Students.FormerSchool
{
    public class CreateFormerSchoolDto
    {
        public string? Description { get; set; }
        [Required(ErrorMessage = "Enter former school name")]
        [Display(Name = "School name")]
        [StringLength(255)]
        public string SchoolName { get; set; }

        [Required(ErrorMessage = "Enter student former class details")]
        [Display(Name = "Class details")]
        [StringLength(255)]
        public string ClassDetails { get; set; }

        [StringLength(255)]
        [Display(Name = "Last exam marks and out of")]
        public string? Score { get; set; }

        [StringLength(255)]
        [Display(Name = "Last position and out of")]
        public string? Position { get; set; }

        public int StudentId { get; set; }
        public StudentDto? Student { get; set; }
        public int CurriculumId { get; set; }
        public CurriculumDto? Curriculum { get; set; }
        public int EducationLevelId { get; set; }
        public EducationLevelDto? EducationLevel { get; set; }
    }
}
