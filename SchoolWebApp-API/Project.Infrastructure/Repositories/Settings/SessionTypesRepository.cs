using Project.Infrastructure.Data;
using Project.Infrastructure.Repositories;
using SchoolWebApp.Core.Entities.Settings;
using SchoolWebApp.Core.Interfaces.IRepositories.Settings;

namespace SchoolWebApp.Infrastructure.Repositories.Settings
{
    public class SessionTypesRepository : BaseRepository<SessionType>, ISessionTypesRepository
    {
        public SessionTypesRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }
    }
}
