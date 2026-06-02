using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using SchoolWebApp.Core.Entities.Academics;
using SchoolWebApp.Core.Entities.CBE.Assessments;
using SchoolWebApp.Core.Entities.CBE.Cocurriculum;
using SchoolWebApp.Core.Entities.CBE.CommunityService;
using SchoolWebApp.Core.Entities.CBE.Exams;
using SchoolWebApp.Core.Entities.CBE.Responsibilities;
using SchoolWebApp.Core.Entities.CBE.Values;
using SchoolWebApp.Core.Entities.Class;
using SchoolWebApp.Core.Entities.Identity;
using SchoolWebApp.Core.Entities.School;
using SchoolWebApp.Core.Entities.Settings;
using SchoolWebApp.Core.Entities.Shared;
using SchoolWebApp.Core.Entities.Staff;
using SchoolWebApp.Core.Entities.Students;
using SchoolWebApp.Core.Entities.Finance;
using SchoolWebApp.Core.Entities.Payroll;
using SchoolWebApp.Core.Entities.Approvals;
using SchoolWebApp.Core.Entities.Sponsorships;
using SchoolWebApp.Core.Entities.Security;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Newtonsoft.Json;
using System.Security.Claims;

namespace Project.Infrastructure.Data
{
    public class ApplicationDbContext : IdentityDbContext<AppUser, AppRole, int>
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
        public DbSet<Grade> Grades { get; set; }
        public DbSet<Subject> Subjects { get; set; }
        public DbSet<SubjectGroup> SubjectGroups { get; set; }
        public DbSet<EducationLevelSubject> EducationLevelSubjects { get; set; }
        #endregion

        #region CBC Assessments
        public DbSet<Competency> Competencies { get; set; }
        public DbSet<GeneralOutcome> GeneralOutcomes { get; set; }
        public DbSet<SubjectOutcome> BroadOutcomes { get; set; }
        public DbSet<SpecificOutcome> SpecificOutcomes { get; set; }
        public DbSet<Theme> Themes { get; set; }
        public DbSet<Strand> Strands { get; set; }
        public DbSet<SubStrand> SubStrands { get; set; }
        public DbSet<AssessmentType> AssessmentTypes { get; set; }
        public DbSet<StudentAssessment> StudentAssessments { get; set; }        
        #endregion

        #region CBC Exams
        public DbSet<SchoolExam> SchoolExams { get; set; }
        public DbSet<Exam> Exams { get; set; }
        public DbSet<ExamResult> ExamResults { get; set; }
        public DbSet<ExamType> ExamTypes { get; set; }
        #endregion

        #region CBC CoCurriculum
        public DbSet<CoCurriculumActivity> CoCurriculumActivities { get; set; }
        public DbSet<CoCurriculumScoreType> CoCurriculumScoreTypes { get; set; }
        public DbSet<CoCurriculumScore> CoCurriculumScores { get; set; }
        public DbSet<StudentCoCurriculumActivity> StudentCoCurriculumActivities { get; set; }
        public DbSet<StudentCoCurriculumScore> StudentCoCurriculumScores { get; set; }

        #endregion

        #region CBC Values
        public DbSet<Value> Values { get; set; }
        public DbSet<ValueScore> ValueScores { get; set; }
        public DbSet<StudentValueScore> StudentValueScores { get; set; }

        #endregion

        #region CBC Community Service Learning
        public DbSet<CommunityServiceActivity> CommunityServiceActivities { get; set; }
        public DbSet<StudentCommunityServiceActivity> StudentCommunityServiceActivities { get; set; }
        #endregion

        #region CBC Responsibilities
        public DbSet<Responsibility> Responsibilities { get; set; }
        public DbSet<StudentResponsibility> StudentResponsibilities { get; set; }
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
        public DbSet<GlobalSetting> GlobalSettings { get; set; }
        #endregion

        #region Identity
        public DbSet<MenuPermission> MenuPermissions { get; set; }
        public DbSet<Log> Logs { get; set; }
        #endregion

        #region Security
        // Audit trail - "who did what" for admin/compliance review.
        // Distinct from Logs (NLog error sink). Auto-written by
        // SaveChangesAsync for entity Create/Update/Delete, and
        // explicitly by IAuditService for non-entity events (login,
        // view, print, etc.). Append-only.
        public DbSet<AuditLog> AuditLogs { get; set; }
        #endregion

