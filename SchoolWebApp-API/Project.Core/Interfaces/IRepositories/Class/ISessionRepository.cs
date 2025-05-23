using Project.Core.Interfaces.IRepositories;
using SchoolWebApp.Core.Entities.Class;

namespace SchoolWebApp.Core.Interfaces.IRepositories.Class
{
    public interface ISessionRepository : IBaseRepository<Session>
    {
        Task<List<Session>> GetByCurriculumId(int curriculumId);
        Task<List<Session>> GetByCurriculumIdYearId(int? curriculumId, int? academicYearId);
    }
}
