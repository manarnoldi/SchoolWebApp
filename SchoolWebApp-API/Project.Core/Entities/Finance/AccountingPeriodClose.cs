using SchoolWebApp.Core.Entities.Shared;
using System.ComponentModel.DataAnnotations;

namespace SchoolWebApp.Core.Entities.Finance
{
    public enum AccountingPeriodCloseStatus { Closed = 0, Reopened = 1 }

    /// <summary>
    /// The closing of an accounting period (a term or an academic year, per the Finance setting). Closing posts the
    /// period's income less expenses to Retained Earnings and locks it: nothing dated in it can be recorded, changed
    /// or deleted. Reopening (an administrator, latest closed period only) takes the closing entry back out and keeps
    /// this row as history. Closed by / on are the row's CreatedBy / Created.
    /// </summary>
    public class AccountingPeriodClose : Base
    {
        // Which period: "T:<session id>" for a term, "Y:<academic year id>" for a year.
        [Required]
        [StringLength(30)]
        public required string PeriodKey { get; set; }

        [Required]
        [StringLength(100)]
        public required string Name { get; set; }

        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        // What was closed: the period's income and expenses, their difference going to Retained Earnings.
        public decimal Income { get; set; }
        public decimal Expenses { get; set; }

        public int? ClosingJournalId { get; set; }

        public AccountingPeriodCloseStatus Status { get; set; } = AccountingPeriodCloseStatus.Closed;
        public DateTime? ReopenedAt { get; set; }
        [StringLength(100)]
        public string? ReopenedBy { get; set; }
    }
}
