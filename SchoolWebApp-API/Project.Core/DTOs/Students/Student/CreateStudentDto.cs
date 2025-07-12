using SchoolWebApp.Core.DTOs.School.LearningMode;
using System.ComponentModel.DataAnnotations;

namespace SchoolWebApp.Core.DTOs.Students.Student
{
    public class CreateStudentDto: PersonDto
    {
        [Display(Name = "Addmission date")]
        public DateTime? AdmissionDate { get; set; }

        [Display(Name = "Application date")]
        public DateTime? ApplicationDate { get; set; }

        [Display(Name = "Health concerns")]
        [StringLength(500)]
        public string? HealthConcerns { get; set; }
        public int LearningModeId { get; set; }
        public LearningModeDto? LearningMode { get; set; }
    }
}
