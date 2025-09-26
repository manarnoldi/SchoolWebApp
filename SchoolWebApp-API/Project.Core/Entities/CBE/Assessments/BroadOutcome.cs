using SchoolWebApp.Core.Entities.Academics;
using SchoolWebApp.Core.Entities.School;
using SchoolWebApp.Core.Entities.Shared;

namespace SchoolWebApp.Core.Entities.CBE.Assessments
{
    public class BroadOutcome : SettingsBase
    {
        public int EducationLevelId { get; set; }
        public EducationLevel? EducationLevel { get; set; } = null;

        public int SubjectId { get; set; }
        public Subject? Subject { get; set; } = null;

        public List<SpecificOutcome> SpecificOutcomes { get; set; } = new();
    }
}
