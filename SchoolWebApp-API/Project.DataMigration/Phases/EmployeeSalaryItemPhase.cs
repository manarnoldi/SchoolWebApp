using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Project.DataMigration.Mssql;
using Project.Infrastructure.Data;
using SchoolWebApp.Core.Entities.Payroll;

namespace Project.DataMigration.Phases;

// Migrates tblPayrollEmployeeDeduction -> EmployeeSalaryItems (deduction lines).
// Each source row carries (EmpId, DeductionId, Amount). The DeductionId references
// tblPayrollDeductions whose Code/Abbreviation/Name we match against the MySQL
// DeductionTypes (seeded by EF migration 20260416082030).
//
// Matching priority: Code -> Abbreviation -> Name (all case-insensitive). Rows
// whose deduction type can't be resolved are skipped with a warning.
//
// The parent EmployeeSalary row must exist (created by EmployeeSalaryPhase).
public sealed class EmployeeSalaryItemPhase : IMigrationPhase
{
    public string Name => "EmployeeSalaryItem";

    // Aliases from source Code/Abbreviation/Name to a canonical MySQL DeductionType.Code.
    // Used when the source text doesn't match exactly - extend as new mismatches surface.
    private static readonly Dictionary<string, string> SourceAliasToMysqlCode
        = new(StringComparer.OrdinalIgnoreCase)
        {
            ["Staff Welfare"] = "WELFARE", // source name -> seeded 'Welfare' (Code='WELFARE')
            // NITA is a Kenyan employer levy not in the EF seed; auto-created below.
            ["NITA"] = "NITA",
        };

    // DeductionTypes that don't ship with the EF seed but exist in the legacy data.
    // Auto-created before the main lookup runs so the alias map above resolves.
    private static readonly (string Code, string Name, bool IsStatutory, string? Description)[] AutoCreateDeductions =
    {
        ("NITA", "NITA (Industrial Training Levy)", true, "Kenyan National Industrial Training Authority levy; auto-created by data migration."),
    };

    private record SourceItem(long EmpId, double Amount, string? Code, string? Abbreviation, string? DeductionName);

