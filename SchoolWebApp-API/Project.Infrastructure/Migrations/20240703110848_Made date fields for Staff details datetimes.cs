using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SchoolWebApp.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class MadedatefieldsforStaffdetailsdatetimes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "EndofEmploymentDate",
                table: "Person",
                type: "datetime(6)",
                nullable: true,
                oldClrType: typeof(DateOnly),
                oldType: "date",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "EmploymentDate",
                table: "Person",
                type: "datetime(6)",
                nullable: true,
                oldClrType: typeof(DateOnly),
                oldType: "date",
                oldNullable: true);

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "269f0cf3-405e-4163-83f3-1b63ebebd62e",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2024, 7, 3, 14, 8, 46, 267, DateTimeKind.Local).AddTicks(3321), new DateTime(2024, 7, 3, 14, 8, 46, 267, DateTimeKind.Local).AddTicks(3333) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "448df289-142c-4959-a912-60733515e1b4",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2024, 7, 3, 14, 8, 46, 267, DateTimeKind.Local).AddTicks(3204), new DateTime(2024, 7, 3, 14, 8, 46, 267, DateTimeKind.Local).AddTicks(3206) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "48c50c3a-9958-453b-b649-4e21af131322",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2024, 7, 3, 14, 8, 46, 267, DateTimeKind.Local).AddTicks(3151), new DateTime(2024, 7, 3, 14, 8, 46, 267, DateTimeKind.Local).AddTicks(3153) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "717d9b15-a428-440c-b26b-08d3bbb68b02",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2024, 7, 3, 14, 8, 46, 267, DateTimeKind.Local).AddTicks(2894), new DateTime(2024, 7, 3, 14, 8, 46, 267, DateTimeKind.Local).AddTicks(2938) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "95ed2407-3e58-4af2-88a4-1c4e96473f68",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2024, 7, 3, 14, 8, 46, 267, DateTimeKind.Local).AddTicks(3071), new DateTime(2024, 7, 3, 14, 8, 46, 267, DateTimeKind.Local).AddTicks(3077) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "97942bee-ef12-4425-8225-4f293d0f36dd",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2024, 7, 3, 14, 8, 46, 267, DateTimeKind.Local).AddTicks(3439), new DateTime(2024, 7, 3, 14, 8, 46, 267, DateTimeKind.Local).AddTicks(3441) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "cd12b44b-103b-48df-8887-a2bf42e0651e",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2024, 7, 3, 14, 8, 46, 267, DateTimeKind.Local).AddTicks(3395), new DateTime(2024, 7, 3, 14, 8, 46, 267, DateTimeKind.Local).AddTicks(3397) });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "7e67d486-af3e-49f1-a109-a2b864b8e0ec",
                columns: new[] { "ConcurrencyStamp", "Created", "Modified", "PasswordHash", "SecurityStamp" },
                values: new object[] { "7ea4b02c-99fe-420c-86b9-f0c1885e96b8", new DateTime(2024, 7, 3, 14, 8, 46, 267, DateTimeKind.Local).AddTicks(3601), new DateTime(2024, 7, 3, 14, 8, 46, 267, DateTimeKind.Local).AddTicks(3603), "AQAAAAIAAYagAAAAEHP8d2kjSLRPEKccTnPlNF5uSLkqPQUAia4k10um1vXEdkSRoV3y+rsvOw5KvjuA+g==", "661f3f3f-8f0d-48f4-8370-278acb13299c" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateOnly>(
                name: "EndofEmploymentDate",
                table: "Person",
                type: "date",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateOnly>(
                name: "EmploymentDate",
                table: "Person",
                type: "date",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldNullable: true);

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
    }
}
