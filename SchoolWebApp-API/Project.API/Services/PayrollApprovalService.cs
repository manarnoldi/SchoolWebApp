using Microsoft.EntityFrameworkCore;
using Project.Infrastructure.Data;
using SchoolWebApp.Core.Entities.Approvals;
using SchoolWebApp.Core.Entities.Finance;
using SchoolWebApp.Core.Entities.Payroll;
using SchoolWebApp.Core.Interfaces.IRepositories;

namespace SchoolWebApp.API.Services
{
    /// <summary>
    /// Approving a processed payroll period: the period is locked and its journal goes to the ledger.
    ///
    /// A payroll goes through the "Payroll" approval workflow (review, then approval) when one is active;
    /// its final approval calls Approve here. A school that switches the workflow off approves straight from
    /// the Payroll Processing page, which calls the same method.
    /// </summary>
    public class PayrollApprovalService
    {
        public const string FormKey = "Payroll";
        public const string EntityType = "PayrollPeriod";

        private readonly IUnitOfWork _unitOfWork;
        private readonly ApplicationDbContext _db;

        public PayrollApprovalService(IUnitOfWork unitOfWork, ApplicationDbContext db)
        {
            _unitOfWork = unitOfWork;
            _db = db;
        }

        /// <summary>True when payroll has to go through the approval workflow rather than a single click.</summary>
        public Task<bool> WorkflowActiveAsync() =>
            _db.ApprovalWorkflows.AnyAsync(w => w.FormKey == FormKey && w.IsActive && w.Steps.Any());

        /// <summary>The period's approval request, if it has been submitted.</summary>
        public Task<ApprovalRequest?> RequestAsync(int periodId) =>
            _db.ApprovalRequests.AsNoTracking().FirstOrDefaultAsync(r => r.EntityType == EntityType && r.EntityId == periodId);

        /// <summary>
        /// Why the period cannot be changed (re-processed or deleted) just now, or null when it can: while it is
        /// with the approvers it stays as they are reviewing it.
        /// </summary>
        public async Task<string?> LockedReasonAsync(int periodId)
        {
            var request = await RequestAsync(periodId);
            return request?.Status == ApprovalRequestStatus.Submitted
                ? "This payroll is with the approvers. Ask them to return it first if it needs changing."
                : null;
        }

        /// <summary>Why a period cannot be submitted for approval, or null when it can.</summary>
        public async Task<string?> SubmitProblemAsync(int periodId)
        {
            var period = await _db.PayrollPeriods.AsNoTracking().Include(p => p.Payslips).FirstOrDefaultAsync(p => p.Id == periodId);
            if (period == null) return "Payroll period not found.";
            if (period.Status != PayrollPeriodStatus.Processed) return "Only a processed payroll can be submitted for approval.";
            if (period.Payslips.Count == 0) return "This payroll has no payslips. Process it first.";
            return null;
        }

