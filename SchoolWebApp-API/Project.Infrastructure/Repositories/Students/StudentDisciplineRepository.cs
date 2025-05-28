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
            var studentDisciplines = await _dbContext.StudentDisciplines.Where(e => e.StudentId == studentId)
                .Include(s => s.Student)
                .Include(s => s.Outcome)
                .Include(s => s.OccurenceType)
                .ToListAsync();
            return studentDisciplines;
        }

        public async Task<List<StudentDiscipline>> GetByStudentDateFromandDateTo(int studentId, DateOnly dateFrom, DateOnly dateTo)
        {
            var studentDisciplines = await _dbContext.StudentDisciplines
                .Where(s => s.StudentId == studentId && DateOnly.FromDateTime(s.OccurenceStartDate) >= dateFrom &&
                DateOnly.FromDateTime(s.OccurenceStartDate) <= dateTo)
                .Include(s => s.Student)
                .Include(s => s.Outcome)
                .Include(s => s.OccurenceType)
                .ToListAsync();

            return studentDisciplines;
        }
    }
}
