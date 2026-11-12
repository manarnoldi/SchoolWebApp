using SchoolWebApp.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Project.Infrastructure.Data;
using SchoolWebApp.API.Licensing;
using SchoolWebApp.API.Security;
using SchoolWebApp.API.Services.TeachingDocuments;
using SchoolWebApp.API.Services.Timetable;
using SchoolWebApp.Core.DTOs.Timetable;
using SchoolWebApp.Core.Entities.Timetable;

namespace SchoolWebApp.API.Controllers.Timetable
{
    /// <summary>
    /// Building timetables: create a draft for a term, generate it, publish it.
    /// Needs the Timetable menus. Published timetables are read by everyone
    /// through <see cref="PublishedTimetablesController"/>.
    /// </summary>
    [Route("api/timetables")]
    [ApiController]
    [Authorize]
    [RequiresModule(AppModules.Academics)]
    [RequiresMenu(MenuGroup.Timetable)]
    public class TimetablesController : ControllerBase
    {
        private readonly ApplicationDbContext _db;
        private readonly TimetableService _svc;

        public TimetablesController(ApplicationDbContext db, TimetableService svc)
        {
            _db = db;
            _svc = svc;
        }

        // GET api/timetables?sessionId=6
        [HttpGet]
        public async Task<IActionResult> List(int? sessionId) => Ok(await _svc.SummariesAsync(sessionId));

        // GET api/timetables/5
        [HttpGet("{id:int}")]
        public async Task<IActionResult> Get(int id)
        {
            var dto = await _svc.DetailAsync(id);
            return dto == null ? NotFound(new { message = "Timetable not found." }) : Ok(dto);
        }

