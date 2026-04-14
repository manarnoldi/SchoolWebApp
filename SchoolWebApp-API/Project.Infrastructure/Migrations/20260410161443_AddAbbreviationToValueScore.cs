using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SchoolWebApp.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddAbbreviationToValueScore : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Abbreviation",
                table: "ValueScores",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Abbreviation",
                table: "ValueScores");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "269f0cf3-405e-4163-83f3-1b63ebebd62e",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 10, 8, 29, 57, 771, DateTimeKind.Local).AddTicks(9806), new DateTime(2026, 4, 10, 8, 29, 57, 771, DateTimeKind.Local).AddTicks(9808) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "448df289-142c-4959-a912-60733515e1b4",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 10, 8, 29, 57, 771, DateTimeKind.Local).AddTicks(9757), new DateTime(2026, 4, 10, 8, 29, 57, 771, DateTimeKind.Local).AddTicks(9759) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "48c50c3a-9958-453b-b649-4e21af131322",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 10, 8, 29, 57, 771, DateTimeKind.Local).AddTicks(9703), new DateTime(2026, 4, 10, 8, 29, 57, 771, DateTimeKind.Local).AddTicks(9705) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "717d9b15-a428-440c-b26b-08d3bbb68b02",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 10, 8, 29, 57, 771, DateTimeKind.Local).AddTicks(9535), new DateTime(2026, 4, 10, 8, 29, 57, 771, DateTimeKind.Local).AddTicks(9553) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "95ed2407-3e58-4af2-88a4-1c4e96473f68",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 10, 8, 29, 57, 771, DateTimeKind.Local).AddTicks(9645), new DateTime(2026, 4, 10, 8, 29, 57, 771, DateTimeKind.Local).AddTicks(9648) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "97942bee-ef12-4425-8225-4f293d0f36dd",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 10, 8, 29, 57, 771, DateTimeKind.Local).AddTicks(9910), new DateTime(2026, 4, 10, 8, 29, 57, 771, DateTimeKind.Local).AddTicks(9912) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "cd12b44b-103b-48df-8887-a2bf42e0651e",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 10, 8, 29, 57, 771, DateTimeKind.Local).AddTicks(9860), new DateTime(2026, 4, 10, 8, 29, 57, 771, DateTimeKind.Local).AddTicks(9862) });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "7e67d486-af3e-49f1-a109-a2b864b8e0ec",
                columns: new[] { "ConcurrencyStamp", "Created", "Modified", "PasswordHash", "SecurityStamp" },
                values: new object[] { "b304ba0b-a947-4073-9a34-0e5a5a281fe5", new DateTime(2026, 4, 10, 8, 29, 57, 772, DateTimeKind.Local).AddTicks(369), new DateTime(2026, 4, 10, 8, 29, 57, 772, DateTimeKind.Local).AddTicks(372), "AQAAAAIAAYagAAAAEKgyk0q9TmX+hPiJdBMujBUuVLnvkIG2JPUNvfapmEYMIwzrMoXuo4Cdja5zbo//Zg==", "fff66614-b00d-4d6e-bb4a-d1593dc3802d" });

            migrationBuilder.UpdateData(
                table: "Designations",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 10, 8, 29, 57, 772, DateTimeKind.Local).AddTicks(8), new DateTime(2026, 4, 10, 8, 29, 57, 772, DateTimeKind.Local).AddTicks(10) });

            migrationBuilder.UpdateData(
                table: "EmploymentTypes",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 10, 8, 29, 57, 772, DateTimeKind.Local).AddTicks(63), new DateTime(2026, 4, 10, 8, 29, 57, 772, DateTimeKind.Local).AddTicks(65) });

            migrationBuilder.UpdateData(
                table: "Genders",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 10, 8, 29, 57, 772, DateTimeKind.Local).AddTicks(188), new DateTime(2026, 4, 10, 8, 29, 57, 772, DateTimeKind.Local).AddTicks(190) });

            migrationBuilder.UpdateData(
                table: "Nationalities",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 10, 8, 29, 57, 772, DateTimeKind.Local).AddTicks(107), new DateTime(2026, 4, 10, 8, 29, 57, 772, DateTimeKind.Local).AddTicks(109) });

            migrationBuilder.UpdateData(
                table: "Person",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "DateOfBirth", "EmploymentDate", "EndofEmploymentDate", "Modified" },
                values: new object[] { new DateTime(2026, 4, 10, 8, 29, 57, 772, DateTimeKind.Local).AddTicks(263), new DateTime(2026, 4, 10, 8, 29, 57, 772, DateTimeKind.Local).AddTicks(260), new DateTime(2026, 4, 10, 8, 29, 57, 772, DateTimeKind.Local).AddTicks(254), new DateTime(2026, 4, 10, 8, 29, 57, 772, DateTimeKind.Local).AddTicks(256), new DateTime(2026, 4, 10, 8, 29, 57, 772, DateTimeKind.Local).AddTicks(267) });

            migrationBuilder.UpdateData(
                table: "Religions",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 10, 8, 29, 57, 772, DateTimeKind.Local).AddTicks(152), new DateTime(2026, 4, 10, 8, 29, 57, 772, DateTimeKind.Local).AddTicks(154) });

            migrationBuilder.UpdateData(
                table: "StaffCategories",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 10, 8, 29, 57, 771, DateTimeKind.Local).AddTicks(9966), new DateTime(2026, 4, 10, 8, 29, 57, 771, DateTimeKind.Local).AddTicks(9969) });
        }
    }
}
