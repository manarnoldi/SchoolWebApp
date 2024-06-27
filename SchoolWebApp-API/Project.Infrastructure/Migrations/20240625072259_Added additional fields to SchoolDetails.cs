using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SchoolWebApp.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddedadditionalfieldstoSchoolDetails : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "OtherDetails",
                table: "SchoolDetails",
                type: "varchar(255)",
                maxLength: 255,
                nullable: false,
                defaultValue: "")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "ReportHeader",
                table: "SchoolDetails",
                type: "varchar(255)",
                maxLength: 255,
                nullable: false,
                defaultValue: "")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "ReportSubTitle",
                table: "SchoolDetails",
                type: "varchar(255)",
                maxLength: 255,
                nullable: false,
                defaultValue: "")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "ReportTitle",
                table: "SchoolDetails",
                type: "varchar(255)",
                maxLength: 255,
                nullable: false,
                defaultValue: "")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "ReportTitleDetails",
                table: "SchoolDetails",
                type: "varchar(255)",
                maxLength: 255,
                nullable: false,
                defaultValue: "")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "269f0cf3-405e-4163-83f3-1b63ebebd62e",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2024, 6, 25, 10, 22, 57, 620, DateTimeKind.Local).AddTicks(8407), new DateTime(2024, 6, 25, 10, 22, 57, 620, DateTimeKind.Local).AddTicks(8418) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "448df289-142c-4959-a912-60733515e1b4",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2024, 6, 25, 10, 22, 57, 620, DateTimeKind.Local).AddTicks(8361), new DateTime(2024, 6, 25, 10, 22, 57, 620, DateTimeKind.Local).AddTicks(8362) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "48c50c3a-9958-453b-b649-4e21af131322",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2024, 6, 25, 10, 22, 57, 620, DateTimeKind.Local).AddTicks(8332), new DateTime(2024, 6, 25, 10, 22, 57, 620, DateTimeKind.Local).AddTicks(8333) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "717d9b15-a428-440c-b26b-08d3bbb68b02",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2024, 6, 25, 10, 22, 57, 620, DateTimeKind.Local).AddTicks(8200), new DateTime(2024, 6, 25, 10, 22, 57, 620, DateTimeKind.Local).AddTicks(8235) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "95ed2407-3e58-4af2-88a4-1c4e96473f68",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2024, 6, 25, 10, 22, 57, 620, DateTimeKind.Local).AddTicks(8300), new DateTime(2024, 6, 25, 10, 22, 57, 620, DateTimeKind.Local).AddTicks(8302) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "97942bee-ef12-4425-8225-4f293d0f36dd",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2024, 6, 25, 10, 22, 57, 620, DateTimeKind.Local).AddTicks(8486), new DateTime(2024, 6, 25, 10, 22, 57, 620, DateTimeKind.Local).AddTicks(8487) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "cd12b44b-103b-48df-8887-a2bf42e0651e",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2024, 6, 25, 10, 22, 57, 620, DateTimeKind.Local).AddTicks(8459), new DateTime(2024, 6, 25, 10, 22, 57, 620, DateTimeKind.Local).AddTicks(8460) });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "7e67d486-af3e-49f1-a109-a2b864b8e0ec",
                columns: new[] { "ConcurrencyStamp", "Created", "Modified", "PasswordHash", "SecurityStamp" },
                values: new object[] { "beab849d-2f1d-4621-b77e-8e1c5b010c63", new DateTime(2024, 6, 25, 10, 22, 57, 620, DateTimeKind.Local).AddTicks(8595), new DateTime(2024, 6, 25, 10, 22, 57, 620, DateTimeKind.Local).AddTicks(8596), "AQAAAAIAAYagAAAAEMvf/kbRMgbtgMOKtP5IB0ngJq3qNpncVsE5jF8hqCrgxUgE8DLcg7X79IMf7ZFiBQ==", "291cd81f-23a4-4901-9c28-bb3de42d3e7d" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OtherDetails",
                table: "SchoolDetails");

            migrationBuilder.DropColumn(
                name: "ReportHeader",
                table: "SchoolDetails");

            migrationBuilder.DropColumn(
                name: "ReportSubTitle",
                table: "SchoolDetails");

            migrationBuilder.DropColumn(
                name: "ReportTitle",
                table: "SchoolDetails");

            migrationBuilder.DropColumn(
                name: "ReportTitleDetails",
                table: "SchoolDetails");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "269f0cf3-405e-4163-83f3-1b63ebebd62e",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2024, 6, 13, 10, 38, 28, 858, DateTimeKind.Local).AddTicks(5582), new DateTime(2024, 6, 13, 10, 38, 28, 858, DateTimeKind.Local).AddTicks(5585) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "448df289-142c-4959-a912-60733515e1b4",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2024, 6, 13, 10, 38, 28, 858, DateTimeKind.Local).AddTicks(5487), new DateTime(2024, 6, 13, 10, 38, 28, 858, DateTimeKind.Local).AddTicks(5490) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "48c50c3a-9958-453b-b649-4e21af131322",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2024, 6, 13, 10, 38, 28, 858, DateTimeKind.Local).AddTicks(5400), new DateTime(2024, 6, 13, 10, 38, 28, 858, DateTimeKind.Local).AddTicks(5403) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "717d9b15-a428-440c-b26b-08d3bbb68b02",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2024, 6, 13, 10, 38, 28, 858, DateTimeKind.Local).AddTicks(5030), new DateTime(2024, 6, 13, 10, 38, 28, 858, DateTimeKind.Local).AddTicks(5084) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "95ed2407-3e58-4af2-88a4-1c4e96473f68",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2024, 6, 13, 10, 38, 28, 858, DateTimeKind.Local).AddTicks(5298), new DateTime(2024, 6, 13, 10, 38, 28, 858, DateTimeKind.Local).AddTicks(5314) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "97942bee-ef12-4425-8225-4f293d0f36dd",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2024, 6, 13, 10, 38, 28, 858, DateTimeKind.Local).AddTicks(5844), new DateTime(2024, 6, 13, 10, 38, 28, 858, DateTimeKind.Local).AddTicks(5849) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "cd12b44b-103b-48df-8887-a2bf42e0651e",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2024, 6, 13, 10, 38, 28, 858, DateTimeKind.Local).AddTicks(5678), new DateTime(2024, 6, 13, 10, 38, 28, 858, DateTimeKind.Local).AddTicks(5707) });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "7e67d486-af3e-49f1-a109-a2b864b8e0ec",
                columns: new[] { "ConcurrencyStamp", "Created", "Modified", "PasswordHash", "SecurityStamp" },
                values: new object[] { "7cca4916-3313-4939-b1be-4d745c4607d5", new DateTime(2024, 6, 13, 10, 38, 28, 858, DateTimeKind.Local).AddTicks(6072), new DateTime(2024, 6, 13, 10, 38, 28, 858, DateTimeKind.Local).AddTicks(6078), "AQAAAAIAAYagAAAAEN8LBhvdpM8iS3wO2z1JdhPI0vz0Q0I+xT133zXLwcP1s2laE+qDrUU+fz6DsoYnWQ==", "90d62686-08bf-4733-9cfd-21c90cce9544" });
        }
    }
}
