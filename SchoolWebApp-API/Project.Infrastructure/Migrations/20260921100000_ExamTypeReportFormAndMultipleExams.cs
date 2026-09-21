using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Project.Infrastructure.Data;

#nullable disable

namespace SchoolWebApp.Infrastructure.Migrations
{
    // Exam types gain two settings, so a school can run exams that are tracked but
    // never reported on - weekly marathons for candidate classes, say.
    //
    // - Internal is renamed ShowOnReportForm. The flag always decided whether a
    //   type gets a report-form column, but "Internal" read as though it meant the
    //   opposite, which invited it being ticked for exactly the exams meant to be
    //   left off. Renaming keeps every existing setting as it is.
    // - AllowMultiplePerTerm is new and off everywhere: a term still holds one
    //   opening, mid-term and end-term exam each. Turning it on lets a type hold as
    //   many exams in a term as needed.
    //
    // Hand-written rather than scaffolded - see the note on UniquePayrollTypeCodes.
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20260921100000_ExamTypeReportFormAndMultipleExams")]
    public partial class ExamTypeReportFormAndMultipleExams : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Internal",
                table: "ExamTypes",
                newName: "ShowOnReportForm");

            migrationBuilder.AddColumn<bool>(
                name: "AllowMultiplePerTerm",
                table: "ExamTypes",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AllowMultiplePerTerm",
                table: "ExamTypes");

            migrationBuilder.RenameColumn(
                name: "ShowOnReportForm",
                table: "ExamTypes",
                newName: "Internal");
        }
    }
}
