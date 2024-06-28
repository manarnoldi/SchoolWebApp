using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SchoolWebApp.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Routineupdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "269f0cf3-405e-4163-83f3-1b63ebebd62e",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2024, 6, 28, 7, 54, 9, 615, DateTimeKind.Local).AddTicks(1257), new DateTime(2024, 6, 28, 7, 54, 9, 615, DateTimeKind.Local).AddTicks(1287) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "448df289-142c-4959-a912-60733515e1b4",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2024, 6, 28, 7, 54, 9, 615, DateTimeKind.Local).AddTicks(1186), new DateTime(2024, 6, 28, 7, 54, 9, 615, DateTimeKind.Local).AddTicks(1189) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "48c50c3a-9958-453b-b649-4e21af131322",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2024, 6, 28, 7, 54, 9, 615, DateTimeKind.Local).AddTicks(1083), new DateTime(2024, 6, 28, 7, 54, 9, 615, DateTimeKind.Local).AddTicks(1090) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "717d9b15-a428-440c-b26b-08d3bbb68b02",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2024, 6, 28, 7, 54, 9, 615, DateTimeKind.Local).AddTicks(754), new DateTime(2024, 6, 28, 7, 54, 9, 615, DateTimeKind.Local).AddTicks(818) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "95ed2407-3e58-4af2-88a4-1c4e96473f68",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2024, 6, 28, 7, 54, 9, 615, DateTimeKind.Local).AddTicks(1004), new DateTime(2024, 6, 28, 7, 54, 9, 615, DateTimeKind.Local).AddTicks(1008) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "97942bee-ef12-4425-8225-4f293d0f36dd",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2024, 6, 28, 7, 54, 9, 615, DateTimeKind.Local).AddTicks(1484), new DateTime(2024, 6, 28, 7, 54, 9, 615, DateTimeKind.Local).AddTicks(1486) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "cd12b44b-103b-48df-8887-a2bf42e0651e",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2024, 6, 28, 7, 54, 9, 615, DateTimeKind.Local).AddTicks(1381), new DateTime(2024, 6, 28, 7, 54, 9, 615, DateTimeKind.Local).AddTicks(1385) });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "7e67d486-af3e-49f1-a109-a2b864b8e0ec",
                columns: new[] { "ConcurrencyStamp", "Created", "Modified", "PasswordHash", "SecurityStamp" },
                values: new object[] { "2187da7c-6723-473b-9163-c2776f057ccf", new DateTime(2024, 6, 28, 7, 54, 9, 615, DateTimeKind.Local).AddTicks(1783), new DateTime(2024, 6, 28, 7, 54, 9, 615, DateTimeKind.Local).AddTicks(1797), "AQAAAAIAAYagAAAAEDX8j0noY9jAvK3esSUG2LOgJTfl//L3J+YawyWs4lapTJ4//8LNgnGiodFu+D1G3Q==", "676a59d5-06be-4224-a101-d5d36f6aae27" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "269f0cf3-405e-4163-83f3-1b63ebebd62e",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2024, 6, 28, 7, 51, 8, 550, DateTimeKind.Local).AddTicks(1230), new DateTime(2024, 6, 28, 7, 51, 8, 550, DateTimeKind.Local).AddTicks(1231) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "448df289-142c-4959-a912-60733515e1b4",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2024, 6, 28, 7, 51, 8, 550, DateTimeKind.Local).AddTicks(1205), new DateTime(2024, 6, 28, 7, 51, 8, 550, DateTimeKind.Local).AddTicks(1206) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "48c50c3a-9958-453b-b649-4e21af131322",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2024, 6, 28, 7, 51, 8, 550, DateTimeKind.Local).AddTicks(1178), new DateTime(2024, 6, 28, 7, 51, 8, 550, DateTimeKind.Local).AddTicks(1181) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "717d9b15-a428-440c-b26b-08d3bbb68b02",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2024, 6, 28, 7, 51, 8, 550, DateTimeKind.Local).AddTicks(1060), new DateTime(2024, 6, 28, 7, 51, 8, 550, DateTimeKind.Local).AddTicks(1084) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "95ed2407-3e58-4af2-88a4-1c4e96473f68",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2024, 6, 28, 7, 51, 8, 550, DateTimeKind.Local).AddTicks(1145), new DateTime(2024, 6, 28, 7, 51, 8, 550, DateTimeKind.Local).AddTicks(1152) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "97942bee-ef12-4425-8225-4f293d0f36dd",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2024, 6, 28, 7, 51, 8, 550, DateTimeKind.Local).AddTicks(1341), new DateTime(2024, 6, 28, 7, 51, 8, 550, DateTimeKind.Local).AddTicks(1343) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "cd12b44b-103b-48df-8887-a2bf42e0651e",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2024, 6, 28, 7, 51, 8, 550, DateTimeKind.Local).AddTicks(1259), new DateTime(2024, 6, 28, 7, 51, 8, 550, DateTimeKind.Local).AddTicks(1271) });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "7e67d486-af3e-49f1-a109-a2b864b8e0ec",
                columns: new[] { "ConcurrencyStamp", "Created", "Modified", "PasswordHash", "SecurityStamp" },
                values: new object[] { "a357ff00-87c1-4375-8ed6-3e2727721589", new DateTime(2024, 6, 28, 7, 51, 8, 550, DateTimeKind.Local).AddTicks(1541), new DateTime(2024, 6, 28, 7, 51, 8, 550, DateTimeKind.Local).AddTicks(1542), "AQAAAAIAAYagAAAAEOu/MKv+vl7UAdVIxdRTyfg3SkDUT9t9YUQRXWDHwTLYBTyDpB4HE4x1d03Ms6P0JA==", "3467cf75-f3e7-4d6a-9470-a1d8db06d260" });
        }
    }
}
