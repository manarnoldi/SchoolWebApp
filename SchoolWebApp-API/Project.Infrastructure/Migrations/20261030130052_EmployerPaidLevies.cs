using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SchoolWebApp.Infrastructure.Migrations
{
    /// <summary>
    /// Deduction types the employer pays rather than deducts (NITA, where the school bears it), and each
    /// payslip's total of them. Off for every existing type, so a school deducting NITA from pay carries on.
    /// </summary>
    public partial class EmployerPaidLevies : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "EmployerCosts",
                table: "Payslips",
                type: "decimal(65,30)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<bool>(
                name: "IsEmployerCost",
                table: "DeductionTypes",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EmployerCosts",
                table: "Payslips");

            migrationBuilder.DropColumn(
                name: "IsEmployerCost",
                table: "DeductionTypes");
        }
    }
}
