using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SchoolWebApp.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class MadedatefieldsforStafdetailsnullable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "DateOfBirth",
                table: "Person",
                type: "datetime(6)",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "269f0cf3-405e-4163-83f3-1b63ebebd62e",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2024, 7, 3, 13, 11, 2, 927, DateTimeKind.Local).AddTicks(3306), new DateTime(2024, 7, 3, 13, 11, 2, 927, DateTimeKind.Local).AddTicks(3335) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "448df289-142c-4959-a912-60733515e1b4",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2024, 7, 3, 13, 11, 2, 927, DateTimeKind.Local).AddTicks(3279), new DateTime(2024, 7, 3, 13, 11, 2, 927, DateTimeKind.Local).AddTicks(3280) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "48c50c3a-9958-453b-b649-4e21af131322",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2024, 7, 3, 13, 11, 2, 927, DateTimeKind.Local).AddTicks(3251), new DateTime(2024, 7, 3, 13, 11, 2, 927, DateTimeKind.Local).AddTicks(3253) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "717d9b15-a428-440c-b26b-08d3bbb68b02",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2024, 7, 3, 13, 11, 2, 927, DateTimeKind.Local).AddTicks(3109), new DateTime(2024, 7, 3, 13, 11, 2, 927, DateTimeKind.Local).AddTicks(3142) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "95ed2407-3e58-4af2-88a4-1c4e96473f68",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2024, 7, 3, 13, 11, 2, 927, DateTimeKind.Local).AddTicks(3221), new DateTime(2024, 7, 3, 13, 11, 2, 927, DateTimeKind.Local).AddTicks(3223) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "97942bee-ef12-4425-8225-4f293d0f36dd",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2024, 7, 3, 13, 11, 2, 927, DateTimeKind.Local).AddTicks(3400), new DateTime(2024, 7, 3, 13, 11, 2, 927, DateTimeKind.Local).AddTicks(3401) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "cd12b44b-103b-48df-8887-a2bf42e0651e",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2024, 7, 3, 13, 11, 2, 927, DateTimeKind.Local).AddTicks(3374), new DateTime(2024, 7, 3, 13, 11, 2, 927, DateTimeKind.Local).AddTicks(3376) });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "7e67d486-af3e-49f1-a109-a2b864b8e0ec",
                columns: new[] { "ConcurrencyStamp", "Created", "Modified", "PasswordHash", "SecurityStamp" },
                values: new object[] { "79b5edae-837e-45a9-a107-59451204c83e", new DateTime(2024, 7, 3, 13, 11, 2, 927, DateTimeKind.Local).AddTicks(3524), new DateTime(2024, 7, 3, 13, 11, 2, 927, DateTimeKind.Local).AddTicks(3526), "AQAAAAIAAYagAAAAEOLmYcHihUF8KtoPUg+QwdW/UX6J70VEeUhBAaNFa0sZ5bKD4zeph4387a4BjPQmqg==", "b6787386-ce3a-4bbd-929e-02737e57fcd3" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "DateOfBirth",
                table: "Person",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldNullable: true);

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "269f0cf3-405e-4163-83f3-1b63ebebd62e",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2024, 7, 2, 17, 58, 39, 260, DateTimeKind.Local).AddTicks(2811), new DateTime(2024, 7, 2, 17, 58, 39, 260, DateTimeKind.Local).AddTicks(2835) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "448df289-142c-4959-a912-60733515e1b4",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2024, 7, 2, 17, 58, 39, 260, DateTimeKind.Local).AddTicks(2758), new DateTime(2024, 7, 2, 17, 58, 39, 260, DateTimeKind.Local).AddTicks(2759) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "48c50c3a-9958-453b-b649-4e21af131322",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2024, 7, 2, 17, 58, 39, 260, DateTimeKind.Local).AddTicks(2716), new DateTime(2024, 7, 2, 17, 58, 39, 260, DateTimeKind.Local).AddTicks(2718) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "717d9b15-a428-440c-b26b-08d3bbb68b02",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2024, 7, 2, 17, 58, 39, 260, DateTimeKind.Local).AddTicks(2457), new DateTime(2024, 7, 2, 17, 58, 39, 260, DateTimeKind.Local).AddTicks(2496) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "95ed2407-3e58-4af2-88a4-1c4e96473f68",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2024, 7, 2, 17, 58, 39, 260, DateTimeKind.Local).AddTicks(2577), new DateTime(2024, 7, 2, 17, 58, 39, 260, DateTimeKind.Local).AddTicks(2579) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "97942bee-ef12-4425-8225-4f293d0f36dd",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2024, 7, 2, 17, 58, 39, 260, DateTimeKind.Local).AddTicks(2956), new DateTime(2024, 7, 2, 17, 58, 39, 260, DateTimeKind.Local).AddTicks(2958) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "cd12b44b-103b-48df-8887-a2bf42e0651e",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2024, 7, 2, 17, 58, 39, 260, DateTimeKind.Local).AddTicks(2915), new DateTime(2024, 7, 2, 17, 58, 39, 260, DateTimeKind.Local).AddTicks(2918) });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "7e67d486-af3e-49f1-a109-a2b864b8e0ec",
                columns: new[] { "ConcurrencyStamp", "Created", "Modified", "PasswordHash", "SecurityStamp" },
                values: new object[] { "2a54c3ca-73e3-4686-a5aa-cb377a152987", new DateTime(2024, 7, 2, 17, 58, 39, 260, DateTimeKind.Local).AddTicks(3080), new DateTime(2024, 7, 2, 17, 58, 39, 260, DateTimeKind.Local).AddTicks(3082), "AQAAAAIAAYagAAAAEEYr6OAq38p+oSiTq5gSyxLspYR6rP37q/6/I0cUWoGXWFdb//BBLdzbus4BtF9FrQ==", "f0e21041-cfe1-46bc-b0ea-0610e8543647" });
        }
    }
}
