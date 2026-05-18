using AutoMapper;
using SchoolWebApp.Core.DTOs.Payroll;
using SchoolWebApp.Core.Entities.Payroll;

namespace SchoolWebApp.Core.Profiles.Payroll
{
    public class PayrollProfile : Profile
    {
        public PayrollProfile()
        {
            CreateMap<EarningType, EarningTypeDto>();
            CreateMap<CreateEarningTypeDto, EarningType>();

            CreateMap<DeductionType, DeductionTypeDto>();
            CreateMap<CreateDeductionTypeDto, DeductionType>();

            CreateMap<TaxBand, TaxBandDto>();
            CreateMap<CreateTaxBandDto, TaxBand>();

            CreateMap<PayrollSetting, PayrollSettingDto>();
            CreateMap<CreatePayrollSettingDto, PayrollSetting>();

            CreateMap<LoanAdvance, LoanAdvanceDto>()
                .ForMember(d => d.StaffName, o => o.MapFrom(s => s.StaffDetails != null ? s.StaffDetails.FullName : null))
                .ForMember(d => d.Status, o => o.MapFrom(s => (int)s.Status));
            CreateMap<CreateLoanAdvanceDto, LoanAdvance>()
                .ForMember(d => d.Status, o => o.MapFrom(s => (LoanStatus)s.Status));

            CreateMap<EmployeeSalary, EmployeeSalaryDto>()
                .ForMember(d => d.StaffName, o => o.MapFrom(s => s.StaffDetails != null ? s.StaffDetails.FullName : null))
                .ForMember(d => d.StaffUpi, o => o.MapFrom(s => s.StaffDetails != null ? s.StaffDetails.UPI : null))
                .ForMember(d => d.TotalEarnings, o => o.MapFrom(s =>
                    s.BasicSalary + s.HouseAllowance + s.TransportAllowance + s.OtherAllowances
                    + s.Items.Where(i => i.EarningTypeId != null).Sum(i => i.Amount)));
            CreateMap<CreateEmployeeSalaryDto, EmployeeSalary>();

            CreateMap<EmployeeSalaryItem, EmployeeSalaryItemDto>()
                .ForMember(d => d.EarningTypeName, o => o.MapFrom(s => s.EarningType != null ? s.EarningType.Name : null))
                .ForMember(d => d.DeductionTypeName, o => o.MapFrom(s => s.DeductionType != null ? s.DeductionType.Name : null));
            CreateMap<EmployeeSalaryItemDto, EmployeeSalaryItem>();

            CreateMap<PayrollPeriod, PayrollPeriodDto>()
                .ForMember(d => d.Status, o => o.MapFrom(s => (int)s.Status))
                .ForMember(d => d.StatusLabel, o => o.MapFrom(s => s.Status.ToString()))
                .ForMember(d => d.PayslipCount, o => o.MapFrom(s => s.Payslips.Count))
                .ForMember(d => d.TotalGross, o => o.MapFrom(s => s.Payslips.Sum(p => p.GrossPay)))
                .ForMember(d => d.TotalNet, o => o.MapFrom(s => s.Payslips.Sum(p => p.NetPay)))
                .ForMember(d => d.TotalPaye, o => o.MapFrom(s => s.Payslips.Sum(p => p.Paye)))
                .ForMember(d => d.TotalNssf, o => o.MapFrom(s => s.Payslips.Sum(p => p.NssfEmployee)))
                .ForMember(d => d.TotalShif, o => o.MapFrom(s => s.Payslips.Sum(p => p.Shif)));
            CreateMap<CreatePayrollPeriodDto, PayrollPeriod>();

            CreateMap<Payslip, PayslipDto>()
                .ForMember(d => d.StaffName, o => o.MapFrom(s => s.StaffDetails != null ? s.StaffDetails.FullName : null))
                .ForMember(d => d.StaffUpi, o => o.MapFrom(s => s.StaffDetails != null ? s.StaffDetails.UPI : null))
                .ForMember(d => d.KraPin, o => o.MapFrom(s => s.StaffDetails != null ? s.StaffDetails.KraPinNo : null))
                .ForMember(d => d.NssfNumber, o => o.MapFrom(s => s.StaffDetails != null ? s.StaffDetails.NssfNo : null))
                .ForMember(d => d.DesignationName, o => o.MapFrom(s =>
                    s.StaffDetails != null && s.StaffDetails.Designation != null ? s.StaffDetails.Designation.Name : null))
                .ForMember(d => d.Earnings, o => o.MapFrom(s => s.Earnings))
                .ForMember(d => d.Deductions, o => o.MapFrom(s => s.Deductions));

            CreateMap<PayslipEarning, PayslipLineDto>()
                .ForMember(d => d.Name, o => o.MapFrom(s => s.EarningType != null ? s.EarningType.Name : null))
                .ForMember(d => d.Code, o => o.MapFrom(s => s.EarningType != null ? s.EarningType.Code : null));

            CreateMap<PayslipDeduction, PayslipLineDto>()
                .ForMember(d => d.Name, o => o.MapFrom(s => s.DeductionType != null ? s.DeductionType.Name : null))
                .ForMember(d => d.Code, o => o.MapFrom(s => s.DeductionType != null ? s.DeductionType.Code : null));
        }
    }
}
