using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Project.Infrastructure.Data;
using SchoolWebApp.Core.Entities.Security;
using SchoolWebApp.Core.Services;

namespace Project.Infrastructure.Services
{
    public class AuditService : IAuditService
    {
        private readonly ApplicationDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<AuditService> _logger;

        public AuditService(
            ApplicationDbContext context,
            IHttpContextAccessor httpContextAccessor,
            ILogger<AuditService> logger)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
        }

        public Task LogLoginAsync(string userName, string userId) =>
            Persist(BuildRow("Login", "User", userId, null, userName: userName, userId: userId));

        public Task LogLoginFailedAsync(string attemptedUserName, string reason) =>
            Persist(BuildRow(
                "LoginFailed",
                "User",
                null,
                reason,
                userName: attemptedUserName ?? "(unknown)",
                userId: null));

        public Task LogLogoutAsync(string userName, string userId) =>
            Persist(BuildRow("Logout", "User", userId, null, userName: userName, userId: userId));

        public Task LogViewAsync(string entityType, string entityId, string notes = null) =>
            Persist(BuildRow("View", entityType, entityId, notes));

        public Task LogPrintAsync(string entityType, string entityId, string notes = null) =>
            Persist(BuildRow("Print", entityType, entityId, notes));

        public Task LogAsync(string action, string entityType = null, string entityId = null, string notes = null) =>
            Persist(BuildRow(action, entityType, entityId, notes));

        private AuditLog BuildRow(
            string action,
            string entityType,
            string entityId,
            string notes,
            string userName = null,
            string userId = null)
        {
            var http = _httpContextAccessor.HttpContext;
            var principal = http?.User;

            // Username lives on the "username" claim per JwtService -
            // matches what the entity audit interceptor reads.
            var resolvedName = userName
                ?? principal?.FindFirstValue("username")
                ?? principal?.Identity?.Name;
            var resolvedId = userId
                ?? principal?.FindFirstValue(ClaimTypes.NameIdentifier)
                ?? principal?.FindFirstValue("userid");

            return new AuditLog
            {
                Timestamp = DateTime.UtcNow,
                Action = action,
                EntityType = entityType,
                EntityId = entityId,
                UserName = string.IsNullOrEmpty(resolvedName) ? "system" : resolvedName,
                UserId = resolvedId,
                IpAddress = http?.Connection?.RemoteIpAddress?.ToString(),
                UserAgent = http?.Request?.Headers["User-Agent"].ToString(),
                RequestPath = http?.Request?.Path.Value,
                Notes = notes
            };
        }

        private async Task Persist(AuditLog row)
        {
            try
            {
                _context.AuditLogs.Add(row);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                // Audit writes must NEVER block the user-facing flow.
                // Surface the failure to NLog (via ILogger pipeline)
                // so operators can chase it, then carry on - a
                // missing audit row beats a 500 on login.
                _logger.LogError(ex,
                    "Failed to persist audit row: {Action} / {EntityType} / {EntityId}",
                    row.Action, row.EntityType, row.EntityId);
            }
        }
    }
}
