using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SchoolWebApp.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddBudgetModule : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Budgets",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Description = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    StartDate = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    EndDate = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    AcademicYearId = table.Column<int>(type: "int", nullable: true),
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
                    table.PrimaryKey("PK_Budgets", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Budgets_AcademicYears_AcademicYearId",
                        column: x => x.AcademicYearId,
                        principalTable: "AcademicYears",
                        principalColumn: "Id");
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "BudgetLines",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    BudgetId = table.Column<int>(type: "int", nullable: false),
                    AccountId = table.Column<int>(type: "int", nullable: false),
                    BudgetedAmount = table.Column<decimal>(type: "decimal(65,30)", nullable: false),
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
                    table.PrimaryKey("PK_BudgetLines", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BudgetLines_Accounts_AccountId",
                        column: x => x.AccountId,
                        principalTable: "Accounts",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_BudgetLines_Budgets_BudgetId",
                        column: x => x.BudgetId,
                        principalTable: "Budgets",
                        principalColumn: "Id");
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 15, 15, 51, 46, 947, DateTimeKind.Local).AddTicks(9367), new DateTime(2026, 4, 15, 15, 51, 46, 947, DateTimeKind.Local).AddTicks(9442) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 15, 15, 51, 46, 947, DateTimeKind.Local).AddTicks(9832), new DateTime(2026, 4, 15, 15, 51, 46, 947, DateTimeKind.Local).AddTicks(9839) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 15, 15, 51, 46, 948, DateTimeKind.Local).AddTicks(1489), new DateTime(2026, 4, 15, 15, 51, 46, 948, DateTimeKind.Local).AddTicks(1535) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 15, 15, 51, 46, 948, DateTimeKind.Local).AddTicks(1677), new DateTime(2026, 4, 15, 15, 51, 46, 948, DateTimeKind.Local).AddTicks(1680) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 15, 15, 51, 46, 948, DateTimeKind.Local).AddTicks(1757), new DateTime(2026, 4, 15, 15, 51, 46, 948, DateTimeKind.Local).AddTicks(1760) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 6,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 15, 15, 51, 46, 948, DateTimeKind.Local).AddTicks(1923), new DateTime(2026, 4, 15, 15, 51, 46, 948, DateTimeKind.Local).AddTicks(1926) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 7,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 15, 15, 51, 46, 948, DateTimeKind.Local).AddTicks(2026), new DateTime(2026, 4, 15, 15, 51, 46, 948, DateTimeKind.Local).AddTicks(2029) });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "ConcurrencyStamp", "Created", "Modified", "PasswordHash" },
                values: new object[] { "b7aa04e5-30ed-481a-9c3b-b5f72d12aeb9", new DateTime(2026, 4, 15, 15, 51, 46, 948, DateTimeKind.Local).AddTicks(4816), new DateTime(2026, 4, 15, 15, 51, 46, 948, DateTimeKind.Local).AddTicks(4820), "AQAAAAIAAYagAAAAEAZLOubaBzLXhA8JFgndkmhS7vGRNVJaNT48zMy24pMfLZ9fMu0mOTbVcmMyN5384w==" });

            migrationBuilder.UpdateData(
                table: "Designations",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 15, 15, 51, 46, 948, DateTimeKind.Local).AddTicks(2484), new DateTime(2026, 4, 15, 15, 51, 46, 948, DateTimeKind.Local).AddTicks(2487) });

            migrationBuilder.UpdateData(
                table: "EmploymentTypes",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 15, 15, 51, 46, 948, DateTimeKind.Local).AddTicks(2605), new DateTime(2026, 4, 15, 15, 51, 46, 948, DateTimeKind.Local).AddTicks(2608) });

            migrationBuilder.UpdateData(
                table: "Genders",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 15, 15, 51, 46, 948, DateTimeKind.Local).AddTicks(4128), new DateTime(2026, 4, 15, 15, 51, 46, 948, DateTimeKind.Local).AddTicks(4134) });

            migrationBuilder.UpdateData(
                table: "Nationalities",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 15, 15, 51, 46, 948, DateTimeKind.Local).AddTicks(2762), new DateTime(2026, 4, 15, 15, 51, 46, 948, DateTimeKind.Local).AddTicks(2765) });

            migrationBuilder.UpdateData(
                table: "Person",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "DateOfBirth", "EmploymentDate", "EndofEmploymentDate", "Modified" },
                values: new object[] { new DateTime(2026, 4, 15, 15, 51, 46, 948, DateTimeKind.Local).AddTicks(4324), new DateTime(2026, 4, 15, 15, 51, 46, 948, DateTimeKind.Local).AddTicks(4310), new DateTime(2026, 4, 15, 15, 51, 46, 948, DateTimeKind.Local).AddTicks(4296), new DateTime(2026, 4, 15, 15, 51, 46, 948, DateTimeKind.Local).AddTicks(4300), new DateTime(2026, 4, 15, 15, 51, 46, 948, DateTimeKind.Local).AddTicks(4342) });

            migrationBuilder.UpdateData(
                table: "Religions",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 15, 15, 51, 46, 948, DateTimeKind.Local).AddTicks(3923), new DateTime(2026, 4, 15, 15, 51, 46, 948, DateTimeKind.Local).AddTicks(3938) });

            migrationBuilder.UpdateData(
                table: "StaffCategories",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 15, 15, 51, 46, 948, DateTimeKind.Local).AddTicks(2249), new DateTime(2026, 4, 15, 15, 51, 46, 948, DateTimeKind.Local).AddTicks(2261) });

            migrationBuilder.CreateIndex(
                name: "IX_BudgetLines_AccountId",
                table: "BudgetLines",
                column: "AccountId");

            migrationBuilder.CreateIndex(
                name: "IX_BudgetLines_BudgetId",
                table: "BudgetLines",
                column: "BudgetId");

            migrationBuilder.CreateIndex(
                name: "IX_Budgets_AcademicYearId",
                table: "Budgets",
                column: "AcademicYearId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BudgetLines");

            migrationBuilder.DropTable(
                name: "Budgets");

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
        }
    }
}