        /// <summary>
        /// Approves a processed period: builds the GL journal first (an incomplete or unbalanced journal stops the
        /// approval), then marks the period approved. The caller's own tracked changes - the approval request when
        /// this is the workflow's final step - are saved in the same SaveChanges, so both happen or neither does.
        /// </summary>
        public async Task<(bool Ok, string Message)> ApproveAsync(int periodId, int? approvedById)
        {
            var period = await _unitOfWork.PayrollPeriods.GetByIdWithPayslips(periodId);
            if (period == null) return (false, "Payroll period not found.");

            if (period.Status != PayrollPeriodStatus.Processed)
                return (false, "Only processed payroll periods can be approved.");

            var approvedAt = DateTime.UtcNow;
            var mode = await GetPayrollJournalMode();

            // Build the GL journal before anything is committed. A payroll must not
            // end up approved on a journal that is incomplete or does not balance,
            // so any problem stops the approval rather than surfacing afterwards.
            JournalEntry? journal = null;
            if (mode != PayrollJournalMode.Off)
            {
                var (built, problem) = await BuildPayrollJournal(period);
                if (problem != null) return (false, problem);
                journal = built;
            }

            if (journal != null && mode == PayrollJournalMode.Draft)
            {
                // Handed to finance rather than posted: it sits in Journal Entries as
                // a draft with every figure filled in, and reaches the ledger only
                // once it has been reviewed, submitted and approved there.
                journal.IsPosted = false;
                journal.Status = JournalEntryStatus.Draft;
                journal.Description = $"Payroll {period.Name} - generated for review";
            }

            period.Status = PayrollPeriodStatus.Approved;
            period.ApprovedDate = approvedAt;
            period.ApprovedById = approvedById;
            if (journal != null) _unitOfWork.JournalEntries.Create(journal);
            await _unitOfWork.SaveChangesAsync();

            string message = mode switch
            {
                PayrollJournalMode.Off =>
                    "Payroll period approved. No journal was created - payroll journal posting is switched off, so post it manually under Finance > Journal Entries.",
                _ when journal == null =>
                    "Payroll period approved. No journal was created because the salary expense and cash accounts are not set up under Finance settings.",
                PayrollJournalMode.Draft =>
                    $"Payroll period approved. Journal {journal!.ReferenceNumber} is waiting as a draft under Finance > Journal Entries for review and approval.",
                _ => "Payroll period approved and posted to GL."
            };
            return (true, message);
        }

        /// <summary>
        /// Undoes an approval (a super administrator reversing it): the period goes back to Processed and its
        /// journal is removed. A journal in a closed accounting period cannot be removed, so the period's
        /// accounts have to be reopened first. Returns a problem, or null when done.
        /// </summary>
        public async Task<string?> UnapproveAsync(int periodId)
        {
            var period = await _db.PayrollPeriods.FirstOrDefaultAsync(p => p.Id == periodId);
            if (period == null) return "Payroll period not found.";
            if (period.Status != PayrollPeriodStatus.Approved && period.Status != PayrollPeriodStatus.Posted)
                return "This payroll is not approved.";

            var reference = JournalReference(period);
            var journal = await _db.JournalEntries.Include(j => j.Lines).FirstOrDefaultAsync(j => j.ReferenceNumber == reference);
            if (journal != null)
            {
                // A draft journal may itself be in finance's approval queue.
                var requests = await _db.ApprovalRequests.Include(r => r.Actions)
                    .Where(r => r.EntityType == "JournalEntry" && r.EntityId == journal.Id).ToListAsync();
                _db.ApprovalStepActions.RemoveRange(requests.SelectMany(r => r.Actions));
                _db.ApprovalRequests.RemoveRange(requests);
                _db.JournalLines.RemoveRange(journal.Lines);
                _db.JournalEntries.Remove(journal);
            }
            period.Status = PayrollPeriodStatus.Processed;
            period.ApprovedDate = null;
            period.ApprovedById = null;
            period.PostedDate = null;
            await _db.SaveChangesAsync();
            return null;
        }

        // PRL (payroll), not PAY: fee payment journals are PAY-JNL-<receipt>, and
        // sharing the prefix made payroll journals impossible to pick out by
        // reference alone.
        public static string JournalReference(PayrollPeriod period) =>
            $"PRL-JNL-{period.Name?.Replace(" ", "-") ?? $"{period.Month}-{period.Year}"}";

        private async Task<int?> GetSettingAccountId(string key)
        {
            var settings = await _unitOfWork.Repository<SchoolWebApp.Core.Entities.Settings.GlobalSetting>()
                .Find(s => s.Module == "Finance" && s.SettingKey == key);
            var val = settings.FirstOrDefault()?.SettingValue;
            return int.TryParse(val, out var id) ? id : null;
        }

        private enum PayrollJournalMode { Auto, Draft, Off }

