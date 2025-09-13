using Microsoft.EntityFrameworkCore;
using Project.Infrastructure.Data;
using Project.Infrastructure.Repositories;
using SchoolWebApp.Core.Entities.Academics;
using SchoolWebApp.Core.Entities.Class;
using SchoolWebApp.Core.Interfaces.IRepositories.Academics;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace SchoolWebApp.Infrastructure.Repositories.Academics
{
    public class GradeRepository : BaseRepository<Grade>, IGradeRepository
    {
        public GradeRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }
        public async Task<List<Grade>> GetByCurriculumId(int? curriculumId)
        {
            var query = _dbContext.Grades
                .Include(e => e.Curriculum)
                .AsQueryable();
            if (curriculumId != null)
                query = query.Where(e => e.CurriculumId == curriculumId);

            var grades = await query.ToListAsync();
            return grades;
        }

        public async Task<Grade> GetByScore(float score)
        {
            var query = await _dbContext.Grades
                .Include(e => e.Curriculum)
                .Where(g => score >= g.MinScore && score <= g.MaxScore)
                .FirstOrDefaultAsync();
            return query;
        }
    }
}
