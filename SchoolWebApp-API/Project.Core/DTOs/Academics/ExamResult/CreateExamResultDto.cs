using SchoolWebApp.Core.DTOs.Academics.Exam;
using SchoolWebApp.Core.DTOs.Students.Student;

namespace SchoolWebApp.Core.DTOs.Academics.ExamResult
{
    public class CreateExamResultDto
    {
        public float? Score { get; set; }

        public int StudentId { get; set; }
        public StudentDto? Student { get; set; }
        public int ExamId { get; set; }
        public ExamDto? Exam { get; set; }
    }
}
