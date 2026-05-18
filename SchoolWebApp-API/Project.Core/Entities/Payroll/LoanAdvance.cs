using SchoolWebApp.Core.Entities.Shared;
using SchoolWebApp.Core.Entities.Staff;
using System.ComponentModel.DataAnnotations;

namespace SchoolWebApp.Core.Entities.Payroll
{
    public enum LoanStatus { Active = 1, FullyPaid = 2, Cancelled = 3 }

    public class LoanAdvance : Base
    {
        public int StaffDetailsId { get; set; }
        public StaffDetails? StaffDetails { get; set; }

        [Required, StringLength(255)]
        public string Description { get; set; } = string.Empty;

        public decimal PrincipalAmount { get; set; }
        public decimal MonthlyDeduction { get; set; }
        public decimal Balance { get; set; }
        public DateTime IssueDate { get; set; }
        public LoanStatus Status { get; set; } = LoanStatus.Active;

        [StringLength(500)]
        public string? Notes { get; set; }
    }
}
