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
        public string Name { get; set; }

        [Required(ErrorMessage = "Enter the academic year abbreviation")]
        [Display(Name = "Abbreviation")]
        [StringLength(255)]
        public string Abbreviation { get; set; }

        [Display(Name = "Start date")]
        public DateTime StartDate { get; set; }

        [Display(Name = "End date")]
        public DateTime EndDate { get; set; }

        [StringLength(500)]
        public string Description { get; set; }
        public Status Status { get; set; }

        public List<SchoolClass> SchoolClasses { get; set; }
        public List<StaffSubject> StaffSubjects { get; set; }
        public List<StudentSubject> StudentSubjects { get; set; }
        public List<Session> Sessions { get; set; }
    }
}
