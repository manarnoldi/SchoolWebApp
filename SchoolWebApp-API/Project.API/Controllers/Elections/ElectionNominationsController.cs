using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Project.Infrastructure.Data;
using SchoolWebApp.API.Services.Elections;
using SchoolWebApp.Core.DTOs.Elections;
using SchoolWebApp.Core.Entities.Elections;
using SchoolWebApp.API.Licensing;

namespace SchoolWebApp.API.Controllers.Elections
{
    /// <summary>
    /// Nominations (who stands in each race, with photos) and the voter register.
    /// Nominations are taken while the election is in Nominations; photos can still
    /// be changed until polling opens.
    /// </summary>
    [Authorize]
    [Route("api/elections")]
    [ApiController]
    [RequiresModule(AppModules.Elections)]
    public class ElectionNominationsController : ControllerBase
    {
        private readonly ApplicationDbContext _db;
        private readonly ElectionService _elections;
        private readonly ElectionPhotoStore _photos;

        public ElectionNominationsController(ApplicationDbContext db, ElectionService elections, ElectionPhotoStore photos)
        {
            _db = db;
            _elections = elections;
            _photos = photos;
        }

        private static bool PhotosChangeable(ElectionStage s) =>
            s == ElectionStage.Nominations || s == ElectionStage.CandidatesFinal;

        // GET api/elections/5/nominations
        [HttpGet("{id:int}/nominations")]
        public async Task<IActionResult> Get(int id)
        {
            var me = await _elections.CurrentAsync();
            if (me == null) return Unauthorized();
            var e = await _db.Elections.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
            if (e == null) return NotFound();
            return Ok(await NominationsAsync(e, me.CanManage));
        }

        // GET api/elections/contests/9/eligible?role=candidate|mate
        [HttpGet("contests/{contestId:int}/eligible")]
        public async Task<IActionResult> Eligible(int contestId, [FromQuery] string role = "candidate")
        {
            var me = await _elections.CurrentAsync();
            if (me == null) return Unauthorized();
            if (!me.CanManage) return Forbid();
            var contest = await LoadContestAsync(contestId);
            if (contest == null) return NotFound();
            var p = contest.ElectionPosition!;
            var asRole = role == "mate" ? ContestRole.RunningMate : ContestRole.Candidate;

            var places = await _elections.LivePlacesAsync(p.Election!.AcademicYearId,
                p.Levels.Where(l => l.Role == PositionLevelRole.Voter).Select(l => l.LearningLevelId).ToList());
            var standing = await StandingAsync(p.ElectionId);
            var genders = await GenderNamesAsync();
            return Ok(places.Where(s => ElectionService.InContest(contest, p, s, asRole)).Select(s => new EligibleStudentDto
            {
                StudentId = s.StudentId,
                Name = s.FullName,
                Upi = s.Upi,
                ClassName = s.ClassName,
                GenderName = genders.GetValueOrDefault(s.GenderId, ""),
                Taken = standing.GetValueOrDefault(s.StudentId)
            }).ToList());
        }

        // POST api/elections/contests/9/candidates
        [HttpPost("contests/{contestId:int}/candidates")]
        public async Task<IActionResult> Nominate(int contestId, [FromBody] SaveCandidateDto dto)
        {
            var me = await _elections.CurrentAsync();
            if (me == null) return Unauthorized();
            if (!me.CanManage) return Forbid();
            var contest = await LoadContestAsync(contestId);
            if (contest == null) return NotFound();
            var p = contest.ElectionPosition!;
            if (p.Election!.Stage != ElectionStage.Nominations) return BadRequest("Nominations are not open.");

            var error = await CheckPeopleAsync(contest, dto.StudentId, dto.RunningMateStudentId, null);
            if (error != null) return BadRequest(error);

            // A rejected or withdrawn nomination in the same race is taken up again.
            var c = await _db.ElectionCandidates.FirstOrDefaultAsync(x => x.ElectionContestId == contestId && x.StudentId == dto.StudentId);
            if (c == null)
            {
                c = new ElectionCandidate { ElectionContestId = contestId, StudentId = dto.StudentId };
                _db.ElectionCandidates.Add(c);
            }
            c.Status = CandidateStatus.Nominated;
            c.StatusNote = null;
            c.Slogan = Clean(dto.Slogan);
            await SetRunningMateAsync(c, dto.RunningMateStudentId);
            // Start from the photo on the student's record, when there is one.
            if (c.PhotoFile == null) c.PhotoFile = await _photos.SaveDataUrlAsync(await RecordPhotoAsync(c.StudentId));
            await _db.SaveChangesAsync();
            return Ok(await NominationsAsync(p.Election, true));
        }

