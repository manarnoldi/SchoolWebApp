using SchoolWebApp.Core.Entities.Finance;
using System.ComponentModel.DataAnnotations;

namespace SchoolWebApp.Core.DTOs.Finance.BudgetAmendment
{
    public class CreateBudgetAmendmentLineDto
    {
        public int AccountId { get; set; }
        public decimal PreviousAmount { get; set; }
        public decimal NewAmount { get; set; }
        public string? Notes { get; set; }
    }

    public class CreateBudgetAmendmentDto
    {
        public int BudgetId { get; set; }

        [StringLength(50)]
        public string? ReferenceNumber { get; set; }

        public DateTime AmendmentDate { get; set; }

        [StringLength(1000)]
        public string? Reason { get; set; }

        public List<CreateBudgetAmendmentLineDto> Lines { get; set; } = new();
    }

    public class BudgetAmendmentLineDto
    {
        public int Id { get; set; }
        public int BudgetAmendmentId { get; set; }
        public int AccountId { get; set; }
        public string? AccountCode { get; set; }
        public string? AccountName { get; set; }
        public decimal PreviousAmount { get; set; }
        public decimal NewAmount { get; set; }
        public decimal Delta { get; set; }
        public string? Notes { get; set; }
    }

    public class BudgetAmendmentDto
    {
        public int Id { get; set; }
        public int BudgetId { get; set; }
        public string? BudgetName { get; set; }
        public string? ReferenceNumber { get; set; }
        public DateTime AmendmentDate { get; set; }
        public string? Reason { get; set; }
        public BudgetAmendmentStatus Status { get; set; }
        public int? ApprovedById { get; set; }
        public string? ApprovedByName { get; set; }
        public DateTime? ApprovedDate { get; set; }
        public List<BudgetAmendmentLineDto> Lines { get; set; } = new();
    }
}
