using Microsoft.EntityFrameworkCore;
using Project.Infrastructure.Data;
using Project.Infrastructure.Repositories;
using SchoolWebApp.Core.Entities.Academics;
using SchoolWebApp.Core.Interfaces.IRepositories.Academics;

namespace SchoolWebApp.Infrastructure.Repositories.Academics
{
    public class EducationLevelSubjectRepository : BaseRepository<EducationLevelSubject>, IEducationLevelSubjectRepository
    {
        public EducationLevelSubjectRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }

        public async Task<List<EducationLevelSubject>> GetByEducationLevelId(int educationLevelId)
        {
            var educationLevelSubjects = await _dbContext.EducationLevelSubjects
                .Include(els => els.AcademicYear)
                .Include(els => els.EducationLevel)
                .Include(els => els.Subject)
                .Where(e => e.EducationLevelId == educationLevelId).ToListAsync();
            return educationLevelSubjects;
        }

        public async Task<List<EducationLevelSubject>> GetBySubjectId(int subjectId)
        {
            var educationLevelSubjects = await _dbContext.EducationLevelSubjects
                .Include(els => els.AcademicYear)
                .Include(els => els.EducationLevel)
                .Include(els => els.Subject)
                .Where(e => e.SubjectId == subjectId).ToListAsync();
            return educationLevelSubjects;
        }

        public async Task<List<EducationLevelSubject>> GetByAcademicYearId(int academicYearId)
        {
            var educationLevelSubjects = await _dbContext.EducationLevelSubjects
                .Include(els => els.AcademicYear)
                .Include(els => els.EducationLevel)
                .Include(els => els.Subject)
                .Where(e => e.AcademicYearId == academicYearId).ToListAsync();
            return educationLevelSubjects;
        }

        public async Task<List<EducationLevelSubject>> GetByEducationLevelYearId(int educationLevelId, int academicYearId)
        {
            var educationLevelSubjects = await _dbContext.EducationLevelSubjects
                .Include(els => els.AcademicYear)
                .Include(els => els.EducationLevel)
                .Include(els => els.Subject)
                .Where(e => e.AcademicYearId == academicYearId && e.EducationLevelId == educationLevelId).ToListAsync();
            return educationLevelSubjects;
        }
        public async Task<EducationLevelSubject> GetByEducationLevelYearSubjectId(int educationLevelId, int academicYearId, int subjectId)
        {
            var educationLevelSubjects = await _dbContext.EducationLevelSubjects
                .Include(els => els.AcademicYear)
                .Include(els => els.EducationLevel)
                .Include(els => els.Subject)
                .Where(e => e.AcademicYearId == academicYearId && e.EducationLevelId == educationLevelId && e.SubjectId == subjectId).FirstOrDefaultAsync();
            return educationLevelSubjects;
        }
    }
}
