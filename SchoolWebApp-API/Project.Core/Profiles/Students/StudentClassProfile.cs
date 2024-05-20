using AutoMapper;
using SchoolWebApp.Core.DTOs.Students.StudentClass;
using SchoolWebApp.Core.Entities.Students;

namespace SchoolWebApp.Core.Profiles.Students
{
    public class StudentClassProfile: Profile
    {
        public StudentClassProfile()
        {
            CreateMap<StudentClass, StudentClassDto>();
            CreateMap<StudentClassDto, StudentClass>();
            CreateMap<CreateStudentClassDto, StudentClass>();
            CreateMap<CreateStudentClassDto, StudentClassDto>();
        }
    }
}
