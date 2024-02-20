using AutoMapper;
using SchoolWebApp.Core.DTOs.Settings.Nationality;
using SchoolWebApp.Core.Entities.Settings;

namespace SchoolWebApp.Core.Profiles.Settings
{
    public class NationalityProfile: Profile
    {
        public NationalityProfile()
        {
            CreateMap<Nationality, NationalityDto>();
            CreateMap<NationalityDto, Nationality>();
            CreateMap<CreateNationalityDto, Nationality>();
            CreateMap<CreateNationalityDto, NationalityDto>();
        }
    }
}
