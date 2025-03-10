using SchoolWebApp.Core.DTOs.Settings.OccurenceType;
using SchoolWebApp.Core.DTOs.Settings.Outcome;
using SchoolWebApp.Core.Entities.Settings;
using System.ComponentModel.DataAnnotations;

namespace SchoolWebApp.Core.DTOs
{
    public class DisciplineDto
    {
        public string? OccurenceDetails { get; set; }


        [Required(ErrorMessage = "Enter occurence start date")]
        [Display(Name = "Occurence start date")]
        public DateTime OccurenceStartDate { get; set; }

        [Required(ErrorMessage = "Enter occurence end date")]
        [Display(Name = "Occurence end date")]
        public DateTime OccurenceEndDate { get; set; }

        [Display(Name = "Outcome details")]
        [StringLength(255)]
        public string? OutcomeDetails { get; set; }

        public int? OutcomeId { get; set; }
        public OutcomeDto? Outcome { get; set; }
        public int? OccurenceTypeId { get; set; }
        public OccurenceTypeDto? OccurenceType { get; set; }
    }
}
