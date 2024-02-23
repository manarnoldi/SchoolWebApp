using AutoMapper;
using SchoolWebApp.Core.DTOs.Academics.Curriculum;
using SchoolWebApp.Core.Entities.Academics;

namespace SchoolWebApp.Core.Profiles.Academics
{
    public class CurriculumProfile: Profile
    {
        public CurriculumProfile()
        {
            CreateMap<Curriculum, CurriculumDto>();
            CreateMap<CurriculumDto, Curriculum>();
            CreateMap<CreateCurriculumDto, Curriculum>();
            CreateMap<CreateCurriculumDto, CurriculumDto>();
        }
    }
}
