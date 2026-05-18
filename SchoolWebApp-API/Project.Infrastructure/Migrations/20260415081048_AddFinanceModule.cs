using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SchoolWebApp.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddFinanceModule : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Accounts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Code = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Name = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    AccountType = table.Column<int>(type: "int", nullable: false),
                    ParentAccountId = table.Column<int>(type: "int", nullable: true),
                    Description = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
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
                    table.PrimaryKey("PK_Accounts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Accounts_Accounts_ParentAccountId",
                        column: x => x.ParentAccountId,
                        principalTable: "Accounts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "FeeStructures",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    AcademicYearId = table.Column<int>(type: "int", nullable: false),
                    SessionId = table.Column<int>(type: "int", nullable: true),
                    LearningLevelId = table.Column<int>(type: "int", nullable: true),
                    Description = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
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
                    table.PrimaryKey("PK_FeeStructures", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FeeStructures_AcademicYears_AcademicYearId",
                        column: x => x.AcademicYearId,
                        principalTable: "AcademicYears",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_FeeStructures_LearningLevels_LearningLevelId",
                        column: x => x.LearningLevelId,
                        principalTable: "LearningLevels",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_FeeStructures_Sessions_SessionId",
                        column: x => x.SessionId,
                        principalTable: "Sessions",
                        principalColumn: "Id");
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "JournalEntries",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    ReferenceNumber = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    EntryDate = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    Description = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IsPosted = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    Created = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    CreatedBy = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Modified = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    ModifiedBy = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JournalEntries", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "StudentInvoices",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    InvoiceNumber = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    StudentId = table.Column<int>(type: "int", nullable: false),
                    AcademicYearId = table.Column<int>(type: "int", nullable: false),
                    SessionId = table.Column<int>(type: "int", nullable: true),
                    InvoiceDate = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    DueDate = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    TotalAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    PaidAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    DiscountAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
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
                    table.PrimaryKey("PK_StudentInvoices", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StudentInvoices_AcademicYears_AcademicYearId",
                        column: x => x.AcademicYearId,
                        principalTable: "AcademicYears",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_StudentInvoices_Person_StudentId",
                        column: x => x.StudentId,
                        principalTable: "Person",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_StudentInvoices_Sessions_SessionId",
                        column: x => x.SessionId,
                        principalTable: "Sessions",
                        principalColumn: "Id");
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "ExpenseCategories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Description = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Rank = table.Column<int>(type: "int", nullable: false),
                    ExpenseAccountId = table.Column<int>(type: "int", nullable: true),
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
                    table.PrimaryKey("PK_ExpenseCategories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ExpenseCategories_Accounts_ExpenseAccountId",
                        column: x => x.ExpenseAccountId,
                        principalTable: "Accounts",
                        principalColumn: "Id");
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "FeeCategories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Description = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Rank = table.Column<int>(type: "int", nullable: false),
                    IncomeAccountId = table.Column<int>(type: "int", nullable: true),
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
                    table.PrimaryKey("PK_FeeCategories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FeeCategories_Accounts_IncomeAccountId",
                        column: x => x.IncomeAccountId,
                        principalTable: "Accounts",
                        principalColumn: "Id");
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "JournalLines",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    JournalEntryId = table.Column<int>(type: "int", nullable: false),
                    AccountId = table.Column<int>(type: "int", nullable: false),
                    Debit = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Credit = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Description = table.Column<string>(type: "longtext", nullable: true)
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
                    table.PrimaryKey("PK_JournalLines", x => x.Id);
                    table.ForeignKey(
                        name: "FK_JournalLines_Accounts_AccountId",
                        column: x => x.AccountId,
                        principalTable: "Accounts",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_JournalLines_JournalEntries_JournalEntryId",
                        column: x => x.JournalEntryId,
                        principalTable: "JournalEntries",
                        principalColumn: "Id");
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Payments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    ReceiptNumber = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    StudentId = table.Column<int>(type: "int", nullable: false),
                    StudentInvoiceId = table.Column<int>(type: "int", nullable: true),
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
                    table.PrimaryKey("PK_Payments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Payments_Accounts_BankAccountId",
                        column: x => x.BankAccountId,
                        principalTable: "Accounts",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Payments_Person_StudentId",
                        column: x => x.StudentId,
                        principalTable: "Person",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Payments_StudentInvoices_StudentInvoiceId",
                        column: x => x.StudentInvoiceId,
                        principalTable: "StudentInvoices",
                        principalColumn: "Id");
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Expenses",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    ReferenceNumber = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ExpenseCategoryId = table.Column<int>(type: "int", nullable: false),
                    ExpenseDate = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Vendor = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    PaymentMethod = table.Column<int>(type: "int", nullable: false),
                    TransactionReference = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    PaidFromAccountId = table.Column<int>(type: "int", nullable: true),
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
                    table.PrimaryKey("PK_Expenses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Expenses_Accounts_PaidFromAccountId",
                        column: x => x.PaidFromAccountId,
                        principalTable: "Accounts",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Expenses_ExpenseCategories_ExpenseCategoryId",
                        column: x => x.ExpenseCategoryId,
                        principalTable: "ExpenseCategories",
                        principalColumn: "Id");
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "FeeStructureItems",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    FeeStructureId = table.Column<int>(type: "int", nullable: false),
                    FeeCategoryId = table.Column<int>(type: "int", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    IsMandatory = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    Created = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    CreatedBy = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Modified = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    ModifiedBy = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FeeStructureItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FeeStructureItems_FeeCategories_FeeCategoryId",
                        column: x => x.FeeCategoryId,
                        principalTable: "FeeCategories",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_FeeStructureItems_FeeStructures_FeeStructureId",
                        column: x => x.FeeStructureId,
                        principalTable: "FeeStructures",
                        principalColumn: "Id");
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "StudentInvoiceItems",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    StudentInvoiceId = table.Column<int>(type: "int", nullable: false),
                    FeeCategoryId = table.Column<int>(type: "int", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Discount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Description = table.Column<string>(type: "longtext", nullable: true)
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
                    table.PrimaryKey("PK_StudentInvoiceItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StudentInvoiceItems_FeeCategories_FeeCategoryId",
                        column: x => x.FeeCategoryId,
                        principalTable: "FeeCategories",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_StudentInvoiceItems_StudentInvoices_StudentInvoiceId",
                        column: x => x.StudentInvoiceId,
                        principalTable: "StudentInvoices",
                        principalColumn: "Id");
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 15, 11, 10, 46, 631, DateTimeKind.Local).AddTicks(7712), new DateTime(2026, 4, 15, 11, 10, 46, 631, DateTimeKind.Local).AddTicks(7728) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 15, 11, 10, 46, 631, DateTimeKind.Local).AddTicks(7761), new DateTime(2026, 4, 15, 11, 10, 46, 631, DateTimeKind.Local).AddTicks(7762) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 15, 11, 10, 46, 631, DateTimeKind.Local).AddTicks(7779), new DateTime(2026, 4, 15, 11, 10, 46, 631, DateTimeKind.Local).AddTicks(7780) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 15, 11, 10, 46, 631, DateTimeKind.Local).AddTicks(7793), new DateTime(2026, 4, 15, 11, 10, 46, 631, DateTimeKind.Local).AddTicks(7794) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 15, 11, 10, 46, 631, DateTimeKind.Local).AddTicks(7808), new DateTime(2026, 4, 15, 11, 10, 46, 631, DateTimeKind.Local).AddTicks(7808) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 6,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 15, 11, 10, 46, 631, DateTimeKind.Local).AddTicks(7828), new DateTime(2026, 4, 15, 11, 10, 46, 631, DateTimeKind.Local).AddTicks(7829) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 7,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 15, 11, 10, 46, 631, DateTimeKind.Local).AddTicks(7850), new DateTime(2026, 4, 15, 11, 10, 46, 631, DateTimeKind.Local).AddTicks(7850) });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "ConcurrencyStamp", "Created", "Modified", "PasswordHash" },
                values: new object[] { "b20ad452-8226-472d-9607-2b8a20bd785f", new DateTime(2026, 4, 15, 11, 10, 46, 631, DateTimeKind.Local).AddTicks(8204), new DateTime(2026, 4, 15, 11, 10, 46, 631, DateTimeKind.Local).AddTicks(8205), "AQAAAAIAAYagAAAAELjqj3+53A8QEPKhPC6qqJXYim2nDrLUX3C5Uv7rjdxgO1RoJScmMXUjbOXGTXPztA==" });

            migrationBuilder.UpdateData(
                table: "Designations",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 15, 11, 10, 46, 631, DateTimeKind.Local).AddTicks(7922), new DateTime(2026, 4, 15, 11, 10, 46, 631, DateTimeKind.Local).AddTicks(7922) });

            migrationBuilder.UpdateData(
                table: "EmploymentTypes",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 15, 11, 10, 46, 631, DateTimeKind.Local).AddTicks(7952), new DateTime(2026, 4, 15, 11, 10, 46, 631, DateTimeKind.Local).AddTicks(7953) });

            migrationBuilder.UpdateData(
                table: "Genders",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 15, 11, 10, 46, 631, DateTimeKind.Local).AddTicks(8014), new DateTime(2026, 4, 15, 11, 10, 46, 631, DateTimeKind.Local).AddTicks(8015) });

            migrationBuilder.UpdateData(
                table: "Nationalities",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 15, 11, 10, 46, 631, DateTimeKind.Local).AddTicks(7972), new DateTime(2026, 4, 15, 11, 10, 46, 631, DateTimeKind.Local).AddTicks(7973) });

            migrationBuilder.UpdateData(
                table: "Person",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "DateOfBirth", "EmploymentDate", "EndofEmploymentDate", "Modified" },
                values: new object[] { new DateTime(2026, 4, 15, 11, 10, 46, 631, DateTimeKind.Local).AddTicks(8047), new DateTime(2026, 4, 15, 11, 10, 46, 631, DateTimeKind.Local).AddTicks(8045), new DateTime(2026, 4, 15, 11, 10, 46, 631, DateTimeKind.Local).AddTicks(8041), new DateTime(2026, 4, 15, 11, 10, 46, 631, DateTimeKind.Local).AddTicks(8042), new DateTime(2026, 4, 15, 11, 10, 46, 631, DateTimeKind.Local).AddTicks(8051) });

            migrationBuilder.UpdateData(
                table: "Religions",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 15, 11, 10, 46, 631, DateTimeKind.Local).AddTicks(7996), new DateTime(2026, 4, 15, 11, 10, 46, 631, DateTimeKind.Local).AddTicks(7997) });

            migrationBuilder.UpdateData(
                table: "StaffCategories",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 15, 11, 10, 46, 631, DateTimeKind.Local).AddTicks(7883), new DateTime(2026, 4, 15, 11, 10, 46, 631, DateTimeKind.Local).AddTicks(7884) });

            migrationBuilder.CreateIndex(
                name: "IX_Accounts_ParentAccountId",
                table: "Accounts",
                column: "ParentAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_ExpenseCategories_ExpenseAccountId",
                table: "ExpenseCategories",
                column: "ExpenseAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_Expenses_ExpenseCategoryId",
                table: "Expenses",
                column: "ExpenseCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_Expenses_PaidFromAccountId",
                table: "Expenses",
                column: "PaidFromAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_FeeCategories_IncomeAccountId",
                table: "FeeCategories",
                column: "IncomeAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_FeeStructureItems_FeeCategoryId",
                table: "FeeStructureItems",
                column: "FeeCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_FeeStructureItems_FeeStructureId",
                table: "FeeStructureItems",
                column: "FeeStructureId");

            migrationBuilder.CreateIndex(
                name: "IX_FeeStructures_AcademicYearId",
                table: "FeeStructures",
                column: "AcademicYearId");

            migrationBuilder.CreateIndex(
                name: "IX_FeeStructures_LearningLevelId",
                table: "FeeStructures",
                column: "LearningLevelId");

            migrationBuilder.CreateIndex(
                name: "IX_FeeStructures_SessionId",
                table: "FeeStructures",
                column: "SessionId");

            migrationBuilder.CreateIndex(
                name: "IX_JournalLines_AccountId",
                table: "JournalLines",
                column: "AccountId");

            migrationBuilder.CreateIndex(
                name: "IX_JournalLines_JournalEntryId",
                table: "JournalLines",
                column: "JournalEntryId");

            migrationBuilder.CreateIndex(
                name: "IX_Payments_BankAccountId",
                table: "Payments",
                column: "BankAccountId");

            migrationBuilder.CreateIndex(
                name: "IX_Payments_StudentId",
                table: "Payments",
                column: "StudentId");

            migrationBuilder.CreateIndex(
                name: "IX_Payments_StudentInvoiceId",
                table: "Payments",
                column: "StudentInvoiceId");

            migrationBuilder.CreateIndex(
                name: "IX_StudentInvoiceItems_FeeCategoryId",
                table: "StudentInvoiceItems",
                column: "FeeCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_StudentInvoiceItems_StudentInvoiceId",
                table: "StudentInvoiceItems",
                column: "StudentInvoiceId");

            migrationBuilder.CreateIndex(
                name: "IX_StudentInvoices_AcademicYearId",
                table: "StudentInvoices",
                column: "AcademicYearId");

            migrationBuilder.CreateIndex(
                name: "IX_StudentInvoices_SessionId",
                table: "StudentInvoices",
                column: "SessionId");

            migrationBuilder.CreateIndex(
                name: "IX_StudentInvoices_StudentId",
                table: "StudentInvoices",
                column: "StudentId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Expenses");

            migrationBuilder.DropTable(
                name: "FeeStructureItems");

            migrationBuilder.DropTable(
                name: "JournalLines");

            migrationBuilder.DropTable(
                name: "Payments");

            migrationBuilder.DropTable(
                name: "StudentInvoiceItems");

            migrationBuilder.DropTable(
                name: "ExpenseCategories");

            migrationBuilder.DropTable(
                name: "FeeStructures");

            migrationBuilder.DropTable(
                name: "JournalEntries");

            migrationBuilder.DropTable(
                name: "FeeCategories");

            migrationBuilder.DropTable(
                name: "StudentInvoices");

            migrationBuilder.DropTable(
                name: "Accounts");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 13, 10, 58, 8, 312, DateTimeKind.Local).AddTicks(9077), new DateTime(2026, 4, 13, 10, 58, 8, 312, DateTimeKind.Local).AddTicks(9094) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 13, 10, 58, 8, 312, DateTimeKind.Local).AddTicks(9138), new DateTime(2026, 4, 13, 10, 58, 8, 312, DateTimeKind.Local).AddTicks(9139) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 13, 10, 58, 8, 312, DateTimeKind.Local).AddTicks(9160), new DateTime(2026, 4, 13, 10, 58, 8, 312, DateTimeKind.Local).AddTicks(9160) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 13, 10, 58, 8, 312, DateTimeKind.Local).AddTicks(9179), new DateTime(2026, 4, 13, 10, 58, 8, 312, DateTimeKind.Local).AddTicks(9180) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 13, 10, 58, 8, 312, DateTimeKind.Local).AddTicks(9198), new DateTime(2026, 4, 13, 10, 58, 8, 312, DateTimeKind.Local).AddTicks(9199) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 6,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 13, 10, 58, 8, 312, DateTimeKind.Local).AddTicks(9217), new DateTime(2026, 4, 13, 10, 58, 8, 312, DateTimeKind.Local).AddTicks(9218) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 7,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 13, 10, 58, 8, 312, DateTimeKind.Local).AddTicks(9237), new DateTime(2026, 4, 13, 10, 58, 8, 312, DateTimeKind.Local).AddTicks(9237) });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "ConcurrencyStamp", "Created", "Modified", "PasswordHash" },
                values: new object[] { "000e9ebb-cdf4-4a10-b2b3-ee4b660e0fe3", new DateTime(2026, 4, 13, 10, 58, 8, 312, DateTimeKind.Local).AddTicks(9575), new DateTime(2026, 4, 13, 10, 58, 8, 312, DateTimeKind.Local).AddTicks(9577), "AQAAAAIAAYagAAAAEN/xn+ca9bs2aJlGeoX8DhZZ7+6JwKY6YVpSOXRWvArG4kXtf80l704WF+YnqTKRwA==" });

            migrationBuilder.UpdateData(
                table: "Designations",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 13, 10, 58, 8, 312, DateTimeKind.Local).AddTicks(9295), new DateTime(2026, 4, 13, 10, 58, 8, 312, DateTimeKind.Local).AddTicks(9296) });

            migrationBuilder.UpdateData(
                table: "EmploymentTypes",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 13, 10, 58, 8, 312, DateTimeKind.Local).AddTicks(9325), new DateTime(2026, 4, 13, 10, 58, 8, 312, DateTimeKind.Local).AddTicks(9326) });

            migrationBuilder.UpdateData(
                table: "Genders",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 13, 10, 58, 8, 312, DateTimeKind.Local).AddTicks(9404), new DateTime(2026, 4, 13, 10, 58, 8, 312, DateTimeKind.Local).AddTicks(9405) });

            migrationBuilder.UpdateData(
                table: "Nationalities",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 13, 10, 58, 8, 312, DateTimeKind.Local).AddTicks(9346), new DateTime(2026, 4, 13, 10, 58, 8, 312, DateTimeKind.Local).AddTicks(9346) });

            migrationBuilder.UpdateData(
                table: "Person",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "DateOfBirth", "EmploymentDate", "EndofEmploymentDate", "Modified" },
                values: new object[] { new DateTime(2026, 4, 13, 10, 58, 8, 312, DateTimeKind.Local).AddTicks(9451), new DateTime(2026, 4, 13, 10, 58, 8, 312, DateTimeKind.Local).AddTicks(9448), new DateTime(2026, 4, 13, 10, 58, 8, 312, DateTimeKind.Local).AddTicks(9444), new DateTime(2026, 4, 13, 10, 58, 8, 312, DateTimeKind.Local).AddTicks(9446), new DateTime(2026, 4, 13, 10, 58, 8, 312, DateTimeKind.Local).AddTicks(9457) });

            migrationBuilder.UpdateData(
                table: "Religions",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 13, 10, 58, 8, 312, DateTimeKind.Local).AddTicks(9380), new DateTime(2026, 4, 13, 10, 58, 8, 312, DateTimeKind.Local).AddTicks(9380) });

            migrationBuilder.UpdateData(
                table: "StaffCategories",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 13, 10, 58, 8, 312, DateTimeKind.Local).AddTicks(9274), new DateTime(2026, 4, 13, 10, 58, 8, 312, DateTimeKind.Local).AddTicks(9276) });
        }
    }
}
