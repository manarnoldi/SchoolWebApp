using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Project.Infrastructure.Data;
using SchoolWebApp.API.Services.Elections;
using SchoolWebApp.Core.DTOs.Elections;
using SchoolWebApp.Core.Entities.CBE.Responsibilities;
using SchoolWebApp.Core.Entities.Class;
using SchoolWebApp.Core.Entities.Elections;
using SchoolWebApp.Core.Entities.Enums;
using System.Text;

namespace SchoolWebApp.API.Controllers.Elections
{
    /// <summary>
    /// Results: the live count (elections officers, while polling), drawing lots for
    /// a tie, the declared results (everyone), CSV, and - after declaration - writing
    /// the winners into the students' leadership records.
    /// </summary>
    [Authorize]
    [Route("api/elections")]
    [ApiController]
    public class ElectionResultsController : ControllerBase
    {
        private readonly ApplicationDbContext _db;
        private readonly ElectionService _elections;
        private readonly ElectionResultsService _results;

        public ElectionResultsController(ApplicationDbContext db, ElectionService elections, ElectionResultsService results)
        {
            _db = db;
            _elections = elections;
            _results = results;
        }

        // GET api/elections/5/results
        [HttpGet("{id:int}/results")]
        public async Task<IActionResult> Results(int id)
        {
            var (e, error) = await ElectionForResultsAsync(id);
            if (error != null) return error;
            return Ok(await _results.CountAsync(e!));
        }

        // GET api/elections/5/results/csv
        [HttpGet("{id:int}/results/csv")]
        public async Task<IActionResult> ResultsCsv(int id)
        {
            var (e, error) = await ElectionForResultsAsync(id);
            if (error != null) return error;
            var r = await _results.CountAsync(e!);
            static string Q(string? s) => "\"" + (s ?? "").Replace("\"", "\"\"") + "\"";
            var sb = new StringBuilder();
            sb.AppendLine("Position,Race,Candidate,Running mate,Class,Gender,Votes,Percent,Result,Race voters,Race voted,Race ballots,Race blank");
            foreach (var race in r.Races)
                foreach (var c in race.Candidates)
                    sb.AppendLine(string.Join(",", Q(race.PositionName), Q(race.Name), Q(c.Name), Q(c.RunningMateName), Q(c.ClassName),
                        Q(c.GenderName), c.Votes, c.Percent, Q(c.Winning ? (r.Stage == ElectionStage.Declared ? "Elected" : "Leading") : c.InTie ? "Tied" : ""),
                        race.Voters, race.Voted, race.Ballots, race.Blank));
            var bytes = Encoding.UTF8.GetPreamble().Concat(Encoding.UTF8.GetBytes(sb.ToString())).ToArray();
            return File(bytes, "text/csv", $"{e!.Name} results.csv");
        }

        /// <summary>
        /// POST api/elections/contests/9/tiebreak - the lots drawn for a tie, as the
        /// tied candidates in the order drawn (first wins). Only once polling is closed.
        /// </summary>
        [HttpPost("contests/{contestId:int}/tiebreak")]
        public async Task<IActionResult> TieBreak(int contestId, [FromBody] TieBreakDto dto)
        {
            var me = await _elections.CurrentAsync();
            if (me == null) return Unauthorized();
            if (!me.CanManage) return Forbid();
            var contest = await _db.ElectionContests.Include(c => c.ElectionPosition).ThenInclude(p => p!.Election)
                .Include(c => c.Candidates).FirstOrDefaultAsync(c => c.Id == contestId);
            if (contest == null) return NotFound();
            var e = contest.ElectionPosition!.Election!;
            if (e.Stage != ElectionStage.PollingClosed) return BadRequest("Lots are drawn after polling closes and before the results are declared.");
            var ids = dto.CandidateIds.Distinct().ToList();
            if (ids.Count < 2 || ids.Any(id => !contest.Candidates.Any(c => c.Id == id && c.Status == CandidateStatus.Approved)))
                return BadRequest("List the tied candidates of this race in the order drawn.");
            foreach (var c in contest.Candidates) c.TieBreak = 0;
            for (var i = 0; i < ids.Count; i++) contest.Candidates.First(c => c.Id == ids[i]).TieBreak = ids.Count - i;
            await _db.SaveChangesAsync();
            return Ok(await _results.CountAsync(e));
        }

