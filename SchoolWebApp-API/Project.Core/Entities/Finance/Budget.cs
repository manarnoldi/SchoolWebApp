using SchoolWebApp.Core.Entities.Academics;
using SchoolWebApp.Core.Entities.School;
using SchoolWebApp.Core.Entities.Shared;
using System.ComponentModel.DataAnnotations;

namespace SchoolWebApp.Core.Entities.Finance
{
    public class Budget : Base
    {
        [Required]
        [StringLength(255)]
        public required string Name { get; set; }

        [StringLength(500)]
        public string? Description { get; set; }

        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public int AcademicYearId { get; set; }
        public AcademicYear? AcademicYear { get; set; }

        public int DepartmentId { get; set; }
        public Department? Department { get; set; }

        public int? BudgetMasterId { get; set; }
        public BudgetMaster? BudgetMaster { get; set; }

        public bool IsActive { get; set; } = true;

        public List<BudgetLine> Lines { get; set; } = new();
        public List<BudgetAmendment> Amendments { get; set; } = new();
    }
}
