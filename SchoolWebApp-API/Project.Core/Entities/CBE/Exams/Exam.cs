using SchoolWebApp.Core.Entities.Academics;
using SchoolWebApp.Core.Entities.Class;
using SchoolWebApp.Core.Entities.Shared;

namespace SchoolWebApp.Core.Entities.CBE.Exams
{
    /// <summary>
    /// A single class + subject instance under a <see cref="SchoolExam"/>
    /// header. The event-level properties (type, term, dates) live on the
    /// header; only the per-class-per-subject max mark and results live here.
    /// </summary>
    public class Exam : Base
    {
        public float ExamMark { get; set; }
        public string? Description { get; set; }

        public int SchoolExamId { get; set; }
        public SchoolExam? SchoolExam { get; set; }
        public int SchoolClassId { get; set; }
        public SchoolClass? SchoolClass { get; set; }
        public int SubjectId { get; set; }
        public Subject? Subject { get; set; }

        public List<ExamResult> ExamResults { get; set; } = new();
    }
}