        // GET api/elections/5/leadership - after declaration: where each position's winners go
        [HttpGet("{id:int}/leadership")]
        public async Task<IActionResult> Leadership(int id)
        {
            var me = await _elections.CurrentAsync();
            if (me == null) return Unauthorized();
            if (!me.CanManage) return Forbid();
            var e = await _db.Elections.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
            if (e == null) return NotFound();
            return Ok(await LeadershipAsync(e));
        }

        /// <summary>
        /// POST api/elections/5/leadership - saves where each position's winners are
        /// recorded and records them: a class leadership role for class races, and/or
        /// a student responsibility for the year (the running mate too, under their
        /// title). Records that already exist are not added again.
        /// </summary>
        [HttpPost("{id:int}/leadership")]
        public async Task<IActionResult> RecordLeadership(int id, [FromBody] RecordLeadershipDto dto)
        {
            var me = await _elections.CurrentAsync();
            if (me == null) return Unauthorized();
            if (!me.CanManage) return Forbid();
            var e = await _db.Elections.FirstOrDefaultAsync(x => x.Id == id);
            if (e == null) return NotFound();
            if (e.Stage != ElectionStage.Declared) return BadRequest("Winners are recorded after the results are declared.");

            var positions = await _db.ElectionPositions
                .Include(p => p.Contests).ThenInclude(c => c.Candidates.Where(x => x.IsElected))
                .Where(p => p.ElectionId == id).ToListAsync();
            var studentRoles = await _db.ClassLeadershipRoles.Where(r => r.PersonType == PersonType.Student).Select(r => r.Id).ToListAsync();
            var responsibilities = await _db.Responsibilities.Select(r => r.Id).ToListAsync();
            foreach (var m in dto.Positions)
            {
                var p = positions.FirstOrDefault(x => x.Id == m.PositionId);
                if (p == null) return BadRequest("A position no longer exists.");
                if (m.ClassLeadershipRoleId != null && !studentRoles.Contains(m.ClassLeadershipRoleId.Value))
                    return BadRequest($"{p.Name}: choose a student class leadership role.");
                if (m.ClassLeadershipRoleId != null && p.Scope != PositionScope.PerClass)
                    return BadRequest($"{p.Name}: a class leadership role is only for class races.");
                if (m.ResponsibilityId != null && !responsibilities.Contains(m.ResponsibilityId.Value))
                    return BadRequest($"{p.Name}: the responsibility no longer exists.");
                p.ClassLeadershipRoleId = m.ClassLeadershipRoleId;
                p.ResponsibilityId = m.ResponsibilityId;
            }

            var added = 0;
            foreach (var p in positions.Where(p => p.ClassLeadershipRoleId != null || p.ResponsibilityId != null))
                foreach (var c in p.Contests)
                    foreach (var w in c.Candidates)
                    {
                        var note = $"{c.Name} - elected, {e.Name}";
                        if (p.ClassLeadershipRoleId != null && c.SchoolClassId != null
                            && !await _db.SchoolClassLeaders.AnyAsync(l => l.SchoolClassId == c.SchoolClassId && l.PersonId == w.StudentId
                                && l.ClassLeadershipRoleId == p.ClassLeadershipRoleId))
                        {
                            _db.SchoolClassLeaders.Add(new SchoolClassLeaders
                            {
                                SchoolClassId = c.SchoolClassId.Value,
                                PersonId = w.StudentId,
                                ClassLeadershipRoleId = p.ClassLeadershipRoleId.Value,
                                Description = note
                            });
                            added++;
                        }
                        if (p.ResponsibilityId != null)
                        {
                            added += await AddResponsibilityAsync(e.AcademicYearId, w.StudentId, p.ResponsibilityId.Value, note);
                            if (w.RunningMateStudentId != null)
                                added += await AddResponsibilityAsync(e.AcademicYearId, w.RunningMateStudentId.Value, p.ResponsibilityId.Value,
                                    $"{p.RunningMateTitle} ({c.Name}) - elected, {e.Name}");
                        }
                        w.LeadershipRecordedAt ??= DateTime.UtcNow;
                    }
            await _db.SaveChangesAsync();
            var result = await LeadershipAsync(e);
            return Ok(new { added, leadership = result });
        }

