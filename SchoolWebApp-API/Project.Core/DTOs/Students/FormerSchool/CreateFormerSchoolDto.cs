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
        public string Score { get; set; }

        [StringLength(255)]
        [Display(Name = "Last position and out of")]
        public string Position { get; set; }

        public int StudentId { get; set; }
        public int CurriculumId { get; set; }
        public int EducationLevelId { get; set; }
    }
}
