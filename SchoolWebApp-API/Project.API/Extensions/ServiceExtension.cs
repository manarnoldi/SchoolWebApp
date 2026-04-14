using SchoolWebApp.Core.Interfaces.IRepositories;
using SchoolWebApp.Core.Interfaces.IRepositories.Academics;
//using SchoolWebApp.Core.Interfaces.IRepositories.CBE.Assessments;
using SchoolWebApp.Core.Interfaces.IRepositories.Class;
using SchoolWebApp.Core.Interfaces.IRepositories.School;
using SchoolWebApp.Core.Interfaces.IRepositories.Settings;
using SchoolWebApp.Core.Interfaces.IRepositories.Staff;
using SchoolWebApp.Core.Interfaces.IRepositories.Students;
using SchoolWebApp.Core.Interfaces.IServices;
using SchoolWebApp.Core.Interfaces.IServices.CBE.Assessments;
using SchoolWebApp.Core.Interfaces.IServices.CBE.Cocurriculum;
using SchoolWebApp.Core.Interfaces.IServices.CBE.CommunityService;
using SchoolWebApp.Core.Interfaces.IServices.CBE.Responsibilities;
using SchoolWebApp.Core.Interfaces.IServices.CBE.Values;
using SchoolWebApp.Core.Services.CBE.Assessments;
using SchoolWebApp.Core.Services.CBE.Cocurriculum;
using SchoolWebApp.Core.Services.CBE.CommunityService;
using SchoolWebApp.Core.Services.CBE.Responsibilities;
using SchoolWebApp.Core.Services.CBE.Values;
using SchoolWebApp.Core.Services.Students;
using SchoolWebApp.Infrastructure.Repositories;
using SchoolWebApp.Infrastructure.Repositories.Academics;
//using SchoolWebApp.Infrastructure.Repositories.CBE.Assessments;
using SchoolWebApp.Infrastructure.Repositories.Class;
using SchoolWebApp.Infrastructure.Repositories.School;
using SchoolWebApp.Infrastructure.Repositories.Settings;
using SchoolWebApp.Infrastructure.Repositories.Staff;
using SchoolWebApp.Infrastructure.Repositories.Students;

