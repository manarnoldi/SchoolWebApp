using SchoolWebApp.Core.Entities.Finance;

namespace SchoolWebApp.Core.DTOs.Finance.Reports
{
    public class TrialBalanceRowDto
    {
        public int AccountId { get; set; }
        public string? AccountCode { get; set; }
        public string? AccountName { get; set; }
        public AccountType AccountType { get; set; }
        public decimal Debit { get; set; }
        public decimal Credit { get; set; }
    }

    public class IncomeStatementLineDto
    {
        public int AccountId { get; set; }
        public string? AccountCode { get; set; }
        public string? AccountName { get; set; }
        public decimal Amount { get; set; }
    }

    public class IncomeStatementDto
    {
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public List<IncomeStatementLineDto> Income { get; set; } = new();
        public List<IncomeStatementLineDto> Expenses { get; set; } = new();
        public decimal TotalIncome { get; set; }
        public decimal TotalExpenses { get; set; }
        public decimal NetProfit { get; set; }
    }

    public class BalanceSheetLineDto
    {
        public int AccountId { get; set; }
        public string? AccountCode { get; set; }
        public string? AccountName { get; set; }
        public decimal Amount { get; set; }
    }

    public class BalanceSheetDto
    {
        public DateTime AsOfDate { get; set; }
        public List<BalanceSheetLineDto> Assets { get; set; } = new();
        public List<BalanceSheetLineDto> Liabilities { get; set; } = new();
        public List<BalanceSheetLineDto> Equity { get; set; } = new();
        public decimal TotalAssets { get; set; }
        public decimal TotalLiabilities { get; set; }
        public decimal TotalEquity { get; set; }
        public decimal RetainedEarnings { get; set; }
    }

    public class FeeCollectionSummaryDto
    {
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public decimal TotalInvoiced { get; set; }
        public decimal TotalCollected { get; set; }
        public decimal TotalOutstanding { get; set; }
        public int InvoiceCount { get; set; }
        public int PaidCount { get; set; }
    }

    public class OutstandingBalanceRowDto
    {
        public int StudentId { get; set; }
        public string? StudentUPI { get; set; }
        public string? StudentName { get; set; }
        public string? ClassName { get; set; }
        public decimal TotalInvoiced { get; set; }
        public decimal TotalPaid { get; set; }
        public decimal Balance { get; set; }
    }

    public class ConsolidatedBudgetLineDto
    {
        public int AccountId { get; set; }
        public string? AccountCode { get; set; }
        public string? AccountName { get; set; }
        public string? AccountType { get; set; }
        public decimal Budgeted { get; set; }
        public decimal Actual { get; set; }
        public decimal Variance { get; set; }
    }

    public class ConsolidatedBudgetDepartmentDto
    {
        public int DepartmentId { get; set; }
        public string? DepartmentName { get; set; }
        public int BudgetCount { get; set; }
        public decimal TotalBudgetedIncome { get; set; }
        public decimal TotalBudgetedExpense { get; set; }
        public decimal TotalActualIncome { get; set; }
        public decimal TotalActualExpense { get; set; }
        public List<ConsolidatedBudgetLineDto> Lines { get; set; } = new();
        public List<ConsolidatedBudgetEntryDto> Budgets { get; set; } = new();
    }

    public class ConsolidatedBudgetEntryDto
    {
        public int BudgetId { get; set; }
        public string? Name { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public decimal TotalBudgeted { get; set; }
    }

    public class ConsolidatedBudgetDto
    {
        public int AcademicYearId { get; set; }
        public string? AcademicYearName { get; set; }
        public decimal GrandBudgetedIncome { get; set; }
        public decimal GrandBudgetedExpense { get; set; }
        public decimal GrandActualIncome { get; set; }
        public decimal GrandActualExpense { get; set; }
        public decimal NetBudgeted => GrandBudgetedIncome - GrandBudgetedExpense;
        public decimal NetActual => GrandActualIncome - GrandActualExpense;
        public List<ConsolidatedBudgetDepartmentDto> Departments { get; set; } = new();
    }
}
