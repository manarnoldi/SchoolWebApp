using SchoolWebApp.Core.Interfaces.IRepositories;
using SchoolWebApp.Core.Interfaces.IRepositories.School;
using SchoolWebApp.Core.Interfaces.IRepositories.Settings;
using SchoolWebApp.Infrastructure.Repositories;
using SchoolWebApp.Infrastructure.Repositories.School;
using SchoolWebApp.Infrastructure.Repositories.Settings;

namespace Project.API.Extensions
{
    public static class ServiceExtension
    {
        public static IServiceCollection RegisterService(this IServiceCollection services)
        {
            #region IUnitOfWork
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            #endregion

            #region School Repositories
            services.AddTransient<ISchoolDetailsRepository, SchoolDetailsRepository>();
            services.AddTransient<IDepartmentsRepository, DepartmentsRepository>();
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
            services.AddTransient<ISchoolLevelsRepository, SchoolLevelsRepository>();
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
