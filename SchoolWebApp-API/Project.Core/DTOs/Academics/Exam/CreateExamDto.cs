using SchoolWebApp.Core.DTOs.Academics.ExamType;
using SchoolWebApp.Core.DTOs.Academics.Subject;
using SchoolWebApp.Core.DTOs.Class.SchoolClass;
using SchoolWebApp.Core.DTOs.Class.Session;
using System.ComponentModel.DataAnnotations;

namespace SchoolWebApp.Core.DTOs.Academics.Exam
{
    public class CreateExamDto
    {
        [Required(ErrorMessage = "Enter the exam total mark")]
        [Display(Name = "Examination total mark")]
        public float ExamMark { get; set; }

        // The SchoolExam header this detail row belongs to. On create only
        // SchoolExamId + SchoolClassId + SubjectId + ExamMark are used; the
        // header-level fields below are populated on read (flattened from
        // the SchoolExam navigation) so existing consumers keep working.
        public int SchoolExamId { get; set; }

        public DateOnly ExamStartDate { get; set; }
        public DateOnly ExamEndDate { get; set; }
        public DateOnly ExamMarkEntryEndDate { get; set; }
        public string? Description { get; set; }

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
