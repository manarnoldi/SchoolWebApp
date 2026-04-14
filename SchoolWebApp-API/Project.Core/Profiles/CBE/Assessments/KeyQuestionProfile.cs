using AutoMapper;
using SchoolWebApp.Core.DTOs.CBE.Assessments.KeyQuestion;
using SchoolWebApp.Core.Entities.CBE.Assessments;

namespace SchoolWebApp.Core.Profiles.CBE.Assessments
{
    public class KeyQuestionProfile: Profile
    {
        public KeyQuestionProfile()
        {
            CreateMap<Entities.CBE.Assessments.KeyQuestion, KeyQuestionDto>();
            CreateMap<KeyQuestionDto, Entities.CBE.Assessments.KeyQuestion>();
            CreateMap<CreateKeyQuestionDto, Entities.CBE.Assessments.KeyQuestion>();
            CreateMap<CreateKeyQuestionDto, KeyQuestionDto>();
        }
    }
}
