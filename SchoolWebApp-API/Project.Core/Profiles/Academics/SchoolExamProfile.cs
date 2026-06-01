using AutoMapper;
using SchoolWebApp.Core.DTOs.Academics.SchoolExam;
using SchoolWebApp.Core.Entities.CBE.Exams;

namespace SchoolWebApp.Core.Profiles.Academics
{
    public class SchoolExamProfile : Profile
    {
        public SchoolExamProfile()
        {
            CreateMap<SchoolExam, SchoolExamDto>();
            CreateMap<SchoolExamDto, SchoolExam>();
            CreateMap<CreateSchoolExamDto, SchoolExam>();
            CreateMap<CreateSchoolExamDto, SchoolExamDto>();
        }
    }
}
