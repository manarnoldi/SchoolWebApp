using Microsoft.EntityFrameworkCore;
using Project.Infrastructure.Data;
using Project.Infrastructure.Repositories;
using SchoolWebApp.Core.Entities.Students;
using SchoolWebApp.Core.Interfaces.IRepositories.Students;

namespace SchoolWebApp.Infrastructure.Repositories.Students
{
    public class StudentAttendanceRepository : BaseRepository<StudentAttendance>, IStudentAttendanceRepository
    {
        public StudentAttendanceRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }

        public async Task<List<StudentAttendance>> GetByStudentClassId(int studentClassId)
        {
            var staffAttendances = await _dbContext.StudentAttendances
                .Include(s => s.StudentClass)
                .Where(e => e.StudentClassId == studentClassId).ToListAsync();
            return staffAttendances;
        }

        public async Task<List<StudentAttendance>> GetByStudentId(int studentId)
        {
            var staffAttendances = await _dbContext.StudentAttendances
                .Include(s => s.StudentClass)
                .Where(e => e.StudentClass.StudentId == studentId).ToListAsync();
            return staffAttendances;
        }
    }
}
