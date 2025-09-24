using Project.Infrastructure.Data;
using Project.Infrastructure.Repositories;
using SchoolWebApp.Core.Entities.CBE.Assessments;
using SchoolWebApp.Core.Interfaces.IRepositories.CBE.Assessments;

namespace SchoolWebApp.Infrastructure.Repositories.CBE.Assessments
{
    public class AssessmentRepository : BaseRepository<Assessment>, IAssessmentRepository
    {
        public AssessmentRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }

    }
}
