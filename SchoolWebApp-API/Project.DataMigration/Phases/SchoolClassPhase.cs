using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Project.DataMigration.Mssql;
using Project.Infrastructure.Data;
using SchoolWebApp.Core.Entities.Class;
using SchoolWebApp.Core.Entities.School;

namespace Project.DataMigration.Phases;

// Migrates tblClasses -> SchoolClasses. Resolves three FKs:
//   - LearningLevelId  via case-insensitive name match against existing MySQL LearningLevels
//                      (the user maintains this lookup themselves: PP1, PP2, Grade 1..9, Special Class, Day Care)
//   - SchoolStreamId   from MigrationContext.SchoolStreamMap; for source rows with no stream
//                      we fall back to an auto-created "N/A" stream
//   - AcademicYearId   from MigrationContext.AcademicYearMap by year int
//
// Source class names like "GRADE 1" (uppercase) are normalised case-insensitively.
// Rows with no resolvable LearningLevel or AcademicYear are skipped and logged.
public sealed class SchoolClassPhase : IMigrationPhase
{
    public string Name => "SchoolClass";

    // Explicit overrides for source className values that can't be resolved to a
    // LearningLevel via name match - typically because the legacy app crammed extra
    // info into the className column. Each entry maps source -> (target LearningLevel name,
    // target SchoolClass.Name to use verbatim). Extend as new edge cases surface.
    private static readonly Dictionary<string, (string LearningLevel, string ClassName)> ClassNameOverrides
        = new(StringComparer.OrdinalIgnoreCase)
        {
            ["SPECIAL CLASS H"]   = ("Special Class", "Special Class 2026 I"),
            ["SPECIAL CLASS H 2"] = ("Special Class", "Special Class 2026 II"),
        };

    // Collapses whitespace + lowercases so 'PP 1' and 'PP1' resolve to the same key.
    private static string Normalise(string s) => Regex.Replace(s.Trim(), @"\s+", "").ToLowerInvariant();

    private record SourceClass(long ClassId, string? ClassName, string? Stream, int? Year, bool? Status, string? RegBy, DateTime? DateOfReg);

