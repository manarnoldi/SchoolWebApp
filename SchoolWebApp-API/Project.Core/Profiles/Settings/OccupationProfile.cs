using AutoMapper;
using SchoolWebApp.Core.DTOs.Settings.Occupation;
using SchoolWebApp.Core.Entities.Settings;

namespace SchoolWebApp.Core.Profiles.Settings
{
    public class OccupationProfile: Profile
    {
        public OccupationProfile()
        {
            CreateMap<Occupation, OccupationDto>();
            CreateMap<OccupationDto, Occupation>();
            CreateMap<CreateOccupationDto, Occupation>();
            CreateMap<CreateOccupationDto, OccupationDto>();
        }
    }
}
