using SchoolWebApp.Core.DTOs.Class.SchoolClass;
using SchoolWebApp.Core.DTOs.Class.Session;
using SchoolWebApp.Core.DTOs.Students.Student;

namespace SchoolWebApp.Core.DTOs.Reports.Academics
{
    public class StudentPerformanceRowDto
    {
        public int StudentId { get; set; }
        public string FullName { get; set; }
        public string UPI { get; set; }
        public List<SubjectScoreDto> Subjects { get; set; } = new();
        public float TotalMarks { get; set; }
        public float TotalPossibleMarks { get; set; }
        public float Percentage => TotalPossibleMarks > 0
            ? (TotalMarks / TotalPossibleMarks) * 100
            : 0;
        public int SubjectAllocationCount { get; set; }
        public int Rank { get; set; }
        public List<PerExamScoreDto> ExamScores { get; set; } = new();
        public List<string> MissedExams { get; set; } = new();
    }
}
