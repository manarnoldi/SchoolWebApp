using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Project.DataMigration.Mssql;
using Project.Infrastructure.Data;
using SchoolWebApp.Core.Entities.Academics;
using SchoolWebApp.Core.Entities.School;

namespace Project.DataMigration.Phases;

// Migrates tblSubjects -> Subjects. The legacy schema has no subject->department FK,
// so every migrated subject is attached to a single 'General' Department which the
// phase auto-creates if it doesn't already exist. The user re-assigns proper
// departments later via the UI.
//
// SubjectGroup FK resolves from MigrationContext.SubjectGroupMap (case-insensitive)
// populated by SubjectGroupPhase; rows with no resolvable subGroup are skipped.
public sealed class SubjectPhase : IMigrationPhase
{
    public string Name => "Subject";

    private record SourceSubject(long SubjectId, string? Code, string? Name, string? SubGroup, bool? Status, string? Abbr, DateTime? DateOfReg, string? RegBy);

    public async Task RunAsync(ApplicationDbContext db, MssqlSource src, MigrationContext ctx, ILogger logger, CancellationToken ct)
    {
        if (ctx.SubjectGroupMap.Count == 0)
            throw new InvalidOperationException("SubjectGroupMap is empty. Run SubjectGroup phase first.");

        // Ensure the 'General' department exists (created lazily, never truncated by this phase).
        var generalDept = await db.Departments
            .Where(d => d.Name == "General")
            .Select(d => new { d.Id })
            .FirstOrDefaultAsync(ct);

        int generalDeptId;
        if (generalDept is null)
        {
            var newDept = new Department
            {
                Name = "General",
                Code = "GEN",
                Description = "Auto-created by data migration; reassign subjects to real departments via the UI."
            };
            db.Departments.Add(newDept);
            await db.SaveChangesAsync(ct);
            generalDeptId = newDept.Id;
            logger.LogInformation("Created 'General' Department (Id={Id}) as default for migrated subjects.", generalDeptId);
        }
        else
        {
            generalDeptId = generalDept.Id;
        }

        await PhaseHelpers.TruncateAsync(db, "Subjects");

        const string sql = @"
            SELECT subJectId, subCode, subName, subGroup, subStatus, abbr, dateOfReg, regBy
            FROM dbo.tblSubjects
            ORDER BY subJectId;";

        var rows = await src.QueryAsync(sql, r => new SourceSubject(
            MssqlSource.GetLong(r, "subJectId"),
            MssqlSource.GetNullableString(r, "subCode"),
            MssqlSource.GetNullableString(r, "subName"),
            MssqlSource.GetNullableString(r, "subGroup"),
            MssqlSource.GetNullableBool(r, "subStatus"),
            MssqlSource.GetNullableString(r, "abbr"),
            MssqlSource.GetNullableDateTime(r, "dateOfReg"),
            MssqlSource.GetNullableString(r, "regBy")
        ), ct);

        var skipped = 0;
        var added = new List<(long oldId, Subject subj)>();
        var rank = 1;

        foreach (var row in rows)
        {
            var name = row.Name?.Trim();
            if (string.IsNullOrEmpty(name))
            {
                logger.LogWarning("Skipping tblSubjects.subJectId={Id} - subName is null/empty.", row.SubjectId);
                skipped++; continue;
            }

            var groupName = row.SubGroup?.Trim();
            if (string.IsNullOrEmpty(groupName) || !ctx.SubjectGroupMap.TryGetValue(groupName, out var subjectGroupId))
            {
                logger.LogWarning("Skipping subject '{Name}' (subJectId={Id}) - subGroup '{Group}' not in SubjectGroupMap.", name, row.SubjectId, groupName);
                skipped++; continue;
            }

            var code = row.Code?.Trim();
            if (string.IsNullOrEmpty(code)) code = $"SUB-{row.SubjectId}";

            var abbr = row.Abbr?.Trim();
            if (string.IsNullOrEmpty(abbr))
                abbr = name.Length <= 3 ? name : name[..3];

            var subj = new Subject
            {
                Code = code,
                Name = name,
                Abbr = abbr,
                NumOfLessons = 0,
                Description = row.RegBy is null ? null : $"Legacy subjectId={row.SubjectId}; regBy={row.RegBy}",
                Optional = false,
                Examinable = row.Status ?? true,
                Rank = rank++,
                SubjectGroupId = subjectGroupId,
                DepartmentId   = generalDeptId
            };
            db.Subjects.Add(subj);
            added.Add((row.SubjectId, subj));
        }

        await db.SaveChangesAsync(ct);

        ctx.SubjectMap.Clear();
        foreach (var (oldId, s) in added) ctx.SubjectMap[oldId] = s.Id;

        logger.LogInformation("Subject: inserted {Inserted}, skipped {Skipped} (DepartmentId={DeptId}=General).", added.Count, skipped, generalDeptId);
    }
}
