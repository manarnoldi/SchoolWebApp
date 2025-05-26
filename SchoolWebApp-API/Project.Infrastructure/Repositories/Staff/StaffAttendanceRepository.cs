using Microsoft.EntityFrameworkCore;
using Project.Infrastructure.Data;
using Project.Infrastructure.Repositories;
using SchoolWebApp.Core.Entities.Staff;
using SchoolWebApp.Core.Entities.Students;
using SchoolWebApp.Core.Interfaces.IRepositories.Staff;

namespace SchoolWebApp.Infrastructure.Repositories.Staff
{
    public class StaffAttendanceRepository : BaseRepository<StaffAttendance>, IStaffAttendanceRepository
    {
        public StaffAttendanceRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }

        public async Task<List<StaffAttendance>> GetByStaffDetailsId(int staffDetailsId)
        {
            var staffAttendances = await _dbContext.StaffAttendances
                .Where(e => e.StaffDetailsId == staffDetailsId)
                .Include(s => s.StaffDetails)
                .OrderBy(s => s.Date)
                .ToListAsync();
            return staffAttendances;
        }

        public async Task<StaffAttendance> GetByStaffAttendanceDate(int staffId, DateOnly attendanceDate)
        {
            var staffAttendance = await _dbContext.StaffAttendances
                .Include(s => s.StaffDetails)
                .Where(s => s.StaffDetailsId == staffId && DateOnly.FromDateTime(s.Date) == attendanceDate)
                .FirstOrDefaultAsync();

            return staffAttendance;
        }

        public async Task<List<StaffAttendance>> GetByMonthYearStaffId(int month, int year, int staffId)
        {
            var staffAttendances = await _dbContext.StaffAttendances
                .Where(a => a.StaffDetailsId == staffId && a.Date.Month == month && a.Date.Year == year)
                .Include(s => s.StaffDetails)
                .ToListAsync();
            return staffAttendances;
        }

        public async Task<List<int>> GetDistinctMonths()
        {
            var months = await _dbContext.StaffAttendances
                .Include(s => s.StaffDetails)
                .Select(s => s.Date.Month)
                .Distinct()
                .OrderBy(m => m)
                .ToListAsync();
            return months;
        }

        public async Task<List<int>> GetDistinctYears()
        {
            var years = await _dbContext.StaffAttendances
                .Include(s => s.StaffDetails)
                .Select(s => s.Date.Year)
                .Distinct()
                .OrderByDescending(m => m)
                .ToListAsync();
            return years;
        }
    }
}
