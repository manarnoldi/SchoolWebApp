using AutoMapper;
using SchoolWebApp.Core.DTOs.Students.Student;

namespace SchoolWebApp.Core.Profiles.Students
{
    public class StudentProfile: Profile
    {
        public StudentProfile()
        {
            CreateMap<Entities.Students.Student, StudentDto>();
            CreateMap<StudentDto, Entities.Students.Student>();
            CreateMap<CreateStudentDto, Entities.Students.Student>();
            CreateMap<CreateStudentDto, StudentDto>();
        }
    }
}
