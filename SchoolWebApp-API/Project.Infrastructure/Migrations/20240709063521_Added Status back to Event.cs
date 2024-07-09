using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SchoolWebApp.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddedStatusbacktoEvent : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EventYear",
                table: "Events");

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Events",
                type: "varchar(500)",
                maxLength: 500,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(500)",
                oldMaxLength: 500)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<bool>(
                name: "Status",
                table: "Events",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "269f0cf3-405e-4163-83f3-1b63ebebd62e",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2024, 7, 9, 9, 35, 20, 193, DateTimeKind.Local).AddTicks(3255), new DateTime(2024, 7, 9, 9, 35, 20, 193, DateTimeKind.Local).AddTicks(3257) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "448df289-142c-4959-a912-60733515e1b4",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2024, 7, 9, 9, 35, 20, 193, DateTimeKind.Local).AddTicks(3224), new DateTime(2024, 7, 9, 9, 35, 20, 193, DateTimeKind.Local).AddTicks(3226) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "48c50c3a-9958-453b-b649-4e21af131322",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2024, 7, 9, 9, 35, 20, 193, DateTimeKind.Local).AddTicks(3192), new DateTime(2024, 7, 9, 9, 35, 20, 193, DateTimeKind.Local).AddTicks(3194) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "717d9b15-a428-440c-b26b-08d3bbb68b02",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2024, 7, 9, 9, 35, 20, 193, DateTimeKind.Local).AddTicks(3048), new DateTime(2024, 7, 9, 9, 35, 20, 193, DateTimeKind.Local).AddTicks(3079) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "95ed2407-3e58-4af2-88a4-1c4e96473f68",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2024, 7, 9, 9, 35, 20, 193, DateTimeKind.Local).AddTicks(3146), new DateTime(2024, 7, 9, 9, 35, 20, 193, DateTimeKind.Local).AddTicks(3158) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "97942bee-ef12-4425-8225-4f293d0f36dd",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2024, 7, 9, 9, 35, 20, 193, DateTimeKind.Local).AddTicks(3367), new DateTime(2024, 7, 9, 9, 35, 20, 193, DateTimeKind.Local).AddTicks(3369) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "cd12b44b-103b-48df-8887-a2bf42e0651e",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2024, 7, 9, 9, 35, 20, 193, DateTimeKind.Local).AddTicks(3310), new DateTime(2024, 7, 9, 9, 35, 20, 193, DateTimeKind.Local).AddTicks(3321) });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "7e67d486-af3e-49f1-a109-a2b864b8e0ec",
                columns: new[] { "ConcurrencyStamp", "Created", "Modified", "PasswordHash", "SecurityStamp" },
                values: new object[] { "a3319857-b7dc-43da-a2d3-76ef714d19ab", new DateTime(2024, 7, 9, 9, 35, 20, 193, DateTimeKind.Local).AddTicks(3478), new DateTime(2024, 7, 9, 9, 35, 20, 193, DateTimeKind.Local).AddTicks(3480), "AQAAAAIAAYagAAAAEALhl+8ZJ0FWomZ5gq8f2A84lT61G2B8KzV1N4ztl85ZoSZbanHghbplrQhDwPCLeQ==", "637134d1-f401-4987-a6a9-f51146a39941" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Status",
                table: "Events");

            migrationBuilder.UpdateData(
                table: "Events",
                keyColumn: "Description",
                keyValue: null,
                column: "Description",
                value: "");

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Events",
                type: "varchar(500)",
                maxLength: 500,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(500)",
                oldMaxLength: 500,
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<int>(
                name: "EventYear",
                table: "Events",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "269f0cf3-405e-4163-83f3-1b63ebebd62e",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2024, 7, 9, 7, 34, 26, 923, DateTimeKind.Local).AddTicks(1141), new DateTime(2024, 7, 9, 7, 34, 26, 923, DateTimeKind.Local).AddTicks(1155) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "448df289-142c-4959-a912-60733515e1b4",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2024, 7, 9, 7, 34, 26, 923, DateTimeKind.Local).AddTicks(1116), new DateTime(2024, 7, 9, 7, 34, 26, 923, DateTimeKind.Local).AddTicks(1117) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "48c50c3a-9958-453b-b649-4e21af131322",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2024, 7, 9, 7, 34, 26, 923, DateTimeKind.Local).AddTicks(1091), new DateTime(2024, 7, 9, 7, 34, 26, 923, DateTimeKind.Local).AddTicks(1093) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "717d9b15-a428-440c-b26b-08d3bbb68b02",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2024, 7, 9, 7, 34, 26, 923, DateTimeKind.Local).AddTicks(981), new DateTime(2024, 7, 9, 7, 34, 26, 923, DateTimeKind.Local).AddTicks(1016) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "95ed2407-3e58-4af2-88a4-1c4e96473f68",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2024, 7, 9, 7, 34, 26, 923, DateTimeKind.Local).AddTicks(1065), new DateTime(2024, 7, 9, 7, 34, 26, 923, DateTimeKind.Local).AddTicks(1067) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "97942bee-ef12-4425-8225-4f293d0f36dd",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2024, 7, 9, 7, 34, 26, 923, DateTimeKind.Local).AddTicks(1242), new DateTime(2024, 7, 9, 7, 34, 26, 923, DateTimeKind.Local).AddTicks(1243) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "cd12b44b-103b-48df-8887-a2bf42e0651e",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2024, 7, 9, 7, 34, 26, 923, DateTimeKind.Local).AddTicks(1202), new DateTime(2024, 7, 9, 7, 34, 26, 923, DateTimeKind.Local).AddTicks(1204) });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "7e67d486-af3e-49f1-a109-a2b864b8e0ec",
                columns: new[] { "ConcurrencyStamp", "Created", "Modified", "PasswordHash", "SecurityStamp" },
                values: new object[] { "2f315b8a-d561-4abb-bc9e-fc573d65cb49", new DateTime(2024, 7, 9, 7, 34, 26, 923, DateTimeKind.Local).AddTicks(1343), new DateTime(2024, 7, 9, 7, 34, 26, 923, DateTimeKind.Local).AddTicks(1345), "AQAAAAIAAYagAAAAEEmDkkkm+9vkAHNvuqvsZ+YG4JGoGsHIdZ9lRcsCJ4vH3VCbTsUfKxfB2kDRhhe4wg==", "ec0339d6-478d-4d1d-a67e-8d43f42ae10b" });
        }
    }
}
