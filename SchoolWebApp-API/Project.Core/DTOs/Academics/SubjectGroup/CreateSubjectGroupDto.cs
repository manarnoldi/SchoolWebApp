﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchoolWebApp.Core.DTOs.Academics.SubjectGroup
{
    public class CreateSubjectGroupDto
    {
        [Required(ErrorMessage = "Enter the subject group")]
        [Display(Name = "Subject group name")]
        [StringLength(255)]
        public string Name { get; set; }
        public string Abbreviation { get; set; }
        public string Description { get; set; }
        public int CurriculumId { get; set; }
    }
}
