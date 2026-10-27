using SchoolWebApp.Core.Entities.School;
using SchoolWebApp.Core.Entities.Shared;

namespace SchoolWebApp.Core.Entities.CBE.Assessments
{
    public class GeneralOutcome: SettingsBase
    {
        public int EducationLevelTypeId { get; set; }
        public EducationLevelType? EducationLevelType { get; set; } = null;
        // Set when the outcome belongs to one education level only (KICD prints level
        // outcomes per level - Primary, Junior School - while types group levels
        // differently). Unset: it serves every level of the type that has none of its own.
        public int? EducationLevelId { get; set; }
        public EducationLevel? EducationLevel { get; set; } = null;

        public List<SpecificOutcome> SpecificOutcomes { get; set; } = new();
    }
}
