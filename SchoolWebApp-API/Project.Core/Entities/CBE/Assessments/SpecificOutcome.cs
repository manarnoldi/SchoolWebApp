using SchoolWebApp.Core.Entities.Class;
using SchoolWebApp.Core.Entities.Shared;

namespace SchoolWebApp.Core.Entities.CBE.Assessments
{
    public class SpecificOutcome: SettingsBase
    {
        public int SubStrandId { get; set; }
        public SubStrand? SubStrand { get; set; } = null;

        public int LearningLevelId { get; set; }
        public LearningLevel? LearningLevel { get; set; } = null;

        public int BroadOutcomeId { get; set; }
        public BroadOutcome? BroadOutcome { get; set; } = null;

        public int GeneralOutcomeId { get; set; }
        public GeneralOutcome? GeneralOutcome { get; set; } = null;

        public List<Competency> Competencies { get; set; } = new();
        public List<StudentAssessment> StudentAssessments { get; set; } = new();
    }
}
