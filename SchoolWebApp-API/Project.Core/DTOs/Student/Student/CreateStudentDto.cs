using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchoolWebApp.Core.DTOs.Student.Student
{
    public class CreateStudentDto: PersonDto
    {
        [Display(Name = "Addmission date")]
        public DateTime AdmissionDate { get; set; }

        [Display(Name = "Application date")]
        public DateTime ApplicationDate { get; set; }

        [Display(Name = "Health concerns")]
        [StringLength(500)]
        public string HealthConcerns { get; set; }
        public int LearningModeId { get; set; }
    }
}
