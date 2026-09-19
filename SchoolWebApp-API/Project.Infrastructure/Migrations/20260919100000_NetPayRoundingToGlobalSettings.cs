using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Project.Infrastructure.Data;

#nullable disable

namespace SchoolWebApp.Infrastructure.Migrations
{
    // Moves net pay rounding from the statutory rates table (PayrollSettings) to
    // Global Settings > Payroll. How far net pay is rounded is the school's own
    // policy, not a Government rate, so it belongs with the other policy settings.
    //
    // NetPayRoundingUnit   0 = keep cents, 1 = whole shillings, or 5/10/50/100
    // NetPayRoundingMethod nearest | down | up
    //
    // The unit carries over whatever the school had set (1 unless changed); the
    // method starts as "nearest", which is how net pay was rounded until now.
    // SHIF rounding stays in PayrollSettings - that one is set by the SHA rules.
    //
    // Data only, so the model snapshot is unchanged. Hand-written rather than
    // scaffolded - see the note on UniquePayrollTypeCodes.
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20260919100000_NetPayRoundingToGlobalSettings")]
    public partial class NetPayRoundingToGlobalSettings : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                INSERT INTO GlobalSettings (Module, SettingKey, SettingValue, Description, Created, CreatedBy)
                SELECT 'Payroll', 'NetPayRoundingUnit',
                       COALESCE((SELECT CAST(CAST(Value AS SIGNED) AS CHAR) FROM PayrollSettings
                                 WHERE `Key` = 'NetPayRoundingUnit' LIMIT 1), '1'),
                       'Net pay is rounded to a multiple of this. 0 keeps the cents.',
                       NOW(), 'migration'
                FROM DUAL
                WHERE NOT EXISTS (SELECT 1 FROM GlobalSettings WHERE Module = 'Payroll' AND SettingKey = 'NetPayRoundingUnit');");

            migrationBuilder.Sql(@"
                INSERT INTO GlobalSettings (Module, SettingKey, SettingValue, Description, Created, CreatedBy)
                SELECT 'Payroll', 'NetPayRoundingMethod', 'nearest',
                       'Whether net pay is rounded to the nearest unit, always down, or always up.',
                       NOW(), 'migration'
                FROM DUAL
                WHERE NOT EXISTS (SELECT 1 FROM GlobalSettings WHERE Module = 'Payroll' AND SettingKey = 'NetPayRoundingMethod');");

            migrationBuilder.Sql("DELETE FROM PayrollSettings WHERE `Key` = 'NetPayRoundingUnit';");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                INSERT INTO PayrollSettings (`Key`, Name, Value, Category, Description, EffectiveDate, IsActive, Created, CreatedBy)
                SELECT 'NetPayRoundingUnit', 'Net Pay Rounding Unit',
                       COALESCE((SELECT CAST(SettingValue AS DECIMAL(18,4)) FROM GlobalSettings
                                 WHERE Module = 'Payroll' AND SettingKey = 'NetPayRoundingUnit' LIMIT 1), 1),
                       'Net Pay', 'Net pay is rounded to the nearest multiple of this. 1 = whole shillings, 0 = keep cents',
                       '2026-01-01', 1, NOW(), 'migration'
                FROM DUAL WHERE NOT EXISTS (SELECT 1 FROM PayrollSettings WHERE `Key` = 'NetPayRoundingUnit');");

            migrationBuilder.Sql(@"
                DELETE FROM GlobalSettings WHERE Module = 'Payroll'
                AND SettingKey IN ('NetPayRoundingUnit','NetPayRoundingMethod');");
        }
    }
}