    public async Task RunAsync(ApplicationDbContext db, MssqlSource src, MigrationContext ctx, ILogger logger, CancellationToken ct)
    {
        if (ctx.AcademicYearMap.Count == 0)
            throw new InvalidOperationException("AcademicYearMap is empty. Run AcademicYear phase first.");
        if (ctx.SchoolStreamMap.Count == 0)
            logger.LogWarning("SchoolStreamMap is empty - all classes will fall back to the 'N/A' stream.");

        // Load LearningLevels from MySQL. We index three ways so we can:
        //   - look up by exact (case-insensitive) name           -> levelByName
        //   - fall back to whitespace-normalised name            -> levelByNorm
        //     ('PP1' in source matches 'PP 1' in MySQL, etc.)
        //   - map back to the canonical level name from an Id   -> levelNameById
        //     so the target SchoolClass.Name uses MySQL's casing, not the source UPPERCASE
        var allLevels       = await db.LearningLevels.AsNoTracking().ToListAsync(ct);
        var levelByName     = allLevels.ToDictionary(l => l.Name, l => l.Id, StringComparer.OrdinalIgnoreCase);
        var levelByNorm     = allLevels.ToDictionary(l => Normalise(l.Name), l => l.Id);
        var levelNameById   = allLevels.ToDictionary(l => l.Id, l => l.Name);
        if (allLevels.Count == 0)
            throw new InvalidOperationException("No LearningLevels found in MySQL. Seed PP1, PP2, Grade 1..9, Special Class, etc. first.");

        // Year-scoped: only migrate classes for the configured TargetYear.
        // Read source rows up front so we can decide whether to create the fallback stream
        // before truncating SchoolClasses.
        var sql = $@"
            SELECT classId, className, stream, [year], status, regBy, dateOfReg
            FROM dbo.tblClasses
            WHERE [year] = {ctx.TargetYear}
            ORDER BY [year], className, stream;";

        logger.LogInformation("SchoolClass: filtering source by year = {Year}.", ctx.TargetYear);

        var rows = await src.QueryAsync(sql, r => new SourceClass(
            MssqlSource.GetLong(r, "classId"),
            MssqlSource.GetNullableString(r, "className"),
            MssqlSource.GetNullableString(r, "stream"),
            MssqlSource.GetNullableInt(r, "year"),
            MssqlSource.GetNullableBool(r, "status"),
            MssqlSource.GetNullableString(r, "regBy"),
            MssqlSource.GetNullableDateTime(r, "dateOfReg")
        ), ct);

        // Ensure a fallback stream exists if any source class has no stream value.
        var needsFallbackStream = rows.Any(r => string.IsNullOrWhiteSpace(r.Stream));
        int? fallbackStreamId = null;
        if (needsFallbackStream)
        {
            const string fallbackName = "N/A";
            if (!ctx.SchoolStreamMap.TryGetValue(fallbackName, out var existingId))
            {
                var fb = new SchoolStream
                {
                    Name = fallbackName,
                    Abbreviation = "NA",
                    Description = "Fallback stream for legacy classes that had no stream value",
                    Rank = ctx.SchoolStreamMap.Count + 1
                };
                db.SchoolStreams.Add(fb);
                await db.SaveChangesAsync(ct);
                ctx.SchoolStreamMap[fallbackName] = fb.Id;
                fallbackStreamId = fb.Id;
                logger.LogInformation("Created fallback SchoolStream 'N/A' (Id={Id}) for {Count} stream-less source classes.", fb.Id, rows.Count(r => string.IsNullOrWhiteSpace(r.Stream)));
            }
            else
            {
                fallbackStreamId = existingId;
            }
        }

        await PhaseHelpers.TruncateAsync(db, "SchoolClasses");

        var rank = 1;
        var skipped = 0;
        var added = new List<(long oldId, SchoolClass cls)>();

        foreach (var row in rows)
        {
            var levelName = row.ClassName?.Trim();
            if (string.IsNullOrEmpty(levelName))
            {
                logger.LogWarning("Skipping classId={Id} - className is null/empty.", row.ClassId);
                skipped++; continue;
            }

            // Resolve LearningLevel: explicit override -> exact name -> whitespace-normalised fallback.
            int learningLevelId;
            string? overrideClassName = null;

            if (ClassNameOverrides.TryGetValue(levelName, out var ov))
            {
                if (!levelByName.TryGetValue(ov.LearningLevel, out learningLevelId))
                {
                    logger.LogWarning("Skipping classId={Id} - override target LearningLevel '{Target}' missing in MySQL.", row.ClassId, ov.LearningLevel);
                    skipped++; continue;
                }
                overrideClassName = ov.ClassName;
            }
            else if (!levelByName.TryGetValue(levelName, out learningLevelId)
                  && !levelByNorm.TryGetValue(Normalise(levelName), out learningLevelId))
            {
                logger.LogWarning("Skipping classId={Id} - LearningLevel '{Level}' not found in MySQL (tried exact + normalised).", row.ClassId, levelName);
                skipped++; continue;
            }

            if (row.Year is null || !ctx.AcademicYearMap.TryGetValue(row.Year.Value, out var academicYearId))
            {
                logger.LogWarning("Skipping classId={Id} - year {Year} not in AcademicYearMap.", row.ClassId, row.Year);
                skipped++; continue;
            }

            int streamId;
            var streamName = row.Stream?.Trim();
            if (string.IsNullOrEmpty(streamName))
            {
                streamId = fallbackStreamId!.Value;
            }
            else if (!ctx.SchoolStreamMap.TryGetValue(streamName, out streamId))
            {
                logger.LogWarning("Skipping classId={Id} - stream '{Stream}' not in SchoolStreamMap.", row.ClassId, streamName);
                skipped++; continue;
            }

            // Compose target Name:
            //   - explicit override wins outright,
            //   - otherwise use the MySQL-canonical level name + optional stream.
            var canonicalLevel = levelNameById[learningLevelId];
            var name = overrideClassName
                       ?? (string.IsNullOrEmpty(streamName) ? canonicalLevel : $"{canonicalLevel} {streamName}");

            var cls = new SchoolClass
            {
                Name = name,
                Description = row.RegBy is null ? null : $"Legacy classId={row.ClassId}; regBy={row.RegBy}",
                LearningLevelId = learningLevelId,
                SchoolStreamId  = streamId,
                AcademicYearId  = academicYearId,
                Rank = rank++
            };
            db.SchoolClasses.Add(cls);
            added.Add((row.ClassId, cls));
        }

        await db.SaveChangesAsync(ct);

        ctx.SchoolClassMap.Clear();
        foreach (var (oldId, cls) in added) ctx.SchoolClassMap[oldId] = cls.Id;

        logger.LogInformation("SchoolClass: inserted {Inserted}, skipped {Skipped} (of {Total} source rows).", added.Count, skipped, rows.Count);
    }
}
