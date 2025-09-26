using SchoolWebApp.Core.DTOs.Academics.Subject;
using SchoolWebApp.Core.DTOs.School.EducationLevel;
using SchoolWebApp.Core.DTOs.Settings;

namespace SchoolWebApp.Core.DTOs.CBE.Assessments.BroadOutcome
{
    public class CreateBroadOutcomeDto: BaseSettinsDto
    {
        public int EducationLevelId { get; set; }
        public EducationLevelDto? EducationLevel { get; set; }
        public int SubjectId { get; set; }
        public SubjectDto? Subject { get; set; }
    }
}
