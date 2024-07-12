using Project.Infrastructure.Data;
using Project.Infrastructure.Repositories;
using SchoolWebApp.Core.Entities.Class;
using SchoolWebApp.Core.Interfaces.IRepositories.Class;

namespace SchoolWebApp.Infrastructure.Repositories.Class
{
    public class ClassLeadershipRoleRepository : BaseRepository<ClassLeadershipRole>, IClassLeadershipRoleRepository
    {
        public ClassLeadershipRoleRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }
    }
}
