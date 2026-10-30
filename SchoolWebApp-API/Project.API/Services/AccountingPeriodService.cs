using Microsoft.EntityFrameworkCore;
using Project.Infrastructure.Data;
using SchoolWebApp.Core.Entities.Finance;

namespace SchoolWebApp.API.Services
{
    /// <summary>
    /// Accounting periods, taken from the academic calendar: terms or whole academic years, per the Finance setting
    /// "AccountingPeriod" (Termly, the default, or Yearly). A term's period runs from its first day to the day before
    /// the next term starts, so the holiday after a term belongs to it; the first term starts with the academic year
    /// and the last runs to its end. Closing a period posts its income less expenses to Retained Earnings and locks it
    /// (the lock itself is in ApplicationDbContext); an administrator can reopen the latest closed one.
    /// </summary>
    public class AccountingPeriodService
    {
        public const string ClosingPrefix = "CLOSE-";
        private readonly ApplicationDbContext _db;
        public AccountingPeriodService(ApplicationDbContext db) { _db = db; }

        public record Period(string Key, string Name, DateTime Start, DateTime End, int AcademicYearId, int? SessionId);

        public async Task<bool> Termly()
        {
            var s = await _db.GlobalSettings.AsNoTracking().FirstOrDefaultAsync(x => x.Module == "Finance" && x.SettingKey == "AccountingPeriod");
            return !string.Equals(s?.SettingValue?.Trim(), "Yearly", StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>Every accounting period, oldest first: terms or years as the school has chosen.</summary>
        public async Task<List<Period>> All() => await Periods(await Termly());

        /// <summary>The periods by term, whatever the setting - also what a term statement brings balances forward to.</summary>
        public async Task<List<Period>> Periods(bool termly)
        {
            var years = await _db.AcademicYears.AsNoTracking().OrderBy(y => y.StartDate).ToListAsync();
            var sessions = await _db.Sessions.AsNoTracking().ToListAsync();
            var result = new List<Period>();
            foreach (var y in years)
            {
                var terms = sessions.Where(s => s.AcademicYearId == y.Id).OrderBy(s => s.StartDate).ToList();
                if (!termly || terms.Count == 0)
                {
                    result.Add(new Period($"Y:{y.Id}", $"Academic year {y.Name}", y.StartDate.Date, y.EndDate.Date, y.Id, null));
                    continue;
                }
                for (var i = 0; i < terms.Count; i++)
                {
                    var start = i == 0 && y.StartDate < terms[i].StartDate ? y.StartDate : terms[i].StartDate;
                    var end = i < terms.Count - 1 ? terms[i + 1].StartDate.AddDays(-1) : (y.EndDate > terms[i].EndDate ? y.EndDate : terms[i].EndDate);
                    result.Add(new Period($"T:{terms[i].Id}", $"{terms[i].SessionName} {y.Name}", start.Date, end.Date, y.Id, terms[i].Id));
                }
            }
            return result;
        }

        /// <summary>The term (or year) window a date falls in, for bringing balances forward.</summary>
        public async Task<Period?> TermOf(int sessionId) =>
            (await Periods(true)).FirstOrDefault(p => p.SessionId == sessionId);

        public async Task<Period?> YearOf(int academicYearId) =>
            (await Periods(false)).FirstOrDefault(p => p.AcademicYearId == academicYearId);

        public Task<List<AccountingPeriodClose>> Closes() =>
            _db.AccountingPeriodCloses.AsNoTracking().OrderByDescending(c => c.Created).ToListAsync();

        public record Result(decimal Income, decimal Expenses, List<(int AccountId, string Code, string Name, AccountType Type, decimal Amount)> Lines);

        /// <summary>A period's income and expenses by account, leaving out closing entries.</summary>
        public async Task<Result> Results(DateTime start, DateTime end)
        {
            var lines = await _db.JournalLines.AsNoTracking()
                .Where(l => l.JournalEntry!.IsPosted && l.JournalEntry.EntryDate >= start && l.JournalEntry.EntryDate < end.AddDays(1)
                    && !l.JournalEntry.ReferenceNumber.StartsWith(ClosingPrefix)
                    && (l.Account!.AccountType == AccountType.Income || l.Account.AccountType == AccountType.Expense))
                .GroupBy(l => new { l.AccountId, l.Account!.Code, l.Account.Name, l.Account.AccountType })
                .Select(g => new { g.Key.AccountId, g.Key.Code, g.Key.Name, g.Key.AccountType, Debit = g.Sum(l => l.Debit), Credit = g.Sum(l => l.Credit) })
                .ToListAsync();
            var rows = lines.Select(l => (l.AccountId, l.Code, l.Name, l.AccountType,
                l.AccountType == AccountType.Income ? l.Credit - l.Debit : l.Debit - l.Credit)).Where(r => r.Item5 != 0).ToList();
            return new Result(rows.Where(r => r.AccountType == AccountType.Income).Sum(r => r.Item5),
                rows.Where(r => r.AccountType == AccountType.Expense).Sum(r => r.Item5), rows);
        }

        /// <summary>What stops a period being closed: work dated in it that is not finished.</summary>
        public async Task<List<string>> Blockers(Period p, List<Period> all, List<AccountingPeriodClose> closes)
        {
            var list = new List<string>();
            var to = p.End.AddDays(1);
            if (p.End >= DateTime.Today)
                list.Add($"{p.Name} runs to {p.End:dd MMM yyyy}; it can be closed from {to:dd MMM yyyy}.");
            var earlierOpen = all.Where(x => x.Start < p.Start && !IsClosed(x, closes)).ToList();
            foreach (var e in earlierOpen)
                if (await HasActivity(e)) list.Add($"{e.Name} is still open: close it first.");
            var drafts = await _db.Expenses.CountAsync(x => x.ExpenseDate >= p.Start && x.ExpenseDate < to
                && (x.Status == ExpenseStatus.Draft || x.Status == ExpenseStatus.Submitted));
            if (drafts > 0) list.Add($"{drafts} expense{(drafts == 1 ? " is" : "s are")} dated in it and not yet approved or rejected.");
            var journals = await _db.JournalEntries.CountAsync(j => j.EntryDate >= p.Start && j.EntryDate < to && !j.IsPosted);
            if (journals > 0) list.Add($"{journals} journal entr{(journals == 1 ? "y is" : "ies are")} dated in it and not yet posted.");
            var notes = await _db.Payments.CountAsync(x => x.PaymentDate >= p.Start && x.PaymentDate < to && x.PaymentType != PaymentType.Receipt
                && (x.ApprovalStatus == PaymentApprovalStatus.Draft || x.ApprovalStatus == PaymentApprovalStatus.Submitted));
            if (notes > 0) list.Add($"{notes} credit or debit note{(notes == 1 ? " is" : "s are")} dated in it and not yet approved or rejected.");
            if (await RetainedEarnings() == null)
                list.Add("Set the Retained Earnings account under Settings > Global Settings > Finance.");
            return list;
        }

        public static bool IsClosed(Period p, List<AccountingPeriodClose> closes) =>
            closes.Any(c => c.PeriodKey == p.Key && c.Status == AccountingPeriodCloseStatus.Closed);

        private async Task<bool> HasActivity(Period p) =>
            await _db.JournalEntries.AnyAsync(j => j.EntryDate >= p.Start && j.EntryDate < p.End.AddDays(1));

        private async Task<int?> RetainedEarnings()
        {
            var s = await _db.GlobalSettings.AsNoTracking().FirstOrDefaultAsync(x => x.Module == "Finance" && x.SettingKey == "RetainedEarningsAccountId");
            return int.TryParse(s?.SettingValue, out var id) ? id : null;
        }

        /// <summary>
        /// Closes a period: posts its income less expenses to Retained Earnings (Debit each income account, Credit each
        /// expense account, the difference to Retained Earnings), dated its last day, and records it closed.
        /// </summary>
        public async Task<AccountingPeriodClose> Close(string key)
        {
            var all = await All();
            var p = all.FirstOrDefault(x => x.Key == key) ?? throw new InvalidOperationException("No such accounting period.");
            var closes = await Closes();
            if (IsClosed(p, closes)) throw new InvalidOperationException($"{p.Name} is already closed.");
            var blockers = await Blockers(p, all, closes);
            if (blockers.Count > 0) throw new InvalidOperationException(string.Join(" ", blockers));

            var result = await Results(p.Start, p.End);
            var retained = (await RetainedEarnings())!.Value;
            await using var tx = await _db.Database.BeginTransactionAsync();
            JournalEntry? journal = null;
            if (result.Lines.Count > 0)
            {
                journal = new JournalEntry
                {
                    ReferenceNumber = $"{ClosingPrefix}{p.Start:yyyyMMdd}-{p.End:yyyyMMdd}",
                    EntryDate = p.End,
                    Description = $"Period close: {p.Name} - income less expenses to Retained Earnings",
                    IsPosted = true,
                    Status = JournalEntryStatus.Approved,
                    Lines = result.Lines.Select(l => new JournalLine
                    {
                        AccountId = l.AccountId,
                        // Income (a credit balance) is debited out; an expense (a debit balance) credited out.
                        Debit = l.Type == AccountType.Income ? Math.Max(0, l.Amount) : Math.Max(0, -l.Amount),
                        Credit = l.Type == AccountType.Expense ? Math.Max(0, l.Amount) : Math.Max(0, -l.Amount),
                        Description = $"Closed to Retained Earnings - {p.Name}"
                    }).ToList()
                };
                var net = result.Income - result.Expenses;
                journal.Lines.Add(new JournalLine
                {
                    AccountId = retained,
                    Debit = net < 0 ? -net : 0,
                    Credit = net > 0 ? net : 0,
                    Description = $"{(net >= 0 ? "Surplus" : "Deficit")} for {p.Name}"
                });
                _db.JournalEntries.Add(journal);
                await _db.SaveChangesAsync();
            }
            var close = new AccountingPeriodClose
            {
                PeriodKey = p.Key, Name = p.Name, StartDate = p.Start, EndDate = p.End,
                Income = result.Income, Expenses = result.Expenses, ClosingJournalId = journal?.Id
            };
            _db.AccountingPeriodCloses.Add(close);
            await _db.SaveChangesAsync();
            await tx.CommitAsync();
            return close;
        }

        /// <summary>Reopens the latest closed period: its closing entry is taken out and it is unlocked.</summary>
        public async Task Reopen(string key, string? user)
        {
            var close = await _db.AccountingPeriodCloses.Where(c => c.Status == AccountingPeriodCloseStatus.Closed)
                .OrderByDescending(c => c.StartDate).FirstOrDefaultAsync();
            if (close == null || close.PeriodKey != key)
                throw new InvalidOperationException(close == null ? "No period is closed." : $"Only the latest closed period ({close.Name}) can be reopened.");

            await using var tx = await _db.Database.BeginTransactionAsync();
            _db.AllowClosedPeriodChanges = true;
            try
            {
                if (close.ClosingJournalId != null)
                {
                    var j = await _db.JournalEntries.Include(x => x.Lines).FirstOrDefaultAsync(x => x.Id == close.ClosingJournalId);
                    if (j != null) { _db.JournalLines.RemoveRange(j.Lines); _db.JournalEntries.Remove(j); }
                }
                close.Status = AccountingPeriodCloseStatus.Reopened;
                close.ReopenedAt = DateTime.UtcNow;
                close.ReopenedBy = user;
                await _db.SaveChangesAsync();
                await tx.CommitAsync();
            }
            finally { _db.AllowClosedPeriodChanges = false; }
        }

        /// <summary>The closed period a date falls in, if any.</summary>
        public async Task<AccountingPeriodClose?> ClosedOn(DateTime date) =>
            await _db.AccountingPeriodCloses.AsNoTracking()
                .FirstOrDefaultAsync(c => c.Status == AccountingPeriodCloseStatus.Closed && c.StartDate <= date.Date && c.EndDate >= date.Date);
    }
}
