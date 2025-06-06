﻿using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SchoolWebApp.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Addedseparatelevelsavailableonschooldetails : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SchoolLevelsAvailable",
                table: "SchoolDetails");

            migrationBuilder.AddColumn<bool>(
                name: "JuniorSchool",
                table: "SchoolDetails",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "LowerPrimary",
                table: "SchoolDetails",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "PrePrimary",
                table: "SchoolDetails",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "SeniorSchool",
                table: "SchoolDetails",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "UpperPrimary",
                table: "SchoolDetails",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "269f0cf3-405e-4163-83f3-1b63ebebd62e",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2024, 6, 26, 10, 39, 23, 541, DateTimeKind.Local).AddTicks(2860), new DateTime(2024, 6, 26, 10, 39, 23, 541, DateTimeKind.Local).AddTicks(2871) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "448df289-142c-4959-a912-60733515e1b4",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2024, 6, 26, 10, 39, 23, 541, DateTimeKind.Local).AddTicks(2822), new DateTime(2024, 6, 26, 10, 39, 23, 541, DateTimeKind.Local).AddTicks(2823) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "48c50c3a-9958-453b-b649-4e21af131322",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2024, 6, 26, 10, 39, 23, 541, DateTimeKind.Local).AddTicks(2784), new DateTime(2024, 6, 26, 10, 39, 23, 541, DateTimeKind.Local).AddTicks(2785) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "717d9b15-a428-440c-b26b-08d3bbb68b02",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2024, 6, 26, 10, 39, 23, 541, DateTimeKind.Local).AddTicks(2621), new DateTime(2024, 6, 26, 10, 39, 23, 541, DateTimeKind.Local).AddTicks(2657) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "95ed2407-3e58-4af2-88a4-1c4e96473f68",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2024, 6, 26, 10, 39, 23, 541, DateTimeKind.Local).AddTicks(2733), new DateTime(2024, 6, 26, 10, 39, 23, 541, DateTimeKind.Local).AddTicks(2734) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "97942bee-ef12-4425-8225-4f293d0f36dd",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2024, 6, 26, 10, 39, 23, 541, DateTimeKind.Local).AddTicks(2973), new DateTime(2024, 6, 26, 10, 39, 23, 541, DateTimeKind.Local).AddTicks(2974) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "cd12b44b-103b-48df-8887-a2bf42e0651e",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2024, 6, 26, 10, 39, 23, 541, DateTimeKind.Local).AddTicks(2938), new DateTime(2024, 6, 26, 10, 39, 23, 541, DateTimeKind.Local).AddTicks(2940) });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "7e67d486-af3e-49f1-a109-a2b864b8e0ec",
                columns: new[] { "ConcurrencyStamp", "Created", "Modified", "PasswordHash", "SecurityStamp" },
                values: new object[] { "a72a9a70-6586-4fb3-9516-5b59ffb7fd26", new DateTime(2024, 6, 26, 10, 39, 23, 541, DateTimeKind.Local).AddTicks(3084), new DateTime(2024, 6, 26, 10, 39, 23, 541, DateTimeKind.Local).AddTicks(3085), "AQAAAAIAAYagAAAAEHktlI/04nqHfRRmKSN4BJb4E/q2jC84Q6mpNUxxpz3etvCe1h9IgV2sytV7NtvCJQ==", "0540e317-4bb2-4a41-8332-26e8fba6d3e4" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "JuniorSchool",
                table: "SchoolDetails");

            migrationBuilder.DropColumn(
                name: "LowerPrimary",
                table: "SchoolDetails");

            migrationBuilder.DropColumn(
                name: "PrePrimary",
                table: "SchoolDetails");

            migrationBuilder.DropColumn(
                name: "SeniorSchool",
                table: "SchoolDetails");

            migrationBuilder.DropColumn(
                name: "UpperPrimary",
                table: "SchoolDetails");

            migrationBuilder.AddColumn<string>(
                name: "SchoolLevelsAvailable",
                table: "SchoolDetails",
                type: "varchar(255)",
                maxLength: 255,
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "269f0cf3-405e-4163-83f3-1b63ebebd62e",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2024, 6, 26, 10, 19, 49, 579, DateTimeKind.Local).AddTicks(7375), new DateTime(2024, 6, 26, 10, 19, 49, 579, DateTimeKind.Local).AddTicks(7382) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "448df289-142c-4959-a912-60733515e1b4",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2024, 6, 26, 10, 19, 49, 579, DateTimeKind.Local).AddTicks(7304), new DateTime(2024, 6, 26, 10, 19, 49, 579, DateTimeKind.Local).AddTicks(7306) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "48c50c3a-9958-453b-b649-4e21af131322",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2024, 6, 26, 10, 19, 49, 579, DateTimeKind.Local).AddTicks(7257), new DateTime(2024, 6, 26, 10, 19, 49, 579, DateTimeKind.Local).AddTicks(7259) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "717d9b15-a428-440c-b26b-08d3bbb68b02",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2024, 6, 26, 10, 19, 49, 579, DateTimeKind.Local).AddTicks(7068), new DateTime(2024, 6, 26, 10, 19, 49, 579, DateTimeKind.Local).AddTicks(7108) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "95ed2407-3e58-4af2-88a4-1c4e96473f68",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2024, 6, 26, 10, 19, 49, 579, DateTimeKind.Local).AddTicks(7203), new DateTime(2024, 6, 26, 10, 19, 49, 579, DateTimeKind.Local).AddTicks(7206) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "97942bee-ef12-4425-8225-4f293d0f36dd",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2024, 6, 26, 10, 19, 49, 579, DateTimeKind.Local).AddTicks(7529), new DateTime(2024, 6, 26, 10, 19, 49, 579, DateTimeKind.Local).AddTicks(7531) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "cd12b44b-103b-48df-8887-a2bf42e0651e",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2024, 6, 26, 10, 19, 49, 579, DateTimeKind.Local).AddTicks(7483), new DateTime(2024, 6, 26, 10, 19, 49, 579, DateTimeKind.Local).AddTicks(7486) });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "7e67d486-af3e-49f1-a109-a2b864b8e0ec",
                columns: new[] { "ConcurrencyStamp", "Created", "Modified", "PasswordHash", "SecurityStamp" },
                values: new object[] { "27fb8029-8588-496c-a2c9-a846ca1beca0", new DateTime(2024, 6, 26, 10, 19, 49, 579, DateTimeKind.Local).AddTicks(7680), new DateTime(2024, 6, 26, 10, 19, 49, 579, DateTimeKind.Local).AddTicks(7683), "AQAAAAIAAYagAAAAEEWNANftR+1smrZ/JsFzUiTmZZ44808vIfiGfppx+sHKyg9UJUCl4ZNP5Kzqgff/MA==", "fc54eb2b-643c-43f6-b95d-bd8918148b91" });
        }
    }
}
