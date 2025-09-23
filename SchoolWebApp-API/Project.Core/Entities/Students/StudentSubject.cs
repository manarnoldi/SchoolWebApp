using SchoolWebApp.Core.Entities.Academics;
using SchoolWebApp.Core.Entities.CBE.Exams;
using SchoolWebApp.Core.Entities.Shared;

namespace SchoolWebApp.Core.Entities.Students
{
    public class StudentSubject : Base
    {
        public int StudentClassId { get; set; }
        public StudentClass? StudentClass { get; set; }
        public int SubjectId { get; set; }
        public Subject? Subject { get; set; }
        public string? Description { get; set; }
        public List<ExamResult> ExamResults { get; set; } = new();
    }
}
