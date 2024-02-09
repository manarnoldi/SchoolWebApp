using SchoolWebApp.Core.Entities.Academics;
using SchoolWebApp.Core.Entities.Class;
using SchoolWebApp.Core.Entities.School;
using SchoolWebApp.Core.Entities.Shared;
using System.ComponentModel.DataAnnotations;

namespace SchoolWebApp.Core.Entities.Students
{
    public  class Student: Person
    {                  
        [Display(Name = "Addmission date")]
        public DateTime AdmissionDate { get; set; }

        [Display(Name = "Application date")]
        public DateTime ApplicationDate { get; set; }        

        [Display(Name = "Health concerns")]
        [StringLength(500)]
        public string HealthConcerns { get; set; }      
        
        public int LearningModeId { get; set; }
        public LearningMode LearningMode { get; set; }

        public List<SchoolClass> SchoolClasses { get; set; }
        public List<StudentParent> Parents { get; set; }
        public List<FormerSchool> FormerSchools { get; set; }
        public List<StudentSubject> StudentSubjects { get; set; }
        public List<StudentDiscipline> StudentDisciplines { get; set; }
        public List<StudentClass> StudentClasses{ get; set; }
        public List<ExamResult> ExamResults { get; set; }
    }
}
