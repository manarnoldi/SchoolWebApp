using AutoMapper;
using SchoolWebApp.Core.DTOs.CBE.Assessments.Competency;
using SchoolWebApp.Core.Entities.CBE.Assessments;

namespace SchoolWebApp.Core.Profiles.CBE.Assessments
{
    public class CompetencyProfile: Profile
    {
        public CompetencyProfile()
        {
            CreateMap<Competency, CompetencyDto>();
            CreateMap<CompetencyDto, Competency>();
            CreateMap<CreateCompetencyDto, Competency>();
            CreateMap<CreateCompetencyDto, CompetencyDto>();
        }
    }
}
