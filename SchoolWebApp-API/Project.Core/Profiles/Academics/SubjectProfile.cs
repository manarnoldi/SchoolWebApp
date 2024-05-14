using AutoMapper;
using SchoolWebApp.Core.DTOs.Academics.Subject;
using SchoolWebApp.Core.Entities.Academics;

namespace SchoolWebApp.Core.Profiles.Academics
{
    public class SubjectProfile : Profile
    {
        public SubjectProfile()
        {
            CreateMap<Subject, SubjectDto>();
            CreateMap<SubjectDto, Subject>();
            CreateMap<CreateSubjectDto, Subject>();
            CreateMap<CreateSubjectDto, SubjectDto>();
        }
    }
}
