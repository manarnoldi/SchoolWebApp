using AutoMapper;
using SchoolWebApp.Core.DTOs.CBE.Assessments.LearningExperience;
using SchoolWebApp.Core.Entities.CBE.Assessments;

namespace SchoolWebApp.Core.Profiles.CBE.Assessments
{
    public class LearningExperienceProfile: Profile
    {
        public LearningExperienceProfile()
        {
            CreateMap<Entities.CBE.Assessments.LearningExperience, LearningExperienceDto>();
            CreateMap<LearningExperienceDto, Entities.CBE.Assessments.LearningExperience>();
            CreateMap<CreateLearningExperienceDto, Entities.CBE.Assessments.LearningExperience>();
            CreateMap<CreateLearningExperienceDto, LearningExperienceDto>();
        }
    }
}
