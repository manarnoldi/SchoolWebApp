using Project.Core.Interfaces.IRepositories;
using SchoolWebApp.Core.Entities.Academics;

namespace SchoolWebApp.Core.Interfaces.IRepositories.Academics
{
    public interface IExamNameRepository : IBaseRepository<ExamName>
    {
        Task<List<ExamName>> GetByExamTypeId(int examTypeId);
    }
}
