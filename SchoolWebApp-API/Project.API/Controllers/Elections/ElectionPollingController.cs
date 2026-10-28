using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Cryptography;
using Microsoft.EntityFrameworkCore;
using Project.Infrastructure.Data;
using SchoolWebApp.API.Services.Elections;
using SchoolWebApp.Core.DTOs.Elections;
using SchoolWebApp.Core.Entities.Elections;
using SchoolWebApp.Core.Entities.Enums;

namespace SchoolWebApp.API.Controllers.Elections
{
    /// <summary>
    /// Polling: the stations (classes and their returning officer), the station
    /// screen where the officer checks a voter in and issues the ballot, and
    /// casting. Who voted and how they voted are stored apart; the choices carry
    /// no voter, time or user.
    /// </summary>
    [Authorize]
    [Route("api/elections")]
    [ApiController]
    public class ElectionPollingController : ControllerBase
    {
        private readonly ApplicationDbContext _db;
        private readonly ElectionService _elections;
        private readonly ILogger<ElectionPollingController> _logger;

        public ElectionPollingController(ApplicationDbContext db, ElectionService elections, ILogger<ElectionPollingController> logger)
        {
            _db = db;
            _elections = elections;
            _logger = logger;
        }

        // Stations can be set up from nominations until polling closes.
        private static bool StationsEditable(ElectionStage s) => s >= ElectionStage.Nominations && s <= ElectionStage.Polling;

        // GET api/elections/5/stations
        [HttpGet("{id:int}/stations")]
        public async Task<IActionResult> Stations(int id)
        {
            var me = await _elections.CurrentAsync();
            if (me == null) return Unauthorized();
            var e = await _db.Elections.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
            if (e == null) return NotFound();
            return Ok(await StationsAsync(e, me.CanManage));
        }

        // POST api/elections/5/stations
        [HttpPost("{id:int}/stations")]
        public async Task<IActionResult> AddStation(int id, [FromBody] SaveStationDto dto)
        {
            var me = await _elections.CurrentAsync();
            if (me == null) return Unauthorized();
            if (!me.CanManage) return Forbid();
            var e = await _db.Elections.FirstOrDefaultAsync(x => x.Id == id);
            if (e == null) return NotFound();
            if (!StationsEditable(e.Stage)) return BadRequest("Stations can be set up from nominations until polling closes.");
            var error = await ValidateStationAsync(e, dto, null);
            if (error != null) return BadRequest(error);

            var s = new ElectionStation { ElectionId = id, Name = dto.Name.Trim(), OfficerUserId = dto.OfficerUserId };
            s.Classes.AddRange(dto.SchoolClassIds.Distinct().Select(c => new ElectionStationClass { SchoolClassId = c }));
            _db.ElectionStations.Add(s);
            await _db.SaveChangesAsync();
            return Ok(await StationsAsync(e, true));
        }

        // PUT api/elections/stations/3
        [HttpPut("stations/{stationId:int}")]
        public async Task<IActionResult> UpdateStation(int stationId, [FromBody] SaveStationDto dto)
        {
            var me = await _elections.CurrentAsync();
            if (me == null) return Unauthorized();
            if (!me.CanManage) return Forbid();
            var s = await _db.ElectionStations.Include(x => x.Election).Include(x => x.Classes).FirstOrDefaultAsync(x => x.Id == stationId);
            if (s == null) return NotFound();
            if (!StationsEditable(s.Election!.Stage)) return BadRequest("Stations can be set up from nominations until polling closes.");
            var error = await ValidateStationAsync(s.Election, dto, s.Id);
            if (error != null) return BadRequest(error);
            // A class whose students have voted here stays here, so the station's count stays whole.
            var removed = s.Classes.Where(c => !dto.SchoolClassIds.Contains(c.SchoolClassId)).Select(c => c.SchoolClassId).ToList();
            if (removed.Count > 0 && await _db.ElectionVoteCasts.AnyAsync(v => v.ElectionStationId == s.Id))
            {
                var places = await _elections.VoterPlacesAsync(s.Election);
                var votedHere = await _db.ElectionVoteCasts.Where(v => v.ElectionStationId == s.Id).Select(v => v.StudentId).ToListAsync();
                if (places.Any(p => removed.Contains(p.SchoolClassId) && votedHere.Contains(p.StudentId)))
                    return BadRequest("Students of a class you are removing have already voted at this station.");
            }

            s.Name = dto.Name.Trim();
            s.OfficerUserId = dto.OfficerUserId;
            s.Classes.RemoveAll(c => removed.Contains(c.SchoolClassId));
            foreach (var c in dto.SchoolClassIds.Distinct().Where(c => !s.Classes.Any(x => x.SchoolClassId == c)))
                s.Classes.Add(new ElectionStationClass { SchoolClassId = c });
            await _db.SaveChangesAsync();
            return Ok(await StationsAsync(s.Election, true));
        }

