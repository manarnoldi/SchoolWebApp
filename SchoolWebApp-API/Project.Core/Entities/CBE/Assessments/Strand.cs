using SchoolWebApp.Core.Entities.Academics;
using SchoolWebApp.Core.Entities.Class;
using SchoolWebApp.Core.Entities.Shared;

namespace SchoolWebApp.Core.Entities.CBE.Assessments
{
    public class Strand : SettingsBase
    {
        public string? Code { get; set; }

        public int? CurriculumId { get; set; }
        public Curriculum? Curriculum { get; set; } = null;

        public int Version { get; set; }

        public int SubjectId { get; set; }
        public Subject? Subject { get; set; } = null;

        public int LearningLevelId { get; set; }
        public LearningLevel? LearningLevel { get; set; } = null;

        public int? ThemeId { get; set; }
        public Theme? Theme { get; set; } = null;

        public List<SubStrand> SubStrands { get; set; } = new();
    }
}
