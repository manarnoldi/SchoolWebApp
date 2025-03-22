using Microsoft.EntityFrameworkCore;
using Project.Infrastructure.Data;
using Project.Infrastructure.Repositories;
using SchoolWebApp.Core.Entities.Academics;
using SchoolWebApp.Core.Interfaces.IRepositories.Academics;

namespace SchoolWebApp.Infrastructure.Repositories.Academics
{
    public class ExamResultRepository : BaseRepository<ExamResult>, IExamResultRepository
    {
        public ExamResultRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }

        public async Task<List<ExamResult>> GetByExamId(int examId)
        {
            var examsResults = await _dbContext.ExamResults
                .Where(e => e.ExamId == examId)
                .Include(e => e.StudentSubject.StudentClass.Student)
                .Include(e => e.Exam.Subject)
                .Include(e => e.Exam.ExamType)
                .ToListAsync();
            return examsResults;
        }

        //public async Task<List<ExamResult>> GetByStudentId(int studentId)
        //{
        //    var examsResults = await _dbContext.ExamResults.Where(e => e.StudentSubject. == studentId)
        //        .Include(e => e.Student)
        //        .Include(e => e.Exam)
        //        .ToListAsync();
        //    return examsResults;
        //}

        public async Task<ExamResult> GetByStudentSubjectExamId(int studentSubjectId, int examId)
        {
            var examsResults = await _dbContext.ExamResults
                .Where(e => e.StudentSubjectId == studentSubjectId && e.ExamId == examId)
                .Include(e => e.StudentSubject)
                .Include(e => e.Exam)
                .FirstOrDefaultAsync();
            return examsResults;
        }

        public async Task<List<ExamResult>> GetByStudentSubjectId(int studentSubjectId)
        {
            var examsResults = await _dbContext.ExamResults
                .Where(e => e.StudentSubjectId == studentSubjectId)
                .Include(e => e.StudentSubject)
                .Include(e => e.Exam)
                .ToListAsync();
            return examsResults;
        }
    }
}
