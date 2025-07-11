﻿using SchoolWebApp.Core.Entities.Shared;
using SchoolWebApp.Core.Entities.Students;
using System.ComponentModel.DataAnnotations;

namespace SchoolWebApp.Core.Entities.Academics
{
    public class ExamResult : Base
    {
        [Required(ErrorMessage = "Enter the examination score")]
        [Display(Name = "Examination score")]
        public float Score { get; set; }

        public int StudentId { get; set; }
        public Student? Student { get; set; }
        public int ExamId { get; set; }
        public Exam? Exam { get; set; }
    }
}
