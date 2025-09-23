using SchoolWebApp.Core.Entities.CBE.Responsibilities;
using SchoolWebApp.Core.Entities.Class;
using SchoolWebApp.Core.Entities.Enums;
using SchoolWebApp.Core.Entities.Shared;
using SchoolWebApp.Core.Entities.Staff;
using SchoolWebApp.Core.Entities.Students;
using System.ComponentModel.DataAnnotations;

namespace SchoolWebApp.Core.Entities.Academics
{
    public class AcademicYear : Base
    {

        [Required(ErrorMessage = "Enter the academic year")]
        [Display(Name = "Academic year")]
        [StringLength(255)]
        public required string Name { get; set; }

        [Required(ErrorMessage = "Enter the academic year abbreviation")]
        [Display(Name = "Abbreviation")]
        [StringLength(255)]
        public required string Abbreviation { get; set; }

        [Display(Name = "Start date")]
        public DateTime StartDate { get; set; }

        [Display(Name = "End date")]
        public DateTime EndDate { get; set; }
        public int Rank { get; set; }

        [StringLength(500)]
        public string? Description { get; set; }
        public bool Status { get; set; }

        public List<SchoolClass> SchoolClasses { get; set; } = new();
        public List<Session> Sessions { get; set; } = new();
        public List<EducationLevelSubject> educationLevelSubjects { get; set; } = new();
        public List<StudentResponsibility> StudentResponsibilities { get; set; } = new();
    }
}
