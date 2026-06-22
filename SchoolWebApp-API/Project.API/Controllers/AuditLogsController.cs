using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Project.Infrastructure.Data;
using SchoolWebApp.Core.Entities.Security;

namespace SchoolWebApp.API.Controllers
{
    /// <summary>
    /// Read-only access to the audit trail. Administrator-gated -
    /// audit logs reveal student-touching activity and must not be
    /// browsable by every operator. Paged + filterable so a school
    /// with millions of rows can still ask focused questions.
    /// </summary>
    [ApiController]
    [Authorize(Roles = "Administrator,SuperAdministrator")]
    [Route("api/auditlogs")]
    public class AuditLogsController : ControllerBase
    {
        private readonly ApplicationDbContext _db;

        public AuditLogsController(ApplicationDbContext db)
        {
            _db = db;
        }

        public class AuditLogPage
        {
            public int Total { get; set; }
            public AuditLog[] Items { get; set; }
        }

        public class ActiveUser
        {
            public string UserId { get; set; }
            public string UserName { get; set; }
            public DateTime LastLogin { get; set; }
            public string IpAddress { get; set; }
        }

        // GET: api/auditlogs
        //   userName    : exact match on the actor (e.g. "emily").
        //   action      : exact match on the verb (Create/Update/Delete/
        //                 Login/LoginFailed/Logout/View/Print).
        //   entityType  : exact match on the domain entity name
        //                 ("Student", "Payment", "Invoice", ...).
        //   search      : substring match on UserName, EntityType, Notes
        //                 or RequestPath.
        //   startDate   : inclusive lower bound on Timestamp (yyyy-MM-dd).
        //   endDate     : inclusive upper bound on Timestamp (yyyy-MM-dd).
        //   page        : 1-based page index (default 1).
        //   pageSize    : default 50, capped at 200.
        [HttpGet]
        public async Task<ActionResult<AuditLogPage>> GetAuditLogs(
            [FromQuery] string userName = null,
            [FromQuery] string action = null,
            [FromQuery] string entityType = null,
            [FromQuery] string search = null,
            [FromQuery] DateTime? startDate = null,
            [FromQuery] DateTime? endDate = null,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 50)
        {
            if (page < 1) page = 1;
            if (pageSize < 1) pageSize = 50;
            if (pageSize > 200) pageSize = 200;

            var endExclusiveUpper = endDate?.Date.AddDays(1);
            var search_ = string.IsNullOrWhiteSpace(search) ? null : search.Trim().ToLower();
            var userName_ = string.IsNullOrWhiteSpace(userName) ? null : userName.Trim();
            var action_ = string.IsNullOrWhiteSpace(action) ? null : action.Trim();
            var entityType_ = string.IsNullOrWhiteSpace(entityType) ? null : entityType.Trim();

            var query = _db.AuditLogs.AsNoTracking().AsQueryable();

            if (userName_ != null)
                query = query.Where(a => a.UserName == userName_);

            if (action_ != null)
                query = query.Where(a => a.Action == action_);

            if (entityType_ != null)
                query = query.Where(a => a.EntityType == entityType_);

            if (startDate.HasValue)
                query = query.Where(a => a.Timestamp >= startDate.Value.Date);

            if (endExclusiveUpper.HasValue)
                query = query.Where(a => a.Timestamp < endExclusiveUpper.Value);

            if (search_ != null)
            {
                query = query.Where(a =>
                    (a.UserName != null && a.UserName.ToLower().Contains(search_)) ||
                    (a.EntityType != null && a.EntityType.ToLower().Contains(search_)) ||
                    (a.Notes != null && a.Notes.ToLower().Contains(search_)) ||
                    (a.RequestPath != null && a.RequestPath.ToLower().Contains(search_)));
            }

            var total = await query.CountAsync();
            var items = await query
                .OrderByDescending(a => a.Timestamp)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToArrayAsync();

            // Timestamps are stored in UTC (DateTime.UtcNow) but MySQL hands
            // them back with Kind=Unspecified, so System.Text.Json serializes
            // them without the trailing 'Z'. Mark them UTC so the client
            // receives an unambiguous ISO-8601 instant and renders it as UTC.
            foreach (var item in items)
                item.Timestamp = DateTime.SpecifyKind(item.Timestamp, DateTimeKind.Utc);

            return Ok(new AuditLogPage { Total = total, Items = items });
        }

        // GET: api/auditlogs/actions
        // Lightweight helper for the frontend's "Action" dropdown -
        // returns the distinct verbs present in the log so admins can
        // filter without typo-prone free-text entry.
        [HttpGet("actions")]
        public async Task<ActionResult<string[]>> GetActions()
        {
            var actions = await _db.AuditLogs
                .AsNoTracking()
                .Select(a => a.Action)
                .Where(a => a != null)
                .Distinct()
                .OrderBy(a => a)
                .ToArrayAsync();
            return Ok(actions);
        }

        // GET: api/auditlogs/entitytypes
        // Distinct entity types currently represented in the log.
        [HttpGet("entitytypes")]
        public async Task<ActionResult<string[]>> GetEntityTypes()
        {
            var entityTypes = await _db.AuditLogs
                .AsNoTracking()
                .Select(a => a.EntityType)
                .Where(a => a != null)
                .Distinct()
                .OrderBy(a => a)
                .ToArrayAsync();
            return Ok(entityTypes);
        }

        // GET: api/auditlogs/active-users
        // "Currently logged in" is inferred from the audit trail because JWT
        // auth is stateless. Within the token lifetime (600 min), a user whose
        // most recent Login/Logout event is a Login is treated as still signed
        // in. Closing the browser without an explicit logout means a user
        // lingers here until their token would have expired.
        [HttpGet("active-users")]
        public async Task<ActionResult<ActiveUser[]>> GetActiveUsers()
        {
            const int tokenLifetimeMinutes = 600;
            var windowStart = DateTime.UtcNow.AddMinutes(-tokenLifetimeMinutes);

            var events = await _db.AuditLogs
                .AsNoTracking()
                .Where(a => (a.Action == "Login" || a.Action == "Logout")
                            && a.UserId != null
                            && a.Timestamp >= windowStart)
                .OrderByDescending(a => a.Timestamp)
                .Select(a => new {a.UserId, a.UserName, a.Action, a.Timestamp, a.IpAddress})
                .ToListAsync();

            var active = events
                .GroupBy(a => a.UserId)
                .Select(g => g.First())          // latest event per user (already ordered desc)
                .Where(a => a.Action == "Login")  // still signed in (no later logout)
                .Select(a => new ActiveUser
                {
                    UserId = a.UserId,
                    UserName = a.UserName,
                    LastLogin = DateTime.SpecifyKind(a.Timestamp, DateTimeKind.Utc),
                    IpAddress = a.IpAddress
                })
                .OrderByDescending(a => a.LastLogin)
                .ToArray();

            return Ok(active);
        }
    }
}
