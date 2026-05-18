using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace SchoolWebApp.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddBudgetApprovalWorkflow : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "ApprovalWorkflowSteps",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 18, 8, 31, 54, 369, DateTimeKind.Local).AddTicks(327), new DateTime(2026, 4, 18, 8, 31, 54, 369, DateTimeKind.Local).AddTicks(327) });

            migrationBuilder.UpdateData(
                table: "ApprovalWorkflowSteps",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 18, 8, 31, 54, 369, DateTimeKind.Local).AddTicks(327), new DateTime(2026, 4, 18, 8, 31, 54, 369, DateTimeKind.Local).AddTicks(327) });

            migrationBuilder.UpdateData(
                table: "ApprovalWorkflowSteps",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 18, 8, 31, 54, 369, DateTimeKind.Local).AddTicks(327), new DateTime(2026, 4, 18, 8, 31, 54, 369, DateTimeKind.Local).AddTicks(327) });

            migrationBuilder.UpdateData(
                table: "ApprovalWorkflowSteps",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 18, 8, 31, 54, 369, DateTimeKind.Local).AddTicks(327), new DateTime(2026, 4, 18, 8, 31, 54, 369, DateTimeKind.Local).AddTicks(327) });

            migrationBuilder.UpdateData(
                table: "ApprovalWorkflowSteps",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 18, 8, 31, 54, 369, DateTimeKind.Local).AddTicks(327), new DateTime(2026, 4, 18, 8, 31, 54, 369, DateTimeKind.Local).AddTicks(327) });

            migrationBuilder.UpdateData(
                table: "ApprovalWorkflowSteps",
                keyColumn: "Id",
                keyValue: 6,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 18, 8, 31, 54, 369, DateTimeKind.Local).AddTicks(327), new DateTime(2026, 4, 18, 8, 31, 54, 369, DateTimeKind.Local).AddTicks(327) });

            migrationBuilder.UpdateData(
                table: "ApprovalWorkflowSteps",
                keyColumn: "Id",
                keyValue: 7,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 18, 8, 31, 54, 369, DateTimeKind.Local).AddTicks(327), new DateTime(2026, 4, 18, 8, 31, 54, 369, DateTimeKind.Local).AddTicks(327) });

            migrationBuilder.UpdateData(
                table: "ApprovalWorkflowSteps",
                keyColumn: "Id",
                keyValue: 8,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 18, 8, 31, 54, 369, DateTimeKind.Local).AddTicks(327), new DateTime(2026, 4, 18, 8, 31, 54, 369, DateTimeKind.Local).AddTicks(327) });

            migrationBuilder.UpdateData(
                table: "ApprovalWorkflows",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 18, 8, 31, 54, 369, DateTimeKind.Local).AddTicks(327), new DateTime(2026, 4, 18, 8, 31, 54, 369, DateTimeKind.Local).AddTicks(327) });

            migrationBuilder.UpdateData(
                table: "ApprovalWorkflows",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 18, 8, 31, 54, 369, DateTimeKind.Local).AddTicks(327), new DateTime(2026, 4, 18, 8, 31, 54, 369, DateTimeKind.Local).AddTicks(327) });

            migrationBuilder.UpdateData(
                table: "ApprovalWorkflows",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 18, 8, 31, 54, 369, DateTimeKind.Local).AddTicks(327), new DateTime(2026, 4, 18, 8, 31, 54, 369, DateTimeKind.Local).AddTicks(327) });

            migrationBuilder.UpdateData(
                table: "ApprovalWorkflows",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 18, 8, 31, 54, 369, DateTimeKind.Local).AddTicks(327), new DateTime(2026, 4, 18, 8, 31, 54, 369, DateTimeKind.Local).AddTicks(327) });

            migrationBuilder.Sql("DELETE FROM ApprovalWorkflows WHERE Id = 5;");
            migrationBuilder.InsertData(
                table: "ApprovalWorkflows",
                columns: new[] { "Id", "Created", "CreatedBy", "Description", "FormKey", "IsActive", "IsMakerChecker", "Modified", "ModifiedBy", "Name" },
                values: new object[] { 5, new DateTime(2026, 4, 18, 8, 31, 54, 369, DateTimeKind.Local).AddTicks(327), "admin", "Default budget approval workflow", "Budget", true, true, new DateTime(2026, 4, 18, 8, 31, 54, 369, DateTimeKind.Local).AddTicks(327), "admin", "Budget Approval" });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 18, 8, 31, 54, 320, DateTimeKind.Local).AddTicks(8053), new DateTime(2026, 4, 18, 8, 31, 54, 320, DateTimeKind.Local).AddTicks(8071) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 18, 8, 31, 54, 320, DateTimeKind.Local).AddTicks(8132), new DateTime(2026, 4, 18, 8, 31, 54, 320, DateTimeKind.Local).AddTicks(8134) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 18, 8, 31, 54, 320, DateTimeKind.Local).AddTicks(8164), new DateTime(2026, 4, 18, 8, 31, 54, 320, DateTimeKind.Local).AddTicks(8166) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 18, 8, 31, 54, 320, DateTimeKind.Local).AddTicks(8191), new DateTime(2026, 4, 18, 8, 31, 54, 320, DateTimeKind.Local).AddTicks(8193) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 18, 8, 31, 54, 320, DateTimeKind.Local).AddTicks(8217), new DateTime(2026, 4, 18, 8, 31, 54, 320, DateTimeKind.Local).AddTicks(8218) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 6,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 18, 8, 31, 54, 320, DateTimeKind.Local).AddTicks(8246), new DateTime(2026, 4, 18, 8, 31, 54, 320, DateTimeKind.Local).AddTicks(8248) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 7,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 18, 8, 31, 54, 320, DateTimeKind.Local).AddTicks(8272), new DateTime(2026, 4, 18, 8, 31, 54, 320, DateTimeKind.Local).AddTicks(8274) });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "ConcurrencyStamp", "Created", "Modified", "PasswordHash" },
                values: new object[] { "d74d0c00-9ba6-476b-a4cd-9d94f95c01c9", new DateTime(2026, 4, 18, 8, 31, 54, 320, DateTimeKind.Local).AddTicks(8723), new DateTime(2026, 4, 18, 8, 31, 54, 320, DateTimeKind.Local).AddTicks(8726), "AQAAAAIAAYagAAAAEJUd6pcSQU2Al6dYSTEHju+AO6O2jl2ERJfBCXKOwE4FdA99b5M2MFospIv8joMSsg==" });

            migrationBuilder.UpdateData(
                table: "Designations",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 18, 8, 31, 54, 320, DateTimeKind.Local).AddTicks(8358), new DateTime(2026, 4, 18, 8, 31, 54, 320, DateTimeKind.Local).AddTicks(8359) });

            migrationBuilder.UpdateData(
                table: "EmploymentTypes",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 18, 8, 31, 54, 320, DateTimeKind.Local).AddTicks(8392), new DateTime(2026, 4, 18, 8, 31, 54, 320, DateTimeKind.Local).AddTicks(8393) });

            migrationBuilder.UpdateData(
                table: "Genders",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 18, 8, 31, 54, 320, DateTimeKind.Local).AddTicks(8521), new DateTime(2026, 4, 18, 8, 31, 54, 320, DateTimeKind.Local).AddTicks(8522) });

            migrationBuilder.UpdateData(
                table: "Nationalities",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 18, 8, 31, 54, 320, DateTimeKind.Local).AddTicks(8419), new DateTime(2026, 4, 18, 8, 31, 54, 320, DateTimeKind.Local).AddTicks(8420) });

            migrationBuilder.UpdateData(
                table: "Person",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "DateOfBirth", "EmploymentDate", "EndofEmploymentDate", "Modified" },
                values: new object[] { new DateTime(2026, 4, 18, 8, 31, 54, 320, DateTimeKind.Local).AddTicks(8572), new DateTime(2026, 4, 18, 8, 31, 54, 320, DateTimeKind.Local).AddTicks(8569), new DateTime(2026, 4, 18, 8, 31, 54, 320, DateTimeKind.Local).AddTicks(8564), new DateTime(2026, 4, 18, 8, 31, 54, 320, DateTimeKind.Local).AddTicks(8565), new DateTime(2026, 4, 18, 8, 31, 54, 320, DateTimeKind.Local).AddTicks(8577) });

            migrationBuilder.UpdateData(
                table: "Religions",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 18, 8, 31, 54, 320, DateTimeKind.Local).AddTicks(8495), new DateTime(2026, 4, 18, 8, 31, 54, 320, DateTimeKind.Local).AddTicks(8497) });

            migrationBuilder.UpdateData(
                table: "StaffCategories",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 18, 8, 31, 54, 320, DateTimeKind.Local).AddTicks(8322), new DateTime(2026, 4, 18, 8, 31, 54, 320, DateTimeKind.Local).AddTicks(8324) });

            migrationBuilder.Sql("DELETE FROM ApprovalWorkflowSteps WHERE Id IN (9, 10);");
            migrationBuilder.InsertData(
                table: "ApprovalWorkflowSteps",
                columns: new[] { "Id", "ApprovalWorkflowId", "Created", "CreatedBy", "IsFinal", "Modified", "ModifiedBy", "Name", "NotifyApplicant", "NotifyNextApprover", "NotifyPreviousApprover", "Rank", "RoleId" },
                values: new object[,]
                {
                    { 9, 5, new DateTime(2026, 4, 18, 8, 31, 54, 369, DateTimeKind.Local).AddTicks(327), "admin", false, new DateTime(2026, 4, 18, 8, 31, 54, 369, DateTimeKind.Local).AddTicks(327), "admin", "Reviewer", true, true, false, 1, 1 },
                    { 10, 5, new DateTime(2026, 4, 18, 8, 31, 54, 369, DateTimeKind.Local).AddTicks(327), "admin", true, new DateTime(2026, 4, 18, 8, 31, 54, 369, DateTimeKind.Local).AddTicks(327), "admin", "Approver", true, false, true, 2, 1 }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "ApprovalWorkflowSteps",
                keyColumn: "Id",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "ApprovalWorkflowSteps",
                keyColumn: "Id",
                keyValue: 10);

            migrationBuilder.DeleteData(
                table: "ApprovalWorkflows",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.UpdateData(
                table: "ApprovalWorkflowSteps",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 18, 7, 13, 28, 369, DateTimeKind.Local).AddTicks(6617), new DateTime(2026, 4, 18, 7, 13, 28, 369, DateTimeKind.Local).AddTicks(6617) });

            migrationBuilder.UpdateData(
                table: "ApprovalWorkflowSteps",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 18, 7, 13, 28, 369, DateTimeKind.Local).AddTicks(6617), new DateTime(2026, 4, 18, 7, 13, 28, 369, DateTimeKind.Local).AddTicks(6617) });

            migrationBuilder.UpdateData(
                table: "ApprovalWorkflowSteps",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 18, 7, 13, 28, 369, DateTimeKind.Local).AddTicks(6617), new DateTime(2026, 4, 18, 7, 13, 28, 369, DateTimeKind.Local).AddTicks(6617) });

            migrationBuilder.UpdateData(
                table: "ApprovalWorkflowSteps",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 18, 7, 13, 28, 369, DateTimeKind.Local).AddTicks(6617), new DateTime(2026, 4, 18, 7, 13, 28, 369, DateTimeKind.Local).AddTicks(6617) });

            migrationBuilder.UpdateData(
                table: "ApprovalWorkflowSteps",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 18, 7, 13, 28, 369, DateTimeKind.Local).AddTicks(6617), new DateTime(2026, 4, 18, 7, 13, 28, 369, DateTimeKind.Local).AddTicks(6617) });

            migrationBuilder.UpdateData(
                table: "ApprovalWorkflowSteps",
                keyColumn: "Id",
                keyValue: 6,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 18, 7, 13, 28, 369, DateTimeKind.Local).AddTicks(6617), new DateTime(2026, 4, 18, 7, 13, 28, 369, DateTimeKind.Local).AddTicks(6617) });

            migrationBuilder.UpdateData(
                table: "ApprovalWorkflowSteps",
                keyColumn: "Id",
                keyValue: 7,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 18, 7, 13, 28, 369, DateTimeKind.Local).AddTicks(6617), new DateTime(2026, 4, 18, 7, 13, 28, 369, DateTimeKind.Local).AddTicks(6617) });

            migrationBuilder.UpdateData(
                table: "ApprovalWorkflowSteps",
                keyColumn: "Id",
                keyValue: 8,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 18, 7, 13, 28, 369, DateTimeKind.Local).AddTicks(6617), new DateTime(2026, 4, 18, 7, 13, 28, 369, DateTimeKind.Local).AddTicks(6617) });

            migrationBuilder.UpdateData(
                table: "ApprovalWorkflows",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 18, 7, 13, 28, 369, DateTimeKind.Local).AddTicks(6617), new DateTime(2026, 4, 18, 7, 13, 28, 369, DateTimeKind.Local).AddTicks(6617) });

            migrationBuilder.UpdateData(
                table: "ApprovalWorkflows",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 18, 7, 13, 28, 369, DateTimeKind.Local).AddTicks(6617), new DateTime(2026, 4, 18, 7, 13, 28, 369, DateTimeKind.Local).AddTicks(6617) });

            migrationBuilder.UpdateData(
                table: "ApprovalWorkflows",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 18, 7, 13, 28, 369, DateTimeKind.Local).AddTicks(6617), new DateTime(2026, 4, 18, 7, 13, 28, 369, DateTimeKind.Local).AddTicks(6617) });

            migrationBuilder.UpdateData(
                table: "ApprovalWorkflows",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 18, 7, 13, 28, 369, DateTimeKind.Local).AddTicks(6617), new DateTime(2026, 4, 18, 7, 13, 28, 369, DateTimeKind.Local).AddTicks(6617) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 18, 7, 13, 28, 333, DateTimeKind.Local).AddTicks(723), new DateTime(2026, 4, 18, 7, 13, 28, 333, DateTimeKind.Local).AddTicks(737) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 18, 7, 13, 28, 333, DateTimeKind.Local).AddTicks(784), new DateTime(2026, 4, 18, 7, 13, 28, 333, DateTimeKind.Local).AddTicks(785) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 18, 7, 13, 28, 333, DateTimeKind.Local).AddTicks(801), new DateTime(2026, 4, 18, 7, 13, 28, 333, DateTimeKind.Local).AddTicks(802) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 18, 7, 13, 28, 333, DateTimeKind.Local).AddTicks(816), new DateTime(2026, 4, 18, 7, 13, 28, 333, DateTimeKind.Local).AddTicks(816) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 18, 7, 13, 28, 333, DateTimeKind.Local).AddTicks(831), new DateTime(2026, 4, 18, 7, 13, 28, 333, DateTimeKind.Local).AddTicks(831) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 6,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 18, 7, 13, 28, 333, DateTimeKind.Local).AddTicks(847), new DateTime(2026, 4, 18, 7, 13, 28, 333, DateTimeKind.Local).AddTicks(847) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 7,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 18, 7, 13, 28, 333, DateTimeKind.Local).AddTicks(861), new DateTime(2026, 4, 18, 7, 13, 28, 333, DateTimeKind.Local).AddTicks(862) });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "ConcurrencyStamp", "Created", "Modified", "PasswordHash" },
                values: new object[] { "633f2909-9ccf-4857-a0ca-dbae5a2946f5", new DateTime(2026, 4, 18, 7, 13, 28, 333, DateTimeKind.Local).AddTicks(1164), new DateTime(2026, 4, 18, 7, 13, 28, 333, DateTimeKind.Local).AddTicks(1165), "AQAAAAIAAYagAAAAEKiH4LQPUxqS+YVLrmvzfM8ThAbcXdHX4MxiAh1yFzqAdhiO7To6T3f1cynLMRjp1g==" });

            migrationBuilder.UpdateData(
                table: "Designations",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 18, 7, 13, 28, 333, DateTimeKind.Local).AddTicks(926), new DateTime(2026, 4, 18, 7, 13, 28, 333, DateTimeKind.Local).AddTicks(926) });

            migrationBuilder.UpdateData(
                table: "EmploymentTypes",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 18, 7, 13, 28, 333, DateTimeKind.Local).AddTicks(951), new DateTime(2026, 4, 18, 7, 13, 28, 333, DateTimeKind.Local).AddTicks(951) });

            migrationBuilder.UpdateData(
                table: "Genders",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 18, 7, 13, 28, 333, DateTimeKind.Local).AddTicks(1014), new DateTime(2026, 4, 18, 7, 13, 28, 333, DateTimeKind.Local).AddTicks(1015) });

            migrationBuilder.UpdateData(
                table: "Nationalities",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 18, 7, 13, 28, 333, DateTimeKind.Local).AddTicks(969), new DateTime(2026, 4, 18, 7, 13, 28, 333, DateTimeKind.Local).AddTicks(970) });

            migrationBuilder.UpdateData(
                table: "Person",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "DateOfBirth", "EmploymentDate", "EndofEmploymentDate", "Modified" },
                values: new object[] { new DateTime(2026, 4, 18, 7, 13, 28, 333, DateTimeKind.Local).AddTicks(1048), new DateTime(2026, 4, 18, 7, 13, 28, 333, DateTimeKind.Local).AddTicks(1045), new DateTime(2026, 4, 18, 7, 13, 28, 333, DateTimeKind.Local).AddTicks(1042), new DateTime(2026, 4, 18, 7, 13, 28, 333, DateTimeKind.Local).AddTicks(1042), new DateTime(2026, 4, 18, 7, 13, 28, 333, DateTimeKind.Local).AddTicks(1052) });

            migrationBuilder.UpdateData(
                table: "Religions",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 18, 7, 13, 28, 333, DateTimeKind.Local).AddTicks(995), new DateTime(2026, 4, 18, 7, 13, 28, 333, DateTimeKind.Local).AddTicks(996) });

            migrationBuilder.UpdateData(
                table: "StaffCategories",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 18, 7, 13, 28, 333, DateTimeKind.Local).AddTicks(899), new DateTime(2026, 4, 18, 7, 13, 28, 333, DateTimeKind.Local).AddTicks(900) });
        }
    }
}
