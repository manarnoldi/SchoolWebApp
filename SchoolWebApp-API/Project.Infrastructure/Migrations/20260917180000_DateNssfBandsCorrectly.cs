using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Project.Infrastructure.Data;

#nullable disable

namespace SchoolWebApp.Infrastructure.Migrations
{
    // ReliefsAndNssfBands seeded the NSSF tiers dated 2000-01-01, purely so that
    // every existing period would resolve to a set. That figure is not true: the
    // 9,000 / 108,000 limits took effect in February 2026, and dating them to 2000
    // claims they applied for twenty-six years.
    //
    // This dates that set correctly and adds the two sets that came before it, so
    // the history is real and a period in any of those years computes on the limits
    // that actually applied:
    //
    //   Feb 2024 (Year 2)  LEL 7,000   UEL 36,000   max 2,160/month
    //   Feb 2025 (Year 3)  LEL 8,000   UEL 72,000   max 4,320/month
    //   Feb 2026 (Year 4)  LEL 9,000   UEL 108,000  max 6,480/month
    //
    // Rates stay at 6% on the pay falling inside each tier throughout.
    //
    // A period before February 2024 now has no set in force and processing will say
    // so rather than compute on the wrong limits - which is the point of dating them.
    //
    // Hand-written rather than scaffolded - see the note on UniquePayrollTypeCodes.
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20260917180000_DateNssfBandsCorrectly")]
    public partial class DateNssfBandsCorrectly : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Correct the placeholder date on the set that is already there.
            migrationBuilder.Sql(@"
                UPDATE NssfBands
                SET EffectiveDate = '2026-02-01',
                    Name = CASE Tier WHEN 1 THEN 'NSSF Tier I' ELSE 'NSSF Tier II' END,
                    Description = 'NSSF Act Third Schedule, year 4. In force from February 2026.',
                    Modified = NOW(), ModifiedBy = 'migration'
                WHERE EffectiveDate = '2000-01-01';");

            // The two earlier sets, so a period in those years computes correctly.
            migrationBuilder.Sql(@"
                INSERT INTO NssfBands (Name, Tier, LowerLimit, UpperLimit, Rate, EffectiveDate, IsActive, Description, Created, CreatedBy)
                SELECT 'NSSF Tier I', 1, 0, 8000, 6, '2025-02-01', 1,
                       'NSSF Act Third Schedule, year 3. In force February 2025 to January 2026.', NOW(), 'migration'
                FROM DUAL WHERE NOT EXISTS (
                    SELECT 1 FROM NssfBands WHERE EffectiveDate = '2025-02-01' AND Tier = 1);");

            migrationBuilder.Sql(@"
                INSERT INTO NssfBands (Name, Tier, LowerLimit, UpperLimit, Rate, EffectiveDate, IsActive, Description, Created, CreatedBy)
                SELECT 'NSSF Tier II', 2, 8000, 72000, 6, '2025-02-01', 1,
                       'NSSF Act Third Schedule, year 3. In force February 2025 to January 2026.', NOW(), 'migration'
                FROM DUAL WHERE NOT EXISTS (
                    SELECT 1 FROM NssfBands WHERE EffectiveDate = '2025-02-01' AND Tier = 2);");

            migrationBuilder.Sql(@"
                INSERT INTO NssfBands (Name, Tier, LowerLimit, UpperLimit, Rate, EffectiveDate, IsActive, Description, Created, CreatedBy)
                SELECT 'NSSF Tier I', 1, 0, 7000, 6, '2024-02-01', 1,
                       'NSSF Act Third Schedule, year 2. In force February 2024 to January 2025.', NOW(), 'migration'
                FROM DUAL WHERE NOT EXISTS (
                    SELECT 1 FROM NssfBands WHERE EffectiveDate = '2024-02-01' AND Tier = 1);");

            migrationBuilder.Sql(@"
                INSERT INTO NssfBands (Name, Tier, LowerLimit, UpperLimit, Rate, EffectiveDate, IsActive, Description, Created, CreatedBy)
                SELECT 'NSSF Tier II', 2, 7000, 36000, 6, '2024-02-01', 1,
                       'NSSF Act Third Schedule, year 2. In force February 2024 to January 2025.', NOW(), 'migration'
                FROM DUAL WHERE NOT EXISTS (
                    SELECT 1 FROM NssfBands WHERE EffectiveDate = '2024-02-01' AND Tier = 2);");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                "DELETE FROM NssfBands WHERE EffectiveDate IN ('2024-02-01','2025-02-01');");
            migrationBuilder.Sql(
                "UPDATE NssfBands SET EffectiveDate = '2000-01-01' WHERE EffectiveDate = '2026-02-01';");
        }
    }
}
