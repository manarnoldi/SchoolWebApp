using AutoMapper;
using SchoolWebApp.Core.DTOs.CBE.Cocurriculum.CoCurriculumActivity;

namespace SchoolWebApp.Core.Profiles.CBE.Cocurriculum
{
    public class CoCurriculumActivityProfile : Profile
    {
        public CoCurriculumActivityProfile()
        {
            CreateMap<Entities.CBE.Cocurriculum.CoCurriculumActivity, CoCurriculumActivityDto>();
            CreateMap<CoCurriculumActivityDto, Entities.CBE.Cocurriculum.CoCurriculumActivity>();
            CreateMap<CreateCoCurriculumActivityDto, Entities.CBE.Cocurriculum.CoCurriculumActivity>();
            CreateMap<CreateCoCurriculumActivityDto, CoCurriculumActivityDto>();
        }
    }
}
