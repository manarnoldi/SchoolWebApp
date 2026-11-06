using SchoolWebApp.Core.Entities.Timetable;
using System.ComponentModel.DataAnnotations;

namespace SchoolWebApp.Core.DTOs.Timetable
{
    public class BellSlotDto
    {
        // 0 for a new slot.
        public int Id { get; set; }
        // "HH:mm".
        [Required] public string StartTime { get; set; } = "";
        [Required] public string EndTime { get; set; } = "";
        public BellSlotKind Kind { get; set; }
        public DayPart Part { get; set; }
        [StringLength(100)] public string? Label { get; set; }
    }

    public class FixedSlotDto
    {
        public int Id { get; set; }
        public int Day { get; set; }
        // Which slot of the schedule, by its position in the day (0-based) - new
        // slots have no id yet when the schedule is saved.
        public int SlotIndex { get; set; }
        public int? LearningLevelId { get; set; }
        public int? SubjectId { get; set; }
        [StringLength(100)] public string? Label { get; set; }
    }

    public class BellScheduleDto
    {
        public int Id { get; set; }
        [Required, StringLength(100)] public string Name { get; set; } = "";
        [StringLength(500)] public string? Description { get; set; }
        public List<int> Days { get; set; } = new();
        public List<BellSlotDto> Slots { get; set; } = new();
        public List<int> LearningLevelIds { get; set; } = new();
        public List<FixedSlotDto> FixedSlots { get; set; } = new();
    }

    public class TimetableSubjectRuleDto
    {
        public int Id { get; set; }
        public int SubjectId { get; set; }
        public int? LearningLevelId { get; set; }
        public bool Excluded { get; set; }
        public SubjectPlacement Placement { get; set; }
        public int DoubleLessons { get; set; }
        public int MaxPerDay { get; set; } = 1;
        public bool AvoidLastLesson { get; set; }
    }

    public class TimetableOptionGroupDto
    {
        public int Id { get; set; }
        [Required, StringLength(100)] public string Name { get; set; } = "";
        public int? LearningLevelId { get; set; }
        public List<int> SubjectIds { get; set; } = new();
    }

    public class TeacherUnavailabilityDto
    {
        public int Id { get; set; }
        public int Day { get; set; }
        [Required] public string StartTime { get; set; } = "";
        [Required] public string EndTime { get; set; } = "";
        [StringLength(255)] public string? Reason { get; set; }
    }

    public class TeacherTimetableRuleDto
    {
        public int StaffDetailsId { get; set; }
        public string? StaffName { get; set; }
        public int? MaxLessonsPerDay { get; set; }
        public int? MaxConsecutiveLessons { get; set; }
        [StringLength(500)] public string? Notes { get; set; }
        public List<TeacherUnavailabilityDto> Unavailable { get; set; } = new();
    }

    /// <summary>Whether the school is ready to generate a timetable for a term, and what is in the way.</summary>
    public class TimetableReadinessDto
    {
        public int SessionId { get; set; }
        public bool Ready { get; set; }
        public List<string> Problems { get; set; } = new();
        public List<string> Warnings { get; set; } = new();
        public List<string> LeftOut { get; set; } = new();
        public List<ReadinessClassDto> Classes { get; set; } = new();
        public List<ReadinessTeacherDto> Teachers { get; set; } = new();
    }

    public class ReadinessClassDto
    {
        public int SchoolClassId { get; set; }
        public string ClassName { get; set; } = "";
        public int LearningLevelId { get; set; }
        public string? BellScheduleName { get; set; }
        // Lessons the class needs a week (single lessons; a double counts two).
        public int LessonsNeeded { get; set; }
        // Lesson slots free for it a week, after fixed activities.
        public int SlotsAvailable { get; set; }
        public List<string> SubjectsWithoutTeacher { get; set; } = new();
        public List<ReadinessSubjectDto> Subjects { get; set; } = new();
    }

    public class ReadinessSubjectDto
    {
        public int SubjectId { get; set; }
        public string SubjectName { get; set; } = "";
        public int LessonsPerWeek { get; set; }
        public int? StaffDetailsId { get; set; }
        public string? TeacherName { get; set; }
        public bool Excluded { get; set; }
    }

    public class ReadinessTeacherDto
    {
        public int StaffDetailsId { get; set; }
        public string Name { get; set; } = "";
        public int LessonsAssigned { get; set; }
        // Lesson slots in the week the teacher could teach in, after unavailable times.
        public int SlotsAvailable { get; set; }
        public int Classes { get; set; }
    }
}
