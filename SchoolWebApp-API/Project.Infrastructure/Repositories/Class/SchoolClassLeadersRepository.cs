using Project.Infrastructure.Data;
using Project.Infrastructure.Repositories;
using SchoolWebApp.Core.Entities.Class;
using SchoolWebApp.Core.Interfaces.IRepositories.Class;

namespace SchoolWebApp.Infrastructure.Repositories.Class
{
    public class SchoolClassLeadersRepository : BaseRepository<SchoolClassLeaders>, ISchoolClassLeadersRepository
    {
        public SchoolClassLeadersRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }
    }
}
