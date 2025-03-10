using SchoolWebApp.Core.DTOs.Academics.AcademicYear;
using SchoolWebApp.Core.DTOs.Academics.Curriculum;
using SchoolWebApp.Core.DTOs.Settings.SessionType;
using SchoolWebApp.Core.Entities.Academics;
using SchoolWebApp.Core.Entities.Settings;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace SchoolWebApp.Core.DTOs.Class.Session
{
    public class CreateSessionDto
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
        public AcademicYearDto? AcademicYear { get; set; }
        public int CurriculumId { get; set; }
        public CurriculumDto? Curriculum { get; set; }
        public int SessionTypeId { get; set; }
        public SessionTypeDto? SessionType { get; set; }
    }
}
