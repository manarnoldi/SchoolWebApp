using AutoMapper;
using SchoolWebApp.Core.DTOs.Settings.Religion;
using SchoolWebApp.Core.Entities.Settings;

namespace SchoolWebApp.Core.Profiles.Settings
{
    public class ReligionProfile: Profile
    {
        public ReligionProfile()
        {
            CreateMap<Religion, ReligionDto>();
            CreateMap<ReligionDto, Religion>();
            CreateMap<CreateReligionDto, Religion>();
            CreateMap<CreateReligionDto, ReligionDto>();
        }
    }
}
