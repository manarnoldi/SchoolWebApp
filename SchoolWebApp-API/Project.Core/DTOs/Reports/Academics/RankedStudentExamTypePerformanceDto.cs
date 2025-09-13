namespace SchoolWebApp.Core.DTOs.Reports.Academics
{
    public class RankedStudentExamTypePerformanceDto: StudentExamTypePerformanceDto
    {
        public string StudentFullName { get; set; }
        public int Rank { get; set; }
    }
}
