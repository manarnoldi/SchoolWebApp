using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Project.API.Extensions;
using Project.Infrastructure.Data;
using SchoolWebApp.Core.Entities.Identity;
using SchoolWebApp.Core.Services;
using Serilog;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

string mySqlConnectionStr = builder.Configuration.GetConnectionString("PrimaryDbConnection");

Log.Logger = new LoggerConfiguration()
        .MinimumLevel.Error()
        // .WriteTo.Console()
        .WriteTo.MySQL(mySqlConnectionStr)
        .CreateLogger();

builder.Logging.ClearProviders();
builder.Host.UseSerilog();

builder.Services.AddDbContext<ApplicationDbContext>(
                options => options.UseMySql(mySqlConnectionStr, ServerVersion.AutoDetect(mySqlConnectionStr)));
builder.Services.AddHttpContextAccessor();
builder.Services.RegisterService();

builder.Services.AddControllers(
           options =>
           {
               //options.InputFormatters.Add()
               options.ReturnHttpNotAcceptable = true;
               options.SuppressAsyncSuffixInActionNames = false;
               options.Filters.Add(new ProducesAttribute("application/json"));
               // options.SuppressImplicitRequiredAttributeForNonNullableReferenceTypes = true;
           }).AddNewtonsoftJson();

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
});

builder.Services.AddApiVersioning(setupAction =>
{
    setupAction.AssumeDefaultVersionWhenUnspecified = true;
    setupAction.DefaultApiVersion = new ApiVersion(1, 0);
    setupAction.ReportApiVersions = true;
});


var app = builder.Build();

app.UseExceptionHandler(errorApp =>
{
    errorApp.Run(context =>
    {
        var exceptionHandlerPathFeature = context.Features.Get<IExceptionHandlerPathFeature>();
        var exception = exceptionHandlerPathFeature?.Error;
        //Log.Error(exception, "Unhandled exception occurred");
        return Task.CompletedTask;
    });
});
app.UseCors("CorsPolicy");
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
