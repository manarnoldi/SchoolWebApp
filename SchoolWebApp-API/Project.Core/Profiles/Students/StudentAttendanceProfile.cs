using AutoMapper;
using SchoolWebApp.Core.DTOs.Students.StudentAttendance;
using SchoolWebApp.Core.Entities.Students;

namespace SchoolWebApp.Core.Profiles.Students
{
    public class StudentAttendanceProfile: Profile
    {
        public StudentAttendanceProfile()
        {
            CreateMap<StudentAttendance, StudentAttendanceDto>();
            CreateMap<StudentAttendanceDto, StudentAttendance>();
            CreateMap<CreateStudentAttendanceDto, StudentAttendance>();
            CreateMap<CreateStudentAttendanceDto, StudentAttendanceDto>();
        }
    }
}
