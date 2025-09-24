using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SchoolWebApp.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddedRanktoompetencies : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Competencies",
                type: "varchar(255)",
                maxLength: 255,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "longtext")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Competencies",
                type: "varchar(500)",
                maxLength: 500,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "longtext",
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<int>(
                name: "Rank",
                table: "Competencies",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "269f0cf3-405e-4163-83f3-1b63ebebd62e",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2025, 9, 23, 16, 26, 53, 664, DateTimeKind.Local).AddTicks(1288), new DateTime(2025, 9, 23, 16, 26, 53, 664, DateTimeKind.Local).AddTicks(1289) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "448df289-142c-4959-a912-60733515e1b4",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2025, 9, 23, 16, 26, 53, 664, DateTimeKind.Local).AddTicks(1271), new DateTime(2025, 9, 23, 16, 26, 53, 664, DateTimeKind.Local).AddTicks(1272) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "48c50c3a-9958-453b-b649-4e21af131322",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2025, 9, 23, 16, 26, 53, 664, DateTimeKind.Local).AddTicks(1255), new DateTime(2025, 9, 23, 16, 26, 53, 664, DateTimeKind.Local).AddTicks(1256) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "717d9b15-a428-440c-b26b-08d3bbb68b02",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2025, 9, 23, 16, 26, 53, 664, DateTimeKind.Local).AddTicks(1186), new DateTime(2025, 9, 23, 16, 26, 53, 664, DateTimeKind.Local).AddTicks(1200) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "95ed2407-3e58-4af2-88a4-1c4e96473f68",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2025, 9, 23, 16, 26, 53, 664, DateTimeKind.Local).AddTicks(1237), new DateTime(2025, 9, 23, 16, 26, 53, 664, DateTimeKind.Local).AddTicks(1238) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "97942bee-ef12-4425-8225-4f293d0f36dd",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2025, 9, 23, 16, 26, 53, 664, DateTimeKind.Local).AddTicks(1319), new DateTime(2025, 9, 23, 16, 26, 53, 664, DateTimeKind.Local).AddTicks(1321) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "cd12b44b-103b-48df-8887-a2bf42e0651e",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2025, 9, 23, 16, 26, 53, 664, DateTimeKind.Local).AddTicks(1304), new DateTime(2025, 9, 23, 16, 26, 53, 664, DateTimeKind.Local).AddTicks(1305) });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "7e67d486-af3e-49f1-a109-a2b864b8e0ec",
                columns: new[] { "ConcurrencyStamp", "Created", "Modified", "PasswordHash", "SecurityStamp" },
                values: new object[] { "803d73f8-2a32-4056-b941-4aa529f1a31c", new DateTime(2025, 9, 23, 16, 26, 53, 664, DateTimeKind.Local).AddTicks(1535), new DateTime(2025, 9, 23, 16, 26, 53, 664, DateTimeKind.Local).AddTicks(1537), "AQAAAAIAAYagAAAAENEQJ2rne1JbA45U6qIiO8hqxBCHYX6VcijafWPd81gqZbHJabsqYm0XMEpqb4E4rA==", "30a3aae7-61f4-4be2-baa8-6ce35d64a743" });

            migrationBuilder.UpdateData(
                table: "Designations",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2025, 9, 23, 16, 26, 53, 664, DateTimeKind.Local).AddTicks(1363), new DateTime(2025, 9, 23, 16, 26, 53, 664, DateTimeKind.Local).AddTicks(1364) });

            migrationBuilder.UpdateData(
                table: "EmploymentTypes",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2025, 9, 23, 16, 26, 53, 664, DateTimeKind.Local).AddTicks(1382), new DateTime(2025, 9, 23, 16, 26, 53, 664, DateTimeKind.Local).AddTicks(1383) });

            migrationBuilder.UpdateData(
                table: "Genders",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2025, 9, 23, 16, 26, 53, 664, DateTimeKind.Local).AddTicks(1434), new DateTime(2025, 9, 23, 16, 26, 53, 664, DateTimeKind.Local).AddTicks(1434) });

            migrationBuilder.UpdateData(
                table: "Nationalities",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2025, 9, 23, 16, 26, 53, 664, DateTimeKind.Local).AddTicks(1405), new DateTime(2025, 9, 23, 16, 26, 53, 664, DateTimeKind.Local).AddTicks(1406) });

            migrationBuilder.UpdateData(
                table: "Person",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "DateOfBirth", "EmploymentDate", "EndofEmploymentDate", "Modified" },
                values: new object[] { new DateTime(2025, 9, 23, 16, 26, 53, 664, DateTimeKind.Local).AddTicks(1464), new DateTime(2025, 9, 23, 16, 26, 53, 664, DateTimeKind.Local).AddTicks(1462), new DateTime(2025, 9, 23, 16, 26, 53, 664, DateTimeKind.Local).AddTicks(1457), new DateTime(2025, 9, 23, 16, 26, 53, 664, DateTimeKind.Local).AddTicks(1458), new DateTime(2025, 9, 23, 16, 26, 53, 664, DateTimeKind.Local).AddTicks(1465) });

            migrationBuilder.UpdateData(
                table: "Religions",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2025, 9, 23, 16, 26, 53, 664, DateTimeKind.Local).AddTicks(1419), new DateTime(2025, 9, 23, 16, 26, 53, 664, DateTimeKind.Local).AddTicks(1420) });

            migrationBuilder.UpdateData(
                table: "StaffCategories",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2025, 9, 23, 16, 26, 53, 664, DateTimeKind.Local).AddTicks(1344), new DateTime(2025, 9, 23, 16, 26, 53, 664, DateTimeKind.Local).AddTicks(1346) });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Rank",
                table: "Competencies");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Competencies",
                type: "longtext",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(255)",
                oldMaxLength: 255)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Competencies",
                type: "longtext",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(500)",
                oldMaxLength: 500,
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "269f0cf3-405e-4163-83f3-1b63ebebd62e",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2025, 9, 22, 7, 8, 37, 322, DateTimeKind.Local).AddTicks(8202), new DateTime(2025, 9, 22, 7, 8, 37, 322, DateTimeKind.Local).AddTicks(8203) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "448df289-142c-4959-a912-60733515e1b4",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2025, 9, 22, 7, 8, 37, 322, DateTimeKind.Local).AddTicks(8179), new DateTime(2025, 9, 22, 7, 8, 37, 322, DateTimeKind.Local).AddTicks(8180) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "48c50c3a-9958-453b-b649-4e21af131322",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2025, 9, 22, 7, 8, 37, 322, DateTimeKind.Local).AddTicks(8146), new DateTime(2025, 9, 22, 7, 8, 37, 322, DateTimeKind.Local).AddTicks(8148) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "717d9b15-a428-440c-b26b-08d3bbb68b02",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2025, 9, 22, 7, 8, 37, 322, DateTimeKind.Local).AddTicks(8048), new DateTime(2025, 9, 22, 7, 8, 37, 322, DateTimeKind.Local).AddTicks(8069) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "95ed2407-3e58-4af2-88a4-1c4e96473f68",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2025, 9, 22, 7, 8, 37, 322, DateTimeKind.Local).AddTicks(8118), new DateTime(2025, 9, 22, 7, 8, 37, 322, DateTimeKind.Local).AddTicks(8120) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "97942bee-ef12-4425-8225-4f293d0f36dd",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2025, 9, 22, 7, 8, 37, 322, DateTimeKind.Local).AddTicks(8256), new DateTime(2025, 9, 22, 7, 8, 37, 322, DateTimeKind.Local).AddTicks(8258) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "cd12b44b-103b-48df-8887-a2bf42e0651e",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2025, 9, 22, 7, 8, 37, 322, DateTimeKind.Local).AddTicks(8226), new DateTime(2025, 9, 22, 7, 8, 37, 322, DateTimeKind.Local).AddTicks(8228) });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "7e67d486-af3e-49f1-a109-a2b864b8e0ec",
                columns: new[] { "ConcurrencyStamp", "Created", "Modified", "PasswordHash", "SecurityStamp" },
                values: new object[] { "224415ca-f5c2-4096-8e91-045a751c46f3", new DateTime(2025, 9, 22, 7, 8, 37, 322, DateTimeKind.Local).AddTicks(8538), new DateTime(2025, 9, 22, 7, 8, 37, 322, DateTimeKind.Local).AddTicks(8540), "AQAAAAIAAYagAAAAEKTocC7ziVXHYzcW67d5NXfEohoKjj+pl3qDd3XLojIdDq88+Gi0fEFJsl5PBQZiIA==", "474fcf4c-f10e-4ee4-bbf5-a4753e5f916e" });

            migrationBuilder.UpdateData(
                table: "Designations",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2025, 9, 22, 7, 8, 37, 322, DateTimeKind.Local).AddTicks(8325), new DateTime(2025, 9, 22, 7, 8, 37, 322, DateTimeKind.Local).AddTicks(8326) });

            migrationBuilder.UpdateData(
                table: "EmploymentTypes",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2025, 9, 22, 7, 8, 37, 322, DateTimeKind.Local).AddTicks(8355), new DateTime(2025, 9, 22, 7, 8, 37, 322, DateTimeKind.Local).AddTicks(8355) });

            migrationBuilder.UpdateData(
                table: "Genders",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2025, 9, 22, 7, 8, 37, 322, DateTimeKind.Local).AddTicks(8420), new DateTime(2025, 9, 22, 7, 8, 37, 322, DateTimeKind.Local).AddTicks(8421) });

            migrationBuilder.UpdateData(
                table: "Nationalities",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2025, 9, 22, 7, 8, 37, 322, DateTimeKind.Local).AddTicks(8381), new DateTime(2025, 9, 22, 7, 8, 37, 322, DateTimeKind.Local).AddTicks(8381) });

            migrationBuilder.UpdateData(
                table: "Person",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "DateOfBirth", "EmploymentDate", "EndofEmploymentDate", "Modified" },
                values: new object[] { new DateTime(2025, 9, 22, 7, 8, 37, 322, DateTimeKind.Local).AddTicks(8459), new DateTime(2025, 9, 22, 7, 8, 37, 322, DateTimeKind.Local).AddTicks(8456), new DateTime(2025, 9, 22, 7, 8, 37, 322, DateTimeKind.Local).AddTicks(8451), new DateTime(2025, 9, 22, 7, 8, 37, 322, DateTimeKind.Local).AddTicks(8452), new DateTime(2025, 9, 22, 7, 8, 37, 322, DateTimeKind.Local).AddTicks(8459) });

            migrationBuilder.UpdateData(
                table: "Religions",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2025, 9, 22, 7, 8, 37, 322, DateTimeKind.Local).AddTicks(8399), new DateTime(2025, 9, 22, 7, 8, 37, 322, DateTimeKind.Local).AddTicks(8400) });

            migrationBuilder.UpdateData(
                table: "StaffCategories",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2025, 9, 22, 7, 8, 37, 322, DateTimeKind.Local).AddTicks(8294), new DateTime(2025, 9, 22, 7, 8, 37, 322, DateTimeKind.Local).AddTicks(8296) });
        }
    }
}
