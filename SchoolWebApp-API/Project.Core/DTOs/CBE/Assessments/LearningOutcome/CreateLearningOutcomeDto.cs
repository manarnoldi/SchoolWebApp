using SchoolWebApp.Core.DTOs.Settings;

namespace SchoolWebApp.Core.DTOs.CBE.Assessments.LearningOutcome
{
    public class CreateLearningOutcomeDto: BaseSettinsDto
    {
        public int SubStrandId { get; set; }
        public int BroadOutcomeId { get; set; }
        public int CompetencyId { get; set; }
    }
}
