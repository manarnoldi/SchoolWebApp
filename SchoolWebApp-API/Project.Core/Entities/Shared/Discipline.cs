using SchoolWebApp.Core.Entities.Settings;
using System.ComponentModel.DataAnnotations;

namespace SchoolWebApp.Core.Entities.Shared
{
    public class Discipline : Base
    {
        [Required(ErrorMessage = "Enter occurence details")]
        [Display(Name = "Occurence details")]
        [StringLength(500)]
        public string OccurenceDetails { get; set; }


        [Required(ErrorMessage = "Enter occurence start date")]
        [Display(Name = "Occurence start date")]
        public DateTime OccurenceStartDate { get; set; }

        [Required(ErrorMessage = "Enter occurence end date")]
        [Display(Name = "Occurence end date")]
        public DateTime OccurenceEndDate { get; set; }
        
        [Display(Name = "Outcome details")]
        [StringLength(255)]
        public string OutcomeDetails { get; set; }

        public int OutcomeId { get; set; }
        public Outcome Outcome { get; set; }
        public int OccurenceTypeId { get; set; }
        public OccurenceType OccurenceType { get; set; }
    }
}
