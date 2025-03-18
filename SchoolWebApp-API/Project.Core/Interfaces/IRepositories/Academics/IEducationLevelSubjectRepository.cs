using Microsoft.EntityFrameworkCore;
using Project.Core.Interfaces.IRepositories;
using SchoolWebApp.Core.Entities.Academics;

namespace SchoolWebApp.Core.Interfaces.IRepositories.Academics
{
    public interface IEducationLevelSubjectRepository : IBaseRepository<EducationLevelSubject>
    {
        public Task<List<EducationLevelSubject>> GetByEducationLevelId(int educationLevelId);

        public Task<List<EducationLevelSubject>> GetBySubjectId(int subjectId);

        public Task<List<EducationLevelSubject>> GetByAcademicYearId(int academicYearId);
        public Task<List<EducationLevelSubject>> GetByEducationLevelYearId(int educationLevelId, int academicYearId);
        public Task<EducationLevelSubject> GetByEducationLevelYearSubjectId(int educationLevelId, int academicYearId, int subjectId);
    }
}
