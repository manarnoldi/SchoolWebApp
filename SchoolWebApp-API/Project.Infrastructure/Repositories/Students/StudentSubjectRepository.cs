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

        public async Task<List<StudentSubject>> GetByAcademicYearId(int academicYearId)
        {
            var studentSubjects = await _dbContext.StudentSubjects.Where(e => e.AcademicYearId == academicYearId).ToListAsync();
            return studentSubjects;
        }

        public async Task<List<StudentSubject>> GetBySubjectId(int subjectId)
        {
            var studentSubjects = await _dbContext.StudentSubjects.Where(e => e.SubjectId == subjectId).ToListAsync();
            return studentSubjects;
        }
    }
}
