using SchoolWebApp.Core.Entities.Academics;
using SchoolWebApp.Core.Entities.CBE.Assessments;
using SchoolWebApp.Core.Entities.CBE.Cocurriculum;
using SchoolWebApp.Core.Entities.CBE.Exams;
using SchoolWebApp.Core.Entities.CBE.Responsibilities;
using SchoolWebApp.Core.Entities.CBE.Values;
using SchoolWebApp.Core.Entities.Class;
using SchoolWebApp.Core.Entities.School;
using SchoolWebApp.Core.Entities.Shared;
using System.ComponentModel.DataAnnotations;

namespace SchoolWebApp.Core.Entities.Students
{
    public  class Student: Person
    {                  
        [Display(Name = "Addmission date")]
        public DateTime? AdmissionDate { get; set; }

        [Display(Name = "Application date")]
        public DateTime? ApplicationDate { get; set; }        

        [Display(Name = "Health concerns")]
        [StringLength(500)]
        public string? HealthConcerns { get; set; }      
        
        public int LearningModeId { get; set; }
        public LearningMode? LearningMode { get; set; }

        public List<Parent> Parents { get; set; } = new();
        public List<StudentParent> StudentParents { get; set; } = new();
        public List<FormerSchool> FormerSchools { get; set; } = new();
        public List<StudentDiscipline> StudentDisciplines { get; set; } = new();
        public List<StudentClass> StudentClasses{ get; set; } = new();
        public List<StudentValueScore> StudentValues { get; set; } = new();
        public List<StudentCoCurriculumActivity> StudentCoCurriculumActivities { get; set; } = new();
        public List<Assessment> Assessments { get; set; } = new();
        public List<ExamResult> ExamResults { get; set; } = new();
        public List<StudentResponsibility> StudentResponsibilities { get; set; } = new();

    }
}
