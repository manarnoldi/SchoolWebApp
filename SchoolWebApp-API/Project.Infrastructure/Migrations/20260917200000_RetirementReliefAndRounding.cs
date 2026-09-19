using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Project.Infrastructure.Data;

#nullable disable

namespace SchoolWebApp.Infrastructure.Migrations
{
    // Reconciled against a payslip from a reference payroll for the same employee
    // (gross 482,154.33), which paid 1,943.60 less net than this system. The gap was:
    //
    // 1. Retirement relief. NSSF was deducted from taxable income in full, and the
    //    pension was then capped at 30,000 on its own - 36,480 allowed. The reference
    //    pools them ("PEN. RELIEF (INCL. NSSF)") and allows the lowest of the actual
    //    total, 30% of basic pay, and 30,000 - so 30,000. That 6,480 at the 30% band
    //    was 1,944 of PAYE not deducted.
    //
    //    Added: DeductionTypes.IsRetirementContribution, and the settings
    //    RetirementReliefPercent (30) and RetirementReliefCap (30,000). PENSION moves
    //    from the per-type tax-deductible path onto the pool, taking its cap with it.
    //
    // 2. SHIF in whole shillings (13,259.00, not 13,259.24)   -> ShifRoundingUnit = 1
    // 3. Net pay in whole shillings (294,522.00)             -> NetPayRoundingUnit = 1
    //
    // Also, earning types flagged non-taxable (commuter, airtime) were being taxed:
    // IsTaxable existed but nothing read it. Processing now leaves them out.
    //
    // The payslip gains the working behind taxable income - TaxablePay,
    // RetirementRelief, OtherTaxDeductible - and the RoundingAdjustment, so it can
    // show how PAYE and net pay were reached and still add up.
    //
    // Hand-written rather than scaffolded - see the note on UniquePayrollTypeCodes.
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20260917200000_RetirementReliefAndRounding")]
    public partial class RetirementReliefAndRounding : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsRetirementContribution", table: "DeductionTypes",
                type: "tinyint(1)", nullable: false, defaultValue: false);

            foreach (var column in new[] { "TaxablePay", "RetirementRelief", "OtherTaxDeductible", "RoundingAdjustment" })
            {
                migrationBuilder.AddColumn<decimal>(
                    name: column, table: "Payslips",
                    type: "decimal(65,30)", nullable: false, defaultValue: 0m);
            }

            // The retirement cap is the one PENSION carried as its own tax-deductible
            // cap, so read it before PENSION is moved off that path.
            migrationBuilder.Sql(@"
                INSERT INTO PayrollSettings (`Key`, Name, Value, Category, Description, EffectiveDate, IsActive, Created, CreatedBy)
                SELECT 'RetirementReliefCap', 'Retirement Relief Cap (Monthly)',
                       COALESCE((SELECT TaxDeductibleCap FROM DeductionTypes WHERE Code = 'PENSION' AND TaxDeductibleCap IS NOT NULL LIMIT 1), 30000),
                       'Relief', 'Most that NSSF plus retirement-scheme contributions together may take off taxable income each month',
                       '2026-01-01', 1, NOW(), 'migration'
                FROM DUAL WHERE NOT EXISTS (SELECT 1 FROM PayrollSettings WHERE `Key` = 'RetirementReliefCap');");

            migrationBuilder.Sql(@"
                INSERT INTO PayrollSettings (`Key`, Name, Value, Category, Description, EffectiveDate, IsActive, Created, CreatedBy)
                SELECT 'RetirementReliefPercent', 'Retirement Relief Limit (% of Basic Pay)', 30,
                       'Relief', 'NSSF plus retirement-scheme contributions may not take off more than this percentage of basic pay',
                       '2026-01-01', 1, NOW(), 'migration'
                FROM DUAL WHERE NOT EXISTS (SELECT 1 FROM PayrollSettings WHERE `Key` = 'RetirementReliefPercent');");

            migrationBuilder.Sql(@"
                INSERT INTO PayrollSettings (`Key`, Name, Value, Category, Description, EffectiveDate, IsActive, Created, CreatedBy)
                SELECT 'ShifRoundingUnit', 'SHIF Rounding Unit', 1,
                       'SHIF', 'SHIF is rounded to the nearest multiple of this. 1 = whole shillings, 0 = keep cents',
                       '2026-01-01', 1, NOW(), 'migration'
                FROM DUAL WHERE NOT EXISTS (SELECT 1 FROM PayrollSettings WHERE `Key` = 'ShifRoundingUnit');");

            migrationBuilder.Sql(@"
                INSERT INTO PayrollSettings (`Key`, Name, Value, Category, Description, EffectiveDate, IsActive, Created, CreatedBy)
                SELECT 'NetPayRoundingUnit', 'Net Pay Rounding Unit', 1,
                       'Net Pay', 'Net pay is rounded to the nearest multiple of this. 1 = whole shillings, 0 = keep cents',
                       '2026-01-01', 1, NOW(), 'migration'
                FROM DUAL WHERE NOT EXISTS (SELECT 1 FROM PayrollSettings WHERE `Key` = 'NetPayRoundingUnit');");

            // PENSION now belongs to the retirement pool rather than being relieved on
            // its own, so it leaves the per-type tax-deductible path entirely.
            migrationBuilder.Sql(@"
                UPDATE DeductionTypes
                SET IsRetirementContribution = 1, IsTaxDeductible = 0, TaxDeductibleCap = NULL
                WHERE Code = 'PENSION';");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                UPDATE DeductionTypes
                SET IsTaxDeductible = 1,
                    TaxDeductibleCap = COALESCE((SELECT Value FROM PayrollSettings WHERE `Key` = 'RetirementReliefCap' LIMIT 1), 30000)
                WHERE Code = 'PENSION';");

            migrationBuilder.Sql(@"
                DELETE FROM PayrollSettings WHERE `Key` IN
                ('RetirementReliefCap','RetirementReliefPercent','ShifRoundingUnit','NetPayRoundingUnit');");

            foreach (var column in new[] { "TaxablePay", "RetirementRelief", "OtherTaxDeductible", "RoundingAdjustment" })
                migrationBuilder.DropColumn(name: column, table: "Payslips");

            migrationBuilder.DropColumn(name: "IsRetirementContribution", table: "DeductionTypes");
        }
    }
}
