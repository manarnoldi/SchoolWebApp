using Microsoft.EntityFrameworkCore;
using Project.Infrastructure.Data;
using Project.Infrastructure.Repositories;
using SchoolWebApp.Core.Entities.Academics;
using SchoolWebApp.Core.Entities.Staff;
using SchoolWebApp.Core.Interfaces.IRepositories.Staff;

namespace SchoolWebApp.Infrastructure.Repositories.Staff
{
    public class StaffDetailsRepository : BaseRepository<StaffDetails>, IStaffDetailsRepository
    {
        public StaffDetailsRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }

        public async Task<List<StaffDetails>> GetByEmploymentTypeId(int employmentTypeId)
        {
            var staff = await _dbContext.StaffDetails.Where(e => e.EmploymentTypeId == employmentTypeId && e.Status == 0).ToListAsync();
            return staff;
        }

        public async Task<List<StaffDetails>> GetByStaffCategoryId(int staffCategoryId)
        {
            var staff = await _dbContext.StaffDetails.Where(e => e.StaffCategoryId == staffCategoryId && e.Status == 0).ToListAsync();
            return staff;
        }

        public async Task<List<StaffDetails>> SearchForStaff(int? staffCategoryId, int? employmentTypeId)
        {
            var query = _dbContext.StaffDetails
                .Include(e => e.StaffCategory)
                .Include(e => e.EmploymentType)
                .AsQueryable();

            if (staffCategoryId != null)
                query = query.Where(s => s.StaffCategoryId == staffCategoryId);
            if (employmentTypeId != null)
                query = query.Where(s => s.EmploymentTypeId == employmentTypeId);

            var staffs = await query.ToListAsync();
            return staffs;
        }
    }
}
