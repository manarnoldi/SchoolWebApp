using Microsoft.EntityFrameworkCore;
using Project.Infrastructure.Data;
using Project.Infrastructure.Repositories;
using SchoolWebApp.Core.Entities.Staff;
using SchoolWebApp.Core.Interfaces.IRepositories.Staff;

namespace SchoolWebApp.Infrastructure.Repositories.Staff
{
    public class StaffDisciplineRepository : BaseRepository<StaffDiscipline>, IStaffDisciplineRepository
    {
        public StaffDisciplineRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }

        public async Task<List<StaffDiscipline>> GetByStaffDetailsId(int staffDetailsId)
        {
            var staffDisciplines = await _dbContext.StaffDisciplines
                .Where(e => e.StaffDetailsId == staffDetailsId)
                .Include(s => s.StaffDetails)
                .Include(s => s.Outcome)
                .Include(s => s.OccurenceType)
                .ToListAsync();
            return staffDisciplines;
        }

        public async Task<List<StaffDiscipline>> GetByStaffDateFromandDateTo(int staffId, DateOnly dateFrom, DateOnly dateTo)
        {
            var staffDisciplines = await _dbContext.StaffDisciplines
                .Where(s => s.StaffDetailsId == staffId && DateOnly.FromDateTime(s.OccurenceStartDate) >= dateFrom &&
                DateOnly.FromDateTime(s.OccurenceStartDate) <= dateTo)
                .Include(s => s.StaffDetails)
                .Include(s => s.Outcome)
                .Include(s => s.OccurenceType)
                .ToListAsync();

            return staffDisciplines;
        }
    }
}
