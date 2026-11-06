using SchoolWebApp.Core.Entities.Academics;
using SchoolWebApp.Core.Entities.Class;
using SchoolWebApp.Core.Entities.Shared;
using SchoolWebApp.Core.Entities.Staff;
using System.ComponentModel.DataAnnotations;

namespace SchoolWebApp.Core.Entities.Timetable
{
    /// <summary>Where in the day a subject's lessons go. "Prefer" is a wish; "Only" is a rule.</summary>
    public enum SubjectPlacement
    {
        Any = 0,
        PreferMorning = 1,
        MorningOnly = 2,
        PreferAfternoon = 3,
        AfternoonOnly = 4
    }

    /// <summary>
    /// How one subject is timetabled - for every grade, or for one grade (a rule
    /// for the grade wins over the subject's general rule). Subjects with no rule
    /// are timetabled with the defaults: anywhere in the day, single lessons, at
    /// most one a day.
    /// </summary>
    public class TimetableSubjectRule : Base
    {
        public int SubjectId { get; set; }
        public Subject? Subject { get; set; }
        public int? LearningLevelId { get; set; }
        public LearningLevel? LearningLevel { get; set; }
        // Taught outside the timetable (or not taught as a class lesson at all).
        public bool Excluded { get; set; }
        public SubjectPlacement Placement { get; set; } = SubjectPlacement.Any;
        // How many of the week's lessons are taught as back-to-back doubles; each double uses two lessons.
        public int DoubleLessons { get; set; }
        // Most times the subject meets in a day (a double counts once).
        public int MaxPerDay { get; set; } = 1;
        public bool AvoidLastLesson { get; set; }
    }

    /// <summary>
    /// Subjects a class takes at the same time, learners splitting between them
    /// with a teacher each - e.g. CRE / IRE / HRE, or a Senior School elective
    /// block. The group meets as often as its busiest subject. With no grade it
    /// applies to every grade that takes its subjects.
    /// </summary>
    public class TimetableOptionGroup : Base
    {
        [StringLength(100)]
        public required string Name { get; set; }
        public int? LearningLevelId { get; set; }
        public LearningLevel? LearningLevel { get; set; }
        public List<TimetableOptionGroupSubject> Subjects { get; set; } = new();
    }

    public class TimetableOptionGroupSubject : Base
    {
        public int TimetableOptionGroupId { get; set; }
        public TimetableOptionGroup? TimetableOptionGroup { get; set; }
        public int SubjectId { get; set; }
        public Subject? Subject { get; set; }
    }

    /// <summary>Limits on one teacher's week. Times they cannot teach are <see cref="TeacherUnavailability"/>.</summary>
    public class TeacherTimetableRule : Base
    {
        public int StaffDetailsId { get; set; }
        public StaffDetails? StaffDetails { get; set; }
        public int? MaxLessonsPerDay { get; set; }
        public int? MaxConsecutiveLessons { get; set; }
        [StringLength(500)]
        public string? Notes { get; set; }
    }

    /// <summary>
    /// A time a teacher cannot teach, e.g. a part-timer's free mornings or a
    /// weekly meeting. Kept as clock times, not slot numbers, because a teacher
    /// can teach grades on different bell schedules whose lessons start at
    /// different times.
    /// </summary>
    public class TeacherUnavailability : Base
    {
        public int StaffDetailsId { get; set; }
        public StaffDetails? StaffDetails { get; set; }
        // ISO day number, 1 = Monday.
        public int Day { get; set; }
        public TimeOnly StartTime { get; set; }
        public TimeOnly EndTime { get; set; }
        [StringLength(255)]
        public string? Reason { get; set; }
    }
}
