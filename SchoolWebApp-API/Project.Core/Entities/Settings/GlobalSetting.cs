using SchoolWebApp.Core.Entities.Shared;
using System.ComponentModel.DataAnnotations;

namespace SchoolWebApp.Core.Entities.Settings
{
    public class GlobalSetting : Base
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
