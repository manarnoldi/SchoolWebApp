using Microsoft.EntityFrameworkCore;
using Project.Infrastructure.Data;
using Project.Infrastructure.Repositories;
using SchoolWebApp.Core.Entities.Class;
using SchoolWebApp.Core.Entities.School;
using SchoolWebApp.Core.Interfaces.IRepositories.Class;

namespace SchoolWebApp.Infrastructure.Repositories.Class
{
    public class LearningLevelRepository : BaseRepository<LearningLevel>, ILearningLevelRepository
    {
        public LearningLevelRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }

        public async Task<List<LearningLevel>> GetByCurriculumId(int? curriculumId)
        {
            var query = _dbContext.LearningLevels
                .Include(e => e.EducationLevel)
                .AsQueryable();

            if (curriculumId != null)
                query = query.Where(e => e.EducationLevel.CurriculumId == curriculumId);

            var learningLevels = await query.ToListAsync();
            return learningLevels;
        }
    }
}
