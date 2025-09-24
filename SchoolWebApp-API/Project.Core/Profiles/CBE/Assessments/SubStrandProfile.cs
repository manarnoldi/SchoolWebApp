using AutoMapper;
using SchoolWebApp.Core.DTOs.CBE.Assessments.SubStrand;
using SchoolWebApp.Core.Entities.CBE.Assessments;

namespace SchoolWebApp.Core.Profiles.CBE.Assessments
{
    public class SubStrandProfile: Profile
    {
        public SubStrandProfile()
        {
            CreateMap<SubStrand, SubStrandDto>();
            CreateMap<SubStrandDto, SubStrand>();
            CreateMap<CreateSubStrandDto, SubStrand>();
            CreateMap<CreateSubStrandDto, SubStrandDto>();
        }
    }
}
