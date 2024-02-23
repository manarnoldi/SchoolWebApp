using Project.Infrastructure.Data;
using Project.Infrastructure.Repositories;
using SchoolWebApp.Core.Entities.Students;
using SchoolWebApp.Core.Interfaces.IRepositories.Students;

namespace SchoolWebApp.Infrastructure.Repositories.Students
{
    public class ParentsRepository : BaseRepository<Parent>, IParentsRepository
    {
        public ParentsRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }
    }
}
