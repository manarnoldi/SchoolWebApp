using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SchoolWebApp.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RandomUpdates : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "269f0cf3-405e-4163-83f3-1b63ebebd62e",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2024, 6, 11, 15, 57, 40, 257, DateTimeKind.Local).AddTicks(808), new DateTime(2024, 6, 11, 15, 57, 40, 257, DateTimeKind.Local).AddTicks(820) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "448df289-142c-4959-a912-60733515e1b4",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2024, 6, 11, 15, 57, 40, 257, DateTimeKind.Local).AddTicks(782), new DateTime(2024, 6, 11, 15, 57, 40, 257, DateTimeKind.Local).AddTicks(783) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "48c50c3a-9958-453b-b649-4e21af131322",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2024, 6, 11, 15, 57, 40, 257, DateTimeKind.Local).AddTicks(742), new DateTime(2024, 6, 11, 15, 57, 40, 257, DateTimeKind.Local).AddTicks(743) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "717d9b15-a428-440c-b26b-08d3bbb68b02",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2024, 6, 11, 15, 57, 40, 257, DateTimeKind.Local).AddTicks(628), new DateTime(2024, 6, 11, 15, 57, 40, 257, DateTimeKind.Local).AddTicks(662) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "95ed2407-3e58-4af2-88a4-1c4e96473f68",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2024, 6, 11, 15, 57, 40, 257, DateTimeKind.Local).AddTicks(715), new DateTime(2024, 6, 11, 15, 57, 40, 257, DateTimeKind.Local).AddTicks(716) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "97942bee-ef12-4425-8225-4f293d0f36dd",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2024, 6, 11, 15, 57, 40, 257, DateTimeKind.Local).AddTicks(895), new DateTime(2024, 6, 11, 15, 57, 40, 257, DateTimeKind.Local).AddTicks(896) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "cd12b44b-103b-48df-8887-a2bf42e0651e",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2024, 6, 11, 15, 57, 40, 257, DateTimeKind.Local).AddTicks(868), new DateTime(2024, 6, 11, 15, 57, 40, 257, DateTimeKind.Local).AddTicks(869) });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "7e67d486-af3e-49f1-a109-a2b864b8e0ec",
                columns: new[] { "ConcurrencyStamp", "Created", "Modified", "PasswordHash", "SecurityStamp" },
                values: new object[] { "13f5b169-30ce-4ba8-a83b-9874b7686684", new DateTime(2024, 6, 11, 15, 57, 40, 257, DateTimeKind.Local).AddTicks(985), new DateTime(2024, 6, 11, 15, 57, 40, 257, DateTimeKind.Local).AddTicks(987), "AQAAAAIAAYagAAAAEBNy/0cD7f9Udpoi5/UgIV4w5Zzpz57EHY2QoVM1qaAaHM4o70qrvrc0qOFnq2febA==", "9cb40420-a91c-40f9-86dd-a37e2abf0113" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "269f0cf3-405e-4163-83f3-1b63ebebd62e",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2024, 6, 11, 15, 50, 39, 39, DateTimeKind.Local).AddTicks(1752), new DateTime(2024, 6, 11, 15, 50, 39, 39, DateTimeKind.Local).AddTicks(1783) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "448df289-142c-4959-a912-60733515e1b4",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2024, 6, 11, 15, 50, 39, 39, DateTimeKind.Local).AddTicks(1689), new DateTime(2024, 6, 11, 15, 50, 39, 39, DateTimeKind.Local).AddTicks(1691) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "48c50c3a-9958-453b-b649-4e21af131322",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2024, 6, 11, 15, 50, 39, 39, DateTimeKind.Local).AddTicks(1636), new DateTime(2024, 6, 11, 15, 50, 39, 39, DateTimeKind.Local).AddTicks(1638) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "717d9b15-a428-440c-b26b-08d3bbb68b02",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2024, 6, 11, 15, 50, 39, 39, DateTimeKind.Local).AddTicks(1414), new DateTime(2024, 6, 11, 15, 50, 39, 39, DateTimeKind.Local).AddTicks(1463) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "95ed2407-3e58-4af2-88a4-1c4e96473f68",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2024, 6, 11, 15, 50, 39, 39, DateTimeKind.Local).AddTicks(1577), new DateTime(2024, 6, 11, 15, 50, 39, 39, DateTimeKind.Local).AddTicks(1579) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "97942bee-ef12-4425-8225-4f293d0f36dd",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2024, 6, 11, 15, 50, 39, 39, DateTimeKind.Local).AddTicks(1903), new DateTime(2024, 6, 11, 15, 50, 39, 39, DateTimeKind.Local).AddTicks(1905) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "cd12b44b-103b-48df-8887-a2bf42e0651e",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2024, 6, 11, 15, 50, 39, 39, DateTimeKind.Local).AddTicks(1853), new DateTime(2024, 6, 11, 15, 50, 39, 39, DateTimeKind.Local).AddTicks(1855) });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "7e67d486-af3e-49f1-a109-a2b864b8e0ec",
                columns: new[] { "ConcurrencyStamp", "Created", "Modified", "PasswordHash", "SecurityStamp" },
                values: new object[] { "91242774-5ae3-4beb-8c25-e2c28c6d0427", new DateTime(2024, 6, 11, 15, 50, 39, 39, DateTimeKind.Local).AddTicks(2052), new DateTime(2024, 6, 11, 15, 50, 39, 39, DateTimeKind.Local).AddTicks(2054), "AQAAAAIAAYagAAAAEDZAw6fQzXBSHU70oy6RSjiVjCmTItqlSPmb+qPHSLshoUMthhGETyjiuasc8eUoCw==", "bcb74c6f-2852-46d2-a0fa-6a401f07c248" });
        }
    }
}
