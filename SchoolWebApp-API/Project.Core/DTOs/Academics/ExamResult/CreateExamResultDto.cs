using System.ComponentModel.DataAnnotations;

namespace SchoolWebApp.Core.DTOs.Academics.ExamResult
{
    public class CreateExamResultDto
    {
        [Required(ErrorMessage = "Enter the examination score")]
        [Display(Name = "Examination score")]
        public float Score { get; set; }

        public int StudentId { get; set; }
        public int ExamId { get; set; }
    }
}
