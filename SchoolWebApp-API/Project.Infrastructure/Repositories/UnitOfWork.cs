using Project.Infrastructure.Data;
using SchoolWebApp.Core.Interfaces.IRepositories;
using SchoolWebApp.Core.Interfaces.IRepositories.School;
using SchoolWebApp.Core.Interfaces.IRepositories.Settings;
using SchoolWebApp.Infrastructure.Repositories.School;

namespace SchoolWebApp.Infrastructure.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;

        #region School
        public ISchoolDetailsRepository SchoolDetails { get; }
        public IDepartmentsRepository Departments { get; }
        #endregion

        #region Settings
        public IDesignationsRepository Designations { get; }
        public IEmploymentTypesRepository EmploymentTypes { get; }
        public IGenderRepository Genders { get; }
        public INationalityRepository Nationalities { get; }
        public IOccupationsRepository Occupations { get; }
        public IOccurenceTypesRepository OccurenceTypes { get; }
        public IOutcomesRepository Outcomes { get; }
        public IRelationshipsRepository Relationships { get; }
        public IReligionsRepository Religions { get; }
        public ISchoolLevelsRepository SchoolLevels { get; }
        public ISessionTypesRepository SessionTypes { get; }
        public IStaffCategoryRepository StaffCategories { get; }
        #endregion
        public UnitOfWork(ApplicationDbContext context,
                ISchoolDetailsRepository schoolDetailsRepository,
                IDepartmentsRepository departmentsRepository,
                IDesignationsRepository designationsRepository,
                IEmploymentTypesRepository employmentTypesRepository,
                IGenderRepository genderRepository,
                INationalityRepository nationalityRepository,
                IOccupationsRepository occupationsRepository,
                IOccurenceTypesRepository occurenceTypesRepository,
                IOutcomesRepository outcomesRepository,
                IRelationshipsRepository relationshipsRepository,
                IReligionsRepository religionsRepository,
                ISchoolLevelsRepository schoolLevelsRepository,
                ISessionTypesRepository sessionTypesRepository,
                IStaffCategoryRepository staffCategoryRepository
            )
        {
            _context = context;

            #region School
            SchoolDetails = schoolDetailsRepository;
            Departments = departmentsRepository;
            #endregion

            #region Settings
            Designations = designationsRepository;
            EmploymentTypes = employmentTypesRepository;
            Genders = genderRepository;
            Nationalities = nationalityRepository;
            Occupations = occupationsRepository;
            OccurenceTypes = occurenceTypesRepository;
            Outcomes = outcomesRepository;
            Relationships = relationshipsRepository;
            Religions = religionsRepository;
            SchoolLevels = schoolLevelsRepository;
            SessionTypes = sessionTypesRepository;
            StaffCategories = staffCategoryRepository;
            #endregion
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
