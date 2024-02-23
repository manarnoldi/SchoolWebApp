using AutoMapper;
using SchoolWebApp.Core.DTOs.Class.Session;
using SchoolWebApp.Core.Entities.Class;

namespace SchoolWebApp.Core.Profiles.Class
{
    public class SessionProfile: Profile
    {
        public SessionProfile()
        {
            CreateMap<Session, SessionDto>();
            CreateMap<SessionDto, Session>();
            CreateMap<CreateSessionDto, Session>();
            CreateMap<CreateSessionDto, SessionDto>();
        }
    }
}
