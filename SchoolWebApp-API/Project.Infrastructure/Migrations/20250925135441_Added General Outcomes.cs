using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SchoolWebApp.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddedGeneralOutcomes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            //migrationBuilder.DropForeignKey(
            //    name: "FK_Assessments_LearningOutcomes_LearningOutcomeId",
            //    table: "Assessments");

            migrationBuilder.DropForeignKey(
                name: "FK_Strands_EducationLevelSubjects_EducationLevelSubjectId",
                table: "Strands");

            migrationBuilder.DropTable(
                name: "LearningOutcomes");

            migrationBuilder.RenameColumn(
                name: "EducationLevelSubjectId",
                table: "Strands",
                newName: "SubjectId");

            migrationBuilder.RenameIndex(
                name: "IX_Strands_EducationLevelSubjectId",
                table: "Strands",
                newName: "IX_Strands_SubjectId");

            migrationBuilder.RenameColumn(
                name: "LearningOutcomeId",
                table: "Assessments",
                newName: "SpecificOutcomeId");

            migrationBuilder.RenameIndex(
                name: "IX_Assessments_LearningOutcomeId",
                table: "Assessments",
                newName: "IX_Assessments_SpecificOutcomeId");

            migrationBuilder.AddColumn<int>(
                name: "SubjectId",
                table: "BroadOutcomes",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "SchoolClassId",
                table: "Assessments",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "GeneralOutcomes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    EducationLevelTypeId = table.Column<int>(type: "int", nullable: false),
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
                    table.PrimaryKey("PK_GeneralOutcomes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GeneralOutcomes_EducationLevelTypes_EducationLevelTypeId",
                        column: x => x.EducationLevelTypeId,
                        principalTable: "EducationLevelTypes",
                        principalColumn: "Id");
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "SpecificOutcomes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    SubStrandId = table.Column<int>(type: "int", nullable: false),
                    LearningLevelId = table.Column<int>(type: "int", nullable: false),
                    BroadOutcomeId = table.Column<int>(type: "int", nullable: false),
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
                    table.PrimaryKey("PK_SpecificOutcomes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SpecificOutcomes_BroadOutcomes_BroadOutcomeId",
                        column: x => x.BroadOutcomeId,
                        principalTable: "BroadOutcomes",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_SpecificOutcomes_LearningLevels_LearningLevelId",
                        column: x => x.LearningLevelId,
                        principalTable: "LearningLevels",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_SpecificOutcomes_SubStrands_SubStrandId",
                        column: x => x.SubStrandId,
                        principalTable: "SubStrands",
                        principalColumn: "Id");
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "SpecificOutcomeCompetencies",
                columns: table => new
                {
                    CompetenciesId = table.Column<int>(type: "int", nullable: false),
                    SpecificOutcomesId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SpecificOutcomeCompetencies", x => new { x.CompetenciesId, x.SpecificOutcomesId });
                    table.ForeignKey(
                        name: "FK_SpecificOutcomeCompetencies_Competencies_CompetenciesId",
                        column: x => x.CompetenciesId,
                        principalTable: "Competencies",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_SpecificOutcomeCompetencies_SpecificOutcomes_SpecificOutcome~",
                        column: x => x.SpecificOutcomesId,
                        principalTable: "SpecificOutcomes",
                        principalColumn: "Id");
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "269f0cf3-405e-4163-83f3-1b63ebebd62e",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2025, 9, 25, 16, 54, 39, 444, DateTimeKind.Local).AddTicks(2520), new DateTime(2025, 9, 25, 16, 54, 39, 444, DateTimeKind.Local).AddTicks(2523) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "448df289-142c-4959-a912-60733515e1b4",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2025, 9, 25, 16, 54, 39, 444, DateTimeKind.Local).AddTicks(2487), new DateTime(2025, 9, 25, 16, 54, 39, 444, DateTimeKind.Local).AddTicks(2490) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "48c50c3a-9958-453b-b649-4e21af131322",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2025, 9, 25, 16, 54, 39, 444, DateTimeKind.Local).AddTicks(2450), new DateTime(2025, 9, 25, 16, 54, 39, 444, DateTimeKind.Local).AddTicks(2453) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "717d9b15-a428-440c-b26b-08d3bbb68b02",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2025, 9, 25, 16, 54, 39, 444, DateTimeKind.Local).AddTicks(2327), new DateTime(2025, 9, 25, 16, 54, 39, 444, DateTimeKind.Local).AddTicks(2346) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "95ed2407-3e58-4af2-88a4-1c4e96473f68",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2025, 9, 25, 16, 54, 39, 444, DateTimeKind.Local).AddTicks(2414), new DateTime(2025, 9, 25, 16, 54, 39, 444, DateTimeKind.Local).AddTicks(2417) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "97942bee-ef12-4425-8225-4f293d0f36dd",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2025, 9, 25, 16, 54, 39, 444, DateTimeKind.Local).AddTicks(2596), new DateTime(2025, 9, 25, 16, 54, 39, 444, DateTimeKind.Local).AddTicks(2599) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "cd12b44b-103b-48df-8887-a2bf42e0651e",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2025, 9, 25, 16, 54, 39, 444, DateTimeKind.Local).AddTicks(2556), new DateTime(2025, 9, 25, 16, 54, 39, 444, DateTimeKind.Local).AddTicks(2559) });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "7e67d486-af3e-49f1-a109-a2b864b8e0ec",
                columns: new[] { "ConcurrencyStamp", "Created", "Modified", "PasswordHash", "SecurityStamp" },
                values: new object[] { "ae9173e1-9017-40ad-88b7-d0aa1396c8f3", new DateTime(2025, 9, 25, 16, 54, 39, 444, DateTimeKind.Local).AddTicks(3098), new DateTime(2025, 9, 25, 16, 54, 39, 444, DateTimeKind.Local).AddTicks(3101), "AQAAAAIAAYagAAAAEClR0+4Thw+uvYQlWCmku0aXjR2p2lVkMrhHdLDonua5T5cYz1mKFUvUxpc1k5uAYQ==", "a9f54644-7441-4d8a-a5b8-5d2e424f81b2" });

            migrationBuilder.UpdateData(
                table: "Designations",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2025, 9, 25, 16, 54, 39, 444, DateTimeKind.Local).AddTicks(2675), new DateTime(2025, 9, 25, 16, 54, 39, 444, DateTimeKind.Local).AddTicks(2676) });

            migrationBuilder.UpdateData(
                table: "EmploymentTypes",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2025, 9, 25, 16, 54, 39, 444, DateTimeKind.Local).AddTicks(2713), new DateTime(2025, 9, 25, 16, 54, 39, 444, DateTimeKind.Local).AddTicks(2714) });

            migrationBuilder.UpdateData(
                table: "Genders",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2025, 9, 25, 16, 54, 39, 444, DateTimeKind.Local).AddTicks(2817), new DateTime(2025, 9, 25, 16, 54, 39, 444, DateTimeKind.Local).AddTicks(2818) });

            migrationBuilder.UpdateData(
                table: "Nationalities",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2025, 9, 25, 16, 54, 39, 444, DateTimeKind.Local).AddTicks(2747), new DateTime(2025, 9, 25, 16, 54, 39, 444, DateTimeKind.Local).AddTicks(2748) });

            migrationBuilder.UpdateData(
                table: "Person",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "DateOfBirth", "EmploymentDate", "EndofEmploymentDate", "Modified" },
                values: new object[] { new DateTime(2025, 9, 25, 16, 54, 39, 444, DateTimeKind.Local).AddTicks(2871), new DateTime(2025, 9, 25, 16, 54, 39, 444, DateTimeKind.Local).AddTicks(2864), new DateTime(2025, 9, 25, 16, 54, 39, 444, DateTimeKind.Local).AddTicks(2858), new DateTime(2025, 9, 25, 16, 54, 39, 444, DateTimeKind.Local).AddTicks(2859), new DateTime(2025, 9, 25, 16, 54, 39, 444, DateTimeKind.Local).AddTicks(2880) });

            migrationBuilder.UpdateData(
                table: "Religions",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2025, 9, 25, 16, 54, 39, 444, DateTimeKind.Local).AddTicks(2786), new DateTime(2025, 9, 25, 16, 54, 39, 444, DateTimeKind.Local).AddTicks(2787) });

            migrationBuilder.UpdateData(
                table: "StaffCategories",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2025, 9, 25, 16, 54, 39, 444, DateTimeKind.Local).AddTicks(2640), new DateTime(2025, 9, 25, 16, 54, 39, 444, DateTimeKind.Local).AddTicks(2643) });

            migrationBuilder.CreateIndex(
                name: "IX_BroadOutcomes_SubjectId",
                table: "BroadOutcomes",
                column: "SubjectId");

            migrationBuilder.CreateIndex(
                name: "IX_Assessments_SchoolClassId",
                table: "Assessments",
                column: "SchoolClassId");

            migrationBuilder.CreateIndex(
                name: "IX_GeneralOutcomes_EducationLevelTypeId",
                table: "GeneralOutcomes",
                column: "EducationLevelTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_SpecificOutcomeCompetencies_SpecificOutcomesId",
                table: "SpecificOutcomeCompetencies",
                column: "SpecificOutcomesId");

            migrationBuilder.CreateIndex(
                name: "IX_SpecificOutcomes_BroadOutcomeId",
                table: "SpecificOutcomes",
                column: "BroadOutcomeId");

            migrationBuilder.CreateIndex(
                name: "IX_SpecificOutcomes_LearningLevelId",
                table: "SpecificOutcomes",
                column: "LearningLevelId");

            migrationBuilder.CreateIndex(
                name: "IX_SpecificOutcomes_SubStrandId",
                table: "SpecificOutcomes",
                column: "SubStrandId");

            migrationBuilder.AddForeignKey(
                name: "FK_Assessments_SchoolClasses_SchoolClassId",
                table: "Assessments",
                column: "SchoolClassId",
                principalTable: "SchoolClasses",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Assessments_SpecificOutcomes_SpecificOutcomeId",
                table: "Assessments",
                column: "SpecificOutcomeId",
                principalTable: "SpecificOutcomes",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_BroadOutcomes_Subjects_SubjectId",
                table: "BroadOutcomes",
                column: "SubjectId",
                principalTable: "Subjects",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Strands_Subjects_SubjectId",
                table: "Strands",
                column: "SubjectId",
                principalTable: "Subjects",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Assessments_SchoolClasses_SchoolClassId",
                table: "Assessments");

            //migrationBuilder.DropForeignKey(
            //    name: "FK_Assessments_SpecificOutcomes_SpecificOutcomeId",
            //    table: "Assessments");

            migrationBuilder.DropForeignKey(
                name: "FK_BroadOutcomes_Subjects_SubjectId",
                table: "BroadOutcomes");

            migrationBuilder.DropForeignKey(
                name: "FK_Strands_Subjects_SubjectId",
                table: "Strands");

            migrationBuilder.DropTable(
                name: "GeneralOutcomes");

            migrationBuilder.DropTable(
                name: "SpecificOutcomeCompetencies");

            migrationBuilder.DropTable(
                name: "SpecificOutcomes");

            migrationBuilder.DropIndex(
                name: "IX_BroadOutcomes_SubjectId",
                table: "BroadOutcomes");

            migrationBuilder.DropIndex(
                name: "IX_Assessments_SchoolClassId",
                table: "Assessments");

            migrationBuilder.DropColumn(
                name: "SubjectId",
                table: "BroadOutcomes");

            migrationBuilder.DropColumn(
                name: "SchoolClassId",
                table: "Assessments");

            migrationBuilder.RenameColumn(
                name: "SubjectId",
                table: "Strands",
                newName: "EducationLevelSubjectId");

            migrationBuilder.RenameIndex(
                name: "IX_Strands_SubjectId",
                table: "Strands",
                newName: "IX_Strands_EducationLevelSubjectId");

            migrationBuilder.RenameColumn(
                name: "SpecificOutcomeId",
                table: "Assessments",
                newName: "LearningOutcomeId");

            migrationBuilder.RenameIndex(
                name: "IX_Assessments_SpecificOutcomeId",
                table: "Assessments",
                newName: "IX_Assessments_LearningOutcomeId");

            migrationBuilder.CreateTable(
                name: "LearningOutcomes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    BroadOutcomeId = table.Column<int>(type: "int", nullable: false),
                    CompetencyId = table.Column<int>(type: "int", nullable: false),
                    SubStrandId = table.Column<int>(type: "int", nullable: false),
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
                    table.PrimaryKey("PK_LearningOutcomes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LearningOutcomes_BroadOutcomes_BroadOutcomeId",
                        column: x => x.BroadOutcomeId,
                        principalTable: "BroadOutcomes",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_LearningOutcomes_Competencies_CompetencyId",
                        column: x => x.CompetencyId,
                        principalTable: "Competencies",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_LearningOutcomes_SubStrands_SubStrandId",
                        column: x => x.SubStrandId,
                        principalTable: "SubStrands",
                        principalColumn: "Id");
                })
                .Annotation("MySql:CharSet", "utf8mb4");

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

            migrationBuilder.CreateIndex(
                name: "IX_LearningOutcomes_BroadOutcomeId",
                table: "LearningOutcomes",
                column: "BroadOutcomeId");

            migrationBuilder.CreateIndex(
                name: "IX_LearningOutcomes_CompetencyId",
                table: "LearningOutcomes",
                column: "CompetencyId");

            migrationBuilder.CreateIndex(
                name: "IX_LearningOutcomes_SubStrandId",
                table: "LearningOutcomes",
                column: "SubStrandId");

            migrationBuilder.AddForeignKey(
                name: "FK_Assessments_LearningOutcomes_LearningOutcomeId",
                table: "Assessments",
                column: "LearningOutcomeId",
                principalTable: "LearningOutcomes",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Strands_EducationLevelSubjects_EducationLevelSubjectId",
                table: "Strands",
                column: "EducationLevelSubjectId",
                principalTable: "EducationLevelSubjects",
                principalColumn: "Id");
        }
    }
}
