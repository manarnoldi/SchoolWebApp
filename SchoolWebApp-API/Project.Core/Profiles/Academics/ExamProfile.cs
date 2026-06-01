using AutoMapper;
using SchoolWebApp.Core.DTOs.Academics.Exam;
using SchoolWebApp.Core.Entities.CBE.Exams;

namespace SchoolWebApp.Core.Profiles.Academics
{
    public class ExamProfile: Profile
    {
        public ExamProfile()
        {
            // Flatten the header (SchoolExam) onto the ExamDto so existing
            // consumers keep reading examType/session/dates off the exam.
            CreateMap<Exam, ExamDto>()
                .ForMember(d => d.ExamStartDate, o => o.MapFrom(s => s.SchoolExam == null ? default : s.SchoolExam.ExamStartDate))
                .ForMember(d => d.ExamEndDate, o => o.MapFrom(s => s.SchoolExam == null ? default : s.SchoolExam.ExamEndDate))
                .ForMember(d => d.ExamMarkEntryEndDate, o => o.MapFrom(s => s.SchoolExam == null ? default : s.SchoolExam.ExamMarkEntryEndDate))
                .ForMember(d => d.ExamTypeId, o => o.MapFrom(s => s.SchoolExam == null ? 0 : s.SchoolExam.ExamTypeId))
                .ForMember(d => d.SessionId, o => o.MapFrom(s => s.SchoolExam == null ? 0 : s.SchoolExam.SessionId))
                .ForMember(d => d.ExamType, o => o.MapFrom(s => s.SchoolExam == null ? null : s.SchoolExam.ExamType))
                .ForMember(d => d.Session, o => o.MapFrom(s => s.SchoolExam == null ? null : s.SchoolExam.Session));
            CreateMap<ExamDto, Exam>();
            CreateMap<CreateExamDto, Exam>();
            CreateMap<CreateExamDto, ExamDto>();
        }
    }
}
