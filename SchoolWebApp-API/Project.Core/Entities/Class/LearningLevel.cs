using SchoolWebApp.Core.Entities.CBE.Assessments;
using SchoolWebApp.Core.Entities.School;
using SchoolWebApp.Core.Entities.Shared;

namespace SchoolWebApp.Core.Entities.Class
{
    public class LearningLevel : Base
    {
        public required string Name { get; set; }
        public int Rank { get; set; }
        public string? Description { get; set; }
        /// <summary>
        /// Whether teachers keep schemes of work, lesson plans and a record of work for it. Off for one with no
        /// curriculum design to plan from (day care, say): it is then left out of Teaching Management.
        /// </summary>
        public bool RequiresTeachingDocuments { get; set; } = true;

        public int EducationLevelId { get; set; }
        public EducationLevel? EducationLevel { get; set; }

        public List<SchoolClass> SchoolClasses { get; set; } = new();
        public List<SubStrand> SubStrands { get; set; } = new();
        public List<LessonAllocation> LessonAllocations { get; set; } = new();
    }
}
