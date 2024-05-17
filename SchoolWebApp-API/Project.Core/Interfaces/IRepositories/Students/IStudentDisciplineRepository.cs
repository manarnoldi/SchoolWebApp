using Project.Core.Interfaces.IRepositories;
using SchoolWebApp.Core.Entities.Students;

namespace SchoolWebApp.Core.Interfaces.IRepositories.Students
{
    public interface IStudentDisciplineRepository : IBaseRepository<StudentDiscipline>
    {
        Task<List<StudentDiscipline>> GetByStudentId(int studentId);
    }
}
