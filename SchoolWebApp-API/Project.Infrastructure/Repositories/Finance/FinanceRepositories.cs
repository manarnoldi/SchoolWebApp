using Microsoft.EntityFrameworkCore;
using Project.Infrastructure.Data;
using Project.Infrastructure.Repositories;
using SchoolWebApp.Core.Entities.Finance;
using SchoolWebApp.Core.Interfaces.IRepositories.Finance;

namespace SchoolWebApp.Infrastructure.Repositories.Finance
{
    public class AccountRepository : BaseRepository<Account>, IAccountRepository
    {
        public AccountRepository(ApplicationDbContext dbContext) : base(dbContext) { }

        public async Task<List<Account>> GetByType(AccountType type)
        {
            return await _dbContext.Set<Account>()
                .Where(a => a.AccountType == type && a.IsActive)
                .ToListAsync();
        }
    }

    public class FeeCategoryRepository : BaseRepository<FeeCategory>, IFeeCategoryRepository
    {
        public FeeCategoryRepository(ApplicationDbContext dbContext) : base(dbContext) { }
    }

    public class FeeStructureRepository : BaseRepository<FeeStructure>, IFeeStructureRepository
    {
        public FeeStructureRepository(ApplicationDbContext dbContext) : base(dbContext) { }

        public async Task<FeeStructure?> GetByIdWithItems(int id)
        {
            return await _dbContext.Set<FeeStructure>()
                .Include(fs => fs.AcademicYear)
                .Include(fs => fs.Session)
                .Include(fs => fs.LearningLevel)
                .Include(fs => fs.Items).ThenInclude(i => i.FeeCategory)
                .FirstOrDefaultAsync(fs => fs.Id == id);
        }
    }

    public class FeeStructureItemRepository : BaseRepository<FeeStructureItem>, IFeeStructureItemRepository
    {
        public FeeStructureItemRepository(ApplicationDbContext dbContext) : base(dbContext) { }
    }

    public class StudentInvoiceRepository : BaseRepository<StudentInvoice>, IStudentInvoiceRepository
    {
        public StudentInvoiceRepository(ApplicationDbContext dbContext) : base(dbContext) { }

        public async Task<StudentInvoice?> GetByIdWithDetails(int id)
        {
            return await _dbContext.Set<StudentInvoice>()
                .Include(i => i.Student)
                .Include(i => i.AcademicYear)
                .Include(i => i.Session)
                .Include(i => i.Items).ThenInclude(it => it.FeeCategory)
                .Include(i => i.Payments)
                .FirstOrDefaultAsync(i => i.Id == id);
        }

        public async Task<List<StudentInvoice>> GetByStudentId(int studentId)
        {
            return await _dbContext.Set<StudentInvoice>()
                .Include(i => i.AcademicYear)
                .Include(i => i.Session)
                .Where(i => i.StudentId == studentId)
                .OrderByDescending(i => i.InvoiceDate)
                .ToListAsync();
        }

        public async Task<List<StudentInvoice>> GetByAcademicYearId(int academicYearId)
        {
            return await _dbContext.Set<StudentInvoice>()
                .Include(i => i.Student)
                .Include(i => i.Session)
                .Where(i => i.AcademicYearId == academicYearId)
                .ToListAsync();
        }
    }

    public class StudentInvoiceItemRepository : BaseRepository<StudentInvoiceItem>, IStudentInvoiceItemRepository
    {
        public StudentInvoiceItemRepository(ApplicationDbContext dbContext) : base(dbContext) { }
    }

    public class PaymentRepository : BaseRepository<Payment>, IPaymentRepository
    {
        public PaymentRepository(ApplicationDbContext dbContext) : base(dbContext) { }

        public async Task<List<Payment>> GetByStudentId(int studentId)
        {
            return await _dbContext.Set<Payment>()
                .Include(p => p.Student)
                .Include(p => p.StudentInvoice)
                .Include(p => p.BankAccount)
                .Where(p => p.StudentId == studentId)
                .OrderByDescending(p => p.PaymentDate)
                .ToListAsync();
        }

