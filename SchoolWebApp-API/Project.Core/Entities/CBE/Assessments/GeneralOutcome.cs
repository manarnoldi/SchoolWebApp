using SchoolWebApp.Core.Entities.School;
using SchoolWebApp.Core.Entities.Shared;

namespace SchoolWebApp.Core.Entities.CBE.Assessments
{
    public class GeneralOutcome: SettingsBase
    {
        public int EducationLevelTypeId { get; set; }
        public EducationLevelType? EducationLevelType { get; set; } = null;

        public List<SpecificOutcome> SpecificOutcomes { get; set; } = new();
    }
}
