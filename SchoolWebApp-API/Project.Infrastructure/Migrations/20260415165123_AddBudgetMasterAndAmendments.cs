using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SchoolWebApp.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddBudgetMasterAndAmendments : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "BudgetMasterId",
                table: "Budgets",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "BudgetAmendments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    BudgetId = table.Column<int>(type: "int", nullable: false),
                    ReferenceNumber = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    AmendmentDate = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    Reason = table.Column<string>(type: "varchar(1000)", maxLength: 1000, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Status = table.Column<int>(type: "int", nullable: false),
                    ApprovedById = table.Column<int>(type: "int", nullable: true),
                    ApprovedDate = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    Created = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    CreatedBy = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Modified = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    ModifiedBy = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BudgetAmendments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BudgetAmendments_AspNetUsers_ApprovedById",
                        column: x => x.ApprovedById,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_BudgetAmendments_Budgets_BudgetId",
                        column: x => x.BudgetId,
                        principalTable: "Budgets",
                        principalColumn: "Id");
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "BudgetMasters",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Code = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Description = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    AcademicYearId = table.Column<int>(type: "int", nullable: false),
                    StartDate = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    EndDate = table.Column<DateTime>(type: "datetime(6)", nullable: false),
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
                    table.PrimaryKey("PK_BudgetMasters", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BudgetMasters_AcademicYears_AcademicYearId",
                        column: x => x.AcademicYearId,
                        principalTable: "AcademicYears",
                        principalColumn: "Id");
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "BudgetAmendmentLines",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    BudgetAmendmentId = table.Column<int>(type: "int", nullable: false),
                    AccountId = table.Column<int>(type: "int", nullable: false),
                    PreviousAmount = table.Column<decimal>(type: "decimal(65,30)", nullable: false),
                    NewAmount = table.Column<decimal>(type: "decimal(65,30)", nullable: false),
                    Delta = table.Column<decimal>(type: "decimal(65,30)", nullable: false),
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
                    table.PrimaryKey("PK_BudgetAmendmentLines", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BudgetAmendmentLines_Accounts_AccountId",
                        column: x => x.AccountId,
                        principalTable: "Accounts",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_BudgetAmendmentLines_BudgetAmendments_BudgetAmendmentId",
                        column: x => x.BudgetAmendmentId,
                        principalTable: "BudgetAmendments",
                        principalColumn: "Id");
                })
                .Annotation("MySql:CharSet", "utf8mb4");

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

            migrationBuilder.CreateIndex(
                name: "IX_Budgets_BudgetMasterId",
                table: "Budgets",
                column: "BudgetMasterId");

            migrationBuilder.CreateIndex(
                name: "IX_BudgetAmendmentLines_AccountId",
                table: "BudgetAmendmentLines",
                column: "AccountId");

            migrationBuilder.CreateIndex(
                name: "IX_BudgetAmendmentLines_BudgetAmendmentId",
                table: "BudgetAmendmentLines",
                column: "BudgetAmendmentId");

            migrationBuilder.CreateIndex(
                name: "IX_BudgetAmendments_ApprovedById",
                table: "BudgetAmendments",
                column: "ApprovedById");

            migrationBuilder.CreateIndex(
                name: "IX_BudgetAmendments_BudgetId",
                table: "BudgetAmendments",
                column: "BudgetId");

            migrationBuilder.CreateIndex(
                name: "IX_BudgetMasters_AcademicYearId",
                table: "BudgetMasters",
                column: "AcademicYearId");

            migrationBuilder.AddForeignKey(
                name: "FK_Budgets_BudgetMasters_BudgetMasterId",
                table: "Budgets",
                column: "BudgetMasterId",
                principalTable: "BudgetMasters",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Budgets_BudgetMasters_BudgetMasterId",
                table: "Budgets");

            migrationBuilder.DropTable(
                name: "BudgetAmendmentLines");

            migrationBuilder.DropTable(
                name: "BudgetMasters");

            migrationBuilder.DropTable(
                name: "BudgetAmendments");

            migrationBuilder.DropIndex(
                name: "IX_Budgets_BudgetMasterId",
                table: "Budgets");

            migrationBuilder.DropColumn(
                name: "BudgetMasterId",
                table: "Budgets");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 15, 16, 29, 42, 792, DateTimeKind.Local).AddTicks(3573), new DateTime(2026, 4, 15, 16, 29, 42, 792, DateTimeKind.Local).AddTicks(3587) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 15, 16, 29, 42, 792, DateTimeKind.Local).AddTicks(3618), new DateTime(2026, 4, 15, 16, 29, 42, 792, DateTimeKind.Local).AddTicks(3619) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 15, 16, 29, 42, 792, DateTimeKind.Local).AddTicks(3634), new DateTime(2026, 4, 15, 16, 29, 42, 792, DateTimeKind.Local).AddTicks(3635) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 15, 16, 29, 42, 792, DateTimeKind.Local).AddTicks(3649), new DateTime(2026, 4, 15, 16, 29, 42, 792, DateTimeKind.Local).AddTicks(3650) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 15, 16, 29, 42, 792, DateTimeKind.Local).AddTicks(3664), new DateTime(2026, 4, 15, 16, 29, 42, 792, DateTimeKind.Local).AddTicks(3665) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 6,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 15, 16, 29, 42, 792, DateTimeKind.Local).AddTicks(3684), new DateTime(2026, 4, 15, 16, 29, 42, 792, DateTimeKind.Local).AddTicks(3685) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 7,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 15, 16, 29, 42, 792, DateTimeKind.Local).AddTicks(3702), new DateTime(2026, 4, 15, 16, 29, 42, 792, DateTimeKind.Local).AddTicks(3702) });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "ConcurrencyStamp", "Created", "Modified", "PasswordHash" },
                values: new object[] { "b335c255-f35e-40c3-bc7e-736f4671136b", new DateTime(2026, 4, 15, 16, 29, 42, 792, DateTimeKind.Local).AddTicks(4047), new DateTime(2026, 4, 15, 16, 29, 42, 792, DateTimeKind.Local).AddTicks(4048), "AQAAAAIAAYagAAAAEAG4Dwb5m8BUHFOOyNI8JFujJPQjkWzWh2xmM1hh/3DSAn8MwWfFw6+MmyAnC2kOHw==" });

            migrationBuilder.UpdateData(
                table: "Designations",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 15, 16, 29, 42, 792, DateTimeKind.Local).AddTicks(3787), new DateTime(2026, 4, 15, 16, 29, 42, 792, DateTimeKind.Local).AddTicks(3788) });

            migrationBuilder.UpdateData(
                table: "EmploymentTypes",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 15, 16, 29, 42, 792, DateTimeKind.Local).AddTicks(3808), new DateTime(2026, 4, 15, 16, 29, 42, 792, DateTimeKind.Local).AddTicks(3809) });

            migrationBuilder.UpdateData(
                table: "Genders",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 15, 16, 29, 42, 792, DateTimeKind.Local).AddTicks(3871), new DateTime(2026, 4, 15, 16, 29, 42, 792, DateTimeKind.Local).AddTicks(3871) });

            migrationBuilder.UpdateData(
                table: "Nationalities",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 15, 16, 29, 42, 792, DateTimeKind.Local).AddTicks(3828), new DateTime(2026, 4, 15, 16, 29, 42, 792, DateTimeKind.Local).AddTicks(3829) });

            migrationBuilder.UpdateData(
                table: "Person",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "DateOfBirth", "EmploymentDate", "EndofEmploymentDate", "Modified" },
                values: new object[] { new DateTime(2026, 4, 15, 16, 29, 42, 792, DateTimeKind.Local).AddTicks(3924), new DateTime(2026, 4, 15, 16, 29, 42, 792, DateTimeKind.Local).AddTicks(3921), new DateTime(2026, 4, 15, 16, 29, 42, 792, DateTimeKind.Local).AddTicks(3917), new DateTime(2026, 4, 15, 16, 29, 42, 792, DateTimeKind.Local).AddTicks(3918), new DateTime(2026, 4, 15, 16, 29, 42, 792, DateTimeKind.Local).AddTicks(3929) });

            migrationBuilder.UpdateData(
                table: "Religions",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 15, 16, 29, 42, 792, DateTimeKind.Local).AddTicks(3854), new DateTime(2026, 4, 15, 16, 29, 42, 792, DateTimeKind.Local).AddTicks(3855) });

            migrationBuilder.UpdateData(
                table: "StaffCategories",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 15, 16, 29, 42, 792, DateTimeKind.Local).AddTicks(3741), new DateTime(2026, 4, 15, 16, 29, 42, 792, DateTimeKind.Local).AddTicks(3743) });
        }
    }
}
