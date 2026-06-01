using SchoolWebApp.Core.DTOs.Academics.ExamType;
using SchoolWebApp.Core.DTOs.Class.Session;
using System.ComponentModel.DataAnnotations;

namespace SchoolWebApp.Core.DTOs.Academics.SchoolExam
{
    public class CreateSchoolExamDto
    {
        [Display(Name = "Exam start date")]
        public DateOnly ExamStartDate { get; set; }

        [Display(Name = "Exam end date")]
        public DateOnly ExamEndDate { get; set; }

        [Display(Name = "Mark entry end date")]
        public DateOnly ExamMarkEntryEndDate { get; set; }

        public string? Description { get; set; }

        [Required(ErrorMessage = "Select the exam type")]
        public int ExamTypeId { get; set; }
        public ExamTypeDto? ExamType { get; set; }

        [Required(ErrorMessage = "Select the term/session")]
        public int SessionId { get; set; }
        public SessionDto? Session { get; set; }
    }
}
