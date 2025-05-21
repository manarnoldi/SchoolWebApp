using Project.Core.Interfaces.IRepositories;
using SchoolWebApp.Core.DTOs.School.Event;
using SchoolWebApp.Core.Entities.School;

namespace SchoolWebApp.Core.Interfaces.IRepositories.School
{
    public interface IEventRepository : IBaseRepository<Event>
    {
        Task<List<Event>> GetBySessionId(int sessionId);
        Task<List<Event>> GetByAcademicYearId(int? academicYearId);
    }
}
