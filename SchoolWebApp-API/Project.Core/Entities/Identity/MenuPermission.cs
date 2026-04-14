using SchoolWebApp.Core.Entities.Shared;
using System.ComponentModel.DataAnnotations;

namespace SchoolWebApp.Core.Entities.Identity
{
    public class MenuPermission : Base
    {
        [Required]
        [StringLength(100)]
        public required string RoleId { get; set; }

        [Required]
        [StringLength(255)]
        public required string MenuPath { get; set; }

        [StringLength(255)]
        public string? MenuName { get; set; }
    }
}
