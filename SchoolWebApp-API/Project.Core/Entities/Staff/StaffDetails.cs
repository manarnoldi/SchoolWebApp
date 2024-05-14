using SchoolWebApp.Core.Entities.Academics;
using SchoolWebApp.Core.Entities.Class;
using SchoolWebApp.Core.Entities.School;
using SchoolWebApp.Core.Entities.Settings;
using SchoolWebApp.Core.Entities.Shared;
using System.ComponentModel.DataAnnotations;

namespace SchoolWebApp.Core.Entities.Staff
{
    public class StaffDetails : Person
    {
        [Display(Name = "School identity number")]
        [StringLength(255)]
        public string? IdNumber { get; set; }

        public int StaffCategoryId { get; set; }
        public StaffCategory StaffCategory { get; set; }
        public int DesignationId { get; set; }
        public Designation Designation { get; set; }
        public int EmploymentTypeId { get; set; }
        public EmploymentType EmploymentType { get; set; }

        public List<StaffSubject> StaffSubjects { get; set; }
        public List<SchoolClass> SchoolClasses { get; set; }
        public List<StaffAttendance> StaffAttendances { get; set; }
        public List<StaffDiscipline> StaffDisciplines { get; set; }
        public List<Department> Departments { get; set; }
        public List<Subject> Subjects { get; set; }
    }
}
