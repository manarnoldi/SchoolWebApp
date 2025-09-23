using SchoolWebApp.Core.Entities.Shared;

namespace SchoolWebApp.Core.Entities.CBE.Assessments
{
    public class LearningOutcome: SettingsBase
    {
        public int SubStrandId { get; set; }
        public SubStrand? SubStrand { get; set; } = null;
        public int BroadOutcomeId { get; set; }
        public BroadOutcome? BroadOutcome { get; set; } = null;
        public int CompetencyId { get; set; }
        public Competency? Competency { get; set; } = null;
        public List<Assessment> Assessments { get; set; } = new();
    }
}
