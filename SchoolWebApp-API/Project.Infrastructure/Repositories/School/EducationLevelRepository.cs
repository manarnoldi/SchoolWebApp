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
        public async Task<List<EducationLevel>> GetByCurriculumId(int curriculumId)
        {
            var educationLevels = await _dbContext.EducationLevels.Where(e => e.CurriculumId == curriculumId).ToListAsync();
            return educationLevels;
        }
    }
}
