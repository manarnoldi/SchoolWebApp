using Microsoft.EntityFrameworkCore;
using Project.Infrastructure.Data;
using Project.Infrastructure.Repositories;
using SchoolWebApp.Core.Entities.Staff;
using SchoolWebApp.Core.Interfaces.IRepositories.Staff;

namespace SchoolWebApp.Infrastructure.Repositories.Staff
{
    public class StaffSubjectRepository : BaseRepository<StaffSubject>, IStaffSubjectRepository
    {
        public StaffSubjectRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }

        public async Task<List<StaffSubject>> GetByAcademicYearId(int academicYearId)
        {
            var staffSubjects = await _dbContext
                .StaffSubjects
                .Where(e => e.SchoolClass.AcademicYearId == academicYearId)                
                .Include(s => s.Subject)
                .Include(s => s.SchoolClass)
                .Include(s => s.StaffDetails)
                .ToListAsync();
            return staffSubjects;
        }
        public async Task<List<StaffSubject>> GetBySchoolClassId(int schoolClassId)
        {
            var staffSubjects = await _dbContext.StaffSubjects
                .Where(e => e.SchoolClassId == schoolClassId)
                .Include(s => s.Subject)
                .Include(s => s.SchoolClass)
                .Include(s => s.StaffDetails)
                .ToListAsync();
            return staffSubjects;
        }

        public async Task<List<StaffSubject>> GetByStaffDetailsId(int staffDetailsId)
        {
            var staffSubjects = await _dbContext.StaffSubjects
                .Where(e => e.StaffDetailsId == staffDetailsId)
                .Include(s => s.Subject)
                .Include(s => s.SchoolClass)
                .Include(s => s.StaffDetails)
                .ToListAsync();
            return staffSubjects;
        }

        public async Task<List<StaffSubject>> GetByStaffYearId(int staffDetailsId, int academicYearId)
        {
            var staffSubjects = await _dbContext.StaffSubjects
                .Where(e => e.StaffDetailsId == staffDetailsId && e.SchoolClass.AcademicYearId == academicYearId)
                .Include(s => s.Subject)
                .Include(s => s.SchoolClass)
                .Include(s => s.StaffDetails)
                .ToListAsync();
            return staffSubjects;
        }
    }
}
