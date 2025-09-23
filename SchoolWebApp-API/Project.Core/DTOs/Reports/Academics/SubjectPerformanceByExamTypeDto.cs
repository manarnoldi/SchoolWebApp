namespace SchoolWebApp.Core.DTOs.Reports.Academics
{
    public class SubjectPerformanceByExamTypeDto
    {
        public int SubjectId { get; set; }
        public double TotalWeightedScore { get; set; }
        public double TotalRawScore { get; set; }
        public List<ExamNameBreakdownDto> BreakdownByExamName { get; set; }
        public int Rank { get; set; }
        public int RankOutOf { get; set; }
    }
}
