using SchoolWebApp.Core.Entities.Identity;
using SchoolWebApp.Core.Entities.Shared;
using System.ComponentModel.DataAnnotations;

namespace SchoolWebApp.Core.Entities.Finance
{
    public enum BudgetAmendmentStatus
    {
        Pending = 0,
        Approved = 1,
        Rejected = 2
    }

    public class BudgetAmendment : Base
    {
        public int BudgetId { get; set; }
        public Budget? Budget { get; set; }

        [StringLength(50)]
        public string? ReferenceNumber { get; set; }

        public DateTime AmendmentDate { get; set; }

        [StringLength(1000)]
        public string? Reason { get; set; }

        public BudgetAmendmentStatus Status { get; set; } = BudgetAmendmentStatus.Pending;

        public int? ApprovedById { get; set; }
        public AppUser? ApprovedBy { get; set; }
        public DateTime? ApprovedDate { get; set; }

        public List<BudgetAmendmentLine> Lines { get; set; } = new();
    }

    public class BudgetAmendmentLine : Base
    {
        public int BudgetAmendmentId { get; set; }
        public BudgetAmendment? BudgetAmendment { get; set; }

        public int AccountId { get; set; }
        public Account? Account { get; set; }

        public decimal PreviousAmount { get; set; }
        public decimal NewAmount { get; set; }
        public decimal Delta { get; set; }

        [StringLength(500)]
        public string? Notes { get; set; }
    }
}
