using Project.Core.Interfaces.IRepositories;
using SchoolWebApp.Core.Entities.School;

namespace SchoolWebApp.Core.Interfaces.IRepositories.School
{
    public interface IToDoListRepository : IBaseRepository<ToDoList>
    {
        Task<List<ToDoList>> GetByStaffId(int staffId);
    }
}
