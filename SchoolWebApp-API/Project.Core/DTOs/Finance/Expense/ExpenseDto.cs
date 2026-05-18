namespace SchoolWebApp.Core.DTOs.Finance.Expense
{
    public class ExpenseLineDto : CreateExpenseLineDto
    {
        public string? ExpenseCategoryName { get; set; }
        public string? BudgetName { get; set; }
        public string? BudgetLineAccountName { get; set; }
    }

    public class ExpenseDto
    {
        public int Id { get; set; }
        public string? ReferenceNumber { get; set; }
        public DateTime ExpenseDate { get; set; }
        public int PaymentMethod { get; set; }
        public string? TransactionReference { get; set; }
        public int? PaidFromAccountId { get; set; }
        public string? PaidFromAccountName { get; set; }
        public int Status { get; set; }
        public string? StatusLabel { get; set; }
        public string? Description { get; set; }
        public decimal TotalAmount { get; set; }
        public int LineCount { get; set; }
        public List<ExpenseLineDto> Lines { get; set; } = new();
    }
}
