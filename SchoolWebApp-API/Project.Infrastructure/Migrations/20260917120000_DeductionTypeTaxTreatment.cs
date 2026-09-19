using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Project.Infrastructure.Data;

#nullable disable

namespace SchoolWebApp.Infrastructure.Migrations
{
    // How a deduction is treated for tax was decided in code, by matching the
    // deduction's Code against "PENSION" and "INSURANCE". That meant a change in
    // the law - a new allowable deduction, a different cap - needed a code change
    // and a deployment.
    //
    // The treatment now lives on the deduction type itself, set from the Deduction
    // Types screen:
    //
    //   IsTaxDeductible            - comes off gross before PAYE is worked out
    //   TaxDeductibleCap           - monthly ceiling on that; null means uncapped
    //   QualifiesForInsuranceRelief - counts towards insurance relief
    //
    // The existing behaviour is carried over: PENSION becomes tax deductible with
    // the cap taken from the PensionReliefCap setting, and INSURANCE qualifies for
    // insurance relief. PensionReliefCap is then removed, because the cap it held
    // now lives on the type and a setting nothing reads is a trap.
    //
    // The insurance relief RATE and CAP stay as payroll settings - both are set
    // globally by law rather than per deduction.
    //
    // Hand-written rather than scaffolded - see the note on UniquePayrollTypeCodes.
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20260917120000_DeductionTypeTaxTreatment")]
    public partial class DeductionTypeTaxTreatment : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsTaxDeductible",
                table: "DeductionTypes",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<decimal>(
                name: "TaxDeductibleCap",
                table: "DeductionTypes",
                type: "decimal(65,30)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "QualifiesForInsuranceRelief",
                table: "DeductionTypes",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            // Carry the hard-coded behaviour over, taking the pension cap from the
            // setting that used to hold it so nothing changes on the next run.
            migrationBuilder.Sql(@"
                UPDATE DeductionTypes
                SET IsTaxDeductible = 1,
                    TaxDeductibleCap = COALESCE(
                        (SELECT Value FROM PayrollSettings WHERE `Key` = 'PensionReliefCap' LIMIT 1), 30000)
                WHERE Code = 'PENSION';");

            migrationBuilder.Sql(
                "UPDATE DeductionTypes SET QualifiesForInsuranceRelief = 1 WHERE Code = 'INSURANCE';");

            migrationBuilder.Sql("DELETE FROM PayrollSettings WHERE `Key` = 'PensionReliefCap';");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                INSERT INTO PayrollSettings (`Key`, Name, Value, Category, Description, EffectiveDate, IsActive, Created, CreatedBy)
                SELECT 'PensionReliefCap', 'Pension Relief Cap (Monthly)',
                       COALESCE((SELECT TaxDeductibleCap FROM DeductionTypes WHERE Code = 'PENSION' LIMIT 1), 30000),
                       'Relief', 'Max monthly pension contribution deductible from taxable income',
                       '2026-01-01', 1, NOW(), 'migration'
                FROM DUAL
                WHERE NOT EXISTS (SELECT 1 FROM PayrollSettings WHERE `Key` = 'PensionReliefCap');");

            migrationBuilder.DropColumn(name: "QualifiesForInsuranceRelief", table: "DeductionTypes");
            migrationBuilder.DropColumn(name: "TaxDeductibleCap", table: "DeductionTypes");
            migrationBuilder.DropColumn(name: "IsTaxDeductible", table: "DeductionTypes");
        }
    }
}
