namespace SchoolWebApp.Core.DTOs.Finance.Account
{
    public class AccountDto : CreateAccountDto
    {
        public int Id { get; set; }
        public string? ParentAccountName { get; set; }
    }
}
