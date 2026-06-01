namespace SchoolWebApp.Core.DTOs.Academics.SchoolExam
{
    public class SchoolExamDto : CreateSchoolExamDto
    {
        public int Id { get; set; }

        public bool IsReleased { get; set; }
        public string? ReleasedBy { get; set; }
        public DateTime? ReleasedDate { get; set; }
        public bool ParentsNotified { get; set; }
        public DateTime? ParentsNotifiedDate { get; set; }
    }
}
