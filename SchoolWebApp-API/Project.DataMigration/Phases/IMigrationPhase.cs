using Microsoft.Extensions.Logging;
using Project.DataMigration.Mssql;
using Project.Infrastructure.Data;

namespace Project.DataMigration.Phases;

public interface IMigrationPhase
{
    string Name { get; }
    Task RunAsync(ApplicationDbContext db, MssqlSource src, MigrationContext ctx, ILogger logger, CancellationToken ct);
}
