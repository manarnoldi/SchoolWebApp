using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SchoolWebApp.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdatedEventStatustoBoolfromEnum : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<bool>(
                name: "Status",
                table: "Events",
                type: "tinyint(1)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "269f0cf3-405e-4163-83f3-1b63ebebd62e",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2024, 2, 23, 16, 3, 19, 180, DateTimeKind.Local).AddTicks(399), new DateTime(2024, 2, 23, 16, 3, 19, 180, DateTimeKind.Local).AddTicks(400) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "448df289-142c-4959-a912-60733515e1b4",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2024, 2, 23, 16, 3, 19, 180, DateTimeKind.Local).AddTicks(373), new DateTime(2024, 2, 23, 16, 3, 19, 180, DateTimeKind.Local).AddTicks(374) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "48c50c3a-9958-453b-b649-4e21af131322",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2024, 2, 23, 16, 3, 19, 180, DateTimeKind.Local).AddTicks(311), new DateTime(2024, 2, 23, 16, 3, 19, 180, DateTimeKind.Local).AddTicks(335) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "717d9b15-a428-440c-b26b-08d3bbb68b02",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2024, 2, 23, 16, 3, 19, 180, DateTimeKind.Local).AddTicks(187), new DateTime(2024, 2, 23, 16, 3, 19, 180, DateTimeKind.Local).AddTicks(217) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "95ed2407-3e58-4af2-88a4-1c4e96473f68",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2024, 2, 23, 16, 3, 19, 180, DateTimeKind.Local).AddTicks(278), new DateTime(2024, 2, 23, 16, 3, 19, 180, DateTimeKind.Local).AddTicks(279) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "97942bee-ef12-4425-8225-4f293d0f36dd",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2024, 2, 23, 16, 3, 19, 180, DateTimeKind.Local).AddTicks(453), new DateTime(2024, 2, 23, 16, 3, 19, 180, DateTimeKind.Local).AddTicks(454) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "cd12b44b-103b-48df-8887-a2bf42e0651e",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2024, 2, 23, 16, 3, 19, 180, DateTimeKind.Local).AddTicks(427), new DateTime(2024, 2, 23, 16, 3, 19, 180, DateTimeKind.Local).AddTicks(428) });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "7e67d486-af3e-49f1-a109-a2b864b8e0ec",
                columns: new[] { "ConcurrencyStamp", "Created", "Modified", "PasswordHash", "SecurityStamp" },
                values: new object[] { "b74dcec5-5346-41a7-88b2-2804cc639b09", new DateTime(2024, 2, 23, 16, 3, 19, 180, DateTimeKind.Local).AddTicks(540), new DateTime(2024, 2, 23, 16, 3, 19, 180, DateTimeKind.Local).AddTicks(541), "AQAAAAIAAYagAAAAEEcj/1sBFKuF/pHcVUVuF0eMkns9hm86JJGsKznNAssq+ERV3uw+e0MCsy+skcr2Lg==", "4cb94c53-ed15-4341-ba1f-1e9b2de12dde" });

            migrationBuilder.UpdateData(
                table: "SchoolLevels",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2024, 2, 23, 16, 3, 19, 260, DateTimeKind.Local).AddTicks(950), new DateTime(2024, 2, 23, 16, 3, 19, 260, DateTimeKind.Local).AddTicks(978) });

            migrationBuilder.UpdateData(
                table: "SchoolLevels",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2024, 2, 23, 16, 3, 19, 260, DateTimeKind.Local).AddTicks(981), new DateTime(2024, 2, 23, 16, 3, 19, 260, DateTimeKind.Local).AddTicks(982) });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "Status",
                table: "Events",
                type: "int",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "tinyint(1)");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "269f0cf3-405e-4163-83f3-1b63ebebd62e",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2024, 2, 23, 13, 14, 38, 816, DateTimeKind.Local).AddTicks(5970), new DateTime(2024, 2, 23, 13, 14, 38, 816, DateTimeKind.Local).AddTicks(5974) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "448df289-142c-4959-a912-60733515e1b4",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2024, 2, 23, 13, 14, 38, 816, DateTimeKind.Local).AddTicks(5850), new DateTime(2024, 2, 23, 13, 14, 38, 816, DateTimeKind.Local).AddTicks(5855) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "48c50c3a-9958-453b-b649-4e21af131322",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2024, 2, 23, 13, 14, 38, 816, DateTimeKind.Local).AddTicks(5690), new DateTime(2024, 2, 23, 13, 14, 38, 816, DateTimeKind.Local).AddTicks(5727) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "717d9b15-a428-440c-b26b-08d3bbb68b02",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2024, 2, 23, 13, 14, 38, 816, DateTimeKind.Local).AddTicks(5330), new DateTime(2024, 2, 23, 13, 14, 38, 816, DateTimeKind.Local).AddTicks(5399) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "95ed2407-3e58-4af2-88a4-1c4e96473f68",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2024, 2, 23, 13, 14, 38, 816, DateTimeKind.Local).AddTicks(5583), new DateTime(2024, 2, 23, 13, 14, 38, 816, DateTimeKind.Local).AddTicks(5588) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "97942bee-ef12-4425-8225-4f293d0f36dd",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2024, 2, 23, 13, 14, 38, 816, DateTimeKind.Local).AddTicks(6229), new DateTime(2024, 2, 23, 13, 14, 38, 816, DateTimeKind.Local).AddTicks(6233) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "cd12b44b-103b-48df-8887-a2bf42e0651e",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2024, 2, 23, 13, 14, 38, 816, DateTimeKind.Local).AddTicks(6125), new DateTime(2024, 2, 23, 13, 14, 38, 816, DateTimeKind.Local).AddTicks(6129) });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "7e67d486-af3e-49f1-a109-a2b864b8e0ec",
                columns: new[] { "ConcurrencyStamp", "Created", "Modified", "PasswordHash", "SecurityStamp" },
                values: new object[] { "c9becb0e-8775-4e2c-be55-1f98d2ce17f1", new DateTime(2024, 2, 23, 13, 14, 38, 816, DateTimeKind.Local).AddTicks(6523), new DateTime(2024, 2, 23, 13, 14, 38, 816, DateTimeKind.Local).AddTicks(6530), "AQAAAAIAAYagAAAAEEt1Ed4omjaNtDI0ivdngYbgXwYZej3OgJpzPir2WJaqwKA54tM1PuZX/UgOJL6rUw==", "625d5b2a-7393-4ae1-8940-f72f7f38aff6" });

            migrationBuilder.UpdateData(
                table: "SchoolLevels",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2024, 2, 23, 13, 14, 39, 324, DateTimeKind.Local).AddTicks(913), new DateTime(2024, 2, 23, 13, 14, 39, 324, DateTimeKind.Local).AddTicks(963) });

            migrationBuilder.UpdateData(
                table: "SchoolLevels",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2024, 2, 23, 13, 14, 39, 324, DateTimeKind.Local).AddTicks(975), new DateTime(2024, 2, 23, 13, 14, 39, 324, DateTimeKind.Local).AddTicks(978) });
        }
    }
}
