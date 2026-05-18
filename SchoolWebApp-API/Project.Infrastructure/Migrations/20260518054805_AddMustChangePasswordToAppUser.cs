using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SchoolWebApp.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddMustChangePasswordToAppUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "MustChangePassword",
                table: "AspNetUsers",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.UpdateData(
                table: "ApprovalWorkflowSteps",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 5, 18, 8, 48, 1, 271, DateTimeKind.Local).AddTicks(4550), new DateTime(2026, 5, 18, 8, 48, 1, 271, DateTimeKind.Local).AddTicks(4550) });

            migrationBuilder.UpdateData(
                table: "ApprovalWorkflowSteps",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 5, 18, 8, 48, 1, 271, DateTimeKind.Local).AddTicks(4550), new DateTime(2026, 5, 18, 8, 48, 1, 271, DateTimeKind.Local).AddTicks(4550) });

            migrationBuilder.UpdateData(
                table: "ApprovalWorkflowSteps",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 5, 18, 8, 48, 1, 271, DateTimeKind.Local).AddTicks(4550), new DateTime(2026, 5, 18, 8, 48, 1, 271, DateTimeKind.Local).AddTicks(4550) });

            migrationBuilder.UpdateData(
                table: "ApprovalWorkflowSteps",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 5, 18, 8, 48, 1, 271, DateTimeKind.Local).AddTicks(4550), new DateTime(2026, 5, 18, 8, 48, 1, 271, DateTimeKind.Local).AddTicks(4550) });

            migrationBuilder.UpdateData(
                table: "ApprovalWorkflowSteps",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 5, 18, 8, 48, 1, 271, DateTimeKind.Local).AddTicks(4550), new DateTime(2026, 5, 18, 8, 48, 1, 271, DateTimeKind.Local).AddTicks(4550) });

            migrationBuilder.UpdateData(
                table: "ApprovalWorkflowSteps",
                keyColumn: "Id",
                keyValue: 6,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 5, 18, 8, 48, 1, 271, DateTimeKind.Local).AddTicks(4550), new DateTime(2026, 5, 18, 8, 48, 1, 271, DateTimeKind.Local).AddTicks(4550) });

            migrationBuilder.UpdateData(
                table: "ApprovalWorkflowSteps",
                keyColumn: "Id",
                keyValue: 7,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 5, 18, 8, 48, 1, 271, DateTimeKind.Local).AddTicks(4550), new DateTime(2026, 5, 18, 8, 48, 1, 271, DateTimeKind.Local).AddTicks(4550) });

            migrationBuilder.UpdateData(
                table: "ApprovalWorkflowSteps",
                keyColumn: "Id",
                keyValue: 8,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 5, 18, 8, 48, 1, 271, DateTimeKind.Local).AddTicks(4550), new DateTime(2026, 5, 18, 8, 48, 1, 271, DateTimeKind.Local).AddTicks(4550) });

            migrationBuilder.UpdateData(
                table: "ApprovalWorkflowSteps",
                keyColumn: "Id",
                keyValue: 9,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 5, 18, 8, 48, 1, 271, DateTimeKind.Local).AddTicks(4550), new DateTime(2026, 5, 18, 8, 48, 1, 271, DateTimeKind.Local).AddTicks(4550) });

            migrationBuilder.UpdateData(
                table: "ApprovalWorkflowSteps",
                keyColumn: "Id",
                keyValue: 10,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 5, 18, 8, 48, 1, 271, DateTimeKind.Local).AddTicks(4550), new DateTime(2026, 5, 18, 8, 48, 1, 271, DateTimeKind.Local).AddTicks(4550) });

            migrationBuilder.UpdateData(
                table: "ApprovalWorkflows",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 5, 18, 8, 48, 1, 271, DateTimeKind.Local).AddTicks(4550), new DateTime(2026, 5, 18, 8, 48, 1, 271, DateTimeKind.Local).AddTicks(4550) });

            migrationBuilder.UpdateData(
                table: "ApprovalWorkflows",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 5, 18, 8, 48, 1, 271, DateTimeKind.Local).AddTicks(4550), new DateTime(2026, 5, 18, 8, 48, 1, 271, DateTimeKind.Local).AddTicks(4550) });

            migrationBuilder.UpdateData(
                table: "ApprovalWorkflows",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 5, 18, 8, 48, 1, 271, DateTimeKind.Local).AddTicks(4550), new DateTime(2026, 5, 18, 8, 48, 1, 271, DateTimeKind.Local).AddTicks(4550) });

            migrationBuilder.UpdateData(
                table: "ApprovalWorkflows",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 5, 18, 8, 48, 1, 271, DateTimeKind.Local).AddTicks(4550), new DateTime(2026, 5, 18, 8, 48, 1, 271, DateTimeKind.Local).AddTicks(4550) });

            migrationBuilder.UpdateData(
                table: "ApprovalWorkflows",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 5, 18, 8, 48, 1, 271, DateTimeKind.Local).AddTicks(4550), new DateTime(2026, 5, 18, 8, 48, 1, 271, DateTimeKind.Local).AddTicks(4550) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 5, 18, 8, 48, 1, 191, DateTimeKind.Local).AddTicks(9179), new DateTime(2026, 5, 18, 8, 48, 1, 191, DateTimeKind.Local).AddTicks(9199) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 5, 18, 8, 48, 1, 191, DateTimeKind.Local).AddTicks(9299), new DateTime(2026, 5, 18, 8, 48, 1, 191, DateTimeKind.Local).AddTicks(9301) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 5, 18, 8, 48, 1, 191, DateTimeKind.Local).AddTicks(9349), new DateTime(2026, 5, 18, 8, 48, 1, 191, DateTimeKind.Local).AddTicks(9351) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 5, 18, 8, 48, 1, 191, DateTimeKind.Local).AddTicks(9394), new DateTime(2026, 5, 18, 8, 48, 1, 191, DateTimeKind.Local).AddTicks(9396) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 5, 18, 8, 48, 1, 191, DateTimeKind.Local).AddTicks(9439), new DateTime(2026, 5, 18, 8, 48, 1, 191, DateTimeKind.Local).AddTicks(9441) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 6,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 5, 18, 8, 48, 1, 191, DateTimeKind.Local).AddTicks(9484), new DateTime(2026, 5, 18, 8, 48, 1, 191, DateTimeKind.Local).AddTicks(9486) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 7,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 5, 18, 8, 48, 1, 191, DateTimeKind.Local).AddTicks(9528), new DateTime(2026, 5, 18, 8, 48, 1, 191, DateTimeKind.Local).AddTicks(9530) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 8,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 5, 18, 8, 48, 1, 191, DateTimeKind.Local).AddTicks(9571), new DateTime(2026, 5, 18, 8, 48, 1, 191, DateTimeKind.Local).AddTicks(9573) });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "ConcurrencyStamp", "Created", "Modified", "MustChangePassword", "PasswordHash" },
                values: new object[] { "d0344471-1d08-49de-b8e5-eef04057632f", new DateTime(2026, 5, 18, 8, 48, 1, 192, DateTimeKind.Local).AddTicks(168), new DateTime(2026, 5, 18, 8, 48, 1, 192, DateTimeKind.Local).AddTicks(171), false, "AQAAAAIAAYagAAAAEJgP1ejCDqwXz5Egfmjyf6U6+fnmzeO1iPSzPiOK6pG3UY/TheOqE4fGY5JvJ1HlyA==" });

            migrationBuilder.UpdateData(
                table: "Designations",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 5, 18, 8, 48, 1, 191, DateTimeKind.Local).AddTicks(9712), new DateTime(2026, 5, 18, 8, 48, 1, 191, DateTimeKind.Local).AddTicks(9713) });

            migrationBuilder.UpdateData(
                table: "EmploymentTypes",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 5, 18, 8, 48, 1, 191, DateTimeKind.Local).AddTicks(9753), new DateTime(2026, 5, 18, 8, 48, 1, 191, DateTimeKind.Local).AddTicks(9755) });

            migrationBuilder.UpdateData(
                table: "Genders",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 5, 18, 8, 48, 1, 191, DateTimeKind.Local).AddTicks(9865), new DateTime(2026, 5, 18, 8, 48, 1, 191, DateTimeKind.Local).AddTicks(9867) });

            migrationBuilder.UpdateData(
                table: "Nationalities",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 5, 18, 8, 48, 1, 191, DateTimeKind.Local).AddTicks(9791), new DateTime(2026, 5, 18, 8, 48, 1, 191, DateTimeKind.Local).AddTicks(9793) });

            migrationBuilder.UpdateData(
                table: "Person",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "DateOfBirth", "EmploymentDate", "EndofEmploymentDate", "Modified" },
                values: new object[] { new DateTime(2026, 5, 18, 8, 48, 1, 191, DateTimeKind.Local).AddTicks(9946), new DateTime(2026, 5, 18, 8, 48, 1, 191, DateTimeKind.Local).AddTicks(9943), new DateTime(2026, 5, 18, 8, 48, 1, 191, DateTimeKind.Local).AddTicks(9930), new DateTime(2026, 5, 18, 8, 48, 1, 191, DateTimeKind.Local).AddTicks(9932), new DateTime(2026, 5, 18, 8, 48, 1, 191, DateTimeKind.Local).AddTicks(9946) });

            migrationBuilder.UpdateData(
                table: "Religions",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 5, 18, 8, 48, 1, 191, DateTimeKind.Local).AddTicks(9829), new DateTime(2026, 5, 18, 8, 48, 1, 191, DateTimeKind.Local).AddTicks(9830) });

            migrationBuilder.UpdateData(
                table: "StaffCategories",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 5, 18, 8, 48, 1, 191, DateTimeKind.Local).AddTicks(9671), new DateTime(2026, 5, 18, 8, 48, 1, 191, DateTimeKind.Local).AddTicks(9674) });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MustChangePassword",
                table: "AspNetUsers");

            migrationBuilder.UpdateData(
                table: "ApprovalWorkflowSteps",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 5, 17, 22, 11, 8, 376, DateTimeKind.Local).AddTicks(2509), new DateTime(2026, 5, 17, 22, 11, 8, 376, DateTimeKind.Local).AddTicks(2509) });

            migrationBuilder.UpdateData(
                table: "ApprovalWorkflowSteps",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 5, 17, 22, 11, 8, 376, DateTimeKind.Local).AddTicks(2509), new DateTime(2026, 5, 17, 22, 11, 8, 376, DateTimeKind.Local).AddTicks(2509) });

            migrationBuilder.UpdateData(
                table: "ApprovalWorkflowSteps",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 5, 17, 22, 11, 8, 376, DateTimeKind.Local).AddTicks(2509), new DateTime(2026, 5, 17, 22, 11, 8, 376, DateTimeKind.Local).AddTicks(2509) });

            migrationBuilder.UpdateData(
                table: "ApprovalWorkflowSteps",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 5, 17, 22, 11, 8, 376, DateTimeKind.Local).AddTicks(2509), new DateTime(2026, 5, 17, 22, 11, 8, 376, DateTimeKind.Local).AddTicks(2509) });

            migrationBuilder.UpdateData(
                table: "ApprovalWorkflowSteps",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 5, 17, 22, 11, 8, 376, DateTimeKind.Local).AddTicks(2509), new DateTime(2026, 5, 17, 22, 11, 8, 376, DateTimeKind.Local).AddTicks(2509) });

            migrationBuilder.UpdateData(
                table: "ApprovalWorkflowSteps",
                keyColumn: "Id",
                keyValue: 6,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 5, 17, 22, 11, 8, 376, DateTimeKind.Local).AddTicks(2509), new DateTime(2026, 5, 17, 22, 11, 8, 376, DateTimeKind.Local).AddTicks(2509) });

            migrationBuilder.UpdateData(
                table: "ApprovalWorkflowSteps",
                keyColumn: "Id",
                keyValue: 7,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 5, 17, 22, 11, 8, 376, DateTimeKind.Local).AddTicks(2509), new DateTime(2026, 5, 17, 22, 11, 8, 376, DateTimeKind.Local).AddTicks(2509) });

            migrationBuilder.UpdateData(
                table: "ApprovalWorkflowSteps",
                keyColumn: "Id",
                keyValue: 8,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 5, 17, 22, 11, 8, 376, DateTimeKind.Local).AddTicks(2509), new DateTime(2026, 5, 17, 22, 11, 8, 376, DateTimeKind.Local).AddTicks(2509) });

            migrationBuilder.UpdateData(
                table: "ApprovalWorkflowSteps",
                keyColumn: "Id",
                keyValue: 9,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 5, 17, 22, 11, 8, 376, DateTimeKind.Local).AddTicks(2509), new DateTime(2026, 5, 17, 22, 11, 8, 376, DateTimeKind.Local).AddTicks(2509) });

            migrationBuilder.UpdateData(
                table: "ApprovalWorkflowSteps",
                keyColumn: "Id",
                keyValue: 10,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 5, 17, 22, 11, 8, 376, DateTimeKind.Local).AddTicks(2509), new DateTime(2026, 5, 17, 22, 11, 8, 376, DateTimeKind.Local).AddTicks(2509) });

            migrationBuilder.UpdateData(
                table: "ApprovalWorkflows",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 5, 17, 22, 11, 8, 376, DateTimeKind.Local).AddTicks(2509), new DateTime(2026, 5, 17, 22, 11, 8, 376, DateTimeKind.Local).AddTicks(2509) });

            migrationBuilder.UpdateData(
                table: "ApprovalWorkflows",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 5, 17, 22, 11, 8, 376, DateTimeKind.Local).AddTicks(2509), new DateTime(2026, 5, 17, 22, 11, 8, 376, DateTimeKind.Local).AddTicks(2509) });

            migrationBuilder.UpdateData(
                table: "ApprovalWorkflows",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 5, 17, 22, 11, 8, 376, DateTimeKind.Local).AddTicks(2509), new DateTime(2026, 5, 17, 22, 11, 8, 376, DateTimeKind.Local).AddTicks(2509) });

            migrationBuilder.UpdateData(
                table: "ApprovalWorkflows",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 5, 17, 22, 11, 8, 376, DateTimeKind.Local).AddTicks(2509), new DateTime(2026, 5, 17, 22, 11, 8, 376, DateTimeKind.Local).AddTicks(2509) });

            migrationBuilder.UpdateData(
                table: "ApprovalWorkflows",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 5, 17, 22, 11, 8, 376, DateTimeKind.Local).AddTicks(2509), new DateTime(2026, 5, 17, 22, 11, 8, 376, DateTimeKind.Local).AddTicks(2509) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 5, 17, 22, 11, 8, 289, DateTimeKind.Local).AddTicks(2330), new DateTime(2026, 5, 17, 22, 11, 8, 289, DateTimeKind.Local).AddTicks(2359) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 5, 17, 22, 11, 8, 289, DateTimeKind.Local).AddTicks(2472), new DateTime(2026, 5, 17, 22, 11, 8, 289, DateTimeKind.Local).AddTicks(2474) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 5, 17, 22, 11, 8, 289, DateTimeKind.Local).AddTicks(2532), new DateTime(2026, 5, 17, 22, 11, 8, 289, DateTimeKind.Local).AddTicks(2534) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 5, 17, 22, 11, 8, 289, DateTimeKind.Local).AddTicks(2585), new DateTime(2026, 5, 17, 22, 11, 8, 289, DateTimeKind.Local).AddTicks(2587) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 5, 17, 22, 11, 8, 289, DateTimeKind.Local).AddTicks(2637), new DateTime(2026, 5, 17, 22, 11, 8, 289, DateTimeKind.Local).AddTicks(2640) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 6,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 5, 17, 22, 11, 8, 289, DateTimeKind.Local).AddTicks(2694), new DateTime(2026, 5, 17, 22, 11, 8, 289, DateTimeKind.Local).AddTicks(2697) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 7,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 5, 17, 22, 11, 8, 289, DateTimeKind.Local).AddTicks(2746), new DateTime(2026, 5, 17, 22, 11, 8, 289, DateTimeKind.Local).AddTicks(2748) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 8,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 5, 17, 22, 11, 8, 289, DateTimeKind.Local).AddTicks(2798), new DateTime(2026, 5, 17, 22, 11, 8, 289, DateTimeKind.Local).AddTicks(2800) });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "ConcurrencyStamp", "Created", "Modified", "PasswordHash" },
                values: new object[] { "38dd72fe-2cc1-4ca2-9a3c-3608da6db4bc", new DateTime(2026, 5, 17, 22, 11, 8, 289, DateTimeKind.Local).AddTicks(3562), new DateTime(2026, 5, 17, 22, 11, 8, 289, DateTimeKind.Local).AddTicks(3565), "AQAAAAIAAYagAAAAELgFbuxl1Quh+ni7aSyxPsPLGfTU+BLaLjycpH9BK5Rn8InarBddywROE/zeThYV8Q==" });

            migrationBuilder.UpdateData(
                table: "Designations",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 5, 17, 22, 11, 8, 289, DateTimeKind.Local).AddTicks(2963), new DateTime(2026, 5, 17, 22, 11, 8, 289, DateTimeKind.Local).AddTicks(2965) });

            migrationBuilder.UpdateData(
                table: "EmploymentTypes",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 5, 17, 22, 11, 8, 289, DateTimeKind.Local).AddTicks(3022), new DateTime(2026, 5, 17, 22, 11, 8, 289, DateTimeKind.Local).AddTicks(3024) });

            migrationBuilder.UpdateData(
                table: "Genders",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 5, 17, 22, 11, 8, 289, DateTimeKind.Local).AddTicks(3267), new DateTime(2026, 5, 17, 22, 11, 8, 289, DateTimeKind.Local).AddTicks(3270) });

            migrationBuilder.UpdateData(
                table: "Nationalities",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 5, 17, 22, 11, 8, 289, DateTimeKind.Local).AddTicks(3074), new DateTime(2026, 5, 17, 22, 11, 8, 289, DateTimeKind.Local).AddTicks(3076) });

            migrationBuilder.UpdateData(
                table: "Person",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "DateOfBirth", "EmploymentDate", "EndofEmploymentDate", "Modified" },
                values: new object[] { new DateTime(2026, 5, 17, 22, 11, 8, 289, DateTimeKind.Local).AddTicks(3369), new DateTime(2026, 5, 17, 22, 11, 8, 289, DateTimeKind.Local).AddTicks(3366), new DateTime(2026, 5, 17, 22, 11, 8, 289, DateTimeKind.Local).AddTicks(3351), new DateTime(2026, 5, 17, 22, 11, 8, 289, DateTimeKind.Local).AddTicks(3354), new DateTime(2026, 5, 17, 22, 11, 8, 289, DateTimeKind.Local).AddTicks(3370) });

            migrationBuilder.UpdateData(
                table: "Religions",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 5, 17, 22, 11, 8, 289, DateTimeKind.Local).AddTicks(3207), new DateTime(2026, 5, 17, 22, 11, 8, 289, DateTimeKind.Local).AddTicks(3210) });

            migrationBuilder.UpdateData(
                table: "StaffCategories",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 5, 17, 22, 11, 8, 289, DateTimeKind.Local).AddTicks(2909), new DateTime(2026, 5, 17, 22, 11, 8, 289, DateTimeKind.Local).AddTicks(2912) });
        }
    }
}
