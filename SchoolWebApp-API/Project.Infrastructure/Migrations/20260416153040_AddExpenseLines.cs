using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SchoolWebApp.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddExpenseLines : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Expenses_BudgetLines_BudgetLineId",
                table: "Expenses");

            migrationBuilder.DropForeignKey(
                name: "FK_Expenses_ExpenseCategories_ExpenseCategoryId",
                table: "Expenses");

            migrationBuilder.DropIndex(
                name: "IX_Expenses_BudgetLineId",
                table: "Expenses");

            migrationBuilder.DropIndex(
                name: "IX_Expenses_ExpenseCategoryId",
                table: "Expenses");

            migrationBuilder.DropColumn(
                name: "Amount",
                table: "Expenses");

            migrationBuilder.DropColumn(
                name: "BudgetLineId",
                table: "Expenses");

            migrationBuilder.RenameColumn(
                name: "ExpenseCategoryId",
                table: "Expenses",
                newName: "Status");

            migrationBuilder.CreateTable(
                name: "ExpenseLines",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    ExpenseId = table.Column<int>(type: "int", nullable: false),
                    ExpenseCategoryId = table.Column<int>(type: "int", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    BudgetLineId = table.Column<int>(type: "int", nullable: true),
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
                    table.PrimaryKey("PK_ExpenseLines", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ExpenseLines_BudgetLines_BudgetLineId",
                        column: x => x.BudgetLineId,
                        principalTable: "BudgetLines",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ExpenseLines_ExpenseCategories_ExpenseCategoryId",
                        column: x => x.ExpenseCategoryId,
                        principalTable: "ExpenseCategories",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ExpenseLines_Expenses_ExpenseId",
                        column: x => x.ExpenseId,
                        principalTable: "Expenses",
                        principalColumn: "Id");
                })
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

            migrationBuilder.CreateIndex(
                name: "IX_ExpenseLines_BudgetLineId",
                table: "ExpenseLines",
                column: "BudgetLineId");

            migrationBuilder.CreateIndex(
                name: "IX_ExpenseLines_ExpenseCategoryId",
                table: "ExpenseLines",
                column: "ExpenseCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_ExpenseLines_ExpenseId",
                table: "ExpenseLines",
                column: "ExpenseId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ExpenseLines");

            migrationBuilder.RenameColumn(
                name: "Status",
                table: "Expenses",
                newName: "ExpenseCategoryId");

            migrationBuilder.AddColumn<decimal>(
                name: "Amount",
                table: "Expenses",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<int>(
                name: "BudgetLineId",
                table: "Expenses",
                type: "int",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 16, 17, 57, 36, 583, DateTimeKind.Local).AddTicks(2085), new DateTime(2026, 4, 16, 17, 57, 36, 583, DateTimeKind.Local).AddTicks(2105) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 16, 17, 57, 36, 583, DateTimeKind.Local).AddTicks(2202), new DateTime(2026, 4, 16, 17, 57, 36, 583, DateTimeKind.Local).AddTicks(2204) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 16, 17, 57, 36, 583, DateTimeKind.Local).AddTicks(2245), new DateTime(2026, 4, 16, 17, 57, 36, 583, DateTimeKind.Local).AddTicks(2247) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 16, 17, 57, 36, 583, DateTimeKind.Local).AddTicks(2293), new DateTime(2026, 4, 16, 17, 57, 36, 583, DateTimeKind.Local).AddTicks(2295) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 16, 17, 57, 36, 583, DateTimeKind.Local).AddTicks(2404), new DateTime(2026, 4, 16, 17, 57, 36, 583, DateTimeKind.Local).AddTicks(2406) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 6,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 16, 17, 57, 36, 583, DateTimeKind.Local).AddTicks(2487), new DateTime(2026, 4, 16, 17, 57, 36, 583, DateTimeKind.Local).AddTicks(2489) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 7,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 16, 17, 57, 36, 583, DateTimeKind.Local).AddTicks(2533), new DateTime(2026, 4, 16, 17, 57, 36, 583, DateTimeKind.Local).AddTicks(2535) });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "ConcurrencyStamp", "Created", "Modified", "PasswordHash" },
                values: new object[] { "83e91782-6a88-4461-92fb-87792eb84b77", new DateTime(2026, 4, 16, 17, 57, 36, 583, DateTimeKind.Local).AddTicks(3301), new DateTime(2026, 4, 16, 17, 57, 36, 583, DateTimeKind.Local).AddTicks(3306), "AQAAAAIAAYagAAAAELwp6MncriVmdcQ+/k/cBzG8s8hm2owZr57CpM1pYm1ms3Msg1KYkvBW0J2bvIkWFw==" });

            migrationBuilder.UpdateData(
                table: "Designations",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 16, 17, 57, 36, 583, DateTimeKind.Local).AddTicks(2627), new DateTime(2026, 4, 16, 17, 57, 36, 583, DateTimeKind.Local).AddTicks(2629) });

            migrationBuilder.UpdateData(
                table: "EmploymentTypes",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 16, 17, 57, 36, 583, DateTimeKind.Local).AddTicks(2702), new DateTime(2026, 4, 16, 17, 57, 36, 583, DateTimeKind.Local).AddTicks(2706) });

            migrationBuilder.UpdateData(
                table: "Genders",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 16, 17, 57, 36, 583, DateTimeKind.Local).AddTicks(2913), new DateTime(2026, 4, 16, 17, 57, 36, 583, DateTimeKind.Local).AddTicks(2917) });

            migrationBuilder.UpdateData(
                table: "Nationalities",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 16, 17, 57, 36, 583, DateTimeKind.Local).AddTicks(2769), new DateTime(2026, 4, 16, 17, 57, 36, 583, DateTimeKind.Local).AddTicks(2774) });

            migrationBuilder.UpdateData(
                table: "Person",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "DateOfBirth", "EmploymentDate", "EndofEmploymentDate", "Modified" },
                values: new object[] { new DateTime(2026, 4, 16, 17, 57, 36, 583, DateTimeKind.Local).AddTicks(3040), new DateTime(2026, 4, 16, 17, 57, 36, 583, DateTimeKind.Local).AddTicks(3034), new DateTime(2026, 4, 16, 17, 57, 36, 583, DateTimeKind.Local).AddTicks(3025), new DateTime(2026, 4, 16, 17, 57, 36, 583, DateTimeKind.Local).AddTicks(3029), new DateTime(2026, 4, 16, 17, 57, 36, 583, DateTimeKind.Local).AddTicks(3051) });

            migrationBuilder.UpdateData(
                table: "Religions",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 16, 17, 57, 36, 583, DateTimeKind.Local).AddTicks(2844), new DateTime(2026, 4, 16, 17, 57, 36, 583, DateTimeKind.Local).AddTicks(2848) });

            migrationBuilder.UpdateData(
                table: "StaffCategories",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 16, 17, 57, 36, 583, DateTimeKind.Local).AddTicks(2587), new DateTime(2026, 4, 16, 17, 57, 36, 583, DateTimeKind.Local).AddTicks(2589) });

            migrationBuilder.CreateIndex(
                name: "IX_Expenses_BudgetLineId",
                table: "Expenses",
                column: "BudgetLineId");

            migrationBuilder.CreateIndex(
                name: "IX_Expenses_ExpenseCategoryId",
                table: "Expenses",
                column: "ExpenseCategoryId");

            migrationBuilder.AddForeignKey(
                name: "FK_Expenses_BudgetLines_BudgetLineId",
                table: "Expenses",
                column: "BudgetLineId",
                principalTable: "BudgetLines",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Expenses_ExpenseCategories_ExpenseCategoryId",
                table: "Expenses",
                column: "ExpenseCategoryId",
                principalTable: "ExpenseCategories",
                principalColumn: "Id");
        }
    }
}
