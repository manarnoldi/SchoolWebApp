using SchoolWebApp.Core.Entities.Academics;
using SchoolWebApp.Core.Entities.Class;
using SchoolWebApp.Core.Entities.Shared;
using SchoolWebApp.Core.Entities.Staff;
using System.ComponentModel.DataAnnotations;

namespace SchoolWebApp.Core.Entities.Timetable
{
    /// <summary>
    /// A timetable is edited as a Draft and Published from a date; publishing a
    /// new one archives the one it replaces, so past terms keep their timetable.
    /// </summary>
    public enum TimetableStatus
    {
        Draft = 0,
        Published = 1,
        Archived = 2
    }

    /// <summary>One version of the school's timetable for a term.</summary>
    public class SchoolTimetable : Base
    {
        [StringLength(150)]
        public required string Name { get; set; }
        public int SessionId { get; set; }
        public Session? Session { get; set; }
        public TimetableStatus Status { get; set; } = TimetableStatus.Draft;
        public DateTime? EffectiveFrom { get; set; }
        public DateTime? PublishedAt { get; set; }
        [StringLength(255)]
        public string? PublishedBy { get; set; }
        public DateTime? GeneratedAt { get; set; }
        // The generator's report - what it could not place and why - as JSON.
        public string? GenerationReport { get; set; }
        [StringLength(1000)]
        public string? Notes { get; set; }
        public List<TimetableLesson> Lessons { get; set; } = new();
    }

    /// <summary>
    /// A card on the timetable: one lesson (or double lesson) of a subject for a
    /// class, with its teacher. Unplaced while Day / BellSlotId are empty. A
    /// locked card stays where it is when the timetable is regenerated.
    /// </summary>
    public class TimetableLesson : Base
    {
        public int SchoolTimetableId { get; set; }
        public SchoolTimetable? SchoolTimetable { get; set; }
        public int SchoolClassId { get; set; }
        public SchoolClass? SchoolClass { get; set; }
        public int SubjectId { get; set; }
        public Subject? Subject { get; set; }
        public int? StaffDetailsId { get; set; }
        public StaffDetails? StaffDetails { get; set; }
        // ISO day number, 1 = Monday; null while unplaced.
        public int? Day { get; set; }
        // The first slot the lesson takes; a double also takes the next lesson slot.
        public int? BellSlotId { get; set; }
        public BellSlot? BellSlot { get; set; }
        // 1 = single lesson, 2 = double.
        public int Length { get; set; } = 1;
        // Cards of an option group's meeting (e.g. CRE and IRE together) share a
        // link number within the timetable and always move together.
        public int? LinkNo { get; set; }
        public bool IsLocked { get; set; }
        // Placed by hand rather than by the generator.
        public bool IsManual { get; set; }
    }
}
