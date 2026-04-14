using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SchoolWebApp.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class MergeResponsibilitySocialSkills : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ResponsibilitySocialSkills");

            migrationBuilder.AddColumn<string>(
                name: "Category",
                table: "Responsibilities",
                type: "varchar(50)",
                maxLength: 50,
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<int>(
                name: "SocialSkillId",
                table: "Responsibilities",
                type: "int",
                nullable: true);

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
                name: "IX_StudentResponsibilities_ResponsibilitySocialSkillId",
                table: "StudentResponsibilities",
                column: "ResponsibilitySocialSkillId");

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

            migrationBuilder.AddForeignKey(
                name: "FK_StudentResponsibilities_Responsibilities_ResponsibilitySocia~",
                table: "StudentResponsibilities",
                column: "ResponsibilitySocialSkillId",
                principalTable: "Responsibilities",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Responsibilities_SocialSkills_SocialSkillId",
                table: "Responsibilities");

            migrationBuilder.DropForeignKey(
                name: "FK_StudentResponsibilities_Responsibilities_ResponsibilitySocia~",
                table: "StudentResponsibilities");

            migrationBuilder.DropIndex(
                name: "IX_StudentResponsibilities_ResponsibilitySocialSkillId",
                table: "StudentResponsibilities");

            migrationBuilder.DropIndex(
                name: "IX_Responsibilities_SocialSkillId",
                table: "Responsibilities");

            migrationBuilder.DropColumn(
                name: "Category",
                table: "Responsibilities");

            migrationBuilder.DropColumn(
                name: "SocialSkillId",
                table: "Responsibilities");

            migrationBuilder.CreateTable(
                name: "ResponsibilitySocialSkills",
                columns: table => new
                {
                    ResponsibilitiesId = table.Column<int>(type: "int", nullable: false),
                    SocialSkillsId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ResponsibilitySocialSkills", x => new { x.ResponsibilitiesId, x.SocialSkillsId });
                    table.ForeignKey(
                        name: "FK_ResponsibilitySocialSkills_Responsibilities_Responsibilities~",
                        column: x => x.ResponsibilitiesId,
                        principalTable: "Responsibilities",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ResponsibilitySocialSkills_SocialSkills_SocialSkillsId",
                        column: x => x.SocialSkillsId,
                        principalTable: "SocialSkills",
                        principalColumn: "Id");
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "269f0cf3-405e-4163-83f3-1b63ebebd62e",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 11, 16, 44, 19, 857, DateTimeKind.Local).AddTicks(6986), new DateTime(2026, 4, 11, 16, 44, 19, 857, DateTimeKind.Local).AddTicks(6988) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "448df289-142c-4959-a912-60733515e1b4",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 11, 16, 44, 19, 857, DateTimeKind.Local).AddTicks(6968), new DateTime(2026, 4, 11, 16, 44, 19, 857, DateTimeKind.Local).AddTicks(6969) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "48c50c3a-9958-453b-b649-4e21af131322",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 11, 16, 44, 19, 857, DateTimeKind.Local).AddTicks(6949), new DateTime(2026, 4, 11, 16, 44, 19, 857, DateTimeKind.Local).AddTicks(6950) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "717d9b15-a428-440c-b26b-08d3bbb68b02",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 11, 16, 44, 19, 857, DateTimeKind.Local).AddTicks(6867), new DateTime(2026, 4, 11, 16, 44, 19, 857, DateTimeKind.Local).AddTicks(6889) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "95ed2407-3e58-4af2-88a4-1c4e96473f68",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 11, 16, 44, 19, 857, DateTimeKind.Local).AddTicks(6928), new DateTime(2026, 4, 11, 16, 44, 19, 857, DateTimeKind.Local).AddTicks(6929) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "97942bee-ef12-4425-8225-4f293d0f36dd",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 11, 16, 44, 19, 857, DateTimeKind.Local).AddTicks(7029), new DateTime(2026, 4, 11, 16, 44, 19, 857, DateTimeKind.Local).AddTicks(7030) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "cd12b44b-103b-48df-8887-a2bf42e0651e",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 11, 16, 44, 19, 857, DateTimeKind.Local).AddTicks(7005), new DateTime(2026, 4, 11, 16, 44, 19, 857, DateTimeKind.Local).AddTicks(7007) });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "7e67d486-af3e-49f1-a109-a2b864b8e0ec",
                columns: new[] { "ConcurrencyStamp", "Created", "Modified", "PasswordHash", "SecurityStamp" },
                values: new object[] { "b24c1b74-f9be-4878-a16b-072c8bd138e3", new DateTime(2026, 4, 11, 16, 44, 19, 857, DateTimeKind.Local).AddTicks(7296), new DateTime(2026, 4, 11, 16, 44, 19, 857, DateTimeKind.Local).AddTicks(7298), "AQAAAAIAAYagAAAAENk1tVXH3lpMj5HZdLoDUoN2ORls4icB28mcJzxHEETKU1207tW2tCB14tpN0/fm7g==", "d27aec11-755c-4fdb-a21e-85fc52896d1e" });

            migrationBuilder.UpdateData(
                table: "Designations",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 11, 16, 44, 19, 857, DateTimeKind.Local).AddTicks(7082), new DateTime(2026, 4, 11, 16, 44, 19, 857, DateTimeKind.Local).AddTicks(7083) });

            migrationBuilder.UpdateData(
                table: "EmploymentTypes",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 11, 16, 44, 19, 857, DateTimeKind.Local).AddTicks(7127), new DateTime(2026, 4, 11, 16, 44, 19, 857, DateTimeKind.Local).AddTicks(7128) });

            migrationBuilder.UpdateData(
                table: "Genders",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 11, 16, 44, 19, 857, DateTimeKind.Local).AddTicks(7185), new DateTime(2026, 4, 11, 16, 44, 19, 857, DateTimeKind.Local).AddTicks(7185) });

            migrationBuilder.UpdateData(
                table: "Nationalities",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 11, 16, 44, 19, 857, DateTimeKind.Local).AddTicks(7148), new DateTime(2026, 4, 11, 16, 44, 19, 857, DateTimeKind.Local).AddTicks(7149) });

            migrationBuilder.UpdateData(
                table: "Person",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "DateOfBirth", "EmploymentDate", "EndofEmploymentDate", "Modified" },
                values: new object[] { new DateTime(2026, 4, 11, 16, 44, 19, 857, DateTimeKind.Local).AddTicks(7224), new DateTime(2026, 4, 11, 16, 44, 19, 857, DateTimeKind.Local).AddTicks(7221), new DateTime(2026, 4, 11, 16, 44, 19, 857, DateTimeKind.Local).AddTicks(7218), new DateTime(2026, 4, 11, 16, 44, 19, 857, DateTimeKind.Local).AddTicks(7218), new DateTime(2026, 4, 11, 16, 44, 19, 857, DateTimeKind.Local).AddTicks(7228) });

            migrationBuilder.UpdateData(
                table: "Religions",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 11, 16, 44, 19, 857, DateTimeKind.Local).AddTicks(7171), new DateTime(2026, 4, 11, 16, 44, 19, 857, DateTimeKind.Local).AddTicks(7172) });

            migrationBuilder.UpdateData(
                table: "StaffCategories",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 11, 16, 44, 19, 857, DateTimeKind.Local).AddTicks(7061), new DateTime(2026, 4, 11, 16, 44, 19, 857, DateTimeKind.Local).AddTicks(7062) });

            migrationBuilder.CreateIndex(
                name: "IX_ResponsibilitySocialSkills_SocialSkillsId",
                table: "ResponsibilitySocialSkills",
                column: "SocialSkillsId");
        }
    }
}
