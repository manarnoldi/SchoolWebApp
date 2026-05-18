using System.ComponentModel.DataAnnotations;

namespace SchoolWebApp.Core.DTOs.Finance.Budget
{
    // Not inheriting from CreateBudgetDto so the Lines property type doesn't collide during JSON serialization
    public class BudgetDto
    {
        public int Id { get; set; }

        [Required]
        [StringLength(255)]
        public required string Name { get; set; }

        [StringLength(500)]
        public string? Description { get; set; }

        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int AcademicYearId { get; set; }
        public int DepartmentId { get; set; }
        public int? BudgetMasterId { get; set; }
        public bool IsActive { get; set; } = true;

        public string? AcademicYearName { get; set; }
        public string? DepartmentName { get; set; }
        public string? BudgetMasterName { get; set; }
        public decimal TotalBudgeted { get; set; }
        public decimal TotalAmendments { get; set; }
        public decimal TotalEffective { get; set; }
        public List<BudgetLineDto> Lines { get; set; } = new();
    }

    public class BudgetLineDto
    {
        public int Id { get; set; }
        public int BudgetId { get; set; }
        public int AccountId { get; set; }
        public decimal BudgetedAmount { get; set; }
        public string? Notes { get; set; }
        public string? AccountCode { get; set; }
        public string? AccountName { get; set; }
        public string? AccountType { get; set; }
        public decimal AmendedAmount { get; set; }
        public decimal EffectiveAmount { get; set; }
        public decimal ActualAmount { get; set; }
        public decimal Variance { get; set; }
    }
}
