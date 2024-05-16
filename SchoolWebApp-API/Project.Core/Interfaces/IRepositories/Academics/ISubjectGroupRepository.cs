using Project.Core.Interfaces.IRepositories;
using SchoolWebApp.Core.Entities.Academics;

namespace SchoolWebApp.Core.Interfaces.IRepositories.Academics
{
    public interface ISubjectGroupRepository : IBaseRepository<SubjectGroup>
    {
        Task<List<SubjectGroup>> GetByCurriculumId(int curriculumId);
    }
}