    public async Task RunAsync(ApplicationDbContext db, MssqlSource src, MigrationContext ctx, ILogger logger, CancellationToken ct)
    {
        if (ctx.StaffMap.Count == 0)
            throw new InvalidOperationException("StaffMap is empty. Run StaffDetails phase first.");

        // Pre-create any DeductionTypes the legacy data needs but the EF seed didn't ship.
        await EnsureAutoCreatedDeductionsAsync(db, logger, ct);

        // Load deduction-type lookups from MySQL.
        var dtList = await db.DeductionTypes.AsNoTracking().ToListAsync(ct);
        if (dtList.Count == 0)
            throw new InvalidOperationException("No DeductionTypes found in MySQL. The EF migration seed appears not to have run.");

        var dtByCode = dtList
            .Where(d => !string.IsNullOrWhiteSpace(d.Code))
            .GroupBy(d => d.Code, StringComparer.OrdinalIgnoreCase)
            .ToDictionary(g => g.Key, g => g.First().Id, StringComparer.OrdinalIgnoreCase);
        var dtByName = dtList
            .GroupBy(d => d.Name, StringComparer.OrdinalIgnoreCase)
            .ToDictionary(g => g.Key, g => g.First().Id, StringComparer.OrdinalIgnoreCase);

        // Load StaffDetailsId -> EmployeeSalaryId lookup (one salary per active staff).
        var salaryMap = await db.EmployeeSalaries
            .AsNoTracking()
            .Where(es => es.IsActive)
            .ToDictionaryAsync(es => es.StaffDetailsId, es => es.Id, ct);
        if (salaryMap.Count == 0)
            throw new InvalidOperationException("No EmployeeSalary rows in MySQL. Run EmployeeSalary phase first.");

        // EmployeeSalaryItems was already truncated by EmployeeSalaryPhase. Truncate
        // again here so this phase is also safe to run standalone for re-tries.
        await PhaseHelpers.TruncateAsync(db, "EmployeeSalaryItems");

        const string sql = @"
            SELECT ed.EmpId, ed.Amount, d.Code, d.Abbreviation, d.Name AS DName
            FROM dbo.tblPayrollEmployeeDeduction ed
            JOIN dbo.tblPayrollDeductions        d ON d.Id = ed.DeductionId
            WHERE ed.Amount IS NOT NULL AND ed.Amount > 0
            ORDER BY ed.EmpId, ed.Id;";

        var rows = await src.QueryAsync(sql, r => new SourceItem(
            MssqlSource.GetLong(r, "EmpId"),
            r.GetDouble(r.GetOrdinal("Amount")),
            MssqlSource.GetNullableString(r, "Code"),
            MssqlSource.GetNullableString(r, "Abbreviation"),
            MssqlSource.GetNullableString(r, "DName")
        ), ct);

        var added = 0;
        var skippedNoStaff = 0;
        var skippedNoType  = 0;

        foreach (var row in rows)
        {
            if (!ctx.StaffMap.TryGetValue(row.EmpId, out var staffDetailsId))
            {
                skippedNoStaff++;
                continue;
            }
            if (!salaryMap.TryGetValue(staffDetailsId, out var salaryId))
            {
                skippedNoStaff++;
                continue;
            }

            int? deductionTypeId =
                TryGet(dtByCode, row.Code) ??
                TryGet(dtByCode, row.Abbreviation) ??
                TryGet(dtByName, row.DeductionName) ??
                // Alias fallback: map source Code/Abbreviation/Name to a canonical MySQL Code.
                TryGet(dtByCode, TryGet(SourceAliasToMysqlCode, row.Code)) ??
                TryGet(dtByCode, TryGet(SourceAliasToMysqlCode, row.Abbreviation)) ??
                TryGet(dtByCode, TryGet(SourceAliasToMysqlCode, row.DeductionName));

            if (deductionTypeId is null)
            {
                skippedNoType++;
                logger.LogWarning("Skipping deduction (empId={Emp}, code='{Code}', abbr='{Abbr}', name='{Name}') - no matching MySQL DeductionType.",
                    row.EmpId, row.Code, row.Abbreviation, row.DeductionName);
                continue;
            }

            db.EmployeeSalaryItems.Add(new EmployeeSalaryItem
            {
                EmployeeSalaryId = salaryId,
                DeductionTypeId  = deductionTypeId,
                Amount           = (decimal)row.Amount
            });
            added++;
        }

        await db.SaveChangesAsync(ct);

        logger.LogInformation(
            "EmployeeSalaryItem: inserted {Added}, skipped {NoStaff} (unmapped staff/salary) + {NoType} (unmapped deduction type) of {Total} source rows.",
            added, skippedNoStaff, skippedNoType, rows.Count);
    }

    private static int? TryGet(Dictionary<string, int> map, string? key)
        => !string.IsNullOrWhiteSpace(key) && map.TryGetValue(key.Trim(), out var id) ? id : null;

    private static string? TryGet(Dictionary<string, string> map, string? key)
        => !string.IsNullOrWhiteSpace(key) && map.TryGetValue(key.Trim(), out var v) ? v : null;

    private static async Task EnsureAutoCreatedDeductionsAsync(ApplicationDbContext db, ILogger logger, CancellationToken ct)
    {
        var existing = (await db.DeductionTypes.AsNoTracking().Select(d => d.Code).ToListAsync(ct))
            .ToHashSet(StringComparer.OrdinalIgnoreCase);

        var inserted = 0;
        foreach (var (code, name, isStatutory, description) in AutoCreateDeductions)
        {
            if (existing.Contains(code)) continue;
            db.DeductionTypes.Add(new SchoolWebApp.Core.Entities.Payroll.DeductionType
            {
                Code         = code,
                Name         = name,
                IsStatutory  = isStatutory,
                IsActive     = true,
                Description  = description
            });
            inserted++;
        }
        if (inserted > 0)
        {
            await db.SaveChangesAsync(ct);
            logger.LogInformation("Auto-created {Count} missing DeductionType(s) needed by legacy data.", inserted);
        }
    }
}
