using Microsoft.EntityFrameworkCore;
using Project.Infrastructure.Data;
using Project.Infrastructure.Repositories;
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

        public async Task<List<SchoolClass>> GetByEducationLevelYearId(int educationLevelId, int academicYearId)
        {
            var schoolClasses = await _dbContext.SchoolClasses.Where(s => s.LearningLevel.EducationLevelId == educationLevelId
            && s.AcademicYearId == academicYearId)
                .Include(l => l.LearningLevel)
                .Include(l => l.SchoolStream)
                .Include(l => l.AcademicYear)
                .ToListAsync();
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
