using SchoolWebApp.Core.DTOs.School.EducationLevelType;
using SchoolWebApp.Core.DTOs.Settings;

namespace SchoolWebApp.Core.DTOs.CBE.Assessments.GeneralOutcome
{
    public class CreateGeneralOutcomeDto: BaseSettinsDto
    {
        public int EducationLevelTypeId { get; set; }
        public EducationLevelTypeDto? EducationLevelType { get; set; }
    }
}
