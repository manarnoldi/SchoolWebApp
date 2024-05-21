using AutoMapper;
using SchoolWebApp.Core.DTOs.Students.StudentSubjects;
using SchoolWebApp.Core.Entities.Students;

namespace SchoolWebApp.Core.Profiles.Students
{
    public class StudentSubjectProfile: Profile
    {
        public StudentSubjectProfile()
        {
            CreateMap<StudentSubject, StudentSubjectDto>();
            CreateMap<StudentSubjectDto, StudentSubject>();
            CreateMap<CreateStudentSubjectDto, StudentSubject>();
            CreateMap<CreateStudentSubjectDto, StudentSubjectDto>();
        }
    }
}
