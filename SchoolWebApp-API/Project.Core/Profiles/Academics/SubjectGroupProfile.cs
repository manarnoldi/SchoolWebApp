using AutoMapper;
using SchoolWebApp.Core.DTOs.Academics.SubjectGroup;
using SchoolWebApp.Core.Entities.Academics;

namespace SchoolWebApp.Core.Profiles.Academics
{
    public class SubjectGroupProfile: Profile
    {
        public SubjectGroupProfile()
        {
            CreateMap<SubjectGroup, SubjectGroupDto>();
            CreateMap<SubjectGroupDto, SubjectGroup>();
            CreateMap<CreateSubjectGroupDto, SubjectGroup>();
            CreateMap<CreateSubjectGroupDto, SubjectGroupDto>();
        }
    }
}
