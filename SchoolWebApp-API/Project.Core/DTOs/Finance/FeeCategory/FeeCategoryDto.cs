namespace SchoolWebApp.Core.DTOs.Finance.FeeCategory
{
    public class FeeCategoryDto : CreateFeeCategoryDto
    {
        public int Id { get; set; }
        public string? IncomeAccountName { get; set; }
    }
}
