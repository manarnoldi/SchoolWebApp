using Project.Core.Interfaces.IRepositories;
using SchoolWebApp.Core.Entities.Payroll;

namespace SchoolWebApp.Core.Interfaces.IRepositories.Payroll
{
    public interface IEarningTypeRepository : IBaseRepository<EarningType> { }
    public interface IDeductionTypeRepository : IBaseRepository<DeductionType> { }
    public interface ITaxBandRepository : IBaseRepository<TaxBand> { }
    public interface IPayrollSettingRepository : IBaseRepository<PayrollSetting> { }
    public interface IEmployeeSalaryRepository : IBaseRepository<EmployeeSalary>
    {
        Task<EmployeeSalary?> GetActiveByStaffId(int staffId);
        Task<List<EmployeeSalary>> GetAllWithStaff();
    }
    public interface IEmployeeSalaryItemRepository : IBaseRepository<EmployeeSalaryItem> { }
    public interface ILoanAdvanceRepository : IBaseRepository<LoanAdvance>
    {
        Task<List<LoanAdvance>> GetActiveByStaffId(int staffId);
    }
    public interface IPayrollPeriodRepository : IBaseRepository<PayrollPeriod>
    {
        Task<PayrollPeriod?> GetByIdWithPayslips(int id);
        Task<List<PayrollPeriod>> GetAllWithPayslips();
    }
    public interface IPayslipRepository : IBaseRepository<Payslip>
    {
        Task<List<Payslip>> GetByPeriodId(int periodId);
        Task<Payslip?> GetByIdFull(int id);
    }
    public interface IPayslipEarningRepository : IBaseRepository<PayslipEarning> { }
    public interface IPayslipDeductionRepository : IBaseRepository<PayslipDeduction> { }
}
