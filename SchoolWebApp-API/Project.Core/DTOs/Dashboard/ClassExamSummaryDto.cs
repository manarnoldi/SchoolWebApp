namespace SchoolWebApp.Core.DTOs.Dashboard
{
    /// <summary>
    /// One row of the dashboard's "Class Exam Performance" widget. Computed
    /// server-side so the client only needs a single round-trip instead of
    /// fanning out 3+ requests per class — see DashboardController.
    /// </summary>
    public class ClassExamSummaryDto
    {
        public int SchoolClassId { get; set; }
        public string ClassName { get; set; } = string.Empty;

        public int StudentCount { get; set; }

        public double ClassAverage { get; set; }
        public string ClassAvgGrade { get; set; } = string.Empty;

        public double HighestAvg { get; set; }
        public string HighestGrade { get; set; } = string.Empty;

        public double LowestAvg { get; set; }
        public string LowestGrade { get; set; } = string.Empty;

        public string TopStudent { get; set; } = string.Empty;
    }
}
