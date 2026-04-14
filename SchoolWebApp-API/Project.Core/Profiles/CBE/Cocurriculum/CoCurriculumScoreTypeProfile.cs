using AutoMapper;
using SchoolWebApp.Core.DTOs.CBE.Cocurriculum.CoCurriculumScoreType;

namespace SchoolWebApp.Core.Profiles.CBE.Cocurriculum
{
    public class CoCurriculumScoreTypeProfile : Profile
    {
        public CoCurriculumScoreTypeProfile()
        {
            CreateMap<Entities.CBE.Cocurriculum.CoCurriculumScoreType, CoCurriculumScoreTypeDto>();
            CreateMap<CoCurriculumScoreTypeDto, Entities.CBE.Cocurriculum.CoCurriculumScoreType>();
            CreateMap<CreateCoCurriculumScoreTypeDto, Entities.CBE.Cocurriculum.CoCurriculumScoreType>();
            CreateMap<CreateCoCurriculumScoreTypeDto, CoCurriculumScoreTypeDto>();
        }
    }
}
