using Project.Core.Interfaces.IRepositories;
using SchoolWebApp.Core.Entities.Academics;

namespace SchoolWebApp.Core.Interfaces.IRepositories.Academics
{
    public interface IExamResultRepository : IBaseRepository<ExamResult>
    {
        Task<List<ExamResult>> GetByStudentId(int studentId);
        Task<List<ExamResult>> GetByExamId(int examId);
    }
}
