using AutoMapper;
using SchoolWebApp.Core.DTOs.Settings.Gender;
using SchoolWebApp.Core.Entities.Settings;

namespace SchoolWebApp.Core.Profiles.Settings
{
    public class GenderProfile: Profile
    {
        public GenderProfile()
        {
            CreateMap<Gender, GenderDto>();
            CreateMap<GenderDto, Gender>();
            CreateMap<CreateGenderDto, Gender>();
            CreateMap<CreateGenderDto, GenderDto>();
        }
    }
}
