using SchoolWebApp.Core.Entities.Academics;
using SchoolWebApp.Core.Entities.School;
using SchoolWebApp.Core.Entities.Shared;
using System.ComponentModel.DataAnnotations;

namespace SchoolWebApp.Core.Entities.Students
{
    public class FormerSchool : Base
    {
        [Required(ErrorMessage = "Enter former school name")]
        [Display(Name = "School name")]
        [StringLength(255)]
        public required string SchoolName { get; set; }
        public string? Description { get; set; }

        [Required(ErrorMessage = "Enter student former class details")]
        [Display(Name = "Class details")]
        [StringLength(255)]
        public required string ClassDetails { get; set; }

        [StringLength(255)]
        [Display(Name = "Last exam marks and out of")]
        public string? Score { get; set; }

        [StringLength(255)]
        [Display(Name = "Last position and out of")]
        public string? Position { get; set; }

        public int StudentId { get; set; }
        public Student? Student { get; set; }
        public int CurriculumId { get; set; }
        public Curriculum? Curriculum { get; set; }
        public int EducationLevelId { get; set; }
        public EducationLevel? EducationLevel { get; set; }
    }
}
