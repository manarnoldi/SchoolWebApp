using SchoolWebApp.Core.Entities.Shared;

namespace SchoolWebApp.Core.Entities.CBE.Assessments
{
    public class AssessmentType: SettingsBase
    {
        public List<StudentAssessment> StudentAssessments { get; set; } = new();
    }
}
