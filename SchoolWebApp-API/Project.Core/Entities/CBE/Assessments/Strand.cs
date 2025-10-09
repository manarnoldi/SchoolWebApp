using SchoolWebApp.Core.Entities.Academics;
using SchoolWebApp.Core.Entities.Class;
using SchoolWebApp.Core.Entities.Shared;

namespace SchoolWebApp.Core.Entities.CBE.Assessments
{
    public class Strand : SettingsBase
    {
        public int AcademicYearId { get; set; }
        public AcademicYear? AcademicYear { get; set; } = null;

        public int SubjectId { get; set; }
        public Subject? Subject { get; set; } = null;

        public int LearningLevelId { get; set; }
        public LearningLevel? LearningLevel { get; set; } = null;

        public List<SubStrand> SubStrands { get; set; } = new();
    }
}
