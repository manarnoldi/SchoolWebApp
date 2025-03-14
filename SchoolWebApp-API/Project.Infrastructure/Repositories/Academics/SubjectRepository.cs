using Microsoft.EntityFrameworkCore;
using Project.Infrastructure.Data;
using Project.Infrastructure.Repositories;
using SchoolWebApp.Core.Entities.Academics;
using SchoolWebApp.Core.Entities.School;
using SchoolWebApp.Core.Interfaces.IRepositories.Academics;

namespace SchoolWebApp.Infrastructure.Repositories.Academics
{
    public class SubjectRepository : BaseRepository<Subject>, ISubjectRepository
    {
        public SubjectRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }

        public async Task<List<Subject>> GetByCurriculumId(int curriculumId)
        {
            var subjects = await _dbContext.Subjects.Where(s => s.SubjectGroup.CurriculumId == curriculumId).ToListAsync();
            return subjects;
        }

        public async Task<List<Subject>> GetByDepartmentId(int departmentId)
        {
            var subjects = await _dbContext.Subjects.Where(e => e.DepartmentId == departmentId).ToListAsync();
            return subjects;
        }

        public async Task<List<Subject>> GetBySubjectGroupId(int subjectGroupId)
        {
            var subjects = await _dbContext.Subjects.Where(e => e.SubjectGroupId == subjectGroupId).ToListAsync();
            return subjects;
        }
    }
}
