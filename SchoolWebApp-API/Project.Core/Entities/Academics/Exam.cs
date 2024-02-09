using SchoolWebApp.Core.Entities.Class;
using SchoolWebApp.Core.Entities.Shared;
using System.ComponentModel.DataAnnotations;

namespace SchoolWebApp.Core.Entities.Academics
{
    public class Exam : Base
    {
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
        public ExamType ExamType { get; set; }
        public int SchoolClassId { get; set; }
        public SchoolClass SchoolClass { get; set; }
        public int SessionId { get; set; }
        public Session Session { get; set; }
        public int SubjectId { get; set; }
        public Subject Subject { get; set; }

        public List<ExamResult> ExamResults { get; set; }
    }
}
