using AutoMapper;
using SchoolWebApp.Core.DTOs.Class.SchoolClass;
using SchoolWebApp.Core.Entities.Class;

namespace SchoolWebApp.Core.Profiles.Class
{
    public class SchoolClassProfile: Profile
    {
        public SchoolClassProfile()
        {
            CreateMap<SchoolClass, SchoolClassDto>();
            CreateMap<SchoolClassDto, SchoolClass>();
            CreateMap<CreateSchoolClassDto, SchoolClass>();
            CreateMap<CreateSchoolClassDto, SchoolClassDto>();
        }
    }
}
