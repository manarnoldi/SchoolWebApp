using Project.Core.Interfaces.IRepositories;
using SchoolWebApp.Core.Entities.Staff;
using SchoolWebApp.Core.Entities.Students;

namespace SchoolWebApp.Core.Interfaces.IRepositories.Students
{
    public interface IStudentAttendanceRepository : IBaseRepository<StudentAttendance>
    {
        Task<List<StudentAttendance>> GetByStudentClassId(int studentClassId);
    }
}
