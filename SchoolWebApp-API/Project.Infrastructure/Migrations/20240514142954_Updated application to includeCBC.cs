using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace SchoolWebApp.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdatedapplicationtoincludeCBC : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FormerSchools_SchoolLevels_SchoolLevelId",
                table: "FormerSchools");

            migrationBuilder.DropForeignKey(
                name: "FK_SchoolClasses_Curricula_CurriculumId",
                table: "SchoolClasses");

            migrationBuilder.DropForeignKey(
                name: "FK_SchoolDetails_SchoolLevels_SchoolLevelId",
                table: "SchoolDetails");

            migrationBuilder.DropForeignKey(
                name: "FK_Subjects_Curricula_CurriculumId",
                table: "Subjects");

            migrationBuilder.DropTable(
                name: "SchoolLevels");

            migrationBuilder.DropTable(
                name: "StaffClasses");

            migrationBuilder.DropIndex(
                name: "IX_Subjects_CurriculumId",
                table: "Subjects");

            migrationBuilder.DropIndex(
                name: "IX_SchoolDetails_SchoolLevelId",
                table: "SchoolDetails");

            migrationBuilder.DropColumn(
                name: "CurriculumId",
                table: "Subjects");

            migrationBuilder.DropColumn(
                name: "SchoolLevelId",
                table: "SchoolDetails");

            migrationBuilder.DropColumn(
                name: "ClassName",
                table: "SchoolClasses");

            migrationBuilder.DropColumn(
                name: "Stream",
                table: "SchoolClasses");

            migrationBuilder.RenameColumn(
                name: "CurriculumId",
                table: "SchoolClasses",
                newName: "StaffDetailsId");

            migrationBuilder.RenameIndex(
                name: "IX_SchoolClasses_CurriculumId",
                table: "SchoolClasses",
                newName: "IX_SchoolClasses_StaffDetailsId");

            migrationBuilder.RenameColumn(
                name: "SchoolLevelId",
                table: "FormerSchools",
                newName: "EducationLevelId");

            migrationBuilder.RenameIndex(
                name: "IX_FormerSchools_SchoolLevelId",
                table: "FormerSchools",
                newName: "IX_FormerSchools_EducationLevelId");

            migrationBuilder.AddColumn<int>(
                name: "CurriculumId",
                table: "SubjectGroups",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "SchoolLevelsAvailable",
                table: "SchoolDetails",
                type: "varchar(255)",
                maxLength: 255,
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "SchoolClasses",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<int>(
                name: "LearningLevelId",
                table: "SchoolClasses",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "SchoolClasses",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<int>(
                name: "SchoolStreamId",
                table: "SchoolClasses",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "EducationLevelTypes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<int>(type: "int", maxLength: 255, nullable: false),
                    Abbr = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Description = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Created = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    CreatedBy = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Modified = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    ModifiedBy = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EducationLevelTypes", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "SchoolStreams",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Description = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Created = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    CreatedBy = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Modified = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    ModifiedBy = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SchoolStreams", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "EducationLevels",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Abbr = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    NumOfYears = table.Column<int>(type: "int", nullable: false),
                    Description = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    EducationLevelTypeId = table.Column<int>(type: "int", nullable: false),
                    CurriculumId = table.Column<int>(type: "int", nullable: false),
                    Created = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    CreatedBy = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Modified = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    ModifiedBy = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EducationLevels", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EducationLevels_Curricula_CurriculumId",
                        column: x => x.CurriculumId,
                        principalTable: "Curricula",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_EducationLevels_EducationLevelTypes_EducationLevelTypeId",
                        column: x => x.EducationLevelTypeId,
                        principalTable: "EducationLevelTypes",
                        principalColumn: "Id");
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "LearningLevels",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Description = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    EducationLevelId = table.Column<int>(type: "int", nullable: false),
                    Created = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    CreatedBy = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Modified = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    ModifiedBy = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LearningLevels", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LearningLevels_EducationLevels_EducationLevelId",
                        column: x => x.EducationLevelId,
                        principalTable: "EducationLevels",
                        principalColumn: "Id");
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "269f0cf3-405e-4163-83f3-1b63ebebd62e",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2024, 5, 14, 17, 29, 52, 550, DateTimeKind.Local).AddTicks(5164), new DateTime(2024, 5, 14, 17, 29, 52, 550, DateTimeKind.Local).AddTicks(5165) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "448df289-142c-4959-a912-60733515e1b4",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2024, 5, 14, 17, 29, 52, 550, DateTimeKind.Local).AddTicks(5139), new DateTime(2024, 5, 14, 17, 29, 52, 550, DateTimeKind.Local).AddTicks(5140) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "48c50c3a-9958-453b-b649-4e21af131322",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2024, 5, 14, 17, 29, 52, 550, DateTimeKind.Local).AddTicks(5111), new DateTime(2024, 5, 14, 17, 29, 52, 550, DateTimeKind.Local).AddTicks(5112) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "717d9b15-a428-440c-b26b-08d3bbb68b02",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2024, 5, 14, 17, 29, 52, 550, DateTimeKind.Local).AddTicks(4984), new DateTime(2024, 5, 14, 17, 29, 52, 550, DateTimeKind.Local).AddTicks(5014) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "95ed2407-3e58-4af2-88a4-1c4e96473f68",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2024, 5, 14, 17, 29, 52, 550, DateTimeKind.Local).AddTicks(5073), new DateTime(2024, 5, 14, 17, 29, 52, 550, DateTimeKind.Local).AddTicks(5084) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "97942bee-ef12-4425-8225-4f293d0f36dd",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2024, 5, 14, 17, 29, 52, 550, DateTimeKind.Local).AddTicks(5253), new DateTime(2024, 5, 14, 17, 29, 52, 550, DateTimeKind.Local).AddTicks(5255) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "cd12b44b-103b-48df-8887-a2bf42e0651e",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2024, 5, 14, 17, 29, 52, 550, DateTimeKind.Local).AddTicks(5194), new DateTime(2024, 5, 14, 17, 29, 52, 550, DateTimeKind.Local).AddTicks(5204) });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "7e67d486-af3e-49f1-a109-a2b864b8e0ec",
                columns: new[] { "ConcurrencyStamp", "Created", "Modified", "PasswordHash", "SecurityStamp" },
                values: new object[] { "ac40a9f3-274f-4ff5-9202-779d46cd640c", new DateTime(2024, 5, 14, 17, 29, 52, 550, DateTimeKind.Local).AddTicks(5352), new DateTime(2024, 5, 14, 17, 29, 52, 550, DateTimeKind.Local).AddTicks(5353), "AQAAAAIAAYagAAAAEJB+R3CRrTOkMpAZQLYdZMGozdizrxCy0wCLYO9ejzvbvziGd7GsNjZKqWqY0lUknA==", "d62dbd4a-f799-4645-bf71-a992386f43c1" });

            migrationBuilder.CreateIndex(
                name: "IX_SubjectGroups_CurriculumId",
                table: "SubjectGroups",
                column: "CurriculumId");

            migrationBuilder.CreateIndex(
                name: "IX_SchoolClasses_LearningLevelId",
                table: "SchoolClasses",
                column: "LearningLevelId");

            migrationBuilder.CreateIndex(
                name: "IX_SchoolClasses_SchoolStreamId",
                table: "SchoolClasses",
                column: "SchoolStreamId");

            migrationBuilder.CreateIndex(
                name: "IX_EducationLevels_CurriculumId",
                table: "EducationLevels",
                column: "CurriculumId");

            migrationBuilder.CreateIndex(
                name: "IX_EducationLevels_EducationLevelTypeId",
                table: "EducationLevels",
                column: "EducationLevelTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_LearningLevels_EducationLevelId",
                table: "LearningLevels",
                column: "EducationLevelId");

            migrationBuilder.AddForeignKey(
                name: "FK_FormerSchools_EducationLevels_EducationLevelId",
                table: "FormerSchools",
                column: "EducationLevelId",
                principalTable: "EducationLevels",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_SchoolClasses_LearningLevels_LearningLevelId",
                table: "SchoolClasses",
                column: "LearningLevelId",
                principalTable: "LearningLevels",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_SchoolClasses_Person_StaffDetailsId",
                table: "SchoolClasses",
                column: "StaffDetailsId",
                principalTable: "Person",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_SchoolClasses_SchoolStreams_SchoolStreamId",
                table: "SchoolClasses",
                column: "SchoolStreamId",
                principalTable: "SchoolStreams",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_SubjectGroups_Curricula_CurriculumId",
                table: "SubjectGroups",
                column: "CurriculumId",
                principalTable: "Curricula",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FormerSchools_EducationLevels_EducationLevelId",
                table: "FormerSchools");

            migrationBuilder.DropForeignKey(
                name: "FK_SchoolClasses_LearningLevels_LearningLevelId",
                table: "SchoolClasses");

            migrationBuilder.DropForeignKey(
                name: "FK_SchoolClasses_Person_StaffDetailsId",
                table: "SchoolClasses");

            migrationBuilder.DropForeignKey(
                name: "FK_SchoolClasses_SchoolStreams_SchoolStreamId",
                table: "SchoolClasses");

            migrationBuilder.DropForeignKey(
                name: "FK_SubjectGroups_Curricula_CurriculumId",
                table: "SubjectGroups");

            migrationBuilder.DropTable(
                name: "LearningLevels");

            migrationBuilder.DropTable(
                name: "SchoolStreams");

            migrationBuilder.DropTable(
                name: "EducationLevels");

            migrationBuilder.DropTable(
                name: "EducationLevelTypes");

            migrationBuilder.DropIndex(
                name: "IX_SubjectGroups_CurriculumId",
                table: "SubjectGroups");

            migrationBuilder.DropIndex(
                name: "IX_SchoolClasses_LearningLevelId",
                table: "SchoolClasses");

            migrationBuilder.DropIndex(
                name: "IX_SchoolClasses_SchoolStreamId",
                table: "SchoolClasses");

            migrationBuilder.DropColumn(
                name: "CurriculumId",
                table: "SubjectGroups");

            migrationBuilder.DropColumn(
                name: "SchoolLevelsAvailable",
                table: "SchoolDetails");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "SchoolClasses");

            migrationBuilder.DropColumn(
                name: "LearningLevelId",
                table: "SchoolClasses");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "SchoolClasses");

            migrationBuilder.DropColumn(
                name: "SchoolStreamId",
                table: "SchoolClasses");

            migrationBuilder.RenameColumn(
                name: "StaffDetailsId",
                table: "SchoolClasses",
                newName: "CurriculumId");

            migrationBuilder.RenameIndex(
                name: "IX_SchoolClasses_StaffDetailsId",
                table: "SchoolClasses",
                newName: "IX_SchoolClasses_CurriculumId");

            migrationBuilder.RenameColumn(
                name: "EducationLevelId",
                table: "FormerSchools",
                newName: "SchoolLevelId");

            migrationBuilder.RenameIndex(
                name: "IX_FormerSchools_EducationLevelId",
                table: "FormerSchools",
                newName: "IX_FormerSchools_SchoolLevelId");

            migrationBuilder.AddColumn<int>(
                name: "CurriculumId",
                table: "Subjects",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "SchoolLevelId",
                table: "SchoolDetails",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "ClassName",
                table: "SchoolClasses",
                type: "varchar(255)",
                maxLength: 255,
                nullable: false,
                defaultValue: "")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "Stream",
                table: "SchoolClasses",
                type: "varchar(255)",
                maxLength: 255,
                nullable: false,
                defaultValue: "")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "SchoolLevels",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Created = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    CreatedBy = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Description = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Modified = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    ModifiedBy = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Name = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SchoolLevels", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "StaffClasses",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    SchoolClassId = table.Column<int>(type: "int", nullable: false),
                    StaffDetailsId = table.Column<int>(type: "int", nullable: false),
                    Created = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    CreatedBy = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Modified = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    ModifiedBy = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Remarks = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StaffClasses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StaffClasses_Person_StaffDetailsId",
                        column: x => x.StaffDetailsId,
                        principalTable: "Person",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_StaffClasses_SchoolClasses_SchoolClassId",
                        column: x => x.SchoolClassId,
                        principalTable: "SchoolClasses",
                        principalColumn: "Id");
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "269f0cf3-405e-4163-83f3-1b63ebebd62e",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2024, 4, 4, 13, 4, 5, 187, DateTimeKind.Local).AddTicks(9935), new DateTime(2024, 4, 4, 13, 4, 5, 187, DateTimeKind.Local).AddTicks(9936) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "448df289-142c-4959-a912-60733515e1b4",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2024, 4, 4, 13, 4, 5, 187, DateTimeKind.Local).AddTicks(9906), new DateTime(2024, 4, 4, 13, 4, 5, 187, DateTimeKind.Local).AddTicks(9907) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "48c50c3a-9958-453b-b649-4e21af131322",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2024, 4, 4, 13, 4, 5, 187, DateTimeKind.Local).AddTicks(9851), new DateTime(2024, 4, 4, 13, 4, 5, 187, DateTimeKind.Local).AddTicks(9868) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "717d9b15-a428-440c-b26b-08d3bbb68b02",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2024, 4, 4, 13, 4, 5, 187, DateTimeKind.Local).AddTicks(9723), new DateTime(2024, 4, 4, 13, 4, 5, 187, DateTimeKind.Local).AddTicks(9752) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "95ed2407-3e58-4af2-88a4-1c4e96473f68",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2024, 4, 4, 13, 4, 5, 187, DateTimeKind.Local).AddTicks(9819), new DateTime(2024, 4, 4, 13, 4, 5, 187, DateTimeKind.Local).AddTicks(9820) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "97942bee-ef12-4425-8225-4f293d0f36dd",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2024, 4, 4, 13, 4, 5, 187, DateTimeKind.Local).AddTicks(9994), new DateTime(2024, 4, 4, 13, 4, 5, 187, DateTimeKind.Local).AddTicks(9995) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "cd12b44b-103b-48df-8887-a2bf42e0651e",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2024, 4, 4, 13, 4, 5, 187, DateTimeKind.Local).AddTicks(9963), new DateTime(2024, 4, 4, 13, 4, 5, 187, DateTimeKind.Local).AddTicks(9964) });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "7e67d486-af3e-49f1-a109-a2b864b8e0ec",
                columns: new[] { "ConcurrencyStamp", "Created", "Modified", "PasswordHash", "SecurityStamp" },
                values: new object[] { "483db0dd-d9b2-4c27-84e7-7b69f5c60a32", new DateTime(2024, 4, 4, 13, 4, 5, 188, DateTimeKind.Local).AddTicks(121), new DateTime(2024, 4, 4, 13, 4, 5, 188, DateTimeKind.Local).AddTicks(122), "AQAAAAIAAYagAAAAEGtcxKiMn5cj/4VPXFxq0GJLXpDFyRT5jUdVKCqefQzEv1L+VljrOOQfoE7uITaP1g==", "b056fcce-e242-4777-9b6b-21344f2d6ee1" });

            migrationBuilder.InsertData(
                table: "SchoolLevels",
                columns: new[] { "Id", "Created", "CreatedBy", "Description", "Modified", "ModifiedBy", "Name" },
                values: new object[,]
                {
                    { 1, new DateTime(2024, 4, 4, 13, 4, 5, 272, DateTimeKind.Local).AddTicks(3150), "admin", "A level for primary schools", new DateTime(2024, 4, 4, 13, 4, 5, 272, DateTimeKind.Local).AddTicks(3176), "admin", "Primary" },
                    { 2, new DateTime(2024, 4, 4, 13, 4, 5, 272, DateTimeKind.Local).AddTicks(3178), "admin", "A level for secondary schools", new DateTime(2024, 4, 4, 13, 4, 5, 272, DateTimeKind.Local).AddTicks(3179), "admin", "Secondary" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Subjects_CurriculumId",
                table: "Subjects",
                column: "CurriculumId");

            migrationBuilder.CreateIndex(
                name: "IX_SchoolDetails_SchoolLevelId",
                table: "SchoolDetails",
                column: "SchoolLevelId");

            migrationBuilder.CreateIndex(
                name: "IX_StaffClasses_SchoolClassId",
                table: "StaffClasses",
                column: "SchoolClassId");

            migrationBuilder.CreateIndex(
                name: "IX_StaffClasses_StaffDetailsId",
                table: "StaffClasses",
                column: "StaffDetailsId");

            migrationBuilder.AddForeignKey(
                name: "FK_FormerSchools_SchoolLevels_SchoolLevelId",
                table: "FormerSchools",
                column: "SchoolLevelId",
                principalTable: "SchoolLevels",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_SchoolClasses_Curricula_CurriculumId",
                table: "SchoolClasses",
                column: "CurriculumId",
                principalTable: "Curricula",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_SchoolDetails_SchoolLevels_SchoolLevelId",
                table: "SchoolDetails",
                column: "SchoolLevelId",
                principalTable: "SchoolLevels",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Subjects_Curricula_CurriculumId",
                table: "Subjects",
                column: "CurriculumId",
                principalTable: "Curricula",
                principalColumn: "Id");
        }
    }
}
