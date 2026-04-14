using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SchoolWebApp.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class SyncmodelwithdatabaseforCBEentities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Schema changes already applied to database manually.
            // This migration only updates the EF model snapshot.

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "269f0cf3-405e-4163-83f3-1b63ebebd62e",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 7, 18, 17, 9, 248, DateTimeKind.Local).AddTicks(2933), new DateTime(2026, 4, 7, 18, 17, 9, 248, DateTimeKind.Local).AddTicks(2937) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "448df289-142c-4959-a912-60733515e1b4",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 7, 18, 17, 9, 248, DateTimeKind.Local).AddTicks(2820), new DateTime(2026, 4, 7, 18, 17, 9, 248, DateTimeKind.Local).AddTicks(2824) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "48c50c3a-9958-453b-b649-4e21af131322",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 7, 18, 17, 9, 248, DateTimeKind.Local).AddTicks(2725), new DateTime(2026, 4, 7, 18, 17, 9, 248, DateTimeKind.Local).AddTicks(2729) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "717d9b15-a428-440c-b26b-08d3bbb68b02",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 7, 18, 17, 9, 248, DateTimeKind.Local).AddTicks(2337), new DateTime(2026, 4, 7, 18, 17, 9, 248, DateTimeKind.Local).AddTicks(2369) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "95ed2407-3e58-4af2-88a4-1c4e96473f68",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 7, 18, 17, 9, 248, DateTimeKind.Local).AddTicks(2614), new DateTime(2026, 4, 7, 18, 17, 9, 248, DateTimeKind.Local).AddTicks(2619) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "97942bee-ef12-4425-8225-4f293d0f36dd",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 7, 18, 17, 9, 248, DateTimeKind.Local).AddTicks(3128), new DateTime(2026, 4, 7, 18, 17, 9, 248, DateTimeKind.Local).AddTicks(3132) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "cd12b44b-103b-48df-8887-a2bf42e0651e",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 7, 18, 17, 9, 248, DateTimeKind.Local).AddTicks(3033), new DateTime(2026, 4, 7, 18, 17, 9, 248, DateTimeKind.Local).AddTicks(3037) });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "7e67d486-af3e-49f1-a109-a2b864b8e0ec",
                columns: new[] { "ConcurrencyStamp", "Created", "Modified", "PasswordHash", "SecurityStamp" },
                values: new object[] { "4566e9eb-5bbd-4751-9cd2-84dad3c2522f", new DateTime(2026, 4, 7, 18, 17, 9, 248, DateTimeKind.Local).AddTicks(4035), new DateTime(2026, 4, 7, 18, 17, 9, 248, DateTimeKind.Local).AddTicks(4041), "AQAAAAIAAYagAAAAEJLjJbVvm/I2lmNgFXLkPAEsxNfAfTjOuus2UdS+V6pVvMP6jLjB7rAUJFqZM3Eo7w==", "80408627-f9c2-4fce-b9ff-9fbd62f6cbcc" });

            migrationBuilder.UpdateData(
                table: "Designations",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 7, 18, 17, 9, 248, DateTimeKind.Local).AddTicks(3297), new DateTime(2026, 4, 7, 18, 17, 9, 248, DateTimeKind.Local).AddTicks(3301) });

            migrationBuilder.UpdateData(
                table: "EmploymentTypes",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 7, 18, 17, 9, 248, DateTimeKind.Local).AddTicks(3392), new DateTime(2026, 4, 7, 18, 17, 9, 248, DateTimeKind.Local).AddTicks(3396) });

            migrationBuilder.UpdateData(
                table: "Genders",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 7, 18, 17, 9, 248, DateTimeKind.Local).AddTicks(3604), new DateTime(2026, 4, 7, 18, 17, 9, 248, DateTimeKind.Local).AddTicks(3607) });

            migrationBuilder.UpdateData(
                table: "Nationalities",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 7, 18, 17, 9, 248, DateTimeKind.Local).AddTicks(3473), new DateTime(2026, 4, 7, 18, 17, 9, 248, DateTimeKind.Local).AddTicks(3477) });

            migrationBuilder.UpdateData(
                table: "Person",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "DateOfBirth", "EmploymentDate", "EndofEmploymentDate", "Modified" },
                values: new object[] { new DateTime(2026, 4, 7, 18, 17, 9, 248, DateTimeKind.Local).AddTicks(3705), new DateTime(2026, 4, 7, 18, 17, 9, 248, DateTimeKind.Local).AddTicks(3700), new DateTime(2026, 4, 7, 18, 17, 9, 248, DateTimeKind.Local).AddTicks(3692), new DateTime(2026, 4, 7, 18, 17, 9, 248, DateTimeKind.Local).AddTicks(3695), new DateTime(2026, 4, 7, 18, 17, 9, 248, DateTimeKind.Local).AddTicks(3714) });

            migrationBuilder.UpdateData(
                table: "Religions",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 7, 18, 17, 9, 248, DateTimeKind.Local).AddTicks(3536), new DateTime(2026, 4, 7, 18, 17, 9, 248, DateTimeKind.Local).AddTicks(3540) });

            migrationBuilder.UpdateData(
                table: "StaffCategories",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 7, 18, 17, 9, 248, DateTimeKind.Local).AddTicks(3221), new DateTime(2026, 4, 7, 18, 17, 9, 248, DateTimeKind.Local).AddTicks(3226) });

        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Strands_Curricula_CurriculumId",
                table: "Strands");

            migrationBuilder.DropForeignKey(
                name: "FK_SubStrands_AcademicYears_AcademicYearId",
                table: "SubStrands");

            migrationBuilder.DropForeignKey(
                name: "FK_SubStrands_Curricula_CurriculumId",
                table: "SubStrands");

            migrationBuilder.DropForeignKey(
                name: "FK_SubStrands_LearningLevels_LearningLevelId",
                table: "SubStrands");

            migrationBuilder.DropForeignKey(
                name: "FK_SubStrands_Subjects_SubjectId",
                table: "SubStrands");

            migrationBuilder.DropTable(
                name: "KeyQuestion");

            migrationBuilder.DropTable(
                name: "LessonAllocation");

            migrationBuilder.DropTable(
                name: "PCI");

            migrationBuilder.DropIndex(
                name: "IX_SubStrands_AcademicYearId",
                table: "SubStrands");

            migrationBuilder.DropIndex(
                name: "IX_SubStrands_CurriculumId",
                table: "SubStrands");

            migrationBuilder.DropIndex(
                name: "IX_SubStrands_LearningLevelId",
                table: "SubStrands");

            migrationBuilder.DropIndex(
                name: "IX_SubStrands_SubjectId",
                table: "SubStrands");

            migrationBuilder.DropIndex(
                name: "IX_Strands_CurriculumId",
                table: "Strands");

            migrationBuilder.DropColumn(
                name: "AcademicYearId",
                table: "SubStrands");

            migrationBuilder.DropColumn(
                name: "CurriculumId",
                table: "SubStrands");

            migrationBuilder.DropColumn(
                name: "LearningLevelId",
                table: "SubStrands");

            migrationBuilder.DropColumn(
                name: "LessonNo",
                table: "SubStrands");

            migrationBuilder.DropColumn(
                name: "SubjectId",
                table: "SubStrands");

            migrationBuilder.DropColumn(
                name: "Version",
                table: "SubStrands");

            migrationBuilder.DropColumn(
                name: "CurriculumId",
                table: "Strands");

            migrationBuilder.RenameColumn(
                name: "Version",
                table: "Strands",
                newName: "AcademicYearId");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "269f0cf3-405e-4163-83f3-1b63ebebd62e",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2025, 9, 27, 22, 0, 4, 515, DateTimeKind.Local).AddTicks(3421), new DateTime(2025, 9, 27, 22, 0, 4, 515, DateTimeKind.Local).AddTicks(3422) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "448df289-142c-4959-a912-60733515e1b4",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2025, 9, 27, 22, 0, 4, 515, DateTimeKind.Local).AddTicks(3402), new DateTime(2025, 9, 27, 22, 0, 4, 515, DateTimeKind.Local).AddTicks(3404) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "48c50c3a-9958-453b-b649-4e21af131322",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2025, 9, 27, 22, 0, 4, 515, DateTimeKind.Local).AddTicks(3384), new DateTime(2025, 9, 27, 22, 0, 4, 515, DateTimeKind.Local).AddTicks(3385) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "717d9b15-a428-440c-b26b-08d3bbb68b02",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2025, 9, 27, 22, 0, 4, 515, DateTimeKind.Local).AddTicks(3243), new DateTime(2025, 9, 27, 22, 0, 4, 515, DateTimeKind.Local).AddTicks(3318) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "95ed2407-3e58-4af2-88a4-1c4e96473f68",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2025, 9, 27, 22, 0, 4, 515, DateTimeKind.Local).AddTicks(3363), new DateTime(2025, 9, 27, 22, 0, 4, 515, DateTimeKind.Local).AddTicks(3365) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "97942bee-ef12-4425-8225-4f293d0f36dd",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2025, 9, 27, 22, 0, 4, 515, DateTimeKind.Local).AddTicks(3469), new DateTime(2025, 9, 27, 22, 0, 4, 515, DateTimeKind.Local).AddTicks(3470) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "cd12b44b-103b-48df-8887-a2bf42e0651e",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2025, 9, 27, 22, 0, 4, 515, DateTimeKind.Local).AddTicks(3441), new DateTime(2025, 9, 27, 22, 0, 4, 515, DateTimeKind.Local).AddTicks(3443) });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "7e67d486-af3e-49f1-a109-a2b864b8e0ec",
                columns: new[] { "ConcurrencyStamp", "Created", "Modified", "PasswordHash", "SecurityStamp" },
                values: new object[] { "1fea64c7-5a76-4a35-a64b-7b1b47951d83", new DateTime(2025, 9, 27, 22, 0, 4, 515, DateTimeKind.Local).AddTicks(3755), new DateTime(2025, 9, 27, 22, 0, 4, 515, DateTimeKind.Local).AddTicks(3756), "AQAAAAIAAYagAAAAEGSYUFU9rnAVLzJPze6FcIT+CMOIUAHJpj/9swvubJvcwYHEYg1U0ycL+sI7iqbnJA==", "2ea82e68-1f65-4b65-b8ec-f7bae2e90471" });

            migrationBuilder.UpdateData(
                table: "Designations",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2025, 9, 27, 22, 0, 4, 515, DateTimeKind.Local).AddTicks(3525), new DateTime(2025, 9, 27, 22, 0, 4, 515, DateTimeKind.Local).AddTicks(3526) });

            migrationBuilder.UpdateData(
                table: "EmploymentTypes",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2025, 9, 27, 22, 0, 4, 515, DateTimeKind.Local).AddTicks(3568), new DateTime(2025, 9, 27, 22, 0, 4, 515, DateTimeKind.Local).AddTicks(3569) });

            migrationBuilder.UpdateData(
                table: "Genders",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2025, 9, 27, 22, 0, 4, 515, DateTimeKind.Local).AddTicks(3629), new DateTime(2025, 9, 27, 22, 0, 4, 515, DateTimeKind.Local).AddTicks(3630) });

            migrationBuilder.UpdateData(
                table: "Nationalities",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2025, 9, 27, 22, 0, 4, 515, DateTimeKind.Local).AddTicks(3593), new DateTime(2025, 9, 27, 22, 0, 4, 515, DateTimeKind.Local).AddTicks(3593) });

            migrationBuilder.UpdateData(
                table: "Person",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "DateOfBirth", "EmploymentDate", "EndofEmploymentDate", "Modified" },
                values: new object[] { new DateTime(2025, 9, 27, 22, 0, 4, 515, DateTimeKind.Local).AddTicks(3675), new DateTime(2025, 9, 27, 22, 0, 4, 515, DateTimeKind.Local).AddTicks(3665), new DateTime(2025, 9, 27, 22, 0, 4, 515, DateTimeKind.Local).AddTicks(3661), new DateTime(2025, 9, 27, 22, 0, 4, 515, DateTimeKind.Local).AddTicks(3662), new DateTime(2025, 9, 27, 22, 0, 4, 515, DateTimeKind.Local).AddTicks(3695) });

            migrationBuilder.UpdateData(
                table: "Religions",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2025, 9, 27, 22, 0, 4, 515, DateTimeKind.Local).AddTicks(3610), new DateTime(2025, 9, 27, 22, 0, 4, 515, DateTimeKind.Local).AddTicks(3611) });

            migrationBuilder.UpdateData(
                table: "StaffCategories",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2025, 9, 27, 22, 0, 4, 515, DateTimeKind.Local).AddTicks(3500), new DateTime(2025, 9, 27, 22, 0, 4, 515, DateTimeKind.Local).AddTicks(3503) });

            migrationBuilder.CreateIndex(
                name: "IX_Strands_AcademicYearId",
                table: "Strands",
                column: "AcademicYearId");

            migrationBuilder.AddForeignKey(
                name: "FK_Strands_AcademicYears_AcademicYearId",
                table: "Strands",
                column: "AcademicYearId",
                principalTable: "AcademicYears",
                principalColumn: "Id");
        }
    }
}