        // POST api/timetables - an empty draft for a term
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateTimetableDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Name)) return BadRequest(new { message = "Give the timetable a name." });
            if (!await _db.Sessions.AnyAsync(s => s.Id == dto.SessionId)) return BadRequest(new { message = "Pick a term." });
            var t = new SchoolTimetable { Name = dto.Name.Trim(), SessionId = dto.SessionId, Notes = dto.Notes?.Trim() };
            _db.SchoolTimetables.Add(t);
            await _db.SaveChangesAsync();
            return Ok(new { id = t.Id });
        }

        // PUT api/timetables/5 - name and notes
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateTimetableDto dto)
        {
            var t = await _db.SchoolTimetables.FirstOrDefaultAsync(x => x.Id == id);
            if (t == null) return NotFound(new { message = "Timetable not found." });
            if (string.IsNullOrWhiteSpace(dto.Name)) return BadRequest(new { message = "Give the timetable a name." });
            t.Name = dto.Name.Trim();
            t.Notes = dto.Notes?.Trim();
            await _db.SaveChangesAsync();
            return Ok();
        }

        // DELETE api/timetables/5 - drafts and archived ones; a published timetable is replaced, not deleted
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var t = await _db.SchoolTimetables.FirstOrDefaultAsync(x => x.Id == id);
            if (t == null) return NotFound(new { message = "Timetable not found." });
            if (t.Status == TimetableStatus.Published)
                return BadRequest(new { message = "This timetable is in use. Publish another one for the term first, then delete this one." });
            _db.SchoolTimetables.Remove(t);
            await _db.SaveChangesAsync();
            return Ok();
        }

        // POST api/timetables/5/generate - lay the week out afresh; locked cards stay
        [HttpPost("{id:int}/generate")]
        public async Task<IActionResult> Generate(int id, [FromBody] GenerateTimetableDto? dto)
        {
            var t = await _db.SchoolTimetables.FirstOrDefaultAsync(x => x.Id == id);
            if (t == null) return NotFound(new { message = "Timetable not found." });
            if (t.Status != TimetableStatus.Draft)
                return BadRequest(new { message = "Only a draft can be generated. Make a copy of this timetable to change it." });
            var error = await _svc.GenerateAsync(t, dto?.Seconds ?? 20);
            if (error != null) return BadRequest(new { message = error });
            return Ok(await _svc.DetailAsync(id));
        }

        // POST api/timetables/5/publish - in use from a date; the term's previous one is archived
        [HttpPost("{id:int}/publish")]
        public async Task<IActionResult> Publish(int id, [FromBody] PublishTimetableDto dto)
        {
            var t = await _db.SchoolTimetables.Include(x => x.Lessons).FirstOrDefaultAsync(x => x.Id == id);
            if (t == null) return NotFound(new { message = "Timetable not found." });
            if (t.Status != TimetableStatus.Draft) return BadRequest(new { message = "Only a draft can be published." });
            if (!t.Lessons.Any(l => l.Day != null)) return BadRequest(new { message = "Generate the timetable before publishing it." });

            var current = await _db.SchoolTimetables
                .Where(x => x.SessionId == t.SessionId && x.Status == TimetableStatus.Published && x.Id != id).ToListAsync();
            foreach (var c in current) c.Status = TimetableStatus.Archived;
            t.Status = TimetableStatus.Published;
            t.EffectiveFrom = dto.EffectiveFrom.Date;
            t.PublishedAt = DateTime.Now;
            t.PublishedBy = User.Identity?.Name;
            await _db.SaveChangesAsync();
            return Ok();
        }

        // POST api/timetables/5/copy - a draft copy of any timetable, cards and all
        [HttpPost("{id:int}/copy")]
        public async Task<IActionResult> Copy(int id)
        {
            var t = await _db.SchoolTimetables.AsNoTracking().Include(x => x.Lessons).FirstOrDefaultAsync(x => x.Id == id);
            if (t == null) return NotFound(new { message = "Timetable not found." });
            var copy = new SchoolTimetable
            {
                Name = $"{t.Name} (copy)",
                SessionId = t.SessionId,
                Notes = t.Notes,
                GeneratedAt = t.GeneratedAt,
                GenerationReport = t.GenerationReport,
                Lessons = t.Lessons.Select(l => new TimetableLesson
                {
                    SchoolClassId = l.SchoolClassId, SubjectId = l.SubjectId, StaffDetailsId = l.StaffDetailsId, Day = l.Day,
                    BellSlotId = l.BellSlotId, Length = l.Length, LinkNo = l.LinkNo, IsLocked = l.IsLocked, IsManual = l.IsManual
                }).ToList()
            };
            _db.SchoolTimetables.Add(copy);
            await _db.SaveChangesAsync();
            return Ok(new { id = copy.Id });
        }
    }

    /// <summary>The published timetable, read-only, for every signed-in member of staff.</summary>
    [Route("api/timetables")]
    [ApiController]
    [Authorize]
    [RequiresModule(AppModules.Academics)]
    public class PublishedTimetablesController : ControllerBase
    {
        private readonly ApplicationDbContext _db;
        private readonly TimetableService _svc;
        private readonly TeachingDocumentService _teaching;

        public PublishedTimetablesController(ApplicationDbContext db, TimetableService svc, TeachingDocumentService teaching)
        {
            _db = db;
            _svc = svc;
            _teaching = teaching;
        }

        // GET api/timetables/published?sessionId=6 - the term's timetable in use (the current term if none given)
        [HttpGet("published")]
        public async Task<IActionResult> Published(int? sessionId)
        {
            var id = await PublishedIdAsync(sessionId);
            if (id == null) return Ok(new { timetable = (TimetableDetailDto?)null, sessions = await PublishedSessionsAsync() });
            return Ok(new { timetable = await _svc.DetailAsync(id.Value), sessions = await PublishedSessionsAsync() });
        }

        // GET api/timetables/mine - the caller's own lessons in the current timetable
        [HttpGet("mine")]
        public async Task<IActionResult> Mine(int? sessionId)
        {
            var me = await _teaching.CurrentAsync();
            if (me == null) return Unauthorized();
            var id = await PublishedIdAsync(sessionId);
            return Ok(new
            {
                staffDetailsId = me.StaffDetailsId,
                timetable = id == null ? null : await _svc.DetailAsync(id.Value),
                sessions = await PublishedSessionsAsync()
            });
        }

        private async Task<int?> PublishedIdAsync(int? sessionId)
        {
            var published = _db.SchoolTimetables.AsNoTracking().Where(t => t.Status == TimetableStatus.Published);
            if (sessionId != null)
                return await published.Where(t => t.SessionId == sessionId).Select(t => (int?)t.Id).FirstOrDefaultAsync();
            // The current term's (see CurrentTerm), else the latest one that has started, else the first.
            var today = DateTime.Today;
            var currentId = (await CurrentTerm.GetAsync(_db))?.Id ?? -1;
            return await published.Where(t => t.SessionId == currentId).Select(t => (int?)t.Id).FirstOrDefaultAsync()
                ?? await published.Where(t => t.Session!.StartDate <= today).OrderByDescending(t => t.Session!.StartDate).Select(t => (int?)t.Id).FirstOrDefaultAsync()
                ?? await published.OrderBy(t => t.Session!.StartDate).Select(t => (int?)t.Id).FirstOrDefaultAsync();
        }

        private async Task<object> PublishedSessionsAsync() =>
            await _db.SchoolTimetables.AsNoTracking().Where(t => t.Status == TimetableStatus.Published)
                .OrderByDescending(t => t.Session!.StartDate)
                .Select(t => new { id = t.SessionId, name = t.Session!.AcademicYear!.Name + " " + t.Session.SessionName })
                .ToListAsync();
    }
}
