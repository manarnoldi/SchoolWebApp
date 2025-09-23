using AutoMapper;
using SchoolWebApp.Core.DTOs.Academics.Exam;
using SchoolWebApp.Core.Entities.CBE.Exams;

namespace SchoolWebApp.Core.Profiles.Academics
{
    public class ExamProfile: Profile
    {
        public ExamProfile()
        {
            CreateMap<Exam, ExamDto>();
            CreateMap<ExamDto, Exam>();
            CreateMap<CreateExamDto, Exam>();
            CreateMap<CreateExamDto, ExamDto>();
        }
    }
}
