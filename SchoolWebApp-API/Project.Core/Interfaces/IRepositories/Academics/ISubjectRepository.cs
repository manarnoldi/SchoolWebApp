using Project.Core.Interfaces.IRepositories;
using SchoolWebApp.Core.Entities.Academics;

namespace SchoolWebApp.Core.Interfaces.IRepositories.Academics
{
    public interface ISubjectRepository : IBaseRepository<Subject>
    {
        Task<List<Subject>> GetBySubjectGroupId(int subjectGroupId);
        Task<List<Subject>> GetByDepartmentId(int departmentId);
    }
}
