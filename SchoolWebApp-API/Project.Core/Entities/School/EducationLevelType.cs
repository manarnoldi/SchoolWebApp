using SchoolWebApp.Core.Entities.CBE.Assessments;
using SchoolWebApp.Core.Entities.Shared;
using System.ComponentModel.DataAnnotations;

namespace SchoolWebApp.Core.Entities.School
{
    public class EducationLevelType : Base
    {
        [Required(ErrorMessage = "Enter the education level type name.")]
        [Display(Name = "Education level type name")]
        [StringLength(255)]
        public required string Name { get; set; }
        [StringLength(255)]
        public string? Abbr { get; set; }
        public int Rank { get; set; }
        public string? Description { get; set; }

        public List<EducationLevel> EducationLevels { get; set; } = new();
        public List<GeneralOutcome> GeneralOutcomes { get; set; } = new();
    }
}