        // DELETE api/elections/stations/3
        [HttpDelete("stations/{stationId:int}")]
        public async Task<IActionResult> DeleteStation(int stationId)
        {
            var me = await _elections.CurrentAsync();
            if (me == null) return Unauthorized();
            if (!me.CanManage) return Forbid();
            var s = await _db.ElectionStations.Include(x => x.Election).FirstOrDefaultAsync(x => x.Id == stationId);
            if (s == null) return NotFound();
            if (!StationsEditable(s.Election!.Stage)) return BadRequest("Stations can be set up from nominations until polling closes.");
            if (await _db.ElectionVoteCasts.AnyAsync(v => v.ElectionStationId == s.Id))
                return BadRequest("Votes have been cast at this station; it cannot be removed.");
            _db.ElectionStations.Remove(s);
            await _db.SaveChangesAsync();
            return Ok(await StationsAsync(s.Election, true));
        }

        /// <summary>
        /// POST api/elections/5/stations/generate - one station per class not yet
        /// covered, named after the class, with its class teacher (the staff class
        /// leadership role) as returning officer when they have a login.
        /// </summary>
        [HttpPost("{id:int}/stations/generate")]
        public async Task<IActionResult> Generate(int id)
        {
            var me = await _elections.CurrentAsync();
            if (me == null) return Unauthorized();
            if (!me.CanManage) return Forbid();
            var e = await _db.Elections.FirstOrDefaultAsync(x => x.Id == id);
            if (e == null) return NotFound();
            if (!StationsEditable(e.Stage)) return BadRequest("Stations can be set up from nominations until polling closes.");

            var uncovered = await _elections.UncoveredClassesAsync(e);
            var classIds = uncovered.Select(c => c.Id).ToList();
            var teachers = await _db.SchoolClassLeaders.AsNoTracking()
                .Where(l => classIds.Contains(l.SchoolClassId) && l.ClassLeadershipRole!.PersonType == PersonType.Teacher)
                .Select(l => new { l.SchoolClassId, l.PersonId, l.Id }).ToListAsync();
            var personIds = teachers.Select(t => t.PersonId).ToList();
            var users = await _db.Users.AsNoTracking().Where(u => u.PersonId != null && personIds.Contains(u.PersonId.Value))
                .Select(u => new { u.Id, u.PersonId }).ToListAsync();
            foreach (var (classId, name) in uncovered)
            {
                var teacher = teachers.Where(t => t.SchoolClassId == classId).OrderBy(t => t.Id).FirstOrDefault();
                var s = new ElectionStation
                {
                    ElectionId = id,
                    Name = name,
                    OfficerUserId = teacher == null ? null : users.FirstOrDefault(u => u.PersonId == teacher.PersonId)?.Id
                };
                s.Classes.Add(new ElectionStationClass { SchoolClassId = classId });
                _db.ElectionStations.Add(s);
            }
            await _db.SaveChangesAsync();
            return Ok(await StationsAsync(e, true));
        }

