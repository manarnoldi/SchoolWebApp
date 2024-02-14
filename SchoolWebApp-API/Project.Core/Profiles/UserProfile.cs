using AutoMapper;
using SchoolWebApp.Core.Entities.Identity;

namespace SchoolWebApp.Core.Profiles
{
    public class UserProfile : Profile
    {
        public UserProfile()
        {
            CreateMap<AppUser, UserDTO>();
            CreateMap<AppRole, RoleDTO>();
            CreateMap<AppUser, UserToReturn>();
            CreateMap<AppRole, RoleToReturn>();
        }
    }
}