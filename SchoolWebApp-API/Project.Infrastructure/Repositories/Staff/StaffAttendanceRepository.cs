using Microsoft.EntityFrameworkCore;
using Project.Infrastructure.Data;
using Project.Infrastructure.Repositories;
using SchoolWebApp.Core.Entities.Academics;
using SchoolWebApp.Core.Entities.Staff;
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
            var staffAttendances = await _dbContext.StaffAttendances.Where(e => e.StaffDetailsId == staffDetailsId).ToListAsync();
            return staffAttendances;
        }
    }
}
