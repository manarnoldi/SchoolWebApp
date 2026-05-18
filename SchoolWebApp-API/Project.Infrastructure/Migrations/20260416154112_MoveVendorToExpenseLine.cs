using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SchoolWebApp.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class MoveVendorToExpenseLine : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Vendor",
                table: "Expenses");

            migrationBuilder.AddColumn<string>(
                name: "Vendor",
                table: "ExpenseLines",
                type: "varchar(255)",
                maxLength: 255,
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Vendor",
                table: "ExpenseLines");

            migrationBuilder.AddColumn<string>(
                name: "Vendor",
                table: "Expenses",
                type: "varchar(255)",
                maxLength: 255,
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 16, 18, 30, 35, 162, DateTimeKind.Local).AddTicks(4555), new DateTime(2026, 4, 16, 18, 30, 35, 162, DateTimeKind.Local).AddTicks(4596) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 16, 18, 30, 35, 162, DateTimeKind.Local).AddTicks(5463), new DateTime(2026, 4, 16, 18, 30, 35, 162, DateTimeKind.Local).AddTicks(5477) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 16, 18, 30, 35, 162, DateTimeKind.Local).AddTicks(5653), new DateTime(2026, 4, 16, 18, 30, 35, 162, DateTimeKind.Local).AddTicks(5658) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 16, 18, 30, 35, 162, DateTimeKind.Local).AddTicks(5769), new DateTime(2026, 4, 16, 18, 30, 35, 162, DateTimeKind.Local).AddTicks(5773) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 16, 18, 30, 35, 162, DateTimeKind.Local).AddTicks(5868), new DateTime(2026, 4, 16, 18, 30, 35, 162, DateTimeKind.Local).AddTicks(5872) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 6,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 16, 18, 30, 35, 162, DateTimeKind.Local).AddTicks(6017), new DateTime(2026, 4, 16, 18, 30, 35, 162, DateTimeKind.Local).AddTicks(6021) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 7,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 16, 18, 30, 35, 162, DateTimeKind.Local).AddTicks(6132), new DateTime(2026, 4, 16, 18, 30, 35, 162, DateTimeKind.Local).AddTicks(6137) });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "ConcurrencyStamp", "Created", "Modified", "PasswordHash" },
                values: new object[] { "6905e1c5-49fd-49bf-b9b7-6dc90a6ae29c", new DateTime(2026, 4, 16, 18, 30, 35, 162, DateTimeKind.Local).AddTicks(9149), new DateTime(2026, 4, 16, 18, 30, 35, 162, DateTimeKind.Local).AddTicks(9156), "AQAAAAIAAYagAAAAEAm6ww1nsH45WDipn7PlAr9WgAGhsN+aBn49gbnll+rqE6hbGj/j5HkDlrY3o4M25w==" });

            migrationBuilder.UpdateData(
                table: "Designations",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 16, 18, 30, 35, 162, DateTimeKind.Local).AddTicks(8038), new DateTime(2026, 4, 16, 18, 30, 35, 162, DateTimeKind.Local).AddTicks(8044) });

            migrationBuilder.UpdateData(
                table: "EmploymentTypes",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 16, 18, 30, 35, 162, DateTimeKind.Local).AddTicks(8149), new DateTime(2026, 4, 16, 18, 30, 35, 162, DateTimeKind.Local).AddTicks(8153) });

            migrationBuilder.UpdateData(
                table: "Genders",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 16, 18, 30, 35, 162, DateTimeKind.Local).AddTicks(8439), new DateTime(2026, 4, 16, 18, 30, 35, 162, DateTimeKind.Local).AddTicks(8444) });

            migrationBuilder.UpdateData(
                table: "Nationalities",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 16, 18, 30, 35, 162, DateTimeKind.Local).AddTicks(8244), new DateTime(2026, 4, 16, 18, 30, 35, 162, DateTimeKind.Local).AddTicks(8248) });

            migrationBuilder.UpdateData(
                table: "Person",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "DateOfBirth", "EmploymentDate", "EndofEmploymentDate", "Modified" },
                values: new object[] { new DateTime(2026, 4, 16, 18, 30, 35, 162, DateTimeKind.Local).AddTicks(8778), new DateTime(2026, 4, 16, 18, 30, 35, 162, DateTimeKind.Local).AddTicks(8772), new DateTime(2026, 4, 16, 18, 30, 35, 162, DateTimeKind.Local).AddTicks(8761), new DateTime(2026, 4, 16, 18, 30, 35, 162, DateTimeKind.Local).AddTicks(8767), new DateTime(2026, 4, 16, 18, 30, 35, 162, DateTimeKind.Local).AddTicks(8791) });

            migrationBuilder.UpdateData(
                table: "Religions",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 16, 18, 30, 35, 162, DateTimeKind.Local).AddTicks(8346), new DateTime(2026, 4, 16, 18, 30, 35, 162, DateTimeKind.Local).AddTicks(8350) });

            migrationBuilder.UpdateData(
                table: "StaffCategories",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 16, 18, 30, 35, 162, DateTimeKind.Local).AddTicks(7851), new DateTime(2026, 4, 16, 18, 30, 35, 162, DateTimeKind.Local).AddTicks(7869) });
        }
    }
}
