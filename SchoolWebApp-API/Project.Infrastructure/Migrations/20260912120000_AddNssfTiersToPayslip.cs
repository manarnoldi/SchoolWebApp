using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Project.Infrastructure.Data;

#nullable disable

namespace SchoolWebApp.Infrastructure.Migrations
{
    // The printed payslip shows NSSF Tier I and Tier II as separate lines, but
    // processing only stored their sum in NssfEmployee. Deriving the split at
    // print time would re-read the current ceilings and rates, so a payslip
    // issued before a rate change would silently reprint with different tier
    // figures. Storing them keeps an issued payslip fixed.
    //
    // Also seeds the employer KRA PIN that the payslip header carries. It lives
    // in GlobalSettings (module Payroll) rather than PayrollSettings, whose Value
    // column is a decimal. Seeded empty so it appears on the Global Settings page
    // for the school to fill in.
    //
    // Hand-written rather than scaffolded - see the note on UniquePayrollTypeCodes.
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20260912120000_AddNssfTiersToPayslip")]
    public partial class AddNssfTiersToPayslip : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "NssfTier1",
                table: "Payslips",
                type: "decimal(65,30)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "NssfTier2",
                table: "Payslips",
                type: "decimal(65,30)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.Sql(@"
                INSERT INTO GlobalSettings (Module, SettingKey, SettingValue, Description, Created, CreatedBy)
                SELECT 'Payroll', 'EmployerKraPin', '', 'The school''s KRA PIN, printed in the payslip header.', NOW(), 'migration'
                FROM DUAL
                WHERE NOT EXISTS (
                    SELECT 1 FROM GlobalSettings WHERE Module = 'Payroll' AND SettingKey = 'EmployerKraPin');");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                "DELETE FROM GlobalSettings WHERE Module = 'Payroll' AND SettingKey = 'EmployerKraPin';");

            migrationBuilder.DropColumn(
                name: "NssfTier2",
                table: "Payslips");

            migrationBuilder.DropColumn(
                name: "NssfTier1",
                table: "Payslips");
        }
    }
}
