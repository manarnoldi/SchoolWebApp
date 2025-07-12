using SchoolWebApp.Core.Entities.School;
using SchoolWebApp.Core.Entities.Shared;
using SchoolWebApp.Core.Entities.Staff;
using SchoolWebApp.Core.Entities.Students;
using System.ComponentModel.DataAnnotations;

namespace SchoolWebApp.Core.Entities.Academics
{
    public class Subject : Base
    {
        [Required(ErrorMessage = "Enter the subject code")]
        [Display(Name = "Subject code")]
        [StringLength(255)]
        public required string Code { get; set; }

        [Required(ErrorMessage = "Enter the subject name")]
        [Display(Name = "Subject name")]
        [StringLength(255)]
        public required string Name { get; set; }

        [Required(ErrorMessage = "Enter the subject abbreviation")]
        [Display(Name = "Subject abbreviation")]
        [StringLength(255)]
        public required string Abbr { get; set; }

        public int NumOfLessons { get; set; }
        public string? Description { get; set; }
        public bool Optional { get; set; }
        public int Rank { get; set; }
        public int SubjectGroupId { get; set; }
        public SubjectGroup? SubjectGroup { get; set; }

        public int DepartmentId { get; set; }
        public Department? Department { get; set; }

        public int? StaffDetailsId { get; set; }
        public StaffDetails? StaffDetails { get; set; }

        public List<StudentSubject> StudentSubjects { get; set; } = new();
        public List<StaffSubject> StaffSubjects { get; set; } = new();
        public List<Exam> Exams { get; set; } = new();
        public List<EducationLevelSubject> educationLevelSubjects { get; set; } = new();
    }
}
