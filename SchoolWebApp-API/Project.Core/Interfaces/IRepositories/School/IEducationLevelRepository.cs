using Project.Core.Interfaces.IRepositories;
using SchoolWebApp.Core.Entities.School;

namespace SchoolWebApp.Core.Interfaces.IRepositories.School
{
    public interface IEducationLevelRepository : IBaseRepository<EducationLevel>
    {
        Task<List<EducationLevel>> GetByCurriculumId(int? curriculumId);

    }
}
