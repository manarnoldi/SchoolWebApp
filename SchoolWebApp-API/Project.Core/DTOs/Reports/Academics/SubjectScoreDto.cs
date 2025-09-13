namespace SchoolWebApp.Core.DTOs.Reports.Academics
{
    public class SubjectScoreDto
    {
        public int SubjectId { get; set; }
        public string SubjectCode { get; set; }
        public float? Score { get; set; }  // Final weighted subject score
        public List<ExamComponentScoreDto> ExamComponents { get; set; } = new();
    }
}
