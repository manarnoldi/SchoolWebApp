using Project.Core.Interfaces.IRepositories;
using SchoolWebApp.Core.Entities.Students;

namespace SchoolWebApp.Core.Interfaces.IRepositories.Students
{
    public interface IStudentsRepository : IBaseRepository<Student>
    {
        Task<List<Student>> GetByLearningModeId(int learningModeId);
    }
}
