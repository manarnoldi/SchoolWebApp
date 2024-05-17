using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SchoolWebApp.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class MappedStaffDetailsIdcorrectly : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "StaffId",
                table: "Discipline");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "269f0cf3-405e-4163-83f3-1b63ebebd62e",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2024, 5, 17, 21, 5, 42, 910, DateTimeKind.Local).AddTicks(3830), new DateTime(2024, 5, 17, 21, 5, 42, 910, DateTimeKind.Local).AddTicks(3846) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "448df289-142c-4959-a912-60733515e1b4",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2024, 5, 17, 21, 5, 42, 910, DateTimeKind.Local).AddTicks(3775), new DateTime(2024, 5, 17, 21, 5, 42, 910, DateTimeKind.Local).AddTicks(3777) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "48c50c3a-9958-453b-b649-4e21af131322",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2024, 5, 17, 21, 5, 42, 910, DateTimeKind.Local).AddTicks(3736), new DateTime(2024, 5, 17, 21, 5, 42, 910, DateTimeKind.Local).AddTicks(3738) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "717d9b15-a428-440c-b26b-08d3bbb68b02",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2024, 5, 17, 21, 5, 42, 910, DateTimeKind.Local).AddTicks(3540), new DateTime(2024, 5, 17, 21, 5, 42, 910, DateTimeKind.Local).AddTicks(3586) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "95ed2407-3e58-4af2-88a4-1c4e96473f68",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2024, 5, 17, 21, 5, 42, 910, DateTimeKind.Local).AddTicks(3695), new DateTime(2024, 5, 17, 21, 5, 42, 910, DateTimeKind.Local).AddTicks(3697) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "97942bee-ef12-4425-8225-4f293d0f36dd",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2024, 5, 17, 21, 5, 42, 910, DateTimeKind.Local).AddTicks(3937), new DateTime(2024, 5, 17, 21, 5, 42, 910, DateTimeKind.Local).AddTicks(3938) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "cd12b44b-103b-48df-8887-a2bf42e0651e",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2024, 5, 17, 21, 5, 42, 910, DateTimeKind.Local).AddTicks(3898), new DateTime(2024, 5, 17, 21, 5, 42, 910, DateTimeKind.Local).AddTicks(3900) });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "7e67d486-af3e-49f1-a109-a2b864b8e0ec",
                columns: new[] { "ConcurrencyStamp", "Created", "Modified", "PasswordHash", "SecurityStamp" },
                values: new object[] { "b18976e7-000f-40b6-ab8e-a3c1b4e9c63d", new DateTime(2024, 5, 17, 21, 5, 42, 910, DateTimeKind.Local).AddTicks(4075), new DateTime(2024, 5, 17, 21, 5, 42, 910, DateTimeKind.Local).AddTicks(4078), "AQAAAAIAAYagAAAAEFfK1siaccVhQLbUfQKhdeZfWFW7+3Jrvb916Qy6m0JBtMjKOU6Oxeosh+hr3fZZgw==", "8589a589-5a2f-49ee-a385-fb52a72dd178" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "StaffId",
                table: "Discipline",
                type: "int",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "269f0cf3-405e-4163-83f3-1b63ebebd62e",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2024, 5, 16, 7, 27, 46, 445, DateTimeKind.Local).AddTicks(2487), new DateTime(2024, 5, 16, 7, 27, 46, 445, DateTimeKind.Local).AddTicks(2515) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "448df289-142c-4959-a912-60733515e1b4",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2024, 5, 16, 7, 27, 46, 445, DateTimeKind.Local).AddTicks(2367), new DateTime(2024, 5, 16, 7, 27, 46, 445, DateTimeKind.Local).AddTicks(2373) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "48c50c3a-9958-453b-b649-4e21af131322",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2024, 5, 16, 7, 27, 46, 445, DateTimeKind.Local).AddTicks(2242), new DateTime(2024, 5, 16, 7, 27, 46, 445, DateTimeKind.Local).AddTicks(2246) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "717d9b15-a428-440c-b26b-08d3bbb68b02",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2024, 5, 16, 7, 27, 46, 445, DateTimeKind.Local).AddTicks(1652), new DateTime(2024, 5, 16, 7, 27, 46, 445, DateTimeKind.Local).AddTicks(1726) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "95ed2407-3e58-4af2-88a4-1c4e96473f68",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2024, 5, 16, 7, 27, 46, 445, DateTimeKind.Local).AddTicks(2060), new DateTime(2024, 5, 16, 7, 27, 46, 445, DateTimeKind.Local).AddTicks(2068) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "97942bee-ef12-4425-8225-4f293d0f36dd",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2024, 5, 16, 7, 27, 46, 445, DateTimeKind.Local).AddTicks(2774), new DateTime(2024, 5, 16, 7, 27, 46, 445, DateTimeKind.Local).AddTicks(2778) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "cd12b44b-103b-48df-8887-a2bf42e0651e",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2024, 5, 16, 7, 27, 46, 445, DateTimeKind.Local).AddTicks(2653), new DateTime(2024, 5, 16, 7, 27, 46, 445, DateTimeKind.Local).AddTicks(2659) });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "7e67d486-af3e-49f1-a109-a2b864b8e0ec",
                columns: new[] { "ConcurrencyStamp", "Created", "Modified", "PasswordHash", "SecurityStamp" },
                values: new object[] { "1d4cb7d9-46ef-41c0-8269-fc1339afc368", new DateTime(2024, 5, 16, 7, 27, 46, 445, DateTimeKind.Local).AddTicks(3030), new DateTime(2024, 5, 16, 7, 27, 46, 445, DateTimeKind.Local).AddTicks(3038), "AQAAAAIAAYagAAAAELzMiUwhqIaoWn61ePnEAJZ6wj489B25BJ8dyZWh5aigU6SN+SNl11lpzuq8xh9bcg==", "77581d8a-b79b-472a-8946-5f377bd4bc2f" });
        }
    }
}
