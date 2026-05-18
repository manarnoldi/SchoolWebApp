using SchoolWebApp.Core.Entities.Identity;
using SchoolWebApp.Core.Entities.Shared;
using System.ComponentModel.DataAnnotations;

namespace SchoolWebApp.Core.Entities.Approvals
{
    public class ApprovalWorkflowStep : Base
    {
        public int ApprovalWorkflowId { get; set; }
        public ApprovalWorkflow? ApprovalWorkflow { get; set; }

        public int Rank { get; set; }

        [Required]
        [StringLength(100)]
        public required string Name { get; set; }

        public int RoleId { get; set; }
        public AppRole? Role { get; set; }

        public bool IsFinal { get; set; }

        public bool NotifyNextApprover { get; set; } = true;

        public bool NotifyPreviousApprover { get; set; }

        public bool NotifyApplicant { get; set; } = true;
    }
}
