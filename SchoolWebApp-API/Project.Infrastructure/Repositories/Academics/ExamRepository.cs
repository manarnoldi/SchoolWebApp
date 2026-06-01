using Microsoft.EntityFrameworkCore;
using Project.Infrastructure.Data;
using Project.Infrastructure.Repositories;
using SchoolWebApp.Core.Entities.CBE.Exams;
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
            var exams = await _dbContext.Exams.Where(e => e.SchoolClassId == schoolClassId)
                .Include(e => e.SchoolExam).ThenInclude(se => se.ExamType)
                .Include(e => e.SchoolExam).ThenInclude(se => se.Session)
                .Include(e => e.SchoolClass)
                .Include(e => e.Subject).ToListAsync();
            return exams;
        }

        public async Task<List<Exam>> GetBySessionId(int sessionId)
        {
            var exams = await _dbContext.Exams.Where(e => e.SchoolExam.SessionId == sessionId)
                .Include(e => e.SchoolExam).ThenInclude(se => se.ExamType)
                .Include(e => e.SchoolExam).ThenInclude(se => se.Session)
                .Include(e => e.SchoolClass)
                .Include(e => e.Subject).ToListAsync();
            return exams;
        }

        public async Task<List<Exam>> SearchForExam(int academicYearId, int curriculumId, int sessionId,
            int? schoolClassId, int? subjectId, int? examTypeId)
        {
            // Term/year/curriculum scoping now lives on the SchoolExam header.
            var query = _dbContext.Exams
                .Where(c => c.SchoolClass.AcademicYearId == academicYearId
                && c.SchoolExam.Session.CurriculumId == curriculumId
                && c.SchoolExam.SessionId == sessionId)
                .Include(e => e.SchoolExam).ThenInclude(se => se.ExamType)
                .Include(e => e.SchoolExam).ThenInclude(se => se.Session)
                .Include(e => e.SchoolClass)
                .Include(e => e.Subject)
                .AsQueryable();
            if(schoolClassId != null)
                query = query.Where(s => s.SchoolClassId == schoolClassId);
            if (subjectId != null)
                query = query.Where(s => s.SubjectId == subjectId);
            if (examTypeId != null)
                query = query.Where(s => s.SchoolExam.ExamTypeId == examTypeId);

            var exams = await query.ToListAsync();
            return exams;
        }
    }
}
