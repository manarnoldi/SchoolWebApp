using Project.Core.Interfaces.IRepositories;
using SchoolWebApp.Core.Entities.Staff;

namespace SchoolWebApp.Core.Interfaces.IRepositories.Staff
{
    public interface IStaffSubjectRepository : IBaseRepository<StaffSubject>
    {
        Task<List<StaffSubject>> GetByAcademicYearId(int academicYearId);
        Task<List<StaffSubject>> GetBySchoolClassId(int schoolClassId);
        Task<List<StaffSubject>> GetByStaffDetailsId(int staffDetailsId);
    }
}