        public async Task<List<Payment>> GetByInvoiceId(int invoiceId)
        {
            return await _dbContext.Set<Payment>()
                .Where(p => p.StudentInvoiceId == invoiceId)
                .OrderByDescending(p => p.PaymentDate)
                .ToListAsync();
        }

        public async Task<List<Payment>> GetByDateRange(DateTime from, DateTime to)
        {
            return await _dbContext.Set<Payment>()
                .Include(p => p.Student)
                .Include(p => p.StudentInvoice)
                .Where(p => p.PaymentDate >= from && p.PaymentDate <= to)
                .OrderBy(p => p.PaymentDate)
                .ToListAsync();
        }
    }

    public class ExpenseCategoryRepository : BaseRepository<ExpenseCategory>, IExpenseCategoryRepository
    {
        public ExpenseCategoryRepository(ApplicationDbContext dbContext) : base(dbContext) { }
    }

    public class ExpenseRepository : BaseRepository<Expense>, IExpenseRepository
    {
        public ExpenseRepository(ApplicationDbContext dbContext) : base(dbContext) { }

        public async Task<List<Expense>> GetAllWithLines()
        {
            return await _dbContext.Set<Expense>()
                .Include(e => e.PaidFromAccount)
                .Include(e => e.Lines).ThenInclude(l => l.ExpenseCategory)
                .Include(e => e.Lines).ThenInclude(l => l.BudgetLine).ThenInclude(bl => bl!.Budget)
                .Include(e => e.Lines).ThenInclude(l => l.BudgetLine).ThenInclude(bl => bl!.Account)
                .OrderByDescending(e => e.ExpenseDate)
                .ToListAsync();
        }

        public async Task<Expense?> GetByIdWithLines(int id)
        {
            return await _dbContext.Set<Expense>()
                .Include(e => e.PaidFromAccount)
                .Include(e => e.Lines).ThenInclude(l => l.ExpenseCategory)
                .Include(e => e.Lines).ThenInclude(l => l.BudgetLine).ThenInclude(bl => bl!.Budget)
                .Include(e => e.Lines).ThenInclude(l => l.BudgetLine).ThenInclude(bl => bl!.Account)
                .FirstOrDefaultAsync(e => e.Id == id);
        }

        public async Task<List<Expense>> GetByDateRange(DateTime from, DateTime to)
        {
            return await _dbContext.Set<Expense>()
                .Include(e => e.PaidFromAccount)
                .Include(e => e.Lines).ThenInclude(l => l.ExpenseCategory)
                .Where(e => e.ExpenseDate >= from && e.ExpenseDate <= to)
                .OrderBy(e => e.ExpenseDate)
                .ToListAsync();
        }
    }

    public class ExpenseLineRepository : BaseRepository<ExpenseLine>, IExpenseLineRepository
    {
        public ExpenseLineRepository(ApplicationDbContext dbContext) : base(dbContext) { }
    }

    public class JournalEntryRepository : BaseRepository<JournalEntry>, IJournalEntryRepository
    {
        public JournalEntryRepository(ApplicationDbContext dbContext) : base(dbContext) { }

        public async Task<JournalEntry?> GetByIdWithLines(int id)
        {
            return await _dbContext.Set<JournalEntry>()
                .Include(j => j.Lines).ThenInclude(l => l.Account)
                .FirstOrDefaultAsync(j => j.Id == id);
        }

        public async Task<List<JournalEntry>> GetByDateRange(DateTime from, DateTime to)
        {
            return await _dbContext.Set<JournalEntry>()
                .Include(j => j.Lines).ThenInclude(l => l.Account)
                .Where(j => j.EntryDate >= from && j.EntryDate <= to && j.IsPosted)
                .OrderBy(j => j.EntryDate)
                .ToListAsync();
        }
    }

