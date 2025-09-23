using AutoMapper;
using SchoolWebApp.Core.DTOs.Academics.Exam;
using SchoolWebApp.Core.DTOs.Academics.ExamResult;
using SchoolWebApp.Core.Entities.CBE.Exams;

namespace SchoolWebApp.Core.Profiles.Academics
{
    public class ExamResultProfile : Profile
    {
        public ExamResultProfile()
        {
            CreateMap<ExamResult, ExamResultDto>();
            CreateMap<ExamResultDto, ExamResult>();
            CreateMap<CreateExamResultDto, ExamResult>();
            CreateMap<CreateExamResultDto, ExamResultDto>();
        }
    }
}
