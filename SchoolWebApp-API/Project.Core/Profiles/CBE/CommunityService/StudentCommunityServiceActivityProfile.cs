using AutoMapper;
using SchoolWebApp.Core.DTOs.CBE.CommunityService.StudentCommunityServiceActivity;
using SchoolWebApp.Core.Entities.CBE.CommunityService;

namespace SchoolWebApp.Core.Profiles.CBE.CommunityService
{
    public class StudentCommunityServiceActivityProfile: Profile
    {
        public StudentCommunityServiceActivityProfile()
        {
            CreateMap<Entities.CBE.CommunityService.StudentCommunityServiceActivity, StudentCommunityServiceActivityDto>();
            CreateMap<StudentCommunityServiceActivityDto, Entities.CBE.CommunityService.StudentCommunityServiceActivity>();
            CreateMap<CreateStudentCommunityServiceActivityDto, Entities.CBE.CommunityService.StudentCommunityServiceActivity>();
            CreateMap<CreateStudentCommunityServiceActivityDto, StudentCommunityServiceActivityDto>();
        }
    }
}
