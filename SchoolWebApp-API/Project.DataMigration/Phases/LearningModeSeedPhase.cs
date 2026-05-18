using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Project.DataMigration.Mssql;
using Project.Infrastructure.Data;
using SchoolWebApp.Core.Entities.School;

namespace Project.DataMigration.Phases;

// Seeds the two LearningModes that map cleanly from tblStudDetails.boarder (bit):
//   Day      (Id=1, boarder=false)
//   Boarding (Id=2, boarder=true)
// Idempotent in spirit: truncates the table and re-inserts. The Student phase will
// look these up by name when migrating learners.
public sealed class LearningModeSeedPhase : IMigrationPhase
{
    public string Name => "LearningModeSeed";

    public async Task RunAsync(ApplicationDbContext db, MssqlSource src, MigrationContext ctx, ILogger logger, CancellationToken ct)
    {
        await PhaseHelpers.TruncateAsync(db, "LearningModes");

        db.LearningModes.AddRange(
            new LearningMode { Name = "Day",      Rank = 1, Description = "Day scholar" },
            new LearningMode { Name = "Boarding", Rank = 2, Description = "Boarder" }
        );
        await db.SaveChangesAsync(ct);

        logger.LogInformation("LearningModeSeed: inserted Day + Boarding.");
    }
}
