using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SchoolWebApp.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddPersonIdToAppUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "PersonId",
                table: "AspNetUsers",
                type: "int",
                nullable: true);

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
                columns: new[] { "ConcurrencyStamp", "Created", "Modified", "PasswordHash", "PersonId" },
                values: new object[] { "38dd72fe-2cc1-4ca2-9a3c-3608da6db4bc", new DateTime(2026, 5, 17, 22, 11, 8, 289, DateTimeKind.Local).AddTicks(3562), new DateTime(2026, 5, 17, 22, 11, 8, 289, DateTimeKind.Local).AddTicks(3565), "AQAAAAIAAYagAAAAELgFbuxl1Quh+ni7aSyxPsPLGfTU+BLaLjycpH9BK5Rn8InarBddywROE/zeThYV8Q==", null });

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

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_PersonId",
                table: "AspNetUsers",
                column: "PersonId");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_Person_PersonId",
                table: "AspNetUsers",
                column: "PersonId",
                principalTable: "Person",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_Person_PersonId",
                table: "AspNetUsers");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_PersonId",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "PersonId",
                table: "AspNetUsers");

            migrationBuilder.UpdateData(
                table: "ApprovalWorkflowSteps",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 21, 10, 35, 32, 679, DateTimeKind.Local).AddTicks(7773), new DateTime(2026, 4, 21, 10, 35, 32, 679, DateTimeKind.Local).AddTicks(7773) });

            migrationBuilder.UpdateData(
                table: "ApprovalWorkflowSteps",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 21, 10, 35, 32, 679, DateTimeKind.Local).AddTicks(7773), new DateTime(2026, 4, 21, 10, 35, 32, 679, DateTimeKind.Local).AddTicks(7773) });

            migrationBuilder.UpdateData(
                table: "ApprovalWorkflowSteps",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 21, 10, 35, 32, 679, DateTimeKind.Local).AddTicks(7773), new DateTime(2026, 4, 21, 10, 35, 32, 679, DateTimeKind.Local).AddTicks(7773) });

            migrationBuilder.UpdateData(
                table: "ApprovalWorkflowSteps",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 21, 10, 35, 32, 679, DateTimeKind.Local).AddTicks(7773), new DateTime(2026, 4, 21, 10, 35, 32, 679, DateTimeKind.Local).AddTicks(7773) });

            migrationBuilder.UpdateData(
                table: "ApprovalWorkflowSteps",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 21, 10, 35, 32, 679, DateTimeKind.Local).AddTicks(7773), new DateTime(2026, 4, 21, 10, 35, 32, 679, DateTimeKind.Local).AddTicks(7773) });

            migrationBuilder.UpdateData(
                table: "ApprovalWorkflowSteps",
                keyColumn: "Id",
                keyValue: 6,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 21, 10, 35, 32, 679, DateTimeKind.Local).AddTicks(7773), new DateTime(2026, 4, 21, 10, 35, 32, 679, DateTimeKind.Local).AddTicks(7773) });

            migrationBuilder.UpdateData(
                table: "ApprovalWorkflowSteps",
                keyColumn: "Id",
                keyValue: 7,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 21, 10, 35, 32, 679, DateTimeKind.Local).AddTicks(7773), new DateTime(2026, 4, 21, 10, 35, 32, 679, DateTimeKind.Local).AddTicks(7773) });

            migrationBuilder.UpdateData(
                table: "ApprovalWorkflowSteps",
                keyColumn: "Id",
                keyValue: 8,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 21, 10, 35, 32, 679, DateTimeKind.Local).AddTicks(7773), new DateTime(2026, 4, 21, 10, 35, 32, 679, DateTimeKind.Local).AddTicks(7773) });

            migrationBuilder.UpdateData(
                table: "ApprovalWorkflowSteps",
                keyColumn: "Id",
                keyValue: 9,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 21, 10, 35, 32, 679, DateTimeKind.Local).AddTicks(7773), new DateTime(2026, 4, 21, 10, 35, 32, 679, DateTimeKind.Local).AddTicks(7773) });

            migrationBuilder.UpdateData(
                table: "ApprovalWorkflowSteps",
                keyColumn: "Id",
                keyValue: 10,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 21, 10, 35, 32, 679, DateTimeKind.Local).AddTicks(7773), new DateTime(2026, 4, 21, 10, 35, 32, 679, DateTimeKind.Local).AddTicks(7773) });

            migrationBuilder.UpdateData(
                table: "ApprovalWorkflows",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 21, 10, 35, 32, 679, DateTimeKind.Local).AddTicks(7773), new DateTime(2026, 4, 21, 10, 35, 32, 679, DateTimeKind.Local).AddTicks(7773) });

            migrationBuilder.UpdateData(
                table: "ApprovalWorkflows",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 21, 10, 35, 32, 679, DateTimeKind.Local).AddTicks(7773), new DateTime(2026, 4, 21, 10, 35, 32, 679, DateTimeKind.Local).AddTicks(7773) });

            migrationBuilder.UpdateData(
                table: "ApprovalWorkflows",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 21, 10, 35, 32, 679, DateTimeKind.Local).AddTicks(7773), new DateTime(2026, 4, 21, 10, 35, 32, 679, DateTimeKind.Local).AddTicks(7773) });

            migrationBuilder.UpdateData(
                table: "ApprovalWorkflows",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 21, 10, 35, 32, 679, DateTimeKind.Local).AddTicks(7773), new DateTime(2026, 4, 21, 10, 35, 32, 679, DateTimeKind.Local).AddTicks(7773) });

            migrationBuilder.UpdateData(
                table: "ApprovalWorkflows",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 21, 10, 35, 32, 679, DateTimeKind.Local).AddTicks(7773), new DateTime(2026, 4, 21, 10, 35, 32, 679, DateTimeKind.Local).AddTicks(7773) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 21, 10, 35, 32, 644, DateTimeKind.Local).AddTicks(3525), new DateTime(2026, 4, 21, 10, 35, 32, 644, DateTimeKind.Local).AddTicks(3539) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 21, 10, 35, 32, 644, DateTimeKind.Local).AddTicks(3579), new DateTime(2026, 4, 21, 10, 35, 32, 644, DateTimeKind.Local).AddTicks(3580) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 21, 10, 35, 32, 644, DateTimeKind.Local).AddTicks(3595), new DateTime(2026, 4, 21, 10, 35, 32, 644, DateTimeKind.Local).AddTicks(3596) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 21, 10, 35, 32, 644, DateTimeKind.Local).AddTicks(3611), new DateTime(2026, 4, 21, 10, 35, 32, 644, DateTimeKind.Local).AddTicks(3612) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 21, 10, 35, 32, 644, DateTimeKind.Local).AddTicks(3626), new DateTime(2026, 4, 21, 10, 35, 32, 644, DateTimeKind.Local).AddTicks(3626) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 6,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 21, 10, 35, 32, 644, DateTimeKind.Local).AddTicks(3640), new DateTime(2026, 4, 21, 10, 35, 32, 644, DateTimeKind.Local).AddTicks(3641) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 7,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 21, 10, 35, 32, 644, DateTimeKind.Local).AddTicks(3655), new DateTime(2026, 4, 21, 10, 35, 32, 644, DateTimeKind.Local).AddTicks(3655) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 8,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 21, 10, 35, 32, 644, DateTimeKind.Local).AddTicks(3669), new DateTime(2026, 4, 21, 10, 35, 32, 644, DateTimeKind.Local).AddTicks(3670) });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "ConcurrencyStamp", "Created", "Modified", "PasswordHash" },
                values: new object[] { "ba82375f-9319-47ab-a109-5b00021033be", new DateTime(2026, 4, 21, 10, 35, 32, 644, DateTimeKind.Local).AddTicks(3992), new DateTime(2026, 4, 21, 10, 35, 32, 644, DateTimeKind.Local).AddTicks(3994), "AQAAAAIAAYagAAAAEOIYw9xmIQsTGgty76e1Zu6jDkBxb0q4oWUa1+OAqS928otdgVBRJQOAX9XuD7DfYg==" });

            migrationBuilder.UpdateData(
                table: "Designations",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 21, 10, 35, 32, 644, DateTimeKind.Local).AddTicks(3776), new DateTime(2026, 4, 21, 10, 35, 32, 644, DateTimeKind.Local).AddTicks(3776) });

            migrationBuilder.UpdateData(
                table: "EmploymentTypes",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 21, 10, 35, 32, 644, DateTimeKind.Local).AddTicks(3793), new DateTime(2026, 4, 21, 10, 35, 32, 644, DateTimeKind.Local).AddTicks(3795) });

            migrationBuilder.UpdateData(
                table: "Genders",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 21, 10, 35, 32, 644, DateTimeKind.Local).AddTicks(3846), new DateTime(2026, 4, 21, 10, 35, 32, 644, DateTimeKind.Local).AddTicks(3847) });

            migrationBuilder.UpdateData(
                table: "Nationalities",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 21, 10, 35, 32, 644, DateTimeKind.Local).AddTicks(3811), new DateTime(2026, 4, 21, 10, 35, 32, 644, DateTimeKind.Local).AddTicks(3811) });

            migrationBuilder.UpdateData(
                table: "Person",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "DateOfBirth", "EmploymentDate", "EndofEmploymentDate", "Modified" },
                values: new object[] { new DateTime(2026, 4, 21, 10, 35, 32, 644, DateTimeKind.Local).AddTicks(3889), new DateTime(2026, 4, 21, 10, 35, 32, 644, DateTimeKind.Local).AddTicks(3887), new DateTime(2026, 4, 21, 10, 35, 32, 644, DateTimeKind.Local).AddTicks(3879), new DateTime(2026, 4, 21, 10, 35, 32, 644, DateTimeKind.Local).AddTicks(3880), new DateTime(2026, 4, 21, 10, 35, 32, 644, DateTimeKind.Local).AddTicks(3890) });

            migrationBuilder.UpdateData(
                table: "Religions",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 21, 10, 35, 32, 644, DateTimeKind.Local).AddTicks(3829), new DateTime(2026, 4, 21, 10, 35, 32, 644, DateTimeKind.Local).AddTicks(3830) });

            migrationBuilder.UpdateData(
                table: "StaffCategories",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 21, 10, 35, 32, 644, DateTimeKind.Local).AddTicks(3717), new DateTime(2026, 4, 21, 10, 35, 32, 644, DateTimeKind.Local).AddTicks(3718) });
        }
    }
}
