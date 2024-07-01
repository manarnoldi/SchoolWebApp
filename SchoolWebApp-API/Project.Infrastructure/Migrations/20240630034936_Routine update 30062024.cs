using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SchoolWebApp.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Routineupdate30062024 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "269f0cf3-405e-4163-83f3-1b63ebebd62e",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2024, 6, 30, 6, 49, 35, 43, DateTimeKind.Local).AddTicks(5199), new DateTime(2024, 6, 30, 6, 49, 35, 43, DateTimeKind.Local).AddTicks(5210) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "448df289-142c-4959-a912-60733515e1b4",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2024, 6, 30, 6, 49, 35, 43, DateTimeKind.Local).AddTicks(5176), new DateTime(2024, 6, 30, 6, 49, 35, 43, DateTimeKind.Local).AddTicks(5177) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "48c50c3a-9958-453b-b649-4e21af131322",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2024, 6, 30, 6, 49, 35, 43, DateTimeKind.Local).AddTicks(5152), new DateTime(2024, 6, 30, 6, 49, 35, 43, DateTimeKind.Local).AddTicks(5153) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "717d9b15-a428-440c-b26b-08d3bbb68b02",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2024, 6, 30, 6, 49, 35, 43, DateTimeKind.Local).AddTicks(4943), new DateTime(2024, 6, 30, 6, 49, 35, 43, DateTimeKind.Local).AddTicks(4976) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "95ed2407-3e58-4af2-88a4-1c4e96473f68",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2024, 6, 30, 6, 49, 35, 43, DateTimeKind.Local).AddTicks(5124), new DateTime(2024, 6, 30, 6, 49, 35, 43, DateTimeKind.Local).AddTicks(5126) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "97942bee-ef12-4425-8225-4f293d0f36dd",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2024, 6, 30, 6, 49, 35, 43, DateTimeKind.Local).AddTicks(5289), new DateTime(2024, 6, 30, 6, 49, 35, 43, DateTimeKind.Local).AddTicks(5290) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "cd12b44b-103b-48df-8887-a2bf42e0651e",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2024, 6, 30, 6, 49, 35, 43, DateTimeKind.Local).AddTicks(5246), new DateTime(2024, 6, 30, 6, 49, 35, 43, DateTimeKind.Local).AddTicks(5248) });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "7e67d486-af3e-49f1-a109-a2b864b8e0ec",
                columns: new[] { "ConcurrencyStamp", "Created", "Modified", "PasswordHash", "SecurityStamp" },
                values: new object[] { "e7bfbbc3-dabb-4043-9ee7-817a93cab698", new DateTime(2024, 6, 30, 6, 49, 35, 43, DateTimeKind.Local).AddTicks(5403), new DateTime(2024, 6, 30, 6, 49, 35, 43, DateTimeKind.Local).AddTicks(5404), "AQAAAAIAAYagAAAAEA3ePonTkAFh2VABjQhe2Zd6TjJx3dbTKgvdFXJFKFGzLwqsCo+wrhf4WyspuCi/Mg==", "ad823eb9-762e-48b1-a351-1988d33c8e4e" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
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
    }
}
