using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SchoolWebApp.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddMenuPermissions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "MenuPermissions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    RoleId = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    MenuPath = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    MenuName = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true)
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
                    table.PrimaryKey("PK_MenuPermissions", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "269f0cf3-405e-4163-83f3-1b63ebebd62e",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 11, 20, 37, 19, 146, DateTimeKind.Local).AddTicks(8930), new DateTime(2026, 4, 11, 20, 37, 19, 146, DateTimeKind.Local).AddTicks(8931) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "448df289-142c-4959-a912-60733515e1b4",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 11, 20, 37, 19, 146, DateTimeKind.Local).AddTicks(8907), new DateTime(2026, 4, 11, 20, 37, 19, 146, DateTimeKind.Local).AddTicks(8908) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "48c50c3a-9958-453b-b649-4e21af131322",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 11, 20, 37, 19, 146, DateTimeKind.Local).AddTicks(8883), new DateTime(2026, 4, 11, 20, 37, 19, 146, DateTimeKind.Local).AddTicks(8884) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "717d9b15-a428-440c-b26b-08d3bbb68b02",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 11, 20, 37, 19, 146, DateTimeKind.Local).AddTicks(8778), new DateTime(2026, 4, 11, 20, 37, 19, 146, DateTimeKind.Local).AddTicks(8792) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "95ed2407-3e58-4af2-88a4-1c4e96473f68",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 11, 20, 37, 19, 146, DateTimeKind.Local).AddTicks(8857), new DateTime(2026, 4, 11, 20, 37, 19, 146, DateTimeKind.Local).AddTicks(8858) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "97942bee-ef12-4425-8225-4f293d0f36dd",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 11, 20, 37, 19, 146, DateTimeKind.Local).AddTicks(8977), new DateTime(2026, 4, 11, 20, 37, 19, 146, DateTimeKind.Local).AddTicks(8978) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "cd12b44b-103b-48df-8887-a2bf42e0651e",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 11, 20, 37, 19, 146, DateTimeKind.Local).AddTicks(8954), new DateTime(2026, 4, 11, 20, 37, 19, 146, DateTimeKind.Local).AddTicks(8955) });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "7e67d486-af3e-49f1-a109-a2b864b8e0ec",
                columns: new[] { "ConcurrencyStamp", "Created", "Modified", "PasswordHash", "SecurityStamp" },
                values: new object[] { "a85145b9-4cca-4da8-b693-7e0a71f47dfc", new DateTime(2026, 4, 11, 20, 37, 19, 146, DateTimeKind.Local).AddTicks(9265), new DateTime(2026, 4, 11, 20, 37, 19, 146, DateTimeKind.Local).AddTicks(9267), "AQAAAAIAAYagAAAAEAPGjSP8ke/efQpILYxLS1RLajiJS0CtJ4gPwqaNtsWSnEobKfwZQIeJjFxr3s4uUw==", "8aa7020e-2e8f-4dfc-9dbf-9074ce6ae640" });

            migrationBuilder.UpdateData(
                table: "Designations",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 11, 20, 37, 19, 146, DateTimeKind.Local).AddTicks(9037), new DateTime(2026, 4, 11, 20, 37, 19, 146, DateTimeKind.Local).AddTicks(9038) });

            migrationBuilder.UpdateData(
                table: "EmploymentTypes",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 11, 20, 37, 19, 146, DateTimeKind.Local).AddTicks(9064), new DateTime(2026, 4, 11, 20, 37, 19, 146, DateTimeKind.Local).AddTicks(9065) });

            migrationBuilder.UpdateData(
                table: "Genders",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 11, 20, 37, 19, 146, DateTimeKind.Local).AddTicks(9129), new DateTime(2026, 4, 11, 20, 37, 19, 146, DateTimeKind.Local).AddTicks(9130) });

            migrationBuilder.UpdateData(
                table: "Nationalities",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 11, 20, 37, 19, 146, DateTimeKind.Local).AddTicks(9084), new DateTime(2026, 4, 11, 20, 37, 19, 146, DateTimeKind.Local).AddTicks(9085) });

            migrationBuilder.UpdateData(
                table: "Person",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "DateOfBirth", "EmploymentDate", "EndofEmploymentDate", "Modified" },
                values: new object[] { new DateTime(2026, 4, 11, 20, 37, 19, 146, DateTimeKind.Local).AddTicks(9171), new DateTime(2026, 4, 11, 20, 37, 19, 146, DateTimeKind.Local).AddTicks(9168), new DateTime(2026, 4, 11, 20, 37, 19, 146, DateTimeKind.Local).AddTicks(9164), new DateTime(2026, 4, 11, 20, 37, 19, 146, DateTimeKind.Local).AddTicks(9165), new DateTime(2026, 4, 11, 20, 37, 19, 146, DateTimeKind.Local).AddTicks(9184) });

            migrationBuilder.UpdateData(
                table: "Religions",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 11, 20, 37, 19, 146, DateTimeKind.Local).AddTicks(9109), new DateTime(2026, 4, 11, 20, 37, 19, 146, DateTimeKind.Local).AddTicks(9110) });

            migrationBuilder.UpdateData(
                table: "StaffCategories",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 11, 20, 37, 19, 146, DateTimeKind.Local).AddTicks(9014), new DateTime(2026, 4, 11, 20, 37, 19, 146, DateTimeKind.Local).AddTicks(9016) });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MenuPermissions");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "269f0cf3-405e-4163-83f3-1b63ebebd62e",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 11, 18, 31, 22, 314, DateTimeKind.Local).AddTicks(632), new DateTime(2026, 4, 11, 18, 31, 22, 314, DateTimeKind.Local).AddTicks(636) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "448df289-142c-4959-a912-60733515e1b4",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 11, 18, 31, 22, 314, DateTimeKind.Local).AddTicks(535), new DateTime(2026, 4, 11, 18, 31, 22, 314, DateTimeKind.Local).AddTicks(538) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "48c50c3a-9958-453b-b649-4e21af131322",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 11, 18, 31, 22, 314, DateTimeKind.Local).AddTicks(458), new DateTime(2026, 4, 11, 18, 31, 22, 314, DateTimeKind.Local).AddTicks(461) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "717d9b15-a428-440c-b26b-08d3bbb68b02",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 11, 18, 31, 22, 314, DateTimeKind.Local).AddTicks(52), new DateTime(2026, 4, 11, 18, 31, 22, 314, DateTimeKind.Local).AddTicks(94) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "95ed2407-3e58-4af2-88a4-1c4e96473f68",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 11, 18, 31, 22, 314, DateTimeKind.Local).AddTicks(365), new DateTime(2026, 4, 11, 18, 31, 22, 314, DateTimeKind.Local).AddTicks(369) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "97942bee-ef12-4425-8225-4f293d0f36dd",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 11, 18, 31, 22, 314, DateTimeKind.Local).AddTicks(794), new DateTime(2026, 4, 11, 18, 31, 22, 314, DateTimeKind.Local).AddTicks(797) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "cd12b44b-103b-48df-8887-a2bf42e0651e",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 11, 18, 31, 22, 314, DateTimeKind.Local).AddTicks(717), new DateTime(2026, 4, 11, 18, 31, 22, 314, DateTimeKind.Local).AddTicks(720) });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "7e67d486-af3e-49f1-a109-a2b864b8e0ec",
                columns: new[] { "ConcurrencyStamp", "Created", "Modified", "PasswordHash", "SecurityStamp" },
                values: new object[] { "69fbf80f-f2ee-412d-ad08-26458608427a", new DateTime(2026, 4, 11, 18, 31, 22, 314, DateTimeKind.Local).AddTicks(1808), new DateTime(2026, 4, 11, 18, 31, 22, 314, DateTimeKind.Local).AddTicks(1812), "AQAAAAIAAYagAAAAEMzuwv9HT0/KoB6zCdZXZfv2EGZJX03k8ifYH2MwGsH7NjRoI3cTrZSHY4Bjt8t/Gg==", "a312ef20-770e-4030-ab59-489a164998d4" });

            migrationBuilder.UpdateData(
                table: "Designations",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 11, 18, 31, 22, 314, DateTimeKind.Local).AddTicks(977), new DateTime(2026, 4, 11, 18, 31, 22, 314, DateTimeKind.Local).AddTicks(980) });

            migrationBuilder.UpdateData(
                table: "EmploymentTypes",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 11, 18, 31, 22, 314, DateTimeKind.Local).AddTicks(1062), new DateTime(2026, 4, 11, 18, 31, 22, 314, DateTimeKind.Local).AddTicks(1065) });

            migrationBuilder.UpdateData(
                table: "Genders",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 11, 18, 31, 22, 314, DateTimeKind.Local).AddTicks(1337), new DateTime(2026, 4, 11, 18, 31, 22, 314, DateTimeKind.Local).AddTicks(1340) });

            migrationBuilder.UpdateData(
                table: "Nationalities",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 11, 18, 31, 22, 314, DateTimeKind.Local).AddTicks(1172), new DateTime(2026, 4, 11, 18, 31, 22, 314, DateTimeKind.Local).AddTicks(1176) });

            migrationBuilder.UpdateData(
                table: "Person",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "DateOfBirth", "EmploymentDate", "EndofEmploymentDate", "Modified" },
                values: new object[] { new DateTime(2026, 4, 11, 18, 31, 22, 314, DateTimeKind.Local).AddTicks(1533), new DateTime(2026, 4, 11, 18, 31, 22, 314, DateTimeKind.Local).AddTicks(1529), new DateTime(2026, 4, 11, 18, 31, 22, 314, DateTimeKind.Local).AddTicks(1517), new DateTime(2026, 4, 11, 18, 31, 22, 314, DateTimeKind.Local).AddTicks(1526), new DateTime(2026, 4, 11, 18, 31, 22, 314, DateTimeKind.Local).AddTicks(1550) });

            migrationBuilder.UpdateData(
                table: "Religions",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 11, 18, 31, 22, 314, DateTimeKind.Local).AddTicks(1274), new DateTime(2026, 4, 11, 18, 31, 22, 314, DateTimeKind.Local).AddTicks(1277) });

            migrationBuilder.UpdateData(
                table: "StaffCategories",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 11, 18, 31, 22, 314, DateTimeKind.Local).AddTicks(903), new DateTime(2026, 4, 11, 18, 31, 22, 314, DateTimeKind.Local).AddTicks(908) });
        }
    }
}
