using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Project.DataMigration.Mssql;
using Project.Infrastructure.Data;
using SchoolWebApp.Core.Entities.Settings;

namespace Project.DataMigration.Phases;

// Ensures lookup rows that Student/Staff phases depend on exist in MySQL.
// EF migrations only seed Gender 'Male' (Id=1); we add 'Female' here. Religions
// are auto-created on the fly by the Student/Staff phases as values are
// encountered, so this phase only needs to guarantee the Female gender.
//
// Non-destructive: this phase NEVER truncates - it only inserts what's missing.
public sealed class PersonReferenceSeedPhase : IMigrationPhase
{
    public string Name => "PersonReferenceSeed";

    public async Task RunAsync(ApplicationDbContext db, MssqlSource src, MigrationContext ctx, ILogger logger, CancellationToken ct)
    {
        var existsFemale = await db.Genders.AnyAsync(g => g.Name == "Female", ct);
        if (!existsFemale)
        {
            db.Genders.Add(new Gender
            {
                Name = "Female",
                Description = "Auto-created by data migration",
                Rank = 2
            });
            await db.SaveChangesAsync(ct);
            logger.LogInformation("Inserted missing Gender 'Female'.");
        }
        else
        {
            logger.LogInformation("Gender 'Female' already present.");
        }
    }
}
