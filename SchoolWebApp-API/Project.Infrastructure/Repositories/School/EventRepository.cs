using Microsoft.EntityFrameworkCore;
using Project.Infrastructure.Data;
using Project.Infrastructure.Repositories;
using SchoolWebApp.Core.DTOs.School.Event;
using SchoolWebApp.Core.Entities.Class;
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
            var events = await _dbContext.Events.Include(e => e.Session)
                .Where(e => e.SessionId == sessionId).ToListAsync();
            return events;
        }

        public async Task<List<Event>> GetByAcademicYearId(int? academicYearId)
        {
            var query = _dbContext.Events.Include(e => e.Session).AsQueryable();

            if (academicYearId != null)
                query = query.Where(s => s.Session.AcademicYearId == academicYearId);

            var events = await query.ToListAsync();
            return events;
        }
    }
}
