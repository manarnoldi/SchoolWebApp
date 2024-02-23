using AutoMapper;
using SchoolWebApp.Core.DTOs.Students.StudentParent;
using SchoolWebApp.Core.Entities.Students;

namespace SchoolWebApp.Core.Profiles.Students
{
    public class StudentParentProfile: Profile
    {
        public StudentParentProfile()
        {
            CreateMap<StudentParent, StudentParentDto>();
            CreateMap<StudentParentDto, StudentParent>();
            CreateMap<CreateStudentParentDto, StudentParent>();
            CreateMap<CreateStudentParentDto, StudentParentDto>();
        }
    }
}
