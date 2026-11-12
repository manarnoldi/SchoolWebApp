using Microsoft.EntityFrameworkCore;
using Project.Infrastructure.Data;
using SchoolWebApp.Core.Entities.Class;

namespace SchoolWebApp.API.Services
{
    /// <summary>
    /// The term the school is working in: the latest term marked open (School >
    /// Sessions), so a school that has opened next year sees next year even in
    /// the holiday. Only when no term is open does the calendar decide - the
    /// term running today, else the last one that has started, else the first.
    /// Dashboard cards and the term pickers all use this, so they agree.
    /// </summary>
    public static class CurrentTerm
    {
        public static Session? Pick(IEnumerable<Session> sessions)
        {
            var list = sessions.ToList();
            var today = DateTime.Today;
            return list.Where(s => s.Status).OrderByDescending(s => s.StartDate).FirstOrDefault()
                ?? list.Where(s => s.StartDate.Date <= today && today <= s.EndDate.Date).OrderByDescending(s => s.StartDate).FirstOrDefault()
                ?? list.Where(s => s.StartDate.Date <= today).OrderByDescending(s => s.StartDate).FirstOrDefault()
                ?? list.OrderBy(s => s.StartDate).FirstOrDefault();
        }

        public static async Task<Session?> GetAsync(ApplicationDbContext db) =>
            Pick(await db.Sessions.AsNoTracking().Include(s => s.AcademicYear).ToListAsync());
    }
}
