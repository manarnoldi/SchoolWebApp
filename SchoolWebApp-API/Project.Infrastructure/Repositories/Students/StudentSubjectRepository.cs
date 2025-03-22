using Microsoft.EntityFrameworkCore;
using Project.Infrastructure.Data;
using Project.Infrastructure.Repositories;
using SchoolWebApp.Core.Entities.Students;
using SchoolWebApp.Core.Interfaces.IRepositories.Students;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchoolWebApp.Infrastructure.Repositories.Students
{
    public class StudentSubjectRepository : BaseRepository<StudentSubject>, IStudentSubjectRepository
    {
        public StudentSubjectRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }

        public async Task<List<StudentSubject>> GetByStudentClassId(int studentClassId)
        {
            var studentSubjects = await _dbContext.StudentSubjects
                .Where(e => e.StudentClassId == studentClassId)
                .Include(s => s.Subject)
                .Include(s => s.StudentClass)
                .ToListAsync();
            return studentSubjects;
        }

        public async Task<List<StudentSubject>> GetBySubjectId(int subjectId)
        {
            var studentSubjects = await _dbContext.StudentSubjects
                .Where(e => e.SubjectId == subjectId)
                .Include(s => s.Subject)
                .Include(s => s.StudentClass)
                .ToListAsync();
            return studentSubjects;
        }

        public async Task<List<StudentSubject>> GetByStudentId(int studentId)
        {
            var studentSubjects = await _dbContext.StudentSubjects
                .Where(e => e.StudentClass.StudentId == studentId)
                .Include(s => s.Subject)
                .Include(s => s.StudentClass)
                .ToListAsync();
            return studentSubjects;
        }

        public async Task<List<StudentSubject>> GetBySchoolClassId(int schoolClassId, int studentId)
        {
            var studentSubjects = await _dbContext.StudentSubjects
                .Where(e => e.StudentClass.SchoolClassId == schoolClassId && e.StudentClass.StudentId == studentId)
                .Include(s => s.Subject)
                .Include(s => s.StudentClass)
                .ToListAsync();
            return studentSubjects;
        }

        public async Task<StudentSubject> GetByStudentClassSubjectId(int studentClassId, int subjectId)
        {
            var studentSubject = await _dbContext
                .StudentSubjects
                .Where(ss => ss.SubjectId == subjectId && ss.StudentClassId == studentClassId)
                .FirstOrDefaultAsync();
            return studentSubject;
        }

        public async Task<List<StudentSubject>> GetBySchoolClassSubjectId(int schoolClassId, int subjectId)
        {
            var studentSubjects = await _dbContext.StudentSubjects
               .Where(e => e.StudentClass.SchoolClassId == schoolClassId && e.SubjectId == subjectId)
               .Include(s => s.Subject)
               .Include(s => s.StudentClass)
               .Include(s => s.StudentClass.Student)
               .ToListAsync();
            return studentSubjects;
        }
    }
}
