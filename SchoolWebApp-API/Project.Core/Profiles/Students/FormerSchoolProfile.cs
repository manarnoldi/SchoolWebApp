using AutoMapper;
using SchoolWebApp.Core.DTOs.Students.FormerSchool;
using SchoolWebApp.Core.Entities.Students;

namespace SchoolWebApp.Core.Profiles.Students
{
    public class FormerSchoolProfile : Profile
    {
        public FormerSchoolProfile()
        {
            CreateMap<FormerSchool, FormerSchoolDto>();
            CreateMap<FormerSchoolDto, FormerSchool>();
            CreateMap<CreateFormerSchoolDto, FormerSchool>();
            CreateMap<CreateFormerSchoolDto, FormerSchoolDto>();
        }
    }
}
