using System.Threading.Tasks;

namespace SchoolWebApp.Core.Services
{
    /// <summary>
    /// Explicit audit hooks for events the SaveChanges interceptor can't
    /// see: authentication, reads of sensitive student records, printed
    /// reports, etc. Each call persists immediately. Failures are
    /// swallowed and logged via NLog: an audit write that fails must
    /// never block the user-facing flow.
    /// </summary>
    public interface IAuditService
    {
        Task LogAsync(
            string action,
            string entityType = null,
            string entityId = null,
            string notes = null);

        Task LogLoginAsync(string userName, string userId);
        Task LogLoginFailedAsync(string attemptedUserName, string reason);
        Task LogLogoutAsync(string userName, string userId);
        Task LogViewAsync(string entityType, string entityId, string notes = null);
        Task LogPrintAsync(string entityType, string entityId, string notes = null);
    }
}
