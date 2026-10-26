using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Project.Infrastructure.Data;
using SchoolWebApp.Core.DTOs.TeachingDocuments;
using SchoolWebApp.Core.Entities.Approvals;
using SchoolWebApp.Core.Entities.CBE.Assessments;
using SchoolWebApp.Core.Entities.Class;
using SchoolWebApp.Core.Entities.Identity;
using SchoolWebApp.Core.Entities.TeachingDocuments;
using System.Globalization;

namespace SchoolWebApp.API.Services.TeachingDocuments
{
    /// <summary>
    /// Who the caller is as far as teaching documents go.
    /// - Whole school: Administrator / SuperAdministrator, or a role holding the
    ///   /teaching/admin menu permission (Head Teacher, Deputy by default).
    /// - Head of department: the staff member set as a department's head sees
    ///   and checks that department's documents.
    /// - Everyone else: their own documents only.
    /// </summary>
    public class TeachingUser
    {
        public int UserId { get; set; }
        public string Name { get; set; } = "";
        public int? StaffDetailsId { get; set; }
        public bool IsAdmin { get; set; }
        public bool WholeSchool { get; set; }
        public HashSet<int> HodDepartmentIds { get; set; } = new();

        public bool Owns(SchemeOfWork s) => StaffDetailsId.HasValue && s.StaffDetailsId == StaffDetailsId.Value;

        public bool Oversees(int departmentId) => WholeSchool || HodDepartmentIds.Contains(departmentId);

        public bool CanEdit(SchemeOfWork s) => IsAdmin || Owns(s);

        // Checking your own record of work would defeat the point of the check.
        public bool CanCheck(SchemeOfWork s) => !Owns(s) && Oversees(s.Subject!.DepartmentId);

        // Uploading a scheme workbook: the teacher, an administrator, or the head
        // of the subject's department (e.g. a typist entering paper schemes).
        public bool CanUploadFor(int staffDetailsId, int departmentId) =>
            IsAdmin || StaffDetailsId == staffDetailsId || HodDepartmentIds.Contains(departmentId);
    }

    public class TeachingDocumentService
    {
        public const string EntityScheme = "SchemeOfWork";
        public const string EntityLessonPlanWeek = "LessonPlanWeek";
        public const string WholeSchoolPermissionPath = "/teaching/admin";
        private const string SettingsModule = "TeachingDocuments";

        private readonly ApplicationDbContext _db;
        private readonly UserManager<AppUser> _userManager;
        private readonly IHttpContextAccessor _http;

        public TeachingDocumentService(ApplicationDbContext db, UserManager<AppUser> userManager, IHttpContextAccessor http)
        {
            _db = db;
            _userManager = userManager;
            _http = http;
        }

        // ---------- The caller ----------

        private TeachingUser? _current;

        public async Task<TeachingUser?> CurrentAsync()
        {
            if (_current != null) return _current;
            var claim = _http.HttpContext?.User.Claims.FirstOrDefault(c => c.Type == "userid")?.Value;
            if (!int.TryParse(claim, out var userId)) return null;
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null) return null;

            // Roles from the database, not the token - see ExamResultsController.
            var roles = (await _userManager.GetRolesAsync(user)).ToList();
            var isAdmin = roles.Contains("Administrator") || roles.Contains("SuperAdministrator");
            var wholeSchool = isAdmin || await _db.MenuPermissions.AnyAsync(p =>
                p.MenuPath == WholeSchoolPermissionPath && roles.Contains(p.RoleId));
            var hodDepartments = user.PersonId == null
                ? new List<int>()
                : await _db.Departments.Where(d => d.StaffDetailsId == user.PersonId).Select(d => d.Id).ToListAsync();

            _current = new TeachingUser
            {
                UserId = user.Id,
                Name = $"{user.FirstName} {user.LastName}".Trim(),
                StaffDetailsId = user.PersonId,
                IsAdmin = isAdmin,
                WholeSchool = wholeSchool,
                HodDepartmentIds = hodDepartments.ToHashSet()
            };
            return _current;
        }

