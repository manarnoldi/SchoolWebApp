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
            var exams = await _dbContext.Exams.Where(e => e.SchoolClassId == schoolClassId).Include(e => e.ExamType)
                .Include(e => e.SchoolClass)
                .Include(e => e.Session)
                .Include(e => e.Subject).ToListAsync();
            return exams;
        }

        public async Task<List<Exam>> GetBySessionId(int sessionId)
        {
            var exams = await _dbContext.Exams.Where(e => e.SessionId == sessionId).Include(e => e.ExamType)
                .Include(e => e.SchoolClass)
                .Include(e => e.Session)
                .Include(e => e.Subject).ToListAsync();
            return exams;
        }

        public async Task<List<Exam>> SearchForExam(int academicYearId, int curriculumId, int sessionId, 
            int schoolClassId, int? subjectId, int? examTypeId,string? examName)
        {
            var query = _dbContext.Exams
                .Where(c => c.SchoolClass.AcademicYearId == academicYearId
                && c.Session.CurriculumId == curriculumId
                && c.SessionId == sessionId
                && c.SchoolClassId == schoolClassId)
                .Include(e => e.ExamType)
                .Include(e => e.SchoolClass)
                .Include(e => e.Session)
                .Include(e => e.Subject)
                .AsQueryable();

            if (examTypeId != null)
                query = query.Where(s => s.ExamTypeId == examTypeId);
            if (subjectId != null)
                query = query.Where(s => s.SubjectId == subjectId);
            if (!(examName == null || examName == ""))
                query = query.Where(s => s.Name == examName);

            var exams = await query.ToListAsync();
            return exams;
        }
    }
}
