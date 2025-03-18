using SchoolWebApp.Core.Entities.Class;
using SchoolWebApp.Core.Entities.School;
using SchoolWebApp.Core.Entities.Shared;
using SchoolWebApp.Core.Entities.Students;
using System.ComponentModel.DataAnnotations;

namespace SchoolWebApp.Core.Entities.Academics
{
    public class Curriculum : Base
    {
        [Required(ErrorMessage = "Enter the curriculum code")]
        [Display(Name = "Curriculum code")]
        [StringLength(255)]
        public required string Code { get; set; }

        [Required(ErrorMessage = "Enter the curriculum name")]
        [Display(Name = "Curriculum name")]
        [StringLength(255)]
        public required string Name { get; set; }

        [StringLength(500)]
        public string? Description { get; set; }
        public int Rank { get; set; }
        public List<FormerSchool> FormerSchools  { get; set; } = new();
        public List<EducationLevel> EducationLevels { get; set; } = new();
        public List<SubjectGroup> SubjectGroups { get; set; } = new();
        public List<Grade> Grades { get; set; } = new();
        public List<Session> Sessions { get; set; } = new();
    }
}
