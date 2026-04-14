using AutoMapper;
using SchoolWebApp.Core.DTOs.CBE.Values.ValueScore;
using SchoolWebApp.Core.Entities.CBE.Values;

namespace SchoolWebApp.Core.Profiles.CBE.Values
{
    public class ValueScoreProfile : Profile
    {
        public ValueScoreProfile()
        {
            CreateMap<Entities.CBE.Values.ValueScore, ValueScoreDto>();
            CreateMap<ValueScoreDto, Entities.CBE.Values.ValueScore>();
            CreateMap<CreateValueScoreDto, Entities.CBE.Values.ValueScore>();
            CreateMap<CreateValueScoreDto, ValueScoreDto>();
        }
    }
}
