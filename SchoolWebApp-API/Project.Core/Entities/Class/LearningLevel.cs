using SchoolWebApp.Core.Entities.CBE.Assessments;
using SchoolWebApp.Core.Entities.School;
using SchoolWebApp.Core.Entities.Shared;

namespace SchoolWebApp.Core.Entities.Class
{
    public class LearningLevel : Base
    {
        public required string Name { get; set; }
        public int Rank { get; set; }
        public string? Description { get; set; }
        public int EducationLevelId { get; set; }
        public EducationLevel? EducationLevel { get; set; }

        public List<SchoolClass> SchoolClasses { get; set; } = new();
        public List<SpecificOutcome> SpecificOutcomes { get; set; } = new();
    }
}
