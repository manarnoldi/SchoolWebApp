using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SchoolWebApp.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddPaymentApprovalStatus : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ApprovalStatus",
                table: "Payments",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 17, 21, 55, 38, 291, DateTimeKind.Local).AddTicks(4063), new DateTime(2026, 4, 17, 21, 55, 38, 291, DateTimeKind.Local).AddTicks(4095) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 17, 21, 55, 38, 291, DateTimeKind.Local).AddTicks(4191), new DateTime(2026, 4, 17, 21, 55, 38, 291, DateTimeKind.Local).AddTicks(4194) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 17, 21, 55, 38, 291, DateTimeKind.Local).AddTicks(4247), new DateTime(2026, 4, 17, 21, 55, 38, 291, DateTimeKind.Local).AddTicks(4249) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 17, 21, 55, 38, 291, DateTimeKind.Local).AddTicks(4296), new DateTime(2026, 4, 17, 21, 55, 38, 291, DateTimeKind.Local).AddTicks(4298) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 17, 21, 55, 38, 291, DateTimeKind.Local).AddTicks(4342), new DateTime(2026, 4, 17, 21, 55, 38, 291, DateTimeKind.Local).AddTicks(4344) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 6,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 17, 21, 55, 38, 291, DateTimeKind.Local).AddTicks(4399), new DateTime(2026, 4, 17, 21, 55, 38, 291, DateTimeKind.Local).AddTicks(4401) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 7,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 17, 21, 55, 38, 291, DateTimeKind.Local).AddTicks(4454), new DateTime(2026, 4, 17, 21, 55, 38, 291, DateTimeKind.Local).AddTicks(4456) });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "ConcurrencyStamp", "Created", "Modified", "PasswordHash" },
                values: new object[] { "7f842422-885c-4cd7-92b8-5a6d7761eeeb", new DateTime(2026, 4, 17, 21, 55, 38, 291, DateTimeKind.Local).AddTicks(5418), new DateTime(2026, 4, 17, 21, 55, 38, 291, DateTimeKind.Local).AddTicks(5423), "AQAAAAIAAYagAAAAEOkn+zlg5C63Ugn9ZpFXJhKB/3HNcDudSj8XKCcKT7dlFAtv9cu0dHLP6YHUQXlexg==" });

            migrationBuilder.UpdateData(
                table: "Designations",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 17, 21, 55, 38, 291, DateTimeKind.Local).AddTicks(4659), new DateTime(2026, 4, 17, 21, 55, 38, 291, DateTimeKind.Local).AddTicks(4662) });

            migrationBuilder.UpdateData(
                table: "EmploymentTypes",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 17, 21, 55, 38, 291, DateTimeKind.Local).AddTicks(4876), new DateTime(2026, 4, 17, 21, 55, 38, 291, DateTimeKind.Local).AddTicks(4879) });

            migrationBuilder.UpdateData(
                table: "Genders",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 17, 21, 55, 38, 291, DateTimeKind.Local).AddTicks(5039), new DateTime(2026, 4, 17, 21, 55, 38, 291, DateTimeKind.Local).AddTicks(5041) });

            migrationBuilder.UpdateData(
                table: "Nationalities",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 17, 21, 55, 38, 291, DateTimeKind.Local).AddTicks(4932), new DateTime(2026, 4, 17, 21, 55, 38, 291, DateTimeKind.Local).AddTicks(4934) });

            migrationBuilder.UpdateData(
                table: "Person",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "DateOfBirth", "EmploymentDate", "EndofEmploymentDate", "Modified" },
                values: new object[] { new DateTime(2026, 4, 17, 21, 55, 38, 291, DateTimeKind.Local).AddTicks(5140), new DateTime(2026, 4, 17, 21, 55, 38, 291, DateTimeKind.Local).AddTicks(5133), new DateTime(2026, 4, 17, 21, 55, 38, 291, DateTimeKind.Local).AddTicks(5120), new DateTime(2026, 4, 17, 21, 55, 38, 291, DateTimeKind.Local).AddTicks(5122), new DateTime(2026, 4, 17, 21, 55, 38, 291, DateTimeKind.Local).AddTicks(5159) });

            migrationBuilder.UpdateData(
                table: "Religions",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 17, 21, 55, 38, 291, DateTimeKind.Local).AddTicks(4990), new DateTime(2026, 4, 17, 21, 55, 38, 291, DateTimeKind.Local).AddTicks(4992) });

            migrationBuilder.UpdateData(
                table: "StaffCategories",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 17, 21, 55, 38, 291, DateTimeKind.Local).AddTicks(4554), new DateTime(2026, 4, 17, 21, 55, 38, 291, DateTimeKind.Local).AddTicks(4557) });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ApprovalStatus",
                table: "Payments");

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
    }
}
