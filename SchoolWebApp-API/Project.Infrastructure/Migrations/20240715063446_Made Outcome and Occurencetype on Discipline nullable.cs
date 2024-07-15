using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SchoolWebApp.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class MadeOutcomeandOccurencetypeonDisciplinenullable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "OutcomeId",
                table: "Discipline",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "OccurenceTypeId",
                table: "Discipline",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "269f0cf3-405e-4163-83f3-1b63ebebd62e",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2024, 7, 15, 9, 34, 44, 962, DateTimeKind.Local).AddTicks(6064), new DateTime(2024, 7, 15, 9, 34, 44, 962, DateTimeKind.Local).AddTicks(6073) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "448df289-142c-4959-a912-60733515e1b4",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2024, 7, 15, 9, 34, 44, 962, DateTimeKind.Local).AddTicks(6038), new DateTime(2024, 7, 15, 9, 34, 44, 962, DateTimeKind.Local).AddTicks(6039) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "48c50c3a-9958-453b-b649-4e21af131322",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2024, 7, 15, 9, 34, 44, 962, DateTimeKind.Local).AddTicks(6011), new DateTime(2024, 7, 15, 9, 34, 44, 962, DateTimeKind.Local).AddTicks(6012) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "717d9b15-a428-440c-b26b-08d3bbb68b02",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2024, 7, 15, 9, 34, 44, 962, DateTimeKind.Local).AddTicks(5888), new DateTime(2024, 7, 15, 9, 34, 44, 962, DateTimeKind.Local).AddTicks(5920) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "95ed2407-3e58-4af2-88a4-1c4e96473f68",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2024, 7, 15, 9, 34, 44, 962, DateTimeKind.Local).AddTicks(5981), new DateTime(2024, 7, 15, 9, 34, 44, 962, DateTimeKind.Local).AddTicks(5983) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "97942bee-ef12-4425-8225-4f293d0f36dd",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2024, 7, 15, 9, 34, 44, 962, DateTimeKind.Local).AddTicks(6139), new DateTime(2024, 7, 15, 9, 34, 44, 962, DateTimeKind.Local).AddTicks(6140) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "cd12b44b-103b-48df-8887-a2bf42e0651e",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2024, 7, 15, 9, 34, 44, 962, DateTimeKind.Local).AddTicks(6112), new DateTime(2024, 7, 15, 9, 34, 44, 962, DateTimeKind.Local).AddTicks(6113) });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "7e67d486-af3e-49f1-a109-a2b864b8e0ec",
                columns: new[] { "ConcurrencyStamp", "Created", "Modified", "PasswordHash", "SecurityStamp" },
                values: new object[] { "59467064-24e4-4a3f-bcc3-b85606207c02", new DateTime(2024, 7, 15, 9, 34, 44, 962, DateTimeKind.Local).AddTicks(6240), new DateTime(2024, 7, 15, 9, 34, 44, 962, DateTimeKind.Local).AddTicks(6241), "AQAAAAIAAYagAAAAEFND3Hw2CFUnJTF0Ff017bUNyScAN1aMtuk2S7U7OJEvOW6qJYowXdWHSGxXW7uVCQ==", "1d4172e9-011b-4948-b0df-3061e007fc3c" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "OutcomeId",
                table: "Discipline",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "OccurenceTypeId",
                table: "Discipline",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "269f0cf3-405e-4163-83f3-1b63ebebd62e",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2024, 7, 12, 22, 16, 14, 878, DateTimeKind.Local).AddTicks(9398), new DateTime(2024, 7, 12, 22, 16, 14, 878, DateTimeKind.Local).AddTicks(9408) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "448df289-142c-4959-a912-60733515e1b4",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2024, 7, 12, 22, 16, 14, 878, DateTimeKind.Local).AddTicks(9353), new DateTime(2024, 7, 12, 22, 16, 14, 878, DateTimeKind.Local).AddTicks(9355) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "48c50c3a-9958-453b-b649-4e21af131322",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2024, 7, 12, 22, 16, 14, 878, DateTimeKind.Local).AddTicks(9324), new DateTime(2024, 7, 12, 22, 16, 14, 878, DateTimeKind.Local).AddTicks(9326) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "717d9b15-a428-440c-b26b-08d3bbb68b02",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2024, 7, 12, 22, 16, 14, 878, DateTimeKind.Local).AddTicks(9186), new DateTime(2024, 7, 12, 22, 16, 14, 878, DateTimeKind.Local).AddTicks(9221) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "95ed2407-3e58-4af2-88a4-1c4e96473f68",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2024, 7, 12, 22, 16, 14, 878, DateTimeKind.Local).AddTicks(9282), new DateTime(2024, 7, 12, 22, 16, 14, 878, DateTimeKind.Local).AddTicks(9283) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "97942bee-ef12-4425-8225-4f293d0f36dd",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2024, 7, 12, 22, 16, 14, 878, DateTimeKind.Local).AddTicks(9485), new DateTime(2024, 7, 12, 22, 16, 14, 878, DateTimeKind.Local).AddTicks(9487) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "cd12b44b-103b-48df-8887-a2bf42e0651e",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2024, 7, 12, 22, 16, 14, 878, DateTimeKind.Local).AddTicks(9457), new DateTime(2024, 7, 12, 22, 16, 14, 878, DateTimeKind.Local).AddTicks(9459) });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "7e67d486-af3e-49f1-a109-a2b864b8e0ec",
                columns: new[] { "ConcurrencyStamp", "Created", "Modified", "PasswordHash", "SecurityStamp" },
                values: new object[] { "aefdd437-1b4b-4f98-a753-5f3368092de6", new DateTime(2024, 7, 12, 22, 16, 14, 878, DateTimeKind.Local).AddTicks(9608), new DateTime(2024, 7, 12, 22, 16, 14, 878, DateTimeKind.Local).AddTicks(9610), "AQAAAAIAAYagAAAAENz2q0ntTKnCPhL/LD1a6fIfCI/zgOYoepVYSNtmhIzEGh1R+BuS8v+/Sg239tA2ow==", "c831359a-3d0d-49f4-a2ae-850992644742" });
        }
    }
}
