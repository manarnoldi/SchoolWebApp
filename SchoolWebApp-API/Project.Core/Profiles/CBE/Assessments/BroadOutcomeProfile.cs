using AutoMapper;
using SchoolWebApp.Core.DTOs.CBE.Assessments.BroadOutcome;
using SchoolWebApp.Core.Entities.CBE.Assessments;

namespace SchoolWebApp.Core.Profiles.CBE.Assessments
{
    public class BroadOutcomeProfile: Profile
    {
        public BroadOutcomeProfile()
        {
            CreateMap<BroadOutcome, BroadOutcomeDto>();
            CreateMap<BroadOutcomeDto, BroadOutcome>();
            CreateMap<CreateBroadOutcomeDto, BroadOutcome>();
            CreateMap<CreateBroadOutcomeDto, BroadOutcomeDto>(); 
        }
    }
}
