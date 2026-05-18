using SchoolWebApp.Core.Entities.Shared;
using System.ComponentModel.DataAnnotations;

namespace SchoolWebApp.Core.Entities.Approvals
{
    public class ApprovalWorkflow : Base
    {
        [Required]
        [StringLength(100)]
        public required string Name { get; set; }

        [Required]
        [StringLength(50)]
        public required string FormKey { get; set; }

        [StringLength(500)]
        public string? Description { get; set; }

        public bool IsMakerChecker { get; set; } = true;

        public bool IsActive { get; set; } = true;

        public List<ApprovalWorkflowStep> Steps { get; set; } = new();
    }
}
