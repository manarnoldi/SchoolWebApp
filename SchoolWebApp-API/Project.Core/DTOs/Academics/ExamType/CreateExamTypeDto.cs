using SchoolWebApp.Core.DTOs.Settings;
using System.ComponentModel.DataAnnotations;

namespace SchoolWebApp.Core.DTOs.Academics.ExamType
{
    public class CreateExamTypeDto: BaseSettinsDto
    {
        [Required(ErrorMessage = "Enter the exam type abbreviation")]
        [Display(Name = "Abbreviation")]
        [StringLength(255)]
        public required string Abbreviation { get; set; }

        /// <summary>Results show on the report form, in a column of their own.</summary>
        public bool ShowOnReportForm { get; set; }

        /// <summary>A term may hold more than one exam of this type (weekly marathons).</summary>
        public bool AllowMultiplePerTerm { get; set; }
    }
}
