using AutoMapper;
using SchoolWebApp.Core.DTOs.CBE.Cocurriculum.StudentCoCurriculumScore;
using SchoolWebApp.Core.Entities.CBE.Cocurriculum;

namespace SchoolWebApp.Core.Profiles.CBE.Cocurriculum
{
    public class StudentCoCurriculumScoreProfile : Profile
    {
        public StudentCoCurriculumScoreProfile()
        {
            CreateMap<Entities.CBE.Cocurriculum.StudentCoCurriculumScore, StudentCoCurriculumScoreDto>();
            CreateMap<StudentCoCurriculumScoreDto, Entities.CBE.Cocurriculum.StudentCoCurriculumScore>();
            CreateMap<CreateStudentCoCurriculumScoreDto, Entities.CBE.Cocurriculum.StudentCoCurriculumScore>();
            CreateMap<CreateStudentCoCurriculumScoreDto, StudentCoCurriculumScoreDto>();
        }
    }
}
