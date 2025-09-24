using Project.Infrastructure.Data;
using Project.Infrastructure.Repositories;
using SchoolWebApp.Core.Entities.CBE.Assessments;
using SchoolWebApp.Core.Interfaces.IRepositories.CBE.Assessments;

namespace SchoolWebApp.Infrastructure.Repositories.CBE.Assessments
{
    public class AssessmentTypeRepository : BaseRepository<AssessmentType>, IAssessmentTypeRepository
    {
        public AssessmentTypeRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }
    
    }
}
