using Microsoft.EntityFrameworkCore;
using Project.Infrastructure.Data;
using Project.Infrastructure.Repositories;
using SchoolWebApp.Core.Entities.School;
using SchoolWebApp.Core.Interfaces.IRepositories.School;

namespace SchoolWebApp.Infrastructure.Repositories.School
{
    public class EducationLevelRepository : BaseRepository<EducationLevel>, IEducationLevelRepository
    {
        public EducationLevelRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }
        public async Task<List<EducationLevel>> GetByCurriculumId(int? curriculumId)
        {
            var query = _dbContext.EducationLevels
                .Include(e => e.EducationLevelType)
                .Include(e => e.Curriculum)
                .AsQueryable();

            if (curriculumId != null)
                query = query.Where(e => e.CurriculumId == curriculumId);

            var educationLevels = await query.ToListAsync();
            return educationLevels;
        }
    }
}
