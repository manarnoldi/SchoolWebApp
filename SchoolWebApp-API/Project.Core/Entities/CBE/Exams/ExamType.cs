using SchoolWebApp.Core.Entities.Shared;

namespace SchoolWebApp.Core.Entities.CBE.Exams
{
    public class ExamType : SettingsBase
    {
        public required string Abbreviation { get; set; }
        public bool Internal { get; set; }
        public List<Exam> Exams { get; set; } = new ();
    }
}
