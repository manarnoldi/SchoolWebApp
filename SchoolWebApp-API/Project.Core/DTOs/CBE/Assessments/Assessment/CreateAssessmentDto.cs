using SchoolWebApp.Core.Entities.Academics;
using SchoolWebApp.Core.Entities.Class;
using SchoolWebApp.Core.Entities.Students;

namespace SchoolWebApp.Core.DTOs.CBE.Assessments.Assessment
{
    public class CreateAssessmentDto
    {
        public int StudentId { get; set; }
        public int LearningOutcomeId { get; set; }
        public int GradeId { get; set; }
        public int SessionId { get; set; }
        public int AssessmentTypeId { get; set; }
        public string? Description { get; set; } = null;
    }
}