        /// <summary>
        /// Owner, overseer (HOD / whole school), or an approver assigned to the
        /// scheme or one of its lesson plan weeks - a Head Teacher picked as an
        /// approver must be able to open what they are approving.
        /// </summary>
        public async Task<bool> CanViewAsync(TeachingUser me, SchemeOfWork s)
        {
            if (me.Owns(s) || me.Oversees(s.Subject!.DepartmentId)) return true;
            var weekIds = await _db.LessonPlanWeeks.Where(w => w.SchemeOfWorkId == s.Id).Select(w => w.Id).ToListAsync();
            return await _db.ApprovalRequests.AnyAsync(r =>
                ((r.EntityType == EntityScheme && r.EntityId == s.Id)
                 || (r.EntityType == EntityLessonPlanWeek && weekIds.Contains(r.EntityId)))
                && r.Actions.Any(a => a.AssignedToUserId == me.UserId));
        }

        public Task<SchemeOfWork?> LoadSchemeAsync(int id, bool withLessons = false)
        {
            IQueryable<SchemeOfWork> q = _db.SchemesOfWork
                .Include(s => s.StaffDetails)
                .Include(s => s.Subject)
                .Include(s => s.SchoolClass).ThenInclude(c => c!.LearningLevel)
                .Include(s => s.Session).ThenInclude(x => x!.AcademicYear);
            if (withLessons) q = q.Include(s => s.Lessons);
            return q.FirstOrDefaultAsync(s => s.Id == id);
        }

        // ---------- Term calendar ----------

        /// <summary>Week 1 starts on the Monday of the week the term opens in.</summary>
        public static DateTime Week1Start(Session session)
        {
            var start = session.StartDate.Date;
            var offset = ((int)start.DayOfWeek + 6) % 7;
            return start.AddDays(-offset);
        }

        /// <summary>Lessons a week: the Lesson Allocation for the subject and grade, else the subject's lessons.</summary>
        public async Task<int> DefaultLessonsPerWeekAsync(int subjectId, int learningLevelId)
        {
            var perWeek = await _db.Set<LessonAllocation>()
                .Where(a => a.SubjectId == subjectId && a.LearningLevelId == learningLevelId)
                .Select(a => (int?)a.LessonsPerWeek).FirstOrDefaultAsync();
            var subjectLessons = await _db.Subjects.Where(s => s.Id == subjectId).Select(s => s.NumOfLessons).FirstOrDefaultAsync();
            return Math.Clamp(perWeek ?? subjectLessons, 1, 20);
        }

        public static List<SchemeOfWorkLesson> BlankGrid(int weeks, int perWeek) =>
            Enumerable.Range(1, weeks).SelectMany(w => Enumerable.Range(1, perWeek)
                .Select(l => new SchemeOfWorkLesson { WeekNo = w, LessonNo = l })).ToList();

        public static int TermWeeks(Session session)
        {
            var days = (session.EndDate.Date - Week1Start(session)).Days + 1;
            return Math.Max(1, (int)Math.Ceiling(days / 7.0));
        }

        public static (DateTime Start, DateTime End) WeekRange(Session session, int weekNo)
        {
            var start = Week1Start(session).AddDays(7 * (weekNo - 1));
            return (start, start.AddDays(4));
        }

        public static bool IsCurrent(Session s) => s.StartDate.Date <= DateTime.Today && DateTime.Today <= s.EndDate.Date;

        public static TeachingSessionDto ToSessionDto(Session s) => new()
        {
            Id = s.Id,
            Name = s.SessionName,
            AcademicYearId = s.AcademicYearId,
            AcademicYearName = s.AcademicYear?.Name ?? "",
            StartDate = s.StartDate,
            EndDate = s.EndDate,
            Weeks = TermWeeks(s),
            IsCurrent = IsCurrent(s)
        };

        // ---------- Due dates ----------

        public class DueSettings
        {
            public int SchemeDueDays { get; set; } = 7;
            public int LessonPlanDueDaysBefore { get; set; }
            public TimeSpan LessonPlanDueTime { get; set; } = new(8, 0, 0);
        }