    public class BudgetRepository : BaseRepository<Budget>, IBudgetRepository
    {
        public BudgetRepository(ApplicationDbContext dbContext) : base(dbContext) { }

        public async Task<Budget?> GetByIdWithLines(int id)
        {
            return await _dbContext.Set<Budget>()
                .Include(b => b.AcademicYear)
                .Include(b => b.Department)
                .Include(b => b.BudgetMaster)
                .Include(b => b.Lines).ThenInclude(l => l.Account)
                .FirstOrDefaultAsync(b => b.Id == id);
        }

        public async Task<List<Budget>> GetAllWithLines()
        {
            return await _dbContext.Set<Budget>()
                .Include(b => b.AcademicYear)
                .Include(b => b.Department)
                .Include(b => b.BudgetMaster)
                .Include(b => b.Lines).ThenInclude(l => l.Account)
                .OrderByDescending(b => b.StartDate)
                .ToListAsync();
        }
    }

    public class BudgetLineRepository : BaseRepository<BudgetLine>, IBudgetLineRepository
    {
        public BudgetLineRepository(ApplicationDbContext dbContext) : base(dbContext) { }
    }

    public class BudgetMasterRepository : BaseRepository<BudgetMaster>, IBudgetMasterRepository
    {
        public BudgetMasterRepository(ApplicationDbContext dbContext) : base(dbContext) { }
    }

    public class BudgetAmendmentRepository : BaseRepository<BudgetAmendment>, IBudgetAmendmentRepository
    {
        public BudgetAmendmentRepository(ApplicationDbContext dbContext) : base(dbContext) { }

        public async Task<BudgetAmendment?> GetByIdWithLines(int id)
        {
            return await _dbContext.Set<BudgetAmendment>()
                .Include(a => a.Budget)
                .Include(a => a.ApprovedBy)
                .Include(a => a.Lines).ThenInclude(l => l.Account)
                .FirstOrDefaultAsync(a => a.Id == id);
        }

        public async Task<List<BudgetAmendment>> GetByBudgetId(int budgetId)
        {
            return await _dbContext.Set<BudgetAmendment>()
                .Include(a => a.ApprovedBy)
                .Include(a => a.Lines).ThenInclude(l => l.Account)
                .Where(a => a.BudgetId == budgetId)
                .OrderByDescending(a => a.AmendmentDate)
                .ToListAsync();
        }
    }

    public class BudgetAmendmentLineRepository : BaseRepository<BudgetAmendmentLine>, IBudgetAmendmentLineRepository
    {
        public BudgetAmendmentLineRepository(ApplicationDbContext dbContext) : base(dbContext) { }
    }

    public class JournalLineRepository : BaseRepository<JournalLine>, IJournalLineRepository
    {
        public JournalLineRepository(ApplicationDbContext dbContext) : base(dbContext) { }

        public async Task<List<JournalLine>> GetByAccountIdAndDateRange(int accountId, DateTime from, DateTime to)
        {
            return await _dbContext.Set<JournalLine>()
                .Include(l => l.JournalEntry)
                .Where(l => l.AccountId == accountId
                    && l.JournalEntry != null
                    && l.JournalEntry.IsPosted
                    && l.JournalEntry.EntryDate >= from
                    && l.JournalEntry.EntryDate <= to)
                .ToListAsync();
        }

        public async Task<List<JournalLine>> GetAllWithEntryAndAccount(DateTime from, DateTime to)
        {
            return await _dbContext.Set<JournalLine>()
                .Include(l => l.JournalEntry)
                .Include(l => l.Account)
                .Where(l => l.JournalEntry != null
                    && l.JournalEntry.IsPosted
                    && l.JournalEntry.EntryDate >= from
                    && l.JournalEntry.EntryDate <= to)
                .ToListAsync();
        }
    }
}
