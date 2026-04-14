using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SchoolWebApp.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddAbbreviationToCoCurriculumScore : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Abbreviation",
                table: "CoCurriculumScores",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "269f0cf3-405e-4163-83f3-1b63ebebd62e",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 9, 17, 28, 58, 405, DateTimeKind.Local).AddTicks(6031), new DateTime(2026, 4, 9, 17, 28, 58, 405, DateTimeKind.Local).AddTicks(6034) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "448df289-142c-4959-a912-60733515e1b4",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 9, 17, 28, 58, 405, DateTimeKind.Local).AddTicks(5967), new DateTime(2026, 4, 9, 17, 28, 58, 405, DateTimeKind.Local).AddTicks(5970) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "48c50c3a-9958-453b-b649-4e21af131322",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 9, 17, 28, 58, 405, DateTimeKind.Local).AddTicks(5902), new DateTime(2026, 4, 9, 17, 28, 58, 405, DateTimeKind.Local).AddTicks(5905) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "717d9b15-a428-440c-b26b-08d3bbb68b02",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 9, 17, 28, 58, 405, DateTimeKind.Local).AddTicks(5665), new DateTime(2026, 4, 9, 17, 28, 58, 405, DateTimeKind.Local).AddTicks(5684) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "95ed2407-3e58-4af2-88a4-1c4e96473f68",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 9, 17, 28, 58, 405, DateTimeKind.Local).AddTicks(5829), new DateTime(2026, 4, 9, 17, 28, 58, 405, DateTimeKind.Local).AddTicks(5832) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "97942bee-ef12-4425-8225-4f293d0f36dd",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 9, 17, 28, 58, 405, DateTimeKind.Local).AddTicks(6172), new DateTime(2026, 4, 9, 17, 28, 58, 405, DateTimeKind.Local).AddTicks(6175) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "cd12b44b-103b-48df-8887-a2bf42e0651e",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 9, 17, 28, 58, 405, DateTimeKind.Local).AddTicks(6097), new DateTime(2026, 4, 9, 17, 28, 58, 405, DateTimeKind.Local).AddTicks(6100) });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "7e67d486-af3e-49f1-a109-a2b864b8e0ec",
                columns: new[] { "ConcurrencyStamp", "Created", "Modified", "PasswordHash", "SecurityStamp" },
                values: new object[] { "caed155b-d4c5-41eb-b592-1e1ff0df675f", new DateTime(2026, 4, 9, 17, 28, 58, 405, DateTimeKind.Local).AddTicks(6732), new DateTime(2026, 4, 9, 17, 28, 58, 405, DateTimeKind.Local).AddTicks(6735), "AQAAAAIAAYagAAAAELOeLeb+Pa+rDqTYTZ8NjxMS9S0j7Lcp1WSWpncqIiuSbGvp1wkDi86C9TTfVH0xdw==", "78495c16-7b17-4e71-94bf-0b508da88ec2" });

            migrationBuilder.UpdateData(
                table: "Designations",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 9, 17, 28, 58, 405, DateTimeKind.Local).AddTicks(6294), new DateTime(2026, 4, 9, 17, 28, 58, 405, DateTimeKind.Local).AddTicks(6297) });

            migrationBuilder.UpdateData(
                table: "EmploymentTypes",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 9, 17, 28, 58, 405, DateTimeKind.Local).AddTicks(6350), new DateTime(2026, 4, 9, 17, 28, 58, 405, DateTimeKind.Local).AddTicks(6353) });

            migrationBuilder.UpdateData(
                table: "Genders",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 9, 17, 28, 58, 405, DateTimeKind.Local).AddTicks(6517), new DateTime(2026, 4, 9, 17, 28, 58, 405, DateTimeKind.Local).AddTicks(6519) });

            migrationBuilder.UpdateData(
                table: "Nationalities",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 9, 17, 28, 58, 405, DateTimeKind.Local).AddTicks(6401), new DateTime(2026, 4, 9, 17, 28, 58, 405, DateTimeKind.Local).AddTicks(6404) });

            migrationBuilder.UpdateData(
                table: "Person",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "DateOfBirth", "EmploymentDate", "EndofEmploymentDate", "Modified" },
                values: new object[] { new DateTime(2026, 4, 9, 17, 28, 58, 405, DateTimeKind.Local).AddTicks(6603), new DateTime(2026, 4, 9, 17, 28, 58, 405, DateTimeKind.Local).AddTicks(6598), new DateTime(2026, 4, 9, 17, 28, 58, 405, DateTimeKind.Local).AddTicks(6592), new DateTime(2026, 4, 9, 17, 28, 58, 405, DateTimeKind.Local).AddTicks(6595), new DateTime(2026, 4, 9, 17, 28, 58, 405, DateTimeKind.Local).AddTicks(6610) });

            migrationBuilder.UpdateData(
                table: "Religions",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 9, 17, 28, 58, 405, DateTimeKind.Local).AddTicks(6469), new DateTime(2026, 4, 9, 17, 28, 58, 405, DateTimeKind.Local).AddTicks(6472) });

            migrationBuilder.UpdateData(
                table: "StaffCategories",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 9, 17, 28, 58, 405, DateTimeKind.Local).AddTicks(6247), new DateTime(2026, 4, 9, 17, 28, 58, 405, DateTimeKind.Local).AddTicks(6250) });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Abbreviation",
                table: "CoCurriculumScores");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "269f0cf3-405e-4163-83f3-1b63ebebd62e",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 9, 15, 43, 24, 378, DateTimeKind.Local).AddTicks(9881), new DateTime(2026, 4, 9, 15, 43, 24, 378, DateTimeKind.Local).AddTicks(9882) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "448df289-142c-4959-a912-60733515e1b4",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 9, 15, 43, 24, 378, DateTimeKind.Local).AddTicks(9853), new DateTime(2026, 4, 9, 15, 43, 24, 378, DateTimeKind.Local).AddTicks(9854) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "48c50c3a-9958-453b-b649-4e21af131322",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 9, 15, 43, 24, 378, DateTimeKind.Local).AddTicks(9836), new DateTime(2026, 4, 9, 15, 43, 24, 378, DateTimeKind.Local).AddTicks(9838) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "717d9b15-a428-440c-b26b-08d3bbb68b02",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 9, 15, 43, 24, 378, DateTimeKind.Local).AddTicks(9769), new DateTime(2026, 4, 9, 15, 43, 24, 378, DateTimeKind.Local).AddTicks(9784) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "95ed2407-3e58-4af2-88a4-1c4e96473f68",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 9, 15, 43, 24, 378, DateTimeKind.Local).AddTicks(9817), new DateTime(2026, 4, 9, 15, 43, 24, 378, DateTimeKind.Local).AddTicks(9819) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "97942bee-ef12-4425-8225-4f293d0f36dd",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 9, 15, 43, 24, 378, DateTimeKind.Local).AddTicks(9916), new DateTime(2026, 4, 9, 15, 43, 24, 378, DateTimeKind.Local).AddTicks(9917) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "cd12b44b-103b-48df-8887-a2bf42e0651e",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 9, 15, 43, 24, 378, DateTimeKind.Local).AddTicks(9900), new DateTime(2026, 4, 9, 15, 43, 24, 378, DateTimeKind.Local).AddTicks(9901) });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "7e67d486-af3e-49f1-a109-a2b864b8e0ec",
                columns: new[] { "ConcurrencyStamp", "Created", "Modified", "PasswordHash", "SecurityStamp" },
                values: new object[] { "5ee9a0ff-4a61-43a0-a4e1-7477b94cbdd9", new DateTime(2026, 4, 9, 15, 43, 24, 379, DateTimeKind.Local).AddTicks(139), new DateTime(2026, 4, 9, 15, 43, 24, 379, DateTimeKind.Local).AddTicks(140), "AQAAAAIAAYagAAAAECEXcXmq+oq5Erc+aduae70UVU1+YZejcXC/V0gdXrz3fouRoXnTgngLjd3eaUwFOg==", "2d9b7f15-dd8a-45c8-9a4e-301da84f4267" });

            migrationBuilder.UpdateData(
                table: "Designations",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 9, 15, 43, 24, 378, DateTimeKind.Local).AddTicks(9971), new DateTime(2026, 4, 9, 15, 43, 24, 378, DateTimeKind.Local).AddTicks(9972) });

            migrationBuilder.UpdateData(
                table: "EmploymentTypes",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 9, 15, 43, 24, 378, DateTimeKind.Local).AddTicks(9995), new DateTime(2026, 4, 9, 15, 43, 24, 378, DateTimeKind.Local).AddTicks(9995) });

            migrationBuilder.UpdateData(
                table: "Genders",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 9, 15, 43, 24, 379, DateTimeKind.Local).AddTicks(49), new DateTime(2026, 4, 9, 15, 43, 24, 379, DateTimeKind.Local).AddTicks(50) });

            migrationBuilder.UpdateData(
                table: "Nationalities",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 9, 15, 43, 24, 379, DateTimeKind.Local).AddTicks(13), new DateTime(2026, 4, 9, 15, 43, 24, 379, DateTimeKind.Local).AddTicks(14) });

            migrationBuilder.UpdateData(
                table: "Person",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "DateOfBirth", "EmploymentDate", "EndofEmploymentDate", "Modified" },
                values: new object[] { new DateTime(2026, 4, 9, 15, 43, 24, 379, DateTimeKind.Local).AddTicks(78), new DateTime(2026, 4, 9, 15, 43, 24, 379, DateTimeKind.Local).AddTicks(75), new DateTime(2026, 4, 9, 15, 43, 24, 379, DateTimeKind.Local).AddTicks(72), new DateTime(2026, 4, 9, 15, 43, 24, 379, DateTimeKind.Local).AddTicks(73), new DateTime(2026, 4, 9, 15, 43, 24, 379, DateTimeKind.Local).AddTicks(79) });

            migrationBuilder.UpdateData(
                table: "Religions",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 9, 15, 43, 24, 379, DateTimeKind.Local).AddTicks(31), new DateTime(2026, 4, 9, 15, 43, 24, 379, DateTimeKind.Local).AddTicks(31) });

            migrationBuilder.UpdateData(
                table: "StaffCategories",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 9, 15, 43, 24, 378, DateTimeKind.Local).AddTicks(9943), new DateTime(2026, 4, 9, 15, 43, 24, 378, DateTimeKind.Local).AddTicks(9944) });
        }
    }
}