        private DueSettings? _due;

        public async Task<DueSettings> DueAsync()
        {
            if (_due != null) return _due;
            var rows = await _db.GlobalSettings.AsNoTracking().Where(s => s.Module == SettingsModule).ToListAsync();
            string? Get(string key) => rows.FirstOrDefault(r => r.SettingKey == key)?.SettingValue;
            var due = new DueSettings();
            if (int.TryParse(Get("SchemeDueDays"), out var sd)) due.SchemeDueDays = sd;
            if (int.TryParse(Get("LessonPlanDueDaysBefore"), out var lb)) due.LessonPlanDueDaysBefore = lb;
            if (TimeSpan.TryParseExact(Get("LessonPlanDueTime"), @"hh\:mm", CultureInfo.InvariantCulture, out var t)) due.LessonPlanDueTime = t;
            return _due = due;
        }

        // Due times are school (server) clock times. Marked Local so the JSON
        // serializer converts them to UTC - an Unspecified 08:00 would go out as
        // 08:00Z and show as 11:00 in the browser.
        // Due by the end of the day SchemeDueDays after the term opens.
        public static DateTime SchemeDueAt(Session session, DueSettings due) => DateTime.SpecifyKind(
            session.StartDate.Date.AddDays(due.SchemeDueDays + 1).AddSeconds(-1), DateTimeKind.Local);

        public static DateTime LessonPlanDueAt(Session session, int weekNo, DueSettings due) => DateTime.SpecifyKind(
            WeekRange(session, weekNo).Start.AddDays(-due.LessonPlanDueDaysBefore).Add(due.LessonPlanDueTime), DateTimeKind.Local);

        public static List<WeekDto> Weeks(Session session, int count, DueSettings due) =>
            Enumerable.Range(1, Math.Max(count, 1)).Select(n =>
            {
                var (start, end) = WeekRange(session, n);
                return new WeekDto { WeekNo = n, StartDate = start, EndDate = end, DueAt = LessonPlanDueAt(session, n, due) };
            }).ToList();

        // ---------- Status from the approval engine ----------

        public class ApprovalState
        {
            public ApprovalRequestStatus Status { get; set; }
            public int? AwaitingUserId { get; set; }
        }

        public async Task<Dictionary<int, ApprovalState>> ApprovalStatesAsync(string entityType, IEnumerable<int> ids)
        {
            var list = ids.Distinct().ToList();
            if (list.Count == 0) return new();
            var requests = await _db.ApprovalRequests.AsNoTracking()
                .Where(r => r.EntityType == entityType && list.Contains(r.EntityId))
                .Select(r => new
                {
                    r.EntityId,
                    r.Status,
                    Awaiting = r.Actions.Where(a => a.StepRank == r.CurrentStepRank).Select(a => (int?)a.AssignedToUserId).FirstOrDefault()
                })
                .ToListAsync();
            return requests.GroupBy(r => r.EntityId).ToDictionary(g => g.Key, g => new ApprovalState
            {
                Status = g.First().Status,
                AwaitingUserId = g.First().Status == ApprovalRequestStatus.Submitted ? g.First().Awaiting : null
            });
        }

        public static string StatusOf(ApprovalState? state) => state?.Status switch
        {
            null => DocumentStatus.Draft,
            ApprovalRequestStatus.Submitted => DocumentStatus.Submitted,
            ApprovalRequestStatus.Approved => DocumentStatus.Approved,
            ApprovalRequestStatus.Returned => DocumentStatus.Returned,
            ApprovalRequestStatus.Rejected => DocumentStatus.Rejected,
            // Draft, or Reversed by a Super Administrator - back with the teacher.
            _ => DocumentStatus.Draft
        };

        // An in-flight or approved document is not "overdue" even if it went in late.
        public static bool IsOverdue(string status, DateTime dueAt) =>
            (status == DocumentStatus.NotStarted || status == DocumentStatus.Draft || status == DocumentStatus.Returned)
            && DateTime.Now > dueAt;

