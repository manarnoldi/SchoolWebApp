using SchoolWebApp.Core.DTOs.Students.StudentClass;
using SchoolWebApp.Core.Entities.Students;

namespace SchoolWebApp.Core.DTOs.Students.StudentAttendance
{
    public class CreateStudentAttendanceDto : AttendanceDto
    {
        public int StudentClassId { get; set; }
        public StudentClassDto? StudentClass { get; set; }
    }
}
