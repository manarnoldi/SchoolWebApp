using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using SchoolWebApp.Core.Entities.Academics;
using SchoolWebApp.Core.Entities.Class;
using SchoolWebApp.Core.Entities.Identity;
using SchoolWebApp.Core.Entities.School;
using SchoolWebApp.Core.Entities.Settings;
using SchoolWebApp.Core.Entities.Shared;
using SchoolWebApp.Core.Entities.Staff;
using SchoolWebApp.Core.Entities.Students;
using System.Security.Claims;

namespace Project.Infrastructure.Data
{
    public class ApplicationDbContext : IdentityDbContext<AppUser, AppRole, string>
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options/*, IHttpContextAccessor httpContextAccessor*/, IHttpContextAccessor httpContextAccessor) : base(options)
        {
            _httpContextAccessor = httpContextAccessor;
            //_httpContextAccessor = httpContextAccessor;
        }

        #region Academics
        public DbSet<AcademicYear> AcademicYears { get; set; }
        public DbSet<Curriculum> Curricula { get; set; }
        public DbSet<Exam> Exams { get; set; }
        public DbSet<ExamResult> ExamResults { get; set; }
        public DbSet<ExamType> ExamTypes { get; set; }
        public DbSet<Grade> Grades { get; set; }
        public DbSet<Subject> Subjects { get; set; }
        public DbSet<SubjectGroup> SubjectGroups { get; set; }
        #endregion

        #region Class
        public DbSet<LearningLevel> LearningLevels { get; set; }
        public DbSet<SchoolClass> SchoolClasses { get; set; }
        public DbSet<SchoolClassLeaders> SchoolClassLeaders { get; set; }
        public DbSet<ClassLeadershipRole> ClassLeadershipRoles { get; set; }
        public DbSet<Session> Sessions { get; set; }
        #endregion

        #region School
        public DbSet<Department> Departments { get; set; }
        public DbSet<Event> Events { get; set; }
        public DbSet<LearningMode> LearningModes { get; set; }
        public DbSet<SchoolDetails> SchoolDetails { get; set; }
        public DbSet<SchoolStream> SchoolStreams { get; set; }
        public DbSet<EducationLevelType> EducationLevelTypes { get; set; }
        public DbSet<EducationLevel> EducationLevels { get; set; }
        public DbSet<ToDoList> ToDoLists { get; set; }

        #endregion

        #region Settings
        public DbSet<Designation> Designations { get; set; }
        public DbSet<EmploymentType> EmploymentTypes { get; set; }
        public DbSet<Gender> Genders { get; set; }
        public DbSet<Nationality> Nationalities { get; set; }
        public DbSet<Occupation> Occupations { get; set; }
        public DbSet<OccurenceType> OccurenceTypes { get; set; }
        public DbSet<Outcome> Outcomes { get; set; }
        public DbSet<RelationShip> RelationShips { get; set; }
        public DbSet<Religion> Religions { get; set; }
        public DbSet<SessionType> SessionTypes { get; set; }
        public DbSet<StaffCategory> StaffCategories { get; set; }
        #endregion

        #region Staff
        public DbSet<StaffAttendance> StaffAttendances { get; set; }
        public DbSet<StaffDetails> StaffDetails { get; set; }
        public DbSet<StaffDiscipline> StaffDisciplines { get; set; }
        public DbSet<StaffSubject> StaffSubjects { get; set; }
        #endregion

        #region Students
        public DbSet<FormerSchool> FormerSchools { get; set; }
        public DbSet<Parent> Parents { get; set; }
        public DbSet<Student> Students { get; set; }
        public DbSet<StudentAttendance> StudentAttendances { get; set; }
        public DbSet<StudentClass> StudentClasses { get; set; }
        public DbSet<StudentDiscipline> StudentDisciplines { get; set; }
        public DbSet<StudentParent> StudentParents { get; set; }
        public DbSet<StudentSubject> StudentSubjects { get; set; }
        #endregion
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            ApplicationDbContextConfigurations.Configure(builder);
            ApplicationDbContextConfigurations.SeedData(builder);
        }

        protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
        {
            configurationBuilder.Conventions.Remove(typeof(CascadeDeleteConvention));
            base.ConfigureConventions(configurationBuilder);
        }

        //public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess,
        //    CancellationToken cancellationToken = default(CancellationToken))
        //{
        //    //var currentUserId = _httpContextAccessor.HttpContext.User.FindFirst("username").Value;
        //    var currentUserId = "admin";
        //    var AddedEntities = ChangeTracker.Entries()
        //            .Where(E => E.State == EntityState.Added && (E.Entity is Base))
        //            .ToList();

        //    AddedEntities.ForEach(E =>
        //    {
        //        E.Property("Created").CurrentValue = DateTime.Now;
        //        E.Property("CreatedBy").CurrentValue = currentUserId;
        //        E.Property("Modified").CurrentValue = DateTime.Now;
        //        E.Property("ModifiedBy").CurrentValue = currentUserId;
        //    });

        //    var EditedEntities = ChangeTracker.Entries()
        //        .Where(E => E.State == EntityState.Modified && E.Entity is Base)
        //        .ToList();

        //    EditedEntities.ForEach(E =>
        //    {
        //        E.Property("Modified").CurrentValue = DateTime.Now;
        //        E.Property("ModifiedBy").CurrentValue = currentUserId;
        //    });
        //    return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
        //}

        public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default)
        {
            var entries = ChangeTracker.Entries().Where(e => e.Entity is Base && (e.State == EntityState.Added || e.State == EntityState.Modified));
            var user = _httpContextAccessor.HttpContext?.User?.FindFirstValue("username");
            foreach (var entityEntry in entries)
            {
                ((Base)entityEntry.Entity).Modified = DateTime.Now;
                ((Base)entityEntry.Entity).ModifiedBy = user;

                if (entityEntry.State == EntityState.Added)
                {
                    ((Base)entityEntry.Entity).Created = DateTime.Now;
                    ((Base)entityEntry.Entity).CreatedBy = user;
                }
                else
                {
                    entityEntry.Property("Created").IsModified = false;
                    entityEntry.Property("CreatedBy").IsModified = false;
                }
            }
            return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
        }

    }
}
