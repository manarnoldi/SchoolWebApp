using AutoMapper;
using SchoolWebApp.Core.DTOs.Settings.Outcome;
using SchoolWebApp.Core.Entities.Settings;

namespace SchoolWebApp.Core.Profiles.Settings
{
    public class OutcomeProfile: Profile
    {
        public OutcomeProfile()
        {
            CreateMap<Outcome, OutcomeDto>();
            CreateMap<OutcomeDto, Outcome>();
            CreateMap<CreateOutcomeDto, Outcome>();
            CreateMap<CreateOutcomeDto, OutcomeDto>();
        }
    }
}
