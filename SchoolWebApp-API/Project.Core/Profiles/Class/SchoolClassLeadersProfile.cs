using AutoMapper;
using SchoolWebApp.Core.DTOs.Class.SchoolClassLeaders;
using SchoolWebApp.Core.Entities.Class;

namespace SchoolWebApp.Core.Profiles.Class
{
    public class SchoolClassLeadersProfile : Profile
    {
        public SchoolClassLeadersProfile()
        {
            CreateMap<SchoolClassLeaders, SchoolClassLeadersDto>();
            CreateMap<SchoolClassLeadersDto, SchoolClassLeaders>();
            CreateMap<CreateSchoolClassLeadersDto, SchoolClassLeaders>();
            CreateMap<CreateSchoolClassLeadersDto, SchoolClassLeadersDto>();
        }
    }
}
