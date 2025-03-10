using SchoolWebApp.Core.Entities.Shared;
using SchoolWebApp.Core.Entities.Students;
using System.ComponentModel.DataAnnotations;

namespace SchoolWebApp.Core.Entities.School
{
    public class LearningMode : Base
    {

        [Required(ErrorMessage = "Enter the learning mode.")]
        [Display(Name = "Learning mode")]
        [StringLength(255)]
        public required string Name { get; set; }

        [StringLength(255)]
        public string? Description { get; set; }
        public List<Student> Students { get; set; } = new();
    }
}
