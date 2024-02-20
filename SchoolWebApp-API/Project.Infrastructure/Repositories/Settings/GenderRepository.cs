using Project.Infrastructure.Data;
using Project.Infrastructure.Repositories;
using SchoolWebApp.Core.Entities.Settings;
using SchoolWebApp.Core.Interfaces.IRepositories.Settings;

namespace SchoolWebApp.Infrastructure.Repositories.Settings
{
    public class GenderRepository : BaseRepository<Gender>, IGenderRepository
    {
        public GenderRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }
    }
}
