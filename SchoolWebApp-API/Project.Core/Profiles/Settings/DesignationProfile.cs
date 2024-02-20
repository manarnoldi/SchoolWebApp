using AutoMapper;
using SchoolWebApp.Core.DTOs.Settings.Designation;
using SchoolWebApp.Core.Entities.Settings;

namespace SchoolWebApp.Core.Profiles.Settings
{
    public class DesignationProfile: Profile
    {
        public DesignationProfile()
        {
            CreateMap<Designation, DesignationDto>();
            CreateMap<DesignationDto, Designation>();
            CreateMap<CreateDesignationDto, Designation>();
            CreateMap<CreateDesignationDto, DesignationDto>();
        }
    }
}
