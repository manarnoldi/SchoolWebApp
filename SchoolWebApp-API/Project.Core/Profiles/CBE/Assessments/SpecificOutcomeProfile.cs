using AutoMapper;
using SchoolWebApp.Core.DTOs.CBE.Assessments.SpecificOutcome;
using SchoolWebApp.Core.Entities.CBE.Assessments;

namespace SchoolWebApp.Core.Profiles.CBE.Assessments
{
    public class SpecificOutcomeProfile : Profile
    {
        public SpecificOutcomeProfile()
        {
            CreateMap<SpecificOutcome, SpecificOutcomeDto>();
            CreateMap<SpecificOutcomeDto, SpecificOutcome>();
            CreateMap<CreateSpecificOutcomeDto, SpecificOutcome>();
            CreateMap<CreateSpecificOutcomeDto, SpecificOutcomeDto>();
        }
    }
}
