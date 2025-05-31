namespace SchoolWebApp.Core.DTOs
{
    public class AttendanceDto
    {
        public DateTime Date { get; set; }

        public bool? Present { get; set; }
        public string? Remarks { get; set; }

        public TimeOnly? TimeIn { get; set; }
        public TimeOnly? TimeOut { get; set; }
    }
}
