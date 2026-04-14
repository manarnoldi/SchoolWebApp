using AutoMapper;
using SchoolWebApp.Core.DTOs.CBE.Values.Value;

namespace SchoolWebApp.Core.Profiles.CBE.Values
{
    public class ValueProfile : Profile
    {
        public ValueProfile()
        {
            CreateMap<Entities.CBE.Values.Value, ValueDto>();
            CreateMap<ValueDto, Entities.CBE.Values.Value>();
            CreateMap<CreateValueDto, Entities.CBE.Values.Value>();
            CreateMap<CreateValueDto, ValueDto>();
        }
    }
}
