using System;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Project.Infrastructure.Data;

#nullable disable

namespace SchoolWebApp.Infrastructure.Migrations
{
    // Attributes are placed here (rather than a separate .Designer.cs) so EF
    // discovers and applies the migration at runtime. A BuildTargetModel is not
    // required for Database.Migrate() - it applies the Up operations directly -
    // and the model snapshot is updated separately for future scaffolds.
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20260624120000_AddSubjectAnalysisNotes")]
    public partial class AddSubjectAnalysisNotes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SubjectAnalysisNotes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    AcademicYearId = table.Column<int>(type: "int", nullable: false),
                    SchoolClassId = table.Column<int>(type: "int", nullable: false),
                    SubjectId = table.Column<int>(type: "int", nullable: false),
                    ActionPlan = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Created = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    CreatedBy = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Modified = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    ModifiedBy = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SubjectAnalysisNotes", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_SubjectAnalysisNotes_AcademicYearId_SchoolClassId_SubjectId",
                table: "SubjectAnalysisNotes",
                columns: new[] { "AcademicYearId", "SchoolClassId", "SubjectId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SubjectAnalysisNotes");
        }
    }
}
