using System.ComponentModel.DataAnnotations;

namespace SchoolWebApp.Core.DTOs.Academics.Exam
{
    public class CreateExamDto
    {
        public string Name { get; set; }
       [Required(ErrorMessage = "Enter the exam total mark")]
        [Display(Name = "Examination total mark")]
        public float ExamMark { get; set; }

        [Required(ErrorMessage = "Enter the examination contributing mark")]
        [Display(Name = "Contributing mark")]
        public float ContributingMark { get; set; }

        [Required(ErrorMessage = "Enter the examination duration in hours")]
        [Display(Name = "Examination hours")]
        public float ExamHours { get; set; }

        public int ExamTypeId { get; set; }
        public int SchoolClassId { get; set; }
        public int SessionId { get; set; }
        public int SubjectId { get; set; }
    }
}
