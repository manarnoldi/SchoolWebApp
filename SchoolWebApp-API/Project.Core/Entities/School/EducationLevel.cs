using SchoolWebApp.Core.Entities.Academics;
using SchoolWebApp.Core.Entities.CBE.Assessments;
using SchoolWebApp.Core.Entities.Class;
using SchoolWebApp.Core.Entities.Shared;
using System.ComponentModel.DataAnnotations;

namespace SchoolWebApp.Core.Entities.School
{
    public class EducationLevel : Base
    {
        [Required(ErrorMessage = "Enter the education level name.")]
        [Display(Name = "Education level name")]
        [StringLength(255)]
        public required string Name { get; set; }
        [StringLength(255)]
        public string? Abbr { get; set; }
        public int NumOfYears { get; set; }

        public int Rank { get; set; }
        public string? Description { get; set; }

        public int EducationLevelTypeId { get; set; }
        public EducationLevelType? EducationLevelType { get; set; }
        public int CurriculumId { get; set; }
        public Curriculum? Curriculum { get; set; }

        public List<LearningLevel> LearningLevels { get; set; } = new();
        public List<EducationLevelSubject> educationLevelSubjects { get; set; } = new();
        public List<BroadOutcome> BroadOutcomes { get; set; } = new();
    }
}
