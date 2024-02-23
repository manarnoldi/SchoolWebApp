using Microsoft.EntityFrameworkCore;
using Project.Infrastructure.Data;
using Project.Infrastructure.Repositories;
using SchoolWebApp.Core.Entities.Academics;
using SchoolWebApp.Core.Interfaces.IRepositories.Academics;

namespace SchoolWebApp.Infrastructure.Repositories.Academics
{
    public class GradeRepository : BaseRepository<Grade>, IGradeRepository
    {
        public GradeRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }

        public async Task<List<Grade>> GetByCurriculumId(int curriculumId)
        {
            var events = await _dbContext.Grades.Where(e => e.CurriculumId == curriculumId).ToListAsync();
            return events;
        }
    }
}
