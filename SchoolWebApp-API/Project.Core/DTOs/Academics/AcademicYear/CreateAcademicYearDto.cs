using SchoolWebApp.Core.Entities.Enums;
using System.ComponentModel.DataAnnotations;

namespace SchoolWebApp.Core.DTOs.Academics.AcademicYear
{
    public class CreateAcademicYearDto
    {
        [Required(ErrorMessage = "Enter the academic year")]
        [Display(Name = "Academic year")]
        [StringLength(255)]
        public required string Name { get; set; }

        [Required(ErrorMessage = "Enter the academic year abbreviation")]
        [Display(Name = "Abbreviation")]
        [StringLength(255)]
        public required string Abbreviation { get; set; }

        [Display(Name = "Start date")]
        public DateTime StartDate { get; set; }

        [Display(Name = "End date")]
        public DateTime EndDate { get; set; }

        [StringLength(500)]
        public string? Description { get; set; }
        public bool Status { get; set; }
    }
}
