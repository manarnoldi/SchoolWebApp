using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolWebApp.Core.DTOs.Finance.Reports;
using SchoolWebApp.Core.Entities.Finance;
using SchoolWebApp.Core.Interfaces.IRepositories;

namespace SchoolWebApp.API.Controllers.Finance
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class FinanceReportsController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;

        public FinanceReportsController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        // GET api/financeReports/trialBalance?from=2025-01-01&to=2025-12-31
        [HttpGet("trialBalance")]
        public async Task<IActionResult> TrialBalance(DateTime from, DateTime to)
        {
            var accounts = (await _unitOfWork.Accounts.Find()).ToList();
            var lines = await _unitOfWork.JournalLines.GetAllWithEntryAndAccount(from, to);

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
            var lines = await _unitOfWork.JournalLines.GetAllWithEntryAndAccount(from, to);

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
                equity.Add(new BalanceSheetLineDto { AccountCode = "3999", AccountName = "Retained Earnings (YTD)", Amount = retainedEarnings });

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

        // GET api/financeReports/feeCollection?from=...&to=...
        [HttpGet("feeCollection")]
        public async Task<IActionResult> FeeCollection(DateTime from, DateTime to, int? academicYearId, int? sessionId, int? schoolClassId)
        {
            var payments = (await _unitOfWork.Payments.Find(includeProperties: "Student,StudentInvoice"))
                .Where(p => p.PaymentDate >= from && p.PaymentDate <= to
                    && (p.ApprovalStatus == PaymentApprovalStatus.Approved)).ToList();

            var allInvoices = (await _unitOfWork.StudentInvoices.Find(includeProperties: "Student,AcademicYear,Session"))
                .Where(i => i.InvoiceDate >= from && i.InvoiceDate <= to).ToList();

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
            // Only approved/active budgets are included — drafts pending approval must not show in reports.
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

            // Sum of approved-amendment deltas per (BudgetId, AccountId) — added on top of each budget line.
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
                AcademicYearName = yearName
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
                    l.AccountId,
                    l.Account,
                    EffectiveAmt = Effective(b.Id, l.AccountId, l.BudgetedAmount)
                })).Where(l => l.Account != null);

                var lineGroups = allLines.GroupBy(l => l.AccountId);

                foreach (var lg in lineGroups)
                {
                    var acc = lg.First().Account!;
                    var budgeted = lg.Sum(l => l.EffectiveAmt);
                    var actual = actualsByAccount.TryGetValue(acc.Id, out var v) ? v : 0;
                    var line = new ConsolidatedBudgetLineDto
                    {
                        AccountId = acc.Id,
                        AccountCode = acc.Code,
                        AccountName = acc.Name,
                        AccountType = acc.AccountType.ToString(),
                        Budgeted = budgeted,
                        Actual = actual,
                        Variance = budgeted - actual
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
            var invoices = await _unitOfWork.StudentInvoices.Find(includeProperties: "Student");
            var filtered = invoices.AsEnumerable();
            if (academicYearId.HasValue) filtered = filtered.Where(i => i.AcademicYearId == academicYearId.Value);
            if (sessionId.HasValue) filtered = filtered.Where(i => i.SessionId == sessionId.Value);
            if (!string.IsNullOrWhiteSpace(search))
            {
                var q = search.Trim().ToLower();
                filtered = filtered.Where(i =>
                    (i.Student != null && ((i.Student.FullName ?? "").ToLower().Contains(q) || (i.Student.UPI ?? "").ToLower().Contains(q))));
            }
            var grouped = filtered.GroupBy(i => i.StudentId).Select(g =>
            {
                var s = g.First().Student;
                return new OutstandingBalanceRowDto
                {
                    StudentId = g.Key,
                    StudentUPI = s?.UPI,
                    StudentName = s?.FullName,
                    TotalInvoiced = g.Sum(i => i.TotalAmount),
                    TotalPaid = g.Sum(i => i.PaidAmount),
                    Balance = g.Sum(i => i.TotalAmount - i.PaidAmount - i.DiscountAmount)
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

            // Group invoices by student to get outstanding
            var studentBalances = filteredInv.GroupBy(i => i.StudentId).Select(g =>
            {
                var s = g.First().Student;
                return new
                {
                    StudentId = g.Key,
                    StudentUPI = s?.UPI,
                    StudentName = s?.FullName,
                    TotalInvoiced = g.Sum(i => i.TotalAmount),
                    TotalPaid = g.Sum(i => i.PaidAmount),
                    Balance = g.Sum(i => i.TotalAmount - i.PaidAmount - i.DiscountAmount)
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

        [HttpGet("studentStatement")]
        public async Task<IActionResult> StudentStatement(int studentId, int? academicYearId, int? sessionId, DateTime? from, DateTime? to)
        {
            var student = await _unitOfWork.Students.GetById(studentId);
            if (student == null) return NotFound(new { message = "Student not found." });

            var invoicesQuery = (await _unitOfWork.StudentInvoices.Find(includeProperties: "AcademicYear,Session,Items.FeeCategory"))
                .Where(i => i.StudentId == studentId);
            if (academicYearId.HasValue) invoicesQuery = invoicesQuery.Where(i => i.AcademicYearId == academicYearId.Value);
            if (sessionId.HasValue) invoicesQuery = invoicesQuery.Where(i => i.SessionId == sessionId.Value);
            if (from.HasValue) invoicesQuery = invoicesQuery.Where(i => i.InvoiceDate >= from.Value);
            if (to.HasValue) invoicesQuery = invoicesQuery.Where(i => i.InvoiceDate <= to.Value);

            var payments = (await _unitOfWork.Payments.Find(includeProperties: "StudentInvoice,BankAccount"))
                .Where(p => p.StudentId == studentId && p.ApprovalStatus == PaymentApprovalStatus.Approved).ToList();
            if (academicYearId.HasValue)
                payments = payments.Where(p => p.StudentInvoice == null || p.StudentInvoice.AcademicYearId == academicYearId.Value).ToList();
            if (sessionId.HasValue)
                payments = payments.Where(p => p.StudentInvoice == null || p.StudentInvoice.SessionId == sessionId.Value).ToList();
            if (from.HasValue) payments = payments.Where(p => p.PaymentDate >= from.Value).ToList();
            if (to.HasValue) payments = payments.Where(p => p.PaymentDate <= to.Value).ToList();

            var invoices = invoicesQuery.ToList();

            // Build transaction list: invoices = debit, payments = credit, credit notes = debit, debit notes = debit
            var transactions = new List<(DateTime Date, string Type, string Reference, string Description, decimal Debit, decimal Credit, string? CreatedBy, string? Method)>();

            foreach (var inv in invoices)
            {
                var itemDesc = string.Join(", ", inv.Items.Select(it => it.FeeCategory?.Name ?? "").Where(n => !string.IsNullOrEmpty(n)));
                transactions.Add((inv.InvoiceDate, "Invoice", inv.InvoiceNumber,
                    itemDesc.Length > 0 ? itemDesc : (inv.Description ?? ""), inv.TotalAmount, 0m, inv.CreatedBy, null));
            }

            foreach (var p in payments)
            {
                if (p.PaymentType == PaymentType.Receipt)
                    transactions.Add((p.PaymentDate, "Receipt", p.ReceiptNumber, p.Description ?? "", 0m, p.Amount, p.CreatedBy, p.PaymentMethod.ToString()));
                else if (p.PaymentType == PaymentType.CreditNote)
                    transactions.Add((p.PaymentDate, "Credit Note", p.ReceiptNumber, p.Reason ?? p.Description ?? "", p.Amount, 0m, p.CreatedBy, null));
                else if (p.PaymentType == PaymentType.DebitNote)
                    transactions.Add((p.PaymentDate, "Debit Note", p.ReceiptNumber, p.Reason ?? p.Description ?? "", p.Amount, 0m, p.CreatedBy, null));
            }

            transactions = transactions.OrderBy(t => t.Date).ThenBy(t => t.Reference).ToList();

            decimal running = 0;
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
                    method = t.Method
                };
            }).ToList();

            return Ok(new
            {
                student = new { id = student.Id, upi = student.UPI, fullName = student.FullName },
                fromDate = from,
                toDate = to,
                academicYearId,
                sessionId,
                totalInvoiced = transactions.Sum(t => t.Debit),
                totalPaid = transactions.Sum(t => t.Credit),
                balance = running,
                transactions = rows
            });
        }

        // GET api/financeReports/studentDiscounts?academicYearId=1&sessionId=2&schoolClassId=3
        [HttpGet("studentDiscounts")]
        public async Task<IActionResult> StudentDiscounts(int? academicYearId, int? sessionId, int? schoolClassId)
        {
            var invoices = (await _unitOfWork.StudentInvoices.Find(
                includeProperties: "Student,AcademicYear,Session,Items.FeeCategory")).ToList();

            if (academicYearId.HasValue)
                invoices = invoices.Where(i => i.AcademicYearId == academicYearId.Value).ToList();
            if (sessionId.HasValue)
                invoices = invoices.Where(i => i.SessionId == sessionId.Value).ToList();

            // Only keep invoices with any discounted line or header discount
            invoices = invoices.Where(i => i.DiscountAmount > 0 || i.Items.Any(it => it.Discount > 0)).ToList();

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
                    var rank = sc.SchoolClass?.LearningLevel?.Rank ?? 0;
                    studentClassMap[sc.StudentId] = (sc.SchoolClassId, name, rank);
                }
            }

            if (schoolClassId.HasValue && academicYearId.HasValue)
            {
                var classStudents = studentClassMap.Where(kv => kv.Value.ClassId == schoolClassId.Value).Select(kv => kv.Key).ToHashSet();
                invoices = invoices.Where(i => classStudents.Contains(i.StudentId)).ToList();
            }

            var rows = invoices
                .GroupBy(i => i.StudentId)
                .Select(g =>
                {
                    var s = g.First().Student;
                    var classInfo = studentClassMap.TryGetValue(g.Key, out var c) ? c : (0, "", 999);
                    var invoiceRows = g.Select(inv => new
                    {
                        invoiceId = inv.Id,
                        invoiceNumber = inv.InvoiceNumber,
                        invoiceDate = inv.InvoiceDate,
                        academicYear = inv.AcademicYear?.Name,
                        session = inv.Session?.SessionName,
                        totalAmount = inv.TotalAmount,
                        headerDiscount = inv.DiscountAmount,
                        description = inv.Description,
                        items = inv.Items.Where(it => it.Discount > 0).Select(it => new
                        {
                            feeCategory = it.FeeCategory?.Name,
                            amount = it.Amount,
                            discount = it.Discount,
                            description = it.Description
                        }).ToList(),
                        itemDiscount = inv.Items.Sum(it => it.Discount)
                    }).OrderBy(x => x.invoiceDate).ToList();

                    var totalDiscount = invoiceRows.Sum(r => r.headerDiscount + r.itemDiscount);
                    return new
                    {
                        studentId = g.Key,
                        studentUPI = s?.UPI,
                        studentName = s?.FullName,
                        className = string.IsNullOrEmpty(classInfo.Item2) ? "(No Class Assigned)" : classInfo.Item2,
                        levelRank = classInfo.Item3,
                        totalDiscount,
                        totalInvoiced = g.Sum(i => i.TotalAmount),
                        invoiceCount = g.Count(),
                        invoices = invoiceRows
                    };
                })
                .Where(r => r.totalDiscount > 0)
                .OrderBy(r => r.levelRank)
                .ThenBy(r => r.className)
                .ThenBy(r => r.studentName)
                .ToList();

            return Ok(new
            {
                academicYearId,
                sessionId,
                schoolClassId,
                studentCount = rows.Count,
                grandTotalDiscount = rows.Sum(r => r.totalDiscount),
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
                e.ReferenceNumber,
                e.ExpenseDate,
                l.ExpenseCategory,
                l.Amount,
                l.Vendor,
                l.Description
            }));
            var byCategory = lines.GroupBy(l => l.ExpenseCategory?.Name ?? "Uncategorized").Select(g => new {
                Category = g.Key,
                Amount = g.Sum(x => x.Amount),
                Count = g.Count()
            }).OrderByDescending(x => x.Amount).ToList();

            var detail = lines.Select(l => new {
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
