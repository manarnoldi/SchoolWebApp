using Project.Infrastructure.Data;
using Project.Infrastructure.Repositories;
using SchoolWebApp.Core.Entities.CBE.Exams;
using SchoolWebApp.Core.Interfaces.IRepositories.Academics;

namespace SchoolWebApp.Infrastructure.Repositories.Academics
{
    public class ExamTypeRepository : BaseRepository<ExamType>, IExamTypeRepository
    {
        public ExamTypeRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }
    }
}
