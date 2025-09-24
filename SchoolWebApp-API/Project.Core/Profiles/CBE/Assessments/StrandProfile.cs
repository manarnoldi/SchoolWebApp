using AutoMapper;
using SchoolWebApp.Core.DTOs.CBE.Assessments.Strand;
using SchoolWebApp.Core.Entities.CBE.Assessments;

namespace SchoolWebApp.Core.Profiles.CBE.Assessments
{
    public class StrandProfile:Profile
    {
        public StrandProfile()
        {
            CreateMap<Strand, StrandDto>();
            CreateMap<StrandDto, Strand>();
            CreateMap<CreateStrandDto, Strand>();
            CreateMap<CreateStrandDto, StrandDto>();
        }
    }
}
