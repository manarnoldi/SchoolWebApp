using SchoolWebApp.Core.Entities.Shared;
using System.ComponentModel.DataAnnotations;

namespace SchoolWebApp.Core.Entities.Payroll
{
    public enum PayrollPeriodStatus { Draft = 0, Processed = 1, Approved = 2, Posted = 3 }

    public class PayrollPeriod : Base
    {
        public int Month { get; set; }
        public int Year { get; set; }

        [StringLength(100)]
        public string? Name { get; set; }

        public PayrollPeriodStatus Status { get; set; } = PayrollPeriodStatus.Draft;
        public DateTime? ProcessedDate { get; set; }
        public DateTime? ApprovedDate { get; set; }
        public DateTime? PostedDate { get; set; }

        public int? ApprovedById { get; set; }

        public List<Payslip> Payslips { get; set; } = new();
    }
}
