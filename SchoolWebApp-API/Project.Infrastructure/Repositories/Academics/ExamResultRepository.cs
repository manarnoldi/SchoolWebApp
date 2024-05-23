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
            var examsResults = await _dbContext.ExamResults.Where(e => e.ExamId == examId).ToListAsync();
            return examsResults;
        }

        public async Task<List<ExamResult>> GetByStudentId(int studentId)
        {
            var examsResults = await _dbContext.ExamResults.Where(e => e.StudentId == studentId).ToListAsync();
            return examsResults;
        }
    }
}
