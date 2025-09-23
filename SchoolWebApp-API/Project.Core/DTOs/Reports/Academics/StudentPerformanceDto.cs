namespace SchoolWebApp.Core.DTOs.Reports.Academics
{
    public class StudentPerformanceDto
    {
        public int ExamTypeId { get; set; }
        public int StudentId { get; set; }
        public int SessionId { get; set; }
        public int ClassId { get; set; }
        public int AllocatedSubjectCount { get; set; }
        public List<SubjectPerformanceDto> SubjectPerformance { get; set; }
    }
}
