using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SchoolWebApp.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddBudgetLineToExpense : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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
                values: new object[] { new DateTime(2026, 4, 15, 16, 16, 26, 566, DateTimeKind.Local).AddTicks(6694), new DateTime(2026, 4, 15, 16, 16, 26, 566, DateTimeKind.Local).AddTicks(6715) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 15, 16, 16, 26, 566, DateTimeKind.Local).AddTicks(6834), new DateTime(2026, 4, 15, 16, 16, 26, 566, DateTimeKind.Local).AddTicks(6836) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 15, 16, 16, 26, 566, DateTimeKind.Local).AddTicks(6885), new DateTime(2026, 4, 15, 16, 16, 26, 566, DateTimeKind.Local).AddTicks(6887) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 15, 16, 16, 26, 566, DateTimeKind.Local).AddTicks(6945), new DateTime(2026, 4, 15, 16, 16, 26, 566, DateTimeKind.Local).AddTicks(6948) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 15, 16, 16, 26, 566, DateTimeKind.Local).AddTicks(6991), new DateTime(2026, 4, 15, 16, 16, 26, 566, DateTimeKind.Local).AddTicks(6994) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 6,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 15, 16, 16, 26, 566, DateTimeKind.Local).AddTicks(7045), new DateTime(2026, 4, 15, 16, 16, 26, 566, DateTimeKind.Local).AddTicks(7047) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 7,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 15, 16, 16, 26, 566, DateTimeKind.Local).AddTicks(7098), new DateTime(2026, 4, 15, 16, 16, 26, 566, DateTimeKind.Local).AddTicks(7100) });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "ConcurrencyStamp", "Created", "Modified", "PasswordHash" },
                values: new object[] { "638285de-01a5-4f00-a616-373b8b7989b3", new DateTime(2026, 4, 15, 16, 16, 26, 566, DateTimeKind.Local).AddTicks(7674), new DateTime(2026, 4, 15, 16, 16, 26, 566, DateTimeKind.Local).AddTicks(7676), "AQAAAAIAAYagAAAAELgCiYCJkr3K740oxFRyI4IK14YURMfh2xKPXr5dcF1+JhDg6XnEk4pWzp2uly68dA==" });

            migrationBuilder.UpdateData(
                table: "Designations",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 15, 16, 16, 26, 566, DateTimeKind.Local).AddTicks(7235), new DateTime(2026, 4, 15, 16, 16, 26, 566, DateTimeKind.Local).AddTicks(7237) });

            migrationBuilder.UpdateData(
                table: "EmploymentTypes",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 15, 16, 16, 26, 566, DateTimeKind.Local).AddTicks(7284), new DateTime(2026, 4, 15, 16, 16, 26, 566, DateTimeKind.Local).AddTicks(7286) });

            migrationBuilder.UpdateData(
                table: "Genders",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 15, 16, 16, 26, 566, DateTimeKind.Local).AddTicks(7420), new DateTime(2026, 4, 15, 16, 16, 26, 566, DateTimeKind.Local).AddTicks(7422) });

            migrationBuilder.UpdateData(
                table: "Nationalities",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 15, 16, 16, 26, 566, DateTimeKind.Local).AddTicks(7329), new DateTime(2026, 4, 15, 16, 16, 26, 566, DateTimeKind.Local).AddTicks(7331) });

            migrationBuilder.UpdateData(
                table: "Person",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "DateOfBirth", "EmploymentDate", "EndofEmploymentDate", "Modified" },
                values: new object[] { new DateTime(2026, 4, 15, 16, 16, 26, 566, DateTimeKind.Local).AddTicks(7496), new DateTime(2026, 4, 15, 16, 16, 26, 566, DateTimeKind.Local).AddTicks(7491), new DateTime(2026, 4, 15, 16, 16, 26, 566, DateTimeKind.Local).AddTicks(7486), new DateTime(2026, 4, 15, 16, 16, 26, 566, DateTimeKind.Local).AddTicks(7488), new DateTime(2026, 4, 15, 16, 16, 26, 566, DateTimeKind.Local).AddTicks(7506) });

            migrationBuilder.UpdateData(
                table: "Religions",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 15, 16, 16, 26, 566, DateTimeKind.Local).AddTicks(7381), new DateTime(2026, 4, 15, 16, 16, 26, 566, DateTimeKind.Local).AddTicks(7383) });

            migrationBuilder.UpdateData(
                table: "StaffCategories",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 15, 16, 16, 26, 566, DateTimeKind.Local).AddTicks(7161), new DateTime(2026, 4, 15, 16, 16, 26, 566, DateTimeKind.Local).AddTicks(7163) });

            migrationBuilder.CreateIndex(
                name: "IX_Expenses_BudgetLineId",
                table: "Expenses",
                column: "BudgetLineId");

            migrationBuilder.AddForeignKey(
                name: "FK_Expenses_BudgetLines_BudgetLineId",
                table: "Expenses",
                column: "BudgetLineId",
                principalTable: "BudgetLines",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Expenses_BudgetLines_BudgetLineId",
                table: "Expenses");

            migrationBuilder.DropIndex(
                name: "IX_Expenses_BudgetLineId",
                table: "Expenses");

            migrationBuilder.DropColumn(
                name: "BudgetLineId",
                table: "Expenses");

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
        }
    }
}
