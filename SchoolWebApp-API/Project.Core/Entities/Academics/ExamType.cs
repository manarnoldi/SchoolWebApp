using SchoolWebApp.Core.Entities.Shared;
using System.ComponentModel.DataAnnotations;

namespace SchoolWebApp.Core.Entities.Academics
{
    public class ExamType : SettingsBase
    {
        [Required(ErrorMessage = "Enter the exam type abbreviation")]
        [Display(Name = "Abbreviation")]
        [StringLength(255)]
        public required string Abbreviation { get; set; }

        [Required(ErrorMessage = "Select if exam type is currently featured")]
        public bool Featured { get; set; }

        public List<Exam> Exams { get; set; } = new();
    }
}
