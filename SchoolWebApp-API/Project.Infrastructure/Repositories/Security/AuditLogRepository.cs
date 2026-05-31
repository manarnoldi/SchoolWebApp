using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Project.Infrastructure.Data;
using SchoolWebApp.Core.Entities.Security;
using SchoolWebApp.Core.Interfaces.IRepositories.Security;

namespace Project.Infrastructure.Repositories.Security
{
    public class AuditLogRepository : IAuditLogRepository
    {
        private readonly ApplicationDbContext _context;
        public AuditLogRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public void Add(AuditLog auditLog)
        {
            _context.AuditLogs.Add(auditLog);
        }

        public async Task<IEnumerable<AuditLog>> Find(
            Expression<Func<AuditLog, bool>> filter = null,
            int? skip = null,
            int? take = null)
        {
            IQueryable<AuditLog> q = _context.AuditLogs.AsNoTracking();
            if (filter != null) q = q.Where(filter);

            q = q.OrderByDescending(a => a.Timestamp);

            if (skip.HasValue) q = q.Skip(skip.Value);
            if (take.HasValue) q = q.Take(take.Value);
            return await q.ToListAsync();
        }

        public async Task<int> RecordCount(Expression<Func<AuditLog, bool>> filter = null)
        {
            IQueryable<AuditLog> q = _context.AuditLogs.AsNoTracking();
            if (filter != null) q = q.Where(filter);
            return await q.CountAsync();
        }
    }
}
