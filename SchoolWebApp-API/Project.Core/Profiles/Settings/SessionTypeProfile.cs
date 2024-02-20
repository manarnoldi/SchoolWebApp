using AutoMapper;
using SchoolWebApp.Core.DTOs.Settings.SessionType;
using SchoolWebApp.Core.Entities.Settings;

namespace SchoolWebApp.Core.Profiles.Settings
{
    public class SessionTypeProfile: Profile
    {
        public SessionTypeProfile()
        {
            CreateMap<SessionType, SessionTypeDto>();
            CreateMap<SessionTypeDto, SessionType>();
            CreateMap<CreateSessionTypeDto, SessionType>();
            CreateMap<CreateSessionTypeDto, SessionTypeDto>();
        }
    }
}
