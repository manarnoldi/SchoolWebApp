using SchoolWebApp.Core.DTOs.CBE.Assessments.BroadOutcome;
using SchoolWebApp.Core.DTOs.CBE.Assessments.GeneralOutcome;
using SchoolWebApp.Core.DTOs.CBE.Assessments.SubStrand;
using SchoolWebApp.Core.DTOs.Class.LearningLevel;
using SchoolWebApp.Core.DTOs.Settings;

namespace SchoolWebApp.Core.DTOs.CBE.Assessments.SpecificOutcome
{
    public class CreateSpecificOutcomeDto: BaseSettinsDto
    {
        public int SubStrandId { get; set; }
        public SubStrandDto? SubStrand { get; set; }
        public int BroadOutcomeId { get; set; }
        public BroadOutcomeDto? BroadOutcome { get; set; }
        public int GeneralOutcomeId { get; set; }
        public GeneralOutcomeDto? GeneralOutcome { get; set; } = null;
        public int LearningLevelId { get; set; }
        public LearningLevelDto? LearningLevel { get; set; }
    }
}
