using AutoMapper;
using SchoolWebApp.Core.DTOs.Settings.GlobalSetting;

namespace SchoolWebApp.Core.Profiles.Settings
{
    public class GlobalSettingProfile : Profile
    {
        public GlobalSettingProfile()
        {
            CreateMap<SchoolWebApp.Core.Entities.Settings.GlobalSetting, GlobalSettingDto>();
            CreateMap<GlobalSettingDto, SchoolWebApp.Core.Entities.Settings.GlobalSetting>();
            CreateMap<CreateGlobalSettingDto, SchoolWebApp.Core.Entities.Settings.GlobalSetting>();
            CreateMap<CreateGlobalSettingDto, GlobalSettingDto>();
        }
    }
}