        #region Finance
        public DbSet<Account> Accounts { get; set; }
        public DbSet<FeeCategory> FeeCategories { get; set; }
        public DbSet<FeeStructure> FeeStructures { get; set; }
        public DbSet<FeeStructureItem> FeeStructureItems { get; set; }
        public DbSet<StudentInvoice> StudentInvoices { get; set; }
        public DbSet<StudentInvoiceItem> StudentInvoiceItems { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<PaymentAllocation> PaymentAllocations { get; set; }
        public DbSet<ExpenseCategory> ExpenseCategories { get; set; }
        public DbSet<Expense> Expenses { get; set; }
        public DbSet<ExpenseLine> ExpenseLines { get; set; }
        public DbSet<JournalEntry> JournalEntries { get; set; }
        public DbSet<JournalLine> JournalLines { get; set; }
        public DbSet<Budget> Budgets { get; set; }
        public DbSet<BudgetLine> BudgetLines { get; set; }
        public DbSet<BudgetMaster> BudgetMasters { get; set; }
        public DbSet<BudgetAmendment> BudgetAmendments { get; set; }
        public DbSet<BudgetAmendmentLine> BudgetAmendmentLines { get; set; }
        #endregion

        #region Payroll
        public DbSet<EarningType> EarningTypes { get; set; }
        public DbSet<DeductionType> DeductionTypes { get; set; }
        public DbSet<TaxBand> TaxBands { get; set; }
        public DbSet<PayrollSetting> PayrollSettings { get; set; }
        public DbSet<EmployeeSalary> EmployeeSalaries { get; set; }
        public DbSet<EmployeeSalaryItem> EmployeeSalaryItems { get; set; }
        public DbSet<LoanAdvance> LoanAdvances { get; set; }
        public DbSet<PayrollPeriod> PayrollPeriods { get; set; }
        public DbSet<Payslip> Payslips { get; set; }
        public DbSet<PayslipEarning> PayslipEarnings { get; set; }
        public DbSet<PayslipDeduction> PayslipDeductions { get; set; }
        #endregion

        #region Approvals
        public DbSet<ApprovalWorkflow> ApprovalWorkflows { get; set; }
        public DbSet<ApprovalWorkflowStep> ApprovalWorkflowSteps { get; set; }
        public DbSet<ApprovalRequest> ApprovalRequests { get; set; }
        public DbSet<ApprovalStepAction> ApprovalStepActions { get; set; }
        #endregion

        #region Sponsorships
        public DbSet<Sponsor> Sponsors { get; set; }
        public DbSet<Sponsorship> Sponsorships { get; set; }
        public DbSet<SponsorshipFeeCategory> SponsorshipFeeCategories { get; set; }
        public DbSet<SponsorPayment> SponsorPayments { get; set; }
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

        // Entity types whose lifecycle we deliberately do NOT mirror
        // into AuditLog. AuditLog itself would loop; Log is the NLog
        // error sink (noise, owned by NLog).
        private static readonly HashSet<Type> _auditExcluded = new HashSet<Type>
        {
            typeof(AuditLog),
            typeof(Log)
        };

        // Property names skipped when computing field-level diffs - they
        // either round-trip on every update (audit columns) or carry
        // values that would just bloat the JSON without aiding review.
        private static readonly HashSet<string> _auditSkipProps = new HashSet<string>
        {
            nameof(Base.Modified),
            nameof(Base.ModifiedBy),
            nameof(Base.Created),
            nameof(Base.CreatedBy)
        };

        public override async Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default)
        {
            var entries = ChangeTracker.Entries().Where(e => e.Entity is Base && (e.State == EntityState.Added || e.State == EntityState.Modified));
            var user = _httpContextAccessor.HttpContext?.User?.FindFirstValue("username");
            // Use UtcNow rather than .Now so audit timestamps stay
            // timezone-anchored: the hosting server's local clock may
            // not match the school's (e.g. server on PST, school on
            // EAT). Combined with the Newtonsoft DateTimeZoneHandling.
            // Utc setting in Program.cs the value reaches the browser
            // with a Z suffix and gets localised by the client.
            // Existing rows from before this change need a one-off
            // `Created/Modified + INTERVAL N HOUR` backfill to migrate
            // historical local-time values into UTC.
            foreach (var entityEntry in entries)
            {
                ((Base)entityEntry.Entity).Modified = DateTime.UtcNow;
                ((Base)entityEntry.Entity).ModifiedBy = user;

                if (entityEntry.State == EntityState.Added)
                {
                    ((Base)entityEntry.Entity).Created = DateTime.UtcNow;
                    ((Base)entityEntry.Entity).CreatedBy = user;
                }
                else
                {
                    // Defend against PUT bodies that brought back the
                    // full DTO with Created/CreatedBy omitted (would
                    // overwrite the historical values with defaults).
                    entityEntry.Property("Created").IsModified = false;
                    entityEntry.Property("CreatedBy").IsModified = false;
                }
            }

            // Audit trail capture - one AuditLog row per Added / Modified /
            // Deleted entity. Modified/Deleted rows are ready immediately;
            // Added rows are deferred because EF only assigns a real primary
            // key during SaveChanges - capturing it earlier would record EF's
            // temporary negative placeholder (e.g. -2147482641) instead of the
            // store-generated id. AuditLog itself and the NLog Log sink are
            // skipped to avoid loops / noise.
            var (auditRows, pendingAdds) = BuildAuditRows(user);

            if (auditRows.Count == 0 && pendingAdds.Count == 0)
                return await base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);

