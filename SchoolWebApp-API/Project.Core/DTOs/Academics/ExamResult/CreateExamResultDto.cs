﻿using SchoolWebApp.Core.DTOs.Academics.Exam;
using SchoolWebApp.Core.DTOs.Students.StudentSubjects;
using System.ComponentModel.DataAnnotations;

namespace SchoolWebApp.Core.DTOs.Academics.ExamResult
{
    public class CreateExamResultDto
    {
        [Required(ErrorMessage = "Enter the examination score")]
        [Display(Name = "Examination score")]
        public float Score { get; set; }

        public int StudentSubjectId { get; set; }
        public StudentSubjectDto? StudentSubject { get; set; }
        public int ExamId { get; set; }
        public ExamDto? Exam { get; set; }
    }
}
