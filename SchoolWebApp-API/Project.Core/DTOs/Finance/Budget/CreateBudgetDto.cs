using System.ComponentModel.DataAnnotations;

namespace SchoolWebApp.Core.DTOs.Finance.Budget
{
    public class CreateBudgetDto
    {
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

        public List<CreateBudgetLineDto> Lines { get; set; } = new();
    }

    public class CreateBudgetLineDto
    {
        public int AccountId { get; set; }
        public decimal BudgetedAmount { get; set; }
        public string? Notes { get; set; }
    }
}
