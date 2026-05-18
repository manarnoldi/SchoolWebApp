using SchoolWebApp.Core.Entities.Finance;
using System.ComponentModel.DataAnnotations;

namespace SchoolWebApp.Core.DTOs.Finance.Expense
{
    public class CreateExpenseLineDto
    {
        public int? Id { get; set; }
        public int ExpenseCategoryId { get; set; }
        public decimal Amount { get; set; }
        [StringLength(255)]
        public string? Vendor { get; set; }
        public int? BudgetLineId { get; set; }
        [StringLength(500)]
        public string? Description { get; set; }
    }

    public class CreateExpenseDto
    {
        [StringLength(50)]
        public string? ReferenceNumber { get; set; }

        public DateTime ExpenseDate { get; set; }

        public PaymentMethod PaymentMethod { get; set; }

        [StringLength(100)]
        public string? TransactionReference { get; set; }

        public int? PaidFromAccountId { get; set; }

        public int Status { get; set; }

        [StringLength(500)]
        public string? Description { get; set; }

        public List<CreateExpenseLineDto> Lines { get; set; } = new();
    }
}
