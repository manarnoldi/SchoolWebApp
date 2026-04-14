using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SchoolWebApp.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddThemeEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ThemeId",
                table: "Strands",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Themes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Code = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CurriculumId = table.Column<int>(type: "int", nullable: true),
                    SubjectId = table.Column<int>(type: "int", nullable: false),
                    LearningLevelId = table.Column<int>(type: "int", nullable: false),
                    Created = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    CreatedBy = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Modified = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    ModifiedBy = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Name = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Description = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Rank = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Themes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Themes_Curricula_CurriculumId",
                        column: x => x.CurriculumId,
                        principalTable: "Curricula",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Themes_LearningLevels_LearningLevelId",
                        column: x => x.LearningLevelId,
                        principalTable: "LearningLevels",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Themes_Subjects_SubjectId",
                        column: x => x.SubjectId,
                        principalTable: "Subjects",
                        principalColumn: "Id");
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "269f0cf3-405e-4163-83f3-1b63ebebd62e",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 11, 13, 17, 31, 266, DateTimeKind.Local).AddTicks(5265), new DateTime(2026, 4, 11, 13, 17, 31, 266, DateTimeKind.Local).AddTicks(5266) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "448df289-142c-4959-a912-60733515e1b4",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 11, 13, 17, 31, 266, DateTimeKind.Local).AddTicks(5249), new DateTime(2026, 4, 11, 13, 17, 31, 266, DateTimeKind.Local).AddTicks(5250) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "48c50c3a-9958-453b-b649-4e21af131322",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 11, 13, 17, 31, 266, DateTimeKind.Local).AddTicks(5232), new DateTime(2026, 4, 11, 13, 17, 31, 266, DateTimeKind.Local).AddTicks(5233) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "717d9b15-a428-440c-b26b-08d3bbb68b02",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 11, 13, 17, 31, 266, DateTimeKind.Local).AddTicks(5163), new DateTime(2026, 4, 11, 13, 17, 31, 266, DateTimeKind.Local).AddTicks(5176) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "95ed2407-3e58-4af2-88a4-1c4e96473f68",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 11, 13, 17, 31, 266, DateTimeKind.Local).AddTicks(5215), new DateTime(2026, 4, 11, 13, 17, 31, 266, DateTimeKind.Local).AddTicks(5217) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "97942bee-ef12-4425-8225-4f293d0f36dd",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 11, 13, 17, 31, 266, DateTimeKind.Local).AddTicks(5303), new DateTime(2026, 4, 11, 13, 17, 31, 266, DateTimeKind.Local).AddTicks(5304) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "cd12b44b-103b-48df-8887-a2bf42e0651e",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 11, 13, 17, 31, 266, DateTimeKind.Local).AddTicks(5281), new DateTime(2026, 4, 11, 13, 17, 31, 266, DateTimeKind.Local).AddTicks(5282) });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "7e67d486-af3e-49f1-a109-a2b864b8e0ec",
                columns: new[] { "ConcurrencyStamp", "Created", "Modified", "PasswordHash", "SecurityStamp" },
                values: new object[] { "682b1c2a-86ec-442b-981a-d821f396b0bd", new DateTime(2026, 4, 11, 13, 17, 31, 266, DateTimeKind.Local).AddTicks(5630), new DateTime(2026, 4, 11, 13, 17, 31, 266, DateTimeKind.Local).AddTicks(5632), "AQAAAAIAAYagAAAAEJfiDjYLF8iUwbXGXEIB5Q/qtZiLsphc2e1SlqTKvg+rovmuOZQ1V/n4i1TtJjUKZQ==", "5a2c50aa-c7cb-4417-8bd4-723a7183489a" });

            migrationBuilder.UpdateData(
                table: "Designations",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 11, 13, 17, 31, 266, DateTimeKind.Local).AddTicks(5346), new DateTime(2026, 4, 11, 13, 17, 31, 266, DateTimeKind.Local).AddTicks(5346) });

            migrationBuilder.UpdateData(
                table: "EmploymentTypes",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 11, 13, 17, 31, 266, DateTimeKind.Local).AddTicks(5372), new DateTime(2026, 4, 11, 13, 17, 31, 266, DateTimeKind.Local).AddTicks(5373) });

            migrationBuilder.UpdateData(
                table: "Genders",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 11, 13, 17, 31, 266, DateTimeKind.Local).AddTicks(5531), new DateTime(2026, 4, 11, 13, 17, 31, 266, DateTimeKind.Local).AddTicks(5532) });

            migrationBuilder.UpdateData(
                table: "Nationalities",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 11, 13, 17, 31, 266, DateTimeKind.Local).AddTicks(5389), new DateTime(2026, 4, 11, 13, 17, 31, 266, DateTimeKind.Local).AddTicks(5389) });

            migrationBuilder.UpdateData(
                table: "Person",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "DateOfBirth", "EmploymentDate", "EndofEmploymentDate", "Modified" },
                values: new object[] { new DateTime(2026, 4, 11, 13, 17, 31, 266, DateTimeKind.Local).AddTicks(5564), new DateTime(2026, 4, 11, 13, 17, 31, 266, DateTimeKind.Local).AddTicks(5562), new DateTime(2026, 4, 11, 13, 17, 31, 266, DateTimeKind.Local).AddTicks(5559), new DateTime(2026, 4, 11, 13, 17, 31, 266, DateTimeKind.Local).AddTicks(5559), new DateTime(2026, 4, 11, 13, 17, 31, 266, DateTimeKind.Local).AddTicks(5565) });

            migrationBuilder.UpdateData(
                table: "Religions",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 11, 13, 17, 31, 266, DateTimeKind.Local).AddTicks(5509), new DateTime(2026, 4, 11, 13, 17, 31, 266, DateTimeKind.Local).AddTicks(5511) });

            migrationBuilder.UpdateData(
                table: "StaffCategories",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 11, 13, 17, 31, 266, DateTimeKind.Local).AddTicks(5329), new DateTime(2026, 4, 11, 13, 17, 31, 266, DateTimeKind.Local).AddTicks(5331) });

            migrationBuilder.CreateIndex(
                name: "IX_Strands_ThemeId",
                table: "Strands",
                column: "ThemeId");

            migrationBuilder.CreateIndex(
                name: "IX_Themes_CurriculumId",
                table: "Themes",
                column: "CurriculumId");

            migrationBuilder.CreateIndex(
                name: "IX_Themes_LearningLevelId",
                table: "Themes",
                column: "LearningLevelId");

            migrationBuilder.CreateIndex(
                name: "IX_Themes_SubjectId",
                table: "Themes",
                column: "SubjectId");

            migrationBuilder.AddForeignKey(
                name: "FK_Strands_Themes_ThemeId",
                table: "Strands",
                column: "ThemeId",
                principalTable: "Themes",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Strands_Themes_ThemeId",
                table: "Strands");

            migrationBuilder.DropTable(
                name: "Themes");

            migrationBuilder.DropIndex(
                name: "IX_Strands_ThemeId",
                table: "Strands");

            migrationBuilder.DropColumn(
                name: "ThemeId",
                table: "Strands");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "269f0cf3-405e-4163-83f3-1b63ebebd62e",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 11, 10, 31, 47, 521, DateTimeKind.Local).AddTicks(6794), new DateTime(2026, 4, 11, 10, 31, 47, 521, DateTimeKind.Local).AddTicks(6795) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "448df289-142c-4959-a912-60733515e1b4",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 11, 10, 31, 47, 521, DateTimeKind.Local).AddTicks(6775), new DateTime(2026, 4, 11, 10, 31, 47, 521, DateTimeKind.Local).AddTicks(6776) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "48c50c3a-9958-453b-b649-4e21af131322",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 11, 10, 31, 47, 521, DateTimeKind.Local).AddTicks(6755), new DateTime(2026, 4, 11, 10, 31, 47, 521, DateTimeKind.Local).AddTicks(6756) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "717d9b15-a428-440c-b26b-08d3bbb68b02",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 11, 10, 31, 47, 521, DateTimeKind.Local).AddTicks(6674), new DateTime(2026, 4, 11, 10, 31, 47, 521, DateTimeKind.Local).AddTicks(6688) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "95ed2407-3e58-4af2-88a4-1c4e96473f68",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 11, 10, 31, 47, 521, DateTimeKind.Local).AddTicks(6729), new DateTime(2026, 4, 11, 10, 31, 47, 521, DateTimeKind.Local).AddTicks(6731) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "97942bee-ef12-4425-8225-4f293d0f36dd",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 11, 10, 31, 47, 521, DateTimeKind.Local).AddTicks(6847), new DateTime(2026, 4, 11, 10, 31, 47, 521, DateTimeKind.Local).AddTicks(6849) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "cd12b44b-103b-48df-8887-a2bf42e0651e",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 11, 10, 31, 47, 521, DateTimeKind.Local).AddTicks(6827), new DateTime(2026, 4, 11, 10, 31, 47, 521, DateTimeKind.Local).AddTicks(6828) });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "7e67d486-af3e-49f1-a109-a2b864b8e0ec",
                columns: new[] { "ConcurrencyStamp", "Created", "Modified", "PasswordHash", "SecurityStamp" },
                values: new object[] { "e5842f2a-1f4d-4260-9073-aad6290aaec4", new DateTime(2026, 4, 11, 10, 31, 47, 521, DateTimeKind.Local).AddTicks(7160), new DateTime(2026, 4, 11, 10, 31, 47, 521, DateTimeKind.Local).AddTicks(7161), "AQAAAAIAAYagAAAAEIzuNOH1O1r/ezXAS7P7g38cy48kkW3U+Y3ykEvFd2wVvwKYRi7cMSG91ofO3p8M+g==", "4d144fae-b7b2-4b8e-97bb-cc9420541b95" });

            migrationBuilder.UpdateData(
                table: "Designations",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 11, 10, 31, 47, 521, DateTimeKind.Local).AddTicks(6913), new DateTime(2026, 4, 11, 10, 31, 47, 521, DateTimeKind.Local).AddTicks(6914) });

            migrationBuilder.UpdateData(
                table: "EmploymentTypes",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 11, 10, 31, 47, 521, DateTimeKind.Local).AddTicks(6960), new DateTime(2026, 4, 11, 10, 31, 47, 521, DateTimeKind.Local).AddTicks(6961) });

            migrationBuilder.UpdateData(
                table: "Genders",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 11, 10, 31, 47, 521, DateTimeKind.Local).AddTicks(7020), new DateTime(2026, 4, 11, 10, 31, 47, 521, DateTimeKind.Local).AddTicks(7021) });

            migrationBuilder.UpdateData(
                table: "Nationalities",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 11, 10, 31, 47, 521, DateTimeKind.Local).AddTicks(6981), new DateTime(2026, 4, 11, 10, 31, 47, 521, DateTimeKind.Local).AddTicks(6982) });

            migrationBuilder.UpdateData(
                table: "Person",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "DateOfBirth", "EmploymentDate", "EndofEmploymentDate", "Modified" },
                values: new object[] { new DateTime(2026, 4, 11, 10, 31, 47, 521, DateTimeKind.Local).AddTicks(7069), new DateTime(2026, 4, 11, 10, 31, 47, 521, DateTimeKind.Local).AddTicks(7067), new DateTime(2026, 4, 11, 10, 31, 47, 521, DateTimeKind.Local).AddTicks(7062), new DateTime(2026, 4, 11, 10, 31, 47, 521, DateTimeKind.Local).AddTicks(7063), new DateTime(2026, 4, 11, 10, 31, 47, 521, DateTimeKind.Local).AddTicks(7073) });

            migrationBuilder.UpdateData(
                table: "Religions",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 11, 10, 31, 47, 521, DateTimeKind.Local).AddTicks(7002), new DateTime(2026, 4, 11, 10, 31, 47, 521, DateTimeKind.Local).AddTicks(7003) });

            migrationBuilder.UpdateData(
                table: "StaffCategories",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 11, 10, 31, 47, 521, DateTimeKind.Local).AddTicks(6889), new DateTime(2026, 4, 11, 10, 31, 47, 521, DateTimeKind.Local).AddTicks(6891) });
        }
    }
}
