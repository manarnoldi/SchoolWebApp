using SchoolWebApp.Core.DTOs.School.EducationLevel;
using SchoolWebApp.Core.Entities.School;

namespace SchoolWebApp.Core.DTOs.Class.LearningLevel
{
    public class CreateLearningLevelDto
    {
        public string? Name { get; set; }
        public string? Description { get; set; }
        public int Rank { get; set; }
        /// <summary>Teachers keep teaching documents for its classes; off leaves them out of Teaching Management.</summary>
        public bool RequiresTeachingDocuments { get; set; } = true;
        public int EducationLevelId { get; set; }
        public EducationLevelDto? EducationLevel { get; set; }
    }
}
