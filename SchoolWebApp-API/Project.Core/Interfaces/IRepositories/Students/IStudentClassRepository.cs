using Project.Core.Interfaces.IRepositories;
using SchoolWebApp.Core.Entities.Students;

namespace SchoolWebApp.Core.Interfaces.IRepositories.Students
{
    public interface IStudentClassRepository : IBaseRepository<StudentClass>
    {
        Task<List<StudentClass>> GetByStudentId(int studentId);
        Task<bool> CheckIfStudentAssignedForYear(int schoolClassId, int studentId);
    }
}
