using Project.Core.Interfaces.IRepositories;
using SchoolWebApp.Core.DTOs.Reports.Staff;
using SchoolWebApp.Core.Entities.Staff;

namespace SchoolWebApp.Core.Interfaces.IRepositories.Staff
{
    public interface IStaffAttendanceRepository : IBaseRepository<StaffAttendance>
    {
        Task<List<StaffAttendance>> GetByStaffDetailsId(int staffDetailsId);
        Task<StaffAttendance> GetByStaffAttendanceDate(int staffId, DateOnly attendanceDate);
        Task<List<StaffAttendance>> GetByMonthYearStaffId(int month, int year, int staffId);
        Task<List<int>> GetDistinctMonths();
        Task<List<int>> GetDistinctYears();
        Task<List<StaffAttendanceReportDto>> GetStaffAttendanceReport(int month, int year, int staffCategoryId);
    }
}
