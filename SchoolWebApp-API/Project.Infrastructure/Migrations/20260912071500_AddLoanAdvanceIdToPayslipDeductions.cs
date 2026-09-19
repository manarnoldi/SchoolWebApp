using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Project.Infrastructure.Data;

#nullable disable

namespace SchoolWebApp.Infrastructure.Migrations
{
    // Processing a payroll period reduces each loan's balance by the instalment
    // it deducts. Re-processing discards the payslips and rebuilds them, but had
    // no way to credit those instalments back, because a PayslipDeduction only
    // recorded the deduction type - not which loan the money came off. Every
    // re-run therefore paid the loan down again while the payslip still showed a
    // single instalment.
    //
    // This adds the loan reference so the re-process path can reverse exactly
    // what the previous run took.
    //
    // Hand-written for the same reason as UniquePayrollTypeCodes - see the note
    // on that migration: scaffolding here also emits unrelated seed-data drift.
    // The schema operations below are exactly what `dotnet ef migrations add`
    // produced for this change, with that drift stripped out.
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20260912071500_AddLoanAdvanceIdToPayslipDeductions")]
    public partial class AddLoanAdvanceIdToPayslipDeductions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "LoanAdvanceId",
                table: "PayslipDeductions",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_PayslipDeductions_LoanAdvanceId",
                table: "PayslipDeductions",
                column: "LoanAdvanceId");

            migrationBuilder.AddForeignKey(
                name: "FK_PayslipDeductions_LoanAdvances_LoanAdvanceId",
                table: "PayslipDeductions",
                column: "LoanAdvanceId",
                principalTable: "LoanAdvances",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PayslipDeductions_LoanAdvances_LoanAdvanceId",
                table: "PayslipDeductions");

            migrationBuilder.DropIndex(
                name: "IX_PayslipDeductions_LoanAdvanceId",
                table: "PayslipDeductions");

            migrationBuilder.DropColumn(
                name: "LoanAdvanceId",
                table: "PayslipDeductions");
        }
    }
}
