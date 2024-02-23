using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SchoolWebApp.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdatedAcademicYearStatustoBoolfromEnum : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<bool>(
                name: "Status",
                table: "AcademicYears",
                type: "tinyint(1)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "269f0cf3-405e-4163-83f3-1b63ebebd62e",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2024, 2, 23, 10, 53, 23, 325, DateTimeKind.Local).AddTicks(9636), new DateTime(2024, 2, 23, 10, 53, 23, 325, DateTimeKind.Local).AddTicks(9637) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "448df289-142c-4959-a912-60733515e1b4",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2024, 2, 23, 10, 53, 23, 325, DateTimeKind.Local).AddTicks(9571), new DateTime(2024, 2, 23, 10, 53, 23, 325, DateTimeKind.Local).AddTicks(9574) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "48c50c3a-9958-453b-b649-4e21af131322",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2024, 2, 23, 10, 53, 23, 325, DateTimeKind.Local).AddTicks(9472), new DateTime(2024, 2, 23, 10, 53, 23, 325, DateTimeKind.Local).AddTicks(9499) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "717d9b15-a428-440c-b26b-08d3bbb68b02",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2024, 2, 23, 10, 53, 23, 325, DateTimeKind.Local).AddTicks(9248), new DateTime(2024, 2, 23, 10, 53, 23, 325, DateTimeKind.Local).AddTicks(9289) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "95ed2407-3e58-4af2-88a4-1c4e96473f68",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2024, 2, 23, 10, 53, 23, 325, DateTimeKind.Local).AddTicks(9408), new DateTime(2024, 2, 23, 10, 53, 23, 325, DateTimeKind.Local).AddTicks(9411) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "97942bee-ef12-4425-8225-4f293d0f36dd",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2024, 2, 23, 10, 53, 23, 325, DateTimeKind.Local).AddTicks(9845), new DateTime(2024, 2, 23, 10, 53, 23, 325, DateTimeKind.Local).AddTicks(9848) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "cd12b44b-103b-48df-8887-a2bf42e0651e",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2024, 2, 23, 10, 53, 23, 325, DateTimeKind.Local).AddTicks(9703), new DateTime(2024, 2, 23, 10, 53, 23, 325, DateTimeKind.Local).AddTicks(9705) });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "7e67d486-af3e-49f1-a109-a2b864b8e0ec",
                columns: new[] { "ConcurrencyStamp", "Created", "Modified", "PasswordHash", "SecurityStamp" },
                values: new object[] { "bb69ae8e-94a4-4948-b954-f8b2d014ea44", new DateTime(2024, 2, 23, 10, 53, 23, 326, DateTimeKind.Local).AddTicks(9), new DateTime(2024, 2, 23, 10, 53, 23, 326, DateTimeKind.Local).AddTicks(11), "AQAAAAIAAYagAAAAEC14/7AZut7J7L+DQWRhCJklmcePD2fm6liciMu+LUT+3NEh+JABXPdplfsDar/Q5g==", "e88c4a0b-473c-488b-9b81-f4b868edb730" });

            migrationBuilder.UpdateData(
                table: "SchoolLevels",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2024, 2, 23, 10, 53, 23, 463, DateTimeKind.Local).AddTicks(9227), new DateTime(2024, 2, 23, 10, 53, 23, 463, DateTimeKind.Local).AddTicks(9251) });

            migrationBuilder.UpdateData(
                table: "SchoolLevels",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2024, 2, 23, 10, 53, 23, 463, DateTimeKind.Local).AddTicks(9254), new DateTime(2024, 2, 23, 10, 53, 23, 463, DateTimeKind.Local).AddTicks(9255) });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "Status",
                table: "AcademicYears",
                type: "int",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "tinyint(1)");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "269f0cf3-405e-4163-83f3-1b63ebebd62e",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2024, 2, 20, 16, 51, 23, 29, DateTimeKind.Local).AddTicks(1765), new DateTime(2024, 2, 20, 16, 51, 23, 29, DateTimeKind.Local).AddTicks(1766) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "448df289-142c-4959-a912-60733515e1b4",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2024, 2, 20, 16, 51, 23, 29, DateTimeKind.Local).AddTicks(1739), new DateTime(2024, 2, 20, 16, 51, 23, 29, DateTimeKind.Local).AddTicks(1741) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "48c50c3a-9958-453b-b649-4e21af131322",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2024, 2, 20, 16, 51, 23, 29, DateTimeKind.Local).AddTicks(1695), new DateTime(2024, 2, 20, 16, 51, 23, 29, DateTimeKind.Local).AddTicks(1706) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "717d9b15-a428-440c-b26b-08d3bbb68b02",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2024, 2, 20, 16, 51, 23, 29, DateTimeKind.Local).AddTicks(1571), new DateTime(2024, 2, 20, 16, 51, 23, 29, DateTimeKind.Local).AddTicks(1594) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "95ed2407-3e58-4af2-88a4-1c4e96473f68",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2024, 2, 20, 16, 51, 23, 29, DateTimeKind.Local).AddTicks(1656), new DateTime(2024, 2, 20, 16, 51, 23, 29, DateTimeKind.Local).AddTicks(1658) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "97942bee-ef12-4425-8225-4f293d0f36dd",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2024, 2, 20, 16, 51, 23, 29, DateTimeKind.Local).AddTicks(1818), new DateTime(2024, 2, 20, 16, 51, 23, 29, DateTimeKind.Local).AddTicks(1819) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "cd12b44b-103b-48df-8887-a2bf42e0651e",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2024, 2, 20, 16, 51, 23, 29, DateTimeKind.Local).AddTicks(1793), new DateTime(2024, 2, 20, 16, 51, 23, 29, DateTimeKind.Local).AddTicks(1794) });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "7e67d486-af3e-49f1-a109-a2b864b8e0ec",
                columns: new[] { "ConcurrencyStamp", "Created", "Modified", "PasswordHash", "SecurityStamp" },
                values: new object[] { "dcc3c192-d4a4-44a9-9d05-21df04a61bee", new DateTime(2024, 2, 20, 16, 51, 23, 29, DateTimeKind.Local).AddTicks(1907), new DateTime(2024, 2, 20, 16, 51, 23, 29, DateTimeKind.Local).AddTicks(1909), "AQAAAAIAAYagAAAAEALrsRoH2PqDXpNdus1s1Qgn6DimRUhLfvN/SrRszv+Q+AXytMeovTUrjgpsbKgNKQ==", "9dacce61-a580-4d8b-acd3-c3edda0d4783" });

            migrationBuilder.UpdateData(
                table: "SchoolLevels",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2024, 2, 20, 16, 51, 23, 131, DateTimeKind.Local).AddTicks(5939), new DateTime(2024, 2, 20, 16, 51, 23, 131, DateTimeKind.Local).AddTicks(5968) });

            migrationBuilder.UpdateData(
                table: "SchoolLevels",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2024, 2, 20, 16, 51, 23, 131, DateTimeKind.Local).AddTicks(5972), new DateTime(2024, 2, 20, 16, 51, 23, 131, DateTimeKind.Local).AddTicks(5973) });
        }
    }
}
