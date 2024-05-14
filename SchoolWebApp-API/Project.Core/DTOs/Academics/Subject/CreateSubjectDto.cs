using System.ComponentModel.DataAnnotations;

namespace SchoolWebApp.Core.DTOs.Academics.Subject
{
    public class CreateSubjectDto
    {
        [Required(ErrorMessage = "Enter the subject code")]
        [Display(Name = "Subject code")]
        [StringLength(255)]
        public string Code { get; set; }

        [Required(ErrorMessage = "Enter the subject name")]
        [Display(Name = "Subject name")]
        [StringLength(255)]
        public string Name { get; set; }

        [Required(ErrorMessage = "Enter the subject abbreviation")]
        [Display(Name = "Subject abbreviation")]
        [StringLength(255)]
        public string Abbr { get; set; }

        public int SubjectGroupId { get; set; }
        public int CurriculumId { get; set; }
        public int StaffDetailsId { get; set; }
    }
}
