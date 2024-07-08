using SchoolWebApp.Core.DTOs.Academics.Curriculum;
using System.ComponentModel.DataAnnotations;

namespace SchoolWebApp.Core.DTOs.Academics.SubjectGroup
{
    public class CreateSubjectGroupDto
    {
        [Required(ErrorMessage = "Enter the subject group")]
        [Display(Name = "Subject group name")]
        [StringLength(255)]
        public required string Name { get; set; }
        public string? Abbreviation { get; set; }
        public string? Description { get; set; }
        public int CurriculumId { get; set; }
        public CurriculumDto? Curriculum { get; set; }
    }
}
