using Microsoft.EntityFrameworkCore;
using Project.Infrastructure.Data;
using Project.Infrastructure.Repositories;
using SchoolWebApp.Core.Entities.Academics;
using SchoolWebApp.Core.Interfaces.IRepositories.Academics;

namespace SchoolWebApp.Infrastructure.Repositories.Academics
{
    public class ExamRepository : BaseRepository<Exam>, IExamRepository
    {
        public ExamRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }

        public async Task<List<Exam>> GetBySchoolClassId(int schoolClassId)
        {
            var exams = await _dbContext.Exams.Where(e => e.SchoolClassId == schoolClassId).ToListAsync();
            return exams;
        }

        public async Task<List<Exam>> GetBySessionId(int sessionId)
        {
            var exams = await _dbContext.Exams.Where(e => e.SessionId == sessionId).ToListAsync();
            return exams;
        }
    }
}
