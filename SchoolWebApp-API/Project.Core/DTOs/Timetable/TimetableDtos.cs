using SchoolWebApp.Core.Entities.Timetable;
using System.ComponentModel.DataAnnotations;

namespace SchoolWebApp.Core.DTOs.Timetable
{
    public class TimetableSummaryDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public int SessionId { get; set; }
        public string SessionName { get; set; } = "";
        public TimetableStatus Status { get; set; }
        public DateTime? EffectiveFrom { get; set; }
        public DateTime? GeneratedAt { get; set; }
        public DateTime? PublishedAt { get; set; }
        public string? Notes { get; set; }
        // Cards (an option group's meeting counts once per subject).
        public int Lessons { get; set; }
        public int Unplaced { get; set; }
        public DateTime? Modified { get; set; }
    }

    public class CreateTimetableDto
    {
        [Required, StringLength(150)] public string Name { get; set; } = "";
        public int SessionId { get; set; }
        [StringLength(1000)] public string? Notes { get; set; }
    }

    public class UpdateTimetableDto
    {
        [Required, StringLength(150)] public string Name { get; set; } = "";
        [StringLength(1000)] public string? Notes { get; set; }
    }

    public class PublishTimetableDto
    {
        public DateTime EffectiveFrom { get; set; }
    }

    public class GenerateTimetableDto
    {
        // Seconds the generator may take; it stops early once everything is placed.
        public int Seconds { get; set; } = 20;
    }

    /// <summary>Everything a timetable page draws: the grid shapes, the people and the cards.</summary>
    public class TimetableDetailDto : TimetableSummaryDto
    {
        public TimetableReportDto? Report { get; set; }
        public List<TimetableScheduleDto> Schedules { get; set; } = new();
        public List<TimetableClassDto> Classes { get; set; } = new();
        public List<TimetablePersonDto> Teachers { get; set; } = new();
        public List<TimetableSubjectDto> Subjects { get; set; } = new();
        public List<TimetableLessonDto> Cards { get; set; } = new();
    }

    public class TimetableScheduleDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public List<int> Days { get; set; } = new();
        public List<TimetableSlotDto> Slots { get; set; } = new();
        public List<TimetableFixedDto> Fixed { get; set; } = new();
    }

    public class TimetableSlotDto
    {
        public int Id { get; set; }
        public int Rank { get; set; }
        public string StartTime { get; set; } = "";
        public string EndTime { get; set; } = "";
        public BellSlotKind Kind { get; set; }
        public DayPart Part { get; set; }
        public string? Label { get; set; }
    }

    public class TimetableFixedDto
    {
        public int Day { get; set; }
        public int BellSlotId { get; set; }
        public int? LearningLevelId { get; set; }
        public int? SubjectId { get; set; }
        public string? Label { get; set; }
    }

    public class TimetableClassDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public int LearningLevelId { get; set; }
        public int ScheduleId { get; set; }
    }

    public class TimetablePersonDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public string Initials { get; set; } = "";
    }

    public class TimetableSubjectDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public string Abbr { get; set; } = "";
    }

    public class TimetableLessonDto
    {
        public int Id { get; set; }
        public int SchoolClassId { get; set; }
        public int SubjectId { get; set; }
        public int? StaffDetailsId { get; set; }
        public int? Day { get; set; }
        public int? BellSlotId { get; set; }
        public int Length { get; set; }
        public int? LinkNo { get; set; }
        public bool IsLocked { get; set; }
    }

    /// <summary>What the generator reports, kept on the timetable as JSON.</summary>
    public class TimetableReportDto
    {
        public int Placed { get; set; }
        public int Total { get; set; }
        public int Attempts { get; set; }
        public double Seconds { get; set; }
        public double Score { get; set; }
        public List<TimetableUnplacedDto> Unplaced { get; set; } = new();
        public List<string> Warnings { get; set; } = new();
        public List<string> LeftOut { get; set; } = new();
    }

    public class TimetableUnplacedDto
    {
        public string ClassName { get; set; } = "";
        public string Subjects { get; set; } = "";
        public string Teachers { get; set; } = "";
        public int Length { get; set; }
        public string Reason { get; set; } = "";
    }
}
