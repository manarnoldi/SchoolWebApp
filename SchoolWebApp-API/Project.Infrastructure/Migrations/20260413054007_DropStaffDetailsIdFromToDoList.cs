using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SchoolWebApp.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class DropStaffDetailsIdFromToDoList : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ToDoLists_Person_StaffDetailsId",
                table: "ToDoLists");

            migrationBuilder.DropIndex(
                name: "IX_ToDoLists_StaffDetailsId",
                table: "ToDoLists");

            migrationBuilder.DropColumn(
                name: "StaffDetailsId",
                table: "ToDoLists");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "269f0cf3-405e-4163-83f3-1b63ebebd62e",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 13, 8, 40, 6, 333, DateTimeKind.Local).AddTicks(6081), new DateTime(2026, 4, 13, 8, 40, 6, 333, DateTimeKind.Local).AddTicks(6083) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "448df289-142c-4959-a912-60733515e1b4",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 13, 8, 40, 6, 333, DateTimeKind.Local).AddTicks(6058), new DateTime(2026, 4, 13, 8, 40, 6, 333, DateTimeKind.Local).AddTicks(6060) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "48c50c3a-9958-453b-b649-4e21af131322",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 13, 8, 40, 6, 333, DateTimeKind.Local).AddTicks(6023), new DateTime(2026, 4, 13, 8, 40, 6, 333, DateTimeKind.Local).AddTicks(6025) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "717d9b15-a428-440c-b26b-08d3bbb68b02",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 13, 8, 40, 6, 333, DateTimeKind.Local).AddTicks(5916), new DateTime(2026, 4, 13, 8, 40, 6, 333, DateTimeKind.Local).AddTicks(5937) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "95ed2407-3e58-4af2-88a4-1c4e96473f68",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 13, 8, 40, 6, 333, DateTimeKind.Local).AddTicks(5987), new DateTime(2026, 4, 13, 8, 40, 6, 333, DateTimeKind.Local).AddTicks(5988) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "97942bee-ef12-4425-8225-4f293d0f36dd",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 13, 8, 40, 6, 333, DateTimeKind.Local).AddTicks(6131), new DateTime(2026, 4, 13, 8, 40, 6, 333, DateTimeKind.Local).AddTicks(6132) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "cd12b44b-103b-48df-8887-a2bf42e0651e",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 13, 8, 40, 6, 333, DateTimeKind.Local).AddTicks(6108), new DateTime(2026, 4, 13, 8, 40, 6, 333, DateTimeKind.Local).AddTicks(6110) });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "7e67d486-af3e-49f1-a109-a2b864b8e0ec",
                columns: new[] { "ConcurrencyStamp", "Created", "Modified", "PasswordHash", "SecurityStamp" },
                values: new object[] { "c8217ac2-8538-43de-914f-7dde0f2edcdb", new DateTime(2026, 4, 13, 8, 40, 6, 333, DateTimeKind.Local).AddTicks(6425), new DateTime(2026, 4, 13, 8, 40, 6, 333, DateTimeKind.Local).AddTicks(6427), "AQAAAAIAAYagAAAAEOA2nKs+3xXFg3EwDh1mR8WbeyGvDdGBQwrB2GuZUJSdtGrpdycUYb6tc/GOyvRgMg==", "86ac6180-3d87-4ef6-9dc2-1eed9b8e6fe7" });

            migrationBuilder.UpdateData(
                table: "Designations",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 13, 8, 40, 6, 333, DateTimeKind.Local).AddTicks(6194), new DateTime(2026, 4, 13, 8, 40, 6, 333, DateTimeKind.Local).AddTicks(6195) });

            migrationBuilder.UpdateData(
                table: "EmploymentTypes",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 13, 8, 40, 6, 333, DateTimeKind.Local).AddTicks(6225), new DateTime(2026, 4, 13, 8, 40, 6, 333, DateTimeKind.Local).AddTicks(6226) });

            migrationBuilder.UpdateData(
                table: "Genders",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 13, 8, 40, 6, 333, DateTimeKind.Local).AddTicks(6295), new DateTime(2026, 4, 13, 8, 40, 6, 333, DateTimeKind.Local).AddTicks(6295) });

            migrationBuilder.UpdateData(
                table: "Nationalities",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 13, 8, 40, 6, 333, DateTimeKind.Local).AddTicks(6250), new DateTime(2026, 4, 13, 8, 40, 6, 333, DateTimeKind.Local).AddTicks(6251) });

            migrationBuilder.UpdateData(
                table: "Person",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "DateOfBirth", "EmploymentDate", "EndofEmploymentDate", "Modified" },
                values: new object[] { new DateTime(2026, 4, 13, 8, 40, 6, 333, DateTimeKind.Local).AddTicks(6342), new DateTime(2026, 4, 13, 8, 40, 6, 333, DateTimeKind.Local).AddTicks(6338), new DateTime(2026, 4, 13, 8, 40, 6, 333, DateTimeKind.Local).AddTicks(6334), new DateTime(2026, 4, 13, 8, 40, 6, 333, DateTimeKind.Local).AddTicks(6335), new DateTime(2026, 4, 13, 8, 40, 6, 333, DateTimeKind.Local).AddTicks(6343) });

            migrationBuilder.UpdateData(
                table: "Religions",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 13, 8, 40, 6, 333, DateTimeKind.Local).AddTicks(6274), new DateTime(2026, 4, 13, 8, 40, 6, 333, DateTimeKind.Local).AddTicks(6274) });

            migrationBuilder.UpdateData(
                table: "StaffCategories",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 13, 8, 40, 6, 333, DateTimeKind.Local).AddTicks(6171), new DateTime(2026, 4, 13, 8, 40, 6, 333, DateTimeKind.Local).AddTicks(6173) });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "StaffDetailsId",
                table: "ToDoLists",
                type: "int",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "269f0cf3-405e-4163-83f3-1b63ebebd62e",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 13, 8, 28, 42, 835, DateTimeKind.Local).AddTicks(3659), new DateTime(2026, 4, 13, 8, 28, 42, 835, DateTimeKind.Local).AddTicks(3661) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "448df289-142c-4959-a912-60733515e1b4",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 13, 8, 28, 42, 835, DateTimeKind.Local).AddTicks(3625), new DateTime(2026, 4, 13, 8, 28, 42, 835, DateTimeKind.Local).AddTicks(3626) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "48c50c3a-9958-453b-b649-4e21af131322",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 13, 8, 28, 42, 835, DateTimeKind.Local).AddTicks(3590), new DateTime(2026, 4, 13, 8, 28, 42, 835, DateTimeKind.Local).AddTicks(3592) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "717d9b15-a428-440c-b26b-08d3bbb68b02",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 13, 8, 28, 42, 835, DateTimeKind.Local).AddTicks(3438), new DateTime(2026, 4, 13, 8, 28, 42, 835, DateTimeKind.Local).AddTicks(3463) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "95ed2407-3e58-4af2-88a4-1c4e96473f68",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 13, 8, 28, 42, 835, DateTimeKind.Local).AddTicks(3552), new DateTime(2026, 4, 13, 8, 28, 42, 835, DateTimeKind.Local).AddTicks(3554) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "97942bee-ef12-4425-8225-4f293d0f36dd",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 13, 8, 28, 42, 835, DateTimeKind.Local).AddTicks(3739), new DateTime(2026, 4, 13, 8, 28, 42, 835, DateTimeKind.Local).AddTicks(3740) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "cd12b44b-103b-48df-8887-a2bf42e0651e",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 13, 8, 28, 42, 835, DateTimeKind.Local).AddTicks(3694), new DateTime(2026, 4, 13, 8, 28, 42, 835, DateTimeKind.Local).AddTicks(3696) });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "7e67d486-af3e-49f1-a109-a2b864b8e0ec",
                columns: new[] { "ConcurrencyStamp", "Created", "Modified", "PasswordHash", "SecurityStamp" },
                values: new object[] { "abb2f32b-fdd7-4d72-93ef-e4322112c0bf", new DateTime(2026, 4, 13, 8, 28, 42, 835, DateTimeKind.Local).AddTicks(4227), new DateTime(2026, 4, 13, 8, 28, 42, 835, DateTimeKind.Local).AddTicks(4228), "AQAAAAIAAYagAAAAELwilftTB9dlvFZ34yT2B3CB/ulKQfTJGxTLbSb0kbiaC3HvmldhHaaxdUcpVIwgEA==", "2e04b2c7-d313-4292-a1a4-d42fcfe090bb" });

            migrationBuilder.UpdateData(
                table: "Designations",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 13, 8, 28, 42, 835, DateTimeKind.Local).AddTicks(3836), new DateTime(2026, 4, 13, 8, 28, 42, 835, DateTimeKind.Local).AddTicks(3838) });

            migrationBuilder.UpdateData(
                table: "EmploymentTypes",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 13, 8, 28, 42, 835, DateTimeKind.Local).AddTicks(3876), new DateTime(2026, 4, 13, 8, 28, 42, 835, DateTimeKind.Local).AddTicks(3878) });

            migrationBuilder.UpdateData(
                table: "Genders",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 13, 8, 28, 42, 835, DateTimeKind.Local).AddTicks(4046), new DateTime(2026, 4, 13, 8, 28, 42, 835, DateTimeKind.Local).AddTicks(4048) });

            migrationBuilder.UpdateData(
                table: "Nationalities",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 13, 8, 28, 42, 835, DateTimeKind.Local).AddTicks(3931), new DateTime(2026, 4, 13, 8, 28, 42, 835, DateTimeKind.Local).AddTicks(3934) });

            migrationBuilder.UpdateData(
                table: "Person",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "DateOfBirth", "EmploymentDate", "EndofEmploymentDate", "Modified" },
                values: new object[] { new DateTime(2026, 4, 13, 8, 28, 42, 835, DateTimeKind.Local).AddTicks(4107), new DateTime(2026, 4, 13, 8, 28, 42, 835, DateTimeKind.Local).AddTicks(4103), new DateTime(2026, 4, 13, 8, 28, 42, 835, DateTimeKind.Local).AddTicks(4097), new DateTime(2026, 4, 13, 8, 28, 42, 835, DateTimeKind.Local).AddTicks(4098), new DateTime(2026, 4, 13, 8, 28, 42, 835, DateTimeKind.Local).AddTicks(4112) });

            migrationBuilder.UpdateData(
                table: "Religions",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 13, 8, 28, 42, 835, DateTimeKind.Local).AddTicks(4014), new DateTime(2026, 4, 13, 8, 28, 42, 835, DateTimeKind.Local).AddTicks(4016) });

            migrationBuilder.UpdateData(
                table: "StaffCategories",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 13, 8, 28, 42, 835, DateTimeKind.Local).AddTicks(3794), new DateTime(2026, 4, 13, 8, 28, 42, 835, DateTimeKind.Local).AddTicks(3797) });

            migrationBuilder.CreateIndex(
                name: "IX_ToDoLists_StaffDetailsId",
                table: "ToDoLists",
                column: "StaffDetailsId");

            migrationBuilder.AddForeignKey(
                name: "FK_ToDoLists_Person_StaffDetailsId",
                table: "ToDoLists",
                column: "StaffDetailsId",
                principalTable: "Person",
                principalColumn: "Id");
        }
    }
}
