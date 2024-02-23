using Project.Core.Interfaces.IRepositories;
using SchoolWebApp.Core.Entities.School;
using SchoolWebApp.Core.Entities.Students;

namespace SchoolWebApp.Core.Interfaces.IRepositories.Students
{
    public interface IStudentParentRepository : IBaseRepository<StudentParent>
    {
        Task<List<StudentParent>> GetStudentsByParentId(int parentId);
        Task<List<StudentParent>> GetParentsByStudentId(int studentId);
    }
}
