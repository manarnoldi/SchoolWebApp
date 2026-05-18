using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SchoolWebApp.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateAccountCodesToNumeric : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Point GlobalSettings to the EXISTING numeric-coded accounts
            // Debtors: use existing 1200 "Accounts Receivable (Fees)"
            migrationBuilder.Sql(@"
                INSERT IGNORE INTO GlobalSettings (Module, SettingKey, SettingValue, Description, Created)
                SELECT 'Finance', 'DebtorsAccountId', CAST(Id AS CHAR), 'Student Debtors account', NOW()
                FROM Accounts WHERE Code = '1200' LIMIT 1;
            ");
            // Cash: use existing 1100 "Bank - Main Account"
            migrationBuilder.Sql(@"
                INSERT IGNORE INTO GlobalSettings (Module, SettingKey, SettingValue, Description, Created)
                SELECT 'Finance', 'CashAccountId', CAST(Id AS CHAR), 'Default Cash/Bank account', NOW()
                FROM Accounts WHERE Code = '1100' LIMIT 1;
            ");
            // Salary Expense: use existing 5100
            migrationBuilder.Sql(@"
                INSERT IGNORE INTO GlobalSettings (Module, SettingKey, SettingValue, Description, Created)
                SELECT 'Finance', 'SalaryExpenseAccountId', CAST(Id AS CHAR), 'Salary Expense account', NOW()
                FROM Accounts WHERE Code = '5100' LIMIT 1;
            ");
            // PAYE: use existing 2100
            migrationBuilder.Sql(@"
                INSERT IGNORE INTO GlobalSettings (Module, SettingKey, SettingValue, Description, Created)
                SELECT 'Finance', 'PayeAccountId', CAST(Id AS CHAR), 'PAYE Payable account', NOW()
                FROM Accounts WHERE Code = '2100' LIMIT 1;
            ");
            // NSSF: create new 2110 if it doesn't exist, then point to it
            migrationBuilder.Sql(@"
                INSERT IGNORE INTO Accounts (Code, Name, AccountType, IsActive, Description, Created)
                VALUES ('2110', 'NSSF Payable', 2, 1, 'NSSF contributions payable', NOW());
            ");
            migrationBuilder.Sql(@"
                INSERT IGNORE INTO GlobalSettings (Module, SettingKey, SettingValue, Description, Created)
                SELECT 'Finance', 'NssfAccountId', CAST(Id AS CHAR), 'NSSF Payable account', NOW()
                FROM Accounts WHERE Code = '2110' LIMIT 1;
            ");
            // SHIF: create new 2120
            migrationBuilder.Sql(@"
                INSERT IGNORE INTO Accounts (Code, Name, AccountType, IsActive, Description, Created)
                VALUES ('2120', 'SHIF Payable', 2, 1, 'SHIF contributions payable', NOW());
            ");
            migrationBuilder.Sql(@"
                INSERT IGNORE INTO GlobalSettings (Module, SettingKey, SettingValue, Description, Created)
                SELECT 'Finance', 'ShifAccountId', CAST(Id AS CHAR), 'SHIF Payable account', NOW()
                FROM Accounts WHERE Code = '2120' LIMIT 1;
            ");
            // AHL: create new 2130
            migrationBuilder.Sql(@"
                INSERT IGNORE INTO Accounts (Code, Name, AccountType, IsActive, Description, Created)
                VALUES ('2130', 'Housing Levy Payable', 2, 1, 'Affordable Housing Levy payable', NOW());
            ");
            migrationBuilder.Sql(@"
                INSERT IGNORE INTO GlobalSettings (Module, SettingKey, SettingValue, Description, Created)
                SELECT 'Finance', 'AhlAccountId', CAST(Id AS CHAR), 'Housing Levy Payable account', NOW()
                FROM Accounts WHERE Code = '2130' LIMIT 1;
            ");

            // Delete the text-coded duplicate accounts (only if no journal lines reference them)
            migrationBuilder.Sql(@"
                DELETE FROM Accounts WHERE Code IN ('DEBTORS','CASH','BANK','PAYE-LIA','NSSF-LIA','SHIF-LIA','AHL-LIA','SALEXP')
                AND Id NOT IN (SELECT DISTINCT AccountId FROM JournalLines);
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DELETE FROM GlobalSettings WHERE Module = 'Finance' AND SettingKey IN ('DebtorsAccountId','CashAccountId','SalaryExpenseAccountId','PayeAccountId','NssfAccountId','ShifAccountId','AhlAccountId');");
        }
    }
}
