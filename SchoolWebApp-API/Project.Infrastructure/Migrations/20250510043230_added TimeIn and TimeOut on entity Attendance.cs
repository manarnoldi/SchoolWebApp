using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SchoolWebApp.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class addedTimeInandTimeOutonentityAttendance : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<TimeOnly>(
                name: "TimeIn",
                table: "StudentAttendances",
                type: "time(6)",
                nullable: true);

            migrationBuilder.AddColumn<TimeOnly>(
                name: "TimeOut",
                table: "StudentAttendances",
                type: "time(6)",
                nullable: true);

            migrationBuilder.AddColumn<TimeOnly>(
                name: "TimeIn",
                table: "StaffAttendances",
                type: "time(6)",
                nullable: true);

            migrationBuilder.AddColumn<TimeOnly>(
                name: "TimeOut",
                table: "StaffAttendances",
                type: "time(6)",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "269f0cf3-405e-4163-83f3-1b63ebebd62e",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2025, 5, 10, 7, 32, 26, 864, DateTimeKind.Local).AddTicks(1794), new DateTime(2025, 5, 10, 7, 32, 26, 864, DateTimeKind.Local).AddTicks(1812) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "448df289-142c-4959-a912-60733515e1b4",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2025, 5, 10, 7, 32, 26, 864, DateTimeKind.Local).AddTicks(1754), new DateTime(2025, 5, 10, 7, 32, 26, 864, DateTimeKind.Local).AddTicks(1756) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "48c50c3a-9958-453b-b649-4e21af131322",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2025, 5, 10, 7, 32, 26, 864, DateTimeKind.Local).AddTicks(1714), new DateTime(2025, 5, 10, 7, 32, 26, 864, DateTimeKind.Local).AddTicks(1716) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "717d9b15-a428-440c-b26b-08d3bbb68b02",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2025, 5, 10, 7, 32, 26, 864, DateTimeKind.Local).AddTicks(1560), new DateTime(2025, 5, 10, 7, 32, 26, 864, DateTimeKind.Local).AddTicks(1598) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "95ed2407-3e58-4af2-88a4-1c4e96473f68",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2025, 5, 10, 7, 32, 26, 864, DateTimeKind.Local).AddTicks(1673), new DateTime(2025, 5, 10, 7, 32, 26, 864, DateTimeKind.Local).AddTicks(1675) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "97942bee-ef12-4425-8225-4f293d0f36dd",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2025, 5, 10, 7, 32, 26, 864, DateTimeKind.Local).AddTicks(1904), new DateTime(2025, 5, 10, 7, 32, 26, 864, DateTimeKind.Local).AddTicks(1906) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "cd12b44b-103b-48df-8887-a2bf42e0651e",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2025, 5, 10, 7, 32, 26, 864, DateTimeKind.Local).AddTicks(1864), new DateTime(2025, 5, 10, 7, 32, 26, 864, DateTimeKind.Local).AddTicks(1868) });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "7e67d486-af3e-49f1-a109-a2b864b8e0ec",
                columns: new[] { "ConcurrencyStamp", "Created", "Modified", "PasswordHash", "SecurityStamp" },
                values: new object[] { "5ce54d61-669f-4290-9735-45ce5ac22729", new DateTime(2025, 5, 10, 7, 32, 26, 864, DateTimeKind.Local).AddTicks(2212), new DateTime(2025, 5, 10, 7, 32, 26, 864, DateTimeKind.Local).AddTicks(2215), "AQAAAAIAAYagAAAAELMUZ/rCv/BSX/7yEkvbrD4LjVPQj7n+B0PXtFZ7YCxS21h45FGYk2N/lYSM6L8Qtg==", "5787c3a8-2423-4763-95eb-377b2ec0ee0c" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TimeIn",
                table: "StudentAttendances");

            migrationBuilder.DropColumn(
                name: "TimeOut",
                table: "StudentAttendances");

            migrationBuilder.DropColumn(
                name: "TimeIn",
                table: "StaffAttendances");

            migrationBuilder.DropColumn(
                name: "TimeOut",
                table: "StaffAttendances");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "269f0cf3-405e-4163-83f3-1b63ebebd62e",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2025, 3, 22, 8, 7, 40, 616, DateTimeKind.Local).AddTicks(8909), new DateTime(2025, 3, 22, 8, 7, 40, 616, DateTimeKind.Local).AddTicks(8930) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "448df289-142c-4959-a912-60733515e1b4",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2025, 3, 22, 8, 7, 40, 616, DateTimeKind.Local).AddTicks(8854), new DateTime(2025, 3, 22, 8, 7, 40, 616, DateTimeKind.Local).AddTicks(8856) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "48c50c3a-9958-453b-b649-4e21af131322",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2025, 3, 22, 8, 7, 40, 616, DateTimeKind.Local).AddTicks(8800), new DateTime(2025, 3, 22, 8, 7, 40, 616, DateTimeKind.Local).AddTicks(8802) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "717d9b15-a428-440c-b26b-08d3bbb68b02",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2025, 3, 22, 8, 7, 40, 616, DateTimeKind.Local).AddTicks(8564), new DateTime(2025, 3, 22, 8, 7, 40, 616, DateTimeKind.Local).AddTicks(8616) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "95ed2407-3e58-4af2-88a4-1c4e96473f68",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2025, 3, 22, 8, 7, 40, 616, DateTimeKind.Local).AddTicks(8731), new DateTime(2025, 3, 22, 8, 7, 40, 616, DateTimeKind.Local).AddTicks(8733) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "97942bee-ef12-4425-8225-4f293d0f36dd",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2025, 3, 22, 8, 7, 40, 616, DateTimeKind.Local).AddTicks(9089), new DateTime(2025, 3, 22, 8, 7, 40, 616, DateTimeKind.Local).AddTicks(9091) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "cd12b44b-103b-48df-8887-a2bf42e0651e",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2025, 3, 22, 8, 7, 40, 616, DateTimeKind.Local).AddTicks(9016), new DateTime(2025, 3, 22, 8, 7, 40, 616, DateTimeKind.Local).AddTicks(9019) });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "7e67d486-af3e-49f1-a109-a2b864b8e0ec",
                columns: new[] { "ConcurrencyStamp", "Created", "Modified", "PasswordHash", "SecurityStamp" },
                values: new object[] { "b6bcfecf-b93f-4866-9caf-e97ddcc724e7", new DateTime(2025, 3, 22, 8, 7, 40, 616, DateTimeKind.Local).AddTicks(9258), new DateTime(2025, 3, 22, 8, 7, 40, 616, DateTimeKind.Local).AddTicks(9261), "AQAAAAIAAYagAAAAEEAqGP1DEpqjrvnsbl2z2UpE1q0A60Q3adBzqw5qYhvKPpfDk5QmkUCeUwqPy9pS+g==", "034fc3f2-1902-4821-8fd9-6aa749895c5c" });
        }
    }
}
