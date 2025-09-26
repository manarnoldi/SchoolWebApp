using SchoolWebApp.Core.Entities.Shared;

namespace SchoolWebApp.Core.Entities.CBE.Assessments
{
    public class SubStrand: SettingsBase
    {
        public int StrandId { get; set; }
        public Strand? Strand { get; set; } = null;

        public List<SpecificOutcome> SpecificOutcomes { get; set; } = new();
    }
}
