using AutoMapper;
using SchoolWebApp.Core.DTOs.Class.ClassLeadershipRole;
using SchoolWebApp.Core.Entities.Class;

namespace SchoolWebApp.Core.Profiles.Class
{
    public class ClassLeadershipRoleProfile: Profile
    {
        public ClassLeadershipRoleProfile()
        {
            CreateMap<ClassLeadershipRole, ClassLeadershipRoleDto>();
            CreateMap<ClassLeadershipRoleDto, ClassLeadershipRole>();
            CreateMap<CreateClassLeadershipRoleDto, ClassLeadershipRole>();
            CreateMap<CreateClassLeadershipRoleDto, ClassLeadershipRoleDto>();
        }
    }
}
