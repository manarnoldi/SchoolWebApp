using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Project.DataMigration.Mssql;
using Project.Infrastructure.Data;
using SchoolWebApp.Core.Entities.Students;

namespace Project.DataMigration.Phases;

// Migrates tblClassStudent -> StudentClasses. Year-scoped: only rows where the
// joined tblClasses.year equals ctx.TargetYear are imported.
//
// Source class and student PKs are translated via in-memory maps populated by
// SchoolClassPhase and StudentPhase. Rows whose class or student didn't make it
// through earlier phases (e.g. unmapped LearningLevel or no name) are skipped
// with a warning.
public sealed class StudentClassPhase : IMigrationPhase
{
    public string Name => "StudentClass";

    private record SourceRow(long ClassStudId, long ClassId, long StudId, string? RegBy, bool? Status, DateTime? DateOfReg);

    public async Task RunAsync(ApplicationDbContext db, MssqlSource src, MigrationContext ctx, ILogger logger, CancellationToken ct)
    {
        if (ctx.SchoolClassMap.Count == 0)
            throw new InvalidOperationException("SchoolClassMap is empty. Run SchoolClass phase first.");
        if (ctx.StudentMap.Count == 0)
            throw new InvalidOperationException("StudentMap is empty. Run Student phase first.");

        await PhaseHelpers.TruncateAsync(db, "StudentClasses");

        var sql = $@"
            SELECT cs.classStudId, cs.classId, cs.studId, cs.regBy, cs.status, cs.dateOfReg
            FROM dbo.tblClassStudent cs
            JOIN dbo.tblClasses      c  ON c.classId = cs.classId
            WHERE c.[year] = {ctx.TargetYear}
            ORDER BY cs.classStudId;";

        logger.LogInformation("StudentClass: filtering source by year = {Year}.", ctx.TargetYear);

        var rows = await src.QueryAsync(sql, r => new SourceRow(
            MssqlSource.GetLong(r, "classStudId"),
            MssqlSource.GetLong(r, "classId"),
            MssqlSource.GetLong(r, "studId"),
            MssqlSource.GetNullableString(r, "regBy"),
            MssqlSource.GetNullableBool(r, "status"),
            MssqlSource.GetNullableDateTime(r, "dateOfReg")
        ), ct);

        var skippedNoClass   = 0;
        var skippedNoStudent = 0;
        var added            = 0;

        foreach (var row in rows)
        {
            if (!ctx.SchoolClassMap.TryGetValue(row.ClassId, out var schoolClassId))
            {
                skippedNoClass++;
                logger.LogWarning("Skipping classStudId={Id} - source classId={ClassId} not migrated.", row.ClassStudId, row.ClassId);
                continue;
            }
            if (!ctx.StudentMap.TryGetValue(row.StudId, out var studentId))
            {
                skippedNoStudent++;
                // Quiet - we expect plenty of these because the Student phase already filtered
                // its cohort by year via tblClassStudent. Log at Debug for diagnostic purposes only.
                logger.LogDebug("Skipping classStudId={Id} - source studId={StudId} not migrated.", row.ClassStudId, row.StudId);
                continue;
            }

            db.StudentClasses.Add(new StudentClass
            {
                StudentId     = studentId,
                SchoolClassId = schoolClassId,
                Description   = row.RegBy is null ? null : $"Legacy classStudId={row.ClassStudId}; regBy={row.RegBy}"
            });
            added++;
        }

        await db.SaveChangesAsync(ct);

        logger.LogInformation(
            "StudentClass: inserted {Added}, skipped {NoClass} (unmapped class) + {NoStudent} (unmapped student) of {Total} source rows.",
            added, skippedNoClass, skippedNoStudent, rows.Count);
    }
}