        // PUT api/elections/candidates/12 - slogan and running mate
        [HttpPut("candidates/{candidateId:int}")]
        public async Task<IActionResult> Update(int candidateId, [FromBody] SaveCandidateDto dto)
        {
            var me = await _elections.CurrentAsync();
            if (me == null) return Unauthorized();
            if (!me.CanManage) return Forbid();
            var c = await _db.ElectionCandidates.FirstOrDefaultAsync(x => x.Id == candidateId);
            if (c == null) return NotFound();
            var contest = (await LoadContestAsync(c.ElectionContestId))!;
            if (contest.ElectionPosition!.Election!.Stage != ElectionStage.Nominations) return BadRequest("Nominations are not open.");

            var error = await CheckPeopleAsync(contest, c.StudentId, dto.RunningMateStudentId, c.Id);
            if (error != null) return BadRequest(error);
            c.Slogan = Clean(dto.Slogan);
            await SetRunningMateAsync(c, dto.RunningMateStudentId);
            await _db.SaveChangesAsync();
            return Ok(await NominationsAsync(contest.ElectionPosition.Election, true));
        }

        // POST api/elections/candidates/12/status
        [HttpPost("candidates/{candidateId:int}/status")]
        public async Task<IActionResult> SetStatus(int candidateId, [FromBody] CandidateStatusDto dto)
        {
            var me = await _elections.CurrentAsync();
            if (me == null) return Unauthorized();
            if (!me.CanManage) return Forbid();
            var c = await _db.ElectionCandidates.Include(x => x.ElectionContest).ThenInclude(x => x!.ElectionPosition)
                .ThenInclude(x => x!.Election).FirstOrDefaultAsync(x => x.Id == candidateId);
            if (c == null) return NotFound();
            var e = c.ElectionContest!.ElectionPosition!.Election!;
            if (e.Stage != ElectionStage.Nominations) return BadRequest("Nominations are not open.");
            if (!Enum.IsDefined(dto.Status)) return BadRequest("Choose the status.");
            if (dto.Status == CandidateStatus.Rejected && string.IsNullOrWhiteSpace(dto.Note))
                return BadRequest("Give the reason for rejecting the nomination.");
            // Taking a rejected or withdrawn nomination back up: the student must not be standing elsewhere since.
            if (dto.Status is CandidateStatus.Nominated or CandidateStatus.Approved
                && c.Status is CandidateStatus.Rejected or CandidateStatus.Withdrawn)
            {
                var standing = await StandingAsync(e.Id, c.Id);
                var clash = standing.GetValueOrDefault(c.StudentId)
                    ?? (c.RunningMateStudentId != null ? standing.GetValueOrDefault(c.RunningMateStudentId.Value) : null);
                if (clash != null) return BadRequest($"Already {clash}.");
            }
            c.Status = dto.Status;
            c.StatusNote = Clean(dto.Note);
            await _db.SaveChangesAsync();
            return Ok(await NominationsAsync(e, true));
        }

        // DELETE api/elections/candidates/12 - a mistaken entry; withdraw or reject to keep the record
        [HttpDelete("candidates/{candidateId:int}")]
        public async Task<IActionResult> Delete(int candidateId)
        {
            var me = await _elections.CurrentAsync();
            if (me == null) return Unauthorized();
            if (!me.CanManage) return Forbid();
            var c = await _db.ElectionCandidates.Include(x => x.ElectionContest).ThenInclude(x => x!.ElectionPosition)
                .ThenInclude(x => x!.Election).FirstOrDefaultAsync(x => x.Id == candidateId);
            if (c == null) return NotFound();
            var e = c.ElectionContest!.ElectionPosition!.Election!;
            if (e.Stage != ElectionStage.Nominations) return BadRequest("Nominations are not open.");
            _db.ElectionCandidates.Remove(c);
            await _db.SaveChangesAsync();
            _photos.Delete(c.PhotoFile);
            _photos.Delete(c.RunningMatePhotoFile);
            return Ok(await NominationsAsync(e, true));
        }

