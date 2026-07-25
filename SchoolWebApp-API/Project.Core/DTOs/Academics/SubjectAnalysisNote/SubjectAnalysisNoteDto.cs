namespace SchoolWebApp.Core.DTOs.Academics.SubjectAnalysisNote
{
    /// <summary>
    /// Read/write DTO for the Subject Performance Trend action-plan note. Id is 0
    /// for a not-yet-saved note; the controller upserts by (year, class, subject).
    /// </summary>
    public class SubjectAnalysisNoteDto
    {
        public int Id { get; set; }
        public int AcademicYearId { get; set; }
        public int SchoolClassId { get; set; }
        public int SubjectId { get; set; }
        public int SchoolExamId { get; set; }
        public string? ActionPlan { get; set; }
    }
}
