using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using SchoolWebApp.Core.Entities.Security;

namespace SchoolWebApp.Core.Interfaces.IRepositories.Security
{
    /// <summary>
    /// Read + Add only. AuditLog is append-only by convention - no
    /// Update, no Delete. If rows need to disappear (e.g. retention
    /// pass), do it via a scheduled SQL job, not through this API.
    /// </summary>
    public interface IAuditLogRepository
    {
        void Add(AuditLog auditLog);

        Task<IEnumerable<AuditLog>> Find(
            Expression<Func<AuditLog, bool>> filter = null,
            int? skip = null,
            int? take = null);

        Task<int> RecordCount(Expression<Func<AuditLog, bool>> filter = null);
    }
}
