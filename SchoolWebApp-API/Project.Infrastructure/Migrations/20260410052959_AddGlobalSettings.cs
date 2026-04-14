using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SchoolWebApp.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddGlobalSettings : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "GlobalSettings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Module = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    SettingKey = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    SettingValue = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Description = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Created = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    CreatedBy = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Modified = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    ModifiedBy = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GlobalSettings", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "269f0cf3-405e-4163-83f3-1b63ebebd62e",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 10, 8, 29, 57, 771, DateTimeKind.Local).AddTicks(9806), new DateTime(2026, 4, 10, 8, 29, 57, 771, DateTimeKind.Local).AddTicks(9808) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "448df289-142c-4959-a912-60733515e1b4",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 10, 8, 29, 57, 771, DateTimeKind.Local).AddTicks(9757), new DateTime(2026, 4, 10, 8, 29, 57, 771, DateTimeKind.Local).AddTicks(9759) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "48c50c3a-9958-453b-b649-4e21af131322",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 10, 8, 29, 57, 771, DateTimeKind.Local).AddTicks(9703), new DateTime(2026, 4, 10, 8, 29, 57, 771, DateTimeKind.Local).AddTicks(9705) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "717d9b15-a428-440c-b26b-08d3bbb68b02",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 10, 8, 29, 57, 771, DateTimeKind.Local).AddTicks(9535), new DateTime(2026, 4, 10, 8, 29, 57, 771, DateTimeKind.Local).AddTicks(9553) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "95ed2407-3e58-4af2-88a4-1c4e96473f68",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 10, 8, 29, 57, 771, DateTimeKind.Local).AddTicks(9645), new DateTime(2026, 4, 10, 8, 29, 57, 771, DateTimeKind.Local).AddTicks(9648) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "97942bee-ef12-4425-8225-4f293d0f36dd",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 10, 8, 29, 57, 771, DateTimeKind.Local).AddTicks(9910), new DateTime(2026, 4, 10, 8, 29, 57, 771, DateTimeKind.Local).AddTicks(9912) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "cd12b44b-103b-48df-8887-a2bf42e0651e",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 10, 8, 29, 57, 771, DateTimeKind.Local).AddTicks(9860), new DateTime(2026, 4, 10, 8, 29, 57, 771, DateTimeKind.Local).AddTicks(9862) });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "7e67d486-af3e-49f1-a109-a2b864b8e0ec",
                columns: new[] { "ConcurrencyStamp", "Created", "Modified", "PasswordHash", "SecurityStamp" },
                values: new object[] { "b304ba0b-a947-4073-9a34-0e5a5a281fe5", new DateTime(2026, 4, 10, 8, 29, 57, 772, DateTimeKind.Local).AddTicks(369), new DateTime(2026, 4, 10, 8, 29, 57, 772, DateTimeKind.Local).AddTicks(372), "AQAAAAIAAYagAAAAEKgyk0q9TmX+hPiJdBMujBUuVLnvkIG2JPUNvfapmEYMIwzrMoXuo4Cdja5zbo//Zg==", "fff66614-b00d-4d6e-bb4a-d1593dc3802d" });

            migrationBuilder.UpdateData(
                table: "Designations",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 10, 8, 29, 57, 772, DateTimeKind.Local).AddTicks(8), new DateTime(2026, 4, 10, 8, 29, 57, 772, DateTimeKind.Local).AddTicks(10) });

            migrationBuilder.UpdateData(
                table: "EmploymentTypes",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 10, 8, 29, 57, 772, DateTimeKind.Local).AddTicks(63), new DateTime(2026, 4, 10, 8, 29, 57, 772, DateTimeKind.Local).AddTicks(65) });

            migrationBuilder.UpdateData(
                table: "Genders",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 10, 8, 29, 57, 772, DateTimeKind.Local).AddTicks(188), new DateTime(2026, 4, 10, 8, 29, 57, 772, DateTimeKind.Local).AddTicks(190) });

            migrationBuilder.UpdateData(
                table: "Nationalities",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 10, 8, 29, 57, 772, DateTimeKind.Local).AddTicks(107), new DateTime(2026, 4, 10, 8, 29, 57, 772, DateTimeKind.Local).AddTicks(109) });

            migrationBuilder.UpdateData(
                table: "Person",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "DateOfBirth", "EmploymentDate", "EndofEmploymentDate", "Modified" },
                values: new object[] { new DateTime(2026, 4, 10, 8, 29, 57, 772, DateTimeKind.Local).AddTicks(263), new DateTime(2026, 4, 10, 8, 29, 57, 772, DateTimeKind.Local).AddTicks(260), new DateTime(2026, 4, 10, 8, 29, 57, 772, DateTimeKind.Local).AddTicks(254), new DateTime(2026, 4, 10, 8, 29, 57, 772, DateTimeKind.Local).AddTicks(256), new DateTime(2026, 4, 10, 8, 29, 57, 772, DateTimeKind.Local).AddTicks(267) });

            migrationBuilder.UpdateData(
                table: "Religions",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 10, 8, 29, 57, 772, DateTimeKind.Local).AddTicks(152), new DateTime(2026, 4, 10, 8, 29, 57, 772, DateTimeKind.Local).AddTicks(154) });

            migrationBuilder.UpdateData(
                table: "StaffCategories",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 10, 8, 29, 57, 771, DateTimeKind.Local).AddTicks(9966), new DateTime(2026, 4, 10, 8, 29, 57, 771, DateTimeKind.Local).AddTicks(9969) });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "GlobalSettings");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "269f0cf3-405e-4163-83f3-1b63ebebd62e",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 9, 17, 28, 58, 405, DateTimeKind.Local).AddTicks(6031), new DateTime(2026, 4, 9, 17, 28, 58, 405, DateTimeKind.Local).AddTicks(6034) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "448df289-142c-4959-a912-60733515e1b4",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 9, 17, 28, 58, 405, DateTimeKind.Local).AddTicks(5967), new DateTime(2026, 4, 9, 17, 28, 58, 405, DateTimeKind.Local).AddTicks(5970) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "48c50c3a-9958-453b-b649-4e21af131322",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 9, 17, 28, 58, 405, DateTimeKind.Local).AddTicks(5902), new DateTime(2026, 4, 9, 17, 28, 58, 405, DateTimeKind.Local).AddTicks(5905) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "717d9b15-a428-440c-b26b-08d3bbb68b02",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 9, 17, 28, 58, 405, DateTimeKind.Local).AddTicks(5665), new DateTime(2026, 4, 9, 17, 28, 58, 405, DateTimeKind.Local).AddTicks(5684) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "95ed2407-3e58-4af2-88a4-1c4e96473f68",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 9, 17, 28, 58, 405, DateTimeKind.Local).AddTicks(5829), new DateTime(2026, 4, 9, 17, 28, 58, 405, DateTimeKind.Local).AddTicks(5832) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "97942bee-ef12-4425-8225-4f293d0f36dd",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 9, 17, 28, 58, 405, DateTimeKind.Local).AddTicks(6172), new DateTime(2026, 4, 9, 17, 28, 58, 405, DateTimeKind.Local).AddTicks(6175) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "cd12b44b-103b-48df-8887-a2bf42e0651e",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 9, 17, 28, 58, 405, DateTimeKind.Local).AddTicks(6097), new DateTime(2026, 4, 9, 17, 28, 58, 405, DateTimeKind.Local).AddTicks(6100) });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "7e67d486-af3e-49f1-a109-a2b864b8e0ec",
                columns: new[] { "ConcurrencyStamp", "Created", "Modified", "PasswordHash", "SecurityStamp" },
                values: new object[] { "caed155b-d4c5-41eb-b592-1e1ff0df675f", new DateTime(2026, 4, 9, 17, 28, 58, 405, DateTimeKind.Local).AddTicks(6732), new DateTime(2026, 4, 9, 17, 28, 58, 405, DateTimeKind.Local).AddTicks(6735), "AQAAAAIAAYagAAAAELOeLeb+Pa+rDqTYTZ8NjxMS9S0j7Lcp1WSWpncqIiuSbGvp1wkDi86C9TTfVH0xdw==", "78495c16-7b17-4e71-94bf-0b508da88ec2" });

            migrationBuilder.UpdateData(
                table: "Designations",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 9, 17, 28, 58, 405, DateTimeKind.Local).AddTicks(6294), new DateTime(2026, 4, 9, 17, 28, 58, 405, DateTimeKind.Local).AddTicks(6297) });

            migrationBuilder.UpdateData(
                table: "EmploymentTypes",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 9, 17, 28, 58, 405, DateTimeKind.Local).AddTicks(6350), new DateTime(2026, 4, 9, 17, 28, 58, 405, DateTimeKind.Local).AddTicks(6353) });

            migrationBuilder.UpdateData(
                table: "Genders",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 9, 17, 28, 58, 405, DateTimeKind.Local).AddTicks(6517), new DateTime(2026, 4, 9, 17, 28, 58, 405, DateTimeKind.Local).AddTicks(6519) });

            migrationBuilder.UpdateData(
                table: "Nationalities",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 9, 17, 28, 58, 405, DateTimeKind.Local).AddTicks(6401), new DateTime(2026, 4, 9, 17, 28, 58, 405, DateTimeKind.Local).AddTicks(6404) });

            migrationBuilder.UpdateData(
                table: "Person",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "DateOfBirth", "EmploymentDate", "EndofEmploymentDate", "Modified" },
                values: new object[] { new DateTime(2026, 4, 9, 17, 28, 58, 405, DateTimeKind.Local).AddTicks(6603), new DateTime(2026, 4, 9, 17, 28, 58, 405, DateTimeKind.Local).AddTicks(6598), new DateTime(2026, 4, 9, 17, 28, 58, 405, DateTimeKind.Local).AddTicks(6592), new DateTime(2026, 4, 9, 17, 28, 58, 405, DateTimeKind.Local).AddTicks(6595), new DateTime(2026, 4, 9, 17, 28, 58, 405, DateTimeKind.Local).AddTicks(6610) });

            migrationBuilder.UpdateData(
                table: "Religions",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 9, 17, 28, 58, 405, DateTimeKind.Local).AddTicks(6469), new DateTime(2026, 4, 9, 17, 28, 58, 405, DateTimeKind.Local).AddTicks(6472) });

            migrationBuilder.UpdateData(
                table: "StaffCategories",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 9, 17, 28, 58, 405, DateTimeKind.Local).AddTicks(6247), new DateTime(2026, 4, 9, 17, 28, 58, 405, DateTimeKind.Local).AddTicks(6250) });
        }
    }
}
