using AutoMapper;
using SchoolWebApp.Core.DTOs.CBE.CommunityService.CommunityServiceActivity;
using SchoolWebApp.Core.Entities.CBE.CommunityService;

namespace SchoolWebApp.Core.Profiles.CBE.CommunityService
{
    public class CommunityServiceActivityProfile: Profile
    {
        public CommunityServiceActivityProfile()
        {
            CreateMap<Entities.CBE.CommunityService.CommunityServiceActivity, CommunityServiceActivityDto>();
            CreateMap<CommunityServiceActivityDto, Entities.CBE.CommunityService.CommunityServiceActivity>();
            CreateMap<CreateCommunityServiceActivityDto, Entities.CBE.CommunityService.CommunityServiceActivity>();
            CreateMap<CreateCommunityServiceActivityDto, CommunityServiceActivityDto>();
        }
    }
}
