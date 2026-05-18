using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SchoolWebApp.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddSponsorshipsModule : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Drop any partial sponsorship tables from a previous failed migration attempt.
            migrationBuilder.Sql(@"
                SET FOREIGN_KEY_CHECKS = 0;
                DROP TABLE IF EXISTS SponsorPayments;
                DROP TABLE IF EXISTS SponsorshipFeeCategories;
                DROP TABLE IF EXISTS Sponsorships;
                DROP TABLE IF EXISTS Sponsors;
                SET FOREIGN_KEY_CHECKS = 1;
            ");

            // DatabaseBootstrapper may have already dropped and recreated these FKs with RESTRICT at runtime.
            // Drop them only if they still exist under the EF-generated name.
            migrationBuilder.Sql(@"
                SET @sql = (SELECT IF(COUNT(*) > 0,
                    'ALTER TABLE ApprovalStepActions DROP FOREIGN KEY FK_ApprovalStepActions_ApprovalRequests_ApprovalRequestId',
                    'SELECT 1')
                    FROM information_schema.TABLE_CONSTRAINTS
                    WHERE TABLE_SCHEMA = DATABASE() AND TABLE_NAME = 'ApprovalStepActions'
                      AND CONSTRAINT_NAME = 'FK_ApprovalStepActions_ApprovalRequests_ApprovalRequestId');
                PREPARE stmt FROM @sql; EXECUTE stmt; DEALLOCATE PREPARE stmt;

                SET @sql = (SELECT IF(COUNT(*) > 0,
                    'ALTER TABLE ApprovalWorkflowSteps DROP FOREIGN KEY FK_ApprovalWorkflowSteps_ApprovalWorkflows_ApprovalWorkflowId',
                    'SELECT 1')
                    FROM information_schema.TABLE_CONSTRAINTS
                    WHERE TABLE_SCHEMA = DATABASE() AND TABLE_NAME = 'ApprovalWorkflowSteps'
                      AND CONSTRAINT_NAME = 'FK_ApprovalWorkflowSteps_ApprovalWorkflows_ApprovalWorkflowId');
                PREPARE stmt FROM @sql; EXECUTE stmt; DEALLOCATE PREPARE stmt;
            ");

            // Column may have been added by a partial prior attempt — guard it.
            migrationBuilder.Sql(@"
                SET @sql = (SELECT IF(COUNT(*) = 0,
                    'ALTER TABLE StudentInvoiceItems ADD COLUMN SponsorshipId int NULL',
                    'SELECT 1')
                    FROM information_schema.COLUMNS
                    WHERE TABLE_SCHEMA = DATABASE() AND TABLE_NAME = 'StudentInvoiceItems' AND COLUMN_NAME = 'SponsorshipId');
                PREPARE stmt FROM @sql; EXECUTE stmt; DEALLOCATE PREPARE stmt;
            ");

            migrationBuilder.CreateTable(
                name: "Sponsors",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(type: "varchar(200)", maxLength: 200, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Description = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    SponsorType = table.Column<int>(type: "int", nullable: false),
                    ContactName = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Email = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Phone = table.Column<string>(type: "varchar(30)", maxLength: 30, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Address = table.Column<string>(type: "varchar(300)", maxLength: 300, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ReceivableAccountId = table.Column<int>(type: "int", nullable: true),
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
                    table.PrimaryKey("PK_Sponsors", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Sponsors_Accounts_ReceivableAccountId",
                        column: x => x.ReceivableAccountId,
                        principalTable: "Accounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "SponsorPayments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    SponsorId = table.Column<int>(type: "int", nullable: false),
                    ReferenceNumber = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    PaymentDate = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    PaymentMethod = table.Column<int>(type: "int", nullable: false),
                    TransactionReference = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    BankAccountId = table.Column<int>(type: "int", nullable: true),
                    Description = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Created = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    CreatedBy = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Modified = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    ModifiedBy = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SponsorPayments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SponsorPayments_Accounts_BankAccountId",
                        column: x => x.BankAccountId,
                        principalTable: "Accounts",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_SponsorPayments_Sponsors_SponsorId",
                        column: x => x.SponsorId,
                        principalTable: "Sponsors",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Sponsorships",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    SponsorId = table.Column<int>(type: "int", nullable: false),
                    StudentId = table.Column<int>(type: "int", nullable: true),
                    SchoolClassId = table.Column<int>(type: "int", nullable: true),
                    AcademicYearId = table.Column<int>(type: "int", nullable: false),
                    SessionId = table.Column<int>(type: "int", nullable: true),
                    CoverageType = table.Column<int>(type: "int", nullable: false),
                    FixedAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Percentage = table.Column<decimal>(type: "decimal(5,2)", precision: 5, scale: 2, nullable: false),
                    StartDate = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    EndDate = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    Notes = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Status = table.Column<int>(type: "int", nullable: false),
                    Created = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    CreatedBy = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Modified = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    ModifiedBy = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sponsorships", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Sponsorships_AcademicYears_AcademicYearId",
                        column: x => x.AcademicYearId,
                        principalTable: "AcademicYears",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Sponsorships_Person_StudentId",
                        column: x => x.StudentId,
                        principalTable: "Person",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Sponsorships_SchoolClasses_SchoolClassId",
                        column: x => x.SchoolClassId,
                        principalTable: "SchoolClasses",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Sponsorships_Sessions_SessionId",
                        column: x => x.SessionId,
                        principalTable: "Sessions",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Sponsorships_Sponsors_SponsorId",
                        column: x => x.SponsorId,
                        principalTable: "Sponsors",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "SponsorshipFeeCategories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    SponsorshipId = table.Column<int>(type: "int", nullable: false),
                    FeeCategoryId = table.Column<int>(type: "int", nullable: false),
                    Created = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    CreatedBy = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Modified = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    ModifiedBy = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SponsorshipFeeCategories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SponsorshipFeeCategories_FeeCategories_FeeCategoryId",
                        column: x => x.FeeCategoryId,
                        principalTable: "FeeCategories",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_SponsorshipFeeCategories_Sponsorships_SponsorshipId",
                        column: x => x.SponsorshipId,
                        principalTable: "Sponsorships",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

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

            // SuperAdministrator role may already exist (seeded at runtime by DatabaseBootstrapper on earlier boots).
            migrationBuilder.Sql(@"
                INSERT INTO AspNetRoles (Id, ConcurrencyStamp, Created, CreatedBy, Modified, ModifiedBy, Name, NormalizedName)
                SELECT 8, NULL, NOW(), 'admin', NOW(), 'admin', 'SuperAdministrator', 'SUPERADMINISTRATOR'
                WHERE NOT EXISTS (SELECT 1 FROM AspNetRoles WHERE Id = 8);
            ");

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

            migrationBuilder.Sql(@"
                INSERT INTO AspNetUserRoles (RoleId, UserId)
                SELECT 8, 1
                WHERE NOT EXISTS (SELECT 1 FROM AspNetUserRoles WHERE RoleId = 8 AND UserId = 1)
                  AND EXISTS (SELECT 1 FROM AspNetUsers WHERE Id = 1);
            ");

            migrationBuilder.CreateIndex(
                name: "IX_StudentInvoiceItems_SponsorshipId",
                table: "StudentInvoiceItems",
                column: "SponsorshipId");

            migrationBuilder.CreateIndex(
                name: "IX_SponsorPayments_BankAccountId",
                table: "SponsorPayments",
                column: "BankAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_SponsorPayments_SponsorId",
                table: "SponsorPayments",
                column: "SponsorId");

            migrationBuilder.CreateIndex(
                name: "IX_Sponsors_ReceivableAccountId",
                table: "Sponsors",
                column: "ReceivableAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_SponsorshipFeeCategories_FeeCategoryId",
                table: "SponsorshipFeeCategories",
                column: "FeeCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_SponsorshipFeeCategories_SponsorshipId",
                table: "SponsorshipFeeCategories",
                column: "SponsorshipId");

            migrationBuilder.CreateIndex(
                name: "IX_Sponsorships_AcademicYearId",
                table: "Sponsorships",
                column: "AcademicYearId");

            migrationBuilder.CreateIndex(
                name: "IX_Sponsorships_SchoolClassId",
                table: "Sponsorships",
                column: "SchoolClassId");

            migrationBuilder.CreateIndex(
                name: "IX_Sponsorships_SessionId",
                table: "Sponsorships",
                column: "SessionId");

            migrationBuilder.CreateIndex(
                name: "IX_Sponsorships_SponsorId",
                table: "Sponsorships",
                column: "SponsorId");

            migrationBuilder.CreateIndex(
                name: "IX_Sponsorships_StudentId",
                table: "Sponsorships",
                column: "StudentId");

            // Add the FK back only if it doesn't already exist (may have been added by DatabaseBootstrapper).
            migrationBuilder.Sql(@"
                SET @sql = (SELECT IF(COUNT(*) = 0,
                    'ALTER TABLE ApprovalStepActions ADD CONSTRAINT FK_ApprovalStepActions_ApprovalRequests_ApprovalRequestId FOREIGN KEY (ApprovalRequestId) REFERENCES ApprovalRequests(Id) ON DELETE RESTRICT',
                    'SELECT 1')
                    FROM information_schema.TABLE_CONSTRAINTS
                    WHERE TABLE_SCHEMA = DATABASE() AND TABLE_NAME = 'ApprovalStepActions'
                      AND CONSTRAINT_NAME = 'FK_ApprovalStepActions_ApprovalRequests_ApprovalRequestId');
                PREPARE stmt FROM @sql; EXECUTE stmt; DEALLOCATE PREPARE stmt;

                SET @sql = (SELECT IF(COUNT(*) = 0,
                    'ALTER TABLE ApprovalWorkflowSteps ADD CONSTRAINT FK_ApprovalWorkflowSteps_ApprovalWorkflows_ApprovalWorkflowId FOREIGN KEY (ApprovalWorkflowId) REFERENCES ApprovalWorkflows(Id) ON DELETE RESTRICT',
                    'SELECT 1')
                    FROM information_schema.TABLE_CONSTRAINTS
                    WHERE TABLE_SCHEMA = DATABASE() AND TABLE_NAME = 'ApprovalWorkflowSteps'
                      AND CONSTRAINT_NAME = 'FK_ApprovalWorkflowSteps_ApprovalWorkflows_ApprovalWorkflowId');
                PREPARE stmt FROM @sql; EXECUTE stmt; DEALLOCATE PREPARE stmt;
            ");

            migrationBuilder.AddForeignKey(
                name: "FK_StudentInvoiceItems_Sponsorships_SponsorshipId",
                table: "StudentInvoiceItems",
                column: "SponsorshipId",
                principalTable: "Sponsorships",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ApprovalStepActions_ApprovalRequests_ApprovalRequestId",
                table: "ApprovalStepActions");

            migrationBuilder.DropForeignKey(
                name: "FK_ApprovalWorkflowSteps_ApprovalWorkflows_ApprovalWorkflowId",
                table: "ApprovalWorkflowSteps");

            migrationBuilder.DropForeignKey(
                name: "FK_StudentInvoiceItems_Sponsorships_SponsorshipId",
                table: "StudentInvoiceItems");

            migrationBuilder.DropTable(
                name: "SponsorPayments");

            migrationBuilder.DropTable(
                name: "SponsorshipFeeCategories");

            migrationBuilder.DropTable(
                name: "Sponsorships");

            migrationBuilder.DropTable(
                name: "Sponsors");

            migrationBuilder.DropIndex(
                name: "IX_StudentInvoiceItems_SponsorshipId",
                table: "StudentInvoiceItems");

            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { 8, 1 });

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 8);

            migrationBuilder.DropColumn(
                name: "SponsorshipId",
                table: "StudentInvoiceItems");

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
                table: "ApprovalWorkflowSteps",
                keyColumn: "Id",
                keyValue: 9,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 18, 8, 31, 54, 369, DateTimeKind.Local).AddTicks(327), new DateTime(2026, 4, 18, 8, 31, 54, 369, DateTimeKind.Local).AddTicks(327) });

            migrationBuilder.UpdateData(
                table: "ApprovalWorkflowSteps",
                keyColumn: "Id",
                keyValue: 10,
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

            migrationBuilder.UpdateData(
                table: "ApprovalWorkflows",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 18, 8, 31, 54, 369, DateTimeKind.Local).AddTicks(327), new DateTime(2026, 4, 18, 8, 31, 54, 369, DateTimeKind.Local).AddTicks(327) });

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

            migrationBuilder.AddForeignKey(
                name: "FK_ApprovalStepActions_ApprovalRequests_ApprovalRequestId",
                table: "ApprovalStepActions",
                column: "ApprovalRequestId",
                principalTable: "ApprovalRequests",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ApprovalWorkflowSteps_ApprovalWorkflows_ApprovalWorkflowId",
                table: "ApprovalWorkflowSteps",
                column: "ApprovalWorkflowId",
                principalTable: "ApprovalWorkflows",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
