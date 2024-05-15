using Project.Infrastructure.Data;
using Project.Infrastructure.Repositories;
using SchoolWebApp.Core.Entities.School;
using SchoolWebApp.Core.Interfaces.IRepositories.School;

namespace SchoolWebApp.Infrastructure.Repositories.School
{
    public class EducationLevelTypesRepository : BaseRepository<EducationLevelType>, IEducationLevelTypesRepository
    {
        public EducationLevelTypesRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }
    }
}
