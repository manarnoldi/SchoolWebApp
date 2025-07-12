using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SchoolWebApp.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddedRanktoLeanringModes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Rank",
                table: "LearningModes",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "269f0cf3-405e-4163-83f3-1b63ebebd62e",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2025, 7, 11, 6, 59, 6, 427, DateTimeKind.Local).AddTicks(2277), new DateTime(2025, 7, 11, 6, 59, 6, 427, DateTimeKind.Local).AddTicks(2280) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "448df289-142c-4959-a912-60733515e1b4",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2025, 7, 11, 6, 59, 6, 427, DateTimeKind.Local).AddTicks(2212), new DateTime(2025, 7, 11, 6, 59, 6, 427, DateTimeKind.Local).AddTicks(2214) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "48c50c3a-9958-453b-b649-4e21af131322",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2025, 7, 11, 6, 59, 6, 427, DateTimeKind.Local).AddTicks(2147), new DateTime(2025, 7, 11, 6, 59, 6, 427, DateTimeKind.Local).AddTicks(2149) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "717d9b15-a428-440c-b26b-08d3bbb68b02",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2025, 7, 11, 6, 59, 6, 427, DateTimeKind.Local).AddTicks(1884), new DateTime(2025, 7, 11, 6, 59, 6, 427, DateTimeKind.Local).AddTicks(1924) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "95ed2407-3e58-4af2-88a4-1c4e96473f68",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2025, 7, 11, 6, 59, 6, 427, DateTimeKind.Local).AddTicks(2050), new DateTime(2025, 7, 11, 6, 59, 6, 427, DateTimeKind.Local).AddTicks(2054) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "97942bee-ef12-4425-8225-4f293d0f36dd",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2025, 7, 11, 6, 59, 6, 427, DateTimeKind.Local).AddTicks(2413), new DateTime(2025, 7, 11, 6, 59, 6, 427, DateTimeKind.Local).AddTicks(2415) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "cd12b44b-103b-48df-8887-a2bf42e0651e",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2025, 7, 11, 6, 59, 6, 427, DateTimeKind.Local).AddTicks(2349), new DateTime(2025, 7, 11, 6, 59, 6, 427, DateTimeKind.Local).AddTicks(2352) });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "7e67d486-af3e-49f1-a109-a2b864b8e0ec",
                columns: new[] { "ConcurrencyStamp", "Created", "Modified", "PasswordHash", "SecurityStamp" },
                values: new object[] { "5063d639-4e92-448b-9a7b-000785dbe4f8", new DateTime(2025, 7, 11, 6, 59, 6, 427, DateTimeKind.Local).AddTicks(3161), new DateTime(2025, 7, 11, 6, 59, 6, 427, DateTimeKind.Local).AddTicks(3165), "AQAAAAIAAYagAAAAEI+F4M/qI1kRRucf1GjEJJzY7/YUAuvVDiJUpRTOygBVz95f7k2OKCB1h5ZC8odcDg==", "8e916ebf-f00a-41fa-9939-82be98b6e67a" });

            migrationBuilder.UpdateData(
                table: "Designations",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2025, 7, 11, 6, 59, 6, 427, DateTimeKind.Local).AddTicks(2585), new DateTime(2025, 7, 11, 6, 59, 6, 427, DateTimeKind.Local).AddTicks(2588) });

            migrationBuilder.UpdateData(
                table: "EmploymentTypes",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2025, 7, 11, 6, 59, 6, 427, DateTimeKind.Local).AddTicks(2651), new DateTime(2025, 7, 11, 6, 59, 6, 427, DateTimeKind.Local).AddTicks(2655) });

            migrationBuilder.UpdateData(
                table: "Genders",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2025, 7, 11, 6, 59, 6, 427, DateTimeKind.Local).AddTicks(2881), new DateTime(2025, 7, 11, 6, 59, 6, 427, DateTimeKind.Local).AddTicks(2884) });

            migrationBuilder.UpdateData(
                table: "Nationalities",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2025, 7, 11, 6, 59, 6, 427, DateTimeKind.Local).AddTicks(2722), new DateTime(2025, 7, 11, 6, 59, 6, 427, DateTimeKind.Local).AddTicks(2724) });

            migrationBuilder.UpdateData(
                table: "Person",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "DateOfBirth", "EmploymentDate", "EndofEmploymentDate", "Modified" },
                values: new object[] { new DateTime(2025, 7, 11, 6, 59, 6, 427, DateTimeKind.Local).AddTicks(2991), new DateTime(2025, 7, 11, 6, 59, 6, 427, DateTimeKind.Local).AddTicks(2980), new DateTime(2025, 7, 11, 6, 59, 6, 427, DateTimeKind.Local).AddTicks(2969), new DateTime(2025, 7, 11, 6, 59, 6, 427, DateTimeKind.Local).AddTicks(2972), new DateTime(2025, 7, 11, 6, 59, 6, 427, DateTimeKind.Local).AddTicks(3004) });

            migrationBuilder.UpdateData(
                table: "Religions",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2025, 7, 11, 6, 59, 6, 427, DateTimeKind.Local).AddTicks(2806), new DateTime(2025, 7, 11, 6, 59, 6, 427, DateTimeKind.Local).AddTicks(2808) });

            migrationBuilder.UpdateData(
                table: "StaffCategories",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2025, 7, 11, 6, 59, 6, 427, DateTimeKind.Local).AddTicks(2512), new DateTime(2025, 7, 11, 6, 59, 6, 427, DateTimeKind.Local).AddTicks(2519) });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Rank",
                table: "LearningModes");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "269f0cf3-405e-4163-83f3-1b63ebebd62e",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2025, 7, 11, 5, 37, 36, 287, DateTimeKind.Local).AddTicks(1328), new DateTime(2025, 7, 11, 5, 37, 36, 287, DateTimeKind.Local).AddTicks(1329) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "448df289-142c-4959-a912-60733515e1b4",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2025, 7, 11, 5, 37, 36, 287, DateTimeKind.Local).AddTicks(1303), new DateTime(2025, 7, 11, 5, 37, 36, 287, DateTimeKind.Local).AddTicks(1305) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "48c50c3a-9958-453b-b649-4e21af131322",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2025, 7, 11, 5, 37, 36, 287, DateTimeKind.Local).AddTicks(1278), new DateTime(2025, 7, 11, 5, 37, 36, 287, DateTimeKind.Local).AddTicks(1280) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "717d9b15-a428-440c-b26b-08d3bbb68b02",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2025, 7, 11, 5, 37, 36, 287, DateTimeKind.Local).AddTicks(1163), new DateTime(2025, 7, 11, 5, 37, 36, 287, DateTimeKind.Local).AddTicks(1194) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "95ed2407-3e58-4af2-88a4-1c4e96473f68",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2025, 7, 11, 5, 37, 36, 287, DateTimeKind.Local).AddTicks(1252), new DateTime(2025, 7, 11, 5, 37, 36, 287, DateTimeKind.Local).AddTicks(1253) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "97942bee-ef12-4425-8225-4f293d0f36dd",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2025, 7, 11, 5, 37, 36, 287, DateTimeKind.Local).AddTicks(1388), new DateTime(2025, 7, 11, 5, 37, 36, 287, DateTimeKind.Local).AddTicks(1389) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "cd12b44b-103b-48df-8887-a2bf42e0651e",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2025, 7, 11, 5, 37, 36, 287, DateTimeKind.Local).AddTicks(1362), new DateTime(2025, 7, 11, 5, 37, 36, 287, DateTimeKind.Local).AddTicks(1364) });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "7e67d486-af3e-49f1-a109-a2b864b8e0ec",
                columns: new[] { "ConcurrencyStamp", "Created", "Modified", "PasswordHash", "SecurityStamp" },
                values: new object[] { "04961cbc-0f9e-4b02-9526-3949c3a455ef", new DateTime(2025, 7, 11, 5, 37, 36, 287, DateTimeKind.Local).AddTicks(1796), new DateTime(2025, 7, 11, 5, 37, 36, 287, DateTimeKind.Local).AddTicks(1799), "AQAAAAIAAYagAAAAEMw47RgNQZLTaA6zNazGz5cBdlmp7bud1BgsdDxgeuQhRQHmZ7Xr0KFnFL64KQhNgQ==", "12f91409-4782-4288-a3fb-d960a506bf03" });

            migrationBuilder.UpdateData(
                table: "Designations",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2025, 7, 11, 5, 37, 36, 287, DateTimeKind.Local).AddTicks(1473), new DateTime(2025, 7, 11, 5, 37, 36, 287, DateTimeKind.Local).AddTicks(1474) });

            migrationBuilder.UpdateData(
                table: "EmploymentTypes",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2025, 7, 11, 5, 37, 36, 287, DateTimeKind.Local).AddTicks(1509), new DateTime(2025, 7, 11, 5, 37, 36, 287, DateTimeKind.Local).AddTicks(1510) });

            migrationBuilder.UpdateData(
                table: "Genders",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2025, 7, 11, 5, 37, 36, 287, DateTimeKind.Local).AddTicks(1607), new DateTime(2025, 7, 11, 5, 37, 36, 287, DateTimeKind.Local).AddTicks(1608) });

            migrationBuilder.UpdateData(
                table: "Nationalities",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2025, 7, 11, 5, 37, 36, 287, DateTimeKind.Local).AddTicks(1549), new DateTime(2025, 7, 11, 5, 37, 36, 287, DateTimeKind.Local).AddTicks(1550) });

            migrationBuilder.UpdateData(
                table: "Person",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "DateOfBirth", "EmploymentDate", "EndofEmploymentDate", "Modified" },
                values: new object[] { new DateTime(2025, 7, 11, 5, 37, 36, 287, DateTimeKind.Local).AddTicks(1684), new DateTime(2025, 7, 11, 5, 37, 36, 287, DateTimeKind.Local).AddTicks(1670), new DateTime(2025, 7, 11, 5, 37, 36, 287, DateTimeKind.Local).AddTicks(1666), new DateTime(2025, 7, 11, 5, 37, 36, 287, DateTimeKind.Local).AddTicks(1667), new DateTime(2025, 7, 11, 5, 37, 36, 287, DateTimeKind.Local).AddTicks(1689) });

            migrationBuilder.UpdateData(
                table: "Religions",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2025, 7, 11, 5, 37, 36, 287, DateTimeKind.Local).AddTicks(1583), new DateTime(2025, 7, 11, 5, 37, 36, 287, DateTimeKind.Local).AddTicks(1584) });

            migrationBuilder.UpdateData(
                table: "StaffCategories",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2025, 7, 11, 5, 37, 36, 287, DateTimeKind.Local).AddTicks(1436), new DateTime(2025, 7, 11, 5, 37, 36, 287, DateTimeKind.Local).AddTicks(1439) });
        }
    }
}