        // POST api/elections/candidates/12/photo?mate=false  (multipart, field "file")
        [HttpPost("candidates/{candidateId:int}/photo")]
        [RequestSizeLimit(ElectionPhotoStore.MaxBytes + 64 * 1024)]
        public async Task<IActionResult> UploadPhoto(int candidateId, IFormFile file, [FromQuery] bool mate = false)
        {
            var me = await _elections.CurrentAsync();
            if (me == null) return Unauthorized();
            if (!me.CanManage) return Forbid();
            var c = await _db.ElectionCandidates.Include(x => x.ElectionContest).ThenInclude(x => x!.ElectionPosition)
                .ThenInclude(x => x!.Election).FirstOrDefaultAsync(x => x.Id == candidateId);
            if (c == null) return NotFound();
            var e = c.ElectionContest!.ElectionPosition!.Election!;
            if (!PhotosChangeable(e.Stage)) return BadRequest("Photos can only change until polling opens.");
            if (mate && c.RunningMateStudentId == null) return BadRequest("This candidate has no running mate.");

            string name;
            try { name = await _photos.SaveAsync(file); }
            catch (InvalidDataException ex) { return BadRequest(ex.Message); }
            var old = mate ? c.RunningMatePhotoFile : c.PhotoFile;
            if (mate) c.RunningMatePhotoFile = name; else c.PhotoFile = name;
            await _db.SaveChangesAsync();
            _photos.Delete(old);
            return Ok(await NominationsAsync(e, true));
        }

        // DELETE api/elections/candidates/12/photo?mate=false
        [HttpDelete("candidates/{candidateId:int}/photo")]
        public async Task<IActionResult> DeletePhoto(int candidateId, [FromQuery] bool mate = false)
        {
            var me = await _elections.CurrentAsync();
            if (me == null) return Unauthorized();
            if (!me.CanManage) return Forbid();
            var c = await _db.ElectionCandidates.Include(x => x.ElectionContest).ThenInclude(x => x!.ElectionPosition)
                .ThenInclude(x => x!.Election).FirstOrDefaultAsync(x => x.Id == candidateId);
            if (c == null) return NotFound();
            var e = c.ElectionContest!.ElectionPosition!.Election!;
            if (!PhotosChangeable(e.Stage)) return BadRequest("Photos can only change until polling opens.");
            var old = mate ? c.RunningMatePhotoFile : c.PhotoFile;
            if (mate) c.RunningMatePhotoFile = null; else c.PhotoFile = null;
            await _db.SaveChangesAsync();
            _photos.Delete(old);
            return Ok(await NominationsAsync(e, true));
        }

        // GET api/elections/photos/<file> - open, so <img> tags can load it; names are random.
        [AllowAnonymous]
        [HttpGet("photos/{name}")]
        public IActionResult Photo(string name)
        {
            var found = _photos.Find(name);
            if (found == null) return NotFound();
            Response.Headers.CacheControl = "public, max-age=31536000, immutable";
            return PhysicalFile(found.Value.Path, found.Value.ContentType);
        }

        // GET api/elections/5/register - voters per race: the frozen register, or students where they are now
        [HttpGet("{id:int}/register")]
        public async Task<IActionResult> Register(int id)
        {
            if (await _elections.CurrentAsync() == null) return Unauthorized();
            var e = await _db.Elections.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
            if (e == null) return NotFound();
            var places = await _elections.VoterPlacesAsync(e);
            var contests = await ContestsAsync(id);
            return Ok(new RegisterSummaryDto
            {
                Mode = e.RegisterMode,
                FrozenAt = e.RegisterFrozenAt,
                Students = places.Count,
                Contests = contests.Select(c => new RegisterContestDto
                {
                    ContestId = c.Id,
                    PositionName = c.ElectionPosition!.Name,
                    Name = c.Name,
                    Voters = places.Count(s => ElectionService.InContest(c, c.ElectionPosition, s, ContestRole.Voter))
                }).ToList()
            });
        }

        // GET api/elections/5/register/contests/9
        [HttpGet("{id:int}/register/contests/{contestId:int}")]
        public async Task<IActionResult> RegisterContest(int id, int contestId)
        {
            if (await _elections.CurrentAsync() == null) return Unauthorized();
            var e = await _db.Elections.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
            if (e == null) return NotFound();
            var c = (await ContestsAsync(id)).FirstOrDefault(x => x.Id == contestId);
            if (c == null) return NotFound();
            var genders = await GenderNamesAsync();
            var places = await _elections.VoterPlacesAsync(e);
            return Ok(places.Where(s => ElectionService.InContest(c, c.ElectionPosition!, s, ContestRole.Voter))
                .OrderBy(s => s.ClassName).ThenBy(s => s.FullName)
                .Select(s => new RegisterVoterDto
                {
                    StudentId = s.StudentId,
                    Name = s.FullName,
                    Upi = s.Upi,
                    ClassName = s.ClassName,
                    GenderName = genders.GetValueOrDefault(s.GenderId, "")
                }).ToList());
        }

