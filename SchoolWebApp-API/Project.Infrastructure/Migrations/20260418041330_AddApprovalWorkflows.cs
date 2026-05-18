using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace SchoolWebApp.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddApprovalWorkflows : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ApprovalWorkflows",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    FormKey = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Description = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IsMakerChecker = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    IsActive = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    Created = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    CreatedBy = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Modified = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    ModifiedBy = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApprovalWorkflows", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "ApprovalRequests",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    ApprovalWorkflowId = table.Column<int>(type: "int", nullable: false),
                    EntityType = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    EntityId = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    SubmittedById = table.Column<int>(type: "int", nullable: true),
                    SubmittedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    CurrentStepRank = table.Column<int>(type: "int", nullable: false),
                    Created = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    CreatedBy = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Modified = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    ModifiedBy = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApprovalRequests", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ApprovalRequests_ApprovalWorkflows_ApprovalWorkflowId",
                        column: x => x.ApprovalWorkflowId,
                        principalTable: "ApprovalWorkflows",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ApprovalRequests_AspNetUsers_SubmittedById",
                        column: x => x.SubmittedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "ApprovalWorkflowSteps",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    ApprovalWorkflowId = table.Column<int>(type: "int", nullable: false),
                    Rank = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    RoleId = table.Column<int>(type: "int", nullable: false),
                    IsFinal = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    NotifyNextApprover = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    NotifyPreviousApprover = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    NotifyApplicant = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    Created = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    CreatedBy = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Modified = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    ModifiedBy = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApprovalWorkflowSteps", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ApprovalWorkflowSteps_ApprovalWorkflows_ApprovalWorkflowId",
                        column: x => x.ApprovalWorkflowId,
                        principalTable: "ApprovalWorkflows",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ApprovalWorkflowSteps_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "ApprovalStepActions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    ApprovalRequestId = table.Column<int>(type: "int", nullable: false),
                    StepRank = table.Column<int>(type: "int", nullable: false),
                    StepName = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    AssignedToUserId = table.Column<int>(type: "int", nullable: false),
                    ActionedByUserId = table.Column<int>(type: "int", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    Comment = table.Column<string>(type: "varchar(1000)", maxLength: 1000, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ActionedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    Created = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    CreatedBy = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Modified = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    ModifiedBy = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApprovalStepActions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ApprovalStepActions_ApprovalRequests_ApprovalRequestId",
                        column: x => x.ApprovalRequestId,
                        principalTable: "ApprovalRequests",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ApprovalStepActions_AspNetUsers_ActionedByUserId",
                        column: x => x.ActionedByUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ApprovalStepActions_AspNetUsers_AssignedToUserId",
                        column: x => x.AssignedToUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.InsertData(
                table: "ApprovalWorkflows",
                columns: new[] { "Id", "Created", "CreatedBy", "Description", "FormKey", "IsActive", "IsMakerChecker", "Modified", "ModifiedBy", "Name" },
                values: new object[,]
                {
                    { 1, new DateTime(2026, 4, 18, 7, 13, 28, 369, DateTimeKind.Local).AddTicks(6617), "admin", "Default budget amendment approval workflow", "BudgetAmendment", true, true, new DateTime(2026, 4, 18, 7, 13, 28, 369, DateTimeKind.Local).AddTicks(6617), "admin", "Budget Amendment Approval" },
                    { 2, new DateTime(2026, 4, 18, 7, 13, 28, 369, DateTimeKind.Local).AddTicks(6617), "admin", "Default expense approval workflow", "Expense", true, true, new DateTime(2026, 4, 18, 7, 13, 28, 369, DateTimeKind.Local).AddTicks(6617), "admin", "Expense Approval" },
                    { 3, new DateTime(2026, 4, 18, 7, 13, 28, 369, DateTimeKind.Local).AddTicks(6617), "admin", "Default journal entry approval workflow", "JournalEntry", true, true, new DateTime(2026, 4, 18, 7, 13, 28, 369, DateTimeKind.Local).AddTicks(6617), "admin", "Journal Entry Approval" },
                    { 4, new DateTime(2026, 4, 18, 7, 13, 28, 369, DateTimeKind.Local).AddTicks(6617), "admin", "Default credit/debit note approval workflow", "CreditDebitNote", true, true, new DateTime(2026, 4, 18, 7, 13, 28, 369, DateTimeKind.Local).AddTicks(6617), "admin", "Credit/Debit Note Approval" }
                });

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

            migrationBuilder.InsertData(
                table: "ApprovalWorkflowSteps",
                columns: new[] { "Id", "ApprovalWorkflowId", "Created", "CreatedBy", "IsFinal", "Modified", "ModifiedBy", "Name", "NotifyApplicant", "NotifyNextApprover", "NotifyPreviousApprover", "Rank", "RoleId" },
                values: new object[,]
                {
                    { 1, 1, new DateTime(2026, 4, 18, 7, 13, 28, 369, DateTimeKind.Local).AddTicks(6617), "admin", false, new DateTime(2026, 4, 18, 7, 13, 28, 369, DateTimeKind.Local).AddTicks(6617), "admin", "Reviewer", true, true, false, 1, 1 },
                    { 2, 1, new DateTime(2026, 4, 18, 7, 13, 28, 369, DateTimeKind.Local).AddTicks(6617), "admin", true, new DateTime(2026, 4, 18, 7, 13, 28, 369, DateTimeKind.Local).AddTicks(6617), "admin", "Approver", true, false, true, 2, 1 },
                    { 3, 2, new DateTime(2026, 4, 18, 7, 13, 28, 369, DateTimeKind.Local).AddTicks(6617), "admin", false, new DateTime(2026, 4, 18, 7, 13, 28, 369, DateTimeKind.Local).AddTicks(6617), "admin", "Reviewer", true, true, false, 1, 1 },
                    { 4, 2, new DateTime(2026, 4, 18, 7, 13, 28, 369, DateTimeKind.Local).AddTicks(6617), "admin", true, new DateTime(2026, 4, 18, 7, 13, 28, 369, DateTimeKind.Local).AddTicks(6617), "admin", "Approver", true, false, true, 2, 1 },
                    { 5, 3, new DateTime(2026, 4, 18, 7, 13, 28, 369, DateTimeKind.Local).AddTicks(6617), "admin", false, new DateTime(2026, 4, 18, 7, 13, 28, 369, DateTimeKind.Local).AddTicks(6617), "admin", "Reviewer", true, true, false, 1, 1 },
                    { 6, 3, new DateTime(2026, 4, 18, 7, 13, 28, 369, DateTimeKind.Local).AddTicks(6617), "admin", true, new DateTime(2026, 4, 18, 7, 13, 28, 369, DateTimeKind.Local).AddTicks(6617), "admin", "Approver", true, false, true, 2, 1 },
                    { 7, 4, new DateTime(2026, 4, 18, 7, 13, 28, 369, DateTimeKind.Local).AddTicks(6617), "admin", false, new DateTime(2026, 4, 18, 7, 13, 28, 369, DateTimeKind.Local).AddTicks(6617), "admin", "Reviewer", true, true, false, 1, 1 },
                    { 8, 4, new DateTime(2026, 4, 18, 7, 13, 28, 369, DateTimeKind.Local).AddTicks(6617), "admin", true, new DateTime(2026, 4, 18, 7, 13, 28, 369, DateTimeKind.Local).AddTicks(6617), "admin", "Approver", true, false, true, 2, 1 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_ApprovalRequests_ApprovalWorkflowId",
                table: "ApprovalRequests",
                column: "ApprovalWorkflowId");

            migrationBuilder.CreateIndex(
                name: "IX_ApprovalRequests_EntityType_EntityId",
                table: "ApprovalRequests",
                columns: new[] { "EntityType", "EntityId" });

            migrationBuilder.CreateIndex(
                name: "IX_ApprovalRequests_SubmittedById",
                table: "ApprovalRequests",
                column: "SubmittedById");

            migrationBuilder.CreateIndex(
                name: "IX_ApprovalStepActions_ActionedByUserId",
                table: "ApprovalStepActions",
                column: "ActionedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_ApprovalStepActions_ApprovalRequestId",
                table: "ApprovalStepActions",
                column: "ApprovalRequestId");

            migrationBuilder.CreateIndex(
                name: "IX_ApprovalStepActions_AssignedToUserId",
                table: "ApprovalStepActions",
                column: "AssignedToUserId");

            migrationBuilder.CreateIndex(
                name: "IX_ApprovalWorkflowSteps_ApprovalWorkflowId",
                table: "ApprovalWorkflowSteps",
                column: "ApprovalWorkflowId");

            migrationBuilder.CreateIndex(
                name: "IX_ApprovalWorkflowSteps_RoleId",
                table: "ApprovalWorkflowSteps",
                column: "RoleId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ApprovalStepActions");

            migrationBuilder.DropTable(
                name: "ApprovalWorkflowSteps");

            migrationBuilder.DropTable(
                name: "ApprovalRequests");

            migrationBuilder.DropTable(
                name: "ApprovalWorkflows");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 17, 21, 59, 11, 914, DateTimeKind.Local).AddTicks(8617), new DateTime(2026, 4, 17, 21, 59, 11, 914, DateTimeKind.Local).AddTicks(8629) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 17, 21, 59, 11, 914, DateTimeKind.Local).AddTicks(8706), new DateTime(2026, 4, 17, 21, 59, 11, 914, DateTimeKind.Local).AddTicks(8707) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 17, 21, 59, 11, 914, DateTimeKind.Local).AddTicks(8723), new DateTime(2026, 4, 17, 21, 59, 11, 914, DateTimeKind.Local).AddTicks(8724) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 17, 21, 59, 11, 914, DateTimeKind.Local).AddTicks(8738), new DateTime(2026, 4, 17, 21, 59, 11, 914, DateTimeKind.Local).AddTicks(8739) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 17, 21, 59, 11, 914, DateTimeKind.Local).AddTicks(8752), new DateTime(2026, 4, 17, 21, 59, 11, 914, DateTimeKind.Local).AddTicks(8753) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 6,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 17, 21, 59, 11, 914, DateTimeKind.Local).AddTicks(8772), new DateTime(2026, 4, 17, 21, 59, 11, 914, DateTimeKind.Local).AddTicks(8773) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 7,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 17, 21, 59, 11, 914, DateTimeKind.Local).AddTicks(8791), new DateTime(2026, 4, 17, 21, 59, 11, 914, DateTimeKind.Local).AddTicks(8792) });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "ConcurrencyStamp", "Created", "Modified", "PasswordHash" },
                values: new object[] { "80c4648d-30e6-43a0-a929-4951b09014d7", new DateTime(2026, 4, 17, 21, 59, 11, 914, DateTimeKind.Local).AddTicks(9099), new DateTime(2026, 4, 17, 21, 59, 11, 914, DateTimeKind.Local).AddTicks(9100), "AQAAAAIAAYagAAAAEH9cVUFxDilrpUOhztQWVemD7beJHX+fUTq02WmG/OXudcuBAnOSMh8nQD7DFNf/iw==" });

            migrationBuilder.UpdateData(
                table: "Designations",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 17, 21, 59, 11, 914, DateTimeKind.Local).AddTicks(8865), new DateTime(2026, 4, 17, 21, 59, 11, 914, DateTimeKind.Local).AddTicks(8865) });

            migrationBuilder.UpdateData(
                table: "EmploymentTypes",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 17, 21, 59, 11, 914, DateTimeKind.Local).AddTicks(8888), new DateTime(2026, 4, 17, 21, 59, 11, 914, DateTimeKind.Local).AddTicks(8888) });

            migrationBuilder.UpdateData(
                table: "Genders",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 17, 21, 59, 11, 914, DateTimeKind.Local).AddTicks(8947), new DateTime(2026, 4, 17, 21, 59, 11, 914, DateTimeKind.Local).AddTicks(8947) });

            migrationBuilder.UpdateData(
                table: "Nationalities",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 17, 21, 59, 11, 914, DateTimeKind.Local).AddTicks(8907), new DateTime(2026, 4, 17, 21, 59, 11, 914, DateTimeKind.Local).AddTicks(8907) });

            migrationBuilder.UpdateData(
                table: "Person",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "DateOfBirth", "EmploymentDate", "EndofEmploymentDate", "Modified" },
                values: new object[] { new DateTime(2026, 4, 17, 21, 59, 11, 914, DateTimeKind.Local).AddTicks(8986), new DateTime(2026, 4, 17, 21, 59, 11, 914, DateTimeKind.Local).AddTicks(8983), new DateTime(2026, 4, 17, 21, 59, 11, 914, DateTimeKind.Local).AddTicks(8979), new DateTime(2026, 4, 17, 21, 59, 11, 914, DateTimeKind.Local).AddTicks(8979), new DateTime(2026, 4, 17, 21, 59, 11, 914, DateTimeKind.Local).AddTicks(8991) });

            migrationBuilder.UpdateData(
                table: "Religions",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 17, 21, 59, 11, 914, DateTimeKind.Local).AddTicks(8927), new DateTime(2026, 4, 17, 21, 59, 11, 914, DateTimeKind.Local).AddTicks(8928) });

            migrationBuilder.UpdateData(
                table: "StaffCategories",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 17, 21, 59, 11, 914, DateTimeKind.Local).AddTicks(8827), new DateTime(2026, 4, 17, 21, 59, 11, 914, DateTimeKind.Local).AddTicks(8829) });
        }
    }
}
