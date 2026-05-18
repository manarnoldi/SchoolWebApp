using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Project.DataMigration.Mssql;
using Project.Infrastructure.Data;
using SchoolWebApp.Core.Entities.School;

namespace Project.DataMigration.Phases;

// Copies tblSchoolDepts -> Departments. Source 'deptHead' is a free-text staff name and
// cannot be resolved to StaffDetailsId at this point (Staff phase has not run); we drop
// the value into Description for later manual reconciliation.
//
// Source columns are nullable; rows with no usable name/code are skipped with a warning.
public sealed class DepartmentPhase : IMigrationPhase
{
    public string Name => "Department";

    private record SourceDept(long DeptId, string? DeptCode, string? DeptName, string? DeptHead, bool? Status, DateTime? DateOfReg, string? RegBy);

    public async Task RunAsync(ApplicationDbContext db, MssqlSource src, MigrationContext ctx, ILogger logger, CancellationToken ct)
    {
        await PhaseHelpers.TruncateAsync(db, "Departments");

        const string sql = @"
            SELECT deptId, deptCode, deptName, deptHead, status, dateOfReg, regBy
            FROM dbo.tblSchoolDepts
            ORDER BY deptId;";

        var rows = await src.QueryAsync(sql, r => new SourceDept(
            MssqlSource.GetLong(r, "deptId"),
            MssqlSource.GetNullableString(r, "deptCode"),
            MssqlSource.GetNullableString(r, "deptName"),
            MssqlSource.GetNullableString(r, "deptHead"),
            MssqlSource.GetNullableBool(r, "status"),
            MssqlSource.GetNullableDateTime(r, "dateOfReg"),
            MssqlSource.GetNullableString(r, "regBy")
        ), ct);

        var skipped = 0;
        var added = new List<(long oldId, Department dept)>();

        foreach (var row in rows)
        {
            var name = (row.DeptName ?? row.DeptCode)?.Trim();
            if (string.IsNullOrWhiteSpace(name))
            {
                logger.LogWarning("Skipping tblSchoolDepts.deptId={Id} - both deptName and deptCode are null/empty.", row.DeptId);
                skipped++;
                continue;
            }

            var code = (row.DeptCode ?? row.DeptName)?.Trim();
            if (string.IsNullOrWhiteSpace(code)) code = $"DEPT-{row.DeptId}";

            // Target columns cap at Name(500) / Code(255). Source is varchar(50) so no truncation needed.
            var dept = new Department
            {
                Name = name,
                Code = code,
                Description = string.IsNullOrWhiteSpace(row.DeptHead) ? null : $"Legacy dept head: {row.DeptHead!.Trim()}"
            };
            db.Departments.Add(dept);
            added.Add((row.DeptId, dept));
        }

        await db.SaveChangesAsync(ct);

        ctx.DepartmentMap.Clear();
        foreach (var (oldId, dept) in added) ctx.DepartmentMap[oldId] = dept.Id;

        logger.LogInformation("Department: inserted {Inserted} rows, skipped {Skipped}.", added.Count, skipped);
    }
}
