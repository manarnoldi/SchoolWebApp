using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Project.DataMigration.Mssql;
using Project.Infrastructure.Data;
using SchoolWebApp.Core.Entities.Enums;
using SchoolWebApp.Core.Entities.Settings;
using SchoolWebApp.Core.Entities.Students;

namespace Project.DataMigration.Phases;

// Migrates the cohort of tblStudDetails rows linked to a class in TargetYear via
// tblClassStudent. Year-scoped: a student who only attended classes in earlier
// years is not migrated. Each row's FK lookups:
//   - GenderId       sex bit (true=Male, false=Female; null defaults Male with warning)
//   - ReligionId     auto-created from free text; null/empty -> Christian (Id=1)
//   - LearningModeId boarder bit (true=Boarding, false=Day)
//   - NationalityId  always defaults to Kenyan (Id=1)
//
// UPI = source admNo as a string; Status = Active when source.status=1, Alumni otherwise.
public sealed class StudentPhase : IMigrationPhase
{
    public string Name => "Student";

    private const int DefaultNationalityId = 1; // Kenyan
    private const int DefaultReligionId    = 1; // Christian

    private record SourceStudent(
        long StudId, int AdmNo, string? FullName, string? FName, string? MName, string? LName,
        string? Phone, int? IdNumber, bool? Sex, bool? Boarder, string? Email, string? Address,
        string? Residence, string? Religion, DateTime? DateOfBirth, DateTime? DateOfAdmission,
        DateTime? DateOfReg, bool? Status);

    public async Task RunAsync(ApplicationDbContext db, MssqlSource src, MigrationContext ctx, ILogger logger, CancellationToken ct)
    {
        var maleId      = await db.Genders.Where(g => g.Name == "Male").Select(g => g.Id).FirstAsync(ct);
        var femaleId    = await db.Genders.Where(g => g.Name == "Female").Select(g => g.Id).FirstAsync(ct);
        var dayModeId      = await db.LearningModes.Where(m => m.Name == "Day").Select(m => m.Id).FirstAsync(ct);
        var boardingModeId = await db.LearningModes.Where(m => m.Name == "Boarding").Select(m => m.Id).FirstAsync(ct);
        var religionMap = await PersonHelpers.LoadReligionMapAsync(db, ct);

        // TPH inheritance: Student rows live in the single `Person` table identified
        // by Discriminator='Student'. We can't TRUNCATE that table (would also wipe
        // parents and staff), so DELETE only the Student rows instead.
        await db.Database.ExecuteSqlRawAsync("SET FOREIGN_KEY_CHECKS = 0;");
        try
        {
            await db.Database.ExecuteSqlRawAsync(
                "DELETE FROM `Person` WHERE `Discriminator` = 'Student';");
        }
        finally
        {
            await db.Database.ExecuteSqlRawAsync("SET FOREIGN_KEY_CHECKS = 1;");
        }

        var sql = $@"
            SELECT s.studId, s.admNo, s.FullName, s.FName, s.MName, s.LName, s.phone, s.idNumber,
                   s.sex, s.boarder, s.email, s.address, s.residence, s.religion,
                   s.dateOfBirth, s.dateOfAdmission, s.dateOfReg, s.status
            FROM dbo.tblStudDetails s
            WHERE s.studId IN (
                SELECT DISTINCT cs.studId
                FROM dbo.tblClassStudent cs
                JOIN dbo.tblClasses     c ON c.classId = cs.classId
                WHERE c.[year] = {ctx.TargetYear}
            )
            ORDER BY s.studId;";

        logger.LogInformation("Student: filtering source by membership in year = {Year}.", ctx.TargetYear);

        var rows = await src.QueryAsync(sql, r => new SourceStudent(
            MssqlSource.GetLong(r, "studId"),
            r.GetInt32(r.GetOrdinal("admNo")),
            MssqlSource.GetNullableString(r, "FullName"),
            MssqlSource.GetNullableString(r, "FName"),
            MssqlSource.GetNullableString(r, "MName"),
            MssqlSource.GetNullableString(r, "LName"),
            MssqlSource.GetNullableString(r, "phone"),
            MssqlSource.GetNullableInt(r, "idNumber"),
            MssqlSource.GetNullableBool(r, "sex"),
            MssqlSource.GetNullableBool(r, "boarder"),
            MssqlSource.GetNullableString(r, "email"),
            MssqlSource.GetNullableString(r, "address"),
            MssqlSource.GetNullableString(r, "residence"),
            MssqlSource.GetNullableString(r, "religion"),
            MssqlSource.GetNullableDateTime(r, "dateOfBirth"),
            MssqlSource.GetNullableDateTime(r, "dateOfAdmission"),
            MssqlSource.GetNullableDateTime(r, "dateOfReg"),
            MssqlSource.GetNullableBool(r, "status")
        ), ct);

        var skipped = 0;
        var added = new List<(long oldId, Student stu)>();
        var seenUpi = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        foreach (var row in rows)
        {
            var fullName = PersonHelpers.ComposeFullName(row.FullName, row.FName, row.MName, row.LName);
            if (string.IsNullOrEmpty(fullName))
            {
                logger.LogWarning("Skipping studId={Id} - no name fields populated.", row.StudId);
                skipped++; continue;
            }

            var upi = row.AdmNo.ToString();
            if (!seenUpi.Add(upi))
            {
                logger.LogWarning("Skipping studId={Id} ({Name}) - duplicate admNo={Adm} in source cohort.", row.StudId, fullName, upi);
                skipped++; continue;
            }

            if (row.Sex is null)
                logger.LogWarning("studId={Id} ({Name}): sex is NULL, defaulting to Male.", row.StudId, fullName);

            var religionId = await PersonHelpers.ResolveReligionAsync(db, religionMap, row.Religion, DefaultReligionId, ct);

            var stu = new Student
            {
                FullName        = PersonHelpers.Cap(fullName, 255)!,
                UPI             = PersonHelpers.Cap(upi, 255)!,
                DateOfBirth     = row.DateOfBirth,
                Address         = PersonHelpers.Cap(row.Residence ?? row.Address, 255),
                PhoneNumber     = PersonHelpers.Cap(row.Phone, 255),
                Email           = PersonHelpers.Cap(row.Email, 255),
                Status          = (row.Status ?? true) ? Status.Active : Status.Alumni,
                NationalityId   = DefaultNationalityId,
                ReligionId      = religionId,
                GenderId        = PersonHelpers.GenderIdFromBit(row.Sex, maleId, femaleId),
                LearningModeId  = (row.Boarder ?? false) ? boardingModeId : dayModeId,
                AdmissionDate   = row.DateOfAdmission,
                ApplicationDate = row.DateOfReg,
                HealthConcerns  = null,
            };
            db.Students.Add(stu);
            added.Add((row.StudId, stu));
        }

        await db.SaveChangesAsync(ct);

        ctx.StudentMap.Clear();
        foreach (var (oldId, s) in added) ctx.StudentMap[oldId] = s.Id;

        logger.LogInformation("Student: inserted {Inserted}, skipped {Skipped} (of {Total} cohort source rows).", added.Count, skipped, rows.Count);
    }
}
