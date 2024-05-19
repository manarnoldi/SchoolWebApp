using Project.Core.Interfaces.IRepositories;
using SchoolWebApp.Core.Entities.Academics;
using SchoolWebApp.Core.Entities.School;

namespace SchoolWebApp.Core.Interfaces.IRepositories.Academics
{
    public interface IExamRepository : IBaseRepository<Exam>
    {
        Task<List<Exam>> GetBySessionId(int sessionId);
        Task<List<Exam>> GetBySchoolClassId(int schoolClassId);
    }
}
