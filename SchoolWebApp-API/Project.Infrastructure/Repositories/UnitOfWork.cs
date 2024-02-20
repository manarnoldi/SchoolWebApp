using Project.Infrastructure.Data;
using SchoolWebApp.Core.Interfaces.IRepositories;
using SchoolWebApp.Core.Interfaces.IRepositories.School;
using SchoolWebApp.Infrastructure.Repositories.School;

namespace SchoolWebApp.Infrastructure.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;

        //School
        public ISchoolDetailsRepository SchoolDetails { get; }
        public IDepartmentsRepository Departments { get; }
        public UnitOfWork(ApplicationDbContext context,
            ISchoolDetailsRepository schoolDetailsRepository,
            IDepartmentsRepository departmentsRepository)
        {
            _context = context;

            //School
            SchoolDetails = schoolDetailsRepository;
            Departments = departmentsRepository;
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                _context.Dispose();
            }
        }
    }
}