        private async Task<int> AddResponsibilityAsync(int yearId, int studentId, int responsibilityId, string note)
        {
            if (await _db.StudentResponsibilities.AnyAsync(r => r.AcademicYearId == yearId && r.StudentId == studentId
                    && r.ResponsibilitySocialSkillId == responsibilityId && r.Description == note))
                return 0;
            _db.StudentResponsibilities.Add(new StudentResponsibility
            {
                AcademicYearId = yearId,
                StudentId = studentId,
                ResponsibilitySocialSkillId = responsibilityId,
                Description = note
            });
            return 1;
        }

        private async Task<LeadershipDto> LeadershipAsync(Election e)
        {
            var positions = await _db.ElectionPositions.AsNoTracking()
                .Include(p => p.Contests).ThenInclude(c => c.Candidates.Where(x => x.IsElected)).ThenInclude(x => x.Student)
                .Include(p => p.Contests).ThenInclude(c => c.Candidates.Where(x => x.IsElected)).ThenInclude(x => x.RunningMateStudent)
                .Where(p => p.ElectionId == e.Id).AsSplitQuery()
                .OrderBy(p => p.Rank).ThenBy(p => p.Name).ToListAsync();
            var ids = positions.SelectMany(p => p.Contests).SelectMany(c => c.Candidates).Select(x => x.StudentId).ToList();
            var classes = await _db.StudentClasses.AsNoTracking()
                .Where(sc => ids.Contains(sc.StudentId) && sc.SchoolClass!.AcademicYearId == e.AcademicYearId)
                .Select(sc => new { sc.Id, sc.StudentId, sc.SchoolClass!.Name }).ToListAsync();
            var className = classes.GroupBy(x => x.StudentId).ToDictionary(g => g.Key, g => g.OrderByDescending(x => x.Id).First().Name);
            return new LeadershipDto
            {
                ElectionId = e.Id,
                ElectionName = e.Name,
                Stage = e.Stage,
                ClassRoles = await _db.ClassLeadershipRoles.AsNoTracking().Where(r => r.PersonType == PersonType.Student)
                    .OrderBy(r => r.Rank).Select(r => new IdNameDto { Id = r.Id, Name = r.Name }).ToListAsync(),
                Responsibilities = await _db.Responsibilities.AsNoTracking().OrderBy(r => r.Category).ThenBy(r => r.Rank).ThenBy(r => r.Name)
                    .Select(r => new IdNameDto { Id = r.Id, Name = r.Name + (r.Category == null ? "" : " (" + r.Category + ")") }).ToListAsync(),
                Positions = positions.Select(p => new LeadershipPositionDto
                {
                    PositionId = p.Id,
                    Name = p.Name,
                    RunningMateTitle = p.RunningMateTitle,
                    ClassRaces = p.Scope == PositionScope.PerClass,
                    ClassLeadershipRoleId = p.ClassLeadershipRoleId,
                    ResponsibilityId = p.ResponsibilityId,
                    Winners = p.Contests.OrderBy(c => c.Rank).SelectMany(c => c.Candidates.Select(w => new LeadershipWinnerDto
                    {
                        CandidateId = w.Id,
                        Race = c.Name,
                        Name = w.Student!.FullName,
                        RunningMateName = w.RunningMateStudent?.FullName,
                        ClassName = className.GetValueOrDefault(w.StudentId, ""),
                        RecordedAt = w.LeadershipRecordedAt
                    })).ToList()
                }).ToList()
            };
        }

        // Live counts are for elections officers until the results are declared; then for everyone.
        private async Task<(Election? Election, IActionResult? Error)> ElectionForResultsAsync(int id)
        {
            var me = await _elections.CurrentAsync();
            if (me == null) return (null, Unauthorized());
            var e = await _db.Elections.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
            if (e == null) return (null, NotFound());
            if (e.Stage != ElectionStage.Declared && !me.CanManage) return (null, Forbid());
            return (e, null);
        }
    }
}
