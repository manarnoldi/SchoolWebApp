using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Project.Infrastructure.Data;
using SchoolWebApp.Core.Entities.Elections;
using SchoolWebApp.Core.Entities.Identity;

namespace SchoolWebApp.API.Services.Elections
{
    public class ElectionUser
    {
        public int UserId { get; set; }
        public string Name { get; set; } = "";
        public int? StaffDetailsId { get; set; }
        // Elections officer: sets elections up, clears nominations, moves stages.
        public bool CanManage { get; set; }
    }

    /// <summary>
    /// Who the caller is for elections, the stage rules, and how a position is
    /// split into contests.
    /// </summary>
    public class ElectionService
    {
        public const string ManagePermissionPath = "/elections/manage";
        public const string SettingsModule = "Elections";

        private readonly ApplicationDbContext _db;
        private readonly UserManager<AppUser> _userManager;
        private readonly IHttpContextAccessor _http;
        private ElectionUser? _current;

        public ElectionService(ApplicationDbContext db, UserManager<AppUser> userManager, IHttpContextAccessor http)
        {
            _db = db;
            _userManager = userManager;
            _http = http;
        }

        public async Task<ElectionUser?> CurrentAsync()
        {
            if (_current != null) return _current;
            var claim = _http.HttpContext?.User.Claims.FirstOrDefault(c => c.Type == "userid")?.Value;
            if (!int.TryParse(claim, out var userId)) return null;
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null) return null;

            // Roles from the database, not the token - see ExamResultsController.
            var roles = (await _userManager.GetRolesAsync(user)).ToList();
            var isAdmin = roles.Contains("Administrator") || roles.Contains("SuperAdministrator");
            var canManage = isAdmin || await _db.MenuPermissions.AnyAsync(p =>
                p.MenuPath == ManagePermissionPath && roles.Contains(p.RoleId));
            _current = new ElectionUser
            {
                UserId = user.Id,
                Name = $"{user.FirstName} {user.LastName}".Trim(),
                StaffDetailsId = user.PersonId,
                CanManage = canManage
            };
            return _current;
        }

        public async Task<string?> SettingAsync(string key) =>
            await _db.GlobalSettings.AsNoTracking()
                .Where(s => s.Module == SettingsModule && s.SettingKey == key)
                .Select(s => s.SettingValue).FirstOrDefaultAsync();

        // Positions (and so contests) can change until candidates are final.
        public static bool PositionsEditable(ElectionStage stage) =>
            stage == ElectionStage.Draft || stage == ElectionStage.Nominations;

        /// <summary>
        /// The stages an election can move to from where it is. Forward one step at a
        /// time; Nominations can go back to Draft. Later steps arrive with the
        /// nominations, polling and results screens.
        /// </summary>
        public static List<ElectionStage> NextStages(ElectionStage stage) => stage switch
        {
            ElectionStage.Draft => new() { ElectionStage.Nominations },
            ElectionStage.Nominations => new() { ElectionStage.Draft },
            _ => new()
        };

        /// <summary>Why the election cannot move to a stage, or null when it can.</summary>
        public async Task<string?> CannotMoveAsync(Election e, ElectionStage to)
        {
            if (!NextStages(e.Stage).Contains(to))
                return $"An election in {Label(e.Stage)} cannot move to {Label(to)}.";
            if (to == ElectionStage.Nominations)
            {
                var positions = await _db.ElectionPositions.Where(p => p.ElectionId == e.Id)
                    .Select(p => new { p.Name, Contests = p.Contests.Count }).ToListAsync();
                if (positions.Count == 0) return "Add the electable positions before opening nominations.";
                var empty = positions.Where(p => p.Contests == 0).Select(p => p.Name).ToList();
                if (empty.Count > 0)
                    return $"These positions have no contests - check their grades and that the year has classes: {string.Join(", ", empty)}.";
            }
            return null;
        }

        public static string Label(ElectionStage stage) => stage switch
        {
            ElectionStage.Draft => "Draft",
            ElectionStage.Nominations => "Nominations open",
            ElectionStage.CandidatesFinal => "Candidates final",
            ElectionStage.Polling => "Polling open",
            ElectionStage.PollingClosed => "Polling closed",
            ElectionStage.Declared => "Results declared",
            _ => stage.ToString()
        };

        /// <summary>
        /// Brings a position's contests in line with its scope and voter grades: one
        /// for the whole position, or one per section, grade or class of the
        /// election's year. Contests that still fit are kept (with their ids), new
        /// ones added and ones that no longer fit removed.
        /// </summary>
        public async Task SyncContestsAsync(ElectionPosition position, int academicYearId)
        {
            var voterLevelIds = position.Levels.Where(l => l.Role == PositionLevelRole.Voter)
                .Select(l => l.LearningLevelId).ToList();
            var levels = await _db.LearningLevels.AsNoTracking().Include(l => l.EducationLevel)
                .Where(l => voterLevelIds.Contains(l.Id)).ToListAsync();

            var wanted = new List<ElectionContest>();
            switch (position.Scope)
            {
                case PositionScope.Single:
                    if (levels.Count > 0) wanted.Add(new ElectionContest { Name = position.Name });
                    break;
                case PositionScope.PerEducationLevel:
                    // Grouped by id: untracked rows each carry their own EducationLevel instance.
                    foreach (var section in levels.Where(l => l.EducationLevel != null).GroupBy(l => l.EducationLevelId)
                                 .Select(g => g.First().EducationLevel!).OrderBy(s => s.Rank).ThenBy(s => s.Name))
                        wanted.Add(new ElectionContest { Name = $"{position.Name} - {section.Name}", EducationLevelId = section.Id });
                    break;
                case PositionScope.PerGrade:
                    foreach (var grade in levels.OrderBy(l => l.Rank).ThenBy(l => l.Name))
                        wanted.Add(new ElectionContest { Name = $"{position.Name} - {grade.Name}", LearningLevelId = grade.Id });
                    break;
                case PositionScope.PerClass:
                    var classes = await _db.SchoolClasses.AsNoTracking().Include(c => c.LearningLevel)
                        .Where(c => c.AcademicYearId == academicYearId && voterLevelIds.Contains(c.LearningLevelId))
                        .ToListAsync();
                    foreach (var c in classes.OrderBy(c => c.LearningLevel!.Rank).ThenBy(c => c.Name))
                        wanted.Add(new ElectionContest { Name = $"{position.Name} - {c.Name}", SchoolClassId = c.Id, LearningLevelId = c.LearningLevelId });
                    break;
            }

            static string Key(ElectionContest c) => $"{c.EducationLevelId}|{c.SchoolClassId}|{(c.SchoolClassId == null ? c.LearningLevelId : null)}";
            var existing = position.Contests.ToDictionary(Key);
            var keep = new HashSet<string>();
            for (var i = 0; i < wanted.Count; i++)
            {
                var w = wanted[i];
                if (existing.TryGetValue(Key(w), out var have))
                {
                    have.Name = w.Name;
                    have.Rank = i + 1;
                    keep.Add(Key(w));
                }
                else
                {
                    w.Rank = i + 1;
                    position.Contests.Add(w);
                }
            }
            foreach (var gone in existing.Where(x => !keep.Contains(x.Key)).Select(x => x.Value).ToList())
                position.Contests.Remove(gone);
        }
    }
}
