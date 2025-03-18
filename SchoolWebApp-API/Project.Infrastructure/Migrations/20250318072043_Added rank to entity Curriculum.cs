using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SchoolWebApp.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddedranktoentityCurriculum : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Rank",
                table: "Curricula",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "269f0cf3-405e-4163-83f3-1b63ebebd62e",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2025, 3, 18, 10, 20, 40, 501, DateTimeKind.Local).AddTicks(810), new DateTime(2025, 3, 18, 10, 20, 40, 501, DateTimeKind.Local).AddTicks(837) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "448df289-142c-4959-a912-60733515e1b4",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2025, 3, 18, 10, 20, 40, 501, DateTimeKind.Local).AddTicks(638), new DateTime(2025, 3, 18, 10, 20, 40, 501, DateTimeKind.Local).AddTicks(643) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "48c50c3a-9958-453b-b649-4e21af131322",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2025, 3, 18, 10, 20, 40, 501, DateTimeKind.Local).AddTicks(526), new DateTime(2025, 3, 18, 10, 20, 40, 501, DateTimeKind.Local).AddTicks(532) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "717d9b15-a428-440c-b26b-08d3bbb68b02",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2025, 3, 18, 10, 20, 40, 501, DateTimeKind.Local).AddTicks(63), new DateTime(2025, 3, 18, 10, 20, 40, 501, DateTimeKind.Local).AddTicks(144) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "95ed2407-3e58-4af2-88a4-1c4e96473f68",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2025, 3, 18, 10, 20, 40, 501, DateTimeKind.Local).AddTicks(404), new DateTime(2025, 3, 18, 10, 20, 40, 501, DateTimeKind.Local).AddTicks(410) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "97942bee-ef12-4425-8225-4f293d0f36dd",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2025, 3, 18, 10, 20, 40, 501, DateTimeKind.Local).AddTicks(1085), new DateTime(2025, 3, 18, 10, 20, 40, 501, DateTimeKind.Local).AddTicks(1089) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "cd12b44b-103b-48df-8887-a2bf42e0651e",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2025, 3, 18, 10, 20, 40, 501, DateTimeKind.Local).AddTicks(984), new DateTime(2025, 3, 18, 10, 20, 40, 501, DateTimeKind.Local).AddTicks(990) });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "7e67d486-af3e-49f1-a109-a2b864b8e0ec",
                columns: new[] { "ConcurrencyStamp", "Created", "Modified", "PasswordHash", "SecurityStamp" },
                values: new object[] { "488dbe87-8204-438a-bd9a-0c7db8284a97", new DateTime(2025, 3, 18, 10, 20, 40, 501, DateTimeKind.Local).AddTicks(1395), new DateTime(2025, 3, 18, 10, 20, 40, 501, DateTimeKind.Local).AddTicks(1403), "AQAAAAIAAYagAAAAEFvmcdlKz1DR73WX5WArEO/iMcgbUYbDUJFOY1mgznCnw25x22iLY9E+sollplknIQ==", "1f33cc60-dd89-4f2d-bcc0-6a3271a49348" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Rank",
                table: "Curricula");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "269f0cf3-405e-4163-83f3-1b63ebebd62e",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2025, 3, 18, 8, 34, 13, 151, DateTimeKind.Local).AddTicks(6112), new DateTime(2025, 3, 18, 8, 34, 13, 151, DateTimeKind.Local).AddTicks(6125) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "448df289-142c-4959-a912-60733515e1b4",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2025, 3, 18, 8, 34, 13, 151, DateTimeKind.Local).AddTicks(6040), new DateTime(2025, 3, 18, 8, 34, 13, 151, DateTimeKind.Local).AddTicks(6042) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "48c50c3a-9958-453b-b649-4e21af131322",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2025, 3, 18, 8, 34, 13, 151, DateTimeKind.Local).AddTicks(5979), new DateTime(2025, 3, 18, 8, 34, 13, 151, DateTimeKind.Local).AddTicks(5981) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "717d9b15-a428-440c-b26b-08d3bbb68b02",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2025, 3, 18, 8, 34, 13, 151, DateTimeKind.Local).AddTicks(5693), new DateTime(2025, 3, 18, 8, 34, 13, 151, DateTimeKind.Local).AddTicks(5766) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "95ed2407-3e58-4af2-88a4-1c4e96473f68",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2025, 3, 18, 8, 34, 13, 151, DateTimeKind.Local).AddTicks(5915), new DateTime(2025, 3, 18, 8, 34, 13, 151, DateTimeKind.Local).AddTicks(5918) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "97942bee-ef12-4425-8225-4f293d0f36dd",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2025, 3, 18, 8, 34, 13, 151, DateTimeKind.Local).AddTicks(6264), new DateTime(2025, 3, 18, 8, 34, 13, 151, DateTimeKind.Local).AddTicks(6267) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "cd12b44b-103b-48df-8887-a2bf42e0651e",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2025, 3, 18, 8, 34, 13, 151, DateTimeKind.Local).AddTicks(6204), new DateTime(2025, 3, 18, 8, 34, 13, 151, DateTimeKind.Local).AddTicks(6207) });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "7e67d486-af3e-49f1-a109-a2b864b8e0ec",
                columns: new[] { "ConcurrencyStamp", "Created", "Modified", "PasswordHash", "SecurityStamp" },
                values: new object[] { "336ff891-0c4f-40ff-990f-5904149f9da6", new DateTime(2025, 3, 18, 8, 34, 13, 151, DateTimeKind.Local).AddTicks(6645), new DateTime(2025, 3, 18, 8, 34, 13, 151, DateTimeKind.Local).AddTicks(6656), "AQAAAAIAAYagAAAAEDOlIBURO/Tmy8XQmh4Th6xypVT4GaP2xM2wghhaEBxZVuOqyOS6kMSNr0yeaNPFKQ==", "7af78474-f629-46a7-a0e1-2cf42706f976" });
        }
    }
}
