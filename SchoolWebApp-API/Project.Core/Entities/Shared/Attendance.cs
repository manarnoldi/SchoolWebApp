using System.ComponentModel.DataAnnotations;

namespace SchoolWebApp.Core.Entities.Shared
{
    public class Attendance: Base
    {
        [Display(Name = "Attendance date")]
        public DateTime Date { get; set; }

        public bool Present { get; set; }

        public TimeOnly? TimeIn { get; set; }
        public TimeOnly? TimeOut { get; set; }

        [StringLength(500)]
        public string? Remarks { get; set; }
    }
}
