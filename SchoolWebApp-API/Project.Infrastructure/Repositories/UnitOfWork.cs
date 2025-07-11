﻿using Project.Infrastructure.Data;
using SchoolWebApp.Core.Interfaces.IRepositories;
using SchoolWebApp.Core.Interfaces.IRepositories.Academics;
using SchoolWebApp.Core.Interfaces.IRepositories.Class;
using SchoolWebApp.Core.Interfaces.IRepositories.School;
using SchoolWebApp.Core.Interfaces.IRepositories.Settings;
using SchoolWebApp.Core.Interfaces.IRepositories.Staff;
using SchoolWebApp.Core.Interfaces.IRepositories.Students;
using SchoolWebApp.Infrastructure.Repositories.Academics;

namespace SchoolWebApp.Infrastructure.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;

        #region Academics
        public IAcademicYearRepository AcademicYears { get; }
        public ICurriculumRepository Curricula { get; }
        public IExamTypeRepository ExamTypes { get; }
        public IExamNameRepository ExamNames { get; }
        public IExamRepository Exams { get; }
        public IExamResultRepository ExamResults { get; }
        public IGradeRepository Grades { get; }
        public ISubjectGroupRepository SubjectGroups { get; }
        public ISubjectRepository Subjects { get; }
        public IEducationLevelSubjectRepository EducationLevelSubjects { get; }
        #endregion

        #region School
        public ISchoolDetailsRepository SchoolDetails { get; }
        public IDepartmentsRepository Departments { get; }
        public ILearningModesRepository LearningModes { get; }
        public IEventRepository Events { get; }
        public IEducationLevelTypesRepository EducationLevelTypes { get; }
        public IEducationLevelRepository EducationLevels { get; }
        public ISchoolStreamsRepository SchoolStreams { get; }
        public IToDoListRepository ToDoLists { get; }
        #endregion

        #region Staff
        public IStaffDetailsRepository StaffDetails { get; }
        public IStaffAttendanceRepository StaffAttendances { get; }
        public IStaffDisciplineRepository StaffDisciplines { get; }
        public IStaffSubjectRepository StaffSubjects { get; }
        #endregion

        #region Student
        public IParentsRepository Parents { get; }
        public IFormerSchoolsRepository FormerSchools { get; }
        public IStudentsRepository Students { get; }
        public IStudentDisciplineRepository StudentDisciplines { get; }
        public IStudentParentRepository StudentParent { get; }
        public IStudentAttendanceRepository StudentAttendances { get; }
        public IStudentClassRepository StudentClasses { get; }
        public IStudentSubjectRepository StudentSubjects { get; }
        #endregion

        #region Class
        public ISessionRepository Sessions { get; }
        public ILearningLevelRepository LearningLevels { get; }
        public ISchoolClassRepository SchoolClasses { get; }
        public ISchoolClassLeadersRepository SchoolClassLeaders { get; }
        public IClassLeadershipRoleRepository ClassLeadershipRoles { get; }
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
                IExamNameRepository examNameRepository,
                IExamRepository examRepository,
                IExamResultRepository examResultsRepository,
                IGradeRepository gradeRepository,
                ISubjectGroupRepository subjectGroupRepository,
                ISubjectRepository subjectRepository,
                IEducationLevelSubjectRepository educationLevelSubjectRepository,

                //School
                ISchoolDetailsRepository schoolDetailsRepository,
                IDepartmentsRepository departmentsRepository,
                ILearningModesRepository learningModesRepository,
                IEventRepository eventRepository,
                IEducationLevelTypesRepository educationLevelTypesRepository,
                IEducationLevelRepository educationLevelsRepository,
                ISchoolStreamsRepository schoolStreamsRepository,
                IToDoListRepository toDoListRepository,

                //Staff
                IStaffDetailsRepository staffDetailsRepository,
                IStaffAttendanceRepository staffAttendancesRepository,
                IStaffDisciplineRepository staffDisciplinesRepository,
                IStaffSubjectRepository staffSubjectRepository,

                //Student
                IParentsRepository parentsRepository,
                IFormerSchoolsRepository formerSchoolsRepository,
                IStudentsRepository studentsRepository,
                IStudentParentRepository studentParentRepository,
                IStudentDisciplineRepository studentDisciplineRepository,
                IStudentAttendanceRepository studentAttendanceRepository,
                IStudentClassRepository studentClassRepository,
                IStudentSubjectRepository studentSubjectRepository,

                //Class
                ISessionRepository sessionRepository,
                ILearningLevelRepository learningLevelRepository,
                ISchoolClassRepository schoolClassRepository,
                ISchoolClassLeadersRepository schoolClassLeadersRepository,
                IClassLeadershipRoleRepository classLeadershipRoleRepository,

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
            ExamNames = examNameRepository;
            ExamResults = examResultsRepository;
            Exams = examRepository;
            Grades = gradeRepository;
            SubjectGroups = subjectGroupRepository;
            Subjects = subjectRepository;
            EducationLevelSubjects = educationLevelSubjectRepository;
            #endregion

            #region School
            SchoolDetails = schoolDetailsRepository;
            Departments = departmentsRepository;
            LearningModes = learningModesRepository;
            Events = eventRepository;
            EducationLevelTypes = educationLevelTypesRepository;
            EducationLevels = educationLevelsRepository;
            SchoolStreams = schoolStreamsRepository;
            ToDoLists = toDoListRepository;
            #endregion

            #region Staff
            StaffDetails = staffDetailsRepository;
            StaffAttendances = staffAttendancesRepository;
            StaffDisciplines = staffDisciplinesRepository;
            StaffSubjects = staffSubjectRepository;
            #endregion

            #region Student
            Parents = parentsRepository;
            FormerSchools = formerSchoolsRepository;
            Students = studentsRepository;
            StudentParent = studentParentRepository;
            StudentDisciplines = studentDisciplineRepository;
            StudentAttendances = studentAttendanceRepository;
            StudentClasses = studentClassRepository;
            StudentSubjects = studentSubjectRepository;
            #endregion

            #region Class
            Sessions = sessionRepository;
            LearningLevels = learningLevelRepository;
            SchoolClasses = schoolClassRepository;
            SchoolClassLeaders = schoolClassLeadersRepository;
            ClassLeadershipRoles = classLeadershipRoleRepository;
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
