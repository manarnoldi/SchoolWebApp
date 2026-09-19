using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Project.Infrastructure.Data;

#nullable disable

namespace SchoolWebApp.Infrastructure.Migrations
{
    // Adopts two ideas from the legacy SchoolSoft payroll (tblPayrollDeductions /
    // tblPayrollAllowances), which carried a calculation method and an
    // "AllEmployees" flag on the type itself:
    //
    //   CalculationMethod - 0 fixed amount, 1 percentage. A deduction percentage is
    //                       of gross pay; an earning percentage is of basic salary,
    //                       since an earning off gross would define itself.
    //   DefaultValue      - the amount or the percentage.
    //   AppliesToAll      - applied to every employee without assigning it to each
    //                       one. An employee's own line still wins, which is how one
    //                       person gets a different figure.
    //   IsSystemComputed  - payroll works this one out from the statutory rules, so
    //                       it must never also be applied as a configured line.
    //
    // Without this, something like NITA or staff welfare had to be typed against
    // every employee individually, and again for each new hire.
    //
    // Nothing changes on the next run: AppliesToAll defaults to false everywhere,
    // so payroll behaves exactly as before until a type is ticked.
    //
    // Hand-written rather than scaffolded - see the note on UniquePayrollTypeCodes.
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20260917140000_EarningDeductionCalculationMethod")]
    public partial class EarningDeductionCalculationMethod : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            foreach (var table in new[] { "DeductionTypes", "EarningTypes" })
            {
                migrationBuilder.AddColumn<int>(
                    name: "CalculationMethod", table: table,
                    type: "int", nullable: false, defaultValue: 0);

                migrationBuilder.AddColumn<decimal>(
                    name: "DefaultValue", table: table,
                    type: "decimal(65,30)", nullable: true);

                migrationBuilder.AddColumn<bool>(
                    name: "AppliesToAll", table: table,
                    type: "tinyint(1)", nullable: false, defaultValue: false);

                migrationBuilder.AddColumn<bool>(
                    name: "IsSystemComputed", table: table,
                    type: "tinyint(1)", nullable: false, defaultValue: false);
            }

            // The statutory deductions the engine calculates itself from the rates
            // and bands. Marking them keeps them out of the applies-to-all path.
            migrationBuilder.Sql(@"
                UPDATE DeductionTypes SET IsSystemComputed = 1
                WHERE Code IN ('PAYE','NSSF','NSSFER','SHIF','AHL');");

            // Basic, house and transport already have their own columns on the
            // salary structure and are written onto the payslip from there.
            migrationBuilder.Sql(@"
                UPDATE EarningTypes SET IsSystemComputed = 1
                WHERE Code IN ('BASIC','HSEALL','TRNALL');");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            foreach (var table in new[] { "DeductionTypes", "EarningTypes" })
            {
                migrationBuilder.DropColumn(name: "IsSystemComputed", table: table);
                migrationBuilder.DropColumn(name: "AppliesToAll", table: table);
                migrationBuilder.DropColumn(name: "DefaultValue", table: table);
                migrationBuilder.DropColumn(name: "CalculationMethod", table: table);
            }
        }
    }
}
