using SchoolWebApp.Core.DTOs.Staff.StaffDetails;

namespace SchoolWebApp.Core.DTOs.Staff.StaffAttendance
{
    public class CreateStaffAttendanceDto: AttendanceDto
    {
        public int StaffDetailsId { get; set; }
        public StaffDetailDto? StaffDetails { get; set; }
    }
}
