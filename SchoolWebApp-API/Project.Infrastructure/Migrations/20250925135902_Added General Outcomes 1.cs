using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SchoolWebApp.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddedGeneralOutcomes1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "269f0cf3-405e-4163-83f3-1b63ebebd62e",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2025, 9, 25, 16, 59, 1, 423, DateTimeKind.Local).AddTicks(7400), new DateTime(2025, 9, 25, 16, 59, 1, 423, DateTimeKind.Local).AddTicks(7402) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "448df289-142c-4959-a912-60733515e1b4",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2025, 9, 25, 16, 59, 1, 423, DateTimeKind.Local).AddTicks(7384), new DateTime(2025, 9, 25, 16, 59, 1, 423, DateTimeKind.Local).AddTicks(7386) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "48c50c3a-9958-453b-b649-4e21af131322",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2025, 9, 25, 16, 59, 1, 423, DateTimeKind.Local).AddTicks(7366), new DateTime(2025, 9, 25, 16, 59, 1, 423, DateTimeKind.Local).AddTicks(7368) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "717d9b15-a428-440c-b26b-08d3bbb68b02",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2025, 9, 25, 16, 59, 1, 423, DateTimeKind.Local).AddTicks(7291), new DateTime(2025, 9, 25, 16, 59, 1, 423, DateTimeKind.Local).AddTicks(7306) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "95ed2407-3e58-4af2-88a4-1c4e96473f68",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2025, 9, 25, 16, 59, 1, 423, DateTimeKind.Local).AddTicks(7347), new DateTime(2025, 9, 25, 16, 59, 1, 423, DateTimeKind.Local).AddTicks(7348) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "97942bee-ef12-4425-8225-4f293d0f36dd",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2025, 9, 25, 16, 59, 1, 423, DateTimeKind.Local).AddTicks(7442), new DateTime(2025, 9, 25, 16, 59, 1, 423, DateTimeKind.Local).AddTicks(7443) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "cd12b44b-103b-48df-8887-a2bf42e0651e",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2025, 9, 25, 16, 59, 1, 423, DateTimeKind.Local).AddTicks(7425), new DateTime(2025, 9, 25, 16, 59, 1, 423, DateTimeKind.Local).AddTicks(7427) });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "7e67d486-af3e-49f1-a109-a2b864b8e0ec",
                columns: new[] { "ConcurrencyStamp", "Created", "Modified", "PasswordHash", "SecurityStamp" },
                values: new object[] { "3cfa52af-4d8a-419d-b869-36d6fae607c2", new DateTime(2025, 9, 25, 16, 59, 1, 423, DateTimeKind.Local).AddTicks(7648), new DateTime(2025, 9, 25, 16, 59, 1, 423, DateTimeKind.Local).AddTicks(7650), "AQAAAAIAAYagAAAAEG1AFv8HdZPLzYVRwo7fEzBrDHmu+KsNSyzASnZF8aWt8beJpgaZX6liarnEp09y8A==", "c8ee4ad0-a90e-4037-9a24-e1643ce668f0" });

            migrationBuilder.UpdateData(
                table: "Designations",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2025, 9, 25, 16, 59, 1, 423, DateTimeKind.Local).AddTicks(7492), new DateTime(2025, 9, 25, 16, 59, 1, 423, DateTimeKind.Local).AddTicks(7492) });

            migrationBuilder.UpdateData(
                table: "EmploymentTypes",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2025, 9, 25, 16, 59, 1, 423, DateTimeKind.Local).AddTicks(7511), new DateTime(2025, 9, 25, 16, 59, 1, 423, DateTimeKind.Local).AddTicks(7511) });

            migrationBuilder.UpdateData(
                table: "Genders",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2025, 9, 25, 16, 59, 1, 423, DateTimeKind.Local).AddTicks(7562), new DateTime(2025, 9, 25, 16, 59, 1, 423, DateTimeKind.Local).AddTicks(7563) });

            migrationBuilder.UpdateData(
                table: "Nationalities",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2025, 9, 25, 16, 59, 1, 423, DateTimeKind.Local).AddTicks(7529), new DateTime(2025, 9, 25, 16, 59, 1, 423, DateTimeKind.Local).AddTicks(7529) });

            migrationBuilder.UpdateData(
                table: "Person",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "DateOfBirth", "EmploymentDate", "EndofEmploymentDate", "Modified" },
                values: new object[] { new DateTime(2025, 9, 25, 16, 59, 1, 423, DateTimeKind.Local).AddTicks(7588), new DateTime(2025, 9, 25, 16, 59, 1, 423, DateTimeKind.Local).AddTicks(7586), new DateTime(2025, 9, 25, 16, 59, 1, 423, DateTimeKind.Local).AddTicks(7583), new DateTime(2025, 9, 25, 16, 59, 1, 423, DateTimeKind.Local).AddTicks(7583), new DateTime(2025, 9, 25, 16, 59, 1, 423, DateTimeKind.Local).AddTicks(7589) });

            migrationBuilder.UpdateData(
                table: "Religions",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2025, 9, 25, 16, 59, 1, 423, DateTimeKind.Local).AddTicks(7548), new DateTime(2025, 9, 25, 16, 59, 1, 423, DateTimeKind.Local).AddTicks(7549) });

            migrationBuilder.UpdateData(
                table: "StaffCategories",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2025, 9, 25, 16, 59, 1, 423, DateTimeKind.Local).AddTicks(7474), new DateTime(2025, 9, 25, 16, 59, 1, 423, DateTimeKind.Local).AddTicks(7475) });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "269f0cf3-405e-4163-83f3-1b63ebebd62e",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2025, 9, 25, 16, 54, 39, 444, DateTimeKind.Local).AddTicks(2520), new DateTime(2025, 9, 25, 16, 54, 39, 444, DateTimeKind.Local).AddTicks(2523) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "448df289-142c-4959-a912-60733515e1b4",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2025, 9, 25, 16, 54, 39, 444, DateTimeKind.Local).AddTicks(2487), new DateTime(2025, 9, 25, 16, 54, 39, 444, DateTimeKind.Local).AddTicks(2490) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "48c50c3a-9958-453b-b649-4e21af131322",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2025, 9, 25, 16, 54, 39, 444, DateTimeKind.Local).AddTicks(2450), new DateTime(2025, 9, 25, 16, 54, 39, 444, DateTimeKind.Local).AddTicks(2453) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "717d9b15-a428-440c-b26b-08d3bbb68b02",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2025, 9, 25, 16, 54, 39, 444, DateTimeKind.Local).AddTicks(2327), new DateTime(2025, 9, 25, 16, 54, 39, 444, DateTimeKind.Local).AddTicks(2346) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "95ed2407-3e58-4af2-88a4-1c4e96473f68",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2025, 9, 25, 16, 54, 39, 444, DateTimeKind.Local).AddTicks(2414), new DateTime(2025, 9, 25, 16, 54, 39, 444, DateTimeKind.Local).AddTicks(2417) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "97942bee-ef12-4425-8225-4f293d0f36dd",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2025, 9, 25, 16, 54, 39, 444, DateTimeKind.Local).AddTicks(2596), new DateTime(2025, 9, 25, 16, 54, 39, 444, DateTimeKind.Local).AddTicks(2599) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "cd12b44b-103b-48df-8887-a2bf42e0651e",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2025, 9, 25, 16, 54, 39, 444, DateTimeKind.Local).AddTicks(2556), new DateTime(2025, 9, 25, 16, 54, 39, 444, DateTimeKind.Local).AddTicks(2559) });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "7e67d486-af3e-49f1-a109-a2b864b8e0ec",
                columns: new[] { "ConcurrencyStamp", "Created", "Modified", "PasswordHash", "SecurityStamp" },
                values: new object[] { "ae9173e1-9017-40ad-88b7-d0aa1396c8f3", new DateTime(2025, 9, 25, 16, 54, 39, 444, DateTimeKind.Local).AddTicks(3098), new DateTime(2025, 9, 25, 16, 54, 39, 444, DateTimeKind.Local).AddTicks(3101), "AQAAAAIAAYagAAAAEClR0+4Thw+uvYQlWCmku0aXjR2p2lVkMrhHdLDonua5T5cYz1mKFUvUxpc1k5uAYQ==", "a9f54644-7441-4d8a-a5b8-5d2e424f81b2" });

            migrationBuilder.UpdateData(
                table: "Designations",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2025, 9, 25, 16, 54, 39, 444, DateTimeKind.Local).AddTicks(2675), new DateTime(2025, 9, 25, 16, 54, 39, 444, DateTimeKind.Local).AddTicks(2676) });

            migrationBuilder.UpdateData(
                table: "EmploymentTypes",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2025, 9, 25, 16, 54, 39, 444, DateTimeKind.Local).AddTicks(2713), new DateTime(2025, 9, 25, 16, 54, 39, 444, DateTimeKind.Local).AddTicks(2714) });

            migrationBuilder.UpdateData(
                table: "Genders",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2025, 9, 25, 16, 54, 39, 444, DateTimeKind.Local).AddTicks(2817), new DateTime(2025, 9, 25, 16, 54, 39, 444, DateTimeKind.Local).AddTicks(2818) });

            migrationBuilder.UpdateData(
                table: "Nationalities",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2025, 9, 25, 16, 54, 39, 444, DateTimeKind.Local).AddTicks(2747), new DateTime(2025, 9, 25, 16, 54, 39, 444, DateTimeKind.Local).AddTicks(2748) });

            migrationBuilder.UpdateData(
                table: "Person",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "DateOfBirth", "EmploymentDate", "EndofEmploymentDate", "Modified" },
                values: new object[] { new DateTime(2025, 9, 25, 16, 54, 39, 444, DateTimeKind.Local).AddTicks(2871), new DateTime(2025, 9, 25, 16, 54, 39, 444, DateTimeKind.Local).AddTicks(2864), new DateTime(2025, 9, 25, 16, 54, 39, 444, DateTimeKind.Local).AddTicks(2858), new DateTime(2025, 9, 25, 16, 54, 39, 444, DateTimeKind.Local).AddTicks(2859), new DateTime(2025, 9, 25, 16, 54, 39, 444, DateTimeKind.Local).AddTicks(2880) });

            migrationBuilder.UpdateData(
                table: "Religions",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2025, 9, 25, 16, 54, 39, 444, DateTimeKind.Local).AddTicks(2786), new DateTime(2025, 9, 25, 16, 54, 39, 444, DateTimeKind.Local).AddTicks(2787) });

            migrationBuilder.UpdateData(
                table: "StaffCategories",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2025, 9, 25, 16, 54, 39, 444, DateTimeKind.Local).AddTicks(2640), new DateTime(2025, 9, 25, 16, 54, 39, 444, DateTimeKind.Local).AddTicks(2643) });
        }
    }
}
