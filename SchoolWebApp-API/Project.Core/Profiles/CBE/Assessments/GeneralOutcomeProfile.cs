using AutoMapper;
using SchoolWebApp.Core.DTOs.CBE.Assessments.GeneralOutcome;
using SchoolWebApp.Core.Entities.CBE.Assessments;

namespace SchoolWebApp.Core.Profiles.CBE.Assessments
{
    public class GeneralOutcomeProfile : Profile
    {
        public GeneralOutcomeProfile()
        {
            CreateMap<GeneralOutcome, GeneralOutcomeDto>();
            CreateMap<GeneralOutcomeDto, GeneralOutcome>();
            CreateMap<CreateGeneralOutcomeDto, GeneralOutcome>();
            CreateMap<CreateGeneralOutcomeDto, GeneralOutcomeDto>();
        }
    }
}
