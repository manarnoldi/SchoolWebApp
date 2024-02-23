using Microsoft.EntityFrameworkCore;
using Project.Infrastructure.Data;
using Project.Infrastructure.Repositories;
using SchoolWebApp.Core.Entities.Students;
using SchoolWebApp.Core.Interfaces.IRepositories.Students;

namespace SchoolWebApp.Infrastructure.Repositories.Students
{
    public class StudentsRepository : BaseRepository<Student>, IStudentsRepository
    {
        public StudentsRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }

        public async Task<List<Student>> GetByLearningModeId(int learningModeId)
        {
            var students = await _dbContext.Students.Where(e => e.LearningModeId == learningModeId).ToListAsync();
            return students;
        }

    }
}
