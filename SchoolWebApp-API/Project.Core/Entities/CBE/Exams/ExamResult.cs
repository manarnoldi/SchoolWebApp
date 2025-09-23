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
        public float Score { get; set; }
        public string? Description { get; set; }
    }
}
