using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Project.Infrastructure.Data;
using SchoolWebApp.API.Services.Elections;
using SchoolWebApp.Core.DTOs.Elections;
using SchoolWebApp.Core.Entities.Elections;

namespace SchoolWebApp.API.Controllers.Elections
{
    /// <summary>
    /// Elections set-up: the election, its electable positions (and the contests
    /// they split into) and its stage. Viewing is open to signed-in staff; changes
    /// need the Manage Elections permission.
    /// </summary>
    [Authorize]
    [Route("api/elections")]
    [ApiController]
    public class ElectionsController : ControllerBase
    {
        private readonly ApplicationDbContext _db;
        private readonly ElectionService _elections;

        public ElectionsController(ApplicationDbContext db, ElectionService elections)
        {
            _db = db;
            _elections = elections;
        }

        // GET api/elections/options - lookups for the set-up screens
        [HttpGet("options")]
        public async Task<IActionResult> GetOptions()
        {
            var me = await _elections.CurrentAsync();
            if (me == null) return Unauthorized();

            var years = await _db.AcademicYears.AsNoTracking().OrderByDescending(y => y.Rank).ThenByDescending(y => y.Name)
                .Select(y => new { y.Id, y.Name, y.Status }).ToListAsync();
            var sections = await _db.EducationLevels.AsNoTracking().OrderBy(e => e.Rank).ThenBy(e => e.Name)
                .Select(e => new ElectionSectionDto
                {
                    Id = e.Id,
                    Name = e.Name,
                    Grades = _db.LearningLevels.Where(l => l.EducationLevelId == e.Id).OrderBy(l => l.Rank).ThenBy(l => l.Name)
                        .Select(l => new IdNameDto { Id = l.Id, Name = l.Name }).ToList()
                }).ToListAsync();
            var genders = await _db.Genders.AsNoTracking().OrderBy(g => g.Rank).ThenBy(g => g.Name)
                .Select(g => new IdNameDto { Id = g.Id, Name = g.Name }).ToListAsync();

            var mode = await _elections.SettingAsync("VoterRegisterMode");
            var refresh = await _elections.SettingAsync("DashboardRefreshSeconds");
            return Ok(new ElectionOptionsDto
            {
                CanManage = me.CanManage,
                AcademicYears = years.Select(y => new IdNameDto { Id = y.Id, Name = y.Name }).ToList(),
                CurrentAcademicYearId = years.FirstOrDefault(y => y.Status)?.Id ?? years.FirstOrDefault()?.Id,
                Sections = sections.Where(s => s.Grades.Count > 0).ToList(),
                Genders = genders,
                DefaultRegisterMode = string.Equals(mode, "Live", StringComparison.OrdinalIgnoreCase) ? VoterRegisterMode.Live : VoterRegisterMode.Frozen,
                DefaultRefreshSeconds = int.TryParse(refresh, out var s) && s >= 3 ? s : 10
            });
        }

        // GET api/elections
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            if (await _elections.CurrentAsync() == null) return Unauthorized();
            var list = await _db.Elections.AsNoTracking()
                .OrderByDescending(e => e.Created)
                .Select(e => new ElectionSummaryDto
                {
                    Id = e.Id,
                    Name = e.Name,
                    ElectionType = e.ElectionType,
                    AcademicYearId = e.AcademicYearId,
                    AcademicYearName = e.AcademicYear!.Name,
                    Stage = e.Stage,
                    Positions = e.Positions.Count,
                    Contests = e.Positions.SelectMany(p => p.Contests).Count(),
                    PollingStart = e.PollingStart,
                    PollingEnd = e.PollingEnd
                }).ToListAsync();
            return Ok(list);
        }

        // GET api/elections/5
        [HttpGet("{id:int}")]
        public async Task<IActionResult> Get(int id)
        {
            if (await _elections.CurrentAsync() == null) return Unauthorized();
            var dto = await DetailAsync(id);
            return dto == null ? NotFound() : Ok(dto);
        }

