using AutoMapper;
using SchoolWebApp.Core.DTOs.CBE.Responsibilities.StudentResponsibility;
using SchoolWebApp.Core.Entities.CBE.Responsibilities;

namespace SchoolWebApp.Core.Profiles.CBE.Responsibilities
{
    public class StudentResponsibilityProfile : Profile
    {
        public StudentResponsibilityProfile()
        {
            CreateMap<Entities.CBE.Responsibilities.StudentResponsibility, StudentResponsibilityDto>();
            CreateMap<StudentResponsibilityDto, Entities.CBE.Responsibilities.StudentResponsibility>();
            CreateMap<CreateStudentResponsibilityDto, Entities.CBE.Responsibilities.StudentResponsibility>();
            CreateMap<CreateStudentResponsibilityDto, StudentResponsibilityDto>();
        }
    }
}
