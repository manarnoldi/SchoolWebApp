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

        public Task<List<Core.Entities.Students.Student>> GetParentStudents(int parentId)
        {
            //var students = await _dbContext.Student.Where(e => e.EmploymentTypeId == employmentTypeId && e.Status == 0).ToListAsync();
            //return staff;
            throw new NotImplementedException();
        }
    }
}
