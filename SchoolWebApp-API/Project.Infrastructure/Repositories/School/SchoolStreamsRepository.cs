using Project.Infrastructure.Data;
using Project.Infrastructure.Repositories;
using SchoolWebApp.Core.Entities.School;
using SchoolWebApp.Core.Interfaces.IRepositories.School;

namespace SchoolWebApp.Infrastructure.Repositories.School
{
    public class SchoolStreamsRepository : BaseRepository<SchoolStream>, ISchoolStreamsRepository
    {
        public SchoolStreamsRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }
    }
}
