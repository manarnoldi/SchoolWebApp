using SchoolWebApp.Core.DTOs.Academics.ExamType;
using SchoolWebApp.Core.DTOs.Academics.Subject;
using SchoolWebApp.Core.DTOs.Class.SchoolClass;
using SchoolWebApp.Core.DTOs.Class.Session;
using System.ComponentModel.DataAnnotations;

namespace SchoolWebApp.Core.DTOs.Academics.Exam
{
    public class CreateExamDto
    {
        public required string Name { get; set; }
       [Required(ErrorMessage = "Enter the exam total mark")]
        [Display(Name = "Examination total mark")]
        public float ExamMark { get; set; }

        [Required(ErrorMessage = "Enter the examination contributing mark")]
        [Display(Name = "Contributing mark")]
        public float ContributingMark { get; set; }

        public string? OtherDetails { get; set; }

        public int ExamTypeId { get; set; }
        public ExamTypeDto? ExamType { get; set; }
        public int SchoolClassId { get; set; }
        public SchoolClassDto? SchoolClass { get; set; }
        public int SessionId { get; set; }
        public SessionDto? Session { get; set; }
        public int SubjectId { get; set; }
        public SubjectDto? Subject { get; set; }
    }
}