        private async Task<List<ElectionContest>> ContestsAsync(int electionId) =>
            await _db.ElectionContests.AsNoTracking()
                .Include(c => c.ElectionPosition).ThenInclude(p => p!.Levels)
                .Where(c => c.ElectionPosition!.ElectionId == electionId)
                .OrderBy(c => c.ElectionPosition!.Rank).ThenBy(c => c.ElectionPosition!.Name).ThenBy(c => c.Rank)
                .ToListAsync();

        private async Task<ElectionContest?> LoadContestAsync(int contestId) =>
            await _db.ElectionContests.AsNoTracking()
                .Include(c => c.ElectionPosition).ThenInclude(p => p!.Levels)
                .Include(c => c.ElectionPosition).ThenInclude(p => p!.Election)
                .FirstOrDefaultAsync(c => c.Id == contestId);

        private async Task<Dictionary<int, string>> GenderNamesAsync() =>
            await _db.Genders.AsNoTracking().ToDictionaryAsync(g => g.Id, g => g.Name);

        private async Task<string?> RecordPhotoAsync(int studentId) =>
            await _db.Students.AsNoTracking().Where(s => s.Id == studentId).Select(s => s.StaffImageAsBase64).FirstOrDefaultAsync();

        private static string? Clean(string? s) => string.IsNullOrWhiteSpace(s) ? null : s.Trim();

        /// <summary>
        /// Students already standing in the election (as candidate or running mate,
        /// nominated or approved), with what they stand for - a student stands for one
        /// thing only.
        /// </summary>
        private async Task<Dictionary<int, string>> StandingAsync(int electionId, int? exceptCandidateId = null)
        {
            var rows = await _db.ElectionCandidates.AsNoTracking()
                .Where(c => c.ElectionContest!.ElectionPosition!.ElectionId == electionId
                    && (c.Status == CandidateStatus.Nominated || c.Status == CandidateStatus.Approved)
                    && c.Id != exceptCandidateId)
                .Select(c => new
                {
                    c.StudentId,
                    c.RunningMateStudentId,
                    Contest = c.ElectionContest!.Name,
                    Mate = c.ElectionContest.ElectionPosition!.RunningMateTitle
                }).ToListAsync();
            var map = new Dictionary<int, string>();
            foreach (var r in rows)
            {
                map.TryAdd(r.StudentId, $"standing for {r.Contest}");
                if (r.RunningMateStudentId != null) map.TryAdd(r.RunningMateStudentId.Value, $"running mate ({r.Mate}) in {r.Contest}");
            }
            return map;
        }

        // Why the candidate or running mate cannot stand, or null.
        private async Task<string?> CheckPeopleAsync(ElectionContest contest, int studentId, int? mateId, int? candidateId)
        {
            var p = contest.ElectionPosition!;
            var ticket = p.RunningMateTitle != null;
            if (ticket && mateId == null) return $"Choose the {p.RunningMateTitle}.";
            if (!ticket && mateId != null) return "This position has no running mate.";
            if (mateId == studentId) return "The running mate must be a different student.";

            var ids = new List<int> { studentId };
            if (mateId != null) ids.Add(mateId.Value);
            var places = (await _elections.LivePlacesAsync(p.Election!.AcademicYearId,
                    p.Levels.Where(l => l.Role == PositionLevelRole.Voter).Select(l => l.LearningLevelId).ToList()))
                .Where(s => ids.Contains(s.StudentId)).ToDictionary(s => s.StudentId);
            if (!places.TryGetValue(studentId, out var student) || !ElectionService.InContest(contest, p, student, ContestRole.Candidate))
                return "That student cannot stand in this race - check their class, grade and gender against the position.";
            if (mateId != null && (!places.TryGetValue(mateId.Value, out var mate) || !ElectionService.InContest(contest, p, mate, ContestRole.RunningMate)))
                return $"That student cannot be the {p.RunningMateTitle} in this race.";

            // A re-nomination in the same race takes up the old record, so it does not clash with itself.
            var self = candidateId ?? await _db.ElectionCandidates
                .Where(c => c.ElectionContestId == contest.Id && c.StudentId == studentId).Select(c => (int?)c.Id).FirstOrDefaultAsync();
            var standing = await StandingAsync(p.ElectionId, self);
            if (standing.TryGetValue(studentId, out var s1)) return $"{student.FullName} is already {s1}.";
            if (mateId != null && standing.TryGetValue(mateId.Value, out var s2)) return $"{places[mateId.Value].FullName} is already {s2}.";
            return null;
        }

