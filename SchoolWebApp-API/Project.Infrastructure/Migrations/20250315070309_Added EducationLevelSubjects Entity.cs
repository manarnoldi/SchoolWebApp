using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SchoolWebApp.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddedEducationLevelSubjectsEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "EducationLevelSubjects",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Description = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    EducationLevelId = table.Column<int>(type: "int", nullable: false),
                    SubjectId = table.Column<int>(type: "int", nullable: false),
                    AcademicYearId = table.Column<int>(type: "int", nullable: false),
                    Created = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    CreatedBy = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Modified = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    ModifiedBy = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EducationLevelSubjects", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EducationLevelSubjects_AcademicYears_AcademicYearId",
                        column: x => x.AcademicYearId,
                        principalTable: "AcademicYears",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_EducationLevelSubjects_EducationLevels_EducationLevelId",
                        column: x => x.EducationLevelId,
                        principalTable: "EducationLevels",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_EducationLevelSubjects_Subjects_SubjectId",
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
                values: new object[] { new DateTime(2025, 3, 15, 10, 3, 8, 215, DateTimeKind.Local).AddTicks(6504), new DateTime(2025, 3, 15, 10, 3, 8, 215, DateTimeKind.Local).AddTicks(6505) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "448df289-142c-4959-a912-60733515e1b4",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2025, 3, 15, 10, 3, 8, 215, DateTimeKind.Local).AddTicks(6477), new DateTime(2025, 3, 15, 10, 3, 8, 215, DateTimeKind.Local).AddTicks(6479) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "48c50c3a-9958-453b-b649-4e21af131322",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2025, 3, 15, 10, 3, 8, 215, DateTimeKind.Local).AddTicks(6451), new DateTime(2025, 3, 15, 10, 3, 8, 215, DateTimeKind.Local).AddTicks(6452) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "717d9b15-a428-440c-b26b-08d3bbb68b02",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2025, 3, 15, 10, 3, 8, 215, DateTimeKind.Local).AddTicks(6299), new DateTime(2025, 3, 15, 10, 3, 8, 215, DateTimeKind.Local).AddTicks(6334) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "95ed2407-3e58-4af2-88a4-1c4e96473f68",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2025, 3, 15, 10, 3, 8, 215, DateTimeKind.Local).AddTicks(6412), new DateTime(2025, 3, 15, 10, 3, 8, 215, DateTimeKind.Local).AddTicks(6423) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "97942bee-ef12-4425-8225-4f293d0f36dd",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2025, 3, 15, 10, 3, 8, 215, DateTimeKind.Local).AddTicks(6594), new DateTime(2025, 3, 15, 10, 3, 8, 215, DateTimeKind.Local).AddTicks(6596) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "cd12b44b-103b-48df-8887-a2bf42e0651e",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2025, 3, 15, 10, 3, 8, 215, DateTimeKind.Local).AddTicks(6536), new DateTime(2025, 3, 15, 10, 3, 8, 215, DateTimeKind.Local).AddTicks(6547) });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "7e67d486-af3e-49f1-a109-a2b864b8e0ec",
                columns: new[] { "ConcurrencyStamp", "Created", "Modified", "PasswordHash", "SecurityStamp" },
                values: new object[] { "412c0f9a-92bd-4f9f-8f3a-71643f3b1564", new DateTime(2025, 3, 15, 10, 3, 8, 215, DateTimeKind.Local).AddTicks(6770), new DateTime(2025, 3, 15, 10, 3, 8, 215, DateTimeKind.Local).AddTicks(6772), "AQAAAAIAAYagAAAAECK9c78oA0vWPIukIojad1GNkmdW300fOPc0CgPjWre8HFNjgWhTDhI07WakhlUqzg==", "c896fa23-3919-46f5-95e8-5c549a7b23ad" });

            migrationBuilder.CreateIndex(
                name: "IX_EducationLevelSubjects_AcademicYearId",
                table: "EducationLevelSubjects",
                column: "AcademicYearId");

            migrationBuilder.CreateIndex(
                name: "IX_EducationLevelSubjects_EducationLevelId",
                table: "EducationLevelSubjects",
                column: "EducationLevelId");

            migrationBuilder.CreateIndex(
                name: "IX_EducationLevelSubjects_SubjectId",
                table: "EducationLevelSubjects",
                column: "SubjectId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EducationLevelSubjects");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "269f0cf3-405e-4163-83f3-1b63ebebd62e",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2025, 3, 14, 21, 48, 56, 164, DateTimeKind.Local).AddTicks(4386), new DateTime(2025, 3, 14, 21, 48, 56, 164, DateTimeKind.Local).AddTicks(4398) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "448df289-142c-4959-a912-60733515e1b4",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2025, 3, 14, 21, 48, 56, 164, DateTimeKind.Local).AddTicks(4360), new DateTime(2025, 3, 14, 21, 48, 56, 164, DateTimeKind.Local).AddTicks(4361) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "48c50c3a-9958-453b-b649-4e21af131322",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2025, 3, 14, 21, 48, 56, 164, DateTimeKind.Local).AddTicks(4319), new DateTime(2025, 3, 14, 21, 48, 56, 164, DateTimeKind.Local).AddTicks(4320) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "717d9b15-a428-440c-b26b-08d3bbb68b02",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2025, 3, 14, 21, 48, 56, 164, DateTimeKind.Local).AddTicks(4205), new DateTime(2025, 3, 14, 21, 48, 56, 164, DateTimeKind.Local).AddTicks(4238) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "95ed2407-3e58-4af2-88a4-1c4e96473f68",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2025, 3, 14, 21, 48, 56, 164, DateTimeKind.Local).AddTicks(4289), new DateTime(2025, 3, 14, 21, 48, 56, 164, DateTimeKind.Local).AddTicks(4291) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "97942bee-ef12-4425-8225-4f293d0f36dd",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2025, 3, 14, 21, 48, 56, 164, DateTimeKind.Local).AddTicks(4466), new DateTime(2025, 3, 14, 21, 48, 56, 164, DateTimeKind.Local).AddTicks(4467) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "cd12b44b-103b-48df-8887-a2bf42e0651e",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2025, 3, 14, 21, 48, 56, 164, DateTimeKind.Local).AddTicks(4437), new DateTime(2025, 3, 14, 21, 48, 56, 164, DateTimeKind.Local).AddTicks(4439) });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "7e67d486-af3e-49f1-a109-a2b864b8e0ec",
                columns: new[] { "ConcurrencyStamp", "Created", "Modified", "PasswordHash", "SecurityStamp" },
                values: new object[] { "8f3292ca-3f70-48bd-803d-16e47b6c3541", new DateTime(2025, 3, 14, 21, 48, 56, 164, DateTimeKind.Local).AddTicks(4634), new DateTime(2025, 3, 14, 21, 48, 56, 164, DateTimeKind.Local).AddTicks(4635), "AQAAAAIAAYagAAAAEJhf8T6kORhJQ/nZo3Cc/f/ih8nhIATXOrmIQySWHCloUW52sXrz3SAFICqIArpX6w==", "b23689a4-b0d8-403d-a146-ed650312e6a3" });
        }
    }
}
