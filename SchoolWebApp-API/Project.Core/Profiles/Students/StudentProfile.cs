using AutoMapper;
using SchoolWebApp.Core.DTOs.Students.Student;

namespace SchoolWebApp.Core.Profiles.Students
{
    public class StudentProfile: Profile
    {
        public StudentProfile()
        {
            CreateMap<SchoolWebApp.Core.Entities.Students.Student, StudentDto>();
            CreateMap<StudentDto, SchoolWebApp.Core.Entities.Students.Student>();
            CreateMap<CreateStudentDto, SchoolWebApp.Core.Entities.Students.Student>();
            CreateMap<CreateStudentDto, StudentDto>();
        }
    }
}
