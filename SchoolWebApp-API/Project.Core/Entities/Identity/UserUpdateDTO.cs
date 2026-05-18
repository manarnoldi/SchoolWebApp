namespace SchoolWebApp.Core.Entities.Identity
{
    public class UserUpdateDTO
    {
        public string? UserName { get; set; }
        public string? Email { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Password { get; set; }
        public int? PersonId { get; set; }
    }
}
