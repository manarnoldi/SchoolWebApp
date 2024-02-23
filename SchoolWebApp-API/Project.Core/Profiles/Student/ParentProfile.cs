using AutoMapper;
using SchoolWebApp.Core.DTOs.Student.Parent;
using SchoolWebApp.Core.Entities.Students;

namespace SchoolWebApp.Core.Profiles.Student
{
    public class ParentProfile : Profile
    {
        public ParentProfile()
        {
            CreateMap<Parent, ParentDto>();
            CreateMap<ParentDto, Parent>();
            CreateMap<CreateParentDto, Parent>();
            CreateMap<CreateParentDto, ParentDto>();
        }
    }
}
