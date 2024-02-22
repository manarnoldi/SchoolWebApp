using AutoMapper;
using SchoolWebApp.Core.DTOs.School.LearningMode;
using SchoolWebApp.Core.Entities.School;

namespace SchoolWebApp.Core.Profiles.School
{
    public class LearningModeProfile : Profile
    {
        public LearningModeProfile()
        {
            CreateMap<LearningMode, LearningModeDto>();
            CreateMap<LearningModeDto, LearningMode>();
            CreateMap<CreateLearningModeDto, LearningMode>();
            CreateMap<CreateLearningModeDto, LearningModeDto>();
        }
    }
}
