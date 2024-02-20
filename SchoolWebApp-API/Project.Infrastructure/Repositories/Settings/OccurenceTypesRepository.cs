using Project.Infrastructure.Data;
using Project.Infrastructure.Repositories;
using SchoolWebApp.Core.Entities.Settings;
using SchoolWebApp.Core.Interfaces.IRepositories.Settings;

namespace SchoolWebApp.Infrastructure.Repositories.Settings
{
    public class OccurenceTypesRepository : BaseRepository<OccurenceType>, IOccurenceTypesRepository
    {
        public OccurenceTypesRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }
    }
}
