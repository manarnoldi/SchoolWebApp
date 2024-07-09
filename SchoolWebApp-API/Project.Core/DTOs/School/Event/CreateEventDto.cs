using SchoolWebApp.Core.DTOs.Class.Session;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchoolWebApp.Core.DTOs.School.Event
{
    public class CreateEventDto
    {
        [Required(ErrorMessage = "Enter the event name.")]
        [Display(Name = "Event name")]
        [StringLength(500)]
        public required string EventName { get; set; }

        [Required(ErrorMessage = "Enter the event location.")]
        [Display(Name = "Event location")]
        [StringLength(255)]
        public required string EventLocation { get; set; }

        [Required(ErrorMessage = "Select the event start date.")]
        [Display(Name = "Event start date")]
        public DateTime StartDate { get; set; }

        [Display(Name = "Event start date")]
        [Required(ErrorMessage = "Select the event end date.")]
        public DateTime EndDate { get; set; }

        [StringLength(500)]
        public string? Description { get; set; }

        public int SessionId { get; set; }
        public SessionDto? Session { get; set; }
    }
}
