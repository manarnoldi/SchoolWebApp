using AutoMapper;
using SchoolWebApp.Core.DTOs.Academics.ExamName;
using SchoolWebApp.Core.Entities.Academics;

namespace SchoolWebApp.Core.Profiles.Academics
{
    public class ExamNameProfile : Profile
    {
        public ExamNameProfile()
        {
            CreateMap<ExamName, ExamNameDto>();
            CreateMap<ExamNameDto, ExamName>();
            CreateMap<CreateExamNameDto, ExamName>();
            CreateMap<CreateExamNameDto, ExamNameDto>();
        }
    }
}
