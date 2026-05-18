using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SchoolWebApp.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddJournalEntryStatus : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "JournalEntries",
                type: "int",
                nullable: false,
                defaultValue: 0);

            // Set all existing IsPosted=true entries to Approved (Status=2)
            migrationBuilder.Sql("UPDATE JournalEntries SET Status = 2 WHERE IsPosted = 1;");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 16, 20, 28, 15, 659, DateTimeKind.Local).AddTicks(28), new DateTime(2026, 4, 16, 20, 28, 15, 659, DateTimeKind.Local).AddTicks(47) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 16, 20, 28, 15, 659, DateTimeKind.Local).AddTicks(172), new DateTime(2026, 4, 16, 20, 28, 15, 659, DateTimeKind.Local).AddTicks(173) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 16, 20, 28, 15, 659, DateTimeKind.Local).AddTicks(200), new DateTime(2026, 4, 16, 20, 28, 15, 659, DateTimeKind.Local).AddTicks(201) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 16, 20, 28, 15, 659, DateTimeKind.Local).AddTicks(225), new DateTime(2026, 4, 16, 20, 28, 15, 659, DateTimeKind.Local).AddTicks(226) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 16, 20, 28, 15, 659, DateTimeKind.Local).AddTicks(249), new DateTime(2026, 4, 16, 20, 28, 15, 659, DateTimeKind.Local).AddTicks(250) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 6,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 16, 20, 28, 15, 659, DateTimeKind.Local).AddTicks(293), new DateTime(2026, 4, 16, 20, 28, 15, 659, DateTimeKind.Local).AddTicks(294) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 7,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 16, 20, 28, 15, 659, DateTimeKind.Local).AddTicks(326), new DateTime(2026, 4, 16, 20, 28, 15, 659, DateTimeKind.Local).AddTicks(327) });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "ConcurrencyStamp", "Created", "Modified", "PasswordHash" },
                values: new object[] { "4a99465b-0414-4d14-8889-bda27629f70c", new DateTime(2026, 4, 16, 20, 28, 15, 659, DateTimeKind.Local).AddTicks(876), new DateTime(2026, 4, 16, 20, 28, 15, 659, DateTimeKind.Local).AddTicks(877), "AQAAAAIAAYagAAAAEJjOtMa0FSItpDMKXrRfct6pCxPRoeda0QjgqZEMh/IHlG+05wxkzMaDb6W9HqAb2Q==" });

            migrationBuilder.UpdateData(
                table: "Designations",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 16, 20, 28, 15, 659, DateTimeKind.Local).AddTicks(446), new DateTime(2026, 4, 16, 20, 28, 15, 659, DateTimeKind.Local).AddTicks(447) });

            migrationBuilder.UpdateData(
                table: "EmploymentTypes",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 16, 20, 28, 15, 659, DateTimeKind.Local).AddTicks(500), new DateTime(2026, 4, 16, 20, 28, 15, 659, DateTimeKind.Local).AddTicks(501) });

            migrationBuilder.UpdateData(
                table: "Genders",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 16, 20, 28, 15, 659, DateTimeKind.Local).AddTicks(599), new DateTime(2026, 4, 16, 20, 28, 15, 659, DateTimeKind.Local).AddTicks(600) });

            migrationBuilder.UpdateData(
                table: "Nationalities",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 16, 20, 28, 15, 659, DateTimeKind.Local).AddTicks(544), new DateTime(2026, 4, 16, 20, 28, 15, 659, DateTimeKind.Local).AddTicks(545) });

            migrationBuilder.UpdateData(
                table: "Person",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "DateOfBirth", "EmploymentDate", "EndofEmploymentDate", "Modified" },
                values: new object[] { new DateTime(2026, 4, 16, 20, 28, 15, 659, DateTimeKind.Local).AddTicks(710), new DateTime(2026, 4, 16, 20, 28, 15, 659, DateTimeKind.Local).AddTicks(704), new DateTime(2026, 4, 16, 20, 28, 15, 659, DateTimeKind.Local).AddTicks(699), new DateTime(2026, 4, 16, 20, 28, 15, 659, DateTimeKind.Local).AddTicks(700), new DateTime(2026, 4, 16, 20, 28, 15, 659, DateTimeKind.Local).AddTicks(716) });

            migrationBuilder.UpdateData(
                table: "Religions",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 16, 20, 28, 15, 659, DateTimeKind.Local).AddTicks(575), new DateTime(2026, 4, 16, 20, 28, 15, 659, DateTimeKind.Local).AddTicks(576) });

            migrationBuilder.UpdateData(
                table: "StaffCategories",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 16, 20, 28, 15, 659, DateTimeKind.Local).AddTicks(389), new DateTime(2026, 4, 16, 20, 28, 15, 659, DateTimeKind.Local).AddTicks(394) });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Status",
                table: "JournalEntries");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 16, 18, 41, 7, 623, DateTimeKind.Local).AddTicks(2992), new DateTime(2026, 4, 16, 18, 41, 7, 623, DateTimeKind.Local).AddTicks(3009) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 16, 18, 41, 7, 623, DateTimeKind.Local).AddTicks(3108), new DateTime(2026, 4, 16, 18, 41, 7, 623, DateTimeKind.Local).AddTicks(3111) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 16, 18, 41, 7, 623, DateTimeKind.Local).AddTicks(3156), new DateTime(2026, 4, 16, 18, 41, 7, 623, DateTimeKind.Local).AddTicks(3157) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 16, 18, 41, 7, 623, DateTimeKind.Local).AddTicks(3197), new DateTime(2026, 4, 16, 18, 41, 7, 623, DateTimeKind.Local).AddTicks(3198) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 16, 18, 41, 7, 623, DateTimeKind.Local).AddTicks(3237), new DateTime(2026, 4, 16, 18, 41, 7, 623, DateTimeKind.Local).AddTicks(3239) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 6,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 16, 18, 41, 7, 623, DateTimeKind.Local).AddTicks(3288), new DateTime(2026, 4, 16, 18, 41, 7, 623, DateTimeKind.Local).AddTicks(3290) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 7,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 16, 18, 41, 7, 623, DateTimeKind.Local).AddTicks(3338), new DateTime(2026, 4, 16, 18, 41, 7, 623, DateTimeKind.Local).AddTicks(3340) });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "ConcurrencyStamp", "Created", "Modified", "PasswordHash" },
                values: new object[] { "e8c6f04d-ac59-4bed-b6a1-a6a0557eb091", new DateTime(2026, 4, 16, 18, 41, 7, 623, DateTimeKind.Local).AddTicks(3969), new DateTime(2026, 4, 16, 18, 41, 7, 623, DateTimeKind.Local).AddTicks(3972), "AQAAAAIAAYagAAAAEGHiJGoO6rYAjIt2bwKaFUzs6McbJeO0C93i+Vt23JbMYpTcrPWfHMATHX7wVjUrsg==" });

            migrationBuilder.UpdateData(
                table: "Designations",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 16, 18, 41, 7, 623, DateTimeKind.Local).AddTicks(3456), new DateTime(2026, 4, 16, 18, 41, 7, 623, DateTimeKind.Local).AddTicks(3458) });

            migrationBuilder.UpdateData(
                table: "EmploymentTypes",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 16, 18, 41, 7, 623, DateTimeKind.Local).AddTicks(3508), new DateTime(2026, 4, 16, 18, 41, 7, 623, DateTimeKind.Local).AddTicks(3510) });

            migrationBuilder.UpdateData(
                table: "Genders",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 16, 18, 41, 7, 623, DateTimeKind.Local).AddTicks(3632), new DateTime(2026, 4, 16, 18, 41, 7, 623, DateTimeKind.Local).AddTicks(3634) });

            migrationBuilder.UpdateData(
                table: "Nationalities",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 16, 18, 41, 7, 623, DateTimeKind.Local).AddTicks(3551), new DateTime(2026, 4, 16, 18, 41, 7, 623, DateTimeKind.Local).AddTicks(3553) });

            migrationBuilder.UpdateData(
                table: "Person",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "DateOfBirth", "EmploymentDate", "EndofEmploymentDate", "Modified" },
                values: new object[] { new DateTime(2026, 4, 16, 18, 41, 7, 623, DateTimeKind.Local).AddTicks(3702), new DateTime(2026, 4, 16, 18, 41, 7, 623, DateTimeKind.Local).AddTicks(3698), new DateTime(2026, 4, 16, 18, 41, 7, 623, DateTimeKind.Local).AddTicks(3692), new DateTime(2026, 4, 16, 18, 41, 7, 623, DateTimeKind.Local).AddTicks(3694), new DateTime(2026, 4, 16, 18, 41, 7, 623, DateTimeKind.Local).AddTicks(3708) });

            migrationBuilder.UpdateData(
                table: "Religions",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 16, 18, 41, 7, 623, DateTimeKind.Local).AddTicks(3597), new DateTime(2026, 4, 16, 18, 41, 7, 623, DateTimeKind.Local).AddTicks(3598) });

            migrationBuilder.UpdateData(
                table: "StaffCategories",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 16, 18, 41, 7, 623, DateTimeKind.Local).AddTicks(3395), new DateTime(2026, 4, 16, 18, 41, 7, 623, DateTimeKind.Local).AddTicks(3397) });
        }
    }
}
