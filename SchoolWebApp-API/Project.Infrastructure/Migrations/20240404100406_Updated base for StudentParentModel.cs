using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SchoolWebApp.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdatedbaseforStudentParentModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "269f0cf3-405e-4163-83f3-1b63ebebd62e",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2024, 4, 4, 13, 4, 5, 187, DateTimeKind.Local).AddTicks(9935), new DateTime(2024, 4, 4, 13, 4, 5, 187, DateTimeKind.Local).AddTicks(9936) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "448df289-142c-4959-a912-60733515e1b4",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2024, 4, 4, 13, 4, 5, 187, DateTimeKind.Local).AddTicks(9906), new DateTime(2024, 4, 4, 13, 4, 5, 187, DateTimeKind.Local).AddTicks(9907) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "48c50c3a-9958-453b-b649-4e21af131322",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2024, 4, 4, 13, 4, 5, 187, DateTimeKind.Local).AddTicks(9851), new DateTime(2024, 4, 4, 13, 4, 5, 187, DateTimeKind.Local).AddTicks(9868) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "717d9b15-a428-440c-b26b-08d3bbb68b02",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2024, 4, 4, 13, 4, 5, 187, DateTimeKind.Local).AddTicks(9723), new DateTime(2024, 4, 4, 13, 4, 5, 187, DateTimeKind.Local).AddTicks(9752) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "95ed2407-3e58-4af2-88a4-1c4e96473f68",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2024, 4, 4, 13, 4, 5, 187, DateTimeKind.Local).AddTicks(9819), new DateTime(2024, 4, 4, 13, 4, 5, 187, DateTimeKind.Local).AddTicks(9820) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "97942bee-ef12-4425-8225-4f293d0f36dd",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2024, 4, 4, 13, 4, 5, 187, DateTimeKind.Local).AddTicks(9994), new DateTime(2024, 4, 4, 13, 4, 5, 187, DateTimeKind.Local).AddTicks(9995) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "cd12b44b-103b-48df-8887-a2bf42e0651e",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2024, 4, 4, 13, 4, 5, 187, DateTimeKind.Local).AddTicks(9963), new DateTime(2024, 4, 4, 13, 4, 5, 187, DateTimeKind.Local).AddTicks(9964) });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "7e67d486-af3e-49f1-a109-a2b864b8e0ec",
                columns: new[] { "ConcurrencyStamp", "Created", "Modified", "PasswordHash", "SecurityStamp" },
                values: new object[] { "483db0dd-d9b2-4c27-84e7-7b69f5c60a32", new DateTime(2024, 4, 4, 13, 4, 5, 188, DateTimeKind.Local).AddTicks(121), new DateTime(2024, 4, 4, 13, 4, 5, 188, DateTimeKind.Local).AddTicks(122), "AQAAAAIAAYagAAAAEGtcxKiMn5cj/4VPXFxq0GJLXpDFyRT5jUdVKCqefQzEv1L+VljrOOQfoE7uITaP1g==", "b056fcce-e242-4777-9b6b-21344f2d6ee1" });

            migrationBuilder.UpdateData(
                table: "SchoolLevels",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2024, 4, 4, 13, 4, 5, 272, DateTimeKind.Local).AddTicks(3150), new DateTime(2024, 4, 4, 13, 4, 5, 272, DateTimeKind.Local).AddTicks(3176) });

            migrationBuilder.UpdateData(
                table: "SchoolLevels",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2024, 4, 4, 13, 4, 5, 272, DateTimeKind.Local).AddTicks(3178), new DateTime(2024, 4, 4, 13, 4, 5, 272, DateTimeKind.Local).AddTicks(3179) });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "269f0cf3-405e-4163-83f3-1b63ebebd62e",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2024, 3, 1, 10, 54, 46, 975, DateTimeKind.Local).AddTicks(2694), new DateTime(2024, 3, 1, 10, 54, 46, 975, DateTimeKind.Local).AddTicks(2696) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "448df289-142c-4959-a912-60733515e1b4",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2024, 3, 1, 10, 54, 46, 975, DateTimeKind.Local).AddTicks(2630), new DateTime(2024, 3, 1, 10, 54, 46, 975, DateTimeKind.Local).AddTicks(2645) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "48c50c3a-9958-453b-b649-4e21af131322",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2024, 3, 1, 10, 54, 46, 975, DateTimeKind.Local).AddTicks(2596), new DateTime(2024, 3, 1, 10, 54, 46, 975, DateTimeKind.Local).AddTicks(2597) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "717d9b15-a428-440c-b26b-08d3bbb68b02",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2024, 3, 1, 10, 54, 46, 975, DateTimeKind.Local).AddTicks(2354), new DateTime(2024, 3, 1, 10, 54, 46, 975, DateTimeKind.Local).AddTicks(2464) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "95ed2407-3e58-4af2-88a4-1c4e96473f68",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2024, 3, 1, 10, 54, 46, 975, DateTimeKind.Local).AddTicks(2548), new DateTime(2024, 3, 1, 10, 54, 46, 975, DateTimeKind.Local).AddTicks(2549) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "97942bee-ef12-4425-8225-4f293d0f36dd",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2024, 3, 1, 10, 54, 46, 975, DateTimeKind.Local).AddTicks(2785), new DateTime(2024, 3, 1, 10, 54, 46, 975, DateTimeKind.Local).AddTicks(2787) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "cd12b44b-103b-48df-8887-a2bf42e0651e",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2024, 3, 1, 10, 54, 46, 975, DateTimeKind.Local).AddTicks(2751), new DateTime(2024, 3, 1, 10, 54, 46, 975, DateTimeKind.Local).AddTicks(2752) });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "7e67d486-af3e-49f1-a109-a2b864b8e0ec",
                columns: new[] { "ConcurrencyStamp", "Created", "Modified", "PasswordHash", "SecurityStamp" },
                values: new object[] { "299c5483-0713-434f-a887-4250f2c72dc5", new DateTime(2024, 3, 1, 10, 54, 46, 975, DateTimeKind.Local).AddTicks(2895), new DateTime(2024, 3, 1, 10, 54, 46, 975, DateTimeKind.Local).AddTicks(2897), "AQAAAAIAAYagAAAAEDSD/T5BB+kaE0etksxMJrMkg43NE+JnM9HRqVqBPERirPAFdR5NTygwl6AGcUBo/w==", "11e2229c-53d6-49dc-95dc-ff098face453" });

            migrationBuilder.UpdateData(
                table: "SchoolLevels",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2024, 3, 1, 10, 54, 47, 138, DateTimeKind.Local).AddTicks(1604), new DateTime(2024, 3, 1, 10, 54, 47, 138, DateTimeKind.Local).AddTicks(1643) });

            migrationBuilder.UpdateData(
                table: "SchoolLevels",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2024, 3, 1, 10, 54, 47, 138, DateTimeKind.Local).AddTicks(1648), new DateTime(2024, 3, 1, 10, 54, 47, 138, DateTimeKind.Local).AddTicks(1649) });
        }
    }
}
