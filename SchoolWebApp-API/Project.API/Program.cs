using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json;
using NLog;
using NLog.Web;
using Project.API.Extensions;
using Project.Infrastructure.Data;
using SchoolWebApp.Core.Entities.Identity;
using SchoolWebApp.Core.Services;
using System.Text;
using System.Text.Json.Serialization;

// Bootstrap NLog as early as possible so config-load errors are visible.
var nlogLogger = LogManager.Setup().LoadConfigurationFromAppSettings().GetCurrentClassLogger();
nlogLogger.Debug("Starting ShuleNova API");

var builder = WebApplication.CreateBuilder(args);

string mySqlConnectionStr = builder.Configuration.GetConnectionString("DefaultConnection");

// Cap below site4now's per-MySQL-user limit of 20 concurrent connections.
// EF Core defaults to a pool of 100, which exhausts the hosting quota under
// load and triggers "max_user_connections" errors. NLog's MySQL target also
// uses this same user, so we leave headroom for it.
if (!string.IsNullOrEmpty(mySqlConnectionStr) &&
    !mySqlConnectionStr.Contains("MaximumPoolSize", StringComparison.OrdinalIgnoreCase))
{
    var separator = mySqlConnectionStr.TrimEnd().EndsWith(";") ? "" : ";";
    mySqlConnectionStr = $"{mySqlConnectionStr}{separator}MaximumPoolSize=10;MinimumPoolSize=0";
}

// Replace the default logging providers with NLog so everything that resolves
// ILogger<T> writes through NLog's pipeline (and into the Logs table for
// Error-level entries, per nlog.config).
builder.Logging.ClearProviders();
builder.Logging.SetMinimumLevel(Microsoft.Extensions.Logging.LogLevel.Trace);
builder.Host.UseNLog();

builder.Services.AddDbContext<ApplicationDbContext>(
                options => options.UseMySql(mySqlConnectionStr, ServerVersion.AutoDetect(mySqlConnectionStr)));
builder.Services.RegisterService();
builder.Services.AddHttpContextAccessor();

builder.Services.AddControllers(
           options =>
           {
               //options.InputFormatters.Add()
               options.ReturnHttpNotAcceptable = true;
               options.SuppressAsyncSuffixInActionNames = false;
               options.Filters.Add(new ProducesAttribute("application/json"));
               // options.SuppressImplicitRequiredAttributeForNonNullableReferenceTypes = true;
           }).AddNewtonsoftJson(options =>
           {
               // Use the default property (Pascal) casing
               //options.SerializerSettings.PreserveReferencesHandling = PreserveReferencesHandling.All;
               ////options.SerializerSettings.MaxDepth = 1;
               //options.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
               options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
           });

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(setupAction =>
{
    setupAction.AddSecurityDefinition("SchooSoftWebApiBearerAuth", new OpenApiSecurityScheme()
    {
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        Description = "Input a valid token to access this API"
    });

    setupAction.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "SchooSoftWebApiBearerAuth"
                        }
                    }, new List<string>()
                }
            });
});

builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
builder.Services.ConfigureCors();
builder.Services.AddScoped<JwtService>();

builder.Services
    .AddIdentityCore<AppUser>(options =>
    {
        options.SignIn.RequireConfirmedAccount = false;
        options.User.RequireUniqueEmail = true;
        options.Password.RequireDigit = false;
        options.Password.RequiredLength = 6;
        options.Password.RequireNonAlphanumeric = false;
        options.Password.RequireUppercase = false;
        options.Password.RequireLowercase = false;
    })
    .AddRoles<AppRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services
.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
.AddJwtBearer(
    JwtBearerDefaults.AuthenticationScheme,
    options =>
    {
        options.IncludeErrorDetails = true;
        options.TokenValidationParameters = new TokenValidationParameters()
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidAudience = builder.Configuration["Jwt:Audience"],
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"])
                    )
        };
    });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminRole", policy =>
    {
        policy.RequireAuthenticatedUser();
        policy.RequireRole("Administrator");
        // policy.RequireClaim("roles", "Admin");
    });
    options.AddPolicy("SuperAdminRole", policy =>
    {
        policy.RequireAuthenticatedUser();
        policy.RequireRole("SuperAdministrator");
    });
});

builder.Services.AddApiVersioning(setupAction =>
{
    setupAction.AssumeDefaultVersionWhenUnspecified = true;
    setupAction.DefaultApiVersion = new ApiVersion(1, 0);
    setupAction.ReportApiVersions = true;
});


var app = builder.Build();

app.UseCors("CorsPolicy");

app.UseExceptionHandler(errorApp =>
{
    errorApp.Run(context =>
    {
        var exceptionHandlerPathFeature = context.Features.Get<IExceptionHandlerPathFeature>();
        var exception = exceptionHandlerPathFeature?.Error;
        // Route through Microsoft.Extensions.Logging so NLog picks it up via
        // the provider registered in builder.Host.UseNLog() above.
        var requestLogger = context.RequestServices.GetRequiredService<ILogger<Program>>();
        requestLogger.LogError(exception, "Unhandled exception occurred");
        return Task.CompletedTask;
    });
});

// Apply pending migrations + idempotent seed/schema adjustments at startup
using (var scope = app.Services.CreateScope())
{
    var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
    try
    {
        var dbContext = scope.ServiceProvider.GetRequiredService<Project.Infrastructure.Data.ApplicationDbContext>();
        dbContext.Database.Migrate();
        await Project.Infrastructure.Data.DatabaseBootstrapper.ApplyAsync(dbContext, logger);
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "Error applying database migrations / bootstrapper at startup.");
    }
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

try
{
    app.Run();
}
catch (Exception startupEx)
{
    nlogLogger.Error(startupEx, "Application stopped because of an unhandled exception during startup.");
    throw;
}
finally
{
    // Flush & stop NLog timers/threads before exit (avoids segfaults on Linux,
    // ensures the last few log entries actually hit the DB on graceful stop).
    LogManager.Shutdown();
}
