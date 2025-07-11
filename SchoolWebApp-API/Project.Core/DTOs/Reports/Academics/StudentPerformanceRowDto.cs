using SchoolWebApp.Core.DTOs.Students.Student;
using SchoolWebApp.Core.Entities.Class;

namespace SchoolWebApp.Core.DTOs.Reports.Academics
{
    public class StudentPerformanceRowDto
    {
        public int StudentId { get; set; }
        public StudentDto? Student { get; set; }
        public int SessionId { get; set; }
        public Session? Session { get; set; }
        public int SchoolClassId { get; set; }
        public SchoolClass? SchoolClass { get; set; }
        public Dictionary<string, int> SubjectMarks { get; set; } = new();
        public int TotalMarks { get; set; }
        public int Rank { get; set; }
    }
}
