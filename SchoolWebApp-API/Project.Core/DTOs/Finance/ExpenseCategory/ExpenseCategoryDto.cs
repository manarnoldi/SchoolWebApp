namespace SchoolWebApp.Core.DTOs.Finance.ExpenseCategory
{
    public class ExpenseCategoryDto : CreateExpenseCategoryDto
    {
        public int Id { get; set; }
        public string? ExpenseAccountName { get; set; }
    }
}
