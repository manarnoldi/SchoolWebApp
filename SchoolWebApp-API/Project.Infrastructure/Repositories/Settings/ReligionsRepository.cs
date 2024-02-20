using Project.Infrastructure.Data;
using Project.Infrastructure.Repositories;
using SchoolWebApp.Core.Entities.Settings;
using SchoolWebApp.Core.Interfaces.IRepositories.Settings;

namespace SchoolWebApp.Infrastructure.Repositories.Settings
{
    public class ReligionsRepository : BaseRepository<Religion>, IReligionsRepository
    {
        public ReligionsRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }
    }
}
