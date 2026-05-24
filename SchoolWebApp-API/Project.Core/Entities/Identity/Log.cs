using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SchoolWebApp.Core.Entities.Identity
{
    /// <summary>
    /// Application log row written by NLog's DatabaseTarget. Only Error-level
    /// and above are persisted (see nlog.config). The table is owned by NLog at
    /// runtime; EF only owns the schema definition + the read endpoints.
    /// </summary>
    [Table("Logs")]
    public class Log
    {
        public int Id { get; set; }

        public DateTime Logged { get; set; }

        [StringLength(50)]
        public string? Level { get; set; }

        [Column(TypeName = "text")]
        public string? Message { get; set; }

        [StringLength(255)]
        public string? Logger { get; set; }

        [Column(TypeName = "text")]
        public string? Exception { get; set; }

        [StringLength(2048)]
        public string? Url { get; set; }

        [StringLength(512)]
        public string? CallSite { get; set; }

        [StringLength(255)]
        public string? MachineName { get; set; }

        // Authenticated user at the time the request was made — pulled from
        // User.Identity.Name by NLog via ${aspnet-user-identity}. Null for
        // anonymous or background work (startup seeding, hosted services).
        [StringLength(255)]
        public string? UserName { get; set; }

        // Resolution audit. Resolved=true means an operator has triaged this
        // row and it shouldn't surface in the default "open errors" view
        // anymore. ResolutionNote is optional but strongly encouraged — it
        // doubles as a runbook the next time the same signature shows up.
        public bool Resolved { get; set; }

        [StringLength(255)]
        public string? ResolvedBy { get; set; }

        public DateTime? ResolvedAt { get; set; }

        [Column(TypeName = "text")]
        public string? ResolutionNote { get; set; }
    }
}
