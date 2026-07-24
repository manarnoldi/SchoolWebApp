namespace SchoolWebApp.Core.DTOs.Dashboard
{
    /// <summary>
    /// One row of the dashboard's "Class Exam Performance" widget. Computed
    /// server-side so the client only needs a single round-trip instead of
    /// fanning out 3+ requests per class - see DashboardController.
    /// </summary>
    public class ClassExamSummaryDto
    {
        public int SchoolClassId { get; set; }
        public string ClassName { get; set; } = string.Empty;

        // Ranking group this class belongs to (an education level type or an
        // education level, per the ClassPerformanceRankingBasis setting). Rows
        // are returned ordered by GroupOrder then class average, so the client
        // can render grouped sections. GroupOrder is for ordering only.
        public string GroupName { get; set; } = string.Empty;
        public int GroupOrder { get; set; }

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
