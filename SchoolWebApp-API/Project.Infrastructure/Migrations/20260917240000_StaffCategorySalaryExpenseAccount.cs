using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Project.Infrastructure.Data;

#nullable disable

namespace SchoolWebApp.Infrastructure.Migrations
{
    // The payroll journal charged every salary to the single SalaryExpenseAccountId
    // setting, although the chart of accounts separates 5000 Salaries - Teaching
    // Staff from 5010 Salaries - Non-Teaching Staff. Salaries are now charged per
    // staff category, each category naming its own expense account, with the setting
    // kept as the fallback for a category that has none.
    //
    // Existing categories are mapped using their ForTeaching flag rather than their
    // name, so a renamed category still maps correctly: teaching categories to 5000,
    // the rest to 5010.
    //
    // The fallback setting pointed at 5100 Utilities - Electricity, copied from a
    // misleading hint on the settings page. With every category now mapped, it is
    // cleared rather than left in place: a category without an account then stops
    // approval with a clear message instead of charging salaries to electricity.
    // Payroll and finance were not live, so no real posting was affected.
    //
    // Hand-written rather than scaffolded - see the note on UniquePayrollTypeCodes.
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20260917240000_StaffCategorySalaryExpenseAccount")]
    public partial class StaffCategorySalaryExpenseAccount : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "SalaryExpenseAccountId", table: "StaffCategories",
                type: "int", nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_StaffCategories_SalaryExpenseAccountId",
                table: "StaffCategories",
                column: "SalaryExpenseAccountId");

            migrationBuilder.AddForeignKey(
                name: "FK_StaffCategories_Accounts_SalaryExpenseAccountId",
                table: "StaffCategories",
                column: "SalaryExpenseAccountId",
                principalTable: "Accounts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.Sql(@"
                UPDATE StaffCategories sc
                    JOIN Accounts a ON a.Code = '5000'
                SET sc.SalaryExpenseAccountId = a.Id
                WHERE sc.ForTeaching = 1 AND sc.SalaryExpenseAccountId IS NULL;");

            migrationBuilder.Sql(@"
                UPDATE StaffCategories sc
                    JOIN Accounts a ON a.Code = '5010'
                SET sc.SalaryExpenseAccountId = a.Id
                WHERE sc.ForTeaching = 0 AND sc.SalaryExpenseAccountId IS NULL;");

            // Only clear the fallback if it still points at 5100 Utilities - Electricity.
            migrationBuilder.Sql(@"
                UPDATE GlobalSettings g
                    JOIN Accounts wrong ON wrong.Id = CAST(g.SettingValue AS UNSIGNED) AND wrong.Code = '5100'
                SET g.SettingValue = '', g.Modified = NOW(), g.ModifiedBy = 'migration'
                WHERE g.Module = 'Finance' AND g.SettingKey = 'SalaryExpenseAccountId';");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_StaffCategories_Accounts_SalaryExpenseAccountId",
                table: "StaffCategories");

            migrationBuilder.DropIndex(
                name: "IX_StaffCategories_SalaryExpenseAccountId",
                table: "StaffCategories");

            migrationBuilder.DropColumn(name: "SalaryExpenseAccountId", table: "StaffCategories");

            // The fallback setting is left cleared - restoring Electricity would
            // reintroduce the error.
        }
    }
}
