using System.ComponentModel.DataAnnotations;

namespace SchoolWebApp.Core.Entities.Identity
{
    public class UserDTO
    {
        [Required]
        public required string UserName { get; set; }
        [Required]
        public required string Email { get; set; }
        [Required]
        public required string Password { get; set; }

        public string? FirstName { get; set; }
        public string? LastName { get; set; }
    }
}