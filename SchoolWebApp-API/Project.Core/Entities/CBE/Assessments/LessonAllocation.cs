using SchoolWebApp.Core.Entities.Academics;
using SchoolWebApp.Core.Entities.Class;
using SchoolWebApp.Core.Entities.Shared;

namespace SchoolWebApp.Core.Entities.CBE.Assessments
{
    public class LessonAllocation: Base
    {
        public int LessonsPerWeek { get; set; }
        public string? Description { get; set; }
        public int SubjectId { get; set; }
        public Subject? Subject { get; set; } = null;
        public int LearningLevelId { get; set; }
        public LearningLevel? LearningLevel { get; set; } = null;        
    }
}
