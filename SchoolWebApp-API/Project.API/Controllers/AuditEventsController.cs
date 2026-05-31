using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolWebApp.Core.Services;

namespace SchoolWebApp.API.Controllers
{
    /// <summary>
    /// Endpoint for the frontend to record user-side events that the
    /// server otherwise can't see - chiefly "I printed a PDF" and
    /// "I clicked sign out". Any authenticated user can post (unlike
    /// AuditLogsController, which is admin-gated for *reading*) -
    /// every operator can record their own events.
    /// </summary>
    [ApiController]
    [Authorize]
    [Route("api/auditevents")]
    public class AuditEventsController : ControllerBase
    {
        private readonly IAuditService _audit;

        public AuditEventsController(IAuditService audit)
        {
            _audit = audit;
        }

        public class PrintEventRequest
        {
            // What was printed (e.g. "Invoice", "Report", "Payslip").
            // Free-text so the frontend can introduce new print types
            // without a server change.
            public string EntityType { get; set; }

            // Primary key being printed (where applicable). For bulk
            // prints across a date range this may be null; pass
            // context in Notes instead.
            public string EntityId { get; set; }

            // Optional free-text context. For bulk prints, include the
            // date range, count, or whatever distinguishes this print
            // job from the next.
            public string Notes { get; set; }
        }

        // POST: api/auditevents/print
        // Fire-and-forget from the frontend after a PDF render. We
        // accept-and-record; the response is 204 so the UI doesn't
        // have to parse anything.
        [HttpPost("print")]
        public async Task<IActionResult> Print([FromBody] PrintEventRequest body)
        {
            if (body == null) return BadRequest();
            await _audit.LogPrintAsync(body.EntityType, body.EntityId, body.Notes);
            return NoContent();
        }

        // POST: api/auditevents/logout
        // Sign-out is a client-side action (the JWT just stops being
        // sent) - the server has no other way to know it happened.
        // Frontend should call this BEFORE clearing the token so the
        // request still carries it.
        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            var name = User?.FindFirstValue("username") ?? User?.Identity?.Name;
            var id = User?.FindFirstValue(ClaimTypes.NameIdentifier)
                ?? User?.FindFirstValue("userid");
            await _audit.LogLogoutAsync(name, id);
            return NoContent();
        }
    }
}
