using SchoolWebApp.Core.Entities.Academics;
using SchoolWebApp.Core.Entities.Class;
using SchoolWebApp.Core.Entities.Shared;

namespace SchoolWebApp.Core.Entities.CBE.Exams
{
    public class Exam : Base
    {
        public float ExamMark { get; set; }
        public DateOnly ExamStartDate { get; set; }
        public DateOnly ExamEndDate { get; set; }
        public DateOnly ExamMarkEntryEndDate { get; set; }

        public string? Description { get; set; }

        public int ExamTypeId { get; set; }
        public ExamType? ExamType { get; set; }
        public int SchoolClassId { get; set; }
        public SchoolClass? SchoolClass { get; set; }
        public int SessionId { get; set; }
        public Session? Session { get; set; }
        public int SubjectId { get; set; }
        public Subject? Subject { get; set; }

        public List<ExamResult> ExamResults { get; set; } = new();

    }
}
