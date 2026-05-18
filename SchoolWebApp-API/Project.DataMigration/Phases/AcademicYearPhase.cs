using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Project.DataMigration.Mssql;
using Project.Infrastructure.Data;
using SchoolWebApp.Core.Entities.Academics;

namespace Project.DataMigration.Phases;

// One AcademicYear per DISTINCT tblClasses.year. Status=true only for the max year.
// We also union in tblSchoolCalendar.year so terms tied to a year that never had
// classes still get an AcademicYear row.
public sealed class AcademicYearPhase : IMigrationPhase
{
    public string Name => "AcademicYear";

    public async Task RunAsync(ApplicationDbContext db, MssqlSource src, MigrationContext ctx, ILogger logger, CancellationToken ct)
    {
        await PhaseHelpers.TruncateAsync(db, "AcademicYears");

        const string sql = @"
            SELECT DISTINCT year
            FROM (
                SELECT [year] FROM dbo.tblClasses        WHERE [year] IS NOT NULL
                UNION
                SELECT [year] FROM dbo.tblSchoolCalendar WHERE [year] IS NOT NULL
            ) y
            ORDER BY year;";

        var years = await src.QueryAsync(sql, r => r.GetInt32(r.GetOrdinal("year")), ct);
        if (years.Count == 0)
        {
            logger.LogWarning("No academic years found in source.");
            return;
        }

        var maxYear = years.Max();
        var rank = 1;
        foreach (var y in years)
        {
            var entity = new AcademicYear
            {
                Name = y.ToString(),
                Abbreviation = y.ToString(),
                StartDate = new DateTime(y, 1, 1),
                EndDate = new DateTime(y, 12, 31),
                Rank = rank++,
                Status = (y == maxYear),
                Description = $"Imported from legacy database (year {y})"
            };
            db.AcademicYears.Add(entity);
        }

        await db.SaveChangesAsync(ct);

        // Build old-year -> new-Id map for downstream phases.
        ctx.AcademicYearMap.Clear();
        foreach (var ay in await db.AcademicYears.AsNoTracking().ToListAsync(ct))
        {
            if (int.TryParse(ay.Name, out var yearInt))
                ctx.AcademicYearMap[yearInt] = ay.Id;
        }

        logger.LogInformation("AcademicYear: inserted {Count} rows (max year = {MaxYear}).", years.Count, maxYear);
    }
}
