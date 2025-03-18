using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SchoolWebApp.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddedranktoentitySubject : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Rank",
                table: "Subjects",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "269f0cf3-405e-4163-83f3-1b63ebebd62e",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2025, 3, 18, 12, 15, 52, 925, DateTimeKind.Local).AddTicks(7166), new DateTime(2025, 3, 18, 12, 15, 52, 925, DateTimeKind.Local).AddTicks(7192) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "448df289-142c-4959-a912-60733515e1b4",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2025, 3, 18, 12, 15, 52, 925, DateTimeKind.Local).AddTicks(7050), new DateTime(2025, 3, 18, 12, 15, 52, 925, DateTimeKind.Local).AddTicks(7055) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "48c50c3a-9958-453b-b649-4e21af131322",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2025, 3, 18, 12, 15, 52, 925, DateTimeKind.Local).AddTicks(6964), new DateTime(2025, 3, 18, 12, 15, 52, 925, DateTimeKind.Local).AddTicks(6967) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "717d9b15-a428-440c-b26b-08d3bbb68b02",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2025, 3, 18, 12, 15, 52, 925, DateTimeKind.Local).AddTicks(6547), new DateTime(2025, 3, 18, 12, 15, 52, 925, DateTimeKind.Local).AddTicks(6629) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "95ed2407-3e58-4af2-88a4-1c4e96473f68",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2025, 3, 18, 12, 15, 52, 925, DateTimeKind.Local).AddTicks(6868), new DateTime(2025, 3, 18, 12, 15, 52, 925, DateTimeKind.Local).AddTicks(6872) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "97942bee-ef12-4425-8225-4f293d0f36dd",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2025, 3, 18, 12, 15, 52, 925, DateTimeKind.Local).AddTicks(7421), new DateTime(2025, 3, 18, 12, 15, 52, 925, DateTimeKind.Local).AddTicks(7425) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "cd12b44b-103b-48df-8887-a2bf42e0651e",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2025, 3, 18, 12, 15, 52, 925, DateTimeKind.Local).AddTicks(7333), new DateTime(2025, 3, 18, 12, 15, 52, 925, DateTimeKind.Local).AddTicks(7337) });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "7e67d486-af3e-49f1-a109-a2b864b8e0ec",
                columns: new[] { "ConcurrencyStamp", "Created", "Modified", "PasswordHash", "SecurityStamp" },
                values: new object[] { "e6b93a89-c49a-421a-9af6-0a4c66b6f69b", new DateTime(2025, 3, 18, 12, 15, 52, 925, DateTimeKind.Local).AddTicks(7651), new DateTime(2025, 3, 18, 12, 15, 52, 925, DateTimeKind.Local).AddTicks(7658), "AQAAAAIAAYagAAAAEDvXK87UojbPgfXIBfgbzlBW7rBqJqS5bi42Ri569jE9OQRJJCgB1rMJ55kt/g9Dyg==", "99b06e3b-77b0-4a4c-a516-771b95613786" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Rank",
                table: "Subjects");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "269f0cf3-405e-4163-83f3-1b63ebebd62e",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2025, 3, 18, 10, 28, 38, 772, DateTimeKind.Local).AddTicks(7426), new DateTime(2025, 3, 18, 10, 28, 38, 772, DateTimeKind.Local).AddTicks(7460) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "448df289-142c-4959-a912-60733515e1b4",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2025, 3, 18, 10, 28, 38, 772, DateTimeKind.Local).AddTicks(7336), new DateTime(2025, 3, 18, 10, 28, 38, 772, DateTimeKind.Local).AddTicks(7340) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "48c50c3a-9958-453b-b649-4e21af131322",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2025, 3, 18, 10, 28, 38, 772, DateTimeKind.Local).AddTicks(7237), new DateTime(2025, 3, 18, 10, 28, 38, 772, DateTimeKind.Local).AddTicks(7242) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "717d9b15-a428-440c-b26b-08d3bbb68b02",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2025, 3, 18, 10, 28, 38, 772, DateTimeKind.Local).AddTicks(6736), new DateTime(2025, 3, 18, 10, 28, 38, 772, DateTimeKind.Local).AddTicks(6814) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "95ed2407-3e58-4af2-88a4-1c4e96473f68",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2025, 3, 18, 10, 28, 38, 772, DateTimeKind.Local).AddTicks(7124), new DateTime(2025, 3, 18, 10, 28, 38, 772, DateTimeKind.Local).AddTicks(7129) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "97942bee-ef12-4425-8225-4f293d0f36dd",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2025, 3, 18, 10, 28, 38, 772, DateTimeKind.Local).AddTicks(7710), new DateTime(2025, 3, 18, 10, 28, 38, 772, DateTimeKind.Local).AddTicks(7714) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "cd12b44b-103b-48df-8887-a2bf42e0651e",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2025, 3, 18, 10, 28, 38, 772, DateTimeKind.Local).AddTicks(7591), new DateTime(2025, 3, 18, 10, 28, 38, 772, DateTimeKind.Local).AddTicks(7597) });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "7e67d486-af3e-49f1-a109-a2b864b8e0ec",
                columns: new[] { "ConcurrencyStamp", "Created", "Modified", "PasswordHash", "SecurityStamp" },
                values: new object[] { "431ffce0-421b-4679-bb08-f149cd6d6121", new DateTime(2025, 3, 18, 10, 28, 38, 772, DateTimeKind.Local).AddTicks(8182), new DateTime(2025, 3, 18, 10, 28, 38, 772, DateTimeKind.Local).AddTicks(8188), "AQAAAAIAAYagAAAAEF2gPL1/w1ex+f8l4Z1+OlzL4LvIyAhwgchkjykJLVvKLdEqLvFQ9Pl8lVMEDd9VKA==", "39dab078-34e9-4e27-87fb-76743edd210d" });
        }
    }
}
