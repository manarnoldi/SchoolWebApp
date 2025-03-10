using SchoolWebApp.Core.Entities.Academics;
using SchoolWebApp.Core.Entities.School;
using SchoolWebApp.Core.Entities.Settings;
using SchoolWebApp.Core.Entities.Shared;
using System.ComponentModel.DataAnnotations;

namespace SchoolWebApp.Core.Entities.Class
{
    public class Session : Base
    {

        [Required(ErrorMessage = "Enter the session name.")]
        [Display(Name = "Session name")]
        [StringLength(255)]
        public required string SessionName { get; set; }

        [Required(ErrorMessage = "Enter the session abbreviation.")]
        [Display(Name = "Session abbreviation")]
        [StringLength(255)]
        public required string Abbreviation { get; set; }

        [Required(ErrorMessage = "Enter the session start date.")]
        [Display(Name = "Session start date")]
        public DateTime StartDate { get; set; }

        [Required(ErrorMessage = "Enter the session end date.")]
        [Display(Name = "Session end date")]
        public DateTime EndDate { get; set; }
        public bool Status { get; set; }

        public int AcademicYearId { get; set; }
        public AcademicYear? AcademicYear { get; set; }
        public int CurriculumId { get; set; }
        public Curriculum? Curriculum { get; set; }
        public int SessionTypeId { get; set; }
        public SessionType? SessionType { get; set; }

        public List<Exam> Exams { get; set; } = new();
        public List<Event> Events { get; set; } = new();
    }
}
