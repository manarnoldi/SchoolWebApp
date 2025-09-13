namespace SchoolWebApp.Core.DTOs.Reports.Academics
{
    public class StudentExamTypePerformanceDto
    {
        public int ExamTypeId { get; set; }
        public int StudentId { get; set; }
        public int ClassId { get; set; }
        public int SessionId { get; set; }
        public int SubjectCount { get; set; }
        public List<SubjectPerformanceByExamTypeDto> SubjectPerformance { get; set; }
        public double TotalWeightedScore => SubjectPerformance.Sum(s => s.TotalWeightedScore);
        public double TotalRawScore => SubjectPerformance.Sum(s => s.TotalRawScore);
        public double MeanMark { get; set; }
        public string? MeanMarkGrade { get; set; }
        public double? MeanMarkPoints { get; set; }
    }
}