        // GET api/elections/polling/mine - the stations the caller runs
        [HttpGet("polling/mine")]
        public async Task<IActionResult> MyStations()
        {
            var me = await _elections.CurrentAsync();
            if (me == null) return Unauthorized();
            var stations = await _db.ElectionStations.AsNoTracking()
                .Include(s => s.Election).Include(s => s.OfficerUser).Include(s => s.Classes)
                .Where(s => s.Election!.Stage >= ElectionStage.CandidatesFinal && s.Election.Stage <= ElectionStage.PollingClosed
                    && (me.CanManage || s.OfficerUserId == me.UserId))
                .OrderByDescending(s => s.Election!.Stage == ElectionStage.Polling).ThenBy(s => s.Election!.Name).ThenBy(s => s.Name)
                .ToListAsync();

            var result = new List<MyStationDto>();
            foreach (var group in stations.GroupBy(s => s.ElectionId))
            {
                var e = group.First().Election!;
                var places = await _elections.VoterPlacesAsync(e);
                var casts = await _db.ElectionVoteCasts.Where(v => v.ElectionId == e.Id)
                    .GroupBy(v => v.ElectionStationId).Select(g => new { g.Key, Count = g.Count() }).ToListAsync();
                result.AddRange(group.Select(s => new MyStationDto
                {
                    StationId = s.Id,
                    StationName = s.Name,
                    ElectionId = e.Id,
                    ElectionName = e.Name,
                    Stage = e.Stage,
                    OfficerName = OfficerName(s),
                    Voters = places.Count(p => s.Classes.Any(c => c.SchoolClassId == p.SchoolClassId)),
                    Voted = casts.FirstOrDefault(c => c.Key == s.Id)?.Count ?? 0
                }));
            }
            return Ok(result);
        }

        // GET api/elections/stations/3/register - the station screen's voter list
        [HttpGet("stations/{stationId:int}/register")]
        public async Task<IActionResult> StationRegister(int stationId)
        {
            var (s, error) = await StationForCallerAsync(stationId);
            if (error != null) return error;
            var voters = await StationVotersAsync(s!);
            var casts = await _db.ElectionVoteCasts.AsNoTracking().Where(v => v.ElectionId == s!.ElectionId)
                .Select(v => new { v.StudentId, v.Created }).ToListAsync();
            var genders = await _db.Genders.AsNoTracking().ToDictionaryAsync(g => g.Id, g => g.Name);
            return Ok(new StationRegisterDto
            {
                StationId = s!.Id,
                StationName = s.Name,
                ElectionId = s.ElectionId,
                ElectionName = s.Election!.Name,
                Stage = s.Election.Stage,
                OfficerName = OfficerName(s),
                Voters = voters.OrderBy(v => v.ClassName).ThenBy(v => v.FullName).Select(v =>
                {
                    var cast = casts.FirstOrDefault(c => c.StudentId == v.StudentId);
                    return new StationVoterDto
                    {
                        StudentId = v.StudentId,
                        Name = v.FullName,
                        Upi = v.Upi,
                        ClassName = v.ClassName,
                        GenderName = genders.GetValueOrDefault(v.GenderId, ""),
                        Voted = cast != null,
                        VotedAt = cast?.Created
                    };
                }).ToList()
            });
        }

