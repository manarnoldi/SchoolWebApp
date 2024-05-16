using AutoMapper;
using SchoolWebApp.Core.DTOs.School.SchoolStream;
using SchoolWebApp.Core.Entities.School;

namespace SchoolWebApp.Core.Profiles.School
{
    public class SchoolStreamProfile : Profile
    {
        public SchoolStreamProfile()
        {
            CreateMap<SchoolStream, SchoolStreamDto>();
            CreateMap<SchoolStreamDto, SchoolStream>();
            CreateMap<CreateSchoolStreamDto, SchoolStream>();
            CreateMap<CreateSchoolStreamDto, SchoolStreamDto>();
        }
    }
}