            // Persist the data and its audit trail atomically: save the data
            // first so the real keys land on Added rows, fill in the deferred
            // audit rows, then save them - all in one transaction so audit
            // always accompanies the data it describes.
            var ownsTransaction = Database.CurrentTransaction == null;
            var transaction = ownsTransaction
                ? await Database.BeginTransactionAsync(cancellationToken)
                : null;
            try
            {
                var affected = await base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);

                foreach (var pending in pendingAdds)
                {
                    // Real key is now assigned - patch both the EntityId column
                    // and the "Id" inside the captured new-values JSON.
                    pending.Row.EntityId = TryGetKey(pending.Entry);
                    var pk = pending.Entry.Metadata.FindPrimaryKey();
                    if (pk != null)
                        foreach (var keyProp in pk.Properties)
                            pending.NewValues[keyProp.Name] = pending.Entry.Property(keyProp.Name).CurrentValue;
                    pending.Row.NewValues = JsonConvert.SerializeObject(pending.NewValues);
                    auditRows.Add(pending.Row);
                }

                if (auditRows.Count > 0)
                {
                    AuditLogs.AddRange(auditRows);
                    await base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
                }

                if (transaction != null)
                    await transaction.CommitAsync(cancellationToken);

                return affected;
            }
            catch
            {
                if (transaction != null)
                    await transaction.RollbackAsync(cancellationToken);
                throw;
            }
            finally
            {
                if (transaction != null)
                    await transaction.DisposeAsync();
            }
        }

        // Carries an Added entity's audit row until after SaveChanges, when the
        // real primary key is available to patch into EntityId / NewValues.
        private sealed class PendingAdd
        {
            public EntityEntry Entry { get; set; }
            public AuditLog Row { get; set; }
            public Dictionary<string, object> NewValues { get; set; }
        }

        private (List<AuditLog> ready, List<PendingAdd> pendingAdds) BuildAuditRows(string user)
        {
            // Snapshot the relevant entries before we begin adding new
            // AuditLog rows - mutating the ChangeTracker mid-enumeration
            // would throw InvalidOperationException.
            var trackedNow = ChangeTracker
                .Entries()
                .Where(e =>
                    !_auditExcluded.Contains(e.Entity.GetType()) &&
                    (e.State == EntityState.Added ||
                     e.State == EntityState.Modified ||
                     e.State == EntityState.Deleted))
                .ToList();

            var http = _httpContextAccessor.HttpContext;
            var ip = http?.Connection?.RemoteIpAddress?.ToString();
            var ua = http?.Request?.Headers["User-Agent"].ToString();
            var path = http?.Request?.Path.Value;
            var userId = http?.User?.FindFirstValue(ClaimTypes.NameIdentifier);
            var timestamp = DateTime.UtcNow;

            var actor = string.IsNullOrEmpty(user) ? "system" : user;

            var ready = new List<AuditLog>(trackedNow.Count);
            var pendingAdds = new List<PendingAdd>();

            foreach (var entry in trackedNow)
            {
                var entityType = entry.Entity.GetType().Name;

                // Added entities are deferred: their primary key is still a
                // temporary placeholder until SaveChanges runs. Capture the
                // values now, but finish EntityId / NewValues after the save.
                if (entry.State == EntityState.Added)
                {
                    var newAdded = entry.Properties
                        .Where(p => !_auditSkipProps.Contains(p.Metadata.Name))
                        .ToDictionary(p => p.Metadata.Name, p => p.CurrentValue);

                    pendingAdds.Add(new PendingAdd
                    {
                        Entry = entry,
                        NewValues = newAdded,
                        Row = new AuditLog
                        {
                            Timestamp = timestamp,
                            UserId = userId,
                            UserName = actor,
                            Action = "Create",
                            EntityType = entityType,
                            OldValues = null,
                            IpAddress = ip,
                            UserAgent = string.IsNullOrEmpty(ua) ? null : ua,
                            RequestPath = path
                        }
                    });
                    continue;
                }

                var (oldJson, newJson, action) = SnapshotForAudit(entry);
                if (oldJson == null && newJson == null && action != "Delete")
                    continue;

                ready.Add(new AuditLog
                {
                    Timestamp = timestamp,
                    UserId = userId,
                    UserName = actor,
                    Action = action,
                    EntityType = entityType,
                    EntityId = TryGetKey(entry),
                    OldValues = oldJson,
                    NewValues = newJson,
                    IpAddress = ip,
                    UserAgent = string.IsNullOrEmpty(ua) ? null : ua,
                    RequestPath = path
                });
            }
            return (ready, pendingAdds);
        }

        private static (string oldJson, string newJson, string action) SnapshotForAudit(EntityEntry entry)
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    var newAdded = entry.Properties
                        .Where(p => !_auditSkipProps.Contains(p.Metadata.Name))
                        .ToDictionary(p => p.Metadata.Name, p => p.CurrentValue);
                    return (null, JsonConvert.SerializeObject(newAdded), "Create");

                case EntityState.Deleted:
                    var oldDeleted = entry.Properties
                        .Where(p => !_auditSkipProps.Contains(p.Metadata.Name))
                        .ToDictionary(p => p.Metadata.Name, p => p.OriginalValue);
                    return (JsonConvert.SerializeObject(oldDeleted), null, "Delete");

                case EntityState.Modified:
                    var modifiedProps = entry.Properties
                        .Where(p =>
                            p.IsModified &&
                            !_auditSkipProps.Contains(p.Metadata.Name))
                        .ToList();
                    if (modifiedProps.Count == 0) return (null, null, "Update");

                    // Tight diff when OriginalValues survived; full
                    // post-state fallback if EF was forced to mark
                    // every prop modified without an original (rare
                    // here since BaseRepository.Update doesn't call
                    // ChangeTracker.Clear()).
                    var changed = modifiedProps
                        .Where(p => !object.Equals(p.OriginalValue, p.CurrentValue))
                        .ToList();
                    if (changed.Count > 0)
                    {
                        var oldUpd = changed.ToDictionary(p => p.Metadata.Name, p => p.OriginalValue);
                        var newUpd = changed.ToDictionary(p => p.Metadata.Name, p => p.CurrentValue);
                        return (JsonConvert.SerializeObject(oldUpd), JsonConvert.SerializeObject(newUpd), "Update");
                    }

                    var postState = modifiedProps.ToDictionary(p => p.Metadata.Name, p => p.CurrentValue);
                    return (null, JsonConvert.SerializeObject(postState), "Update");

                default:
                    return (null, null, null);
            }
        }

        private static string TryGetKey(EntityEntry entry)
        {
            var pk = entry.Metadata.FindPrimaryKey();
            if (pk == null) return null;
            var values = pk.Properties
                .Select(p => entry.Property(p.Name).CurrentValue?.ToString())
                .ToArray();
            return string.Join(",", values);
        }

    }
}
