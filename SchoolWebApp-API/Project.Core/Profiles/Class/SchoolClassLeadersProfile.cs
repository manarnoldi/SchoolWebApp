using AutoMapper;
using SchoolWebApp.Core.DTOs;
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

            CreateMap<SchoolWebApp.Core.Entities.Students.Student, PersonDto>();
            CreateMap<SchoolWebApp.Core.Entities.Students.Parent, PersonDto>();
            CreateMap<SchoolWebApp.Core.Entities.Staff.StaffDetails, PersonDto>();

            CreateMap<PersonDto, SchoolWebApp.Core.Entities.Students.Student>();
            CreateMap<PersonDto, SchoolWebApp.Core.Entities.Students.Parent>();
            CreateMap<PersonDto, SchoolWebApp.Core.Entities.Staff.StaffDetails>();
        }
    }
}