        /// <summary>
        /// What approving a payroll does in the ledger - the PayrollJournalMode finance
        /// setting. "auto" posts at once, "draft" leaves a filled-in draft for finance
        /// to review and approve, "off" creates nothing. A missing or unrecognised
        /// value is treated as draft: nothing reaches the ledger without review.
        /// </summary>
        private async Task<PayrollJournalMode> GetPayrollJournalMode()
        {
            var settings = await _unitOfWork.Repository<SchoolWebApp.Core.Entities.Settings.GlobalSetting>()
                .Find(s => s.Module == "Finance" && s.SettingKey == "PayrollJournalMode");
            return (settings.FirstOrDefault()?.SettingValue ?? "").Trim().ToLowerInvariant() switch
            {
                "auto" => PayrollJournalMode.Auto,
                "off" => PayrollJournalMode.Off,
                _ => PayrollJournalMode.Draft
            };
        }

        /// <summary>
        /// Builds, but does not save, the journal for an approved payroll period.
        ///
        /// Debit  salary expense  = gross pay + employer NSSF and Housing Levy + other
        ///                          employer-paid levies (NITA) + net pay rounding
        /// Credit cash            = net pay
        /// Credit PAYE / NSSF (employee + employer) / SHIF / Housing Levy payables
        /// Credit each other deduction, and each employer-paid levy, to its own liability
        ///        account (pension scheme, SACCO, welfare, NITA) - or the general payroll
        ///        deductions account
        /// Credit staff loans receivable = loan and advance instalments recovered
        ///
        /// It is dated the last day of the payroll month: the salaries are that
        /// month's cost, whenever the approval happens to be given.
        ///
        /// Returns (null, null) when the GL is not set up at all, so payroll can still
        /// be approved by a school that does not use the ledger. Returns a problem
        /// message when it is set up but the journal cannot be completed or balanced.
        /// </summary>
        private async Task<(JournalEntry? Journal, string? Problem)> BuildPayrollJournal(PayrollPeriod period)
        {
            if (period.Payslips.Count == 0) return (null, null);

            var salaryExpId = await GetSettingAccountId("SalaryExpenseAccountId");
            var cashId = await GetSettingAccountId("CashAccountId");

            // Salaries are charged per staff category (teaching and non-teaching have
            // their own expense accounts), with the SalaryExpenseAccountId setting as
            // the fallback for a category that has no account of its own.
            var categories = (await _unitOfWork.StaffCategories.Find())
                .ToDictionary(c => c.Id, c => (c.Name, c.SalaryExpenseAccountId));

            // Not set up at all: no cash account, or no salary account anywhere.
            if (cashId == null) return (null, null);
            if (salaryExpId == null && !categories.Values.Any(c => c.SalaryExpenseAccountId != null))
                return (null, null);

            var payeId = await GetSettingAccountId("PayeAccountId");
            var nssfId = await GetSettingAccountId("NssfAccountId");
            var shifId = await GetSettingAccountId("ShifAccountId");
            var ahlId = await GetSettingAccountId("AhlAccountId");
            var deductionsFallbackId = await GetSettingAccountId("PayrollDeductionsAccountId");
            var staffLoansId = await GetSettingAccountId("StaffLoansAccountId");

            var slips = period.Payslips;
            var lines = new List<JournalLine>();
            var missing = new List<string>();

            void Line(int? accountId, decimal debit, decimal credit, string description, string missingSetting)
            {
                if (debit == 0 && credit == 0) return;
                if (accountId == null) { missing.Add(missingSetting); return; }
                lines.Add(new JournalLine
                {
                    AccountId = accountId.Value,
                    Debit = debit,
                    Credit = credit,
                    Description = description
                });
            }

            var totalNssfEr = slips.Sum(p => p.NssfEmployer);

            // Salary expense, one debit per staff category. Each carries that
            // category's gross pay, the employer's NSSF and Housing Levy, and net pay
            // rounding - the employer contributions are costs of employment, and the
            // rounding is a real cost (or saving) since cash paid is rounded.
            foreach (var group in slips.GroupBy(p => p.StaffDetails?.StaffCategoryId))
            {
                var hasCategory = group.Key != null && categories.ContainsKey(group.Key.Value);
                var category = hasCategory ? categories[group.Key!.Value] : default;
                var categoryName = hasCategory ? category.Name : "Uncategorised";
                var accountId = category.SalaryExpenseAccountId ?? salaryExpId;

                Line(accountId,
                    group.Sum(p => p.GrossPay + p.NssfEmployer + p.AhlEmployer + p.EmployerCosts + p.RoundingAdjustment), 0,
                    $"Salary expense - {categoryName} - {period.Name}",
                    $"a salary expense account for the '{categoryName}' staff category (or SalaryExpenseAccountId)");
            }
            Line(cashId, 0, slips.Sum(p => p.NetPay),
                $"Net salaries paid - {period.Name}", "CashAccountId");

            Line(payeId, 0, slips.Sum(p => p.Paye),
                $"PAYE payable - {period.Name}", "PayeAccountId");
            Line(nssfId, 0, slips.Sum(p => p.NssfEmployee) + totalNssfEr,
                $"NSSF payable - {period.Name}", "NssfAccountId");
            Line(shifId, 0, slips.Sum(p => p.Shif),
                $"SHIF payable - {period.Name}", "ShifAccountId");
            // Employee and employer levy together - both are owed to the Housing Fund.
            Line(ahlId, 0, slips.Sum(p => p.Ahl + p.AhlEmployer),
                $"Housing Levy payable (employee and employer) - {period.Name}", "AhlAccountId");

            // Other deductions, grouped by type so each is owed to the right party.
            // Statutory rows are skipped - they are credited from the totals above.
            var allDeductionRows = slips.SelectMany(p => p.Deductions).ToList();
            foreach (var group in allDeductionRows
                         .Where(d => d.LoanAdvanceId == null
                                     && (d.DeductionType == null || !d.DeductionType.IsSystemComputed))
                         .GroupBy(d => d.DeductionTypeId))
            {
                var type = group.First().DeductionType;
                Line(type?.LiabilityAccountId ?? deductionsFallbackId, 0, group.Sum(d => d.Amount),
                    $"{type?.Name ?? "Payroll deduction"} payable - {period.Name}",
                    type?.LiabilityAccountId == null
                        ? $"a liability account for '{type?.Name ?? "deduction"}' (or PayrollDeductionsAccountId)"
                        : "PayrollDeductionsAccountId");
            }

            // Loan and advance instalments reduce what staff owe the school.
            Line(staffLoansId, 0, allDeductionRows.Where(d => d.LoanAdvanceId != null).Sum(d => d.Amount),
                $"Staff loan and advance recoveries - {period.Name}", "StaffLoansAccountId");

            if (missing.Count > 0)
                return (null, "Payroll could not be posted to the GL, so it has not been approved. Set up these accounts under Finance settings first: "
                    + string.Join(", ", missing.Distinct()) + ".");

            var debits = lines.Sum(l => l.Debit);
            var credits = lines.Sum(l => l.Credit);
            if (Math.Round(debits - credits, 2) != 0)
                return (null, $"The payroll journal for {period.Name} does not balance (debits {debits:N2}, credits {credits:N2}), so it has not been approved. Re-process the period and try again.");

            var monthEnd = new DateTime(period.Year, period.Month, 1).AddMonths(1).AddDays(-1);
            return (new JournalEntry
            {
                ReferenceNumber = JournalReference(period),
                EntryDate = monthEnd,
                Description = $"Auto-posted: Payroll {period.Name}",
                IsPosted = true,
                Status = JournalEntryStatus.Approved,
                Lines = lines
            }, null);
        }
    }
}
