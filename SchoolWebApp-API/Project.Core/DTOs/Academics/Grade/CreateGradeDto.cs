using SchoolWebApp.Core.DTOs.Academics.Curriculum;
using System.ComponentModel.DataAnnotations;

namespace SchoolWebApp.Core.DTOs.Academics.Grade
{
    public class CreateGradeDto
    {
        [Required(ErrorMessage = "Enter the grade name")]
        [Display(Name = "Grade name")]
        [StringLength(255)]
        public required string Name { get; set; }

        [Required(ErrorMessage = "Enter the abbreviation")]
        [Display(Name = "Grade abbreviation")]
        [StringLength(255)]
        public required string Abbr { get; set; }

        [Required(ErrorMessage = "Enter the minimum score")]
        [Display(Name = "Minimum score")]
        public required float MinScore { get; set; }

        [Required(ErrorMessage = "Enter the maximum score")]
        [Display(Name = "Maximum score")]
        public required float MaxScore { get; set; }

        [Required(ErrorMessage = "Enter the grade points")]
        public required float Points { get; set; }

        [Display(Name = "Remarks in Kiswahili")]
        public string? RemarksSwa { get; set; }

        [Display(Name = "Remarks in English")]
        public string? RemarksEng { get; set; }

        public int CurriculumId { get; set; }
        public CurriculumDto? Curriculum { get; set; }
    }
}