        // ---------- Mapping ----------

        public async Task<SchemeOfWorkDto> ToDtoAsync(SchemeOfWork s, TeachingUser me, bool withLessons)
        {
            var due = await DueAsync();
            var state = (await ApprovalStatesAsync(EntityScheme, new[] { s.Id })).GetValueOrDefault(s.Id);
            var status = StatusOf(state);
            var locked = await IsLockedAsync(EntityScheme, s.Id);
            return new SchemeOfWorkDto
            {
                Id = s.Id,
                StaffDetailsId = s.StaffDetailsId,
                StaffName = s.StaffDetails?.FullName ?? "",
                SubjectId = s.SubjectId,
                SubjectName = s.Subject?.Name ?? "",
                SchoolClassId = s.SchoolClassId,
                ClassName = s.SchoolClass?.Name ?? "",
                LearningLevelId = s.SchoolClass?.LearningLevelId ?? 0,
                LearningLevelName = s.SchoolClass?.LearningLevel?.Name ?? "",
                SessionId = s.SessionId,
                SessionName = s.Session?.SessionName ?? "",
                AcademicYearName = s.Session?.AcademicYear?.Name ?? "",
                DepartmentId = s.Subject?.DepartmentId ?? 0,
                References = s.References,
                NumberOfWeeks = s.NumberOfWeeks,
                LessonsPerWeek = s.LessonsPerWeek,
                Status = status,
                IsLocked = locked,
                CanEdit = me.CanEdit(s) && !locked,
                DueAt = SchemeDueAt(s.Session!, due),
                Weeks = Weeks(s.Session!, s.NumberOfWeeks, due),
                Lessons = withLessons
                    ? s.Lessons.OrderBy(l => l.WeekNo).ThenBy(l => l.LessonNo).ThenBy(l => l.Id).Select(ToDto).ToList()
                    : new(),
                Modified = s.Modified,
                ModifiedBy = s.ModifiedBy
            };
        }

        public static SchemeOfWorkLessonDto ToDto(SchemeOfWorkLesson l) => new()
        {
            Id = l.Id,
            WeekNo = l.WeekNo,
            LessonNo = l.LessonNo,
            StrandId = l.StrandId,
            SubStrandId = l.SubStrandId,
            StrandName = l.StrandName,
            SubStrandName = l.SubStrandName,
            LearningOutcomes = l.LearningOutcomes,
            LearningExperiences = l.LearningExperiences,
            KeyInquiryQuestions = l.KeyInquiryQuestions,
            LearningResources = l.LearningResources,
            AssessmentMethods = l.AssessmentMethods
        };

        // Same rule as ApprovalLockHelper (global edit-lock policy), on this context.
        public async Task<bool> IsLockedAsync(string entityType, int entityId)
        {
            var req = await _db.ApprovalRequests.AsNoTracking().Include(r => r.Actions)
                .FirstOrDefaultAsync(r => r.EntityType == entityType && r.EntityId == entityId);
            if (req == null) return false;
            if (req.Status == ApprovalRequestStatus.Approved) return true;
            if (req.Status != ApprovalRequestStatus.Submitted) return false;
            var policy = await _db.GlobalSettings.AsNoTracking()
                .Where(s => s.Module == "General" && s.SettingKey == "ApprovalEditLockPolicy")
                .Select(s => s.SettingValue).FirstOrDefaultAsync();
            if (policy == Core.Helpers.ApprovalLockHelper.POLICY_AFTER_FIRST_APPROVAL)
                return req.Actions.Any(a => a.Status == StepActionStatus.Approved);
            return true;
        }

