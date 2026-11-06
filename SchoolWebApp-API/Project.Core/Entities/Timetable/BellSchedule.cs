using SchoolWebApp.Core.Entities.Academics;
using SchoolWebApp.Core.Entities.Class;
using SchoolWebApp.Core.Entities.Shared;
using System.ComponentModel.DataAnnotations;

namespace SchoolWebApp.Core.Entities.Timetable
{
    /// <summary>What a slot in the school day is used for. Only Lesson slots are timetabled.</summary>
    public enum BellSlotKind
    {
        Lesson = 0,
        Break = 1,
        Lunch = 2,
        // Assembly, games, clubs, prep: on the bell but not a timetabled lesson.
        Activity = 3
    }

    /// <summary>Which part of the day a slot falls in, for subjects kept to mornings or afternoons.</summary>
    public enum DayPart
    {
        Morning = 0,
        Afternoon = 1
    }

    /// <summary>
    /// The shape of the school day for a group of grades: the teaching days and
    /// the ordered slots (lessons, breaks, lunch). Lower grades usually have
    /// shorter lessons and finish earlier than Junior School, so a school has one
    /// schedule per group of grades. Each grade follows at most one schedule.
    /// </summary>
    public class BellSchedule : Base
    {
        [StringLength(100)]
        public required string Name { get; set; }
        [StringLength(500)]
        public string? Description { get; set; }
        // ISO day numbers taught, comma-separated: "1,2,3,4,5" is Monday to Friday.
        [StringLength(20)]
        public string Days { get; set; } = "1,2,3,4,5";
        public List<BellSlot> Slots { get; set; } = new();
        public List<BellScheduleLevel> Levels { get; set; } = new();
        public List<TimetableFixedSlot> FixedSlots { get; set; } = new();
    }

    /// <summary>One slot of the day, in order. The same slots apply on every teaching day.</summary>
    public class BellSlot : Base
    {
        public int BellScheduleId { get; set; }
        public BellSchedule? BellSchedule { get; set; }
        public int Rank { get; set; }
        public TimeOnly StartTime { get; set; }
        public TimeOnly EndTime { get; set; }
        public BellSlotKind Kind { get; set; } = BellSlotKind.Lesson;
        public DayPart Part { get; set; } = DayPart.Morning;
        // Shown on the timetable, e.g. "Tea break", "Lunch", "Assembly".
        [StringLength(100)]
        public string? Label { get; set; }
    }

    /// <summary>A grade that follows a bell schedule.</summary>
    public class BellScheduleLevel : Base
    {
        public int BellScheduleId { get; set; }
        public BellSchedule? BellSchedule { get; set; }
        public int LearningLevelId { get; set; }
        public LearningLevel? LearningLevel { get; set; }
    }

    /// <summary>
    /// A lesson slot on one day that is taken before the timetable is generated:
    /// either blocked for an activity (no subject - e.g. games on Wednesday
    /// afternoon) or given to a subject (e.g. PPI on Friday, first lesson). With
    /// no grade it applies to every grade on the schedule.
    /// </summary>
    public class TimetableFixedSlot : Base
    {
        public int BellScheduleId { get; set; }
        public BellSchedule? BellSchedule { get; set; }
        public int BellSlotId { get; set; }
        public BellSlot? BellSlot { get; set; }
        // ISO day number, 1 = Monday.
        public int Day { get; set; }
        public int? LearningLevelId { get; set; }
        public LearningLevel? LearningLevel { get; set; }
        public int? SubjectId { get; set; }
        public Subject? Subject { get; set; }
        // Shown in the blocked slot when there is no subject, e.g. "Games".
        [StringLength(100)]
        public string? Label { get; set; }
    }
}
