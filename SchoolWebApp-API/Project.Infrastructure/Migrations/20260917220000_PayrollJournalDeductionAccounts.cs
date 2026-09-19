using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Project.Infrastructure.Data;

#nullable disable

namespace SchoolWebApp.Infrastructure.Migrations
{
    // The payroll journal credited only PAYE, NSSF, SHIF and Housing Levy against
    // net pay, so any payroll with pension, welfare or loan deductions posted a
    // journal that did not balance. The money deducted for those is owed onward -
    // to the pension scheme, the SACCO, back against a staff loan - and needs its
    // own credit.
    //
    // Adds:
    //   DeductionTypes.LiabilityAccountId - where that deduction is credited, since
    //       each is owed to a different party. Null falls back to the general one.
    //   Account 2140 Payroll Deductions Payable (liability) and the
    //       PayrollDeductionsAccountId finance setting pointing at it - the fallback.
    //   Account 1310 Staff Loans & Advances (asset) and the StaffLoansAccountId
    //       finance setting - credited as instalments are recovered from pay.
    //
    // Both accounts are only created when the code is free, and a setting is only
    // added when absent, so a school that already has them keeps its own.
    //
    // Hand-written rather than scaffolded - see the note on UniquePayrollTypeCodes.
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20260917220000_PayrollJournalDeductionAccounts")]
    public partial class PayrollJournalDeductionAccounts : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "LiabilityAccountId", table: "DeductionTypes",
                type: "int", nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_DeductionTypes_LiabilityAccountId",
                table: "DeductionTypes",
                column: "LiabilityAccountId");

            migrationBuilder.AddForeignKey(
                name: "FK_DeductionTypes_Accounts_LiabilityAccountId",
                table: "DeductionTypes",
                column: "LiabilityAccountId",
                principalTable: "Accounts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            // AccountType: 1 = Asset, 2 = Liability.
            migrationBuilder.Sql(@"
                INSERT INTO Accounts (Code, Name, AccountType, IsActive, Description, Created, CreatedBy)
                SELECT '2140', 'Payroll Deductions Payable', 2, 1,
                       'Deductions taken from staff pay and owed onward (pension, SACCO, welfare) where the deduction type has no account of its own.',
                       NOW(), 'migration'
                FROM DUAL WHERE NOT EXISTS (SELECT 1 FROM Accounts WHERE Code = '2140');");

            migrationBuilder.Sql(@"
                INSERT INTO Accounts (Code, Name, AccountType, IsActive, Description, Created, CreatedBy)
                SELECT '1310', 'Staff Loans & Advances', 1, 1,
                       'Loans and salary advances owed by staff, reduced as instalments are recovered through payroll.',
                       NOW(), 'migration'
                FROM DUAL WHERE NOT EXISTS (SELECT 1 FROM Accounts WHERE Code = '1310');");

            migrationBuilder.Sql(@"
                INSERT INTO GlobalSettings (Module, SettingKey, SettingValue, Description, Created, CreatedBy)
                SELECT 'Finance', 'PayrollDeductionsAccountId', CAST(a.Id AS CHAR),
                       'Liability account credited with payroll deductions whose type has no account of its own.',
                       NOW(), 'migration'
                FROM Accounts a
                WHERE a.Code = '2140'
                  AND NOT EXISTS (SELECT 1 FROM GlobalSettings WHERE Module = 'Finance' AND SettingKey = 'PayrollDeductionsAccountId');");

            migrationBuilder.Sql(@"
                INSERT INTO GlobalSettings (Module, SettingKey, SettingValue, Description, Created, CreatedBy)
                SELECT 'Finance', 'StaffLoansAccountId', CAST(a.Id AS CHAR),
                       'Asset account credited as staff loan and advance instalments are recovered through payroll.',
                       NOW(), 'migration'
                FROM Accounts a
                WHERE a.Code = '1310'
                  AND NOT EXISTS (SELECT 1 FROM GlobalSettings WHERE Module = 'Finance' AND SettingKey = 'StaffLoansAccountId');");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                DELETE FROM GlobalSettings WHERE Module = 'Finance'
                AND SettingKey IN ('PayrollDeductionsAccountId','StaffLoansAccountId');");

            migrationBuilder.DropForeignKey(
                name: "FK_DeductionTypes_Accounts_LiabilityAccountId",
                table: "DeductionTypes");

            migrationBuilder.DropIndex(
                name: "IX_DeductionTypes_LiabilityAccountId",
                table: "DeductionTypes");

            migrationBuilder.DropColumn(name: "LiabilityAccountId", table: "DeductionTypes");

            // The two accounts are left in place: journals may already reference them.
        }
    }
}