        // GET api/elections/stations/3/ballot/77 - the races a voter votes in, with their candidates
        [HttpGet("stations/{stationId:int}/ballot/{studentId:int}")]
        public async Task<IActionResult> Ballot(int stationId, int studentId)
        {
            var (s, error) = await StationForCallerAsync(stationId);
            if (error != null) return error;
            if (s!.Election!.Stage != ElectionStage.Polling) return BadRequest("Polling is not open.");
            var voter = (await StationVotersAsync(s)).FirstOrDefault(v => v.StudentId == studentId);
            if (voter == null) return BadRequest("That student is not on this station's register.");
            if (await _db.ElectionVoteCasts.AnyAsync(v => v.ElectionId == s.ElectionId && v.StudentId == studentId))
                return BadRequest($"{voter.FullName} has already voted.");

            var races = await RacesForAsync(s.ElectionId, voter);
            var genders = await _db.Genders.AsNoTracking().ToDictionaryAsync(g => g.Id, g => g.Name);
            var ids = races.SelectMany(r => r.Candidates).Select(c => c.StudentId).ToList();
            var classes = await _db.StudentClasses.AsNoTracking()
                .Where(sc => ids.Contains(sc.StudentId) && sc.SchoolClass!.AcademicYearId == s.Election.AcademicYearId)
                .Select(sc => new { sc.Id, sc.StudentId, sc.SchoolClass!.Name }).ToListAsync();
            var className = classes.GroupBy(x => x.StudentId).ToDictionary(g => g.Key, g => g.OrderByDescending(x => x.Id).First().Name);
            static string? Url(string? f) => f == null ? null : $"/elections/photos/{f}";
            var shuffle = await _elections.SettingAsync(ElectionService.BallotOrderSetting) == ElectionService.BallotOrderRandom;

            return Ok(new BallotDto
            {
                StudentId = voter.StudentId,
                StudentName = voter.FullName,
                ClassName = voter.ClassName,
                Races = races.Select(c => new BallotRaceDto
                {
                    ContestId = c.Id,
                    Name = c.Name,
                    PositionName = c.ElectionPosition!.Name,
                    RunningMateTitle = c.ElectionPosition.RunningMateTitle,
                    Seats = c.ElectionPosition.Seats,
                    SeatsPerGender = c.ElectionPosition.GenderRule == PositionGenderRule.SeatsPerGender,
                    Candidates = Order(c.Candidates.OrderBy(x => x.BallotOrder).Select(x => new BallotCandidateDto
                    {
                        CandidateId = x.Id,
                        BallotOrder = x.BallotOrder,
                        Name = x.Student!.FullName,
                        ClassName = className.GetValueOrDefault(x.StudentId, ""),
                        GenderName = genders.GetValueOrDefault(x.Student.GenderId, ""),
                        RunningMateName = x.RunningMateStudent?.FullName,
                        Slogan = x.Slogan,
                        PhotoUrl = Url(x.PhotoFile),
                        RunningMatePhotoUrl = Url(x.RunningMatePhotoFile)
                    }).ToList(), shuffle)
                }).ToList()
            });
        }

        // Alphabetical (the order fixed when nominations closed), or a fresh random
        // order for each ballot issued, numbered as shown - per the Elections settings.
        private static List<BallotCandidateDto> Order(List<BallotCandidateDto> candidates, bool shuffle)
        {
            if (!shuffle) return candidates;
            for (var i = candidates.Count - 1; i > 0; i--)
            {
                var j = RandomNumberGenerator.GetInt32(i + 1);
                (candidates[i], candidates[j]) = (candidates[j], candidates[i]);
            }
            for (var i = 0; i < candidates.Count; i++) candidates[i].BallotOrder = i + 1;
            return candidates;
        }

