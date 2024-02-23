using Project.Core.Interfaces.IRepositories;
using SchoolWebApp.Core.Entities.Staff;
using SchoolWebApp.Core.Entities.Students;

namespace SchoolWebApp.Core.Interfaces.IRepositories.Student
{
    public interface IParentsRepository : IBaseRepository<Parent>
    {
        Task<List<SchoolWebApp.Core.Entities.Students.Student>> GetParentStudents(int parentId);
    }
}
