using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SchoolWebApp.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class SeedPayrollAndAutoPostAccounts : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // ============================================================
            // Chart of Accounts — Auto-posting accounts
            // ============================================================
            migrationBuilder.Sql(@"
                INSERT IGNORE INTO Accounts (Code, Name, AccountType, IsActive, Description, Created)
                VALUES
                ('DEBTORS', 'Student Debtors', 1, 1, 'Accounts Receivable — student fees', NOW()),
                ('CASH', 'Cash at Hand', 1, 1, 'Cash account for receipts and payments', NOW()),
                ('BANK', 'Bank Account', 1, 1, 'Main school bank account', NOW()),
                ('SALEXP', 'Salary Expense', 5, 1, 'Staff salaries and wages', NOW()),
                ('PAYE-LIA', 'PAYE Payable', 2, 1, 'PAYE tax liability to KRA', NOW()),
                ('NSSF-LIA', 'NSSF Payable', 2, 1, 'NSSF contributions payable', NOW()),
                ('SHIF-LIA', 'SHIF Payable', 2, 1, 'SHIF contributions payable', NOW()),
                ('AHL-LIA', 'Housing Levy Payable', 2, 1, 'Affordable Housing Levy payable', NOW());
            ");

            // ============================================================
            // Payroll — Earning Types
            // ============================================================
            migrationBuilder.Sql(@"
                INSERT IGNORE INTO EarningTypes (Name, Code, IsTaxable, IsActive, Created)
                VALUES
                ('Basic Salary', 'BASIC', 1, 1, NOW()),
                ('House Allowance', 'HSEALL', 1, 1, NOW()),
                ('Transport Allowance', 'TRNALL', 1, 1, NOW()),
                ('Overtime', 'OVRTME', 1, 1, NOW()),
                ('Leave Allowance', 'LVALL', 1, 1, NOW()),
                ('Hardship Allowance', 'HRDALL', 1, 1, NOW()),
                ('Acting Allowance', 'ACTALL', 1, 1, NOW()),
                ('Responsibility Allowance', 'RSPALL', 1, 1, NOW()),
                ('Commuter Allowance', 'COMALL', 0, 1, NOW()),
                ('Airtime Allowance', 'AIRALL', 0, 1, NOW());
            ");

            // ============================================================
            // Payroll — Deduction Types
            // ============================================================
            migrationBuilder.Sql(@"
                INSERT IGNORE INTO DeductionTypes (Name, Code, IsStatutory, IsActive, Created)
                VALUES
                ('PAYE', 'PAYE', 1, 1, NOW()),
                ('NSSF Employee', 'NSSF', 1, 1, NOW()),
                ('SHIF', 'SHIF', 1, 1, NOW()),
                ('Affordable Housing Levy', 'AHL', 1, 1, NOW()),
                ('NSSF Employer', 'NSSFER', 1, 1, NOW()),
                ('Loan Deduction', 'LOAN', 0, 1, NOW()),
                ('Salary Advance', 'ADVANCE', 0, 1, NOW()),
                ('SACCO', 'SACCO', 0, 1, NOW()),
                ('Union Dues', 'UNION', 0, 1, NOW()),
                ('Welfare', 'WELFARE', 0, 1, NOW()),
                ('HELB Loan', 'HELB', 0, 1, NOW()),
                ('Pension - Voluntary', 'PENSION', 0, 1, NOW());
            ");

            // ============================================================
            // Payroll — PAYE Tax Bands (Kenya 2025 Monthly)
            // ============================================================
            migrationBuilder.Sql(@"
                INSERT IGNORE INTO TaxBands (Description, LowerLimit, UpperLimit, Rate, EffectiveDate, IsActive, Created)
                VALUES
                ('Up to KES 24,000', 0, 24000, 10.00, '2025-01-01', 1, NOW()),
                ('KES 24,001 - 32,333', 24000, 32333, 25.00, '2025-01-01', 1, NOW()),
                ('KES 32,334 - 500,000', 32333, 500000, 30.00, '2025-01-01', 1, NOW()),
                ('KES 500,001 - 800,000', 500000, 800000, 32.50, '2025-01-01', 1, NOW()),
                ('Above KES 800,000', 800000, 99999999, 35.00, '2025-01-01', 1, NOW());
            ");

            // ============================================================
            // Payroll — Statutory Settings (Kenya 2025)
            // ============================================================
            migrationBuilder.Sql(@"
                INSERT IGNORE INTO PayrollSettings (`Key`, Name, Value, Category, Description, EffectiveDate, IsActive, Created)
                VALUES
                ('NssfTier1Ceiling', 'NSSF Tier I Ceiling', 7000, 'NSSF', 'Maximum pensionable earnings for Tier I', '2025-01-01', 1, NOW()),
                ('NssfTier1Rate', 'NSSF Tier I Rate (%)', 6, 'NSSF', 'Employee contribution rate for Tier I', '2025-01-01', 1, NOW()),
                ('NssfTier2Ceiling', 'NSSF Tier II Ceiling', 36000, 'NSSF', 'Maximum pensionable earnings for Tier II', '2025-01-01', 1, NOW()),
                ('NssfTier2Rate', 'NSSF Tier II Rate (%)', 6, 'NSSF', 'Employee contribution rate for Tier II', '2025-01-01', 1, NOW()),
                ('ShifRate', 'SHIF Rate (%)', 2.75, 'SHIF', 'Percentage of gross pay', '2025-01-01', 1, NOW()),
                ('AhlRate', 'AHL Rate (%)', 1.5, 'AHL', 'Percentage of gross pay', '2025-01-01', 1, NOW()),
                ('PersonalRelief', 'Personal Relief (Monthly)', 2400, 'Relief', 'Monthly personal tax relief', '2025-01-01', 1, NOW()),
                ('InsuranceReliefRate', 'Insurance Relief Rate (%)', 15, 'Relief', 'Percentage of SHIF for insurance relief', '2025-01-01', 1, NOW()),
                ('InsuranceReliefCap', 'Insurance Relief Cap (Monthly)', 5000, 'Relief', 'Maximum monthly insurance relief', '2025-01-01', 1, NOW()),
                ('PensionReliefCap', 'Pension Relief Cap (Monthly)', 20000, 'Relief', 'Max monthly pension contribution deductible from taxable income', '2025-01-01', 1, NOW());
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DELETE FROM PayrollSettings WHERE `Key` IN ('NssfTier1Ceiling','NssfTier1Rate','NssfTier2Ceiling','NssfTier2Rate','ShifRate','AhlRate','PersonalRelief','InsuranceReliefRate','InsuranceReliefCap','PensionReliefCap');");
            migrationBuilder.Sql("DELETE FROM TaxBands WHERE EffectiveDate = '2025-01-01';");
            migrationBuilder.Sql("DELETE FROM DeductionTypes WHERE Code IN ('PAYE','NSSF','SHIF','AHL','NSSFER','LOAN','ADVANCE','SACCO','UNION','WELFARE','HELB','PENSION');");
            migrationBuilder.Sql("DELETE FROM EarningTypes WHERE Code IN ('BASIC','HSEALL','TRNALL','OVRTME','LVALL','HRDALL','ACTALL','RSPALL','COMALL','AIRALL');");
            migrationBuilder.Sql("DELETE FROM Accounts WHERE Code IN ('DEBTORS','CASH','BANK','SALEXP','PAYE-LIA','NSSF-LIA','SHIF-LIA','AHL-LIA');");
        }
    }
}
