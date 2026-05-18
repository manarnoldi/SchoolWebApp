using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Project.DataMigration.Mssql;
using Project.Infrastructure.Data;
using SchoolWebApp.Core.Entities.Enums;
using SchoolWebApp.Core.Entities.Settings;
using SchoolWebApp.Core.Entities.Staff;

namespace Project.DataMigration.Phases;

// Migrates tblSchoolStaff WHERE status = 1 (currently employed) -> StaffDetails.
// The seeded admin StaffDetails row (Id=1) is preserved across runs; everything
// else with Id <> 1 is wiped before reinsert.
//
// FK defaults (StaffCategory/Designation/EmploymentType) all resolve to Id=1
// since the legacy schema doesn't carry equivalent classifications cleanly. The
// user can re-classify staff via the UI after migration.
//
// Religions are auto-created from the free-text source value; null/empty falls
// back to the default Religion Id=1 (Christian).
public sealed class StaffDetailsPhase : IMigrationPhase
{
    public string Name => "StaffDetails";

    private const int DefaultStaffCategoryId = 1; // Non-teaching (re-assign in UI)
    private const int DefaultDesignationId   = 1; // Supplier      (re-assign in UI)
    private const int DefaultEmploymentTypeId = 1; // Contract     (re-assign in UI)
    private const int DefaultNationalityId   = 1; // Kenyan
    private const int DefaultReligionId      = 1; // Christian
    private const int AdminStaffId           = 1; // Seeded by EF migrations - preserve.

    private record SourceStaff(
        long EmpId, string EmpNo, string? FullName, string? FName, string? MName, string? LName,
        string? Email, int? IdNumber, string? PhoneNo, bool? Sex,
        string? Residence, string? ContactAddress, string? Religion,
        DateTime? DateOfEmployment, DateTime? DateOfBirth, bool? Status,
        DateTime? ExitDate, string? Kraa, string? Nssf, string? Sha);

    public async Task RunAsync(ApplicationDbContext db, MssqlSource src, MigrationContext ctx, ILogger logger, CancellationToken ct)
    {
        var maleId   = await db.Genders.Where(g => g.Name == "Male").Select(g => g.Id).FirstAsync(ct);
        var femaleId = await db.Genders.Where(g => g.Name == "Female").Select(g => g.Id).FirstAsync(ct);
        var religionMap = await PersonHelpers.LoadReligionMapAsync(db, ct);

        // TPH inheritance: Student / StaffDetails / Parent all live in the single
        // `Person` table, separated by the `Discriminator` column. We delete only
        // StaffDetails rows (keeping the seeded admin at Id=1) so students/parents
        // are untouched.
        await db.Database.ExecuteSqlRawAsync("SET FOREIGN_KEY_CHECKS = 0;");
        try
        {
#pragma warning disable EF1002
            await db.Database.ExecuteSqlRawAsync(
                $"DELETE FROM `Person` WHERE `Discriminator` = 'StaffDetails' AND `Id` <> {AdminStaffId};");
#pragma warning restore EF1002
        }
        finally
        {
            await db.Database.ExecuteSqlRawAsync("SET FOREIGN_KEY_CHECKS = 1;");
        }

        const string sql = @"
            SELECT empId, empNo, FullName, FName, MName, LName, Email, idNumber, phoneNo, sex,
                   residence, contactAddress, religion, dateOfEmployment, dateOfBirth, status,
                   EXITDATE, KRAPIN, NSSFNo, SHANo
            FROM dbo.tblSchoolStaff
            WHERE status = 1
            ORDER BY empId;";

        var rows = await src.QueryAsync(sql, r => new SourceStaff(
            MssqlSource.GetLong(r, "empId"),
            r.GetString(r.GetOrdinal("empNo")),  // NOT NULL in source schema
            MssqlSource.GetNullableString(r, "FullName"),
            MssqlSource.GetNullableString(r, "FName"),
            MssqlSource.GetNullableString(r, "MName"),
            MssqlSource.GetNullableString(r, "LName"),
            MssqlSource.GetNullableString(r, "Email"),
            MssqlSource.GetNullableInt(r, "idNumber"),
            MssqlSource.GetNullableString(r, "phoneNo"),
            MssqlSource.GetNullableBool(r, "sex"),
            MssqlSource.GetNullableString(r, "residence"),
            MssqlSource.GetNullableString(r, "contactAddress"),
            MssqlSource.GetNullableString(r, "religion"),
            MssqlSource.GetNullableDateTime(r, "dateOfEmployment"),
            MssqlSource.GetNullableDateTime(r, "dateOfBirth"),
            MssqlSource.GetNullableBool(r, "status"),
            MssqlSource.GetNullableDateTime(r, "EXITDATE"),
            MssqlSource.GetNullableString(r, "KRAPIN"),
            MssqlSource.GetNullableString(r, "NSSFNo"),
            MssqlSource.GetNullableString(r, "SHANo")
        ), ct);

        var skipped = 0;
        var added = new List<(long oldId, StaffDetails staff)>();

        foreach (var row in rows)
        {
            // Avoid colliding with the seeded admin's UPI.
            if (string.Equals(row.EmpNo?.Trim(), "Admin", StringComparison.OrdinalIgnoreCase))
            {
                logger.LogWarning("Skipping empId={Id} - empNo='Admin' would collide with seeded admin row.", row.EmpId);
                skipped++; continue;
            }

            var fullName = PersonHelpers.ComposeFullName(row.FullName, row.FName, row.MName, row.LName);
            if (string.IsNullOrEmpty(fullName))
            {
                logger.LogWarning("Skipping empId={Id} - no name fields populated.", row.EmpId);
                skipped++; continue;
            }

            if (row.Sex is null)
                logger.LogWarning("empId={Id} ({Name}): sex is NULL, defaulting to Male.", row.EmpId, fullName);

            var religionId = await PersonHelpers.ResolveReligionAsync(db, religionMap, row.Religion, DefaultReligionId, ct);

            var staff = new StaffDetails
            {
                FullName        = PersonHelpers.Cap(fullName, 255)!,
                UPI             = PersonHelpers.Cap(row.EmpNo!.Trim(), 255)!,
                DateOfBirth     = row.DateOfBirth,
                Address         = PersonHelpers.Cap(row.Residence ?? row.ContactAddress, 255),
                PhoneNumber     = PersonHelpers.Cap(row.PhoneNo, 255),
                Email           = PersonHelpers.Cap(row.Email, 255),
                Status          = (row.Status ?? true) ? Status.Active : Status.Terminated,
                NationalityId   = DefaultNationalityId,
                ReligionId      = religionId,
                GenderId        = PersonHelpers.GenderIdFromBit(row.Sex, maleId, femaleId),
                IdNumber        = row.IdNumber?.ToString(),
                NhifNo          = row.Sha,
                NssfNo          = row.Nssf,
                KraPinNo        = row.Kraa,
                EmploymentDate  = row.DateOfEmployment,
                EndofEmploymentDate = row.ExitDate,
                CurrentlyEmployed = row.Status ?? true,
                StaffCategoryId = DefaultStaffCategoryId,
                DesignationId   = DefaultDesignationId,
                EmploymentTypeId = DefaultEmploymentTypeId,
            };
            db.StaffDetails.Add(staff);
            added.Add((row.EmpId, staff));
        }

        await db.SaveChangesAsync(ct);

        ctx.StaffMap.Clear();
        foreach (var (oldId, s) in added) ctx.StaffMap[oldId] = s.Id;

        logger.LogInformation("StaffDetails: inserted {Inserted}, skipped {Skipped} (of {Total} active source rows).", added.Count, skipped, rows.Count);
    }
}
