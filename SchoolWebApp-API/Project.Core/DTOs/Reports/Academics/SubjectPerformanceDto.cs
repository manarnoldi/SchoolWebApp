namespace SchoolWebApp.Core.DTOs.Reports.Academics
{
    public class SubjectPerformanceDto
    {
        public int SubjectId { get; set; }
        public double? Score { get; set; }
        public double? ExamMark { get; set; }
        public double? ContributingMark { get; set; }
    }
}
