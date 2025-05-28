using Microsoft.EntityFrameworkCore;
using Project.Infrastructure.Data;
using Project.Infrastructure.Repositories;
using SchoolWebApp.Core.Entities.Staff;
using SchoolWebApp.Core.Entities.Students;
using SchoolWebApp.Core.Interfaces.IRepositories.Students;
using System;

namespace SchoolWebApp.Infrastructure.Repositories.Students
{
    public class StudentAttendanceRepository : BaseRepository<StudentAttendance>, IStudentAttendanceRepository
    {
        public StudentAttendanceRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }

        public async Task<List<StudentAttendance>> GetByStudentClassId(int studentClassId)
        {
            var studentAttendances = await _dbContext.StudentAttendances
                .Include(s => s.StudentClass)
                .Where(e => e.StudentClassId == studentClassId).ToListAsync();
            return studentAttendances;
        }

        public async Task<List<StudentAttendance>> GetByStudentId(int studentId)
        {
            var studentAttendances = await _dbContext.StudentAttendances
                .Include(s => s.StudentClass)
                .Where(e => e.StudentClass.StudentId == studentId).ToListAsync();
            return studentAttendances;
        }

        public async Task<StudentAttendance> GetByStudentClassAttendanceDate(int studentClassId, DateOnly attendanceDate)
        {
            var studentAttendance = await _dbContext.StudentAttendances
                .Include(s => s.StudentClass)
                .Where(s => s.StudentClassId == studentClassId && DateOnly.FromDateTime(s.Date) == attendanceDate)
                .FirstOrDefaultAsync();

            return studentAttendance;
        }

        public async Task<List<int>> GetDistinctMonths()
        {
            var months = await _dbContext.StudentAttendances
                .Select(s => s.Date.Month)
                .Distinct()
                .OrderBy(m => m)
                .ToListAsync();
            return months;
        }

        public async Task<List<int>> GetDistinctYears()
        {
            var years = await _dbContext.StudentAttendances
                .Select(s => s.Date.Year)
                .Distinct()
                .OrderByDescending(m => m)
                .ToListAsync();
            return years;
        }

        public async Task<List<StudentAttendance>> GetByMonthYearStudentClassId(int month, int year, int studentClassId)
        {
            var studentAttendances = await _dbContext.StudentAttendances
              .Where(a => a.StudentClassId == studentClassId && a.Date.Month == month && a.Date.Year == year)
               .Include(s => s.StudentClass)
              .ToListAsync();

            return studentAttendances;
        }
    }
}
