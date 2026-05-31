using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SchoolWebApp.Core.Entities.Security
{
    /// <summary>
    /// Append-only audit trail of user-meaningful actions. Distinct from
    /// the NLog-driven Log table - this answers "who did what, to what,
    /// when" for admins and compliance, not "what crashed". Writes
    /// happen either automatically from the ApplicationDbContext
    /// SaveChanges hook (entity Create/Update/Delete) or explicitly via
    /// IAuditService (Login, Logout, View, Print, etc.).
    ///
    /// Deliberately does NOT inherit Base - audit rows have their own
    /// timestamp, the actor is captured explicitly, and they must not
    /// themselves be audited (would loop).
    /// </summary>
    public class AuditLog
    {
        public long Id { get; set; }

        // Wall-clock UTC at the moment the action was recorded. Set by
        // the writer, not the audit interceptor (the interceptor skips
        // this table) so we don't double-stamp.
        public DateTime Timestamp { get; set; }

        // Identity of the actor. UserId is the AspNetUsers.Id; UserName
        // is the human-readable handle pulled off the JWT for fast
        // reports without a join. Either may be null on anon pre-auth
        // events (e.g. a failed login attempt where the submitted
        // username doesn't exist).
        [StringLength(450)]
        public string? UserId { get; set; }

        [StringLength(255)]
        public string? UserName { get; set; }

        // Short verb: Create / Update / Delete / Login / LoginFailed /
        // Logout / View / Print. Free string rather than enum so
        // feature code can introduce new verbs without a schema
        // migration.
        [Required]
        [StringLength(40)]
        public string Action { get; set; } = "";

        [StringLength(100)]
        public string? EntityType { get; set; }

        [StringLength(100)]
        public string? EntityId { get; set; }

        // Field-level snapshots. For Create: only NewValues. For
        // Update: only the *changed* properties on both sides. For
        // Delete: only OldValues. For non-entity events (Login etc.):
        // both null.
        [Column(TypeName = "longtext")]
        public string? OldValues { get; set; }

        [Column(TypeName = "longtext")]
        public string? NewValues { get; set; }

        [StringLength(64)]
        public string? IpAddress { get; set; }

        [StringLength(512)]
        public string? UserAgent { get; set; }

        [StringLength(512)]
        public string? RequestPath { get; set; }

        // Free-text bag for verb-specific context that doesn't fit
        // OldValues/NewValues. Example: LoginFailed reason, Print job
        // page count.
        [StringLength(2048)]
        public string? Notes { get; set; }
    }
}
