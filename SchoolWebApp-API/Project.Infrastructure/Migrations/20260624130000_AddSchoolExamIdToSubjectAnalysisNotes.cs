using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Project.Infrastructure.Data;

#nullable disable

namespace SchoolWebApp.Infrastructure.Migrations
{
    // Adds the exam dimension to the action-plan note so a plan can be kept per
    // exam/term. Separate migration (not folded into AddSubjectAnalysisNotes)
    // because that one was already applied without the column. Attributes are
    // inline so EF discovers/applies it at runtime.
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20260624130000_AddSchoolExamIdToSubjectAnalysisNotes")]
    public partial class AddSchoolExamIdToSubjectAnalysisNotes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_SubjectAnalysisNotes_AcademicYearId_SchoolClassId_SubjectId",
                table: "SubjectAnalysisNotes");

            migrationBuilder.AddColumn<int>(
                name: "SchoolExamId",
                table: "SubjectAnalysisNotes",
                type: "int",
                nullable: false,
                defaultValue: 0);

            // Short explicit name: the default 4-column name would exceed
            // MySQL's 64-character identifier limit.
            migrationBuilder.CreateIndex(
                name: "IX_SubjectAnalysisNotes_Year_Class_Subject_Exam",
                table: "SubjectAnalysisNotes",
                columns: new[] { "AcademicYearId", "SchoolClassId", "SubjectId", "SchoolExamId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_SubjectAnalysisNotes_Year_Class_Subject_Exam",
                table: "SubjectAnalysisNotes");

            migrationBuilder.DropColumn(
                name: "SchoolExamId",
                table: "SubjectAnalysisNotes");

            migrationBuilder.CreateIndex(
                name: "IX_SubjectAnalysisNotes_AcademicYearId_SchoolClassId_SubjectId",
                table: "SubjectAnalysisNotes",
                columns: new[] { "AcademicYearId", "SchoolClassId", "SubjectId" },
                unique: true);
        }
    }
}
