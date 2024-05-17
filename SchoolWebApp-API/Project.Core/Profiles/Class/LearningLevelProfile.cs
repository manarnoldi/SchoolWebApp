using AutoMapper;
using SchoolWebApp.Core.DTOs.Class.LearningLevel;
using SchoolWebApp.Core.Entities.Class;

namespace SchoolWebApp.Core.Profiles.Class
{
    public class LearningLevelProfile: Profile
    {
        public LearningLevelProfile()
        {
            CreateMap<LearningLevel, LearningLevelDto>();
            CreateMap<LearningLevelDto, LearningLevel>();
            CreateMap<CreateLearningLevelDto, LearningLevel>();
            CreateMap<CreateLearningLevelDto, LearningLevelDto>();
        }
    }
}
