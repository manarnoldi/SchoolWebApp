using SchoolWebApp.Core.Entities.Shared;

namespace SchoolWebApp.Core.Entities.Staff
{
    public  class StaffAttendance: Attendance
    {
        public int StaffDetailsId { get; set; }
        public StaffDetails StaffDetails { get; set; }  
    }
}
