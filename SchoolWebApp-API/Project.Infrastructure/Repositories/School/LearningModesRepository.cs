using Project.Infrastructure.Data;
using Project.Infrastructure.Repositories;
using SchoolWebApp.Core.Entities.School;
using SchoolWebApp.Core.Interfaces.IRepositories.School;

namespace SchoolWebApp.Infrastructure.Repositories.School
{
    public class LearningModesRepository : BaseRepository<LearningMode>, ILearningModesRepository
    {
        public LearningModesRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }
    }
}
