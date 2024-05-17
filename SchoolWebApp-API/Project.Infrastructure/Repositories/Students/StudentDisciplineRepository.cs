using Microsoft.EntityFrameworkCore;
using Project.Infrastructure.Data;
using Project.Infrastructure.Repositories;
using SchoolWebApp.Core.Entities.Staff;
using SchoolWebApp.Core.Entities.Students;
using SchoolWebApp.Core.Interfaces.IRepositories.Students;

namespace SchoolWebApp.Infrastructure.Repositories.Students
{
    public class StudentDisciplineRepository : BaseRepository<StudentDiscipline>, IStudentDisciplineRepository
    {
        public StudentDisciplineRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }

        public async Task<List<StudentDiscipline>> GetByStudentId(int studentId)
        {
            var studentDisciplines = await _dbContext.StudentDisciplines.Where(e => e.StudentId == studentId).ToListAsync();
            return studentDisciplines;
        }
    }
}
