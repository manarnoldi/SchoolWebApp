using AutoMapper;
using SchoolWebApp.Core.DTOs.School.EducationLevel;
using SchoolWebApp.Core.Entities.School;

namespace SchoolWebApp.Core.Profiles.School
{
    public class EducationLevelProfile : Profile
    {
        public EducationLevelProfile()
        {
            CreateMap<EducationLevel, EducationLevelDto>();
            CreateMap<EducationLevelDto, EducationLevel>();
            CreateMap<CreateEducationLevelDto, EducationLevel>();
            CreateMap<CreateEducationLevelDto, EducationLevelDto>();
        }
    }
}
