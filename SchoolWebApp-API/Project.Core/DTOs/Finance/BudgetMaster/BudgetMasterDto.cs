using SchoolWebApp.Core.Entities.Finance;
using System.ComponentModel.DataAnnotations;

namespace SchoolWebApp.Core.DTOs.Finance.BudgetMaster
{
    public class CreateBudgetMasterDto
    {
        [Required]
        [StringLength(255)]
        public required string Name { get; set; }

        [StringLength(50)]
        public string? Code { get; set; }

        [StringLength(500)]
        public string? Description { get; set; }

        public int AcademicYearId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public BudgetMasterStatus Status { get; set; } = BudgetMasterStatus.Draft;
    }

    public class BudgetMasterDto
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public string? Code { get; set; }
        public string? Description { get; set; }
        public int AcademicYearId { get; set; }
        public string? AcademicYearName { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public BudgetMasterStatus Status { get; set; }
        public int BudgetCount { get; set; }
    }
}
