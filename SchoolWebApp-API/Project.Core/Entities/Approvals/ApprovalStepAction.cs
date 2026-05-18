using SchoolWebApp.Core.Entities.Identity;
using SchoolWebApp.Core.Entities.Shared;
using System.ComponentModel.DataAnnotations;

namespace SchoolWebApp.Core.Entities.Approvals
{
    public enum StepActionStatus
    {
        Pending = 0,
        Approved = 1,
        Rejected = 2,
        Skipped = 3,
        Returned = 4
    }

    public class ApprovalStepAction : Base
    {
        public int ApprovalRequestId { get; set; }
        public ApprovalRequest? ApprovalRequest { get; set; }

        public int StepRank { get; set; }

        [Required]
        [StringLength(100)]
        public required string StepName { get; set; }

        public int AssignedToUserId { get; set; }
        public AppUser? AssignedTo { get; set; }

        public int? ActionedByUserId { get; set; }
        public AppUser? ActionedBy { get; set; }

        public StepActionStatus Status { get; set; } = StepActionStatus.Pending;

        [StringLength(1000)]
        public string? Comment { get; set; }

        public DateTime? ActionedAt { get; set; }
    }
}
