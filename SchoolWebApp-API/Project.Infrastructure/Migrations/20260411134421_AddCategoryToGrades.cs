using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SchoolWebApp.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddCategoryToGrades : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Category",
                table: "Grades",
                type: "varchar(50)",
                maxLength: 50,
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "269f0cf3-405e-4163-83f3-1b63ebebd62e",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 11, 16, 44, 19, 857, DateTimeKind.Local).AddTicks(6986), new DateTime(2026, 4, 11, 16, 44, 19, 857, DateTimeKind.Local).AddTicks(6988) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "448df289-142c-4959-a912-60733515e1b4",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 11, 16, 44, 19, 857, DateTimeKind.Local).AddTicks(6968), new DateTime(2026, 4, 11, 16, 44, 19, 857, DateTimeKind.Local).AddTicks(6969) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "48c50c3a-9958-453b-b649-4e21af131322",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 11, 16, 44, 19, 857, DateTimeKind.Local).AddTicks(6949), new DateTime(2026, 4, 11, 16, 44, 19, 857, DateTimeKind.Local).AddTicks(6950) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "717d9b15-a428-440c-b26b-08d3bbb68b02",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 11, 16, 44, 19, 857, DateTimeKind.Local).AddTicks(6867), new DateTime(2026, 4, 11, 16, 44, 19, 857, DateTimeKind.Local).AddTicks(6889) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "95ed2407-3e58-4af2-88a4-1c4e96473f68",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 11, 16, 44, 19, 857, DateTimeKind.Local).AddTicks(6928), new DateTime(2026, 4, 11, 16, 44, 19, 857, DateTimeKind.Local).AddTicks(6929) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "97942bee-ef12-4425-8225-4f293d0f36dd",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 11, 16, 44, 19, 857, DateTimeKind.Local).AddTicks(7029), new DateTime(2026, 4, 11, 16, 44, 19, 857, DateTimeKind.Local).AddTicks(7030) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "cd12b44b-103b-48df-8887-a2bf42e0651e",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 11, 16, 44, 19, 857, DateTimeKind.Local).AddTicks(7005), new DateTime(2026, 4, 11, 16, 44, 19, 857, DateTimeKind.Local).AddTicks(7007) });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "7e67d486-af3e-49f1-a109-a2b864b8e0ec",
                columns: new[] { "ConcurrencyStamp", "Created", "Modified", "PasswordHash", "SecurityStamp" },
                values: new object[] { "b24c1b74-f9be-4878-a16b-072c8bd138e3", new DateTime(2026, 4, 11, 16, 44, 19, 857, DateTimeKind.Local).AddTicks(7296), new DateTime(2026, 4, 11, 16, 44, 19, 857, DateTimeKind.Local).AddTicks(7298), "AQAAAAIAAYagAAAAENk1tVXH3lpMj5HZdLoDUoN2ORls4icB28mcJzxHEETKU1207tW2tCB14tpN0/fm7g==", "d27aec11-755c-4fdb-a21e-85fc52896d1e" });

            migrationBuilder.UpdateData(
                table: "Designations",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 11, 16, 44, 19, 857, DateTimeKind.Local).AddTicks(7082), new DateTime(2026, 4, 11, 16, 44, 19, 857, DateTimeKind.Local).AddTicks(7083) });

            migrationBuilder.UpdateData(
                table: "EmploymentTypes",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 11, 16, 44, 19, 857, DateTimeKind.Local).AddTicks(7127), new DateTime(2026, 4, 11, 16, 44, 19, 857, DateTimeKind.Local).AddTicks(7128) });

            migrationBuilder.UpdateData(
                table: "Genders",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 11, 16, 44, 19, 857, DateTimeKind.Local).AddTicks(7185), new DateTime(2026, 4, 11, 16, 44, 19, 857, DateTimeKind.Local).AddTicks(7185) });

            migrationBuilder.UpdateData(
                table: "Nationalities",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 11, 16, 44, 19, 857, DateTimeKind.Local).AddTicks(7148), new DateTime(2026, 4, 11, 16, 44, 19, 857, DateTimeKind.Local).AddTicks(7149) });

            migrationBuilder.UpdateData(
                table: "Person",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "DateOfBirth", "EmploymentDate", "EndofEmploymentDate", "Modified" },
                values: new object[] { new DateTime(2026, 4, 11, 16, 44, 19, 857, DateTimeKind.Local).AddTicks(7224), new DateTime(2026, 4, 11, 16, 44, 19, 857, DateTimeKind.Local).AddTicks(7221), new DateTime(2026, 4, 11, 16, 44, 19, 857, DateTimeKind.Local).AddTicks(7218), new DateTime(2026, 4, 11, 16, 44, 19, 857, DateTimeKind.Local).AddTicks(7218), new DateTime(2026, 4, 11, 16, 44, 19, 857, DateTimeKind.Local).AddTicks(7228) });

            migrationBuilder.UpdateData(
                table: "Religions",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 11, 16, 44, 19, 857, DateTimeKind.Local).AddTicks(7171), new DateTime(2026, 4, 11, 16, 44, 19, 857, DateTimeKind.Local).AddTicks(7172) });

            migrationBuilder.UpdateData(
                table: "StaffCategories",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 11, 16, 44, 19, 857, DateTimeKind.Local).AddTicks(7061), new DateTime(2026, 4, 11, 16, 44, 19, 857, DateTimeKind.Local).AddTicks(7062) });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Category",
                table: "Grades");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "269f0cf3-405e-4163-83f3-1b63ebebd62e",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 11, 13, 17, 31, 266, DateTimeKind.Local).AddTicks(5265), new DateTime(2026, 4, 11, 13, 17, 31, 266, DateTimeKind.Local).AddTicks(5266) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "448df289-142c-4959-a912-60733515e1b4",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 11, 13, 17, 31, 266, DateTimeKind.Local).AddTicks(5249), new DateTime(2026, 4, 11, 13, 17, 31, 266, DateTimeKind.Local).AddTicks(5250) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "48c50c3a-9958-453b-b649-4e21af131322",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 11, 13, 17, 31, 266, DateTimeKind.Local).AddTicks(5232), new DateTime(2026, 4, 11, 13, 17, 31, 266, DateTimeKind.Local).AddTicks(5233) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "717d9b15-a428-440c-b26b-08d3bbb68b02",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 11, 13, 17, 31, 266, DateTimeKind.Local).AddTicks(5163), new DateTime(2026, 4, 11, 13, 17, 31, 266, DateTimeKind.Local).AddTicks(5176) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "95ed2407-3e58-4af2-88a4-1c4e96473f68",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 11, 13, 17, 31, 266, DateTimeKind.Local).AddTicks(5215), new DateTime(2026, 4, 11, 13, 17, 31, 266, DateTimeKind.Local).AddTicks(5217) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "97942bee-ef12-4425-8225-4f293d0f36dd",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 11, 13, 17, 31, 266, DateTimeKind.Local).AddTicks(5303), new DateTime(2026, 4, 11, 13, 17, 31, 266, DateTimeKind.Local).AddTicks(5304) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "cd12b44b-103b-48df-8887-a2bf42e0651e",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 11, 13, 17, 31, 266, DateTimeKind.Local).AddTicks(5281), new DateTime(2026, 4, 11, 13, 17, 31, 266, DateTimeKind.Local).AddTicks(5282) });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "7e67d486-af3e-49f1-a109-a2b864b8e0ec",
                columns: new[] { "ConcurrencyStamp", "Created", "Modified", "PasswordHash", "SecurityStamp" },
                values: new object[] { "682b1c2a-86ec-442b-981a-d821f396b0bd", new DateTime(2026, 4, 11, 13, 17, 31, 266, DateTimeKind.Local).AddTicks(5630), new DateTime(2026, 4, 11, 13, 17, 31, 266, DateTimeKind.Local).AddTicks(5632), "AQAAAAIAAYagAAAAEJfiDjYLF8iUwbXGXEIB5Q/qtZiLsphc2e1SlqTKvg+rovmuOZQ1V/n4i1TtJjUKZQ==", "5a2c50aa-c7cb-4417-8bd4-723a7183489a" });

            migrationBuilder.UpdateData(
                table: "Designations",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 11, 13, 17, 31, 266, DateTimeKind.Local).AddTicks(5346), new DateTime(2026, 4, 11, 13, 17, 31, 266, DateTimeKind.Local).AddTicks(5346) });

            migrationBuilder.UpdateData(
                table: "EmploymentTypes",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 11, 13, 17, 31, 266, DateTimeKind.Local).AddTicks(5372), new DateTime(2026, 4, 11, 13, 17, 31, 266, DateTimeKind.Local).AddTicks(5373) });

            migrationBuilder.UpdateData(
                table: "Genders",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 11, 13, 17, 31, 266, DateTimeKind.Local).AddTicks(5531), new DateTime(2026, 4, 11, 13, 17, 31, 266, DateTimeKind.Local).AddTicks(5532) });

            migrationBuilder.UpdateData(
                table: "Nationalities",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 11, 13, 17, 31, 266, DateTimeKind.Local).AddTicks(5389), new DateTime(2026, 4, 11, 13, 17, 31, 266, DateTimeKind.Local).AddTicks(5389) });

            migrationBuilder.UpdateData(
                table: "Person",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "DateOfBirth", "EmploymentDate", "EndofEmploymentDate", "Modified" },
                values: new object[] { new DateTime(2026, 4, 11, 13, 17, 31, 266, DateTimeKind.Local).AddTicks(5564), new DateTime(2026, 4, 11, 13, 17, 31, 266, DateTimeKind.Local).AddTicks(5562), new DateTime(2026, 4, 11, 13, 17, 31, 266, DateTimeKind.Local).AddTicks(5559), new DateTime(2026, 4, 11, 13, 17, 31, 266, DateTimeKind.Local).AddTicks(5559), new DateTime(2026, 4, 11, 13, 17, 31, 266, DateTimeKind.Local).AddTicks(5565) });

            migrationBuilder.UpdateData(
                table: "Religions",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 11, 13, 17, 31, 266, DateTimeKind.Local).AddTicks(5509), new DateTime(2026, 4, 11, 13, 17, 31, 266, DateTimeKind.Local).AddTicks(5511) });

            migrationBuilder.UpdateData(
                table: "StaffCategories",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 11, 13, 17, 31, 266, DateTimeKind.Local).AddTicks(5329), new DateTime(2026, 4, 11, 13, 17, 31, 266, DateTimeKind.Local).AddTicks(5331) });
        }
    }
}
