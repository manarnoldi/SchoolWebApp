using Microsoft.EntityFrameworkCore;
using Project.Infrastructure.Data;
using Project.Infrastructure.Repositories;
using SchoolWebApp.Core.Entities.Students;
using SchoolWebApp.Core.Interfaces.IRepositories.Students;

namespace SchoolWebApp.Infrastructure.Repositories.Students
{
    public class StudentClassRepository : BaseRepository<StudentClass>, IStudentClassRepository
    {
        public StudentClassRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }

        public async Task<List<StudentClass>> GetByStudentId(int studentId)
        {
            var studentClasses = await _dbContext.StudentClasses
                .Where(e => e.StudentId == studentId)
                .Include(f => f.Student)
                .Include(f => f.SchoolClass)
                .ToListAsync();
            return studentClasses;
        }
    }
}