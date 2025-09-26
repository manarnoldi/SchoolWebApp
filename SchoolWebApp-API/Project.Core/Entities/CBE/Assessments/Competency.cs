using SchoolWebApp.Core.Entities.Shared;

namespace SchoolWebApp.Core.Entities.CBE.Assessments
{
    public class Competency: SettingsBase   
    {
        public List<SpecificOutcome> SpecificOutcomes { get; set; } = new();
    }
}
