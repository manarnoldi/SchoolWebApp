using SchoolWebApp.Core.DTOs.Settings;
using System.ComponentModel.DataAnnotations;

namespace SchoolWebApp.Core.DTOs.Academics.ExamType
{
    public class CreateExamTypeDto: BaseSettinsDto
    {
        [Required(ErrorMessage = "Enter the exam type abbreviation")]
        [Display(Name = "Abbreviation")]
        [StringLength(255)]
        public required string Abbreviation { get; set; }

        [Required(ErrorMessage = "Select if exam type is currently featured")]
        public bool Featured { get; set; }
    }
}
