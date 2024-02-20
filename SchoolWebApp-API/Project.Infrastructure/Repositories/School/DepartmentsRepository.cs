using Project.Infrastructure.Data;
using Project.Infrastructure.Repositories;
using SchoolWebApp.Core.Entities.School;
using SchoolWebApp.Core.Interfaces.IRepositories.School;

namespace SchoolWebApp.Infrastructure.Repositories.School
{
    public class DepartmentsRepository : BaseRepository<Department>, IDepartmentsRepository
    {
        public DepartmentsRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }
    }
}