        /// <summary>
        /// Checks run by the approval engine before a teaching document is
        /// submitted: only its teacher (or an administrator) submits it, and it
        /// must have something in it. Returns an error message, or null.
        /// </summary>
        /// <summary>
        /// What a lesson plan still needs before it can be saved - date, start and
        /// end time (end after start), roll and lesson flow minutes - as e.g.
        /// "the date and the roll"; null when complete. Times are HH:mm.
        /// </summary>
        public static string? MissingToSave(DateTime? date, string? start, string? end, int? roll,
            int? introductionMinutes, int? developmentMinutes, int? conclusionMinutes)
        {
            var missing = new List<string>();
            if (date == null) missing.Add("the date");
            if (string.IsNullOrWhiteSpace(start)) missing.Add("the start time");
            if (string.IsNullOrWhiteSpace(end)) missing.Add("the end time");
            else if (!string.IsNullOrWhiteSpace(start) && string.CompareOrdinal(end.Trim(), start.Trim()) <= 0)
                missing.Add("an end time after the start time");
            if (!(roll > 0)) missing.Add("the roll");
            if ((introductionMinutes ?? 0) + (developmentMinutes ?? 0) + (conclusionMinutes ?? 0) <= 0)
                missing.Add("the minutes in the lesson flow");
            if (missing.Count == 0) return null;
            return missing.Count == 1 ? missing[0] : string.Join(", ", missing.SkipLast(1)) + " and " + missing[^1];
        }

        public async Task<string?> ValidateSubmitAsync(string entityType, int entityId)
        {
            if (entityType != EntityScheme && entityType != EntityLessonPlanWeek) return null;
            var me = await CurrentAsync();
            if (me == null) return "Unable to identify the current user.";

            int schemeId = entityId;
            if (entityType == EntityLessonPlanWeek)
            {
                var week = await _db.LessonPlanWeeks.AsNoTracking().Include(w => w.Plans).FirstOrDefaultAsync(w => w.Id == entityId);
                if (week == null) return "Lesson plan week not found.";
                if (week.Plans.Count == 0) return "Add at least one lesson plan before submitting the week.";
                // Lessons left as prepared never went through Save, so check them all here.
                var incomplete = week.Plans.OrderBy(p => p.LessonNo)
                    .Select(p => (p.LessonNo, Missing: MissingToSave(p.LessonDate, p.StartTime, p.EndTime, p.Roll,
                        p.IntroductionMinutes, p.DevelopmentMinutes, p.ConclusionMinutes)))
                    .Where(x => x.Missing != null).ToList();
                if (incomplete.Count > 0)
                    return "Complete every lesson before submitting the week - "
                        + string.Join("; ", incomplete.Select(x => $"lesson {x.LessonNo}: enter {x.Missing}")) + ".";
                schemeId = week.SchemeOfWorkId;
            }
            var scheme = await _db.SchemesOfWork.AsNoTracking().Include(s => s.Lessons).FirstOrDefaultAsync(s => s.Id == schemeId);
            if (scheme == null) return "Scheme of work not found.";
            if (!me.CanEdit(scheme)) return "Only the teacher who owns this document can submit it.";
            if (entityType == EntityScheme && !scheme.Lessons.Any(l =>
                    !string.IsNullOrWhiteSpace(l.SubStrandName) || !string.IsNullOrWhiteSpace(l.LearningOutcomes)))
                return "Fill in at least one lesson before submitting the scheme.";
            return null;
        }

