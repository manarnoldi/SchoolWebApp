using SchoolWebApp.Core.Entities.Academics;
using SchoolWebApp.Core.Entities.Shared;
using System.ComponentModel.DataAnnotations;

namespace SchoolWebApp.Core.Entities.Finance
{
    public enum BudgetMasterStatus
    {
        Draft = 0,
        Open = 1,
        Closed = 2
    }

    public class BudgetMaster : Base
    {
        [Required]
        [StringLength(255)]
        public required string Name { get; set; }

        [StringLength(50)]
        public string? Code { get; set; }

        [StringLength(500)]
        public string? Description { get; set; }

        public int AcademicYearId { get; set; }
        public AcademicYear? AcademicYear { get; set; }

        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public BudgetMasterStatus Status { get; set; } = BudgetMasterStatus.Draft;

        public List<Budget> Budgets { get; set; } = new();
    }
}
