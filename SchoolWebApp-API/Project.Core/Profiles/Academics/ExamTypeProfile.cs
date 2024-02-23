using AutoMapper;
using SchoolWebApp.Core.DTOs.Academics.ExamType;
using SchoolWebApp.Core.Entities.Academics;

namespace SchoolWebApp.Core.Profiles.Academics
{
    public class ExamTypeProfile: Profile
    {
        public ExamTypeProfile()
        {
            CreateMap<ExamType, ExamTypeDto>();
            CreateMap<ExamTypeDto, ExamType>();
            CreateMap<CreateExamTypeDto, ExamType>();
            CreateMap<CreateExamTypeDto, ExamTypeDto>();
        }
    }
}
