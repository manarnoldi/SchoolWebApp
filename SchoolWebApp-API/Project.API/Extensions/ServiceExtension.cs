using SchoolWebApp.Core.Interfaces.IRepositories;
using SchoolWebApp.Core.Interfaces.IRepositories.Academics;
using SchoolWebApp.Core.Interfaces.IRepositories.Class;
using SchoolWebApp.Core.Interfaces.IRepositories.School;
using SchoolWebApp.Core.Interfaces.IRepositories.Settings;
using SchoolWebApp.Core.Interfaces.IRepositories.Staff;
using SchoolWebApp.Core.Interfaces.IRepositories.Students;
using SchoolWebApp.Infrastructure.Repositories;
using SchoolWebApp.Infrastructure.Repositories.Academics;
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
            services.AddTransient<IExamRepository, ExamRepository>();
            services.AddTransient<IGradeRepository, GradeRepository>();
            services.AddTransient<ISubjectGroupRepository, SubjectGroupRepository>();
            services.AddTransient<ISubjectRepository, SubjectRepository>();
            #endregion

            #region School Repositories
            services.AddTransient<ISchoolDetailsRepository, SchoolDetailsRepository>();
            services.AddTransient<IDepartmentsRepository, DepartmentsRepository>();
            services.AddTransient<ILearningModesRepository, LearningModesRepository>();
            services.AddTransient<IEventRepository, EventRepository>();
            services.AddTransient<IEducationLevelTypesRepository, EducationLevelTypesRepository>();
            services.AddTransient<IEducationLevelRepository, EducationLevelRepository>();
            services.AddTransient<ISchoolStreamsRepository, SchoolStreamsRepository>();
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
