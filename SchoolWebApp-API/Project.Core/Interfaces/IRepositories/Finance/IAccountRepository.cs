using Project.Core.Interfaces.IRepositories;
using SchoolWebApp.Core.Entities.Finance;

namespace SchoolWebApp.Core.Interfaces.IRepositories.Finance
{
    public interface IAccountRepository : IBaseRepository<Account>
    {
        Task<List<Account>> GetByType(AccountType type);
    }

    public interface IFeeCategoryRepository : IBaseRepository<FeeCategory> { }

    public interface IFeeStructureRepository : IBaseRepository<FeeStructure>
    {
        Task<FeeStructure?> GetByIdWithItems(int id);
    }

    public interface IFeeStructureItemRepository : IBaseRepository<FeeStructureItem> { }

    public interface IStudentInvoiceRepository : IBaseRepository<StudentInvoice>
    {
        Task<StudentInvoice?> GetByIdWithDetails(int id);
        Task<List<StudentInvoice>> GetByStudentId(int studentId);
        Task<List<StudentInvoice>> GetByAcademicYearId(int academicYearId);
    }

    public interface IStudentInvoiceItemRepository : IBaseRepository<StudentInvoiceItem> { }

    public interface IPaymentRepository : IBaseRepository<Payment>
    {
        Task<List<Payment>> GetByStudentId(int studentId);
        Task<List<Payment>> GetByInvoiceId(int invoiceId);
        Task<List<Payment>> GetByDateRange(DateTime from, DateTime to);
    }

    public interface IExpenseCategoryRepository : IBaseRepository<ExpenseCategory> { }

    public interface IExpenseRepository : IBaseRepository<Expense>
    {
        Task<List<Expense>> GetByDateRange(DateTime from, DateTime to);
        Task<List<Expense>> GetAllWithLines();
        Task<Expense?> GetByIdWithLines(int id);
    }

    public interface IExpenseLineRepository : IBaseRepository<ExpenseLine> { }

    public interface IJournalEntryRepository : IBaseRepository<JournalEntry>
    {
        Task<JournalEntry?> GetByIdWithLines(int id);
        Task<List<JournalEntry>> GetByDateRange(DateTime from, DateTime to);
    }

    public interface IJournalLineRepository : IBaseRepository<JournalLine>
    {
        Task<List<JournalLine>> GetByAccountIdAndDateRange(int accountId, DateTime from, DateTime to);
        Task<List<JournalLine>> GetAllWithEntryAndAccount(DateTime from, DateTime to);
    }

    public interface IBudgetRepository : IBaseRepository<Budget>
    {
        Task<Budget?> GetByIdWithLines(int id);
        Task<List<Budget>> GetAllWithLines();
    }

    public interface IBudgetLineRepository : IBaseRepository<BudgetLine> { }

    public interface IBudgetMasterRepository : IBaseRepository<BudgetMaster> { }

    public interface IBudgetAmendmentRepository : IBaseRepository<BudgetAmendment>
    {
        Task<BudgetAmendment?> GetByIdWithLines(int id);
        Task<List<BudgetAmendment>> GetByBudgetId(int budgetId);
    }

    public interface IBudgetAmendmentLineRepository : IBaseRepository<BudgetAmendmentLine> { }
}
