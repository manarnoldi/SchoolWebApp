using AutoMapper;
using SchoolWebApp.Core.DTOs.Staff.StaffAttendance;
using SchoolWebApp.Core.Entities.Staff;

namespace SchoolWebApp.Core.Profiles.Staff
{
    public class StaffAttendanceProfile: Profile
    {
        public StaffAttendanceProfile()
        {
            CreateMap<StaffAttendance, StaffAttendanceDto>();
            CreateMap<StaffAttendanceDto, StaffAttendance>();
            CreateMap<StaffAttendanceDto, StaffAttendance>();
            CreateMap<CreateStaffAttendanceDto, StaffAttendance>();
            CreateMap<CreateStaffAttendanceDto, StaffAttendanceDto>();
        }
    }
}
