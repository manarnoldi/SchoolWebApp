using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SchoolWebApp.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddInvoiceItemPaidAmount : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "PaidAmount",
                table: "StudentInvoiceItems",
                type: "decimal(65,30)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 16, 14, 56, 36, 180, DateTimeKind.Local).AddTicks(633), new DateTime(2026, 4, 16, 14, 56, 36, 180, DateTimeKind.Local).AddTicks(652) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 16, 14, 56, 36, 180, DateTimeKind.Local).AddTicks(692), new DateTime(2026, 4, 16, 14, 56, 36, 180, DateTimeKind.Local).AddTicks(693) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 16, 14, 56, 36, 180, DateTimeKind.Local).AddTicks(709), new DateTime(2026, 4, 16, 14, 56, 36, 180, DateTimeKind.Local).AddTicks(709) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 16, 14, 56, 36, 180, DateTimeKind.Local).AddTicks(725), new DateTime(2026, 4, 16, 14, 56, 36, 180, DateTimeKind.Local).AddTicks(726) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 16, 14, 56, 36, 180, DateTimeKind.Local).AddTicks(741), new DateTime(2026, 4, 16, 14, 56, 36, 180, DateTimeKind.Local).AddTicks(742) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 6,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 16, 14, 56, 36, 180, DateTimeKind.Local).AddTicks(841), new DateTime(2026, 4, 16, 14, 56, 36, 180, DateTimeKind.Local).AddTicks(841) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 7,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 16, 14, 56, 36, 180, DateTimeKind.Local).AddTicks(869), new DateTime(2026, 4, 16, 14, 56, 36, 180, DateTimeKind.Local).AddTicks(870) });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "ConcurrencyStamp", "Created", "Modified", "PasswordHash" },
                values: new object[] { "524b50a9-1557-4f38-83ca-b994471dff03", new DateTime(2026, 4, 16, 14, 56, 36, 180, DateTimeKind.Local).AddTicks(1192), new DateTime(2026, 4, 16, 14, 56, 36, 180, DateTimeKind.Local).AddTicks(1194), "AQAAAAIAAYagAAAAEGWfuAjnxucFZXqmvku3bdfZgNctUQWPVNzV3PCi6LTADp4ftO1gaXmEglJHZgMhrA==" });

            migrationBuilder.UpdateData(
                table: "Designations",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 16, 14, 56, 36, 180, DateTimeKind.Local).AddTicks(946), new DateTime(2026, 4, 16, 14, 56, 36, 180, DateTimeKind.Local).AddTicks(947) });

            migrationBuilder.UpdateData(
                table: "EmploymentTypes",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 16, 14, 56, 36, 180, DateTimeKind.Local).AddTicks(972), new DateTime(2026, 4, 16, 14, 56, 36, 180, DateTimeKind.Local).AddTicks(973) });

            migrationBuilder.UpdateData(
                table: "Genders",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 16, 14, 56, 36, 180, DateTimeKind.Local).AddTicks(1029), new DateTime(2026, 4, 16, 14, 56, 36, 180, DateTimeKind.Local).AddTicks(1030) });

            migrationBuilder.UpdateData(
                table: "Nationalities",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 16, 14, 56, 36, 180, DateTimeKind.Local).AddTicks(992), new DateTime(2026, 4, 16, 14, 56, 36, 180, DateTimeKind.Local).AddTicks(993) });

            migrationBuilder.UpdateData(
                table: "Person",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "DateOfBirth", "EmploymentDate", "EndofEmploymentDate", "Modified" },
                values: new object[] { new DateTime(2026, 4, 16, 14, 56, 36, 180, DateTimeKind.Local).AddTicks(1068), new DateTime(2026, 4, 16, 14, 56, 36, 180, DateTimeKind.Local).AddTicks(1064), new DateTime(2026, 4, 16, 14, 56, 36, 180, DateTimeKind.Local).AddTicks(1061), new DateTime(2026, 4, 16, 14, 56, 36, 180, DateTimeKind.Local).AddTicks(1062), new DateTime(2026, 4, 16, 14, 56, 36, 180, DateTimeKind.Local).AddTicks(1076) });

            migrationBuilder.UpdateData(
                table: "Religions",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 16, 14, 56, 36, 180, DateTimeKind.Local).AddTicks(1012), new DateTime(2026, 4, 16, 14, 56, 36, 180, DateTimeKind.Local).AddTicks(1013) });

            migrationBuilder.UpdateData(
                table: "StaffCategories",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 16, 14, 56, 36, 180, DateTimeKind.Local).AddTicks(906), new DateTime(2026, 4, 16, 14, 56, 36, 180, DateTimeKind.Local).AddTicks(908) });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PaidAmount",
                table: "StudentInvoiceItems");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 16, 11, 20, 24, 936, DateTimeKind.Local).AddTicks(2145), new DateTime(2026, 4, 16, 11, 20, 24, 936, DateTimeKind.Local).AddTicks(2208) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 16, 11, 20, 24, 936, DateTimeKind.Local).AddTicks(2508), new DateTime(2026, 4, 16, 11, 20, 24, 936, DateTimeKind.Local).AddTicks(2517) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 16, 11, 20, 24, 936, DateTimeKind.Local).AddTicks(2715), new DateTime(2026, 4, 16, 11, 20, 24, 936, DateTimeKind.Local).AddTicks(2722) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 16, 11, 20, 24, 936, DateTimeKind.Local).AddTicks(2898), new DateTime(2026, 4, 16, 11, 20, 24, 936, DateTimeKind.Local).AddTicks(2905) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 16, 11, 20, 24, 936, DateTimeKind.Local).AddTicks(3072), new DateTime(2026, 4, 16, 11, 20, 24, 936, DateTimeKind.Local).AddTicks(3080) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 6,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 16, 11, 20, 24, 936, DateTimeKind.Local).AddTicks(4103), new DateTime(2026, 4, 16, 11, 20, 24, 936, DateTimeKind.Local).AddTicks(4113) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 7,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 16, 11, 20, 24, 936, DateTimeKind.Local).AddTicks(4336), new DateTime(2026, 4, 16, 11, 20, 24, 936, DateTimeKind.Local).AddTicks(4344) });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "ConcurrencyStamp", "Created", "Modified", "PasswordHash" },
                values: new object[] { "fffb4dff-c64a-4fba-81ea-56f26bd03897", new DateTime(2026, 4, 16, 11, 20, 24, 936, DateTimeKind.Local).AddTicks(6846), new DateTime(2026, 4, 16, 11, 20, 24, 936, DateTimeKind.Local).AddTicks(6856), "AQAAAAIAAYagAAAAELIy9jAM7kNHST9a+B1ZBWFzmq0JVWGDoLba8MAR/db85/pnFPb64KRpsULr937ukQ==" });

            migrationBuilder.UpdateData(
                table: "Designations",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 16, 11, 20, 24, 936, DateTimeKind.Local).AddTicks(4829), new DateTime(2026, 4, 16, 11, 20, 24, 936, DateTimeKind.Local).AddTicks(4837) });

            migrationBuilder.UpdateData(
                table: "EmploymentTypes",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 16, 11, 20, 24, 936, DateTimeKind.Local).AddTicks(5010), new DateTime(2026, 4, 16, 11, 20, 24, 936, DateTimeKind.Local).AddTicks(5018) });

            migrationBuilder.UpdateData(
                table: "Genders",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 16, 11, 20, 24, 936, DateTimeKind.Local).AddTicks(5486), new DateTime(2026, 4, 16, 11, 20, 24, 936, DateTimeKind.Local).AddTicks(5494) });

            migrationBuilder.UpdateData(
                table: "Nationalities",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 16, 11, 20, 24, 936, DateTimeKind.Local).AddTicks(5170), new DateTime(2026, 4, 16, 11, 20, 24, 936, DateTimeKind.Local).AddTicks(5176) });

            migrationBuilder.UpdateData(
                table: "Person",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "DateOfBirth", "EmploymentDate", "EndofEmploymentDate", "Modified" },
                values: new object[] { new DateTime(2026, 4, 16, 11, 20, 24, 936, DateTimeKind.Local).AddTicks(5739), new DateTime(2026, 4, 16, 11, 20, 24, 936, DateTimeKind.Local).AddTicks(5729), new DateTime(2026, 4, 16, 11, 20, 24, 936, DateTimeKind.Local).AddTicks(5713), new DateTime(2026, 4, 16, 11, 20, 24, 936, DateTimeKind.Local).AddTicks(5720), new DateTime(2026, 4, 16, 11, 20, 24, 936, DateTimeKind.Local).AddTicks(5772) });

            migrationBuilder.UpdateData(
                table: "Religions",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 16, 11, 20, 24, 936, DateTimeKind.Local).AddTicks(5333), new DateTime(2026, 4, 16, 11, 20, 24, 936, DateTimeKind.Local).AddTicks(5340) });

            migrationBuilder.UpdateData(
                table: "StaffCategories",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 16, 11, 20, 24, 936, DateTimeKind.Local).AddTicks(4577), new DateTime(2026, 4, 16, 11, 20, 24, 936, DateTimeKind.Local).AddTicks(4586) });
        }
    }
}
