using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolWebApp.Core.DTOs.Finance.Reports;
using SchoolWebApp.Core.Entities.Finance;
using SchoolWebApp.Core.Interfaces.IRepositories;
using SchoolWebApp.API.Licensing;
using SchoolWebApp.API.Security;

namespace SchoolWebApp.API.Controllers.Finance
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    [RequiresModule(AppModules.Finance)]
    [RequiresMenu(MenuGroup.Finance)]
    public class FinanceReportsController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly SchoolWebApp.API.Services.AccountingPeriodService _periods;

        public FinanceReportsController(IUnitOfWork unitOfWork, SchoolWebApp.API.Services.AccountingPeriodService periods)
        {
            _unitOfWork = unitOfWork;
            _periods = periods;
        }

        // The dates a term (or, with no term, a year) covers - from its first day to the day before the next starts -
        // for bringing balances forward into it. Null when neither is given.
        private async Task<(DateTime Start, DateTime End)?> Window(int? academicYearId, int? sessionId)
        {
            var p = sessionId.HasValue ? await _periods.TermOf(sessionId.Value)
                : academicYearId.HasValue ? await _periods.YearOf(academicYearId.Value) : null;
            return p == null ? null : (p.Start, p.End);
        }

        // GET api/financeReports/trialBalance?from=2025-01-01&to=2025-12-31
        [HttpGet("trialBalance")]
        public async Task<IActionResult> TrialBalance(DateTime from, DateTime to)
        {
            var accounts = (await _unitOfWork.Accounts.Find()).ToList();
            // Closing entries left out: a closed period still shows what it earned and spent.
            var lines = (await _unitOfWork.JournalLines.GetAllWithEntryAndAccount(from, to))
                .Where(l => !l.JournalEntry!.ReferenceNumber.StartsWith(SchoolWebApp.API.Services.AccountingPeriodService.ClosingPrefix)).ToList();

            var rows = accounts.Select(a =>
            {
                var accLines = lines.Where(l => l.AccountId == a.Id);
                var totalDebit = accLines.Sum(l => l.Debit);
                var totalCredit = accLines.Sum(l => l.Credit);
                var net = totalDebit - totalCredit;
                // For Asset/Expense: debit positive = debit balance
                // For Liability/Equity/Income: credit positive = credit balance
                bool isDebitNatural = a.AccountType == AccountType.Asset || a.AccountType == AccountType.Expense;
                return new TrialBalanceRowDto
                {
                    AccountId = a.Id,
                    AccountCode = a.Code,
                    AccountName = a.Name,
                    AccountType = a.AccountType,
                    Debit = isDebitNatural ? (net > 0 ? net : 0) : (net > 0 ? net : 0),
                    Credit = isDebitNatural ? (net < 0 ? -net : 0) : (net < 0 ? -net : 0)
                };
            }).Where(r => r.Debit != 0 || r.Credit != 0).ToList();

            return Ok(rows);
        }

        // GET api/financeReports/incomeStatement?from=...&to=...
        [HttpGet("incomeStatement")]
        public async Task<IActionResult> IncomeStatement(DateTime from, DateTime to)
        {
            var accounts = (await _unitOfWork.Accounts.Find()).ToList();
            // Closing entries left out: a closed period still shows what it earned and spent.
            var lines = (await _unitOfWork.JournalLines.GetAllWithEntryAndAccount(from, to))
                .Where(l => !l.JournalEntry!.ReferenceNumber.StartsWith(SchoolWebApp.API.Services.AccountingPeriodService.ClosingPrefix)).ToList();

            var income = new List<IncomeStatementLineDto>();
            var expenses = new List<IncomeStatementLineDto>();

            foreach (var a in accounts)
            {
                var accLines = lines.Where(l => l.AccountId == a.Id);
                var totalDebit = accLines.Sum(l => l.Debit);
                var totalCredit = accLines.Sum(l => l.Credit);

                if (a.AccountType == AccountType.Income)
                {
                    var amount = totalCredit - totalDebit;
                    if (amount != 0)
                        income.Add(new IncomeStatementLineDto { AccountId = a.Id, AccountCode = a.Code, AccountName = a.Name, Amount = amount });
                }
                else if (a.AccountType == AccountType.Expense)
                {
                    var amount = totalDebit - totalCredit;
                    if (amount != 0)
                        expenses.Add(new IncomeStatementLineDto { AccountId = a.Id, AccountCode = a.Code, AccountName = a.Name, Amount = amount });
                }
            }

            var totalIncome = income.Sum(x => x.Amount);
            var totalExpenses = expenses.Sum(x => x.Amount);

            return Ok(new IncomeStatementDto
            {
                FromDate = from,
                ToDate = to,
                Income = income.OrderBy(x => x.AccountCode).ToList(),
                Expenses = expenses.OrderBy(x => x.AccountCode).ToList(),
                TotalIncome = totalIncome,
                TotalExpenses = totalExpenses,
                NetProfit = totalIncome - totalExpenses
            });
        }

        // GET api/financeReports/balanceSheet?asOf=2025-12-31
        [HttpGet("balanceSheet")]
        public async Task<IActionResult> BalanceSheet(DateTime asOf)
        {
            var accounts = (await _unitOfWork.Accounts.Find()).ToList();
            // Include all journal lines from earliest date up to asOf
            var lines = await _unitOfWork.JournalLines.GetAllWithEntryAndAccount(new DateTime(1900, 1, 1), asOf);

            var assets = new List<BalanceSheetLineDto>();
            var liabilities = new List<BalanceSheetLineDto>();
            var equity = new List<BalanceSheetLineDto>();
            decimal totalIncome = 0, totalExpenses = 0;

            foreach (var a in accounts)
            {
                var accLines = lines.Where(l => l.AccountId == a.Id);
                var totalDebit = accLines.Sum(l => l.Debit);
                var totalCredit = accLines.Sum(l => l.Credit);

                switch (a.AccountType)
                {
                    case AccountType.Asset:
                        var assetAmt = totalDebit - totalCredit;
                        if (assetAmt != 0)
                            assets.Add(new BalanceSheetLineDto { AccountId = a.Id, AccountCode = a.Code, AccountName = a.Name, Amount = assetAmt });
                        break;
                    case AccountType.Liability:
                        var liabAmt = totalCredit - totalDebit;
                        if (liabAmt != 0)
                            liabilities.Add(new BalanceSheetLineDto { AccountId = a.Id, AccountCode = a.Code, AccountName = a.Name, Amount = liabAmt });
                        break;
                    case AccountType.Equity:
                        var eqAmt = totalCredit - totalDebit;
                        if (eqAmt != 0)
                            equity.Add(new BalanceSheetLineDto { AccountId = a.Id, AccountCode = a.Code, AccountName = a.Name, Amount = eqAmt });
                        break;
                    case AccountType.Income:
                        totalIncome += totalCredit - totalDebit;
                        break;
                    case AccountType.Expense:
                        totalExpenses += totalDebit - totalCredit;
                        break;
                }
            }

            var retainedEarnings = totalIncome - totalExpenses;
            if (retainedEarnings != 0)
                equity.Add(new BalanceSheetLineDto { AccountCode = "3999", AccountName = "Surplus not yet closed", Amount = retainedEarnings });

            return Ok(new BalanceSheetDto
            {
                AsOfDate = asOf,
                Assets = assets.OrderBy(a => a.AccountCode).ToList(),
                Liabilities = liabilities.OrderBy(a => a.AccountCode).ToList(),
                Equity = equity.OrderBy(a => a.AccountCode).ToList(),
                TotalAssets = assets.Sum(a => a.Amount),
                TotalLiabilities = liabilities.Sum(a => a.Amount),
                TotalEquity = equity.Sum(a => a.Amount),
                RetainedEarnings = retainedEarnings
            });
        }

        // GET api/financeReports/feeCollection?academicYearId=&sessionId=&from=&to=
        // The dates are optional: without them the year and term decide what is in.
        [HttpGet("feeCollection")]
        public async Task<IActionResult> FeeCollection(DateTime? from, DateTime? to, int? academicYearId, int? sessionId, int? schoolClassId)
        {
            var start = from ?? DateTime.MinValue;
            var end = to.HasValue ? to.Value.Date.AddDays(1).AddTicks(-1) : DateTime.MaxValue;
            var payments = (await _unitOfWork.Payments.Find(includeProperties: "Student,StudentInvoice"))
                .Where(p => p.PaymentDate >= start && p.PaymentDate <= end
                    && (p.ApprovalStatus == PaymentApprovalStatus.Approved)).ToList();

            var allInvoices = (await _unitOfWork.StudentInvoices.Find(includeProperties: "Student,AcademicYear,Session"))
                .Where(i => i.InvoiceDate >= start && i.InvoiceDate <= end).ToList();

            if (academicYearId.HasValue)
            {
                allInvoices = allInvoices.Where(i => i.AcademicYearId == academicYearId.Value).ToList();
                payments = payments.Where(p => p.StudentInvoice == null || p.StudentInvoice.AcademicYearId == academicYearId.Value).ToList();
            }
            if (sessionId.HasValue)
            {
                allInvoices = allInvoices.Where(i => i.SessionId == sessionId.Value).ToList();
                payments = payments.Where(p => p.StudentInvoice == null || p.StudentInvoice.SessionId == sessionId.Value).ToList();
            }

            // Resolve student → class for the given year
            var studentClassMap = new Dictionary<int, (int ClassId, string ClassName, int Rank)>();
            if (academicYearId.HasValue)
            {
                var studentClasses = (await _unitOfWork.StudentClasses.Find(
                    includeProperties: "SchoolClass,SchoolClass.LearningLevel,SchoolClass.SchoolStream"))
                    .Where(sc => sc.SchoolClass != null && sc.SchoolClass.AcademicYearId == academicYearId.Value);
                foreach (var sc in studentClasses)
                {
                    var level = sc.SchoolClass?.LearningLevel?.Name ?? "";
                    var stream = sc.SchoolClass?.SchoolStream?.Name ?? "";
                    var name = string.IsNullOrEmpty(stream) ? level : $"{level} - {stream}";
                    var rank = sc.SchoolClass?.LearningLevel?.Rank ?? 0;
                    studentClassMap[sc.StudentId] = (sc.SchoolClassId, name, rank);
                }
            }

            if (schoolClassId.HasValue && academicYearId.HasValue)
            {
                var classStudents = studentClassMap.Where(kv => kv.Value.ClassId == schoolClassId.Value).Select(kv => kv.Key).ToHashSet();
                allInvoices = allInvoices.Where(i => classStudents.Contains(i.StudentId)).ToList();
                payments = payments.Where(p => classStudents.Contains(p.StudentId)).ToList();
            }

            // Summary totals: Receipts add to collected, Credit Notes reverse collected (and also add to invoiced via debit note)
            // Net collected = Receipts - Credit Notes. Debit Notes add to invoiced amount.
            decimal CollectedOf(IEnumerable<SchoolWebApp.Core.Entities.Finance.Payment> ps) =>
                ps.Where(p => p.PaymentType == PaymentType.Receipt).Sum(p => p.Amount)
              - ps.Where(p => p.PaymentType == PaymentType.CreditNote).Sum(p => p.Amount);

            decimal ExtraChargesOf(IEnumerable<SchoolWebApp.Core.Entities.Finance.Payment> ps) =>
                ps.Where(p => p.PaymentType == PaymentType.DebitNote).Sum(p => p.Amount);

            var totalInvoiced = allInvoices.Sum(i => i.TotalAmount);
            var totalCollected = CollectedOf(payments);
            var totalOutstanding = allInvoices.Sum(i => i.TotalAmount - i.PaidAmount - i.DiscountAmount);

            // Per-student detail
            var allStudentIds = allInvoices.Select(i => i.StudentId).Concat(payments.Select(p => p.StudentId)).Distinct();
            var studentRows = allStudentIds.Select(sid =>
            {
                var studentInvoices = allInvoices.Where(i => i.StudentId == sid).ToList();
                var studentPayments = payments.Where(p => p.StudentId == sid).ToList();
                var student = studentInvoices.FirstOrDefault()?.Student ?? studentPayments.FirstOrDefault()?.Student;
                var classInfo = studentClassMap.TryGetValue(sid, out var c) ? c : (0, "", 999);
                return new
                {
                    StudentId = sid,
                    StudentUPI = student?.UPI,
                    StudentName = student?.FullName,
                    ClassId = classInfo.Item1,
                    ClassName = classInfo.Item2,
                    LevelRank = classInfo.Item3,
                    Invoiced = studentInvoices.Sum(i => i.TotalAmount),
                    Paid = CollectedOf(studentPayments),
                    Balance = studentInvoices.Sum(i => i.TotalAmount - i.PaidAmount - i.DiscountAmount)
                };
            })
            .Where(r => r.Invoiced > 0 || r.Paid != 0)
            .ToList();

            // Group by class
            var classes = studentRows
                .GroupBy(r => new { r.ClassId, r.ClassName, r.LevelRank })
                .Select(g => new {
                    g.Key.ClassId,
                    ClassName = string.IsNullOrEmpty(g.Key.ClassName) ? "(No Class Assigned)" : g.Key.ClassName,
                    g.Key.LevelRank,
                    StudentCount = g.Count(),
                    TotalInvoiced = g.Sum(r => r.Invoiced),
                    TotalCollected = g.Sum(r => r.Paid),
                    TotalOutstanding = g.Sum(r => r.Balance),
                    Students = g.OrderBy(r => r.StudentName).ToList()
                })
                .OrderBy(c => c.LevelRank)
                .ThenBy(c => c.ClassName)
                .ToList();

            return Ok(new
            {
                fromDate = from,
                toDate = to,
                totalInvoiced,
                totalCollected,
                totalOutstanding,
                invoiceCount = allInvoices.Count,
                paidCount = allInvoices.Count(i => i.Status == InvoiceStatus.Paid),
                classes
            });
        }

        // GET api/financeReports/consolidatedBudget?academicYearId=1
        [HttpGet("consolidatedBudget")]
        public async Task<IActionResult> ConsolidatedBudget(int academicYearId)
        {
            // Only approved/active budgets are included - drafts pending approval must not show in reports.
            var budgets = (await _unitOfWork.Budgets.GetAllWithLines())
                .Where(b => b.AcademicYearId == academicYearId && b.IsActive)
                .ToList();

            if (budgets.Count == 0)
            {
                var yearOnly = (await _unitOfWork.Repository<SchoolWebApp.Core.Entities.Academics.AcademicYear>().Find(y => y.Id == academicYearId)).FirstOrDefault();
                return Ok(new ConsolidatedBudgetDto { AcademicYearId = academicYearId, AcademicYearName = yearOnly?.Name });
            }

            var yearName = budgets.First().AcademicYear?.Name;

            // Date range covering all budgets in this year (for actuals lookup)
            var minDate = budgets.Min(b => b.StartDate);
            var maxDate = budgets.Max(b => b.EndDate);
            var journalLines = await _unitOfWork.JournalLines.GetAllWithEntryAndAccount(minDate, maxDate);
            var actualsByAccount = journalLines
                .GroupBy(l => l.AccountId)
                .ToDictionary(
                    g => g.Key,
                    g => g.Sum(l => (l.Account != null && (l.Account.AccountType == AccountType.Income || l.Account.AccountType == AccountType.Liability || l.Account.AccountType == AccountType.Equity))
                        ? l.Credit - l.Debit
                        : l.Debit - l.Credit)
                );

            // An account only one department budgets takes its actual from the ledger. One that several
            // departments budget (stationery, transport...) is split by the approved expenses each
            // department charged to its own budget lines - otherwise every one of them would show the
            // whole school's spend on it, and the totals would count it several times.
            var sharedAccounts = budgets.SelectMany(b => b.Lines.Select(l => new { b.DepartmentId, l.AccountId })).Distinct()
                .GroupBy(x => x.AccountId).Where(g => g.Count() > 1).Select(g => g.Key).ToHashSet();
            var budgetLineIds = budgets.SelectMany(b => b.Lines).Select(l => l.Id).ToList();
            var chargedByLine = (await _unitOfWork.Repository<ExpenseLine>().Find(l => l.BudgetLineId != null
                        && budgetLineIds.Contains(l.BudgetLineId.Value) && l.Expense!.Status == ExpenseStatus.Approved
                        && l.Expense.ExpenseDate >= minDate && l.Expense.ExpenseDate <= maxDate,
                    includeProperties: "Expense"))
                .GroupBy(l => l.BudgetLineId!.Value)
                .ToDictionary(g => g.Key, g => g.Sum(l => l.Amount));

            // Sum of approved-amendment deltas per (BudgetId, AccountId) - added on top of each budget line.
            var budgetIds = budgets.Select(b => b.Id).ToHashSet();
            var approvedAmendments = (await _unitOfWork.BudgetAmendments.Find(
                    a => budgetIds.Contains(a.BudgetId) && a.Status == BudgetAmendmentStatus.Approved,
                    includeProperties: "Lines"))
                .ToList();
            var deltaByBudgetAccount = approvedAmendments
                .SelectMany(a => a.Lines.Select(l => new { a.BudgetId, l.AccountId, l.Delta }))
                .GroupBy(x => new { x.BudgetId, x.AccountId })
                .ToDictionary(g => g.Key, g => g.Sum(x => x.Delta));

            decimal Effective(int budgetId, int accountId, decimal budgeted)
            {
                return deltaByBudgetAccount.TryGetValue(new { BudgetId = budgetId, AccountId = accountId }, out var d)
                    ? budgeted + d
                    : budgeted;
            }

            var result = new ConsolidatedBudgetDto
            {
                AcademicYearId = academicYearId,
                AcademicYearName = yearName,
                StartDate = minDate,
                EndDate = maxDate
            };

            foreach (var deptGroup in budgets.GroupBy(b => new { b.DepartmentId, DeptName = b.Department?.Name }))
            {
                var deptDto = new ConsolidatedBudgetDepartmentDto
                {
                    DepartmentId = deptGroup.Key.DepartmentId,
                    DepartmentName = deptGroup.Key.DeptName,
                    BudgetCount = deptGroup.Count(),
                    Budgets = deptGroup.Select(b => new ConsolidatedBudgetEntryDto
                    {
                        BudgetId = b.Id,
                        Name = b.Name,
                        StartDate = b.StartDate,
                        EndDate = b.EndDate,
                        TotalBudgeted = b.Lines.Sum(l => Effective(b.Id, l.AccountId, l.BudgetedAmount))
                    }).OrderBy(e => e.StartDate).ToList()
                };

                // Aggregate budget lines per account across all the department's budgets,
                // using effective (budgeted + approved-amendment-delta).
                var allLines = deptGroup.SelectMany(b => b.Lines.Select(l => new {
                    BudgetId = b.Id,
                    LineId = l.Id,
                    l.AccountId,
                    l.Account,
                    EffectiveAmt = Effective(b.Id, l.AccountId, l.BudgetedAmount)
                })).Where(l => l.Account != null);

                var lineGroups = allLines.GroupBy(l => l.AccountId);

                foreach (var lg in lineGroups)
                {
                    var acc = lg.First().Account!;
                    var budgeted = lg.Sum(l => l.EffectiveAmt);
                    var actual = sharedAccounts.Contains(acc.Id)
                        ? lg.Sum(l => chargedByLine.GetValueOrDefault(l.LineId))
                        : actualsByAccount.TryGetValue(acc.Id, out var v) ? v : 0;
                    var line = new ConsolidatedBudgetLineDto
                    {
                        AccountId = acc.Id,
                        AccountCode = acc.Code,
                        AccountName = acc.Name,
                        AccountType = acc.AccountType.ToString(),
                        Budgeted = budgeted,
                        Actual = actual,
                        Variance = budgeted - actual,
                        Shared = sharedAccounts.Contains(acc.Id)
                    };
                    deptDto.Lines.Add(line);

                    if (acc.AccountType == AccountType.Income)
                    {
                        deptDto.TotalBudgetedIncome += budgeted;
                        deptDto.TotalActualIncome += actual;
                    }
                    else if (acc.AccountType == AccountType.Expense)
                    {
                        deptDto.TotalBudgetedExpense += budgeted;
                        deptDto.TotalActualExpense += actual;
                    }
                }

                deptDto.Lines = deptDto.Lines.OrderBy(l => l.AccountCode).ToList();
                result.Departments.Add(deptDto);

                result.GrandBudgetedIncome += deptDto.TotalBudgetedIncome;
                result.GrandBudgetedExpense += deptDto.TotalBudgetedExpense;
                result.GrandActualIncome += deptDto.TotalActualIncome;
                result.GrandActualExpense += deptDto.TotalActualExpense;
            }

            result.Departments = result.Departments.OrderBy(d => d.DepartmentName).ToList();
            return Ok(result);
        }

        // GET api/financeReports/outstandingBalances
        [HttpGet("outstandingBalances")]
        public async Task<IActionResult> OutstandingBalances(int? academicYearId, int? sessionId, string? search)
        {
            var invoices = await _unitOfWork.StudentInvoices.Find(includeProperties: "Student,Items");
            var filtered = invoices.AsEnumerable();
            if (academicYearId.HasValue) filtered = filtered.Where(i => i.AcademicYearId == academicYearId.Value);
            if (sessionId.HasValue) filtered = filtered.Where(i => i.SessionId == sessionId.Value);
            if (!string.IsNullOrWhiteSpace(search))
            {
                var q = search.Trim().ToLower();
                filtered = filtered.Where(i =>
                    (i.Student != null && ((i.Student.FullName ?? "").ToLower().Contains(q) || (i.Student.UPI ?? "").ToLower().Contains(q))));
            }
            // What students still owe on invoices from before the term (or year) comes forward into it - so a student
            // owing from earlier is listed even with nothing invoiced in this term.
            var window = await Window(academicYearId, sessionId);
            var term = filtered.ToList();
            var earlier = window == null ? new List<StudentInvoice>()
                : invoices.Where(i => i.InvoiceDate.Date < window.Value.Start.Date && i.Status != InvoiceStatus.Cancelled
                    && (string.IsNullOrWhiteSpace(search) || (i.Student != null && ((i.Student.FullName ?? "").ToLower().Contains(search.Trim().ToLower())
                        || (i.Student.UPI ?? "").ToLower().Contains(search.Trim().ToLower()))))).ToList();
            var grouped = term.Select(i => i.StudentId).Concat(earlier.Select(i => i.StudentId)).Distinct().Select(id =>
            {
                var t = term.Where(i => i.StudentId == id).ToList();
                var e = earlier.Where(i => i.StudentId == id).ToList();
                var s = (t.FirstOrDefault() ?? e.First()).Student;
                var bf = e.Sum(i => i.TotalAmount - i.PaidAmount - i.DiscountAmount);
                return new OutstandingBalanceRowDto
                {
                    StudentId = id,
                    StudentUPI = s?.UPI,
                    StudentName = s?.FullName,
                    BroughtForward = bf,
                    TotalInvoiced = t.Sum(i => i.TotalAmount),
                    TotalPaid = t.Sum(i => i.PaidAmount),
                    Balance = bf + t.Sum(i => i.TotalAmount - i.PaidAmount - i.DiscountAmount),
                    AwaitingSponsor = e.Concat(t).SelectMany(i => i.Items).Sum(AwaitingSponsor)
                };
            }).Where(r => r.Balance > 0).OrderByDescending(r => r.Balance).ToList();
            return Ok(grouped);
        }
        [HttpGet("feeBalancesByClass")]
        public async Task<IActionResult> FeeBalancesByClass(int? academicYearId, int? sessionId, int? schoolClassId)
        {
            // Invoices filtered by academic year / session
            var invoices = await _unitOfWork.StudentInvoices.Find(includeProperties: "Student");
            var filteredInv = invoices.AsEnumerable();
            if (academicYearId.HasValue) filteredInv = filteredInv.Where(i => i.AcademicYearId == academicYearId.Value);
            if (sessionId.HasValue) filteredInv = filteredInv.Where(i => i.SessionId == sessionId.Value);

            // What students still owe on invoices from before the term (or year) comes forward into it.
            var window = await Window(academicYearId, sessionId);
            var termInv = filteredInv.ToList();
            var earlier = window == null ? new List<StudentInvoice>()
                : invoices.Where(i => i.InvoiceDate.Date < window.Value.Start.Date && i.Status != InvoiceStatus.Cancelled).ToList();

            // Per student: arrears brought forward, this term's invoices and payments, and the balance - a student owing
            // from earlier is listed even with nothing invoiced in this term.
            var studentBalances = termInv.Select(i => i.StudentId).Concat(earlier.Select(i => i.StudentId)).Distinct().Select(id =>
            {
                var t = termInv.Where(i => i.StudentId == id).ToList();
                var e = earlier.Where(i => i.StudentId == id).ToList();
                var s = (t.FirstOrDefault() ?? e.First()).Student;
                var bf = e.Sum(i => i.TotalAmount - i.PaidAmount - i.DiscountAmount);
                return new
                {
                    StudentId = id,
                    StudentUPI = s?.UPI,
                    StudentName = s?.FullName,
                    BroughtForward = bf,
                    TotalInvoiced = t.Sum(i => i.TotalAmount),
                    TotalPaid = t.Sum(i => i.PaidAmount),
                    Balance = bf + t.Sum(i => i.TotalAmount - i.PaidAmount - i.DiscountAmount)
                };
            }).Where(r => r.Balance > 0).ToDictionary(r => r.StudentId);

            // Student classes for the given academic year (and optional class)
            var studentClasses = (await _unitOfWork.StudentClasses.Find(
                includeProperties: "Student,SchoolClass,SchoolClass.LearningLevel,SchoolClass.SchoolStream,SchoolClass.AcademicYear"))
                .Where(sc => sc.Student != null && sc.Student.Status == SchoolWebApp.Core.Entities.Enums.Status.Active);

            if (academicYearId.HasValue)
                studentClasses = studentClasses.Where(sc => sc.SchoolClass != null && sc.SchoolClass.AcademicYearId == academicYearId.Value);
            if (schoolClassId.HasValue)
                studentClasses = studentClasses.Where(sc => sc.SchoolClassId == schoolClassId.Value);

            var classGroups = studentClasses
                .GroupBy(sc => sc.SchoolClassId)
                .Select(g =>
                {
                    var firstClass = g.First().SchoolClass;
                    var levelName = firstClass?.LearningLevel?.Name ?? "";
                    var streamName = firstClass?.SchoolStream?.Name ?? "";
                    var className = string.IsNullOrEmpty(streamName) ? levelName : $"{levelName} - {streamName}";
                    var rank = firstClass?.LearningLevel?.Rank ?? 0;

                    var rows = g
                        .Where(sc => studentBalances.ContainsKey(sc.StudentId))
                        .Select(sc =>
                        {
                            var b = studentBalances[sc.StudentId];
                            return new {
                                b.StudentId,
                                b.StudentUPI,
                                b.StudentName,
                                b.BroughtForward,
                                b.TotalInvoiced,
                                b.TotalPaid,
                                b.Balance
                            };
                        })
                        .OrderBy(r => r.StudentName)
                        .ToList();

                    return new {
                        SchoolClassId = g.Key,
                        ClassName = className,
                        LevelRank = rank,
                        StudentCount = rows.Count,
                        TotalBalance = rows.Sum(r => r.Balance),
                        Students = rows
                    };
                })
                .Where(c => c.StudentCount > 0)
                .OrderBy(c => c.LevelRank)
                .ThenBy(c => c.ClassName)
                .ToList();

            return Ok(new
            {
                academicYearId,
                sessionId,
                schoolClassId,
                totalClasses = classGroups.Count,
                grandTotal = classGroups.Sum(c => c.TotalBalance),
                classes = classGroups
            });
        }

        private static decimal AwaitingSponsor(StudentInvoiceItem it) =>
            Math.Max(0, Math.Min(it.SponsoredAmount - it.SponsorPaidAmount, it.Amount - it.Discount - it.PaidAmount));

        [HttpGet("studentStatement")]
        public async Task<IActionResult> StudentStatement(int studentId, int? academicYearId, int? sessionId, DateTime? from, DateTime? to)
        {
            var student = await _unitOfWork.Students.GetById(studentId);
            if (student == null) return NotFound(new { message = "Student not found." });

            // A statement for a term (or a year, or dates) covers what happened in it, opening with the balance brought
            // forward from before: everything invoiced, paid and noted earlier, so arrears carry into the new term.
            var window = await Window(academicYearId, sessionId);
            var start = from ?? window?.Start;
            var end = to ?? window?.End;
            bool Within(DateTime d) => (start == null || d.Date >= start.Value.Date) && (end == null || d.Date <= end.Value.Date);
            bool Before(DateTime d) => start != null && d.Date < start.Value.Date;

            // Only this student's records are read.
            var allInvoices = (await _unitOfWork.StudentInvoices.Find(i => i.StudentId == studentId && i.Status != InvoiceStatus.Cancelled,
                includeProperties: "AcademicYear,Session,Items.FeeCategory,Items.Sponsorship.Sponsor")).ToList();
            var allPayments = (await _unitOfWork.Payments.Find(p => p.StudentId == studentId && p.ApprovalStatus == PaymentApprovalStatus.Approved,
                includeProperties: "StudentInvoice,BankAccount,SponsorPayment.Sponsor")).ToList();
            var invoices = allInvoices.Where(i => Within(i.InvoiceDate)).ToList();
            var payments = allPayments.Where(p => Within(p.PaymentDate)).ToList();
            var broughtForward = allInvoices.Where(i => Before(i.InvoiceDate)).Sum(i => i.TotalAmount - i.DiscountAmount)
                - allPayments.Where(p => Before(p.PaymentDate)).Sum(p => p.PaymentType == PaymentType.Receipt ? p.Amount : -p.Amount);

            // Build transaction list: invoices = debit, payments = credit, credit notes = debit, debit notes = debit
            var transactions = new List<(DateTime Date, string Type, string Reference, string Description, decimal Debit, decimal Credit, string? CreatedBy, string? Method, string SourceType, int SourceId)>();

            foreach (var inv in invoices)
            {
                var itemDesc = string.Join(", ", inv.Items.Select(it => it.FeeCategory?.Name ?? "").Where(n => !string.IsNullOrEmpty(n)));
                if (itemDesc.Length == 0) itemDesc = inv.Description ?? "";
                // The student is invoiced the whole fee; the line says what a discount took off and what sponsors have
                // pledged to pay of it, so the statement shows who is expected to pay what.
                var notes = new List<string>();
                var discount = inv.Items.Sum(it => it.Discount);
                if (discount > 0) notes.Add($"less {discount:N0} discount");
                notes.AddRange(inv.Items.Where(it => it.SponsoredAmount > 0).GroupBy(it => it.Sponsorship?.Sponsor?.Name ?? "a sponsor")
                    .Select(g => $"{g.Sum(it => it.SponsoredAmount):N0} pledged by {g.Key}"));
                if (notes.Count > 0) itemDesc += $" ({string.Join("; ", notes)})";
                transactions.Add((inv.InvoiceDate, "Invoice", inv.InvoiceNumber, itemDesc, inv.TotalAmount, 0m, inv.CreatedBy, null, "invoice", inv.Id));
            }

            foreach (var p in payments)
            {
                if (p.PaymentType == PaymentType.Receipt)
                    transactions.Add((p.PaymentDate, p.SponsorPaymentId != null ? "Sponsor payment" : "Receipt", p.ReceiptNumber, p.Description ?? "", 0m, p.Amount, p.CreatedBy, FinanceDrilldownController.Label(p.PaymentMethod), "payment", p.Id));
                else if (p.PaymentType == PaymentType.CreditNote)
                    transactions.Add((p.PaymentDate, "Credit Note", p.ReceiptNumber, p.Reason ?? p.Description ?? "", p.Amount, 0m, p.CreatedBy, null, "payment", p.Id));
                else if (p.PaymentType == PaymentType.DebitNote)
                    transactions.Add((p.PaymentDate, "Debit Note", p.ReceiptNumber, p.Reason ?? p.Description ?? "", p.Amount, 0m, p.CreatedBy, null, "payment", p.Id));
            }

            transactions = transactions.OrderBy(t => t.Date).ThenBy(t => t.Reference).ToList();

            decimal running = broughtForward;
            var rows = transactions.Select(t =>
            {
                running += t.Debit - t.Credit;
                return new
                {
                    date = t.Date,
                    type = t.Type,
                    reference = t.Reference,
                    description = t.Description,
                    debit = t.Debit,
                    credit = t.Credit,
                    balance = running,
                    createdBy = t.CreatedBy,
                    method = t.Method,
                    sourceType = t.SourceType,
                    sourceId = t.SourceId
                };
            }).ToList();
            if (start != null)
                rows.Insert(0, new
                {
                    date = start.Value, type = "Balance b/f", reference = "",
                    description = broughtForward == 0 ? "Nothing owed from before" : "Balance brought forward from before",
                    debit = broughtForward > 0 ? broughtForward : 0m, credit = broughtForward < 0 ? -broughtForward : 0m,
                    balance = broughtForward, createdBy = (string?)null, method = (string?)null, sourceType = "", sourceId = 0
                });

            return Ok(new
            {
                student = new { id = student.Id, upi = student.UPI, fullName = student.FullName },
                fromDate = start,
                toDate = end,
                academicYearId,
                sessionId,
                broughtForward,
                totalInvoiced = transactions.Sum(t => t.Debit),
                totalPaid = transactions.Sum(t => t.Credit),
                balance = running,
                // Sponsors' part: pledged on this period's invoices, paid in it, and still awaited (part of the balance).
                sponsorPledged = invoices.SelectMany(i => i.Items).Sum(it => it.SponsoredAmount),
                sponsorPaid = payments.Where(p => p.PaymentType == PaymentType.Receipt && p.SponsorPaymentId != null).Sum(p => p.Amount),
                awaitingSponsor = allInvoices.Where(i => end == null || i.InvoiceDate.Date <= end.Value.Date).SelectMany(i => i.Items).Sum(AwaitingSponsor),
                paidByFamily = payments.Where(p => p.PaymentType == PaymentType.Receipt && p.SponsorPaymentId == null).Sum(p => p.Amount),
                transactions = rows
            });
        }

        // GET api/financeReports/studentDiscounts?academicYearId=1&sessionId=2&schoolClassId=3&sponsorId=4
        // Students whose fees are partly discounted or pledged by a sponsor: per student the fees, any discount, what
        // sponsors pledged and have paid, and the family's own share. sponsorId narrows to one sponsor's pledges;
        // -1 to discounts only.
        [HttpGet("studentDiscounts")]
        public async Task<IActionResult> StudentDiscounts(int? academicYearId, int? sessionId, int? schoolClassId, int? sponsorId)
        {
            var invoices = (await _unitOfWork.StudentInvoices.Find(
                includeProperties: "Student,AcademicYear,Session,Items.FeeCategory,Items.Sponsorship.Sponsor")).ToList();
            if (academicYearId.HasValue) invoices = invoices.Where(i => i.AcademicYearId == academicYearId.Value).ToList();
            if (sessionId.HasValue) invoices = invoices.Where(i => i.SessionId == sessionId.Value).ToList();

            bool discounts = sponsorId == null || sponsorId == -1;
            bool Pledge(StudentInvoiceItem it) => it.SponsoredAmount > 0 && (sponsorId == null || it.Sponsorship?.SponsorId == sponsorId);
            bool Counts(StudentInvoiceItem it) => Pledge(it) || (discounts && it.Discount > 0);
            invoices = invoices.Where(i => (discounts && i.DiscountAmount > 0) || i.Items.Any(Counts)).ToList();

            // Resolve student class for the given year
            var studentClassMap = new Dictionary<int, (int ClassId, string ClassName, int Rank)>();
            if (academicYearId.HasValue)
            {
                var studentClasses = (await _unitOfWork.StudentClasses.Find(
                    includeProperties: "SchoolClass,SchoolClass.LearningLevel,SchoolClass.SchoolStream"))
                    .Where(sc => sc.SchoolClass != null && sc.SchoolClass.AcademicYearId == academicYearId.Value);
                foreach (var sc in studentClasses)
                {
                    var level = sc.SchoolClass?.LearningLevel?.Name ?? "";
                    var stream = sc.SchoolClass?.SchoolStream?.Name ?? "";
                    var name = string.IsNullOrEmpty(stream) ? level : $"{level} - {stream}";
                    studentClassMap[sc.StudentId] = (sc.SchoolClassId, name, sc.SchoolClass?.LearningLevel?.Rank ?? 0);
                }
            }
            if (schoolClassId.HasValue && academicYearId.HasValue)
            {
                var classStudents = studentClassMap.Where(kv => kv.Value.ClassId == schoolClassId.Value).Select(kv => kv.Key).ToHashSet();
                invoices = invoices.Where(i => classStudents.Contains(i.StudentId)).ToList();
            }

            var rows = invoices.GroupBy(i => i.StudentId).Select(g =>
            {
                var s = g.First().Student;
                var classInfo = studentClassMap.TryGetValue(g.Key, out var c) ? c : (0, "", 999);
                var items = g.SelectMany(i => i.Items).ToList();
                var discount = (discounts ? items.Sum(it => it.Discount) + g.Sum(i => i.DiscountAmount) : 0m);
                var pledged = items.Where(Pledge).Sum(it => it.SponsoredAmount);
                var sponsorPaid = items.Where(Pledge).Sum(it => it.SponsorPaidAmount);
                var givers = items.Where(Pledge).Select(it => it.Sponsorship?.Sponsor?.Name ?? "Sponsor")
                    .Concat(discount > 0 ? new[] { "School discount" } : Array.Empty<string>()).Distinct();
                return new
                {
                    studentId = g.Key,
                    studentUPI = s?.UPI,
                    studentName = s?.FullName,
                    className = string.IsNullOrEmpty(classInfo.Item2) ? "(No Class Assigned)" : classInfo.Item2,
                    levelRank = classInfo.Item3,
                    givenBy = string.Join(", ", givers),
                    totalFees = items.Sum(it => it.Amount),
                    totalDiscount = discount,
                    sponsorPledged = pledged,
                    sponsorPaid,
                    // What the family is expected to pay itself: the fees less discounts and sponsors' pledges.
                    familyShare = items.Sum(it => it.Amount - it.Discount - it.SponsoredAmount) - g.Sum(i => i.DiscountAmount),
                    invoiceCount = g.Count(),
                    invoices = g.OrderBy(i => i.InvoiceDate).Select(inv => new
                    {
                        invoiceId = inv.Id,
                        invoiceNumber = inv.InvoiceNumber,
                        invoiceDate = inv.InvoiceDate,
                        academicYear = inv.AcademicYear?.Name,
                        session = inv.Session?.SessionName,
                        totalAmount = inv.TotalAmount - inv.DiscountAmount,
                        headerDiscount = discounts ? inv.DiscountAmount : 0m,
                        items = inv.Items.Where(Counts).Select(it => new
                        {
                            feeCategory = it.FeeCategory?.Name,
                            amount = it.Amount,
                            discount = discounts ? it.Discount : 0m,
                            pledged = Pledge(it) ? it.SponsoredAmount : 0m,
                            sponsorPaid = Pledge(it) ? it.SponsorPaidAmount : 0m,
                            givenBy = it.SponsoredAmount > 0 ? it.Sponsorship?.Sponsor?.Name ?? "Sponsor" : "School discount",
                            description = it.Description
                        }).ToList()
                    }).ToList()
                };
            })
            .Where(r => r.totalDiscount > 0 || r.sponsorPledged > 0)
            .OrderBy(r => r.levelRank).ThenBy(r => r.className).ThenBy(r => r.studentName)
            .ToList();

            return Ok(new
            {
                academicYearId,
                sessionId,
                schoolClassId,
                studentCount = rows.Count,
                totalFees = rows.Sum(r => r.totalFees),
                totalDiscount = rows.Sum(r => r.totalDiscount),
                totalPledged = rows.Sum(r => r.sponsorPledged),
                totalSponsorPaid = rows.Sum(r => r.sponsorPaid),
                totalFamilyShare = rows.Sum(r => r.familyShare),
                students = rows
            });
        }

        // GET api/financeReports/creditDebitNotes?from=...&to=...&type=0|1&status=0..3
        [HttpGet("creditDebitNotes")]
        public async Task<IActionResult> CreditDebitNotes(DateTime from, DateTime to, int? academicYearId, int? sessionId, int? schoolClassId, string? noteType, string? status)
        {
            var fromStart = from.Date;
            var toEnd = to.Date.AddDays(1).AddTicks(-1);

            var payments = (await _unitOfWork.Payments.Find(
                includeProperties: "Student,StudentInvoice,StudentInvoice.AcademicYear,StudentInvoice.Session,OriginalPayment"))
                .Where(p => p.PaymentType != PaymentType.Receipt
                    && p.PaymentDate >= fromStart && p.PaymentDate <= toEnd).ToList();

            if (!string.IsNullOrEmpty(noteType))
            {
                if (noteType.Equals("credit", StringComparison.OrdinalIgnoreCase))
                    payments = payments.Where(p => p.PaymentType == PaymentType.CreditNote).ToList();
                else if (noteType.Equals("debit", StringComparison.OrdinalIgnoreCase))
                    payments = payments.Where(p => p.PaymentType == PaymentType.DebitNote).ToList();
            }

            if (!string.IsNullOrEmpty(status))
            {
                if (Enum.TryParse<PaymentApprovalStatus>(status, true, out var s))
                    payments = payments.Where(p => p.ApprovalStatus == s).ToList();
            }

            if (academicYearId.HasValue)
                payments = payments.Where(p => p.StudentInvoice == null || p.StudentInvoice.AcademicYearId == academicYearId.Value).ToList();
            if (sessionId.HasValue)
                payments = payments.Where(p => p.StudentInvoice == null || p.StudentInvoice.SessionId == sessionId.Value).ToList();

            // Resolve student class for the year
            var studentClassMap = new Dictionary<int, (int ClassId, string ClassName, int Rank)>();
            if (academicYearId.HasValue)
            {
                var studentClasses = (await _unitOfWork.StudentClasses.Find(
                    includeProperties: "SchoolClass,SchoolClass.LearningLevel,SchoolClass.SchoolStream"))
                    .Where(sc => sc.SchoolClass != null && sc.SchoolClass.AcademicYearId == academicYearId.Value);
                foreach (var sc in studentClasses)
                {
                    var level = sc.SchoolClass?.LearningLevel?.Name ?? "";
                    var stream = sc.SchoolClass?.SchoolStream?.Name ?? "";
                    var name = string.IsNullOrEmpty(stream) ? level : $"{level} - {stream}";
                    var rank = sc.SchoolClass?.LearningLevel?.Rank ?? 0;
                    studentClassMap[sc.StudentId] = (sc.SchoolClassId, name, rank);
                }
            }

            if (schoolClassId.HasValue && academicYearId.HasValue)
            {
                var classStudents = studentClassMap.Where(kv => kv.Value.ClassId == schoolClassId.Value).Select(kv => kv.Key).ToHashSet();
                payments = payments.Where(p => classStudents.Contains(p.StudentId)).ToList();
            }

            var rows = payments.Select(p =>
            {
                var classInfo = studentClassMap.TryGetValue(p.StudentId, out var c) ? c : (0, "", 999);
                return new
                {
                    id = p.Id,
                    receiptNumber = p.ReceiptNumber,
                    paymentType = p.PaymentType.ToString(),
                    approvalStatus = p.ApprovalStatus.ToString(),
                    paymentDate = p.PaymentDate,
                    studentId = p.StudentId,
                    studentUPI = p.Student?.UPI,
                    studentName = p.Student?.FullName,
                    className = string.IsNullOrEmpty(classInfo.Item2) ? "" : classInfo.Item2,
                    levelRank = classInfo.Item3,
                    invoiceNumber = p.StudentInvoice?.InvoiceNumber,
                    academicYear = p.StudentInvoice?.AcademicYear?.Name,
                    session = p.StudentInvoice?.Session?.SessionName,
                    originalReceipt = p.OriginalPayment?.ReceiptNumber,
                    amount = p.Amount,
                    reason = p.Reason,
                    description = p.Description,
                    createdBy = p.CreatedBy
                };
            })
            .OrderBy(r => r.paymentDate)
            .ThenBy(r => r.receiptNumber)
            .ToList();

            return Ok(new
            {
                fromDate = from,
                toDate = to,
                academicYearId,
                sessionId,
                schoolClassId,
                noteType,
                status,
                totalCount = rows.Count,
                creditCount = rows.Count(r => r.paymentType == "CreditNote"),
                debitCount = rows.Count(r => r.paymentType == "DebitNote"),
                totalCredit = rows.Where(r => r.paymentType == "CreditNote").Sum(r => r.amount),
                totalDebit = rows.Where(r => r.paymentType == "DebitNote").Sum(r => r.amount),
                notes = rows
            });
        }

        [HttpGet("expenseReport")]
        public async Task<IActionResult> ExpenseReport(DateTime from, DateTime to)
        {
            var expenses = await _unitOfWork.Expenses.GetAllWithLines();
            var filtered = expenses.Where(e => e.ExpenseDate >= from && e.ExpenseDate <= to && e.Status == ExpenseStatus.Approved).ToList();
            var lines = filtered.SelectMany(e => e.Lines.Select(l => new {
                ExpenseId = e.Id,
                e.ReferenceNumber,
                e.ExpenseDate,
                l.ExpenseCategoryId,
                l.ExpenseCategory,
                l.Amount,
                l.Vendor,
                l.Description
            }));
            var byCategory = lines.GroupBy(l => new { l.ExpenseCategoryId, Name = l.ExpenseCategory?.Name ?? "Uncategorized" }).Select(g => new {
                CategoryId = g.Key.ExpenseCategoryId,
                Category = g.Key.Name,
                Amount = g.Sum(x => x.Amount),
                Count = g.Count()
            }).OrderByDescending(x => x.Amount).ToList();

            var detail = lines.Select(l => new {
                l.ExpenseId,
                l.ExpenseCategoryId,
                l.ReferenceNumber,
                Date = l.ExpenseDate,
                Category = l.ExpenseCategory?.Name,
                l.Vendor,
                l.Amount,
                l.Description
            }).OrderBy(x => x.Date).ToList();

            return Ok(new {
                fromDate = from,
                toDate = to,
                totalAmount = lines.Sum(l => l.Amount),
                totalCount = filtered.Count,
                byCategory,
                detail
            });
        }
    }
}