        // POST api/elections
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] SaveElectionDto dto)
        {
            var me = await _elections.CurrentAsync();
            if (me == null) return Unauthorized();
            if (!me.CanManage) return Forbid();
            var error = await ValidateAsync(dto);
            if (error != null) return BadRequest(error);

            var e = new Election { Name = dto.Name.Trim() };
            Apply(e, dto);
            _db.Elections.Add(e);
            await _db.SaveChangesAsync();
            return Ok(await DetailAsync(e.Id));
        }

        // PUT api/elections/5
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] SaveElectionDto dto)
        {
            var me = await _elections.CurrentAsync();
            if (me == null) return Unauthorized();
            if (!me.CanManage) return Forbid();
            var e = await _db.Elections.FirstOrDefaultAsync(x => x.Id == id);
            if (e == null) return NotFound();
            var error = await ValidateAsync(dto);
            if (error != null) return BadRequest(error);
            if (e.Stage >= ElectionStage.Declared) return BadRequest("The results are declared; the election can no longer change.");
            // The year decides the classes the contests are drawn from.
            if (dto.AcademicYearId != e.AcademicYearId && e.Stage != ElectionStage.Draft)
                return BadRequest("The academic year can only change while the election is a draft.");
            // Changing the register rule after the register is taken would make it mean something else.
            if (dto.RegisterMode != e.RegisterMode && e.RegisterFrozenAt != null)
                return BadRequest("The voter register is already taken; the register mode can no longer change.");

            var yearChanged = dto.AcademicYearId != e.AcademicYearId;
            Apply(e, dto);
            if (yearChanged)
            {
                var positions = await _db.ElectionPositions.Include(p => p.Levels).Include(p => p.Contests)
                    .Where(p => p.ElectionId == e.Id && p.Scope == PositionScope.PerClass).ToListAsync();
                foreach (var p in positions) await _elections.SyncContestsAsync(p, e.AcademicYearId);
            }
            await _db.SaveChangesAsync();
            return Ok(await DetailAsync(e.Id));
        }

        // DELETE api/elections/5 - drafts only
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var me = await _elections.CurrentAsync();
            if (me == null) return Unauthorized();
            if (!me.CanManage) return Forbid();
            var e = await _db.Elections.FirstOrDefaultAsync(x => x.Id == id);
            if (e == null) return NotFound();
            if (e.Stage != ElectionStage.Draft) return BadRequest("Only a draft election can be deleted.");
            _db.Elections.Remove(e);
            await _db.SaveChangesAsync();
            return NoContent();
        }

        // POST api/elections/5/stage
        [HttpPost("{id:int}/stage")]
        public async Task<IActionResult> ChangeStage(int id, [FromBody] ChangeStageDto dto)
        {
            var me = await _elections.CurrentAsync();
            if (me == null) return Unauthorized();
            if (!me.CanManage) return Forbid();
            var e = await _db.Elections.FirstOrDefaultAsync(x => x.Id == id);
            if (e == null) return NotFound();
            var error = await _elections.CannotMoveAsync(e, dto.ToStage);
            if (error != null) return BadRequest(error);

            _db.ElectionStageChanges.Add(new ElectionStageChange
            {
                ElectionId = e.Id,
                FromStage = e.Stage,
                ToStage = dto.ToStage,
                Note = string.IsNullOrWhiteSpace(dto.Note) ? null : dto.Note.Trim()
            });
            e.Stage = dto.ToStage;
            await _db.SaveChangesAsync();
            return Ok(await DetailAsync(e.Id));
        }

        // POST api/elections/5/positions
        [HttpPost("{id:int}/positions")]
        public async Task<IActionResult> AddPosition(int id, [FromBody] SavePositionDto dto)
        {
            var me = await _elections.CurrentAsync();
            if (me == null) return Unauthorized();
            if (!me.CanManage) return Forbid();
            var e = await _db.Elections.FirstOrDefaultAsync(x => x.Id == id);
            if (e == null) return NotFound();
            if (!ElectionService.PositionsEditable(e.Stage))
                return BadRequest("Positions can only change before the candidates are final.");
            var error = await ValidatePositionAsync(dto);
            if (error != null) return BadRequest(error);

            var p = new ElectionPosition { ElectionId = e.Id, Name = dto.Name.Trim() };
            if (dto.Rank <= 0)
                dto.Rank = (await _db.ElectionPositions.Where(x => x.ElectionId == e.Id).MaxAsync(x => (int?)x.Rank) ?? 0) + 1;
            ApplyPosition(p, dto);
            await _elections.SyncContestsAsync(p, e.AcademicYearId);
            _db.ElectionPositions.Add(p);
            await _db.SaveChangesAsync();
            return Ok(await DetailAsync(e.Id));
        }

        // PUT api/elections/positions/7
        [HttpPut("positions/{positionId:int}")]
        public async Task<IActionResult> UpdatePosition(int positionId, [FromBody] SavePositionDto dto)
        {
            var me = await _elections.CurrentAsync();
            if (me == null) return Unauthorized();
            if (!me.CanManage) return Forbid();
            var p = await _db.ElectionPositions.Include(x => x.Election).Include(x => x.Levels).Include(x => x.Contests)
                .FirstOrDefaultAsync(x => x.Id == positionId);
            if (p == null) return NotFound();
            if (!ElectionService.PositionsEditable(p.Election!.Stage))
                return BadRequest("Positions can only change before the candidates are final.");
            var error = await ValidatePositionAsync(dto);
            if (error != null) return BadRequest(error);

            p.Name = dto.Name.Trim();
            if (dto.Rank <= 0) dto.Rank = p.Rank;
            ApplyPosition(p, dto);
            await _elections.SyncContestsAsync(p, p.Election.AcademicYearId);
            await _db.SaveChangesAsync();
            return Ok(await DetailAsync(p.ElectionId));
        }

        // DELETE api/elections/positions/7
        [HttpDelete("positions/{positionId:int}")]
        public async Task<IActionResult> DeletePosition(int positionId)
        {
            var me = await _elections.CurrentAsync();
            if (me == null) return Unauthorized();
            if (!me.CanManage) return Forbid();
            var p = await _db.ElectionPositions.Include(x => x.Election).FirstOrDefaultAsync(x => x.Id == positionId);
            if (p == null) return NotFound();
            if (!ElectionService.PositionsEditable(p.Election!.Stage))
                return BadRequest("Positions can only change before the candidates are final.");
            _db.ElectionPositions.Remove(p);
            await _db.SaveChangesAsync();
            return Ok(await DetailAsync(p.ElectionId));
        }

        private static void Apply(Election e, SaveElectionDto dto)
        {
            e.Name = dto.Name.Trim();
            e.Description = string.IsNullOrWhiteSpace(dto.Description) ? null : dto.Description.Trim();
            e.AcademicYearId = dto.AcademicYearId;
            e.RegisterMode = dto.RegisterMode;
            e.PollingStart = dto.PollingStart;
            e.PollingEnd = dto.PollingEnd;
            e.DashboardRefreshSeconds = dto.DashboardRefreshSeconds;
        }

        private async Task<string?> ValidateAsync(SaveElectionDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Name)) return "Enter the election name.";
            if (!await _db.AcademicYears.AnyAsync(y => y.Id == dto.AcademicYearId)) return "Choose the academic year.";
            if (!Enum.IsDefined(dto.RegisterMode)) return "Choose the voter register mode.";
            if (dto.DashboardRefreshSeconds < 3 || dto.DashboardRefreshSeconds > 600)
                return "The results screen refresh must be between 3 and 600 seconds.";
            if (dto.PollingStart != null && dto.PollingEnd != null && dto.PollingEnd <= dto.PollingStart)
                return "Polling must end after it starts.";
            return null;
        }

        private static void ApplyPosition(ElectionPosition p, SavePositionDto dto)
        {
            p.RunningMateTitle = string.IsNullOrWhiteSpace(dto.RunningMateTitle) ? null : dto.RunningMateTitle.Trim();
            p.Scope = dto.Scope;
            p.Seats = dto.Seats;
            p.GenderRule = dto.GenderRule;
            p.GenderId = dto.GenderRule == PositionGenderRule.SingleGender ? dto.GenderId : null;
            p.VotersSameGenderOnly = dto.GenderRule == PositionGenderRule.SingleGender && dto.VotersSameGenderOnly;
            p.Rank = dto.Rank;

            // Candidate grades the same as the voter grades are stored as none (= any voter grade).
            var voters = dto.VoterLevelIds.Distinct().ToHashSet();
            var candidates = dto.CandidateLevelIds.Distinct().Where(voters.Contains).ToHashSet();
            if (candidates.SetEquals(voters)) candidates.Clear();
            SyncLevels(p, PositionLevelRole.Voter, voters);
            SyncLevels(p, PositionLevelRole.Candidate, candidates);
        }

        private static void SyncLevels(ElectionPosition p, PositionLevelRole role, HashSet<int> levelIds)
        {
            foreach (var gone in p.Levels.Where(l => l.Role == role && !levelIds.Contains(l.LearningLevelId)).ToList())
                p.Levels.Remove(gone);
            foreach (var id in levelIds.Where(id => !p.Levels.Any(l => l.Role == role && l.LearningLevelId == id)))
                p.Levels.Add(new ElectionPositionLevel { LearningLevelId = id, Role = role });
        }

        private async Task<string?> ValidatePositionAsync(SavePositionDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Name)) return "Enter the position name.";
            if (!Enum.IsDefined(dto.Scope)) return "Choose how the position is contested.";
            if (!Enum.IsDefined(dto.GenderRule)) return "Choose the gender rule.";
            if (dto.Seats < 1 || dto.Seats > 20) return "Seats must be between 1 and 20.";
            if (dto.VoterLevelIds.Count == 0) return "Choose the grades that vote for this position.";
            var ids = dto.VoterLevelIds.Concat(dto.CandidateLevelIds).Distinct().ToList();
            if (await _db.LearningLevels.CountAsync(l => ids.Contains(l.Id)) != ids.Count) return "One of the grades no longer exists.";
            if (dto.CandidateLevelIds.Any(id => !dto.VoterLevelIds.Contains(id)))
                return "Candidates must come from the grades that vote for the position.";
            if (dto.GenderRule == PositionGenderRule.SingleGender &&
                (dto.GenderId == null || !await _db.Genders.AnyAsync(g => g.Id == dto.GenderId)))
                return "Choose the gender the candidates must be.";
            if (!string.IsNullOrWhiteSpace(dto.RunningMateTitle))
            {
                if (dto.Seats != 1) return "A position with a running mate elects one ticket - set seats to 1.";
                if (dto.GenderRule == PositionGenderRule.SeatsPerGender)
                    return "A position with a running mate cannot also elect seats per gender.";
            }
            return null;
        }

        private async Task<ElectionDetailDto?> DetailAsync(int id)
        {
            var e = await _db.Elections.AsNoTracking()
                .Include(x => x.AcademicYear)
                .Include(x => x.StageChanges)
                .Include(x => x.Positions).ThenInclude(p => p.Levels)
                .Include(x => x.Positions).ThenInclude(p => p.Contests)
                .Include(x => x.Positions).ThenInclude(p => p.Gender)
                .AsSplitQuery()
                .FirstOrDefaultAsync(x => x.Id == id);
            if (e == null) return null;

            return new ElectionDetailDto
            {
                Id = e.Id,
                Name = e.Name,
                Description = e.Description,
                ElectionType = e.ElectionType,
                AcademicYearId = e.AcademicYearId,
                AcademicYearName = e.AcademicYear?.Name ?? "",
                Stage = e.Stage,
                RegisterMode = e.RegisterMode,
                RegisterFrozenAt = e.RegisterFrozenAt,
                PollingStart = e.PollingStart,
                PollingEnd = e.PollingEnd,
                DashboardRefreshSeconds = e.DashboardRefreshSeconds,
                DeclaredAt = e.DeclaredAt,
                DeclaredBy = e.DeclaredBy,
                Positions = e.Positions.Count,
                Contests = e.Positions.Sum(p => p.Contests.Count),
                PositionsEditable = ElectionService.PositionsEditable(e.Stage),
                NextStages = ElectionService.NextStages(e.Stage),
                PositionList = e.Positions.OrderBy(p => p.Rank).ThenBy(p => p.Name).Select(p => new ElectionPositionDto
                {
                    Id = p.Id,
                    Name = p.Name,
                    RunningMateTitle = p.RunningMateTitle,
                    Scope = p.Scope,
                    Seats = p.Seats,
                    GenderRule = p.GenderRule,
                    GenderId = p.GenderId,
                    GenderName = p.Gender?.Name,
                    VotersSameGenderOnly = p.VotersSameGenderOnly,
                    Rank = p.Rank,
                    VoterLevelIds = p.Levels.Where(l => l.Role == PositionLevelRole.Voter).Select(l => l.LearningLevelId).ToList(),
                    CandidateLevelIds = p.Levels.Where(l => l.Role == PositionLevelRole.Candidate).Select(l => l.LearningLevelId).ToList(),
                    Contests = p.Contests.OrderBy(c => c.Rank).Select(c => new ElectionContestDto
                    {
                        Id = c.Id,
                        Name = c.Name,
                        EducationLevelId = c.EducationLevelId,
                        LearningLevelId = c.LearningLevelId,
                        SchoolClassId = c.SchoolClassId
                    }).ToList()
                }).ToList(),
                History = e.StageChanges.OrderByDescending(s => s.Created).Select(s => new ElectionStageChangeDto
                {
                    FromStage = s.FromStage,
                    ToStage = s.ToStage,
                    Note = s.Note,
                    At = s.Created,
                    By = s.CreatedBy
                }).ToList()
            };
        }
    }
}
