using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Project.Infrastructure.Data;
using Project.Infrastructure.Repositories;
using SchoolWebApp.Core.Entities.Academics;
using SchoolWebApp.Core.Entities.Class;
using SchoolWebApp.Core.Interfaces.IRepositories.Class;

namespace SchoolWebApp.Infrastructure.Repositories.Class
{
    public class SchoolClassLeadersRepository : BaseRepository<SchoolClassLeaders>, ISchoolClassLeadersRepository
    {
        public SchoolClassLeadersRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }
        public async Task<List<SchoolClassLeaders>> GetBySchoolClassId(int schoolClassId)
        {
            var schoolClassLeaders = await _dbContext.SchoolClassLeaders
                .Where(e => e.SchoolClassId == schoolClassId)
                .Include(sc => sc.ClassLeadershipRole).Include(sc => sc.ClassLeadershipRole).Include(sc=>sc.Person).ToListAsync();
            return schoolClassLeaders;
        }
    }
}
