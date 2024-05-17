using AutoMapper;
using SchoolWebApp.Core.DTOs.Students.StudentDiscipline;
using SchoolWebApp.Core.Entities.Students;

namespace SchoolWebApp.Core.Profiles.Students
{
    public class StudentDisciplineProfile : Profile
    {
        public StudentDisciplineProfile()
        {

            CreateMap<StudentDiscipline, StudentDisciplineDto>();
            CreateMap<StudentDisciplineDto, StudentDiscipline>();
            CreateMap<CreateStudentDisciplineDto, StudentDiscipline>();
            CreateMap<CreateStudentDisciplineDto, StudentDisciplineDto>();
        }
    }
}
