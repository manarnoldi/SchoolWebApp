using AutoMapper;
using SchoolWebApp.Core.DTOs.CBE.Cocurriculum.CoCurriculumScore;
using SchoolWebApp.Core.Entities.CBE.Cocurriculum;

namespace SchoolWebApp.Core.Profiles.CBE.Cocurriculum
{
    public class CoCurriculumScoreProfile : Profile
    {
        public CoCurriculumScoreProfile()
        {
            CreateMap<Entities.CBE.Cocurriculum.CoCurriculumScore, CoCurriculumScoreDto>();
            CreateMap<CoCurriculumScoreDto, Entities.CBE.Cocurriculum.CoCurriculumScore>();
            CreateMap<CreateCoCurriculumScoreDto, Entities.CBE.Cocurriculum.CoCurriculumScore>();
            CreateMap<CreateCoCurriculumScoreDto, CoCurriculumScoreDto>();
        }
    }
}
