namespace SchoolWebApp.Core.DTOs.CBE.Assessments.LessonAllocation
{
    public class CreateLessonAllocationDto
    {
        public int LessonsPerWeek { get; set; }
        public string? Description { get; set; }
        public int SubjectId { get; set; }
        public int LearningLevelId { get; set; }
    }
}
