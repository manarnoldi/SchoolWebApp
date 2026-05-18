namespace SchoolWebApp.Core.Entities.Identity
{
    public class AuthenticationResponse
    {
        public int Id { get; set; }
        public string? Token { get; set; }
        public DateTime Expiration { get; set; }
        public IList<string>? Roles { get; set; }
        public bool MustChangePassword { get; set; }
    }
}
