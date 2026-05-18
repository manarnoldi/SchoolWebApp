using SchoolWebApp.Core.Entities.Shared;
using System.ComponentModel.DataAnnotations;

namespace SchoolWebApp.Core.Entities.Finance
{
    public enum ExpenseStatus { Draft = 0, Submitted = 1, Approved = 2, Rejected = 3 }

    public class Expense : Base
    {
        [Required]
        [StringLength(50)]
        public required string ReferenceNumber { get; set; }

        public DateTime ExpenseDate { get; set; }

        public PaymentMethod PaymentMethod { get; set; }

        [StringLength(100)]
        public string? TransactionReference { get; set; }

        public int? PaidFromAccountId { get; set; }
        public Account? PaidFromAccount { get; set; }

        public ExpenseStatus Status { get; set; } = ExpenseStatus.Draft;

        [StringLength(500)]
        public string? Description { get; set; }

        public List<ExpenseLine> Lines { get; set; } = new();
    }

    public class ExpenseLine : Base
    {
        public int ExpenseId { get; set; }
        public Expense? Expense { get; set; }

        public int ExpenseCategoryId { get; set; }
        public ExpenseCategory? ExpenseCategory { get; set; }

        public decimal Amount { get; set; }

        [StringLength(255)]
        public string? Vendor { get; set; }

        public int? BudgetLineId { get; set; }
        public BudgetLine? BudgetLine { get; set; }

        [StringLength(500)]
        public string? Description { get; set; }
    }
}
