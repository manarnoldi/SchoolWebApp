using AutoMapper;
using SchoolWebApp.Core.DTOs.CBE.Assessments.Theme;
using ThemeEntity = SchoolWebApp.Core.Entities.CBE.Assessments.Theme;

namespace SchoolWebApp.Core.Profiles.CBE.Assessments
{
    public class ThemeProfile : Profile
    {
        public ThemeProfile()
        {
            CreateMap<ThemeEntity, ThemeDto>();
            CreateMap<ThemeDto, ThemeEntity>();
            CreateMap<CreateThemeDto, ThemeEntity>();
            CreateMap<CreateThemeDto, ThemeDto>();
        }
    }
}
