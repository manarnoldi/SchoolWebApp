using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SchoolWebApp.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Createdautomanytomanyonresponsibilityandsocialskills : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ResponsibilitySocialSkills_Responsibilities_ResponsibilityId",
                table: "ResponsibilitySocialSkills");

            migrationBuilder.DropForeignKey(
                name: "FK_ResponsibilitySocialSkills_SocialSkills_SocialSkillId",
                table: "ResponsibilitySocialSkills");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ResponsibilitySocialSkills",
                table: "ResponsibilitySocialSkills");

            migrationBuilder.DropIndex(
                name: "IX_ResponsibilitySocialSkills_ResponsibilityId",
                table: "ResponsibilitySocialSkills");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "ResponsibilitySocialSkills");

            migrationBuilder.DropColumn(
                name: "Created",
                table: "ResponsibilitySocialSkills");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "ResponsibilitySocialSkills");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "ResponsibilitySocialSkills");

            migrationBuilder.DropColumn(
                name: "Modified",
                table: "ResponsibilitySocialSkills");

            migrationBuilder.DropColumn(
                name: "ModifiedBy",
                table: "ResponsibilitySocialSkills");

            migrationBuilder.RenameColumn(
                name: "SocialSkillId",
                table: "ResponsibilitySocialSkills",
                newName: "SocialSkillsId");

            migrationBuilder.RenameColumn(
                name: "ResponsibilityId",
                table: "ResponsibilitySocialSkills",
                newName: "ResponsibilitiesId");

            migrationBuilder.RenameIndex(
                name: "IX_ResponsibilitySocialSkills_SocialSkillId",
                table: "ResponsibilitySocialSkills",
                newName: "IX_ResponsibilitySocialSkills_SocialSkillsId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ResponsibilitySocialSkills",
                table: "ResponsibilitySocialSkills",
                columns: new[] { "ResponsibilitiesId", "SocialSkillsId" });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "269f0cf3-405e-4163-83f3-1b63ebebd62e",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2025, 9, 26, 8, 32, 7, 13, DateTimeKind.Local).AddTicks(3934), new DateTime(2025, 9, 26, 8, 32, 7, 13, DateTimeKind.Local).AddTicks(3937) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "448df289-142c-4959-a912-60733515e1b4",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2025, 9, 26, 8, 32, 7, 13, DateTimeKind.Local).AddTicks(3876), new DateTime(2025, 9, 26, 8, 32, 7, 13, DateTimeKind.Local).AddTicks(3879) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "48c50c3a-9958-453b-b649-4e21af131322",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2025, 9, 26, 8, 32, 7, 13, DateTimeKind.Local).AddTicks(3843), new DateTime(2025, 9, 26, 8, 32, 7, 13, DateTimeKind.Local).AddTicks(3845) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "717d9b15-a428-440c-b26b-08d3bbb68b02",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2025, 9, 26, 8, 32, 7, 13, DateTimeKind.Local).AddTicks(3712), new DateTime(2025, 9, 26, 8, 32, 7, 13, DateTimeKind.Local).AddTicks(3741) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "95ed2407-3e58-4af2-88a4-1c4e96473f68",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2025, 9, 26, 8, 32, 7, 13, DateTimeKind.Local).AddTicks(3808), new DateTime(2025, 9, 26, 8, 32, 7, 13, DateTimeKind.Local).AddTicks(3810) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "97942bee-ef12-4425-8225-4f293d0f36dd",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2025, 9, 26, 8, 32, 7, 13, DateTimeKind.Local).AddTicks(4011), new DateTime(2025, 9, 26, 8, 32, 7, 13, DateTimeKind.Local).AddTicks(4013) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "cd12b44b-103b-48df-8887-a2bf42e0651e",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2025, 9, 26, 8, 32, 7, 13, DateTimeKind.Local).AddTicks(3977), new DateTime(2025, 9, 26, 8, 32, 7, 13, DateTimeKind.Local).AddTicks(3979) });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "7e67d486-af3e-49f1-a109-a2b864b8e0ec",
                columns: new[] { "ConcurrencyStamp", "Created", "Modified", "PasswordHash", "SecurityStamp" },
                values: new object[] { "e65e6df3-4d34-49cc-ba41-539ee240b915", new DateTime(2025, 9, 26, 8, 32, 7, 13, DateTimeKind.Local).AddTicks(4420), new DateTime(2025, 9, 26, 8, 32, 7, 13, DateTimeKind.Local).AddTicks(4422), "AQAAAAIAAYagAAAAEBsoP7MucQUkPcb0R/I1ngFdXa2mVHLkhLBlEvgF6PQeAwQAtkYmxAQFI93bFKFXrw==", "924a0c0e-c590-48b1-aa62-23be37a5a689" });

            migrationBuilder.UpdateData(
                table: "Designations",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2025, 9, 26, 8, 32, 7, 13, DateTimeKind.Local).AddTicks(4089), new DateTime(2025, 9, 26, 8, 32, 7, 13, DateTimeKind.Local).AddTicks(4090) });

            migrationBuilder.UpdateData(
                table: "EmploymentTypes",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2025, 9, 26, 8, 32, 7, 13, DateTimeKind.Local).AddTicks(4168), new DateTime(2025, 9, 26, 8, 32, 7, 13, DateTimeKind.Local).AddTicks(4169) });

            migrationBuilder.UpdateData(
                table: "Genders",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2025, 9, 26, 8, 32, 7, 13, DateTimeKind.Local).AddTicks(4257), new DateTime(2025, 9, 26, 8, 32, 7, 13, DateTimeKind.Local).AddTicks(4258) });

            migrationBuilder.UpdateData(
                table: "Nationalities",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2025, 9, 26, 8, 32, 7, 13, DateTimeKind.Local).AddTicks(4198), new DateTime(2025, 9, 26, 8, 32, 7, 13, DateTimeKind.Local).AddTicks(4199) });

            migrationBuilder.UpdateData(
                table: "Person",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "DateOfBirth", "EmploymentDate", "EndofEmploymentDate", "Modified" },
                values: new object[] { new DateTime(2025, 9, 26, 8, 32, 7, 13, DateTimeKind.Local).AddTicks(4311), new DateTime(2025, 9, 26, 8, 32, 7, 13, DateTimeKind.Local).AddTicks(4307), new DateTime(2025, 9, 26, 8, 32, 7, 13, DateTimeKind.Local).AddTicks(4301), new DateTime(2025, 9, 26, 8, 32, 7, 13, DateTimeKind.Local).AddTicks(4303), new DateTime(2025, 9, 26, 8, 32, 7, 13, DateTimeKind.Local).AddTicks(4315) });

            migrationBuilder.UpdateData(
                table: "Religions",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2025, 9, 26, 8, 32, 7, 13, DateTimeKind.Local).AddTicks(4228), new DateTime(2025, 9, 26, 8, 32, 7, 13, DateTimeKind.Local).AddTicks(4230) });

            migrationBuilder.UpdateData(
                table: "StaffCategories",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2025, 9, 26, 8, 32, 7, 13, DateTimeKind.Local).AddTicks(4057), new DateTime(2025, 9, 26, 8, 32, 7, 13, DateTimeKind.Local).AddTicks(4059) });

            migrationBuilder.AddForeignKey(
                name: "FK_ResponsibilitySocialSkills_Responsibilities_Responsibilities~",
                table: "ResponsibilitySocialSkills",
                column: "ResponsibilitiesId",
                principalTable: "Responsibilities",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ResponsibilitySocialSkills_SocialSkills_SocialSkillsId",
                table: "ResponsibilitySocialSkills",
                column: "SocialSkillsId",
                principalTable: "SocialSkills",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ResponsibilitySocialSkills_Responsibilities_Responsibilities~",
                table: "ResponsibilitySocialSkills");

            migrationBuilder.DropForeignKey(
                name: "FK_ResponsibilitySocialSkills_SocialSkills_SocialSkillsId",
                table: "ResponsibilitySocialSkills");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ResponsibilitySocialSkills",
                table: "ResponsibilitySocialSkills");

            migrationBuilder.RenameColumn(
                name: "SocialSkillsId",
                table: "ResponsibilitySocialSkills",
                newName: "SocialSkillId");

            migrationBuilder.RenameColumn(
                name: "ResponsibilitiesId",
                table: "ResponsibilitySocialSkills",
                newName: "ResponsibilityId");

            migrationBuilder.RenameIndex(
                name: "IX_ResponsibilitySocialSkills_SocialSkillsId",
                table: "ResponsibilitySocialSkills",
                newName: "IX_ResponsibilitySocialSkills_SocialSkillId");

            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "ResponsibilitySocialSkills",
                type: "int",
                nullable: false,
                defaultValue: 0)
                .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AddColumn<DateTime>(
                name: "Created",
                table: "ResponsibilitySocialSkills",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                table: "ResponsibilitySocialSkills",
                type: "varchar(255)",
                maxLength: 255,
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "ResponsibilitySocialSkills",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<DateTime>(
                name: "Modified",
                table: "ResponsibilitySocialSkills",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ModifiedBy",
                table: "ResponsibilitySocialSkills",
                type: "varchar(255)",
                maxLength: 255,
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ResponsibilitySocialSkills",
                table: "ResponsibilitySocialSkills",
                column: "Id");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "269f0cf3-405e-4163-83f3-1b63ebebd62e",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2025, 9, 26, 7, 40, 30, 475, DateTimeKind.Local).AddTicks(2104), new DateTime(2025, 9, 26, 7, 40, 30, 475, DateTimeKind.Local).AddTicks(2105) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "448df289-142c-4959-a912-60733515e1b4",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2025, 9, 26, 7, 40, 30, 475, DateTimeKind.Local).AddTicks(2074), new DateTime(2025, 9, 26, 7, 40, 30, 475, DateTimeKind.Local).AddTicks(2075) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "48c50c3a-9958-453b-b649-4e21af131322",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2025, 9, 26, 7, 40, 30, 475, DateTimeKind.Local).AddTicks(2055), new DateTime(2025, 9, 26, 7, 40, 30, 475, DateTimeKind.Local).AddTicks(2056) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "717d9b15-a428-440c-b26b-08d3bbb68b02",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2025, 9, 26, 7, 40, 30, 475, DateTimeKind.Local).AddTicks(1987), new DateTime(2025, 9, 26, 7, 40, 30, 475, DateTimeKind.Local).AddTicks(2002) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "95ed2407-3e58-4af2-88a4-1c4e96473f68",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2025, 9, 26, 7, 40, 30, 475, DateTimeKind.Local).AddTicks(2036), new DateTime(2025, 9, 26, 7, 40, 30, 475, DateTimeKind.Local).AddTicks(2037) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "97942bee-ef12-4425-8225-4f293d0f36dd",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2025, 9, 26, 7, 40, 30, 475, DateTimeKind.Local).AddTicks(2141), new DateTime(2025, 9, 26, 7, 40, 30, 475, DateTimeKind.Local).AddTicks(2142) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "cd12b44b-103b-48df-8887-a2bf42e0651e",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2025, 9, 26, 7, 40, 30, 475, DateTimeKind.Local).AddTicks(2123), new DateTime(2025, 9, 26, 7, 40, 30, 475, DateTimeKind.Local).AddTicks(2125) });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "7e67d486-af3e-49f1-a109-a2b864b8e0ec",
                columns: new[] { "ConcurrencyStamp", "Created", "Modified", "PasswordHash", "SecurityStamp" },
                values: new object[] { "92dfc7ea-8656-4193-bf14-b720b5527424", new DateTime(2025, 9, 26, 7, 40, 30, 475, DateTimeKind.Local).AddTicks(2351), new DateTime(2025, 9, 26, 7, 40, 30, 475, DateTimeKind.Local).AddTicks(2353), "AQAAAAIAAYagAAAAENJ/cUj9li32HENRbk6gI8jgRzMdA7i1A0Xbfm6po+x4Z8Uhj3gkkbSzrOPAqBgG5w==", "0b9f5df0-5913-4067-b7bb-9730d61d98f4" });

            migrationBuilder.UpdateData(
                table: "Designations",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2025, 9, 26, 7, 40, 30, 475, DateTimeKind.Local).AddTicks(2189), new DateTime(2025, 9, 26, 7, 40, 30, 475, DateTimeKind.Local).AddTicks(2190) });

            migrationBuilder.UpdateData(
                table: "EmploymentTypes",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2025, 9, 26, 7, 40, 30, 475, DateTimeKind.Local).AddTicks(2209), new DateTime(2025, 9, 26, 7, 40, 30, 475, DateTimeKind.Local).AddTicks(2210) });

            migrationBuilder.UpdateData(
                table: "Genders",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2025, 9, 26, 7, 40, 30, 475, DateTimeKind.Local).AddTicks(2262), new DateTime(2025, 9, 26, 7, 40, 30, 475, DateTimeKind.Local).AddTicks(2262) });

            migrationBuilder.UpdateData(
                table: "Nationalities",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2025, 9, 26, 7, 40, 30, 475, DateTimeKind.Local).AddTicks(2228), new DateTime(2025, 9, 26, 7, 40, 30, 475, DateTimeKind.Local).AddTicks(2228) });

            migrationBuilder.UpdateData(
                table: "Person",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "DateOfBirth", "EmploymentDate", "EndofEmploymentDate", "Modified" },
                values: new object[] { new DateTime(2025, 9, 26, 7, 40, 30, 475, DateTimeKind.Local).AddTicks(2297), new DateTime(2025, 9, 26, 7, 40, 30, 475, DateTimeKind.Local).AddTicks(2294), new DateTime(2025, 9, 26, 7, 40, 30, 475, DateTimeKind.Local).AddTicks(2290), new DateTime(2025, 9, 26, 7, 40, 30, 475, DateTimeKind.Local).AddTicks(2291), new DateTime(2025, 9, 26, 7, 40, 30, 475, DateTimeKind.Local).AddTicks(2297) });

            migrationBuilder.UpdateData(
                table: "Religions",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2025, 9, 26, 7, 40, 30, 475, DateTimeKind.Local).AddTicks(2245), new DateTime(2025, 9, 26, 7, 40, 30, 475, DateTimeKind.Local).AddTicks(2246) });

            migrationBuilder.UpdateData(
                table: "StaffCategories",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2025, 9, 26, 7, 40, 30, 475, DateTimeKind.Local).AddTicks(2171), new DateTime(2025, 9, 26, 7, 40, 30, 475, DateTimeKind.Local).AddTicks(2173) });

            migrationBuilder.CreateIndex(
                name: "IX_ResponsibilitySocialSkills_ResponsibilityId",
                table: "ResponsibilitySocialSkills",
                column: "ResponsibilityId");

            migrationBuilder.AddForeignKey(
                name: "FK_ResponsibilitySocialSkills_Responsibilities_ResponsibilityId",
                table: "ResponsibilitySocialSkills",
                column: "ResponsibilityId",
                principalTable: "Responsibilities",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ResponsibilitySocialSkills_SocialSkills_SocialSkillId",
                table: "ResponsibilitySocialSkills",
                column: "SocialSkillId",
                principalTable: "SocialSkills",
                principalColumn: "Id");
        }
    }
}
