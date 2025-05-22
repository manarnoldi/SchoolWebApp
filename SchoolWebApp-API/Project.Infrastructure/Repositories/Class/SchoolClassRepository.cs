using Microsoft.EntityFrameworkCore;
using Project.Infrastructure.Data;
using Project.Infrastructure.Repositories;
using SchoolWebApp.Core.Entities.Academics;
using SchoolWebApp.Core.Entities.Class;
using SchoolWebApp.Core.Interfaces.IRepositories.Class;

namespace SchoolWebApp.Infrastructure.Repositories.Class
{
    public class SchoolClassRepository : BaseRepository<SchoolClass>, ISchoolClassRepository
    {
        public SchoolClassRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }

        public async Task<List<SchoolClass>> GetByAcademicYearId(int academicYearId)
        {
            var schoolClasses = await _dbContext.SchoolClasses.Where(e => e.AcademicYearId == academicYearId)
                .Include(l => l.LearningLevel)
                .Include(l => l.SchoolStream)
                .Include(l => l.AcademicYear)
                .ToListAsync();
            return schoolClasses;
        }

        public async Task<List<SchoolClass>> GetByEducationLevelId(int educationLevelId)
        {
            var schoolClasses = await _dbContext.SchoolClasses.Where(s => s.LearningLevel.EducationLevelId == educationLevelId)
                .Include(l => l.LearningLevel)
                .Include(l => l.SchoolStream)
                .Include(l => l.AcademicYear)
                .ToListAsync();
            return schoolClasses;
        }

        public async Task<List<SchoolClass>> GetByEducationLevelYearId(int? educationLevelId, int? academicYearId)
        {
            var query = _dbContext.SchoolClasses
                .Include(l => l.LearningLevel)
                .Include(l => l.SchoolStream)
                .Include(l => l.AcademicYear)
                .AsQueryable();
            if (educationLevelId != null)
                query = query.Where(s => s.LearningLevel.EducationLevelId == educationLevelId);
            if (academicYearId != null)
                query = query.Where(s => s.AcademicYearId == academicYearId);

            var schoolClasses = await query.ToListAsync();
            return schoolClasses;
        }

        public async Task<SchoolClass> GetByYearClassStream(int academicYearId, int learningLevelId, int schoolStreamId)
        {
            var schoolClass = await _dbContext.SchoolClasses.Where(e => e.AcademicYearId == academicYearId &&
            e.LearningLevelId == learningLevelId && e.SchoolStreamId == schoolStreamId)
                .Include(l => l.LearningLevel)
                .Include(l => l.SchoolStream)
                .Include(l => l.AcademicYear)
                .FirstOrDefaultAsync();
            return schoolClass;
        }
    }
}
