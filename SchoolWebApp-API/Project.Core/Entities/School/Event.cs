using SchoolWebApp.Core.Entities.Class;
using SchoolWebApp.Core.Entities.Enums;
using SchoolWebApp.Core.Entities.Shared;
using System.ComponentModel.DataAnnotations;

namespace SchoolWebApp.Core.Entities.School
{
    public class Event : Base
    {
        [Required(ErrorMessage = "Enter the event name.")]
        [Display(Name ="Event name")]
        [StringLength(500)]
        public string EventName { get; set; }

        [Required(ErrorMessage = "Enter the event location.")]
        [Display(Name ="Event location")]
        [StringLength(255)]
        public string EventLocation { get; set; }

        [Required(ErrorMessage = "Select the event start date.")]
        [Display(Name ="Event start date")]
        public DateTime StartDate { get; set; }

        [Display(Name ="Event start date")]
        [Required(ErrorMessage = "Select the event end date.")]
        public DateTime EndDate { get; set; }

        [Display(Name = "Event year")]
        [Required(ErrorMessage = "Select the event year.")]
        public int EventYear { get; set; }

        [StringLength(500)]
        public string Description { get; set; }
        public Status Status { get; set; }

        public int SessionId { get; set; }
        public Session Session { get; set; }        
    }
}
