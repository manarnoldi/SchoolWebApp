using SchoolWebApp.Core.Entities.Shared;
using SchoolWebApp.Core.Entities.Students;

namespace SchoolWebApp.Core.Entities.CBE.Exams
{
    public class ExamResult : Base
    {
        public int StudentId { get; set; }
        public Student? Student { get; set; }
        public int ExamId { get; set; }
        public Exam? Exam { get; set; }

        // The subject allocation this result belongs to. Deleting the
        // StudentSubject cascade-deletes its results (configured in
        // ApplicationDbContextConfigurations), so a result can never outlive
        // the allocation that justified it.
        public int StudentSubjectId { get; set; }
        public StudentSubject? StudentSubject { get; set; }

        public float Score { get; set; }
        public string? Description { get; set; }
    }
}
