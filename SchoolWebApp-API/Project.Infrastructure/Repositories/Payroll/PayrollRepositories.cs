using Microsoft.EntityFrameworkCore;
using Project.Infrastructure.Data;
using Project.Infrastructure.Repositories;
using SchoolWebApp.Core.Entities.Payroll;
using SchoolWebApp.Core.Interfaces.IRepositories.Payroll;

namespace SchoolWebApp.Infrastructure.Repositories.Payroll
{
    public class EarningTypeRepository : BaseRepository<EarningType>, IEarningTypeRepository
    {
        public EarningTypeRepository(ApplicationDbContext db) : base(db) { }
    }

    public class DeductionTypeRepository : BaseRepository<DeductionType>, IDeductionTypeRepository
    {
        public DeductionTypeRepository(ApplicationDbContext db) : base(db) { }
    }

    public class TaxBandRepository : BaseRepository<TaxBand>, ITaxBandRepository
    {
        public TaxBandRepository(ApplicationDbContext db) : base(db) { }
    }

    public class PayrollSettingRepository : BaseRepository<PayrollSetting>, IPayrollSettingRepository
    {
        public PayrollSettingRepository(ApplicationDbContext db) : base(db) { }
    }

    public class EmployeeSalaryRepository : BaseRepository<EmployeeSalary>, IEmployeeSalaryRepository
    {
        public EmployeeSalaryRepository(ApplicationDbContext db) : base(db) { }

        public async Task<EmployeeSalary?> GetActiveByStaffId(int staffId)
        {
            return await _dbContext.Set<EmployeeSalary>()
                .Include(s => s.Items).ThenInclude(i => i.EarningType)
                .Include(s => s.Items).ThenInclude(i => i.DeductionType)
                .Where(s => s.StaffDetailsId == staffId && s.IsActive)
                .OrderByDescending(s => s.EffectiveDate)
                .FirstOrDefaultAsync();
        }

        public async Task<List<EmployeeSalary>> GetAllWithStaff()
        {
            return await _dbContext.Set<EmployeeSalary>()
                .Include(s => s.StaffDetails)
                .Include(s => s.Items).ThenInclude(i => i.EarningType)
                .Include(s => s.Items).ThenInclude(i => i.DeductionType)
                .OrderByDescending(s => s.EffectiveDate)
                .ToListAsync();
        }
    }

    public class EmployeeSalaryItemRepository : BaseRepository<EmployeeSalaryItem>, IEmployeeSalaryItemRepository
    {
        public EmployeeSalaryItemRepository(ApplicationDbContext db) : base(db) { }
    }

    public class LoanAdvanceRepository : BaseRepository<LoanAdvance>, ILoanAdvanceRepository
    {
        public LoanAdvanceRepository(ApplicationDbContext db) : base(db) { }

        public async Task<List<LoanAdvance>> GetActiveByStaffId(int staffId)
        {
            return await _dbContext.Set<LoanAdvance>()
                .Where(l => l.StaffDetailsId == staffId && l.Status == LoanStatus.Active && l.Balance > 0)
                .ToListAsync();
        }
    }

    public class PayrollPeriodRepository : BaseRepository<PayrollPeriod>, IPayrollPeriodRepository
    {
        public PayrollPeriodRepository(ApplicationDbContext db) : base(db) { }

        public async Task<PayrollPeriod?> GetByIdWithPayslips(int id)
        {
            return await _dbContext.Set<PayrollPeriod>()
                .Include(p => p.Payslips).ThenInclude(ps => ps.StaffDetails).ThenInclude(s => s!.Designation)
                .Include(p => p.Payslips).ThenInclude(ps => ps.Earnings).ThenInclude(e => e.EarningType)
                .Include(p => p.Payslips).ThenInclude(ps => ps.Deductions).ThenInclude(d => d.DeductionType)
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<List<PayrollPeriod>> GetAllWithPayslips()
        {
            return await _dbContext.Set<PayrollPeriod>()
                .Include(p => p.Payslips)
                .OrderByDescending(p => p.Year).ThenByDescending(p => p.Month)
                .ToListAsync();
        }
    }

    public class PayslipRepository : BaseRepository<Payslip>, IPayslipRepository
    {
        public PayslipRepository(ApplicationDbContext db) : base(db) { }

        public async Task<List<Payslip>> GetByPeriodId(int periodId)
        {
            return await _dbContext.Set<Payslip>()
                .Include(p => p.StaffDetails).ThenInclude(s => s!.Designation)
                .Include(p => p.Earnings).ThenInclude(e => e.EarningType)
                .Include(p => p.Deductions).ThenInclude(d => d.DeductionType)
                .Where(p => p.PayrollPeriodId == periodId)
                .OrderBy(p => p.StaffDetails!.FullName)
                .ToListAsync();
        }

        public async Task<Payslip?> GetByIdFull(int id)
        {
            return await _dbContext.Set<Payslip>()
                .Include(p => p.StaffDetails).ThenInclude(s => s!.Designation)
                .Include(p => p.PayrollPeriod)
                .Include(p => p.Earnings).ThenInclude(e => e.EarningType)
                .Include(p => p.Deductions).ThenInclude(d => d.DeductionType)
                .FirstOrDefaultAsync(p => p.Id == id);
        }
    }

    public class PayslipEarningRepository : BaseRepository<PayslipEarning>, IPayslipEarningRepository
    {
        public PayslipEarningRepository(ApplicationDbContext db) : base(db) { }
    }

    public class PayslipDeductionRepository : BaseRepository<PayslipDeduction>, IPayslipDeductionRepository
    {
        public PayslipDeductionRepository(ApplicationDbContext db) : base(db) { }
    }
}
