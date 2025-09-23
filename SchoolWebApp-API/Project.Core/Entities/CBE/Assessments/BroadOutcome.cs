using SchoolWebApp.Core.Entities.School;
using SchoolWebApp.Core.Entities.Shared;

namespace SchoolWebApp.Core.Entities.CBE.Assessments
{
    public class BroadOutcome : SettingsBase
    {
        public int EducationLevelId { get; set; }
        public EducationLevel? EducationLevel { get; set; } = null;
        public List<LearningOutcome> LearningOutcomes { get; set; } = new();
    }
}
