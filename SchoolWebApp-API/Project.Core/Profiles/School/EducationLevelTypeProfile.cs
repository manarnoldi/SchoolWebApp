using AutoMapper;
using SchoolWebApp.Core.DTOs.School.EducationLevelType;
using SchoolWebApp.Core.Entities.School;

namespace SchoolWebApp.Core.Profiles.School
{
    public class EducationLevelTypeProfile : Profile
    {
        public EducationLevelTypeProfile()
        {
            CreateMap<EducationLevelType, EducationLevelTypeDto>();
            CreateMap<EducationLevelTypeDto, EducationLevelType>();
            CreateMap<CreateEducationLevelTypeDto, EducationLevelType>();
            CreateMap<CreateEducationLevelTypeDto, EducationLevelTypeDto>();
        }
    }
}
