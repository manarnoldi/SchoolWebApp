using AutoMapper;
using SchoolWebApp.Core.DTOs.CBE.Assessments.BroadOutcome;
using SchoolWebApp.Core.Entities.CBE.Assessments;

namespace SchoolWebApp.Core.Profiles.CBE.Assessments
{
    public class BroadOutcomeProfile: Profile
    {
        public BroadOutcomeProfile()
        {
            CreateMap<SubjectOutcome, BroadOutcomeDto>();
            CreateMap<BroadOutcomeDto, SubjectOutcome>();
            CreateMap<CreateBroadOutcomeDto, SubjectOutcome>();
            CreateMap<CreateBroadOutcomeDto, BroadOutcomeDto>(); 
        }
    }
}
