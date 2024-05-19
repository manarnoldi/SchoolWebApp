using Project.Core.Interfaces.IRepositories;
using SchoolWebApp.Core.Entities.Class;
using SchoolWebApp.Core.Entities.Students;

namespace SchoolWebApp.Core.Interfaces.IRepositories.Class
{
    public interface ISchoolClassRepository : IBaseRepository<SchoolClass>
    {
        Task<List<SchoolClass>> GetByAcademicYearId(int academicYearId);
    }
}
