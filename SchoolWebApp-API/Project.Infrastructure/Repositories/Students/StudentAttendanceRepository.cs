using Microsoft.EntityFrameworkCore;
using Project.Infrastructure.Data;
using Project.Infrastructure.Repositories;
using SchoolWebApp.Core.Entities.Staff;
using SchoolWebApp.Core.Entities.Students;
using SchoolWebApp.Core.Interfaces.IRepositories.Students;

namespace SchoolWebApp.Infrastructure.Repositories.Students
{
    public class StudentAttendanceRepository : BaseRepository<StudentAttendance>, IStudentAttendanceRepository
    {
        public StudentAttendanceRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }

        public async Task<List<StudentAttendance>> GetByStudentId(int studentId, int classId)
        {
            var studentClass = await _dbContext.StudentClasses.Where(s => s.StudentId == studentId && s.SchoolClassId == classId).FirstOrDefaultAsync();
            var staffAttendances = await _dbContext.StudentAttendances.Where(e => e.StudentClassId == studentClass.SchoolClassId).ToListAsync();
            return staffAttendances;
        }
    }
}
