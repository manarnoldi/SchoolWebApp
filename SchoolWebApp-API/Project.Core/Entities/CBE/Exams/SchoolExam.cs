using SchoolWebApp.Core.Entities.Class;
using SchoolWebApp.Core.Entities.Shared;

namespace SchoolWebApp.Core.Entities.CBE.Exams
{
    /// <summary>
    /// The exam "event" header - e.g. "Opening Exam, Term 2 2026, CBE".
    /// Holds the schedule (start/end/mark-entry dates) and the release
    /// workflow once, so a deadline can be changed in one place and the
    /// whole exam released at once. The per-class-per-subject detail
    /// (mark, results) lives on the child <see cref="Exam"/> rows.
    /// The Session FK gives term + academic year + curriculum.
    /// </summary>
    public class SchoolExam : Base
    {
        public DateOnly ExamStartDate { get; set; }
        public DateOnly ExamEndDate { get; set; }
        public DateOnly ExamMarkEntryEndDate { get; set; }

        public string? Description { get; set; }

        public int ExamTypeId { get; set; }
        public ExamType? ExamType { get; set; }
        public int SessionId { get; set; }
        public Session? Session { get; set; }

        // Release workflow. Releasing publishes results (e.g. to the
        // dashboard summary) and is the hook for parent notifications.
        public bool IsReleased { get; set; }
        public string? ReleasedBy { get; set; }
        public DateTime? ReleasedDate { get; set; }

        // Parent-notification stubs - persisted now, wired to delivery later.
        public bool ParentsNotified { get; set; }
        public DateTime? ParentsNotifiedDate { get; set; }

        public List<Exam> Exams { get; set; } = new();
    }
}
