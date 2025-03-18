using SchoolWebApp.Core.DTOs.School.EducationLevel;
using SchoolWebApp.Core.Entities.School;

namespace SchoolWebApp.Core.DTOs.Class.LearningLevel
{
    public class CreateLearningLevelDto
    {
        public string? Name { get; set; }
        public string? Description { get; set; }
        public int Rank { get; set; }
        public int EducationLevelId { get; set; }
        public EducationLevelDto? EducationLevel { get; set; }
    }
}
