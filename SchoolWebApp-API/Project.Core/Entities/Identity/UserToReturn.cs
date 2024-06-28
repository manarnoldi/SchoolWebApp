namespace SchoolWebApp.Core.Entities.Identity
{
    public class UserToReturn
    {
        public string? Id { get; set; }
        public string? UserName { get; set; }
        public string? Email { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public int PersonId { get; set; }
    }
}