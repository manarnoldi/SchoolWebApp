using Project.Core.Interfaces.IRepositories;
using SchoolWebApp.Core.DTOs.Students.StudentParent;
using SchoolWebApp.Core.Entities.Students;

namespace SchoolWebApp.Core.Interfaces.IRepositories.Students
{
    public interface IStudentsRepository : IBaseRepository<Student>
    {
        Task<List<Student>> GetByLearningModeId(int learningModeId);
        Task<List<StudentParentDetailsDto>> GetParentsByStudentId(int studentId);
    }
}
