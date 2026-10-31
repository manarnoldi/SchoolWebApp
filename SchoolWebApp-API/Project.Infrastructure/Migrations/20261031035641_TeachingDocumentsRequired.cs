using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SchoolWebApp.Infrastructure.Migrations
{
    /// <summary>
    /// Subjects and learning levels say whether teachers keep teaching documents for them. Every existing one
    /// does (true), so nothing leaves Teaching Management until a school unticks it.
    /// </summary>
    public partial class TeachingDocumentsRequired : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "RequiresTeachingDocuments",
                table: "Subjects",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddColumn<bool>(
                name: "RequiresTeachingDocuments",
                table: "LearningLevels",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RequiresTeachingDocuments",
                table: "Subjects");

            migrationBuilder.DropColumn(
                name: "RequiresTeachingDocuments",
                table: "LearningLevels");
        }
    }
}
