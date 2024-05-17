using Project.Core.Interfaces.IRepositories;
using SchoolWebApp.Core.Entities.Academics;
using SchoolWebApp.Core.Entities.Staff;

namespace SchoolWebApp.Core.Interfaces.IRepositories.Staff
{
    public interface IStaffAttendanceRepository : IBaseRepository<StaffAttendance>
    {
        Task<List<StaffAttendance>> GetByStaffDetailsId(int staffDetailsId);
    }
}
