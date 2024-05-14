using SchoolWebApp.Core.Entities.Class;
using SchoolWebApp.Core.Entities.School;
using SchoolWebApp.Core.Entities.Shared;
using System.ComponentModel.DataAnnotations;

namespace SchoolWebApp.Core.Entities.Academics
{
    public class Curriculum : Base
    {
        [Required(ErrorMessage = "Enter the curriculum code")]
        [Display(Name = "Curriculum code")]
        [StringLength(255)]
        public string Code { get; set; }

        [Required(ErrorMessage = "Enter the curriculum name")]
        [Display(Name = "Curriculum name")]
        [StringLength(255)]
        public string Name { get; set; }

        [StringLength(500)]
        public string Description { get; set; }

        public List<EducationLevel> EducationLevels { get; set; }
        public List<SubjectGroup> SubjectGroups { get; set; }
        public List<Grade> Grades { get; set; }
        public List<Session> Sessions { get; set; }
    }
}
