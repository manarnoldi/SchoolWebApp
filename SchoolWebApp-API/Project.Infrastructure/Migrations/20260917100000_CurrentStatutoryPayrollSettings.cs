using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Project.Infrastructure.Data;

#nullable disable

namespace SchoolWebApp.Infrastructure.Migrations
{
    // Brings the payroll reference data in line with the rules the processing code
    // now applies (Tax Laws (Amendment) Act 2024, in force 27 Dec 2024):
    //
    //  * ShifMinimum - SHIF is 2.75% of gross subject to a KES 300 monthly floor.
    //    The floor was never modelled, so low earners were under-deducted.
    //  * PensionReliefCap - raised to 30,000/month. The value was seeded at 20,000
    //    and never read by the code at all until now.
    //  * INSURANCE deduction type - insurance relief is 15% of life/health/education
    //    premiums, capped monthly. It used to be computed off SHIF, which no longer
    //    qualifies now that SHIF is an allowable deduction against taxable income.
    //    Without a premium to compute from, the relief had no legitimate source.
    //
    // NSSF ceilings (9,000 / 108,000 at 6%), PAYE bands, personal relief (2,400),
    // SHIF 2.75%, AHL 1.5% and the insurance relief rate/cap (15% / 5,000) were
    // already correct for 2026 and are left alone.
    //
    // Hand-written rather than scaffolded - see the note on UniquePayrollTypeCodes.
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20260917100000_CurrentStatutoryPayrollSettings")]
    public partial class CurrentStatutoryPayrollSettings : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                INSERT INTO PayrollSettings (`Key`, Name, Value, Category, Description, EffectiveDate, IsActive, Created, CreatedBy)
                SELECT 'ShifMinimum', 'SHIF Minimum (Monthly)', 300, 'SHIF',
                       'Statutory minimum monthly SHIF contribution', '2026-01-01', 1, NOW(), 'migration'
                FROM DUAL
                WHERE NOT EXISTS (SELECT 1 FROM PayrollSettings WHERE `Key` = 'ShifMinimum');");

            migrationBuilder.Sql(@"
                UPDATE PayrollSettings
                SET Value = 30000,
                    Description = 'Max monthly pension contribution deductible from taxable income',
                    Modified = NOW(), ModifiedBy = 'migration'
                WHERE `Key` = 'PensionReliefCap' AND Value <> 30000;");

            // INSERT IGNORE is safe here: UniquePayrollTypeCodes added the unique
            // key on Code that makes it actually skip an existing row.
            migrationBuilder.Sql(@"
                INSERT IGNORE INTO DeductionTypes (Name, Code, IsStatutory, IsActive, Description, Created, CreatedBy)
                VALUES ('Insurance Premium', 'INSURANCE', 0, 1,
                        'Life, health or education premiums paid through the payroll. Attracts insurance relief.',
                        NOW(), 'migration');");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DELETE FROM PayrollSettings WHERE `Key` = 'ShifMinimum';");
            migrationBuilder.Sql("DELETE FROM DeductionTypes WHERE Code = 'INSURANCE';");
            // PensionReliefCap is left at its corrected value - restoring 20,000
            // would reintroduce a figure that is simply out of date.
        }
    }
}
