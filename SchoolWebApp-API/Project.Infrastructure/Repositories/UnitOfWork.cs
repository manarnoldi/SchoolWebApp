using Project.Infrastructure.Data;
using SchoolWebApp.Core.Interfaces.IRepositories;
using SchoolWebApp.Core.Interfaces.IRepositories.Academics;
using SchoolWebApp.Core.Interfaces.IRepositories.Class;
using SchoolWebApp.Core.Interfaces.IRepositories.School;
using SchoolWebApp.Core.Interfaces.IRepositories.Settings;
using SchoolWebApp.Infrastructure.Repositories.School;

namespace SchoolWebApp.Infrastructure.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;

        #region Academics
        public IAcademicYearRepository AcademicYears { get; }
        public ICurriculumRepository Curricula { get; }
        public IExamTypeRepository ExamTypes { get; }
        #endregion

        #region School
        public ISchoolDetailsRepository SchoolDetails { get; }
        public IDepartmentsRepository Departments { get; }
        public ILearningModesRepository LearningModes { get; }
        #endregion

        #region Class
        public ISessionRepository Sessions { get; }
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
                //Academics
                IAcademicYearRepository academicYearRepository,
                ICurriculumRepository curriculumRepository,
                IExamTypeRepository examTypeRepository,

                //School
                ISchoolDetailsRepository schoolDetailsRepository,
                IDepartmentsRepository departmentsRepository,
                ILearningModesRepository learningModesRepository,

                //Class
                ISessionRepository sessionRepository,

                //Settings
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
            #region Academics
            AcademicYears = academicYearRepository;
            Curricula = curriculumRepository;
            ExamTypes = examTypeRepository;
            #endregion

            #region School
            SchoolDetails = schoolDetailsRepository;
            Departments = departmentsRepository;
            LearningModes = learningModesRepository;
            #endregion

            #region Class
            Sessions = sessionRepository;
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
