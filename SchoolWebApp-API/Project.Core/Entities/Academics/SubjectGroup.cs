using SchoolWebApp.Core.Entities.Shared;
using System.ComponentModel.DataAnnotations;

namespace SchoolWebApp.Core.Entities.Academics
{
    public class SubjectGroup : Base
    {
        [Required(ErrorMessage = "Enter the subject group")]
        [Display(Name = "Subject group name")]
        [StringLength(255)]
        public string Name { get; set; }
        public string Abbreviation { get; set; }
        public string Description { get; set; }

        public int CurriculumId { get; set; }
        public Curriculum Curriculum { get; set; }

        public List<Subject> Subjects { get; set; }
    }
}
