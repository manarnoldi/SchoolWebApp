﻿using SchoolWebApp.Core.DTOs.Settings.Occupation;
using System.ComponentModel.DataAnnotations;

namespace SchoolWebApp.Core.DTOs.Students.Parent
{
    public class CreateParentDto: PersonDto
    {
        [Display(Name = "Notify about student progress")]
        public bool Notifiable { get; set; }

        [Display(Name = "Pays school fees")]
        public bool Payer { get; set; }

        [Display(Name = "Picks up student")]
        public bool Pickup { get; set; }

        public int OccupationId { get; set; }
        public OccupationDto? Occupation { get; set; }
    }
}
