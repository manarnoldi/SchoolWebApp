using Project.Infrastructure.Data;
using Project.Infrastructure.Repositories;
using SchoolWebApp.Core.Entities.Settings;
using SchoolWebApp.Core.Interfaces.IRepositories.Settings;

namespace SchoolWebApp.Infrastructure.Repositories.Settings
{
    public class SchoolLevelsRepository : BaseRepository<SchoolLevel>, ISchoolLevelsRepository
    {
        public SchoolLevelsRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }
    }
}
