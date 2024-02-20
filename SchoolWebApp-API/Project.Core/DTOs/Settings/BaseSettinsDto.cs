using System.ComponentModel.DataAnnotations;

namespace SchoolWebApp.Core.DTOs.Settings
{
    public class BaseSettinsDto
    {
        [Required(ErrorMessage = "Enter the name")]
        [StringLength(255)]
        public string Name { get; set; }

        [StringLength(500)]
        public string Description { get; set; }
    }
}
