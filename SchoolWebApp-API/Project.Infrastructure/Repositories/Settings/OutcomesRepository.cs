using Project.Infrastructure.Data;
using Project.Infrastructure.Repositories;
using SchoolWebApp.Core.Entities.Settings;
using SchoolWebApp.Core.Interfaces.IRepositories.Settings;

namespace SchoolWebApp.Infrastructure.Repositories.Settings
{
    public class OutcomesRepository : BaseRepository<Outcome>, IOutcomesRepository
    {
        public OutcomesRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }
    }
}
