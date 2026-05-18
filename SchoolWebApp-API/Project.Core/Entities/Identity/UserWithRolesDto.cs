namespace SchoolWebApp.Core.Entities.Identity
{
    public class UserWithRolesDto
    {
        public int Id { get; set; }
        public string? UserName { get; set; }
        public string? Email { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public int? PersonId { get; set; }
        public string? PersonName { get; set; }
        public string? PersonType { get; set; }
        public string? PersonUPI { get; set; }
        public List<string> Roles { get; set; } = new();
    }
}
