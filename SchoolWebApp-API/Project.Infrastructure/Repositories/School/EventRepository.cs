﻿using Microsoft.EntityFrameworkCore;
using Project.Infrastructure.Data;
using Project.Infrastructure.Repositories;
using SchoolWebApp.Core.DTOs.School.Event;
using SchoolWebApp.Core.Entities.School;
using SchoolWebApp.Core.Interfaces.IRepositories.School;

namespace SchoolWebApp.Infrastructure.Repositories.School
{
    public class EventRepository : BaseRepository<Event>, IEventRepository
    {
        public EventRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
            
        }

        public async Task<List<Event>> GetBySessionId(int sessionId)
        {
            var events = await _dbContext.Events.Where(e=>e.SessionId == sessionId && e.Status).ToListAsync();
            return events;
        }
    }
}
