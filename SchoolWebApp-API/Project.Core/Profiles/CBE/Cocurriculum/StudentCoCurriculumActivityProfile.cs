using AutoMapper;
using SchoolWebApp.Core.DTOs.CBE.Cocurriculum.StudentCoCurriculumActivity;
using SchoolWebApp.Core.Entities.CBE.Cocurriculum;

namespace SchoolWebApp.Core.Profiles.CBE.Cocurriculum
{
    public class StudentCoCurriculumActivityProfile : Profile
    {
        public StudentCoCurriculumActivityProfile()
        {
            CreateMap<Entities.CBE.Cocurriculum.StudentCoCurriculumActivity, StudentCoCurriculumActivityDto>();
            CreateMap<StudentCoCurriculumActivityDto, Entities.CBE.Cocurriculum.StudentCoCurriculumActivity>();
            CreateMap<CreateStudentCoCurriculumActivityDto, Entities.CBE.Cocurriculum.StudentCoCurriculumActivity>();
            CreateMap<CreateStudentCoCurriculumActivityDto, StudentCoCurriculumActivityDto>();
        }
    }
}
