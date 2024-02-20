using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SchoolWebApp.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddedDescriptiononEntityDepartment : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Departments",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Description",
                table: "Departments");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "269f0cf3-405e-4163-83f3-1b63ebebd62e",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2024, 2, 16, 8, 47, 14, 235, DateTimeKind.Local).AddTicks(9324), new DateTime(2024, 2, 16, 8, 47, 14, 235, DateTimeKind.Local).AddTicks(9325) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "448df289-142c-4959-a912-60733515e1b4",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2024, 2, 16, 8, 47, 14, 235, DateTimeKind.Local).AddTicks(9266), new DateTime(2024, 2, 16, 8, 47, 14, 235, DateTimeKind.Local).AddTicks(9285) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "48c50c3a-9958-453b-b649-4e21af131322",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2024, 2, 16, 8, 47, 14, 235, DateTimeKind.Local).AddTicks(9237), new DateTime(2024, 2, 16, 8, 47, 14, 235, DateTimeKind.Local).AddTicks(9238) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "717d9b15-a428-440c-b26b-08d3bbb68b02",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2024, 2, 16, 8, 47, 14, 235, DateTimeKind.Local).AddTicks(9120), new DateTime(2024, 2, 16, 8, 47, 14, 235, DateTimeKind.Local).AddTicks(9142) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "95ed2407-3e58-4af2-88a4-1c4e96473f68",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2024, 2, 16, 8, 47, 14, 235, DateTimeKind.Local).AddTicks(9203), new DateTime(2024, 2, 16, 8, 47, 14, 235, DateTimeKind.Local).AddTicks(9205) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "97942bee-ef12-4425-8225-4f293d0f36dd",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2024, 2, 16, 8, 47, 14, 235, DateTimeKind.Local).AddTicks(9386), new DateTime(2024, 2, 16, 8, 47, 14, 235, DateTimeKind.Local).AddTicks(9387) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "cd12b44b-103b-48df-8887-a2bf42e0651e",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2024, 2, 16, 8, 47, 14, 235, DateTimeKind.Local).AddTicks(9356), new DateTime(2024, 2, 16, 8, 47, 14, 235, DateTimeKind.Local).AddTicks(9357) });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "7e67d486-af3e-49f1-a109-a2b864b8e0ec",
                columns: new[] { "ConcurrencyStamp", "Created", "Modified", "PasswordHash", "SecurityStamp" },
                values: new object[] { "10b72d51-fd81-4d21-98b8-e1ad28942ce2", new DateTime(2024, 2, 16, 8, 47, 14, 235, DateTimeKind.Local).AddTicks(9485), new DateTime(2024, 2, 16, 8, 47, 14, 235, DateTimeKind.Local).AddTicks(9487), "AQAAAAIAAYagAAAAEHco3a/cPZXNLang8dliIgLaojlqLoMtzAJZbkpaMYw9E8i/aQjF1VU2GCd8X0qYSg==", "676bd051-face-4ace-bf58-65ba588e92e9" });

            migrationBuilder.UpdateData(
                table: "SchoolLevels",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2024, 2, 16, 8, 47, 14, 318, DateTimeKind.Local).AddTicks(3090), new DateTime(2024, 2, 16, 8, 47, 14, 318, DateTimeKind.Local).AddTicks(3110) });

            migrationBuilder.UpdateData(
                table: "SchoolLevels",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2024, 2, 16, 8, 47, 14, 318, DateTimeKind.Local).AddTicks(3112), new DateTime(2024, 2, 16, 8, 47, 14, 318, DateTimeKind.Local).AddTicks(3113) });
        }
    }
}
