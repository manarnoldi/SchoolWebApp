using SchoolWebApp.Core.Entities.Identity;
using SchoolWebApp.Core.Entities.Shared;
using System.ComponentModel.DataAnnotations;

namespace SchoolWebApp.Core.Entities.Approvals
{
    public enum ApprovalRequestStatus
    {
        Draft = 0,
        Submitted = 1,
        Approved = 2,
        Rejected = 3,
        Returned = 4,
        Reversed = 5
    }

    public class ApprovalRequest : Base
    {
        public int ApprovalWorkflowId { get; set; }
        public ApprovalWorkflow? ApprovalWorkflow { get; set; }

        [Required]
        [StringLength(50)]
        public required string EntityType { get; set; }

        public int EntityId { get; set; }

        public ApprovalRequestStatus Status { get; set; } = ApprovalRequestStatus.Draft;

        public int? SubmittedById { get; set; }
        public AppUser? SubmittedBy { get; set; }

        public DateTime? SubmittedAt { get; set; }

        public int CurrentStepRank { get; set; } = 1;

        public List<ApprovalStepAction> Actions { get; set; } = new();
    }
}
