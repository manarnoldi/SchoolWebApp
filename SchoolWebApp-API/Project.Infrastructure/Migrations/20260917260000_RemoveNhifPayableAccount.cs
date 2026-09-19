using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Project.Infrastructure.Data;

#nullable disable

namespace SchoolWebApp.Infrastructure.Migrations
{
    // Removes 2210 NHIF Payable. The National Health Insurance Fund was replaced by
    // the Social Health Insurance Fund, whose deductions post to 2120 SHIF Payable.
    // The account came from the manually run seed-finance-data.sql (now removed from
    // that script) and nothing posts to it.
    //
    // Deleted only when nothing refers to it, so an environment holding NHIF history
    // against it keeps the account.
    //
    // Hand-written rather than scaffolded - see the note on UniquePayrollTypeCodes.
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20260917260000_RemoveNhifPayableAccount")]
    public partial class RemoveNhifPayableAccount : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                DELETE a FROM Accounts a
                WHERE a.Code = '2210' AND a.Name = 'NHIF Payable'
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
            // Not recreated: NHIF no longer exists.
        }
    }
}
