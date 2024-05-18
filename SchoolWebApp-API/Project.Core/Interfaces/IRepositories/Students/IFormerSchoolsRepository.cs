using Project.Core.Interfaces.IRepositories;
using SchoolWebApp.Core.Entities.Students;

namespace SchoolWebApp.Core.Interfaces.IRepositories.Students
{
    public interface IFormerSchoolsRepository : IBaseRepository<FormerSchool>
    {
        Task<List<FormerSchool>> GetByStudentId(int studentId);
    }
}
