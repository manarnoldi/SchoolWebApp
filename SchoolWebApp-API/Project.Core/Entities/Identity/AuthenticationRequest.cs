namespace SchoolWebApp.Core.Entities.Identity
{
    public class AuthenticationRequest
    {
        public required string UserName { get; set; }
        public required string Password { get; set; }
    }
}