        // POST api/elections/stations/3/cast
        [HttpPost("stations/{stationId:int}/cast")]
        public async Task<IActionResult> Cast(int stationId, [FromBody] CastBallotDto dto)
        {
            var (s, error) = await StationForCallerAsync(stationId);
            if (error != null) return error;
            if (s!.Election!.Stage != ElectionStage.Polling) return BadRequest("Polling is not open.");
            var voter = (await StationVotersAsync(s)).FirstOrDefault(v => v.StudentId == dto.StudentId);
            if (voter == null) return BadRequest("That student is not on this station's register.");

            var races = await RacesForAsync(s.ElectionId, voter);
            var genders = await _db.Genders.AsNoTracking().ToDictionaryAsync(g => g.Id, g => g.Name);
            var picks = dto.Choices.GroupBy(c => c.ContestId).ToDictionary(g => g.Key, g => g.SelectMany(c => c.CandidateIds).Distinct().ToList());
            if (picks.Keys.Any(k => !races.Any(r => r.Id == k))) return BadRequest("The ballot has a race this voter does not vote in.");

            // Check every race: candidates on its ballot, no more than its seats (per gender when seats are per gender).
            foreach (var r in races)
            {
                var chosen = picks.GetValueOrDefault(r.Id) ?? new List<int>();
                var onBallot = r.Candidates.ToDictionary(c => c.Id);
                if (chosen.Any(id => !onBallot.ContainsKey(id))) return BadRequest($"{r.Name}: a choice is not on the ballot.");
                var p = r.ElectionPosition!;
                var tooMany = p.GenderRule == PositionGenderRule.SeatsPerGender
                    ? chosen.GroupBy(id => onBallot[id].Student!.GenderId).Any(g => g.Count() > p.Seats)
                    : chosen.Count > p.Seats;
                if (tooMany) return BadRequest($"{r.Name}: more choices than seats.");
            }

            var ballotId = Guid.NewGuid();
            var choices = races.SelectMany(r =>
            {
                var chosen = picks.GetValueOrDefault(r.Id) ?? new List<int>();
                return chosen.Count == 0
                    ? new[] { new ElectionBallotChoice { Id = Guid.NewGuid(), ElectionId = s.ElectionId, BallotId = ballotId, ElectionContestId = r.Id } }
                    : chosen.Select(id => new ElectionBallotChoice { Id = Guid.NewGuid(), ElectionId = s.ElectionId, BallotId = ballotId, ElectionContestId = r.Id, ElectionCandidateId = id }).ToArray();
            }).ToList();

            // Who voted and what they chose are saved together or not at all; the
            // unique (election, student) index stops a second vote racing the first.
            await using var tx = await _db.Database.BeginTransactionAsync();
            try
            {
                _db.ElectionVoteCasts.Add(new ElectionVoteCast { ElectionId = s.ElectionId, StudentId = voter.StudentId, ElectionStationId = s.Id });
                _db.ElectionBallotChoices.AddRange(choices);
                await _db.SaveChangesAsync();
                await tx.CommitAsync();
            }
            catch (DbUpdateException ex)
            {
                await tx.RollbackAsync();
                _db.ChangeTracker.Clear();
                if (await _db.ElectionVoteCasts.AnyAsync(v => v.ElectionId == s.ElectionId && v.StudentId == voter.StudentId))
                    return BadRequest($"{voter.FullName} has already voted.");
                _logger.LogError(ex, "Casting a ballot at station {StationId} failed", s.Id);
                return StatusCode(500, "The vote could not be saved. Try again.");
            }
            return Ok(new { voted = voter.FullName, races = races.Count });
        }

        // The station, if the caller runs it (its officer) or manages elections.
        private async Task<(ElectionStation? Station, IActionResult? Error)> StationForCallerAsync(int stationId)
        {
            var me = await _elections.CurrentAsync();
            if (me == null) return (null, Unauthorized());
            var s = await _db.ElectionStations.AsNoTracking().Include(x => x.Election).Include(x => x.Classes).Include(x => x.OfficerUser)
                .FirstOrDefaultAsync(x => x.Id == stationId);
            if (s == null) return (null, NotFound());
            if (!me.CanManage && s.OfficerUserId != me.UserId) return (null, Forbid());
            return (s, null);
        }

        private async Task<List<StudentPlace>> StationVotersAsync(ElectionStation s)
        {
            var classIds = s.Classes.Select(c => c.SchoolClassId).ToHashSet();
            return (await _elections.VoterPlacesAsync(s.Election!)).Where(p => classIds.Contains(p.SchoolClassId)).ToList();
        }

        // The races a voter votes in, with their approved candidates, in ballot order.
        private async Task<List<ElectionContest>> RacesForAsync(int electionId, StudentPlace voter)
        {
            var contests = await _db.ElectionContests.AsNoTracking()
                .Include(c => c.ElectionPosition).ThenInclude(p => p!.Levels)
                .Include(c => c.Candidates.Where(x => x.Status == CandidateStatus.Approved)).ThenInclude(x => x.Student)
                .Include(c => c.Candidates.Where(x => x.Status == CandidateStatus.Approved)).ThenInclude(x => x.RunningMateStudent)
                .Where(c => c.ElectionPosition!.ElectionId == electionId)
                .AsSplitQuery()
                .OrderBy(c => c.ElectionPosition!.Rank).ThenBy(c => c.ElectionPosition!.Name).ThenBy(c => c.Rank)
                .ToListAsync();
            return contests.Where(c => ElectionService.InContest(c, c.ElectionPosition!, voter, ContestRole.Voter)).ToList();
        }

