using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SchoolWebApp.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddedFieldstoStaffDetails : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "CurrentlyEmployed",
                table: "Person",
                type: "tinyint(1)",
                nullable: true);

            migrationBuilder.AddColumn<DateOnly>(
                name: "EmploymentDate",
                table: "Person",
                type: "date",
                nullable: true);

            migrationBuilder.AddColumn<DateOnly>(
                name: "EndofEmploymentDate",
                table: "Person",
                type: "date",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "KraPinNo",
                table: "Person",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "NhifNo",
                table: "Person",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "NssfNo",
                table: "Person",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "OtherDetails",
                table: "Person",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "StaffImageAsBase64",
                table: "Person",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "269f0cf3-405e-4163-83f3-1b63ebebd62e",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2024, 7, 2, 17, 43, 0, 923, DateTimeKind.Local).AddTicks(1700), new DateTime(2024, 7, 2, 17, 43, 0, 923, DateTimeKind.Local).AddTicks(1703) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "448df289-142c-4959-a912-60733515e1b4",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2024, 7, 2, 17, 43, 0, 923, DateTimeKind.Local).AddTicks(1638), new DateTime(2024, 7, 2, 17, 43, 0, 923, DateTimeKind.Local).AddTicks(1640) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "48c50c3a-9958-453b-b649-4e21af131322",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2024, 7, 2, 17, 43, 0, 923, DateTimeKind.Local).AddTicks(1554), new DateTime(2024, 7, 2, 17, 43, 0, 923, DateTimeKind.Local).AddTicks(1558) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "717d9b15-a428-440c-b26b-08d3bbb68b02",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2024, 7, 2, 17, 43, 0, 923, DateTimeKind.Local).AddTicks(1302), new DateTime(2024, 7, 2, 17, 43, 0, 923, DateTimeKind.Local).AddTicks(1338) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "95ed2407-3e58-4af2-88a4-1c4e96473f68",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2024, 7, 2, 17, 43, 0, 923, DateTimeKind.Local).AddTicks(1455), new DateTime(2024, 7, 2, 17, 43, 0, 923, DateTimeKind.Local).AddTicks(1474) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "97942bee-ef12-4425-8225-4f293d0f36dd",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2024, 7, 2, 17, 43, 0, 923, DateTimeKind.Local).AddTicks(1855), new DateTime(2024, 7, 2, 17, 43, 0, 923, DateTimeKind.Local).AddTicks(1860) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "cd12b44b-103b-48df-8887-a2bf42e0651e",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2024, 7, 2, 17, 43, 0, 923, DateTimeKind.Local).AddTicks(1765), new DateTime(2024, 7, 2, 17, 43, 0, 923, DateTimeKind.Local).AddTicks(1788) });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "7e67d486-af3e-49f1-a109-a2b864b8e0ec",
                columns: new[] { "ConcurrencyStamp", "Created", "Modified", "PasswordHash", "SecurityStamp" },
                values: new object[] { "bef56cb6-bd7c-42c1-8751-bdddc65ee974", new DateTime(2024, 7, 2, 17, 43, 0, 923, DateTimeKind.Local).AddTicks(2034), new DateTime(2024, 7, 2, 17, 43, 0, 923, DateTimeKind.Local).AddTicks(2036), "AQAAAAIAAYagAAAAEEzO+kQYeL58J9yCay8QSI5maTw21hUs9T5Bm/N6a8jV3CC0KnHkyTOEcp0ZyfuzWA==", "4f317d58-f476-4bb5-ba8f-fa73efee91f3" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CurrentlyEmployed",
                table: "Person");

            migrationBuilder.DropColumn(
                name: "EmploymentDate",
                table: "Person");

            migrationBuilder.DropColumn(
                name: "EndofEmploymentDate",
                table: "Person");

            migrationBuilder.DropColumn(
                name: "KraPinNo",
                table: "Person");

            migrationBuilder.DropColumn(
                name: "NhifNo",
                table: "Person");

            migrationBuilder.DropColumn(
                name: "NssfNo",
                table: "Person");

            migrationBuilder.DropColumn(
                name: "OtherDetails",
                table: "Person");

            migrationBuilder.DropColumn(
                name: "StaffImageAsBase64",
                table: "Person");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "269f0cf3-405e-4163-83f3-1b63ebebd62e",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2024, 6, 30, 6, 49, 35, 43, DateTimeKind.Local).AddTicks(5199), new DateTime(2024, 6, 30, 6, 49, 35, 43, DateTimeKind.Local).AddTicks(5210) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "448df289-142c-4959-a912-60733515e1b4",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2024, 6, 30, 6, 49, 35, 43, DateTimeKind.Local).AddTicks(5176), new DateTime(2024, 6, 30, 6, 49, 35, 43, DateTimeKind.Local).AddTicks(5177) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "48c50c3a-9958-453b-b649-4e21af131322",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2024, 6, 30, 6, 49, 35, 43, DateTimeKind.Local).AddTicks(5152), new DateTime(2024, 6, 30, 6, 49, 35, 43, DateTimeKind.Local).AddTicks(5153) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "717d9b15-a428-440c-b26b-08d3bbb68b02",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2024, 6, 30, 6, 49, 35, 43, DateTimeKind.Local).AddTicks(4943), new DateTime(2024, 6, 30, 6, 49, 35, 43, DateTimeKind.Local).AddTicks(4976) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "95ed2407-3e58-4af2-88a4-1c4e96473f68",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2024, 6, 30, 6, 49, 35, 43, DateTimeKind.Local).AddTicks(5124), new DateTime(2024, 6, 30, 6, 49, 35, 43, DateTimeKind.Local).AddTicks(5126) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "97942bee-ef12-4425-8225-4f293d0f36dd",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2024, 6, 30, 6, 49, 35, 43, DateTimeKind.Local).AddTicks(5289), new DateTime(2024, 6, 30, 6, 49, 35, 43, DateTimeKind.Local).AddTicks(5290) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "cd12b44b-103b-48df-8887-a2bf42e0651e",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2024, 6, 30, 6, 49, 35, 43, DateTimeKind.Local).AddTicks(5246), new DateTime(2024, 6, 30, 6, 49, 35, 43, DateTimeKind.Local).AddTicks(5248) });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "7e67d486-af3e-49f1-a109-a2b864b8e0ec",
                columns: new[] { "ConcurrencyStamp", "Created", "Modified", "PasswordHash", "SecurityStamp" },
                values: new object[] { "e7bfbbc3-dabb-4043-9ee7-817a93cab698", new DateTime(2024, 6, 30, 6, 49, 35, 43, DateTimeKind.Local).AddTicks(5403), new DateTime(2024, 6, 30, 6, 49, 35, 43, DateTimeKind.Local).AddTicks(5404), "AQAAAAIAAYagAAAAEA3ePonTkAFh2VABjQhe2Zd6TjJx3dbTKgvdFXJFKFGzLwqsCo+wrhf4WyspuCi/Mg==", "ad823eb9-762e-48b1-a351-1988d33c8e4e" });
        }
    }
}
