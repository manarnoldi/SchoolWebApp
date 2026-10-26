using SchoolWebApp.Core.Entities.Identity;
using SchoolWebApp.Core.Entities.Shared;
using System.ComponentModel.DataAnnotations;

namespace SchoolWebApp.Core.Entities.TeachingDocuments
{
    public enum LessonTaughtStatus
    {
        NotRecorded = 0,
        Taught = 1,
        PartlyTaught = 2,
        NotTaught = 3
    }

    /// <summary>
    /// What actually happened to a scheme lesson. The record of work is these
    /// entries read alongside the scheme - nothing is typed twice.
    /// </summary>
    public class RecordOfWorkEntry : Base
    {
        public int SchemeOfWorkLessonId { get; set; }
        public SchemeOfWorkLesson? SchemeOfWorkLesson { get; set; }
        public LessonTaughtStatus Status { get; set; }
        public DateTime? TaughtOn { get; set; }
        public string? WorkCovered { get; set; }
        public string? Remarks { get; set; }
    }

    /// <summary>
    /// The HOD's weekly "checked" stamp on a scheme's record of work. A checked
    /// week's entries are locked until the check is removed.
    /// </summary>
    public class RecordOfWorkCheck : Base
    {
        public int SchemeOfWorkId { get; set; }
        public SchemeOfWork? SchemeOfWork { get; set; }
        public int WeekNo { get; set; }
        public int CheckedByUserId { get; set; }
        public AppUser? CheckedBy { get; set; }
        public DateTime CheckedAt { get; set; }
        [StringLength(1000)]
        public string? Comment { get; set; }
    }
}
