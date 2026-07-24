using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SchoolWebApp.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddStaffNoToStaffDetails : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // StaffDetails is a TPH-derived type of Person (HasBaseType("Person")
            // with no separate table), so its columns live in the Person table.
            // Targeting "StaffDetails" fails with "table doesn't exist".
            migrationBuilder.AddColumn<string>(
                name: "StaffNo",
                table: "Person",
                type: "varchar(255)",
                maxLength: 255,
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "StaffNo",
                table: "Person");
        }
    }
}
