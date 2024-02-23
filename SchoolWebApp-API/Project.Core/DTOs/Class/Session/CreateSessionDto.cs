using System.ComponentModel.DataAnnotations;

namespace SchoolWebApp.Core.DTOs.Class.Session
{
    public class CreateSessionDto
    {
        [Required(ErrorMessage = "Enter the session name.")]
        [Display(Name = "Session name")]
        [StringLength(255)]
        public string SessionName { get; set; }

        [Required(ErrorMessage = "Enter the session abbreviation.")]
        [Display(Name = "Session abbreviation")]
        [StringLength(255)]
        public string Abbreviation { get; set; }

        [Required(ErrorMessage = "Enter the session start date.")]
        [Display(Name = "Session start date")]
        public DateTime StartDate { get; set; }

        [Required(ErrorMessage = "Enter the session end date.")]
        [Display(Name = "Session end date")]
        public DateTime EndDate { get; set; }
        public bool Status { get; set; }

        public int AcademicYearId { get; set; }
        public int CurriculumId { get; set; }
        public int SessionTypeId { get; set; }
    }
}
