using SchoolWebApp.Core.Entities.Approvals;
using System.ComponentModel.DataAnnotations;

namespace SchoolWebApp.Core.DTOs.Approvals
{
    public class ApprovalWorkflowDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public string FormKey { get; set; } = "";
        public string? Description { get; set; }
        public bool IsMakerChecker { get; set; } = true;
        public bool IsActive { get; set; } = true;
        public List<ApprovalWorkflowStepDto> Steps { get; set; } = new();
    }

    public class ApprovalWorkflowStepDto
    {
        public int Id { get; set; }
        public int ApprovalWorkflowId { get; set; }
        public int Rank { get; set; }
        public string Name { get; set; } = "";
        public int RoleId { get; set; }
        public string? RoleName { get; set; }
        public bool IsFinal { get; set; }
        public bool NotifyNextApprover { get; set; } = true;
        public bool NotifyPreviousApprover { get; set; }
        public bool NotifyApplicant { get; set; } = true;
    }

    public class CreateApprovalWorkflowDto
    {
        [Required, StringLength(100)]
        public string Name { get; set; } = "";
        [Required, StringLength(50)]
        public string FormKey { get; set; } = "";
        [StringLength(500)]
        public string? Description { get; set; }
        public bool IsMakerChecker { get; set; } = true;
        public bool IsActive { get; set; } = true;
        public List<CreateApprovalWorkflowStepDto> Steps { get; set; } = new();
    }

    public class CreateApprovalWorkflowStepDto
    {
        public int Rank { get; set; }
        [Required, StringLength(100)]
        public string Name { get; set; } = "";
        public int RoleId { get; set; }
        public bool IsFinal { get; set; }
        public bool NotifyNextApprover { get; set; } = true;
        public bool NotifyPreviousApprover { get; set; }
        public bool NotifyApplicant { get; set; } = true;
    }

    public class ApprovalRequestDto
    {
        public int Id { get; set; }
        public int ApprovalWorkflowId { get; set; }
        public ApprovalWorkflowDto? ApprovalWorkflow { get; set; }
        public string EntityType { get; set; } = "";
        public int EntityId { get; set; }
        public ApprovalRequestStatus Status { get; set; }
        public int? SubmittedById { get; set; }
        public string? SubmittedByName { get; set; }
        public DateTime? SubmittedAt { get; set; }
        public int CurrentStepRank { get; set; }
        public List<ApprovalStepActionDto> Actions { get; set; } = new();
    }

    public class ApprovalStepActionDto
    {
        public int Id { get; set; }
        public int ApprovalRequestId { get; set; }
        public int StepRank { get; set; }
        public string StepName { get; set; } = "";
        public int AssignedToUserId { get; set; }
        public string? AssignedToName { get; set; }
        public int? ActionedByUserId { get; set; }
        public string? ActionedByName { get; set; }
        public StepActionStatus Status { get; set; }
        public string? Comment { get; set; }
        public DateTime? ActionedAt { get; set; }
    }

    public class SubmitApprovalDto
    {
        [Required]
        public string EntityType { get; set; } = "";
        [Required]
        public int EntityId { get; set; }
        [Required]
        public string FormKey { get; set; } = "";
        // One entry per step: { stepRank, assignedToUserId }
        public List<SubmitApprovalStepDto> StepAssignments { get; set; } = new();
    }

    public class SubmitApprovalStepDto
    {
        public int StepRank { get; set; }
        public int AssignedToUserId { get; set; }
    }

    public class ApprovalActionRequestDto
    {
        [Required]
        public string Action { get; set; } = ""; // "approve" | "reject"
        public string? Comment { get; set; }
    }

    public class ReverseApprovalDto
    {
        [Required, StringLength(1000)]
        public string Comment { get; set; } = "";
    }

    public class UserInRoleDto
    {
        public int Id { get; set; }
        public string? UserName { get; set; }
        public string? Email { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string FullName => $"{FirstName} {LastName}".Trim();
    }
}
