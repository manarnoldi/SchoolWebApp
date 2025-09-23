using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SchoolWebApp.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class MadeStaffDetailsIdoptionaltoentityDepartment : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "StaffDetailsId",
                table: "Departments",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "269f0cf3-405e-4163-83f3-1b63ebebd62e",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2025, 9, 22, 7, 8, 37, 322, DateTimeKind.Local).AddTicks(8202), new DateTime(2025, 9, 22, 7, 8, 37, 322, DateTimeKind.Local).AddTicks(8203) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "448df289-142c-4959-a912-60733515e1b4",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2025, 9, 22, 7, 8, 37, 322, DateTimeKind.Local).AddTicks(8179), new DateTime(2025, 9, 22, 7, 8, 37, 322, DateTimeKind.Local).AddTicks(8180) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "48c50c3a-9958-453b-b649-4e21af131322",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2025, 9, 22, 7, 8, 37, 322, DateTimeKind.Local).AddTicks(8146), new DateTime(2025, 9, 22, 7, 8, 37, 322, DateTimeKind.Local).AddTicks(8148) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "717d9b15-a428-440c-b26b-08d3bbb68b02",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2025, 9, 22, 7, 8, 37, 322, DateTimeKind.Local).AddTicks(8048), new DateTime(2025, 9, 22, 7, 8, 37, 322, DateTimeKind.Local).AddTicks(8069) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "95ed2407-3e58-4af2-88a4-1c4e96473f68",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2025, 9, 22, 7, 8, 37, 322, DateTimeKind.Local).AddTicks(8118), new DateTime(2025, 9, 22, 7, 8, 37, 322, DateTimeKind.Local).AddTicks(8120) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "97942bee-ef12-4425-8225-4f293d0f36dd",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2025, 9, 22, 7, 8, 37, 322, DateTimeKind.Local).AddTicks(8256), new DateTime(2025, 9, 22, 7, 8, 37, 322, DateTimeKind.Local).AddTicks(8258) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "cd12b44b-103b-48df-8887-a2bf42e0651e",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2025, 9, 22, 7, 8, 37, 322, DateTimeKind.Local).AddTicks(8226), new DateTime(2025, 9, 22, 7, 8, 37, 322, DateTimeKind.Local).AddTicks(8228) });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "7e67d486-af3e-49f1-a109-a2b864b8e0ec",
                columns: new[] { "ConcurrencyStamp", "Created", "Modified", "PasswordHash", "SecurityStamp" },
                values: new object[] { "224415ca-f5c2-4096-8e91-045a751c46f3", new DateTime(2025, 9, 22, 7, 8, 37, 322, DateTimeKind.Local).AddTicks(8538), new DateTime(2025, 9, 22, 7, 8, 37, 322, DateTimeKind.Local).AddTicks(8540), "AQAAAAIAAYagAAAAEKTocC7ziVXHYzcW67d5NXfEohoKjj+pl3qDd3XLojIdDq88+Gi0fEFJsl5PBQZiIA==", "474fcf4c-f10e-4ee4-bbf5-a4753e5f916e" });

            migrationBuilder.UpdateData(
                table: "Designations",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2025, 9, 22, 7, 8, 37, 322, DateTimeKind.Local).AddTicks(8325), new DateTime(2025, 9, 22, 7, 8, 37, 322, DateTimeKind.Local).AddTicks(8326) });

            migrationBuilder.UpdateData(
                table: "EmploymentTypes",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2025, 9, 22, 7, 8, 37, 322, DateTimeKind.Local).AddTicks(8355), new DateTime(2025, 9, 22, 7, 8, 37, 322, DateTimeKind.Local).AddTicks(8355) });

            migrationBuilder.UpdateData(
                table: "Genders",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2025, 9, 22, 7, 8, 37, 322, DateTimeKind.Local).AddTicks(8420), new DateTime(2025, 9, 22, 7, 8, 37, 322, DateTimeKind.Local).AddTicks(8421) });

            migrationBuilder.UpdateData(
                table: "Nationalities",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2025, 9, 22, 7, 8, 37, 322, DateTimeKind.Local).AddTicks(8381), new DateTime(2025, 9, 22, 7, 8, 37, 322, DateTimeKind.Local).AddTicks(8381) });

            migrationBuilder.UpdateData(
                table: "Person",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "DateOfBirth", "EmploymentDate", "EndofEmploymentDate", "Modified" },
                values: new object[] { new DateTime(2025, 9, 22, 7, 8, 37, 322, DateTimeKind.Local).AddTicks(8459), new DateTime(2025, 9, 22, 7, 8, 37, 322, DateTimeKind.Local).AddTicks(8456), new DateTime(2025, 9, 22, 7, 8, 37, 322, DateTimeKind.Local).AddTicks(8451), new DateTime(2025, 9, 22, 7, 8, 37, 322, DateTimeKind.Local).AddTicks(8452), new DateTime(2025, 9, 22, 7, 8, 37, 322, DateTimeKind.Local).AddTicks(8459) });

            migrationBuilder.UpdateData(
                table: "Religions",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2025, 9, 22, 7, 8, 37, 322, DateTimeKind.Local).AddTicks(8399), new DateTime(2025, 9, 22, 7, 8, 37, 322, DateTimeKind.Local).AddTicks(8400) });

            migrationBuilder.UpdateData(
                table: "StaffCategories",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2025, 9, 22, 7, 8, 37, 322, DateTimeKind.Local).AddTicks(8294), new DateTime(2025, 9, 22, 7, 8, 37, 322, DateTimeKind.Local).AddTicks(8296) });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "StaffDetailsId",
                table: "Departments",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "269f0cf3-405e-4163-83f3-1b63ebebd62e",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2025, 9, 22, 6, 28, 3, 623, DateTimeKind.Local).AddTicks(8449), new DateTime(2025, 9, 22, 6, 28, 3, 623, DateTimeKind.Local).AddTicks(8450) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "448df289-142c-4959-a912-60733515e1b4",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2025, 9, 22, 6, 28, 3, 623, DateTimeKind.Local).AddTicks(8432), new DateTime(2025, 9, 22, 6, 28, 3, 623, DateTimeKind.Local).AddTicks(8433) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "48c50c3a-9958-453b-b649-4e21af131322",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2025, 9, 22, 6, 28, 3, 623, DateTimeKind.Local).AddTicks(8409), new DateTime(2025, 9, 22, 6, 28, 3, 623, DateTimeKind.Local).AddTicks(8410) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "717d9b15-a428-440c-b26b-08d3bbb68b02",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2025, 9, 22, 6, 28, 3, 623, DateTimeKind.Local).AddTicks(8334), new DateTime(2025, 9, 22, 6, 28, 3, 623, DateTimeKind.Local).AddTicks(8353) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "95ed2407-3e58-4af2-88a4-1c4e96473f68",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2025, 9, 22, 6, 28, 3, 623, DateTimeKind.Local).AddTicks(8390), new DateTime(2025, 9, 22, 6, 28, 3, 623, DateTimeKind.Local).AddTicks(8392) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "97942bee-ef12-4425-8225-4f293d0f36dd",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2025, 9, 22, 6, 28, 3, 623, DateTimeKind.Local).AddTicks(8483), new DateTime(2025, 9, 22, 6, 28, 3, 623, DateTimeKind.Local).AddTicks(8485) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "cd12b44b-103b-48df-8887-a2bf42e0651e",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2025, 9, 22, 6, 28, 3, 623, DateTimeKind.Local).AddTicks(8466), new DateTime(2025, 9, 22, 6, 28, 3, 623, DateTimeKind.Local).AddTicks(8468) });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "7e67d486-af3e-49f1-a109-a2b864b8e0ec",
                columns: new[] { "ConcurrencyStamp", "Created", "Modified", "PasswordHash", "SecurityStamp" },
                values: new object[] { "9daa67a0-a096-4b61-981f-b5c7e634c7b8", new DateTime(2025, 9, 22, 6, 28, 3, 623, DateTimeKind.Local).AddTicks(8731), new DateTime(2025, 9, 22, 6, 28, 3, 623, DateTimeKind.Local).AddTicks(8733), "AQAAAAIAAYagAAAAEEmhztvf3HL+VlPwyZrY67QI9XXETjrrndDVu4n2YfhrpzoTe8Xf8z15QXUMWr7V4Q==", "3b3075c1-968a-48a5-ac36-9e2183f5ac6b" });

            migrationBuilder.UpdateData(
                table: "Designations",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2025, 9, 22, 6, 28, 3, 623, DateTimeKind.Local).AddTicks(8530), new DateTime(2025, 9, 22, 6, 28, 3, 623, DateTimeKind.Local).AddTicks(8531) });

            migrationBuilder.UpdateData(
                table: "EmploymentTypes",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2025, 9, 22, 6, 28, 3, 623, DateTimeKind.Local).AddTicks(8551), new DateTime(2025, 9, 22, 6, 28, 3, 623, DateTimeKind.Local).AddTicks(8551) });

            migrationBuilder.UpdateData(
                table: "Genders",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2025, 9, 22, 6, 28, 3, 623, DateTimeKind.Local).AddTicks(8608), new DateTime(2025, 9, 22, 6, 28, 3, 623, DateTimeKind.Local).AddTicks(8608) });

            migrationBuilder.UpdateData(
                table: "Nationalities",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2025, 9, 22, 6, 28, 3, 623, DateTimeKind.Local).AddTicks(8573), new DateTime(2025, 9, 22, 6, 28, 3, 623, DateTimeKind.Local).AddTicks(8574) });

            migrationBuilder.UpdateData(
                table: "Person",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "DateOfBirth", "EmploymentDate", "EndofEmploymentDate", "Modified" },
                values: new object[] { new DateTime(2025, 9, 22, 6, 28, 3, 623, DateTimeKind.Local).AddTicks(8644), new DateTime(2025, 9, 22, 6, 28, 3, 623, DateTimeKind.Local).AddTicks(8640), new DateTime(2025, 9, 22, 6, 28, 3, 623, DateTimeKind.Local).AddTicks(8634), new DateTime(2025, 9, 22, 6, 28, 3, 623, DateTimeKind.Local).AddTicks(8635), new DateTime(2025, 9, 22, 6, 28, 3, 623, DateTimeKind.Local).AddTicks(8645) });

            migrationBuilder.UpdateData(
                table: "Religions",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2025, 9, 22, 6, 28, 3, 623, DateTimeKind.Local).AddTicks(8589), new DateTime(2025, 9, 22, 6, 28, 3, 623, DateTimeKind.Local).AddTicks(8589) });

            migrationBuilder.UpdateData(
                table: "StaffCategories",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2025, 9, 22, 6, 28, 3, 623, DateTimeKind.Local).AddTicks(8513), new DateTime(2025, 9, 22, 6, 28, 3, 623, DateTimeKind.Local).AddTicks(8515) });
        }
    }
}
