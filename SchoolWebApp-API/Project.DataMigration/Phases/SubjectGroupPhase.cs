using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Project.DataMigration.Mssql;
using Project.Infrastructure.Data;
using SchoolWebApp.Core.Entities.Academics;

namespace Project.DataMigration.Phases;

// One SubjectGroup per DISTINCT non-empty tblSubjects.subGroup value. Required FK
// CurriculumId resolves to the FIRST Curriculum row found in MySQL (the project
// seeds Kenya CBC curriculum data via seed-kenya-cbc-curriculum.sql etc.).
// Aborts if no curriculum exists.
public sealed class SubjectGroupPhase : IMigrationPhase
{
    public string Name => "SubjectGroup";

    public async Task RunAsync(ApplicationDbContext db, MssqlSource src, MigrationContext ctx, ILogger logger, CancellationToken ct)
    {
        var defaultCurriculumId = await db.Curricula.AsNoTracking().OrderBy(c => c.Id).Select(c => (int?)c.Id).FirstOrDefaultAsync(ct);
        if (defaultCurriculumId is null)
        {
            throw new InvalidOperationException(
                "No Curriculum rows found in MySQL. Seed at least one Curriculum (e.g. via seed-kenya-cbc-curriculum.sql) before running SubjectGroup.");
        }

        await PhaseHelpers.TruncateAsync(db, "SubjectGroups");

        const string sql = @"
            SELECT DISTINCT LTRIM(RTRIM(subGroup)) AS subGroup
            FROM dbo.tblSubjects
            WHERE subGroup IS NOT NULL AND LTRIM(RTRIM(subGroup)) <> ''
            ORDER BY subGroup;";

        var groups = await src.QueryAsync(sql, r => r.GetString(r.GetOrdinal("subGroup")), ct);
        if (groups.Count == 0)
        {
            logger.LogWarning("No subject groups found in source.");
            return;
        }

        var rank = 1;
        foreach (var g in groups)
        {
            db.SubjectGroups.Add(new SubjectGroup
            {
                Name = g,
                Abbreviation = g.Length <= 10 ? g : g[..10],
                Description = "Imported from legacy database",
                Rank = rank++,
                CurriculumId = defaultCurriculumId.Value
            });
        }
        await db.SaveChangesAsync(ct);

        ctx.SubjectGroupMap.Clear();
        foreach (var sg in await db.SubjectGroups.AsNoTracking().ToListAsync(ct))
            ctx.SubjectGroupMap[sg.Name] = sg.Id;

        logger.LogInformation("SubjectGroup: inserted {Count} rows (CurriculumId={CurriculumId}).", groups.Count, defaultCurriculumId);
    }
}
