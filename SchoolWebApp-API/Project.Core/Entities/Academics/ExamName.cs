using SchoolWebApp.Core.Entities.Shared;

namespace SchoolWebApp.Core.Entities.Academics
{
    public class ExamName: SettingsBase
    {
        public int ExamtypeId { get; set; }
        public ExamType? ExamType { get; set; }

        public List<Exam> Exams { get; set; } = new();
    }
}