        /// <summary>
        /// Strands and sub-strands of a subject at a grade with their outcomes,
        /// experiences and key questions. With a term, each sub-strand says which
        /// terms of that year already have it in a scheme.
        /// </summary>
        public async Task<List<CurriculumStrandDto>> CurriculumAsync(int subjectId, int learningLevelId, int? sessionId)
        {
            var strands = await _db.Strands.AsNoTracking()
                .Where(s => s.SubjectId == subjectId && s.LearningLevelId == learningLevelId)
                .OrderBy(s => s.Rank).ThenBy(s => s.Code).ThenBy(s => s.Id)
                .Select(s => new { s.Id, s.Name, s.Code })
                .ToListAsync();
            var strandIds = strands.Select(s => s.Id).ToList();
            var subStrands = await _db.SubStrands.AsNoTracking()
                .Where(s => strandIds.Contains(s.StrandId))
                .OrderBy(s => s.Rank).ThenBy(s => s.Code).ThenBy(s => s.Id)
                .Select(s => new { s.Id, s.StrandId, s.Name, s.Code, s.LessonNo })
                .ToListAsync();
            var subIds = subStrands.Select(s => s.Id).ToList();
            var outcomes = await _db.SpecificOutcomes.AsNoTracking().Where(o => subIds.Contains(o.SubStrandId))
                .OrderBy(o => o.Rank).ThenBy(o => o.Id).Select(o => new { o.SubStrandId, o.Name }).ToListAsync();
            var experiences = await _db.Set<LearningExperience>().AsNoTracking().Where(o => subIds.Contains(o.SubStrandId))
                .OrderBy(o => o.Rank).ThenBy(o => o.Id).Select(o => new { o.SubStrandId, o.Name }).ToListAsync();
            var questions = await _db.Set<KeyQuestion>().AsNoTracking().Where(o => subIds.Contains(o.SubStrandId))
                .OrderBy(o => o.Rank).ThenBy(o => o.Id).Select(o => new { o.SubStrandId, o.Name }).ToListAsync();

            var scheduled = new Dictionary<int, List<string>>();
            if (sessionId.HasValue)
            {
                var yearId = await _db.Sessions.Where(s => s.Id == sessionId).Select(s => s.AcademicYearId).FirstOrDefaultAsync();
                var used = await _db.SchemeOfWorkLessons.AsNoTracking()
                    .Where(l => l.SubStrandId != null && subIds.Contains(l.SubStrandId.Value)
                        && l.SchemeOfWork!.SubjectId == subjectId
                        && l.SchemeOfWork.SchoolClass!.LearningLevelId == learningLevelId
                        && l.SchemeOfWork.Session!.AcademicYearId == yearId)
                    .Select(l => new { SubStrandId = l.SubStrandId!.Value, l.SchemeOfWork!.Session!.SessionName, l.SchemeOfWork.Session.StartDate })
                    .Distinct().ToListAsync();
                scheduled = used.GroupBy(u => u.SubStrandId)
                    .ToDictionary(g => g.Key, g => g.OrderBy(u => u.StartDate).Select(u => u.SessionName).Distinct().ToList());
            }

            return strands.Select(s => new CurriculumStrandDto
            {
                Id = s.Id,
                Name = s.Name,
                Code = s.Code,
                SubStrands = subStrands.Where(ss => ss.StrandId == s.Id).Select(ss => new CurriculumSubStrandDto
                {
                    Id = ss.Id,
                    Name = ss.Name,
                    Code = ss.Code,
                    Lessons = ss.LessonNo,
                    SpecificOutcomes = outcomes.Where(o => o.SubStrandId == ss.Id).Select(o => o.Name).ToList(),
                    LearningExperiences = experiences.Where(o => o.SubStrandId == ss.Id).Select(o => o.Name).ToList(),
                    KeyQuestions = questions.Where(o => o.SubStrandId == ss.Id).Select(o => o.Name).ToList(),
                    ScheduledIn = scheduled.GetValueOrDefault(ss.Id) ?? new()
                }).ToList()
            }).ToList();
        }

        /// <summary>The users to pre-select as approvers: the head of the subject's department.</summary>
        public async Task<List<int>> PreferredApproverUserIdsAsync(int subjectId)
        {
            var hodStaffId = await _db.Subjects.Where(s => s.Id == subjectId)
                .Select(s => s.Department!.StaffDetailsId).FirstOrDefaultAsync();
            if (hodStaffId == null) return new();
            return await _db.Users.Where(u => u.PersonId == hodStaffId).Select(u => u.Id).ToListAsync();
        }

        public static string Lines(IEnumerable<string> items, string? heading = null)
        {
            var list = items.Where(i => !string.IsNullOrWhiteSpace(i)).Select(i => i.Trim()).ToList();
            if (list.Count == 0) return "";
            var body = string.Join("\n", list.Select(i => "- " + i));
            return heading == null ? body : heading + "\n" + body;
        }
    }
}