        private static string? OfficerName(ElectionStation s) =>
            s.OfficerUser == null ? null : $"{s.OfficerUser.FirstName} {s.OfficerUser.LastName}".Trim();

        private async Task<string?> ValidateStationAsync(Election e, SaveStationDto dto, int? stationId)
        {
            if (string.IsNullOrWhiteSpace(dto.Name)) return "Enter the station name.";
            if (dto.SchoolClassIds.Count == 0) return "Choose the classes that vote at this station.";
            var ids = dto.SchoolClassIds.Distinct().ToList();
            if (await _db.SchoolClasses.CountAsync(c => ids.Contains(c.Id) && c.AcademicYearId == e.AcademicYearId) != ids.Count)
                return "The classes must be classes of the election's year.";
            var taken = await _db.ElectionStationClasses
                .Where(c => c.ElectionStation!.ElectionId == e.Id && c.ElectionStationId != stationId && ids.Contains(c.SchoolClassId))
                .Select(c => c.SchoolClass!.Name + " (" + c.ElectionStation!.Name + ")").ToListAsync();
            if (taken.Count > 0) return $"Already at another station: {string.Join(", ", taken)}.";
            if (dto.OfficerUserId != null && !await _db.Users.AnyAsync(u => u.Id == dto.OfficerUserId))
                return "The returning officer no longer exists.";
            return null;
        }

        private async Task<StationsDto> StationsAsync(Election e, bool canManage)
        {
            var stations = await _db.ElectionStations.AsNoTracking()
                .Include(s => s.OfficerUser).Include(s => s.Classes).ThenInclude(c => c.SchoolClass)
                .Where(s => s.ElectionId == e.Id).OrderBy(s => s.Name).ToListAsync();
            var places = await _elections.VoterPlacesAsync(e);
            var casts = await _db.ElectionVoteCasts.Where(v => v.ElectionId == e.Id)
                .GroupBy(v => v.ElectionStationId).Select(g => new { g.Key, Count = g.Count() }).ToListAsync();
            var classes = places.GroupBy(p => p.SchoolClassId)
                .Select(g => new IdNameDto { Id = g.Key, Name = g.First().ClassName }).OrderBy(c => c.Name).ToList();
            var covered = stations.SelectMany(s => s.Classes).Select(c => c.SchoolClassId).ToHashSet();

            // Returning officers: users who are staff.
            var officers = await _db.Users.AsNoTracking()
                .Where(u => u.PersonId != null && _db.StaffDetails.Any(st => st.Id == u.PersonId))
                .OrderBy(u => u.FirstName).ThenBy(u => u.LastName)
                .Select(u => new IdNameDto { Id = u.Id, Name = (u.FirstName + " " + u.LastName).Trim() }).ToListAsync();

            return new StationsDto
            {
                ElectionId = e.Id,
                ElectionName = e.Name,
                Stage = e.Stage,
                CanManage = canManage && StationsEditable(e.Stage),
                Classes = classes,
                UncoveredClasses = classes.Where(c => !covered.Contains(c.Id)).ToList(),
                Officers = officers,
                Stations = stations.Select(s =>
                {
                    var ids = s.Classes.Select(c => c.SchoolClassId).ToHashSet();
                    return new StationDto
                    {
                        Id = s.Id,
                        ElectionId = e.Id,
                        Name = s.Name,
                        OfficerUserId = s.OfficerUserId,
                        OfficerName = OfficerName(s),
                        Classes = s.Classes.OrderBy(c => c.SchoolClass!.Name)
                            .Select(c => new IdNameDto { Id = c.SchoolClassId, Name = c.SchoolClass!.Name }).ToList(),
                        Voters = places.Count(p => ids.Contains(p.SchoolClassId)),
                        Voted = casts.FirstOrDefault(c => c.Key == s.Id)?.Count ?? 0
                    };
                }).ToList()
            };
        }
    }
}
