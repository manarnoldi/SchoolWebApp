using Project.Core.Interfaces.IRepositories;
using SchoolWebApp.Core.Entities.Shared;
using SchoolWebApp.Core.Interfaces.IRepositories.Academics;
using SchoolWebApp.Core.Interfaces.IRepositories.Class;
using SchoolWebApp.Core.Interfaces.IRepositories.School;
using SchoolWebApp.Core.Interfaces.IRepositories.Settings;
using SchoolWebApp.Core.Interfaces.IRepositories.Staff;
using SchoolWebApp.Core.Interfaces.IRepositories.Students;

namespace SchoolWebApp.Core.Interfaces.IRepositories
{
    public interface IUnitOfWork
    {
        #region School
        ISchoolDetailsRepository SchoolDetails { get; }
        IDepartmentsRepository Departments { get; }
        ILearningModesRepository LearningModes { get; }
        IEventRepository Events { get; }
        IEducationLevelTypesRepository EducationLevelTypes { get; }
        IEducationLevelRepository EducationLevels { get; }
        ISchoolStreamsRepository SchoolStreams { get; }
        IToDoListRepository ToDoLists { get; }
        #endregion

        #region Staff
        IStaffDetailsRepository StaffDetails { get; }
        IStaffAttendanceRepository StaffAttendances { get; }
        IStaffDisciplineRepository StaffDisciplines { get; }
        IStaffSubjectRepository StaffSubjects { get; }
        #endregion

        #region Student
        IFormerSchoolsRepository FormerSchools { get; }
        IParentsRepository Parents { get; }
        IStudentsRepository Students { get; }
        IStudentParentRepository StudentParent { get; }
        IStudentDisciplineRepository StudentDisciplines { get; }
        IStudentAttendanceRepository StudentAttendances { get; }
        IStudentClassRepository StudentClasses { get; }
        IStudentSubjectRepository StudentSubjects { get; }

        #endregion

        #region Academics
        IAcademicYearRepository AcademicYears { get; }
        ICurriculumRepository Curricula { get; }
        IGradeRepository Grades { get; }
        ISubjectGroupRepository SubjectGroups { get; }
        ISubjectRepository Subjects { get; }
        IEducationLevelSubjectRepository EducationLevelSubjects { get; }
        #endregion

        #region CBC Assessments
        #endregion

        #region CBC Exams
        IExamTypeRepository ExamTypes { get; }
        IExamRepository Exams { get; }
        IExamResultRepository ExamResults { get; }
        #endregion

        #region CBC CoCurriculum
        #endregion

        #region CBC Values
        #endregion

        #region CBC Responsibilities
        #endregion

        #region Class
        ISessionRepository Sessions { get; }
        ILearningLevelRepository LearningLevels { get; }
        ISchoolClassRepository SchoolClasses { get; }
        ISchoolClassLeadersRepository SchoolClassLeaders { get; }
        IClassLeadershipRoleRepository ClassLeadershipRoles { get; }
        #endregion

        #region Settings
        IDesignationsRepository Designations { get; }
        IEmploymentTypesRepository EmploymentTypes { get; }
        IGenderRepository Genders { get; }
        INationalityRepository Nationalities { get; }
        IOccupationsRepository Occupations { get; }
        IOccurenceTypesRepository OccurenceTypes { get; }
        IOutcomesRepository Outcomes { get; }
        IRelationshipsRepository Relationships { get; }
        IReligionsRepository Religions { get; }
        ISessionTypesRepository SessionTypes { get; }
        IStaffCategoryRepository StaffCategories { get; }
        #endregion

        // new generic repo accessor
        IBaseRepository<T> Repository<T>() where T : Base;

        Task<int> SaveChangesAsync();
    }
}
