using SchoolWebApp.Core.Entities.Shared;
using System.ComponentModel.DataAnnotations;

namespace SchoolWebApp.Core.Entities.CBE.Responsibilities
{
    public class Responsibility : SettingsBase
    {
        [StringLength(50)]
        public string? Category { get; set; }
    }
}
