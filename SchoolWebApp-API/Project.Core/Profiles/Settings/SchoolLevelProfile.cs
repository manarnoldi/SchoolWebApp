using AutoMapper;
using SchoolWebApp.Core.DTOs.Settings.SchoolLevel;
using SchoolWebApp.Core.Entities.Settings;

namespace SchoolWebApp.Core.Profiles.Settings
{
    public class SchoolLevelProfile: Profile
    {
        public SchoolLevelProfile()
        {
            CreateMap<SchoolLevel, SchoolLevelDto>();
            CreateMap<SchoolLevelDto, SchoolLevel>();
            CreateMap<CreateSchoolLevelDto, SchoolLevel>();
            CreateMap<CreateSchoolLevelDto, SchoolLevelDto>();
        }
    }
}
