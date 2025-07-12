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
                .Include(e => e.Student)
                .Include(e => e.Exam.SchoolClass)
                .Include(e => e.Exam.Subject)
                .Include(e => e.Exam.ExamName)
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

        public async Task<ExamResult> GetByStudentExamId(int studentId, int examId)
        {
            var examsResults = await _dbContext.ExamResults
                .Where(e => e.StudentId == studentId && e.ExamId == examId)
                .Include(e => e.Student)
                .Include(e => e.Exam)
                .Include(e => e.Exam.Subject)
                .FirstOrDefaultAsync();
            return examsResults;
        }

        public async Task<List<ExamResult>> GetByStudentSubjectId(int studentId, int subjectId)
        {
            var examsResults = await _dbContext.ExamResults
                  .Where(e => e.StudentId == studentId && e.Exam.SubjectId == subjectId)
                .Include(e => e.Student)
                .Include(e => e.Exam.Subject)
                .ToListAsync();
            return examsResults;
        }
    }
}
