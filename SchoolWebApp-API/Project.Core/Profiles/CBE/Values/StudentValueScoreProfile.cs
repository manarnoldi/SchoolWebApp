using AutoMapper;
using SchoolWebApp.Core.DTOs.CBE.Values.StudentValueScore;
using SchoolWebApp.Core.Entities.CBE.Values;

namespace SchoolWebApp.Core.Profiles.CBE.Values
{
    public class StudentValueScoreProfile : Profile
    {
        public StudentValueScoreProfile()
        {
            CreateMap<Entities.CBE.Values.StudentValueScore, StudentValueScoreDto>();
            CreateMap<StudentValueScoreDto, Entities.CBE.Values.StudentValueScore>();
            CreateMap<CreateStudentValueScoreDto, Entities.CBE.Values.StudentValueScore>();
            CreateMap<CreateStudentValueScoreDto, StudentValueScoreDto>();
        }
    }
}
