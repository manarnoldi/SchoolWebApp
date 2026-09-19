using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Project.Infrastructure.Data;

#nullable disable

namespace SchoolWebApp.Infrastructure.Migrations
{
    // Approving a payroll posted its journal straight to the ledger, with no way to
    // have finance review it first. Adds the PayrollJournalMode finance setting:
    //
    //   draft - the journal is created with every figure filled in and left as a
    //           draft under Journal Entries for finance to review, adjust and approve
    //   auto  - posted to the ledger on approval (the previous behaviour)
    //   off   - no journal created; finance posts one by hand
    //
    // Defaults to draft, so nothing reaches the ledger unreviewed.
    //
    // Also corrects PayeAccountId, which pointed at account 2100 Accrued Salaries
    // rather than 2200 PAYE Payable. The settings page hint read "e.g. 2100 PAYE
    // Payable", but 2100 is Accrued Salaries in this chart of accounts, and the
    // setting had followed it. Payroll and finance were not live, so no real posting
    // was affected. The salary expense account had the same kind of error (5100 is
    // Utilities - Electricity) but is left for the school to choose, since teaching
    // and non-teaching salaries have separate accounts.
    //
    // Hand-written rather than scaffolded - see the note on UniquePayrollTypeCodes.
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20260917230000_PayrollJournalModeSetting")]
    public partial class PayrollJournalModeSetting : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                INSERT INTO GlobalSettings (Module, SettingKey, SettingValue, Description, Created, CreatedBy)
                SELECT 'Finance', 'PayrollJournalMode', 'draft',
                       'What approving a payroll does in the ledger: draft, auto or off.',
                       NOW(), 'migration'
                FROM DUAL WHERE NOT EXISTS (
                    SELECT 1 FROM GlobalSettings WHERE Module = 'Finance' AND SettingKey = 'PayrollJournalMode');");

            // Only move the setting if it still points at Accrued Salaries and a PAYE
            // Payable account exists to point at instead.
            migrationBuilder.Sql(@"
                UPDATE GlobalSettings g
                    JOIN Accounts wrong ON wrong.Id = CAST(g.SettingValue AS UNSIGNED) AND wrong.Code = '2100'
                    JOIN Accounts paye ON paye.Code = '2200'
                SET g.SettingValue = CAST(paye.Id AS CHAR), g.Modified = NOW(), g.ModifiedBy = 'migration'
                WHERE g.Module = 'Finance' AND g.SettingKey = 'PayeAccountId';");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                "DELETE FROM GlobalSettings WHERE Module = 'Finance' AND SettingKey = 'PayrollJournalMode';");
            // PayeAccountId is left on PAYE Payable - pointing it back at Accrued
            // Salaries would reintroduce the error.
        }
    }
}
