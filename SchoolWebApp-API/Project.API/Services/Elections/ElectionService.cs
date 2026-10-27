using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Project.Infrastructure.Data;
using SchoolWebApp.Core.Entities.Elections;
using SchoolWebApp.Core.Entities.Enums;
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

    /// <summary>Where a student sits for an election: their class, grade, section and gender.</summary>
    public record StudentPlace(int StudentId, string FullName, string Upi, int GenderId, int SchoolClassId,
        string ClassName, int LearningLevelId, int EducationLevelId);

    public enum ContestRole
    {
        Voter,
        Candidate,
        RunningMate
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
        /// The stages an election can move to from where it is: forward one step at a
        /// time; back to Draft from Nominations, to Nominations from Candidates final,
        /// and polling can reopen until the results are declared.
        /// </summary>
        public static List<ElectionStage> NextStages(ElectionStage stage) => stage switch
        {
            ElectionStage.Draft => new() { ElectionStage.Nominations },
            ElectionStage.Nominations => new() { ElectionStage.Draft, ElectionStage.CandidatesFinal },
            ElectionStage.CandidatesFinal => new() { ElectionStage.Nominations, ElectionStage.Polling },
            ElectionStage.Polling => new() { ElectionStage.PollingClosed },
            ElectionStage.PollingClosed => new() { ElectionStage.Polling, ElectionStage.Declared },
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
            if (to == ElectionStage.CandidatesFinal)
            {
                var contests = await _db.ElectionContests.Where(c => c.ElectionPosition!.ElectionId == e.Id)
                    .Select(c => new
                    {
                        c.Name,
                        Pending = c.Candidates.Count(x => x.Status == CandidateStatus.Nominated),
                        Approved = c.Candidates.Count(x => x.Status == CandidateStatus.Approved)
                    }).ToListAsync();
                var pending = contests.Sum(c => c.Pending);
                if (pending > 0)
                    return $"{pending} nomination(s) still need approving or rejecting.";
                var empty = contests.Where(c => c.Approved == 0).Select(c => c.Name).ToList();
                if (empty.Count > 0)
                    return $"These races have no approved candidate: {string.Join(", ", empty.Take(10))}{(empty.Count > 10 ? $" and {empty.Count - 10} more" : "")}.";
            }
            if (to == ElectionStage.Polling && e.Stage == ElectionStage.CandidatesFinal)
            {
                var uncovered = await UncoveredClassesAsync(e);
                if (!await _db.ElectionStations.AnyAsync(s => s.ElectionId == e.Id))
                    return "Set up the polling stations before opening polling.";
                if (uncovered.Count > 0)
                    return $"These classes have voters but no polling station: {string.Join(", ", uncovered.Select(c => c.Name))}.";
            }
            return null;
        }

        /// <summary>Classes with voters that no polling station of the election covers.</summary>
        public async Task<List<(int Id, string Name)>> UncoveredClassesAsync(Election e)
        {
            var covered = await _db.ElectionStationClasses.Where(c => c.ElectionStation!.ElectionId == e.Id)
                .Select(c => c.SchoolClassId).ToListAsync();
            return (await VoterPlacesAsync(e)).Where(p => !covered.Contains(p.SchoolClassId))
                .GroupBy(p => p.SchoolClassId).Select(g => (g.Key, g.First().ClassName)).OrderBy(c => c.Item2).ToList();
        }

        /// <summary>
        /// What a stage move does besides changing the stage. Closing nominations sets
        /// the ballot order (alphabetical) and, for a frozen register, takes the
        /// register; reopening them undoes both.
        /// </summary>
        public async Task ApplyStageAsync(Election e, ElectionStage to)
        {
            if (to == ElectionStage.CandidatesFinal)
            {
                var candidates = await _db.ElectionCandidates.Include(c => c.Student)
                    .Where(c => c.ElectionContest!.ElectionPosition!.ElectionId == e.Id && c.Status == CandidateStatus.Approved)
                    .ToListAsync();
                foreach (var race in candidates.GroupBy(c => c.ElectionContestId))
                {
                    var n = 0;
                    foreach (var c in race.OrderBy(c => c.Student!.FullName)) c.BallotOrder = ++n;
                }
                if (e.RegisterMode == VoterRegisterMode.Frozen)
                {
                    var places = await LivePlacesAsync(e.AcademicYearId, await VoterLevelIdsAsync(e.Id));
                    _db.ElectionVoters.AddRange(places.Select(p => new ElectionVoter
                    {
                        ElectionId = e.Id,
                        StudentId = p.StudentId,
                        SchoolClassId = p.SchoolClassId,
                        LearningLevelId = p.LearningLevelId,
                        GenderId = p.GenderId
                    }));
                    e.RegisterFrozenAt = DateTime.UtcNow;
                }
            }
            else if (e.Stage == ElectionStage.CandidatesFinal && to == ElectionStage.Nominations)
            {
                await _db.ElectionVoters.Where(v => v.ElectionId == e.Id).ExecuteDeleteAsync();
                e.RegisterFrozenAt = null;
                await _db.ElectionCandidates.Where(c => c.ElectionContest!.ElectionPosition!.ElectionId == e.Id)
                    .ExecuteUpdateAsync(s => s.SetProperty(c => c.BallotOrder, 0));
            }
        }

        public async Task<List<int>> VoterLevelIdsAsync(int electionId) =>
            await _db.ElectionPositionLevels
                .Where(l => l.ElectionPosition!.ElectionId == electionId && l.Role == PositionLevelRole.Voter)
                .Select(l => l.LearningLevelId).Distinct().ToListAsync();

        /// <summary>
        /// Active students in the given grades with their class for the year (the
        /// latest one if they moved class).
        /// </summary>
        public async Task<List<StudentPlace>> LivePlacesAsync(int academicYearId, ICollection<int> levelIds)
        {
            var rows = await _db.StudentClasses.AsNoTracking()
                .Where(sc => sc.SchoolClass!.AcademicYearId == academicYearId
                    && levelIds.Contains(sc.SchoolClass.LearningLevelId)
                    && sc.Student!.Status == Status.Active)
                .Select(sc => new
                {
                    sc.Id,
                    sc.StudentId,
                    sc.Student!.FullName,
                    sc.Student.UPI,
                    sc.Student.GenderId,
                    sc.SchoolClassId,
                    ClassName = sc.SchoolClass!.Name,
                    sc.SchoolClass.LearningLevelId,
                    sc.SchoolClass.LearningLevel!.EducationLevelId
                }).ToListAsync();
            return rows.GroupBy(r => r.StudentId).Select(g => g.OrderByDescending(r => r.Id).First())
                .Select(r => new StudentPlace(r.StudentId, r.FullName, r.UPI, r.GenderId, r.SchoolClassId,
                    r.ClassName, r.LearningLevelId, r.EducationLevelId))
                .OrderBy(p => p.FullName).ToList();
        }

        /// <summary>The voters: the frozen register once taken, otherwise students where they are now.</summary>
        public async Task<List<StudentPlace>> VoterPlacesAsync(Election e)
        {
            if (e.RegisterFrozenAt == null)
                return await LivePlacesAsync(e.AcademicYearId, await VoterLevelIdsAsync(e.Id));
            var rows = await _db.ElectionVoters.AsNoTracking().Where(v => v.ElectionId == e.Id)
                .Select(v => new StudentPlace(v.StudentId, v.Student!.FullName, v.Student.UPI, v.GenderId, v.SchoolClassId,
                    v.SchoolClass!.Name, v.LearningLevelId, v.SchoolClass.LearningLevel!.EducationLevelId))
                .ToListAsync();
            return rows.OrderBy(p => p.FullName).ToList();
        }

        /// <summary>
        /// Whether a student belongs in a race: a voter grade of the position and the
        /// race's section, grade or class. Candidates must also be from a candidate
        /// grade (when the position limits them); the gender rule applies to
        /// candidates and running mates, and to voters when only that gender votes.
        /// </summary>
        public static bool InContest(ElectionContest c, ElectionPosition p, StudentPlace s, ContestRole role)
        {
            if (!p.Levels.Any(l => l.Role == PositionLevelRole.Voter && l.LearningLevelId == s.LearningLevelId)) return false;
            if (c.SchoolClassId != null) { if (c.SchoolClassId != s.SchoolClassId) return false; }
            else if (c.LearningLevelId != null) { if (c.LearningLevelId != s.LearningLevelId) return false; }
            else if (c.EducationLevelId != null && c.EducationLevelId != s.EducationLevelId) return false;

            var oneGender = p.GenderRule == PositionGenderRule.SingleGender && p.GenderId != null;
            if (role == ContestRole.Voter) return !(oneGender && p.VotersSameGenderOnly && s.GenderId != p.GenderId);
            if (oneGender && s.GenderId != p.GenderId) return false;
            if (role == ContestRole.Candidate)
            {
                var candidateLevels = p.Levels.Where(l => l.Role == PositionLevelRole.Candidate).Select(l => l.LearningLevelId).ToList();
                if (candidateLevels.Count > 0 && !candidateLevels.Contains(s.LearningLevelId)) return false;
            }
            return true;
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
