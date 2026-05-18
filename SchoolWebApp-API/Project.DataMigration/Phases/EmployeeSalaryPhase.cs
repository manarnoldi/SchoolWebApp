using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Project.DataMigration.Mssql;
using Project.Infrastructure.Data;
using SchoolWebApp.Core.Entities.Payroll;

namespace Project.DataMigration.Phases;

// Creates one EmployeeSalary row per currently-employed staff member migrated
// in StaffDetailsPhase. BasicSalary maps from tblSchoolStaff.BasicSalary. The
// difference (GROSSSALARY - BasicSalary), if positive, lands in OtherAllowances
// as a placeholder; House/Transport breakdowns are left at 0 for the user to
// split later via the UI.
//
// EffectiveDate is forced to Jan 1 of ctx.TargetYear (2026) so all migrated
// salaries are picked up by the first 2026 payroll run.
//
// Wipe strategy: child EmployeeSalaryItems get wiped here too (FK to
// EmployeeSalaries) so the next phase can rebuild them cleanly.
public sealed class EmployeeSalaryPhase : IMigrationPhase
{
    public string Name => "EmployeeSalary";

    private record SourceStaffSalary(long EmpId, int? BasicSalary, int? GrossSalary);

    public async Task RunAsync(ApplicationDbContext db, MssqlSource src, MigrationContext ctx, ILogger logger, CancellationToken ct)
    {
        if (ctx.StaffMap.Count == 0)
            throw new InvalidOperationException("StaffMap is empty. Run StaffDetails phase first.");

        const string sql = @"
            SELECT empId, BasicSalary, GROSSSALARY
            FROM dbo.tblSchoolStaff
            WHERE status = 1
            ORDER BY empId;";

        var rows = await src.QueryAsync(sql, r => new SourceStaffSalary(
            MssqlSource.GetLong(r, "empId"),
            MssqlSource.GetNullableInt(r, "BasicSalary"),
            MssqlSource.GetNullableInt(r, "GROSSSALARY")
        ), ct);

        // Wipe child items first to clear the FK, then the salary rows themselves.
        await PhaseHelpers.TruncateAsync(db, "EmployeeSalaryItems");
        await PhaseHelpers.TruncateAsync(db, "EmployeeSalaries");

        var added = 0;
        var skipped = 0;
        var effective = new DateTime(ctx.TargetYear, 1, 1);

        foreach (var row in rows)
        {
            if (!ctx.StaffMap.TryGetValue(row.EmpId, out var staffDetailsId))
            {
                skipped++;
                logger.LogDebug("Skipping empId={Id} - staff not in StaffMap (probably skipped by StaffDetailsPhase).", row.EmpId);
                continue;
            }

            decimal basic = row.BasicSalary ?? 0;
            decimal gross = row.GrossSalary ?? 0;
            decimal other = gross > basic ? gross - basic : 0m;

            db.EmployeeSalaries.Add(new EmployeeSalary
            {
                StaffDetailsId      = staffDetailsId,
                BasicSalary         = basic,
                HouseAllowance      = 0m,
                TransportAllowance  = 0m,
                OtherAllowances     = other,
                EffectiveDate       = effective,
                IsActive            = true,
                Notes               = $"Imported from legacy database (legacy empId={row.EmpId}; gross={gross})"
            });
            added++;
        }

        await db.SaveChangesAsync(ct);

        logger.LogInformation("EmployeeSalary: inserted {Added}, skipped {Skipped} (of {Total} source rows). EffectiveDate={Date:yyyy-MM-dd}.",
            added, skipped, rows.Count, effective);
    }
}
