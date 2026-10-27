using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Project.Infrastructure.Data;

#nullable disable

namespace SchoolWebApp.Infrastructure.Migrations
{
    // A level learning outcome can belong to one education level (KICD prints
    // level outcomes for Primary and for Junior School, while education level
    // types group the levels differently). Unset, it serves the whole type.
    //
    // Hand-written rather than scaffolded - see the note on UniquePayrollTypeCodes.
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20261027120000_GeneralOutcomeEducationLevel")]
    public partial class GeneralOutcomeEducationLevel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "EducationLevelId",
                table: "GeneralOutcomes",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_GeneralOutcomes_EducationLevelId",
                table: "GeneralOutcomes",
                column: "EducationLevelId");

            migrationBuilder.AddForeignKey(
                name: "FK_GeneralOutcomes_EducationLevels_EducationLevelId",
                table: "GeneralOutcomes",
                column: "EducationLevelId",
                principalTable: "EducationLevels",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_GeneralOutcomes_EducationLevels_EducationLevelId",
                table: "GeneralOutcomes");

            migrationBuilder.DropIndex(
                name: "IX_GeneralOutcomes_EducationLevelId",
                table: "GeneralOutcomes");

            migrationBuilder.DropColumn(
                name: "EducationLevelId",
                table: "GeneralOutcomes");
        }
    }
}
