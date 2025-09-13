using Project.Core.Interfaces.IRepositories;
using SchoolWebApp.Core.Entities.Academics;
using SchoolWebApp.Core.Entities.School;

namespace SchoolWebApp.Core.Interfaces.IRepositories.Academics
{
    public interface IGradeRepository : IBaseRepository<Grade>
    {
        Task<List<Grade>> GetByCurriculumId(int? curriculumId);
        Task<Grade> GetByScore(float score);
    }
}
