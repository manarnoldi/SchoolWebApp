using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SchoolWebApp.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RoutineUpdates : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            
            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] { "Id", "AccessFailedCount", "ConcurrencyStamp", "Created", "CreatedBy", "Email", "EmailConfirmed", "FirstName", "LastName", "LockoutEnabled", "LockoutEnd", "Modified", "ModifiedBy", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "SecurityStamp", "TwoFactorEnabled", "UserName", "PersonId" },
                values: new object[] { "7e67d486-af3e-49f1-a109-a2b864b8e0ec", 0, "299c5483-0713-434f-a887-4250f2c72dc5", new DateTime(2024, 3, 1, 10, 54, 46, 975, DateTimeKind.Local).AddTicks(2895), "admin", "admin@kodetek.co.ke", true, "SchoolSoft", "Administrator", false, null, new DateTime(2024, 3, 1, 10, 54, 46, 975, DateTimeKind.Local).AddTicks(2897), "admin", "ADMIN@KODETEK.CO.KE", "ADMIN", "AQAAAAIAAYagAAAAEDSD/T5BB+kaE0etksxMJrMkg43NE+JnM9HRqVqBPERirPAFdR5NTygwl6AGcUBo/w==", "+254724920000", true, "11e2229c-53d6-49dc-95dc-ff098face453", false, "admin", 5 });

        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AspNetUsers");
        }
    }
}
