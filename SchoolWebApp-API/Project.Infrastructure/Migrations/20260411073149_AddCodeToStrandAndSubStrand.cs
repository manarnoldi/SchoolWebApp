using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SchoolWebApp.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddCodeToStrandAndSubStrand : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Code",
                table: "SubStrands",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "Code",
                table: "Strands",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "269f0cf3-405e-4163-83f3-1b63ebebd62e",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 11, 10, 31, 47, 521, DateTimeKind.Local).AddTicks(6794), new DateTime(2026, 4, 11, 10, 31, 47, 521, DateTimeKind.Local).AddTicks(6795) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "448df289-142c-4959-a912-60733515e1b4",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 11, 10, 31, 47, 521, DateTimeKind.Local).AddTicks(6775), new DateTime(2026, 4, 11, 10, 31, 47, 521, DateTimeKind.Local).AddTicks(6776) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "48c50c3a-9958-453b-b649-4e21af131322",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 11, 10, 31, 47, 521, DateTimeKind.Local).AddTicks(6755), new DateTime(2026, 4, 11, 10, 31, 47, 521, DateTimeKind.Local).AddTicks(6756) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "717d9b15-a428-440c-b26b-08d3bbb68b02",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 11, 10, 31, 47, 521, DateTimeKind.Local).AddTicks(6674), new DateTime(2026, 4, 11, 10, 31, 47, 521, DateTimeKind.Local).AddTicks(6688) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "95ed2407-3e58-4af2-88a4-1c4e96473f68",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 11, 10, 31, 47, 521, DateTimeKind.Local).AddTicks(6729), new DateTime(2026, 4, 11, 10, 31, 47, 521, DateTimeKind.Local).AddTicks(6731) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "97942bee-ef12-4425-8225-4f293d0f36dd",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 11, 10, 31, 47, 521, DateTimeKind.Local).AddTicks(6847), new DateTime(2026, 4, 11, 10, 31, 47, 521, DateTimeKind.Local).AddTicks(6849) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "cd12b44b-103b-48df-8887-a2bf42e0651e",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 11, 10, 31, 47, 521, DateTimeKind.Local).AddTicks(6827), new DateTime(2026, 4, 11, 10, 31, 47, 521, DateTimeKind.Local).AddTicks(6828) });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "7e67d486-af3e-49f1-a109-a2b864b8e0ec",
                columns: new[] { "ConcurrencyStamp", "Created", "Modified", "PasswordHash", "SecurityStamp" },
                values: new object[] { "e5842f2a-1f4d-4260-9073-aad6290aaec4", new DateTime(2026, 4, 11, 10, 31, 47, 521, DateTimeKind.Local).AddTicks(7160), new DateTime(2026, 4, 11, 10, 31, 47, 521, DateTimeKind.Local).AddTicks(7161), "AQAAAAIAAYagAAAAEIzuNOH1O1r/ezXAS7P7g38cy48kkW3U+Y3ykEvFd2wVvwKYRi7cMSG91ofO3p8M+g==", "4d144fae-b7b2-4b8e-97bb-cc9420541b95" });

            migrationBuilder.UpdateData(
                table: "Designations",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 11, 10, 31, 47, 521, DateTimeKind.Local).AddTicks(6913), new DateTime(2026, 4, 11, 10, 31, 47, 521, DateTimeKind.Local).AddTicks(6914) });

            migrationBuilder.UpdateData(
                table: "EmploymentTypes",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 11, 10, 31, 47, 521, DateTimeKind.Local).AddTicks(6960), new DateTime(2026, 4, 11, 10, 31, 47, 521, DateTimeKind.Local).AddTicks(6961) });

            migrationBuilder.UpdateData(
                table: "Genders",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 11, 10, 31, 47, 521, DateTimeKind.Local).AddTicks(7020), new DateTime(2026, 4, 11, 10, 31, 47, 521, DateTimeKind.Local).AddTicks(7021) });

            migrationBuilder.UpdateData(
                table: "Nationalities",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 11, 10, 31, 47, 521, DateTimeKind.Local).AddTicks(6981), new DateTime(2026, 4, 11, 10, 31, 47, 521, DateTimeKind.Local).AddTicks(6982) });

            migrationBuilder.UpdateData(
                table: "Person",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "DateOfBirth", "EmploymentDate", "EndofEmploymentDate", "Modified" },
                values: new object[] { new DateTime(2026, 4, 11, 10, 31, 47, 521, DateTimeKind.Local).AddTicks(7069), new DateTime(2026, 4, 11, 10, 31, 47, 521, DateTimeKind.Local).AddTicks(7067), new DateTime(2026, 4, 11, 10, 31, 47, 521, DateTimeKind.Local).AddTicks(7062), new DateTime(2026, 4, 11, 10, 31, 47, 521, DateTimeKind.Local).AddTicks(7063), new DateTime(2026, 4, 11, 10, 31, 47, 521, DateTimeKind.Local).AddTicks(7073) });

            migrationBuilder.UpdateData(
                table: "Religions",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 11, 10, 31, 47, 521, DateTimeKind.Local).AddTicks(7002), new DateTime(2026, 4, 11, 10, 31, 47, 521, DateTimeKind.Local).AddTicks(7003) });

            migrationBuilder.UpdateData(
                table: "StaffCategories",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 11, 10, 31, 47, 521, DateTimeKind.Local).AddTicks(6889), new DateTime(2026, 4, 11, 10, 31, 47, 521, DateTimeKind.Local).AddTicks(6891) });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Code",
                table: "SubStrands");

            migrationBuilder.DropColumn(
                name: "Code",
                table: "Strands");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "269f0cf3-405e-4163-83f3-1b63ebebd62e",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 10, 19, 14, 42, 327, DateTimeKind.Local).AddTicks(7999), new DateTime(2026, 4, 10, 19, 14, 42, 327, DateTimeKind.Local).AddTicks(8000) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "448df289-142c-4959-a912-60733515e1b4",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 10, 19, 14, 42, 327, DateTimeKind.Local).AddTicks(7981), new DateTime(2026, 4, 10, 19, 14, 42, 327, DateTimeKind.Local).AddTicks(7982) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "48c50c3a-9958-453b-b649-4e21af131322",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 10, 19, 14, 42, 327, DateTimeKind.Local).AddTicks(7963), new DateTime(2026, 4, 10, 19, 14, 42, 327, DateTimeKind.Local).AddTicks(7964) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "717d9b15-a428-440c-b26b-08d3bbb68b02",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 10, 19, 14, 42, 327, DateTimeKind.Local).AddTicks(7889), new DateTime(2026, 4, 10, 19, 14, 42, 327, DateTimeKind.Local).AddTicks(7899) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "95ed2407-3e58-4af2-88a4-1c4e96473f68",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 10, 19, 14, 42, 327, DateTimeKind.Local).AddTicks(7936), new DateTime(2026, 4, 10, 19, 14, 42, 327, DateTimeKind.Local).AddTicks(7938) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "97942bee-ef12-4425-8225-4f293d0f36dd",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 10, 19, 14, 42, 327, DateTimeKind.Local).AddTicks(8033), new DateTime(2026, 4, 10, 19, 14, 42, 327, DateTimeKind.Local).AddTicks(8035) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "cd12b44b-103b-48df-8887-a2bf42e0651e",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 10, 19, 14, 42, 327, DateTimeKind.Local).AddTicks(8017), new DateTime(2026, 4, 10, 19, 14, 42, 327, DateTimeKind.Local).AddTicks(8018) });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "7e67d486-af3e-49f1-a109-a2b864b8e0ec",
                columns: new[] { "ConcurrencyStamp", "Created", "Modified", "PasswordHash", "SecurityStamp" },
                values: new object[] { "b7eace7a-fb98-4534-bce3-11ebc844b89b", new DateTime(2026, 4, 10, 19, 14, 42, 327, DateTimeKind.Local).AddTicks(8291), new DateTime(2026, 4, 10, 19, 14, 42, 327, DateTimeKind.Local).AddTicks(8292), "AQAAAAIAAYagAAAAEGoejsfVQkC9LRZ+bKBiqOQnjELRmDySDSub91wFfsOYGxeqlVBk3HNLrbol1Kt5hw==", "77de6dea-ede3-4d99-a867-36d30943686d" });

            migrationBuilder.UpdateData(
                table: "Designations",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 10, 19, 14, 42, 327, DateTimeKind.Local).AddTicks(8106), new DateTime(2026, 4, 10, 19, 14, 42, 327, DateTimeKind.Local).AddTicks(8107) });

            migrationBuilder.UpdateData(
                table: "EmploymentTypes",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 10, 19, 14, 42, 327, DateTimeKind.Local).AddTicks(8126), new DateTime(2026, 4, 10, 19, 14, 42, 327, DateTimeKind.Local).AddTicks(8127) });

            migrationBuilder.UpdateData(
                table: "Genders",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 10, 19, 14, 42, 327, DateTimeKind.Local).AddTicks(8192), new DateTime(2026, 4, 10, 19, 14, 42, 327, DateTimeKind.Local).AddTicks(8193) });

            migrationBuilder.UpdateData(
                table: "Nationalities",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 10, 19, 14, 42, 327, DateTimeKind.Local).AddTicks(8146), new DateTime(2026, 4, 10, 19, 14, 42, 327, DateTimeKind.Local).AddTicks(8147) });

            migrationBuilder.UpdateData(
                table: "Person",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "DateOfBirth", "EmploymentDate", "EndofEmploymentDate", "Modified" },
                values: new object[] { new DateTime(2026, 4, 10, 19, 14, 42, 327, DateTimeKind.Local).AddTicks(8229), new DateTime(2026, 4, 10, 19, 14, 42, 327, DateTimeKind.Local).AddTicks(8227), new DateTime(2026, 4, 10, 19, 14, 42, 327, DateTimeKind.Local).AddTicks(8223), new DateTime(2026, 4, 10, 19, 14, 42, 327, DateTimeKind.Local).AddTicks(8224), new DateTime(2026, 4, 10, 19, 14, 42, 327, DateTimeKind.Local).AddTicks(8233) });

            migrationBuilder.UpdateData(
                table: "Religions",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 10, 19, 14, 42, 327, DateTimeKind.Local).AddTicks(8168), new DateTime(2026, 4, 10, 19, 14, 42, 327, DateTimeKind.Local).AddTicks(8168) });

            migrationBuilder.UpdateData(
                table: "StaffCategories",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 10, 19, 14, 42, 327, DateTimeKind.Local).AddTicks(8074), new DateTime(2026, 4, 10, 19, 14, 42, 327, DateTimeKind.Local).AddTicks(8075) });
        }
    }
}
