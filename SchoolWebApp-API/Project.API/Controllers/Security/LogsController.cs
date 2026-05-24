using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Project.Infrastructure.Data;
using SchoolWebApp.Core.Entities.Identity;

namespace SchoolWebApp.API.Controllers.Security
{
    /// <summary>
    /// Read access to the application Logs table that NLog writes Error-level
    /// entries into (see nlog.config). Gated behind the Administrator /
    /// SuperAdministrator roles so the support / admin user can monitor issues
    /// without needing direct database access.
    /// </summary>
    [Authorize(Policy = "AdminRole")]
    [Route("api/[controller]")]
    [ApiController]
    public class LogsController : ControllerBase
    {
        private readonly ApplicationDbContext _db;

        public LogsController(ApplicationDbContext db)
        {
            _db = db;
        }

        public class LogsPage
        {
            public int Total { get; set; }
            public Log[] Items { get; set; } = Array.Empty<Log>();
        }

        // GET: api/Logs
        //   level      : filter by NLog level (Error / Warn / Info etc).
        //   search     : substring match on Message / Logger / Url.
        //   startDate  : inclusive lower bound on Logged (yyyy-MM-dd).
        //   endDate    : inclusive upper bound on Logged (yyyy-MM-dd).
        //   resolved   : null/missing = show only OPEN rows; true = show only
        //                resolved; false = both. Default surfaces the ones that
        //                still need triage.
        //   page       : 1-based page index (default 1).
        //   pageSize   : default 50, capped at 200 to keep payloads sane.
        [HttpGet]
        public async Task<ActionResult<LogsPage>> GetLogs(
            [FromQuery] string? level = null,
            [FromQuery] string? search = null,
            [FromQuery] DateTime? startDate = null,
            [FromQuery] DateTime? endDate = null,
            [FromQuery] bool? resolved = null,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 50)
        {
            if (page < 1) page = 1;
            if (pageSize < 1) pageSize = 50;
            if (pageSize > 200) pageSize = 200;

            // Treat endDate as an inclusive day boundary by bumping to the
            // exclusive next-midnight before comparing.
            var endExclusiveUpper = endDate?.Date.AddDays(1);
            var search_ = string.IsNullOrWhiteSpace(search) ? null : search.Trim().ToLower();
            var level_ = string.IsNullOrWhiteSpace(level) ? null : level.Trim();

            var query = _db.Logs.AsNoTracking().AsQueryable();

            if (level_ != null)
                query = query.Where(l => l.Level == level_);

            if (startDate.HasValue)
                query = query.Where(l => l.Logged >= startDate.Value.Date);

            if (endExclusiveUpper.HasValue)
                query = query.Where(l => l.Logged < endExclusiveUpper.Value);

            if (resolved.HasValue)
                query = query.Where(l => l.Resolved == resolved.Value);

            if (search_ != null)
                query = query.Where(l =>
                    (l.Message != null && l.Message.ToLower().Contains(search_)) ||
                    (l.Logger != null && l.Logger.ToLower().Contains(search_)) ||
                    (l.Url != null && l.Url.ToLower().Contains(search_)));

            var total = await query.CountAsync();
            var items = await query
                .OrderByDescending(l => l.Id)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToArrayAsync();

            return Ok(new LogsPage { Total = total, Items = items });
        }

        // GET: api/Logs/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Log>> GetLog(int id)
        {
            var log = await _db.Logs.AsNoTracking().FirstOrDefaultAsync(l => l.Id == id);
            if (log == null) return NotFound();
            return Ok(log);
        }

        // GET: api/Logs/Levels
        // Surface the distinct levels currently in the table so the frontend
        // can populate its Level dropdown without hardcoding.
        [HttpGet("Levels")]
        public async Task<ActionResult<string[]>> GetLevels()
        {
            var levels = await _db.Logs
                .AsNoTracking()
                .Where(l => l.Level != null)
                .Select(l => l.Level!)
                .Distinct()
                .OrderBy(l => l)
                .ToArrayAsync();
            return Ok(levels);
        }

        public class ResolveRequest
        {
            public bool Resolved { get; set; }
            public string? Note { get; set; }
        }

        // POST: api/Logs/5/resolve
        // Marks a log entry as resolved (or reopens it). The "who" comes from
        // the JWT — no separate audit table needed because the row is its own
        // audit trail.
        [HttpPost("{id}/resolve")]
        public async Task<IActionResult> SetResolution(int id, [FromBody] ResolveRequest body)
        {
            if (body == null) return BadRequest("Resolution payload missing.");

            var log = await _db.Logs.FirstOrDefaultAsync(l => l.Id == id);
            if (log == null) return NotFound();

            log.Resolved = body.Resolved;
            if (body.Resolved)
            {
                log.ResolvedBy = User?.FindFirst("username")?.Value ?? User?.Identity?.Name;
                log.ResolvedAt = DateTime.Now;
                log.ResolutionNote = string.IsNullOrWhiteSpace(body.Note)
                    ? null
                    : body.Note.Trim();
            }
            else
            {
                log.ResolvedBy = null;
                log.ResolvedAt = null;
                log.ResolutionNote = null;
            }

            await _db.SaveChangesAsync();
            return Ok(log);
        }
    }
}
