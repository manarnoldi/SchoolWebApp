using SchoolWebApp.Core.Entities.Shared;

namespace SchoolWebApp.Core.Entities.Students
{
    public  class StudentAttendance: Attendance
    {
        public int StudentClassId { get; set; }
        public StudentClass StudentClass { get; set; }        
    }
}