        private async Task SetRunningMateAsync(ElectionCandidate c, int? mateId)
        {
            if (c.RunningMateStudentId == mateId) return;
            _photos.Delete(c.RunningMatePhotoFile);
            c.RunningMateStudentId = mateId;
            c.RunningMatePhotoFile = mateId == null ? null : await _photos.SaveDataUrlAsync(await RecordPhotoAsync(mateId.Value));
        }

        private async Task<NominationsDto> NominationsAsync(Election e, bool canManage)
        {
            var positions = await _db.ElectionPositions.AsNoTracking()
                .Include(p => p.Gender)
                .Include(p => p.Contests).ThenInclude(c => c.Candidates).ThenInclude(c => c.Student)
                .Include(p => p.Contests).ThenInclude(c => c.Candidates).ThenInclude(c => c.RunningMateStudent)
                .Where(p => p.ElectionId == e.Id)
                .AsSplitQuery()
                .OrderBy(p => p.Rank).ThenBy(p => p.Name)
                .ToListAsync();

            // Class and gender for everyone named, from where they are this year.
            var ids = positions.SelectMany(p => p.Contests).SelectMany(c => c.Candidates)
                .SelectMany(c => new[] { c.StudentId, c.RunningMateStudentId ?? 0 }).Where(x => x > 0).Distinct().ToList();
            var classes = await _db.StudentClasses.AsNoTracking()
                .Where(sc => ids.Contains(sc.StudentId) && sc.SchoolClass!.AcademicYearId == e.AcademicYearId)
                .Select(sc => new { sc.Id, sc.StudentId, sc.SchoolClass!.Name }).ToListAsync();
            var className = classes.GroupBy(x => x.StudentId).ToDictionary(g => g.Key, g => g.OrderByDescending(x => x.Id).First().Name);
            var genders = await GenderNamesAsync();
            static string? Url(string? file) => file == null ? null : $"/elections/photos/{file}";

            return new NominationsDto
            {
                ElectionId = e.Id,
                ElectionName = e.Name,
                Stage = e.Stage,
                CanManage = canManage,
                CanNominate = canManage && e.Stage == ElectionStage.Nominations,
                CanChangePhotos = canManage && PhotosChangeable(e.Stage),
                Positions = positions.Select(p => new NominationPositionDto
                {
                    Id = p.Id,
                    Name = p.Name,
                    RunningMateTitle = p.RunningMateTitle,
                    Seats = p.Seats,
                    GenderRule = p.GenderRule,
                    GenderName = p.Gender?.Name,
                    Contests = p.Contests.OrderBy(c => c.Rank).Select(c => new NominationContestDto
                    {
                        Id = c.Id,
                        Name = c.Name,
                        Candidates = c.Candidates
                            .OrderBy(x => x.Status == CandidateStatus.Rejected || x.Status == CandidateStatus.Withdrawn)
                            .ThenBy(x => x.BallotOrder == 0 ? int.MaxValue : x.BallotOrder).ThenBy(x => x.Student!.FullName)
                            .Select(x => new CandidateDto
                            {
                                Id = x.Id,
                                ContestId = c.Id,
                                StudentId = x.StudentId,
                                StudentName = x.Student!.FullName,
                                Upi = x.Student.UPI,
                                ClassName = className.GetValueOrDefault(x.StudentId, ""),
                                GenderName = genders.GetValueOrDefault(x.Student.GenderId, ""),
                                RunningMateStudentId = x.RunningMateStudentId,
                                RunningMateName = x.RunningMateStudent?.FullName,
                                RunningMateClassName = x.RunningMateStudentId == null ? null : className.GetValueOrDefault(x.RunningMateStudentId.Value, ""),
                                Slogan = x.Slogan,
                                PhotoUrl = Url(x.PhotoFile),
                                RunningMatePhotoUrl = Url(x.RunningMatePhotoFile),
                                Status = x.Status,
                                StatusNote = x.StatusNote,
                                BallotOrder = x.BallotOrder,
                                NominatedBy = x.CreatedBy,
                                NominatedAt = x.Created
                            }).ToList()
                    }).ToList()
                }).ToList()
            };
        }
    }
}
