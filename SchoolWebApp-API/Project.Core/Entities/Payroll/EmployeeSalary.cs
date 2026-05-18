using SchoolWebApp.Core.Entities.Shared;
using SchoolWebApp.Core.Entities.Staff;
using System.ComponentModel.DataAnnotations;

namespace SchoolWebApp.Core.Entities.Payroll
{
    public class EmployeeSalary : Base
    {
        public int StaffDetailsId { get; set; }
        public StaffDetails? StaffDetails { get; set; }

        public decimal BasicSalary { get; set; }
        public decimal HouseAllowance { get; set; }
        public decimal TransportAllowance { get; set; }
        public decimal OtherAllowances { get; set; }

        public DateTime EffectiveDate { get; set; }
        public bool IsActive { get; set; } = true;

        [StringLength(500)]
        public string? Notes { get; set; }

        public List<EmployeeSalaryItem> Items { get; set; } = new();
    }

    public class EmployeeSalaryItem : Base
    {
        public int EmployeeSalaryId { get; set; }
        public EmployeeSalary? EmployeeSalary { get; set; }

        public int? EarningTypeId { get; set; }
        public EarningType? EarningType { get; set; }

        public int? DeductionTypeId { get; set; }
        public DeductionType? DeductionType { get; set; }

        public decimal Amount { get; set; }
    }
}
