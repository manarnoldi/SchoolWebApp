﻿using Microsoft.EntityFrameworkCore;
using Project.Infrastructure.Data;
using Project.Infrastructure.Repositories;
using SchoolWebApp.Core.Entities.Academics;
using SchoolWebApp.Core.Entities.Class;
using SchoolWebApp.Core.Interfaces.IRepositories.Class;

namespace SchoolWebApp.Infrastructure.Repositories.Class
{
    public class SessionRepository : BaseRepository<Session>, ISessionRepository
    {
        public SessionRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }

        public async Task<List<Session>> GetByCurriculumId(int curriculumId)
        {
            var sessions = await _dbContext.Sessions.Where(e => e.CurriculumId == curriculumId).ToListAsync();
            return sessions;
        }

        public async Task<List<Session>> GetByCurriculumIdYearId(int? curriculumId, int? academicYearId)
        {
            var query = _dbContext.Sessions
                .Include(c => c.Curriculum)
                .Include(a => a.AcademicYear)
                .Include(st => st.SessionType)
                .AsQueryable();

            if(curriculumId != null)
                query = query.Where(c=>c.CurriculumId == curriculumId);
            if (academicYearId != null)
                query = query.Where(c => c.AcademicYearId == academicYearId);
           
            var sessions = await query.ToListAsync();
            return sessions;
        }
    }
}