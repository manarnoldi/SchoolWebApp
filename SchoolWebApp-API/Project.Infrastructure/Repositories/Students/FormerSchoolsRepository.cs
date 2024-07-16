using Microsoft.EntityFrameworkCore;
using Project.Infrastructure.Data;
using Project.Infrastructure.Repositories;
using SchoolWebApp.Core.Entities.Students;
using SchoolWebApp.Core.Interfaces.IRepositories.Students;

namespace SchoolWebApp.Infrastructure.Repositories.Students
{
    public class FormerSchoolsRepository : BaseRepository<FormerSchool>, IFormerSchoolsRepository
    {
        public FormerSchoolsRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }
        public async Task<List<FormerSchool>> GetByStudentId(int studentId)
        {
            var studentFormerSchools = await _dbContext.FormerSchools
                .Where(e => e.StudentId == studentId)
                .Include(f=>f.Student)
                .Include(f=>f.Curriculum)
                .Include(f=>f.EducationLevel)
                .ToListAsync();
            return studentFormerSchools;
        }
    }
}
