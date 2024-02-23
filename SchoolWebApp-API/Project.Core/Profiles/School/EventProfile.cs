using AutoMapper;
using SchoolWebApp.Core.DTOs.School.Event;
using SchoolWebApp.Core.Entities.School;

namespace SchoolWebApp.Core.Profiles.School
{
    public class EventProfile : Profile
    {
        public EventProfile()
        {
            CreateMap<Event, EventDto>();
            CreateMap<EventDto, Event>();
            CreateMap<CreateEventDto, Event>();
            CreateMap<CreateEventDto, EventDto>();
        }
    }
}
