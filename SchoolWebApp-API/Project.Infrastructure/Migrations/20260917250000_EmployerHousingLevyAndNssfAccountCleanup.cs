using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Project.Infrastructure.Data;

#nullable disable

namespace SchoolWebApp.Infrastructure.Migrations
{
    // 1. Employer's Affordable Housing Levy. The employer matches the employee's
    //    1.5% of gross pay. It is a cost of employment, not a deduction from the
    //    employee, so it never appears on the payslip - like employer NSSF, it is
    //    stored on the payslip record only so the payroll journal can charge it to
    //    salary expense and credit Housing Levy Payable. It was not calculated at all.
    //
    //    Adds Payslips.AhlEmployer and the AhlEmployerRate setting (1.5), kept apart
    //    from the employee rate so either can change on its own.
    //
    // 2. Duplicate NSSF Payable account. The chart of accounts had 2110 NSSF Payable
    //    (created by UpdateAccountCodesToNumeric, and the account payroll posts to)
    //    and 2220 NSSF Payable (from the manually run seed-finance-data.sql, now
    //    removed from that script). 2220 is deleted - but only when nothing refers to
    //    it, so an environment that does use it keeps it.
    //
    // Hand-written rather than scaffolded - see the note on UniquePayrollTypeCodes.
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20260917250000_EmployerHousingLevyAndNssfAccountCleanup")]
    public partial class EmployerHousingLevyAndNssfAccountCleanup : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "AhlEmployer", table: "Payslips",
                type: "decimal(65,30)", nullable: false, defaultValue: 0m);

            migrationBuilder.Sql(@"
                INSERT INTO PayrollSettings (`Key`, Name, Value, Category, Description, EffectiveDate, IsActive, Created, CreatedBy)
                SELECT 'AhlEmployerRate', 'AHL Employer Rate (%)', 1.5, 'AHL',
                       'Employer''s matching Housing Levy, as a percentage of gross pay. An employer cost - posted to the journal, never shown on the payslip.',
                       '2026-01-01', 1, NOW(), 'migration'
                FROM DUAL WHERE NOT EXISTS (SELECT 1 FROM PayrollSettings WHERE `Key` = 'AhlEmployerRate');");

            migrationBuilder.Sql(@"
                DELETE a FROM Accounts a
                WHERE a.Code = '2220' AND a.Name = 'NSSF Payable'
                  AND NOT EXISTS (SELECT 1 FROM (SELECT ParentAccountId FROM Accounts) x WHERE x.ParentAccountId = a.Id)
                  AND NOT EXISTS (SELECT 1 FROM JournalLines WHERE AccountId = a.Id)
                  AND NOT EXISTS (SELECT 1 FROM BudgetLines WHERE AccountId = a.Id)
                  AND NOT EXISTS (SELECT 1 FROM BudgetAmendmentLines WHERE AccountId = a.Id)
                  AND NOT EXISTS (SELECT 1 FROM DeductionTypes WHERE LiabilityAccountId = a.Id)
                  AND NOT EXISTS (SELECT 1 FROM ExpenseCategories WHERE ExpenseAccountId = a.Id)
                  AND NOT EXISTS (SELECT 1 FROM Expenses WHERE PaidFromAccountId = a.Id)
                  AND NOT EXISTS (SELECT 1 FROM FeeCategories WHERE IncomeAccountId = a.Id)
                  AND NOT EXISTS (SELECT 1 FROM Payments WHERE BankAccountId = a.Id)
                  AND NOT EXISTS (SELECT 1 FROM SponsorPayments WHERE BankAccountId = a.Id)
                  AND NOT EXISTS (SELECT 1 FROM Sponsors WHERE ReceivableAccountId = a.Id)
                  AND NOT EXISTS (SELECT 1 FROM StaffCategories WHERE SalaryExpenseAccountId = a.Id)
                  AND NOT EXISTS (SELECT 1 FROM GlobalSettings WHERE Module = 'Finance' AND SettingValue = CAST(a.Id AS CHAR));");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DELETE FROM PayrollSettings WHERE `Key` = 'AhlEmployerRate';");
            migrationBuilder.DropColumn(name: "AhlEmployer", table: "Payslips");
            // The duplicate 2220 NSSF Payable account is not recreated.
        }
    }
}
