using System.ComponentModel.DataAnnotations;

namespace SchoolWebApp.Core.DTOs.Settings.GlobalSetting
{
    public class CreateGlobalSettingDto
    {
        [Required]
        [StringLength(100)]
        public required string Module { get; set; }

        [Required]
        [StringLength(100)]
        public required string SettingKey { get; set; }

        [Required]
        [StringLength(500)]
        public required string SettingValue { get; set; }

        [StringLength(255)]
        public string? Description { get; set; }
    }
}
