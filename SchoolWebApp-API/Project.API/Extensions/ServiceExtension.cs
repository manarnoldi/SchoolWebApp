using SchoolWebApp.Core.Interfaces.IRepositories;
using SchoolWebApp.Core.Interfaces.IRepositories.School;
using SchoolWebApp.Core.Interfaces.IServices.School;
using SchoolWebApp.Core.Services.School;
using SchoolWebApp.Infrastructure.Repositories;
using SchoolWebApp.Infrastructure.Repositories.School;

namespace Project.API.Extensions
{
    public static class ServiceExtension
    {
        public static IServiceCollection RegisterService(this IServiceCollection services)
        {
            #region IUnitOfWork
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            #endregion
            #region Services
            services.AddScoped<ISchoolDetailsService, SchoolDetailsService>();
            #endregion

            #region Repositories
            services.AddTransient<ISchoolDetailsRepository, SchoolDetailsRepository>();
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
