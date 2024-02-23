using AutoMapper;
using SchoolWebApp.Core.DTOs.Academics.Grade;
using SchoolWebApp.Core.Entities.Academics;

namespace SchoolWebApp.Core.Profiles.Academics
{
    public class GradeProfile : Profile
    {
        public GradeProfile()
        {
            CreateMap<Grade, GradeDto>();
            CreateMap<GradeDto, Grade>();
            CreateMap<CreateGradeDto, Grade>();
            CreateMap<CreateGradeDto, GradeDto>();
        }
    }
}
