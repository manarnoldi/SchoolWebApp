using AutoMapper;
using SchoolWebApp.Core.DTOs.Academics.EducationLevelSubject;
using SchoolWebApp.Core.Entities.Academics;

namespace SchoolWebApp.Core.Profiles.Academics
{
    public class EducationLevelSubjectProfile : Profile
    {
        public EducationLevelSubjectProfile()
        {
            CreateMap<EducationLevelSubject, EducationLevelSubjectDto>();
            CreateMap<EducationLevelSubjectDto, EducationLevelSubject>();
            CreateMap<CreateEducationLevelSubjectDto, EducationLevelSubject>();
            CreateMap<CreateEducationLevelSubjectDto, EducationLevelSubjectDto>();
        }
    }
}
