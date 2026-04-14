using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SchoolWebApp.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RemoveSocialSkillTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Responsibilities_SocialSkills_SocialSkillId",
                table: "Responsibilities");

            migrationBuilder.DropTable(
                name: "SocialSkills");

            migrationBuilder.DropIndex(
                name: "IX_Responsibilities_SocialSkillId",
                table: "Responsibilities");

            migrationBuilder.DropColumn(
                name: "SocialSkillId",
                table: "Responsibilities");

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "SocialSkillId",
                table: "Responsibilities",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "SocialSkills",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Created = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    CreatedBy = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Description = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Modified = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    ModifiedBy = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Name = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Rank = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SocialSkills", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "269f0cf3-405e-4163-83f3-1b63ebebd62e",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 11, 17, 57, 6, 439, DateTimeKind.Local).AddTicks(32), new DateTime(2026, 4, 11, 17, 57, 6, 439, DateTimeKind.Local).AddTicks(38) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "448df289-142c-4959-a912-60733515e1b4",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 11, 17, 57, 6, 438, DateTimeKind.Local).AddTicks(9900), new DateTime(2026, 4, 11, 17, 57, 6, 438, DateTimeKind.Local).AddTicks(9905) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "48c50c3a-9958-453b-b649-4e21af131322",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 11, 17, 57, 6, 438, DateTimeKind.Local).AddTicks(9765), new DateTime(2026, 4, 11, 17, 57, 6, 438, DateTimeKind.Local).AddTicks(9770) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "717d9b15-a428-440c-b26b-08d3bbb68b02",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 11, 17, 57, 6, 438, DateTimeKind.Local).AddTicks(9192), new DateTime(2026, 4, 11, 17, 57, 6, 438, DateTimeKind.Local).AddTicks(9241) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "95ed2407-3e58-4af2-88a4-1c4e96473f68",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 11, 17, 57, 6, 438, DateTimeKind.Local).AddTicks(9608), new DateTime(2026, 4, 11, 17, 57, 6, 438, DateTimeKind.Local).AddTicks(9615) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "97942bee-ef12-4425-8225-4f293d0f36dd",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 11, 17, 57, 6, 439, DateTimeKind.Local).AddTicks(315), new DateTime(2026, 4, 11, 17, 57, 6, 439, DateTimeKind.Local).AddTicks(321) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "cd12b44b-103b-48df-8887-a2bf42e0651e",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 11, 17, 57, 6, 439, DateTimeKind.Local).AddTicks(179), new DateTime(2026, 4, 11, 17, 57, 6, 439, DateTimeKind.Local).AddTicks(185) });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "7e67d486-af3e-49f1-a109-a2b864b8e0ec",
                columns: new[] { "ConcurrencyStamp", "Created", "Modified", "PasswordHash", "SecurityStamp" },
                values: new object[] { "7c0ca28e-8eb2-44c5-8239-ee04db7f81ce", new DateTime(2026, 4, 11, 17, 57, 6, 439, DateTimeKind.Local).AddTicks(2958), new DateTime(2026, 4, 11, 17, 57, 6, 439, DateTimeKind.Local).AddTicks(2962), "AQAAAAIAAYagAAAAEDi9Ft7Ti3Sgt8292zAl6+wUIdojVviAuE7VCa3SCyEqp9pIHvSnLJkDSlfwePbADQ==", "22042751-acd3-4d62-b64f-2c0761a63776" });

            migrationBuilder.UpdateData(
                table: "Designations",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 11, 17, 57, 6, 439, DateTimeKind.Local).AddTicks(590), new DateTime(2026, 4, 11, 17, 57, 6, 439, DateTimeKind.Local).AddTicks(596) });

            migrationBuilder.UpdateData(
                table: "EmploymentTypes",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 11, 17, 57, 6, 439, DateTimeKind.Local).AddTicks(2230), new DateTime(2026, 4, 11, 17, 57, 6, 439, DateTimeKind.Local).AddTicks(2237) });

            migrationBuilder.UpdateData(
                table: "Genders",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 11, 17, 57, 6, 439, DateTimeKind.Local).AddTicks(2492), new DateTime(2026, 4, 11, 17, 57, 6, 439, DateTimeKind.Local).AddTicks(2495) });

            migrationBuilder.UpdateData(
                table: "Nationalities",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 11, 17, 57, 6, 439, DateTimeKind.Local).AddTicks(2354), new DateTime(2026, 4, 11, 17, 57, 6, 439, DateTimeKind.Local).AddTicks(2358) });

            migrationBuilder.UpdateData(
                table: "Person",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "DateOfBirth", "EmploymentDate", "EndofEmploymentDate", "Modified" },
                values: new object[] { new DateTime(2026, 4, 11, 17, 57, 6, 439, DateTimeKind.Local).AddTicks(2603), new DateTime(2026, 4, 11, 17, 57, 6, 439, DateTimeKind.Local).AddTicks(2599), new DateTime(2026, 4, 11, 17, 57, 6, 439, DateTimeKind.Local).AddTicks(2592), new DateTime(2026, 4, 11, 17, 57, 6, 439, DateTimeKind.Local).AddTicks(2595), new DateTime(2026, 4, 11, 17, 57, 6, 439, DateTimeKind.Local).AddTicks(2613) });

            migrationBuilder.UpdateData(
                table: "Religions",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 11, 17, 57, 6, 439, DateTimeKind.Local).AddTicks(2436), new DateTime(2026, 4, 11, 17, 57, 6, 439, DateTimeKind.Local).AddTicks(2439) });

            migrationBuilder.UpdateData(
                table: "StaffCategories",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 11, 17, 57, 6, 439, DateTimeKind.Local).AddTicks(476), new DateTime(2026, 4, 11, 17, 57, 6, 439, DateTimeKind.Local).AddTicks(482) });

            migrationBuilder.CreateIndex(
                name: "IX_Responsibilities_SocialSkillId",
                table: "Responsibilities",
                column: "SocialSkillId");

            migrationBuilder.AddForeignKey(
                name: "FK_Responsibilities_SocialSkills_SocialSkillId",
                table: "Responsibilities",
                column: "SocialSkillId",
                principalTable: "SocialSkills",
                principalColumn: "Id");
        }
    }
}
