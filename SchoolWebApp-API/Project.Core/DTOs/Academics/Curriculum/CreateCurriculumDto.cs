using System.ComponentModel.DataAnnotations;

namespace SchoolWebApp.Core.DTOs.Academics.Curriculum
{
    public class CreateCurriculumDto
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
    }
}
