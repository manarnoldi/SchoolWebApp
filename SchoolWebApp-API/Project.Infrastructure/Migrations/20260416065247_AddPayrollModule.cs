using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SchoolWebApp.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddPayrollModule : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DeductionTypes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Code = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IsStatutory = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    IsActive = table.Column<bool>(type: "tinyint(1)", nullable: false),
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
                    table.PrimaryKey("PK_DeductionTypes", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "EarningTypes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Code = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IsTaxable = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    IsActive = table.Column<bool>(type: "tinyint(1)", nullable: false),
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
                    table.PrimaryKey("PK_EarningTypes", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "EmployeeSalaries",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    StaffDetailsId = table.Column<int>(type: "int", nullable: false),
                    BasicSalary = table.Column<decimal>(type: "decimal(65,30)", nullable: false),
                    HouseAllowance = table.Column<decimal>(type: "decimal(65,30)", nullable: false),
                    TransportAllowance = table.Column<decimal>(type: "decimal(65,30)", nullable: false),
                    OtherAllowances = table.Column<decimal>(type: "decimal(65,30)", nullable: false),
                    EffectiveDate = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    IsActive = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    Notes = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true)
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
                    table.PrimaryKey("PK_EmployeeSalaries", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EmployeeSalaries_Person_StaffDetailsId",
                        column: x => x.StaffDetailsId,
                        principalTable: "Person",
                        principalColumn: "Id");
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "LoanAdvances",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    StaffDetailsId = table.Column<int>(type: "int", nullable: false),
                    Description = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    PrincipalAmount = table.Column<decimal>(type: "decimal(65,30)", nullable: false),
                    MonthlyDeduction = table.Column<decimal>(type: "decimal(65,30)", nullable: false),
                    Balance = table.Column<decimal>(type: "decimal(65,30)", nullable: false),
                    IssueDate = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    Notes = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true)
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
                    table.PrimaryKey("PK_LoanAdvances", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LoanAdvances_Person_StaffDetailsId",
                        column: x => x.StaffDetailsId,
                        principalTable: "Person",
                        principalColumn: "Id");
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "PayrollPeriods",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Month = table.Column<int>(type: "int", nullable: false),
                    Year = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Status = table.Column<int>(type: "int", nullable: false),
                    ProcessedDate = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    ApprovedDate = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    PostedDate = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    ApprovedById = table.Column<int>(type: "int", nullable: true),
                    Created = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    CreatedBy = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Modified = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    ModifiedBy = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PayrollPeriods", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "PayrollSettings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Key = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Name = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Value = table.Column<decimal>(type: "decimal(65,30)", nullable: false),
                    Category = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Description = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    EffectiveDate = table.Column<DateTime>(type: "datetime(6)", nullable: false),
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
                    table.PrimaryKey("PK_PayrollSettings", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "TaxBands",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Description = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    LowerLimit = table.Column<decimal>(type: "decimal(65,30)", nullable: false),
                    UpperLimit = table.Column<decimal>(type: "decimal(65,30)", nullable: false),
                    Rate = table.Column<decimal>(type: "decimal(65,30)", nullable: false),
                    EffectiveDate = table.Column<DateTime>(type: "datetime(6)", nullable: false),
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
                    table.PrimaryKey("PK_TaxBands", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "EmployeeSalaryItems",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    EmployeeSalaryId = table.Column<int>(type: "int", nullable: false),
                    EarningTypeId = table.Column<int>(type: "int", nullable: true),
                    DeductionTypeId = table.Column<int>(type: "int", nullable: true),
                    Amount = table.Column<decimal>(type: "decimal(65,30)", nullable: false),
                    Created = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    CreatedBy = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Modified = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    ModifiedBy = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmployeeSalaryItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EmployeeSalaryItems_DeductionTypes_DeductionTypeId",
                        column: x => x.DeductionTypeId,
                        principalTable: "DeductionTypes",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_EmployeeSalaryItems_EarningTypes_EarningTypeId",
                        column: x => x.EarningTypeId,
                        principalTable: "EarningTypes",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_EmployeeSalaryItems_EmployeeSalaries_EmployeeSalaryId",
                        column: x => x.EmployeeSalaryId,
                        principalTable: "EmployeeSalaries",
                        principalColumn: "Id");
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Payslips",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    PayrollPeriodId = table.Column<int>(type: "int", nullable: false),
                    StaffDetailsId = table.Column<int>(type: "int", nullable: false),
                    BasicSalary = table.Column<decimal>(type: "decimal(65,30)", nullable: false),
                    HouseAllowance = table.Column<decimal>(type: "decimal(65,30)", nullable: false),
                    TransportAllowance = table.Column<decimal>(type: "decimal(65,30)", nullable: false),
                    OtherAllowances = table.Column<decimal>(type: "decimal(65,30)", nullable: false),
                    GrossPay = table.Column<decimal>(type: "decimal(65,30)", nullable: false),
                    NssfEmployee = table.Column<decimal>(type: "decimal(65,30)", nullable: false),
                    TaxableIncome = table.Column<decimal>(type: "decimal(65,30)", nullable: false),
                    GrossTax = table.Column<decimal>(type: "decimal(65,30)", nullable: false),
                    PersonalRelief = table.Column<decimal>(type: "decimal(65,30)", nullable: false),
                    InsuranceRelief = table.Column<decimal>(type: "decimal(65,30)", nullable: false),
                    Paye = table.Column<decimal>(type: "decimal(65,30)", nullable: false),
                    Shif = table.Column<decimal>(type: "decimal(65,30)", nullable: false),
                    Ahl = table.Column<decimal>(type: "decimal(65,30)", nullable: false),
                    NssfEmployer = table.Column<decimal>(type: "decimal(65,30)", nullable: false),
                    OtherDeductions = table.Column<decimal>(type: "decimal(65,30)", nullable: false),
                    LoanDeductions = table.Column<decimal>(type: "decimal(65,30)", nullable: false),
                    TotalDeductions = table.Column<decimal>(type: "decimal(65,30)", nullable: false),
                    NetPay = table.Column<decimal>(type: "decimal(65,30)", nullable: false),
                    Created = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    CreatedBy = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Modified = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    ModifiedBy = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Payslips", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Payslips_PayrollPeriods_PayrollPeriodId",
                        column: x => x.PayrollPeriodId,
                        principalTable: "PayrollPeriods",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Payslips_Person_StaffDetailsId",
                        column: x => x.StaffDetailsId,
                        principalTable: "Person",
                        principalColumn: "Id");
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "PayslipDeductions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    PayslipId = table.Column<int>(type: "int", nullable: false),
                    DeductionTypeId = table.Column<int>(type: "int", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(65,30)", nullable: false),
                    Created = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    CreatedBy = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Modified = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    ModifiedBy = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PayslipDeductions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PayslipDeductions_DeductionTypes_DeductionTypeId",
                        column: x => x.DeductionTypeId,
                        principalTable: "DeductionTypes",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_PayslipDeductions_Payslips_PayslipId",
                        column: x => x.PayslipId,
                        principalTable: "Payslips",
                        principalColumn: "Id");
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "PayslipEarnings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    PayslipId = table.Column<int>(type: "int", nullable: false),
                    EarningTypeId = table.Column<int>(type: "int", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(65,30)", nullable: false),
                    Created = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    CreatedBy = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Modified = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    ModifiedBy = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PayslipEarnings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PayslipEarnings_EarningTypes_EarningTypeId",
                        column: x => x.EarningTypeId,
                        principalTable: "EarningTypes",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_PayslipEarnings_Payslips_PayslipId",
                        column: x => x.PayslipId,
                        principalTable: "Payslips",
                        principalColumn: "Id");
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 16, 9, 52, 43, 804, DateTimeKind.Local).AddTicks(7652), new DateTime(2026, 4, 16, 9, 52, 43, 804, DateTimeKind.Local).AddTicks(7674) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 16, 9, 52, 43, 804, DateTimeKind.Local).AddTicks(7760), new DateTime(2026, 4, 16, 9, 52, 43, 804, DateTimeKind.Local).AddTicks(7763) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 16, 9, 52, 43, 804, DateTimeKind.Local).AddTicks(7806), new DateTime(2026, 4, 16, 9, 52, 43, 804, DateTimeKind.Local).AddTicks(7807) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 16, 9, 52, 43, 804, DateTimeKind.Local).AddTicks(7844), new DateTime(2026, 4, 16, 9, 52, 43, 804, DateTimeKind.Local).AddTicks(7846) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 16, 9, 52, 43, 804, DateTimeKind.Local).AddTicks(7893), new DateTime(2026, 4, 16, 9, 52, 43, 804, DateTimeKind.Local).AddTicks(7895) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 6,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 16, 9, 52, 43, 804, DateTimeKind.Local).AddTicks(7947), new DateTime(2026, 4, 16, 9, 52, 43, 804, DateTimeKind.Local).AddTicks(7949) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 7,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 16, 9, 52, 43, 804, DateTimeKind.Local).AddTicks(7993), new DateTime(2026, 4, 16, 9, 52, 43, 804, DateTimeKind.Local).AddTicks(7995) });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "ConcurrencyStamp", "Created", "Modified", "PasswordHash" },
                values: new object[] { "fd2109d2-d364-4730-8049-409f751b9920", new DateTime(2026, 4, 16, 9, 52, 43, 804, DateTimeKind.Local).AddTicks(8494), new DateTime(2026, 4, 16, 9, 52, 43, 804, DateTimeKind.Local).AddTicks(8497), "AQAAAAIAAYagAAAAEGxRsOZV1C9YJTv2K5vh2wQGB/t5qcnn4INGr6H5nWJ6rnsRg8QwXMbqZAUT4RaSuA==" });

            migrationBuilder.UpdateData(
                table: "Designations",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 16, 9, 52, 43, 804, DateTimeKind.Local).AddTicks(8110), new DateTime(2026, 4, 16, 9, 52, 43, 804, DateTimeKind.Local).AddTicks(8112) });

            migrationBuilder.UpdateData(
                table: "EmploymentTypes",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 16, 9, 52, 43, 804, DateTimeKind.Local).AddTicks(8153), new DateTime(2026, 4, 16, 9, 52, 43, 804, DateTimeKind.Local).AddTicks(8155) });

            migrationBuilder.UpdateData(
                table: "Genders",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 16, 9, 52, 43, 804, DateTimeKind.Local).AddTicks(8271), new DateTime(2026, 4, 16, 9, 52, 43, 804, DateTimeKind.Local).AddTicks(8273) });

            migrationBuilder.UpdateData(
                table: "Nationalities",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 16, 9, 52, 43, 804, DateTimeKind.Local).AddTicks(8194), new DateTime(2026, 4, 16, 9, 52, 43, 804, DateTimeKind.Local).AddTicks(8196) });

            migrationBuilder.UpdateData(
                table: "Person",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "DateOfBirth", "EmploymentDate", "EndofEmploymentDate", "Modified" },
                values: new object[] { new DateTime(2026, 4, 16, 9, 52, 43, 804, DateTimeKind.Local).AddTicks(8333), new DateTime(2026, 4, 16, 9, 52, 43, 804, DateTimeKind.Local).AddTicks(8330), new DateTime(2026, 4, 16, 9, 52, 43, 804, DateTimeKind.Local).AddTicks(8326), new DateTime(2026, 4, 16, 9, 52, 43, 804, DateTimeKind.Local).AddTicks(8327), new DateTime(2026, 4, 16, 9, 52, 43, 804, DateTimeKind.Local).AddTicks(8340) });

            migrationBuilder.UpdateData(
                table: "Religions",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 16, 9, 52, 43, 804, DateTimeKind.Local).AddTicks(8234), new DateTime(2026, 4, 16, 9, 52, 43, 804, DateTimeKind.Local).AddTicks(8236) });

            migrationBuilder.UpdateData(
                table: "StaffCategories",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 16, 9, 52, 43, 804, DateTimeKind.Local).AddTicks(8056), new DateTime(2026, 4, 16, 9, 52, 43, 804, DateTimeKind.Local).AddTicks(8059) });

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeSalaries_StaffDetailsId",
                table: "EmployeeSalaries",
                column: "StaffDetailsId");

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeSalaryItems_DeductionTypeId",
                table: "EmployeeSalaryItems",
                column: "DeductionTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeSalaryItems_EarningTypeId",
                table: "EmployeeSalaryItems",
                column: "EarningTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeSalaryItems_EmployeeSalaryId",
                table: "EmployeeSalaryItems",
                column: "EmployeeSalaryId");

            migrationBuilder.CreateIndex(
                name: "IX_LoanAdvances_StaffDetailsId",
                table: "LoanAdvances",
                column: "StaffDetailsId");

            migrationBuilder.CreateIndex(
                name: "IX_PayslipDeductions_DeductionTypeId",
                table: "PayslipDeductions",
                column: "DeductionTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_PayslipDeductions_PayslipId",
                table: "PayslipDeductions",
                column: "PayslipId");

            migrationBuilder.CreateIndex(
                name: "IX_PayslipEarnings_EarningTypeId",
                table: "PayslipEarnings",
                column: "EarningTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_PayslipEarnings_PayslipId",
                table: "PayslipEarnings",
                column: "PayslipId");

            migrationBuilder.CreateIndex(
                name: "IX_Payslips_PayrollPeriodId",
                table: "Payslips",
                column: "PayrollPeriodId");

            migrationBuilder.CreateIndex(
                name: "IX_Payslips_StaffDetailsId",
                table: "Payslips",
                column: "StaffDetailsId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EmployeeSalaryItems");

            migrationBuilder.DropTable(
                name: "LoanAdvances");

            migrationBuilder.DropTable(
                name: "PayrollSettings");

            migrationBuilder.DropTable(
                name: "PayslipDeductions");

            migrationBuilder.DropTable(
                name: "PayslipEarnings");

            migrationBuilder.DropTable(
                name: "TaxBands");

            migrationBuilder.DropTable(
                name: "EmployeeSalaries");

            migrationBuilder.DropTable(
                name: "DeductionTypes");

            migrationBuilder.DropTable(
                name: "EarningTypes");

            migrationBuilder.DropTable(
                name: "Payslips");

            migrationBuilder.DropTable(
                name: "PayrollPeriods");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 15, 19, 51, 19, 922, DateTimeKind.Local).AddTicks(7939), new DateTime(2026, 4, 15, 19, 51, 19, 922, DateTimeKind.Local).AddTicks(7956) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 15, 19, 51, 19, 922, DateTimeKind.Local).AddTicks(8058), new DateTime(2026, 4, 15, 19, 51, 19, 922, DateTimeKind.Local).AddTicks(8060) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 15, 19, 51, 19, 922, DateTimeKind.Local).AddTicks(8107), new DateTime(2026, 4, 15, 19, 51, 19, 922, DateTimeKind.Local).AddTicks(8109) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 15, 19, 51, 19, 922, DateTimeKind.Local).AddTicks(8149), new DateTime(2026, 4, 15, 19, 51, 19, 922, DateTimeKind.Local).AddTicks(8151) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 15, 19, 51, 19, 922, DateTimeKind.Local).AddTicks(8189), new DateTime(2026, 4, 15, 19, 51, 19, 922, DateTimeKind.Local).AddTicks(8191) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 6,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 15, 19, 51, 19, 922, DateTimeKind.Local).AddTicks(8240), new DateTime(2026, 4, 15, 19, 51, 19, 922, DateTimeKind.Local).AddTicks(8242) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 7,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 15, 19, 51, 19, 922, DateTimeKind.Local).AddTicks(8286), new DateTime(2026, 4, 15, 19, 51, 19, 922, DateTimeKind.Local).AddTicks(8288) });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "ConcurrencyStamp", "Created", "Modified", "PasswordHash" },
                values: new object[] { "cecaad27-569f-45fc-b184-2a5688af4104", new DateTime(2026, 4, 15, 19, 51, 19, 922, DateTimeKind.Local).AddTicks(8847), new DateTime(2026, 4, 15, 19, 51, 19, 922, DateTimeKind.Local).AddTicks(8849), "AQAAAAIAAYagAAAAEInmpAAPu9A4NRO9IlLWNFh22zeZcFWVVAG7h0kwroMJ3s4C9jxQKozsBJbGBfNGJA==" });

            migrationBuilder.UpdateData(
                table: "Designations",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 15, 19, 51, 19, 922, DateTimeKind.Local).AddTicks(8416), new DateTime(2026, 4, 15, 19, 51, 19, 922, DateTimeKind.Local).AddTicks(8418) });

            migrationBuilder.UpdateData(
                table: "EmploymentTypes",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 15, 19, 51, 19, 922, DateTimeKind.Local).AddTicks(8480), new DateTime(2026, 4, 15, 19, 51, 19, 922, DateTimeKind.Local).AddTicks(8483) });

            migrationBuilder.UpdateData(
                table: "Genders",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 15, 19, 51, 19, 922, DateTimeKind.Local).AddTicks(8612), new DateTime(2026, 4, 15, 19, 51, 19, 922, DateTimeKind.Local).AddTicks(8613) });

            migrationBuilder.UpdateData(
                table: "Nationalities",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 15, 19, 51, 19, 922, DateTimeKind.Local).AddTicks(8527), new DateTime(2026, 4, 15, 19, 51, 19, 922, DateTimeKind.Local).AddTicks(8529) });

            migrationBuilder.UpdateData(
                table: "Person",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "DateOfBirth", "EmploymentDate", "EndofEmploymentDate", "Modified" },
                values: new object[] { new DateTime(2026, 4, 15, 19, 51, 19, 922, DateTimeKind.Local).AddTicks(8676), new DateTime(2026, 4, 15, 19, 51, 19, 922, DateTimeKind.Local).AddTicks(8672), new DateTime(2026, 4, 15, 19, 51, 19, 922, DateTimeKind.Local).AddTicks(8667), new DateTime(2026, 4, 15, 19, 51, 19, 922, DateTimeKind.Local).AddTicks(8669), new DateTime(2026, 4, 15, 19, 51, 19, 922, DateTimeKind.Local).AddTicks(8688) });

            migrationBuilder.UpdateData(
                table: "Religions",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 15, 19, 51, 19, 922, DateTimeKind.Local).AddTicks(8571), new DateTime(2026, 4, 15, 19, 51, 19, 922, DateTimeKind.Local).AddTicks(8574) });

            migrationBuilder.UpdateData(
                table: "StaffCategories",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 15, 19, 51, 19, 922, DateTimeKind.Local).AddTicks(8346), new DateTime(2026, 4, 15, 19, 51, 19, 922, DateTimeKind.Local).AddTicks(8350) });
        }
    }
}
