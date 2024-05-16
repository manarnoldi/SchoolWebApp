using Microsoft.EntityFrameworkCore;
using Project.Infrastructure.Data;
using Project.Infrastructure.Repositories;
using SchoolWebApp.Core.Entities.Academics;
using SchoolWebApp.Core.Interfaces.IRepositories.Academics;

namespace SchoolWebApp.Infrastructure.Repositories.Academics
{
    public class SubjectGroupRepository : BaseRepository<SubjectGroup>, ISubjectGroupRepository
    {
        public SubjectGroupRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }

        public async Task<List<SubjectGroup>> GetByCurriculumId(int curriculumId)
        {
            var grades = await _dbContext.SubjectGroups.Where(e => e.CurriculumId == curriculumId).ToListAsync();
            return grades;
        }
    }
}
