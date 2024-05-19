using Project.Infrastructure.Data;
using SchoolWebApp.Core.Interfaces.IRepositories;
using SchoolWebApp.Core.Interfaces.IRepositories.Academics;
using SchoolWebApp.Core.Interfaces.IRepositories.Class;
using SchoolWebApp.Core.Interfaces.IRepositories.School;
using SchoolWebApp.Core.Interfaces.IRepositories.Settings;
using SchoolWebApp.Core.Interfaces.IRepositories.Staff;
using SchoolWebApp.Core.Interfaces.IRepositories.Students;

namespace SchoolWebApp.Infrastructure.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;

        #region Academics
        public IAcademicYearRepository AcademicYears { get; }
        public ICurriculumRepository Curricula { get; }
        public IExamTypeRepository ExamTypes { get; }
        public IExamRepository Exams { get; }
        public IGradeRepository Grades { get; }
        public ISubjectGroupRepository SubjectGroups { get; }
        public ISubjectRepository Subjects { get; }
        #endregion

        #region School
        public ISchoolDetailsRepository SchoolDetails { get; }
        public IDepartmentsRepository Departments { get; }
        public ILearningModesRepository LearningModes { get; }
        public IEventRepository Events { get; }
        public IEducationLevelTypesRepository EducationLevelTypes { get; }
        public IEducationLevelRepository EducationLevels { get; }
        public ISchoolStreamsRepository SchoolStreams { get; }
        #endregion

        #region Staff
        public IStaffDetailsRepository StaffDetails { get; }
        public IStaffAttendanceRepository StaffAttendances { get; }
        public IStaffDisciplineRepository StaffDisciplines { get; }
        #endregion

        #region Student
        public IParentsRepository Parents { get; }
        public IFormerSchoolsRepository FormerSchools { get; }
        public IStudentsRepository Students { get; }
        public IStudentDisciplineRepository StudentDisciplines { get; }
        public IStudentParentRepository StudentParent { get; }
        public IStudentAttendanceRepository StudentAttendances { get; }
        #endregion

        #region Class
        public ISessionRepository Sessions { get; }
        public ILearningLevelRepository LearningLevels { get; }
        public ISchoolClassRepository SchoolClasses { get; }
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
        public ISessionTypesRepository SessionTypes { get; }
        public IStaffCategoryRepository StaffCategories { get; }
        #endregion
        public UnitOfWork(ApplicationDbContext context,
                //Academics
                IAcademicYearRepository academicYearRepository,
                ICurriculumRepository curriculumRepository,
                IExamTypeRepository examTypeRepository,
                IExamRepository examRepository,
                IGradeRepository gradeRepository,
                ISubjectGroupRepository subjectGroupRepository,
                ISubjectRepository subjectRepository,

                //School
                ISchoolDetailsRepository schoolDetailsRepository,
                IDepartmentsRepository departmentsRepository,
                ILearningModesRepository learningModesRepository,
                IEventRepository eventRepository,
                IEducationLevelTypesRepository educationLevelTypesRepository,
                IEducationLevelRepository educationLevelsRepository,
                ISchoolStreamsRepository schoolStreamsRepository,

                //Staff
                IStaffDetailsRepository staffDetailsRepository,
                IStaffAttendanceRepository staffAttendancesRepository,
                IStaffDisciplineRepository staffDisciplinesRepository,

                //Student
                IParentsRepository parentsRepository,
                IFormerSchoolsRepository formerSchoolsRepository,
                IStudentsRepository studentsRepository,
                IStudentParentRepository studentParentRepository,
                IStudentDisciplineRepository studentDisciplineRepository,
                IStudentAttendanceRepository studentAttendanceRepository,

                //Class
                ISessionRepository sessionRepository,
                ILearningLevelRepository learningLevelRepository,
                ISchoolClassRepository schoolClassRepository,

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
                ISessionTypesRepository sessionTypesRepository,
                IStaffCategoryRepository staffCategoryRepository
         )
        {
            _context = context;
            #region Academics
            AcademicYears = academicYearRepository;
            Curricula = curriculumRepository;
            ExamTypes = examTypeRepository;
            Exams = examRepository;
            Grades = gradeRepository;
            SubjectGroups = subjectGroupRepository;
            Subjects = subjectRepository;
            #endregion

            #region School
            SchoolDetails = schoolDetailsRepository;
            Departments = departmentsRepository;
            LearningModes = learningModesRepository;
            Events = eventRepository;
            EducationLevelTypes = educationLevelTypesRepository;
            EducationLevels = educationLevelsRepository;
            SchoolStreams = schoolStreamsRepository;
            #endregion

            #region Staff
            StaffDetails = staffDetailsRepository;
            StaffAttendances = staffAttendancesRepository;
            StaffDisciplines = staffDisciplinesRepository;
            #endregion

            #region Student
            Parents = parentsRepository;
            FormerSchools = formerSchoolsRepository;
            Students = studentsRepository;
            StudentParent = studentParentRepository;
            StudentDisciplines = studentDisciplineRepository;
            StudentAttendances = studentAttendanceRepository;
            #endregion

            #region Class
            Sessions = sessionRepository;
            LearningLevels = learningLevelRepository;
            SchoolClasses = schoolClassRepository;
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
