using Project.Core.Interfaces.IRepositories;
using SchoolWebApp.Core.Entities.Class;
using SchoolWebApp.Core.Entities.School;

namespace SchoolWebApp.Core.Interfaces.IRepositories.Class
{
    public interface ILearningLevelRepository : IBaseRepository<LearningLevel>
    {
        Task<List<LearningLevel>> GetByCurriculumId(int? curriculumId);
    }
}
