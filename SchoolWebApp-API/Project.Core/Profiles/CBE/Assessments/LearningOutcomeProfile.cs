using AutoMapper;
using SchoolWebApp.Core.DTOs.CBE.Assessments.LearningOutcome;
using SchoolWebApp.Core.Entities.CBE.Assessments;

namespace SchoolWebApp.Core.Profiles.CBE.Assessments
{
    public class LearningOutcomeProfile: Profile
    {
        public LearningOutcomeProfile()
        {
            CreateMap<LearningOutcome, LearningOutcomeDto>();
            CreateMap<LearningOutcomeDto, LearningOutcome>();
            CreateMap<CreateLearningOutcomeDto, LearningOutcome>();
            CreateMap<CreateLearningOutcomeDto, LearningOutcomeDto>();
        }
    }
}
