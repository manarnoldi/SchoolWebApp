using AutoMapper;
using SchoolWebApp.Core.DTOs.CBE.Responsibilities.SocialSkill;

namespace SchoolWebApp.Core.Profiles.CBE.Responsibilities
{
    public class SocialSkillProfile : Profile
    {
        public SocialSkillProfile()
        {
            CreateMap<Entities.CBE.Responsibilities.SocialSkill, SocialSkillDto>();
            CreateMap<SocialSkillDto, Entities.CBE.Responsibilities.SocialSkill>();
            CreateMap<CreateSocialSkillDto, Entities.CBE.Responsibilities.SocialSkill>();
            CreateMap<CreateSocialSkillDto, SocialSkillDto>();
        }
    }
}
