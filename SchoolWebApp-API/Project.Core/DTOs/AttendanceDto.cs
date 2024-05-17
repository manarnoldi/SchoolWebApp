namespace SchoolWebApp.Core.DTOs
{
    public class AttendanceDto
    {
        public DateTime Date { get; set; }

        public bool Present { get; set; }
        public string? Remarks { get; set; }
    }
}
