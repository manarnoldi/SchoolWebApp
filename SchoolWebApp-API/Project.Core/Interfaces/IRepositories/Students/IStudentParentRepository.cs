using Project.Core.Interfaces.IRepositories;
using SchoolWebApp.Core.Entities.School;
using SchoolWebApp.Core.Entities.Students;
using System.Linq.Expressions;

namespace SchoolWebApp.Core.Interfaces.IRepositories.Students
{
    public interface IStudentParentRepository : IBaseRepository<StudentParent>
    {
        Task<StudentParent> GetStudentParentByParentIdStudentId(int parentId, int studentId);

    }
}
