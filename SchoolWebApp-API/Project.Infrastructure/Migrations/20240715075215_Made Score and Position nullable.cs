using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SchoolWebApp.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class MadeScoreandPositionnullable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Score",
                table: "FormerSchools",
                type: "varchar(255)",
                maxLength: 255,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(255)",
                oldMaxLength: 255)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "Position",
                table: "FormerSchools",
                type: "varchar(255)",
                maxLength: 255,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(255)",
                oldMaxLength: 255)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "269f0cf3-405e-4163-83f3-1b63ebebd62e",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2024, 7, 15, 10, 52, 14, 66, DateTimeKind.Local).AddTicks(7418), new DateTime(2024, 7, 15, 10, 52, 14, 66, DateTimeKind.Local).AddTicks(7441) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "448df289-142c-4959-a912-60733515e1b4",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2024, 7, 15, 10, 52, 14, 66, DateTimeKind.Local).AddTicks(7375), new DateTime(2024, 7, 15, 10, 52, 14, 66, DateTimeKind.Local).AddTicks(7377) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "48c50c3a-9958-453b-b649-4e21af131322",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2024, 7, 15, 10, 52, 14, 66, DateTimeKind.Local).AddTicks(7351), new DateTime(2024, 7, 15, 10, 52, 14, 66, DateTimeKind.Local).AddTicks(7352) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "717d9b15-a428-440c-b26b-08d3bbb68b02",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2024, 7, 15, 10, 52, 14, 66, DateTimeKind.Local).AddTicks(7222), new DateTime(2024, 7, 15, 10, 52, 14, 66, DateTimeKind.Local).AddTicks(7261) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "95ed2407-3e58-4af2-88a4-1c4e96473f68",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2024, 7, 15, 10, 52, 14, 66, DateTimeKind.Local).AddTicks(7324), new DateTime(2024, 7, 15, 10, 52, 14, 66, DateTimeKind.Local).AddTicks(7326) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "97942bee-ef12-4425-8225-4f293d0f36dd",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2024, 7, 15, 10, 52, 14, 66, DateTimeKind.Local).AddTicks(7507), new DateTime(2024, 7, 15, 10, 52, 14, 66, DateTimeKind.Local).AddTicks(7508) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "cd12b44b-103b-48df-8887-a2bf42e0651e",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2024, 7, 15, 10, 52, 14, 66, DateTimeKind.Local).AddTicks(7480), new DateTime(2024, 7, 15, 10, 52, 14, 66, DateTimeKind.Local).AddTicks(7482) });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "7e67d486-af3e-49f1-a109-a2b864b8e0ec",
                columns: new[] { "ConcurrencyStamp", "Created", "Modified", "PasswordHash", "SecurityStamp" },
                values: new object[] { "f6dd82d0-ca6a-4924-b85b-2067ab50db32", new DateTime(2024, 7, 15, 10, 52, 14, 66, DateTimeKind.Local).AddTicks(7622), new DateTime(2024, 7, 15, 10, 52, 14, 66, DateTimeKind.Local).AddTicks(7624), "AQAAAAIAAYagAAAAEGJYVUmbc//gfdaJhEgNjL4lS24jhfT1B+LGPG4DUjp8tL4FuLtftJr6nk5XYKV/hg==", "d01fba01-c80e-42c2-9e42-e7a526894ebb" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "FormerSchools",
                keyColumn: "Score",
                keyValue: null,
                column: "Score",
                value: "");

            migrationBuilder.AlterColumn<string>(
                name: "Score",
                table: "FormerSchools",
                type: "varchar(255)",
                maxLength: 255,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(255)",
                oldMaxLength: 255,
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.UpdateData(
                table: "FormerSchools",
                keyColumn: "Position",
                keyValue: null,
                column: "Position",
                value: "");

            migrationBuilder.AlterColumn<string>(
                name: "Position",
                table: "FormerSchools",
                type: "varchar(255)",
                maxLength: 255,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(255)",
                oldMaxLength: 255,
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

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
    }
}