namespace Project.API.Extensions
{
    public static class ServiceExtension
    {
        public static IServiceCollection RegisterService(this IServiceCollection services)
        {
            #region IUnitOfWork
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            #endregion

            #region Academics Repositories
            services.AddTransient<IAcademicYearRepository, AcademicYearRepository>();
            services.AddTransient<ICurriculumRepository, CurriculumRepository>();
            services.AddTransient<IExamTypeRepository, ExamTypeRepository>();
            services.AddTransient<IExamResultRepository, ExamResultRepository>();
            services.AddTransient<IExamRepository, ExamRepository>();
            services.AddTransient<IGradeRepository, GradeRepository>();
            services.AddTransient<ISubjectGroupRepository, SubjectGroupRepository>();
            services.AddTransient<ISubjectRepository, SubjectRepository>();
            services.AddTransient<IEducationLevelSubjectRepository, EducationLevelSubjectRepository>();
            #endregion

            #region School Repositories
            services.AddTransient<ISchoolDetailsRepository, SchoolDetailsRepository>();
            services.AddTransient<IDepartmentsRepository, DepartmentsRepository>();
            services.AddTransient<ILearningModesRepository, LearningModesRepository>();
            services.AddTransient<IEventRepository, EventRepository>();
            services.AddTransient<IEducationLevelTypesRepository, EducationLevelTypesRepository>();
            services.AddTransient<IEducationLevelRepository, EducationLevelRepository>();
            services.AddTransient<ISchoolStreamsRepository, SchoolStreamsRepository>();
            services.AddTransient<IToDoListRepository, ToDoListRepository>();
            #endregion

            #region Staff Repositories
            services.AddTransient<IStaffDetailsRepository, StaffDetailsRepository>();
            services.AddTransient<IStaffAttendanceRepository, StaffAttendanceRepository>();
            services.AddTransient<IStaffDisciplineRepository, StaffDisciplineRepository>();
            services.AddTransient<IStaffSubjectRepository, StaffSubjectRepository>();
            #endregion

            #region Students repositories
            services.AddTransient<IParentsRepository, ParentsRepository>();
            services.AddTransient<IFormerSchoolsRepository, FormerSchoolsRepository>();
            services.AddTransient<IStudentsRepository, StudentsRepository>();
            services.AddTransient<IStudentParentRepository, StudentParentRepository>();
            services.AddTransient<IStudentDisciplineRepository, StudentDisciplineRepository>();
            services.AddTransient<IStudentAttendanceRepository, StudentAttendanceRepository>();
            services.AddTransient<IStudentClassRepository, StudentClassRepository>();
            services.AddTransient<IStudentSubjectRepository, StudentSubjectRepository>();

            #endregion

            #region Class Repositories
            services.AddTransient<ISessionRepository, SessionRepository>();
            services.AddTransient<ILearningLevelRepository, LearningLevelRepository>();
            services.AddTransient<ISchoolClassRepository, SchoolClassRepository>();
            services.AddTransient<ISchoolClassLeadersRepository, SchoolClassLeadersRepository>();
            services.AddTransient<IClassLeadershipRoleRepository, ClassLeadershipRoleRepository>();
            #endregion

            #region Settings Repositories
            services.AddTransient<IDesignationsRepository, DesignationsRepository>();
            services.AddTransient<IEmploymentTypesRepository, EmploymentTypesRepository>();
            services.AddTransient<IGenderRepository, GenderRepository>();
            services.AddTransient<INationalityRepository, NationalityRepository>();
            services.AddTransient<IOccupationsRepository, OccupationsRepository>();
            services.AddTransient<IOccurenceTypesRepository, OccurenceTypesRepository>();
            services.AddTransient<IOutcomesRepository, OutcomesRepository>();
            services.AddTransient<IRelationshipsRepository, RelationshipsRepository>();
            services.AddTransient<IReligionsRepository, ReligionsRepository>();
            services.AddTransient<ISessionTypesRepository, SessionTypesRepository>();
            services.AddTransient<IStaffCategoryRepository, StaffCategoryRepository>();
            #endregion

            #region Services Dependency Injection
            services.AddScoped<IStudentAttendanceService, StudentAttendanceService>();

            #endregion

            #region Services: CBE Assessments
            services.AddScoped<IThemeService, ThemeService>();
            services.AddScoped<IAssessmentTypeService, AssessmentTypeService>();
            services.AddScoped<IStudentAssessmentService, StudentAssessmentService>();
            services.AddScoped<IBroadOutcomeService, BroadOutcomeService>();
            services.AddScoped<ICompetencyService, CompetencyService>();
            services.AddScoped<IGeneralOutcomeService, GeneralOutcomeService>();
            services.AddScoped<ISpecificOutcomeService, SpecificOutcomeService>();
            services.AddScoped<IStrandService, StrandService>();
            services.AddScoped<ISubStrandService, SubStrandService>();
            #endregion

            #region Services: CBE Values
            services.AddScoped<IValueService, ValueService>();
            services.AddScoped<IValueScoreService, ValueScoreService>();
            services.AddScoped<IStudentValueScoreService, StudentValueScoreService>();
            #endregion

            #region Services: CBE Responsibilities
            services.AddScoped<IResponsibilityService, ResponsibilityService>();
            // SocialSkill merged into Responsibility with Category field
            services.AddScoped<IStudentResponsibilityService, StudentResponsibilityService>();
            #endregion

            #region Services: CBE Cocurriculum
            services.AddScoped<ICoCurriculumActivityService, CoCurriculumActivityService>();
            services.AddScoped<ICoCurriculumScoreTypeService, CoCurriculumScoreTypeService>();
            services.AddScoped<ICoCurriculumScoreService, CoCurriculumScoreService>();
            services.AddScoped<IStudentCoCurriculumActivityService, StudentCoCurriculumActivityService>();
            services.AddScoped<IStudentCoCurriculumScoreService, StudentCoCurriculumScoreService>();
            #endregion

            #region Services: CBE CommunityService
            services.AddScoped<ICommunityServiceActivityService, CommunityServiceActivityService>();
            services.AddScoped<IStudentCommunityServiceActivityService, StudentCommunityServiceActivityService>();
            #endregion

            #region Services: CBE Assessments (additional)
            services.AddScoped<IKeyQuestionService, KeyQuestionService>();
            services.AddScoped<ILearningExperienceService, LearningExperienceService>();
            services.AddScoped<IPCIService, PCIService>();
            services.AddScoped<ILessonAllocationService, LessonAllocationService>();
            #endregion

            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            return services;
        }

        public static void ConfigureCors(this IServiceCollection services)
        {
            services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy",
                    builder => builder.AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader());
            });
        }
    }
}
