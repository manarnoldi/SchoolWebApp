using AutoMapper;
using SchoolWebApp.Core.DTOs.School.SchoolDetails;
using SchoolWebApp.Core.Entities.School;

namespace SchoolWebApp.Core.Profiles.School
{
    public class SchoolDetailsProfile: Profile
    {
        public SchoolDetailsProfile()
        {
            CreateMap<SchoolDetails, SchoolDetailsDto>();
            CreateMap<SchoolDetailsDto, SchoolDetails>();
            CreateMap<CreateSchoolDetailsDto, SchoolDetails>();
            CreateMap<CreateSchoolDetailsDto, SchoolDetailsDto>();
        }        
    }
}
