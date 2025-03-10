using System.ComponentModel.DataAnnotations;

namespace SchoolWebApp.Core.DTOs.School.LearningMode
{
    public class CreateLearningModeDto
    {
        [Required(ErrorMessage = "Enter the learning mode.")]
        [Display(Name = "Learning mode")]
        [StringLength(255)]
        public required string Name { get; set; }

        [StringLength(255)]
        public string? Description { get; set; }
    }
}
