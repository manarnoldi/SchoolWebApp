using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Project.DataMigration.Mssql;
using Project.Infrastructure.Data;
using SchoolWebApp.Core.Entities.School;

namespace Project.DataMigration.Phases;

// One SchoolStream per DISTINCT non-empty tblClasses.stream value.
// Abbreviation defaults to the first 3 chars of the stream name when the name
// is long; otherwise the name itself (which is fine for typical "East"/"A"/etc).
public sealed class SchoolStreamPhase : IMigrationPhase
{
    public string Name => "SchoolStream";

    public async Task RunAsync(ApplicationDbContext db, MssqlSource src, MigrationContext ctx, ILogger logger, CancellationToken ct)
    {
        await PhaseHelpers.TruncateAsync(db, "SchoolStreams");

        const string sql = @"
            SELECT DISTINCT LTRIM(RTRIM(stream)) AS stream
            FROM dbo.tblClasses
            WHERE stream IS NOT NULL AND LTRIM(RTRIM(stream)) <> ''
            ORDER BY stream;";

        var streams = await src.QueryAsync(sql, r => r.GetString(r.GetOrdinal("stream")), ct);
        if (streams.Count == 0)
        {
            logger.LogWarning("No school streams found in source.");
            return;
        }

        var rank = 1;
        foreach (var s in streams)
        {
            db.SchoolStreams.Add(new SchoolStream
            {
                Name = s,
                Abbreviation = s.Length <= 10 ? s : s[..10],
                Description = $"Imported from legacy database",
                Rank = rank++
            });
        }

        await db.SaveChangesAsync(ct);

        ctx.SchoolStreamMap.Clear();
        foreach (var st in await db.SchoolStreams.AsNoTracking().ToListAsync(ct))
            ctx.SchoolStreamMap[st.Name] = st.Id;

        logger.LogInformation("SchoolStream: inserted {Count} rows.", streams.Count);
    }
}
