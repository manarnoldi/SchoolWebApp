using AutoMapper;
using SchoolWebApp.Core.DTOs.CBE.Responsibilities.Responsibility;

namespace SchoolWebApp.Core.Profiles.CBE.Responsibilities
{
    public class ResponsibilityProfile : Profile
    {
        public ResponsibilityProfile()
        {
            CreateMap<Entities.CBE.Responsibilities.Responsibility, ResponsibilityDto>();
            CreateMap<ResponsibilityDto, Entities.CBE.Responsibilities.Responsibility>();
            CreateMap<CreateResponsibilityDto, Entities.CBE.Responsibilities.Responsibility>();
            CreateMap<CreateResponsibilityDto, ResponsibilityDto>();
        }
    }
}
