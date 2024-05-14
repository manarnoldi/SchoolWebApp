using SchoolWebApp.Core.Entities.Shared;
using System.ComponentModel.DataAnnotations;

namespace SchoolWebApp.Core.Entities.School
{
    public class EducationLevelType : Base
    {
        [Required(ErrorMessage = "Enter the education level type name.")]
        [Display(Name = "Education level type name")]
        [StringLength(255)]
        public int Name { get; set; }
        [StringLength(255)]
        public string? Abbr { get; set; }
        public string? Description { get; set; }

        public List<EducationLevel> EducationLevels { get; set; }
    }
}
