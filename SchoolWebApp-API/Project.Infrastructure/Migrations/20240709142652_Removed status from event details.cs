using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SchoolWebApp.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Removedstatusfromeventdetails : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Status",
                table: "Events");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "269f0cf3-405e-4163-83f3-1b63ebebd62e",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2024, 7, 9, 17, 26, 50, 830, DateTimeKind.Local).AddTicks(1130), new DateTime(2024, 7, 9, 17, 26, 50, 830, DateTimeKind.Local).AddTicks(1148) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "448df289-142c-4959-a912-60733515e1b4",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2024, 7, 9, 17, 26, 50, 830, DateTimeKind.Local).AddTicks(1082), new DateTime(2024, 7, 9, 17, 26, 50, 830, DateTimeKind.Local).AddTicks(1084) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "48c50c3a-9958-453b-b649-4e21af131322",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2024, 7, 9, 17, 26, 50, 830, DateTimeKind.Local).AddTicks(1032), new DateTime(2024, 7, 9, 17, 26, 50, 830, DateTimeKind.Local).AddTicks(1034) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "717d9b15-a428-440c-b26b-08d3bbb68b02",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2024, 7, 9, 17, 26, 50, 830, DateTimeKind.Local).AddTicks(824), new DateTime(2024, 7, 9, 17, 26, 50, 830, DateTimeKind.Local).AddTicks(868) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "95ed2407-3e58-4af2-88a4-1c4e96473f68",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2024, 7, 9, 17, 26, 50, 830, DateTimeKind.Local).AddTicks(977), new DateTime(2024, 7, 9, 17, 26, 50, 830, DateTimeKind.Local).AddTicks(980) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "97942bee-ef12-4425-8225-4f293d0f36dd",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2024, 7, 9, 17, 26, 50, 830, DateTimeKind.Local).AddTicks(1291), new DateTime(2024, 7, 9, 17, 26, 50, 830, DateTimeKind.Local).AddTicks(1293) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "cd12b44b-103b-48df-8887-a2bf42e0651e",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2024, 7, 9, 17, 26, 50, 830, DateTimeKind.Local).AddTicks(1222), new DateTime(2024, 7, 9, 17, 26, 50, 830, DateTimeKind.Local).AddTicks(1226) });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "7e67d486-af3e-49f1-a109-a2b864b8e0ec",
                columns: new[] { "ConcurrencyStamp", "Created", "Modified", "PasswordHash", "SecurityStamp" },
                values: new object[] { "7e04e9a7-2699-4dce-9493-2e50c138a473", new DateTime(2024, 7, 9, 17, 26, 50, 830, DateTimeKind.Local).AddTicks(1447), new DateTime(2024, 7, 9, 17, 26, 50, 830, DateTimeKind.Local).AddTicks(1450), "AQAAAAIAAYagAAAAEG9Iskuotq9VAty89ZflSHINR7QrX2mdEwlUTxgydhVeuSEXMNuRO2IYsp91PlbxyA==", "a225155b-085e-43a9-904b-a383a610cece" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Status",
                table: "Events",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "269f0cf3-405e-4163-83f3-1b63ebebd62e",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2024, 7, 9, 9, 35, 20, 193, DateTimeKind.Local).AddTicks(3255), new DateTime(2024, 7, 9, 9, 35, 20, 193, DateTimeKind.Local).AddTicks(3257) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "448df289-142c-4959-a912-60733515e1b4",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2024, 7, 9, 9, 35, 20, 193, DateTimeKind.Local).AddTicks(3224), new DateTime(2024, 7, 9, 9, 35, 20, 193, DateTimeKind.Local).AddTicks(3226) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "48c50c3a-9958-453b-b649-4e21af131322",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2024, 7, 9, 9, 35, 20, 193, DateTimeKind.Local).AddTicks(3192), new DateTime(2024, 7, 9, 9, 35, 20, 193, DateTimeKind.Local).AddTicks(3194) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "717d9b15-a428-440c-b26b-08d3bbb68b02",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2024, 7, 9, 9, 35, 20, 193, DateTimeKind.Local).AddTicks(3048), new DateTime(2024, 7, 9, 9, 35, 20, 193, DateTimeKind.Local).AddTicks(3079) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "95ed2407-3e58-4af2-88a4-1c4e96473f68",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2024, 7, 9, 9, 35, 20, 193, DateTimeKind.Local).AddTicks(3146), new DateTime(2024, 7, 9, 9, 35, 20, 193, DateTimeKind.Local).AddTicks(3158) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "97942bee-ef12-4425-8225-4f293d0f36dd",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2024, 7, 9, 9, 35, 20, 193, DateTimeKind.Local).AddTicks(3367), new DateTime(2024, 7, 9, 9, 35, 20, 193, DateTimeKind.Local).AddTicks(3369) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "cd12b44b-103b-48df-8887-a2bf42e0651e",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2024, 7, 9, 9, 35, 20, 193, DateTimeKind.Local).AddTicks(3310), new DateTime(2024, 7, 9, 9, 35, 20, 193, DateTimeKind.Local).AddTicks(3321) });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "7e67d486-af3e-49f1-a109-a2b864b8e0ec",
                columns: new[] { "ConcurrencyStamp", "Created", "Modified", "PasswordHash", "SecurityStamp" },
                values: new object[] { "a3319857-b7dc-43da-a2d3-76ef714d19ab", new DateTime(2024, 7, 9, 9, 35, 20, 193, DateTimeKind.Local).AddTicks(3478), new DateTime(2024, 7, 9, 9, 35, 20, 193, DateTimeKind.Local).AddTicks(3480), "AQAAAAIAAYagAAAAEALhl+8ZJ0FWomZ5gq8f2A84lT61G2B8KzV1N4ztl85ZoSZbanHghbplrQhDwPCLeQ==", "637134d1-f401-4987-a6a9-f51146a39941" });
        }
    }
}
