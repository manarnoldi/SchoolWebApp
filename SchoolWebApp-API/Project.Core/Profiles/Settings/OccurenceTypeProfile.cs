using AutoMapper;
using SchoolWebApp.Core.DTOs.Settings.OccurenceType;
using SchoolWebApp.Core.Entities.Settings;

namespace SchoolWebApp.Core.Profiles.Settings
{
    public class OccurenceTypeProfile: Profile
    {
        public OccurenceTypeProfile()
        {
            CreateMap<OccurenceType, OccurenceTypeDto>();
            CreateMap<OccurenceTypeDto, OccurenceType>();
            CreateMap<CreateOccurenceTypeDto, OccurenceType>();
            CreateMap<CreateOccurenceTypeDto, OccurenceTypeDto>();
        }
    }
}
