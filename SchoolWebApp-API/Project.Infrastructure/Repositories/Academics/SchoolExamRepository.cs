using Microsoft.EntityFrameworkCore;
using Project.Infrastructure.Data;
using Project.Infrastructure.Repositories;
using SchoolWebApp.Core.Entities.CBE.Exams;
using SchoolWebApp.Core.Interfaces.IRepositories.Academics;

namespace SchoolWebApp.Infrastructure.Repositories.Academics
{
    public class SchoolExamRepository : BaseRepository<SchoolExam>, ISchoolExamRepository
    {
        public SchoolExamRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }

        public async Task<List<SchoolExam>> SearchForSchoolExams(int academicYearId, int curriculumId,
            int? sessionId, int? examTypeId)
        {
            // The Session FK carries academic year + curriculum, so we scope
            // through it rather than duplicating those columns on SchoolExam.
            var query = _dbContext.SchoolExams
                .Where(se => se.Session.AcademicYearId == academicYearId
                    && se.Session.CurriculumId == curriculumId)
                .Include(se => se.ExamType)
                .Include(se => se.Session)
                .AsQueryable();

            if (sessionId != null)
                query = query.Where(se => se.SessionId == sessionId);
            if (examTypeId != null)
                query = query.Where(se => se.ExamTypeId == examTypeId);

            return await query.OrderByDescending(se => se.ExamStartDate).ToListAsync();
        }
    }
}
