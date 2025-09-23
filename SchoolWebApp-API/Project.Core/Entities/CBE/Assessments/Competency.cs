using SchoolWebApp.Core.Entities.Shared;

namespace SchoolWebApp.Core.Entities.CBE.Assessments
{
    public class Competency: Base   
    {
        public required string Name { get; set; }
        public string? Description { get; set; } = null;
        public List<LearningOutcome> LearningOutcomes { get; set; } = new();
    }
}
