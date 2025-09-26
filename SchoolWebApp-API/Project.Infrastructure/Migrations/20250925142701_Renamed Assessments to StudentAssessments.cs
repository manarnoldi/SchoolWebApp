using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SchoolWebApp.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RenamedAssessmentstoStudentAssessments : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Assessments_AssessmentTypes_AssessmentTypeId",
                table: "Assessments");

            migrationBuilder.DropForeignKey(
                name: "FK_Assessments_Grades_GradeId",
                table: "Assessments");

            migrationBuilder.DropForeignKey(
                name: "FK_Assessments_Person_StudentId",
                table: "Assessments");

            migrationBuilder.DropForeignKey(
                name: "FK_Assessments_SchoolClasses_SchoolClassId",
                table: "Assessments");

            migrationBuilder.DropForeignKey(
                name: "FK_Assessments_Sessions_SessionId",
                table: "Assessments");

            migrationBuilder.DropForeignKey(
                name: "FK_Assessments_SpecificOutcomes_SpecificOutcomeId",
                table: "Assessments");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Assessments",
                table: "Assessments");

            migrationBuilder.RenameTable(
                name: "Assessments",
                newName: "StudentAssessments");

            migrationBuilder.RenameIndex(
                name: "IX_Assessments_StudentId",
                table: "StudentAssessments",
                newName: "IX_StudentAssessments_StudentId");

            migrationBuilder.RenameIndex(
                name: "IX_Assessments_SpecificOutcomeId",
                table: "StudentAssessments",
                newName: "IX_StudentAssessments_SpecificOutcomeId");

            migrationBuilder.RenameIndex(
                name: "IX_Assessments_SessionId",
                table: "StudentAssessments",
                newName: "IX_StudentAssessments_SessionId");

            migrationBuilder.RenameIndex(
                name: "IX_Assessments_SchoolClassId",
                table: "StudentAssessments",
                newName: "IX_StudentAssessments_SchoolClassId");

            migrationBuilder.RenameIndex(
                name: "IX_Assessments_GradeId",
                table: "StudentAssessments",
                newName: "IX_StudentAssessments_GradeId");

            migrationBuilder.RenameIndex(
                name: "IX_Assessments_AssessmentTypeId",
                table: "StudentAssessments",
                newName: "IX_StudentAssessments_AssessmentTypeId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_StudentAssessments",
                table: "StudentAssessments",
                column: "Id");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "269f0cf3-405e-4163-83f3-1b63ebebd62e",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2025, 9, 25, 17, 27, 0, 864, DateTimeKind.Local).AddTicks(1787), new DateTime(2025, 9, 25, 17, 27, 0, 864, DateTimeKind.Local).AddTicks(1788) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "448df289-142c-4959-a912-60733515e1b4",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2025, 9, 25, 17, 27, 0, 864, DateTimeKind.Local).AddTicks(1750), new DateTime(2025, 9, 25, 17, 27, 0, 864, DateTimeKind.Local).AddTicks(1752) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "48c50c3a-9958-453b-b649-4e21af131322",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2025, 9, 25, 17, 27, 0, 864, DateTimeKind.Local).AddTicks(1731), new DateTime(2025, 9, 25, 17, 27, 0, 864, DateTimeKind.Local).AddTicks(1733) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "717d9b15-a428-440c-b26b-08d3bbb68b02",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2025, 9, 25, 17, 27, 0, 864, DateTimeKind.Local).AddTicks(1659), new DateTime(2025, 9, 25, 17, 27, 0, 864, DateTimeKind.Local).AddTicks(1672) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "95ed2407-3e58-4af2-88a4-1c4e96473f68",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2025, 9, 25, 17, 27, 0, 864, DateTimeKind.Local).AddTicks(1711), new DateTime(2025, 9, 25, 17, 27, 0, 864, DateTimeKind.Local).AddTicks(1712) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "97942bee-ef12-4425-8225-4f293d0f36dd",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2025, 9, 25, 17, 27, 0, 864, DateTimeKind.Local).AddTicks(1824), new DateTime(2025, 9, 25, 17, 27, 0, 864, DateTimeKind.Local).AddTicks(1826) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "cd12b44b-103b-48df-8887-a2bf42e0651e",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2025, 9, 25, 17, 27, 0, 864, DateTimeKind.Local).AddTicks(1807), new DateTime(2025, 9, 25, 17, 27, 0, 864, DateTimeKind.Local).AddTicks(1808) });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "7e67d486-af3e-49f1-a109-a2b864b8e0ec",
                columns: new[] { "ConcurrencyStamp", "Created", "Modified", "PasswordHash", "SecurityStamp" },
                values: new object[] { "4f3a0b61-d15a-4f11-8d73-1ebc886a2537", new DateTime(2025, 9, 25, 17, 27, 0, 864, DateTimeKind.Local).AddTicks(2016), new DateTime(2025, 9, 25, 17, 27, 0, 864, DateTimeKind.Local).AddTicks(2018), "AQAAAAIAAYagAAAAEBaSGwPuo+YAp45z8P/NGSwOgaj8tgpMsezv7pDUK8SyFZdbyaOHVveQRQBGsLnwCQ==", "fa010c75-d5b7-40c7-a8a1-52218c4e12f6" });

            migrationBuilder.UpdateData(
                table: "Designations",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2025, 9, 25, 17, 27, 0, 864, DateTimeKind.Local).AddTicks(1869), new DateTime(2025, 9, 25, 17, 27, 0, 864, DateTimeKind.Local).AddTicks(1869) });

            migrationBuilder.UpdateData(
                table: "EmploymentTypes",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2025, 9, 25, 17, 27, 0, 864, DateTimeKind.Local).AddTicks(1889), new DateTime(2025, 9, 25, 17, 27, 0, 864, DateTimeKind.Local).AddTicks(1889) });

            migrationBuilder.UpdateData(
                table: "Genders",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2025, 9, 25, 17, 27, 0, 864, DateTimeKind.Local).AddTicks(1936), new DateTime(2025, 9, 25, 17, 27, 0, 864, DateTimeKind.Local).AddTicks(1937) });

            migrationBuilder.UpdateData(
                table: "Nationalities",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2025, 9, 25, 17, 27, 0, 864, DateTimeKind.Local).AddTicks(1907), new DateTime(2025, 9, 25, 17, 27, 0, 864, DateTimeKind.Local).AddTicks(1907) });

            migrationBuilder.UpdateData(
                table: "Person",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "DateOfBirth", "EmploymentDate", "EndofEmploymentDate", "Modified" },
                values: new object[] { new DateTime(2025, 9, 25, 17, 27, 0, 864, DateTimeKind.Local).AddTicks(1963), new DateTime(2025, 9, 25, 17, 27, 0, 864, DateTimeKind.Local).AddTicks(1960), new DateTime(2025, 9, 25, 17, 27, 0, 864, DateTimeKind.Local).AddTicks(1957), new DateTime(2025, 9, 25, 17, 27, 0, 864, DateTimeKind.Local).AddTicks(1958), new DateTime(2025, 9, 25, 17, 27, 0, 864, DateTimeKind.Local).AddTicks(1963) });

            migrationBuilder.UpdateData(
                table: "Religions",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2025, 9, 25, 17, 27, 0, 864, DateTimeKind.Local).AddTicks(1922), new DateTime(2025, 9, 25, 17, 27, 0, 864, DateTimeKind.Local).AddTicks(1923) });

            migrationBuilder.UpdateData(
                table: "StaffCategories",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2025, 9, 25, 17, 27, 0, 864, DateTimeKind.Local).AddTicks(1852), new DateTime(2025, 9, 25, 17, 27, 0, 864, DateTimeKind.Local).AddTicks(1853) });

            migrationBuilder.AddForeignKey(
                name: "FK_StudentAssessments_AssessmentTypes_AssessmentTypeId",
                table: "StudentAssessments",
                column: "AssessmentTypeId",
                principalTable: "AssessmentTypes",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_StudentAssessments_Grades_GradeId",
                table: "StudentAssessments",
                column: "GradeId",
                principalTable: "Grades",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_StudentAssessments_Person_StudentId",
                table: "StudentAssessments",
                column: "StudentId",
                principalTable: "Person",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_StudentAssessments_SchoolClasses_SchoolClassId",
                table: "StudentAssessments",
                column: "SchoolClassId",
                principalTable: "SchoolClasses",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_StudentAssessments_Sessions_SessionId",
                table: "StudentAssessments",
                column: "SessionId",
                principalTable: "Sessions",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_StudentAssessments_SpecificOutcomes_SpecificOutcomeId",
                table: "StudentAssessments",
                column: "SpecificOutcomeId",
                principalTable: "SpecificOutcomes",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_StudentAssessments_AssessmentTypes_AssessmentTypeId",
                table: "StudentAssessments");

            migrationBuilder.DropForeignKey(
                name: "FK_StudentAssessments_Grades_GradeId",
                table: "StudentAssessments");

            migrationBuilder.DropForeignKey(
                name: "FK_StudentAssessments_Person_StudentId",
                table: "StudentAssessments");

            migrationBuilder.DropForeignKey(
                name: "FK_StudentAssessments_SchoolClasses_SchoolClassId",
                table: "StudentAssessments");

            migrationBuilder.DropForeignKey(
                name: "FK_StudentAssessments_Sessions_SessionId",
                table: "StudentAssessments");

            migrationBuilder.DropForeignKey(
                name: "FK_StudentAssessments_SpecificOutcomes_SpecificOutcomeId",
                table: "StudentAssessments");

            migrationBuilder.DropPrimaryKey(
                name: "PK_StudentAssessments",
                table: "StudentAssessments");

            migrationBuilder.RenameTable(
                name: "StudentAssessments",
                newName: "Assessments");

            migrationBuilder.RenameIndex(
                name: "IX_StudentAssessments_StudentId",
                table: "Assessments",
                newName: "IX_Assessments_StudentId");

            migrationBuilder.RenameIndex(
                name: "IX_StudentAssessments_SpecificOutcomeId",
                table: "Assessments",
                newName: "IX_Assessments_SpecificOutcomeId");

            migrationBuilder.RenameIndex(
                name: "IX_StudentAssessments_SessionId",
                table: "Assessments",
                newName: "IX_Assessments_SessionId");

            migrationBuilder.RenameIndex(
                name: "IX_StudentAssessments_SchoolClassId",
                table: "Assessments",
                newName: "IX_Assessments_SchoolClassId");

            migrationBuilder.RenameIndex(
                name: "IX_StudentAssessments_GradeId",
                table: "Assessments",
                newName: "IX_Assessments_GradeId");

            migrationBuilder.RenameIndex(
                name: "IX_StudentAssessments_AssessmentTypeId",
                table: "Assessments",
                newName: "IX_Assessments_AssessmentTypeId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Assessments",
                table: "Assessments",
                column: "Id");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "269f0cf3-405e-4163-83f3-1b63ebebd62e",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2025, 9, 25, 16, 59, 1, 423, DateTimeKind.Local).AddTicks(7400), new DateTime(2025, 9, 25, 16, 59, 1, 423, DateTimeKind.Local).AddTicks(7402) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "448df289-142c-4959-a912-60733515e1b4",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2025, 9, 25, 16, 59, 1, 423, DateTimeKind.Local).AddTicks(7384), new DateTime(2025, 9, 25, 16, 59, 1, 423, DateTimeKind.Local).AddTicks(7386) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "48c50c3a-9958-453b-b649-4e21af131322",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2025, 9, 25, 16, 59, 1, 423, DateTimeKind.Local).AddTicks(7366), new DateTime(2025, 9, 25, 16, 59, 1, 423, DateTimeKind.Local).AddTicks(7368) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "717d9b15-a428-440c-b26b-08d3bbb68b02",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2025, 9, 25, 16, 59, 1, 423, DateTimeKind.Local).AddTicks(7291), new DateTime(2025, 9, 25, 16, 59, 1, 423, DateTimeKind.Local).AddTicks(7306) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "95ed2407-3e58-4af2-88a4-1c4e96473f68",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2025, 9, 25, 16, 59, 1, 423, DateTimeKind.Local).AddTicks(7347), new DateTime(2025, 9, 25, 16, 59, 1, 423, DateTimeKind.Local).AddTicks(7348) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "97942bee-ef12-4425-8225-4f293d0f36dd",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2025, 9, 25, 16, 59, 1, 423, DateTimeKind.Local).AddTicks(7442), new DateTime(2025, 9, 25, 16, 59, 1, 423, DateTimeKind.Local).AddTicks(7443) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "cd12b44b-103b-48df-8887-a2bf42e0651e",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2025, 9, 25, 16, 59, 1, 423, DateTimeKind.Local).AddTicks(7425), new DateTime(2025, 9, 25, 16, 59, 1, 423, DateTimeKind.Local).AddTicks(7427) });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "7e67d486-af3e-49f1-a109-a2b864b8e0ec",
                columns: new[] { "ConcurrencyStamp", "Created", "Modified", "PasswordHash", "SecurityStamp" },
                values: new object[] { "3cfa52af-4d8a-419d-b869-36d6fae607c2", new DateTime(2025, 9, 25, 16, 59, 1, 423, DateTimeKind.Local).AddTicks(7648), new DateTime(2025, 9, 25, 16, 59, 1, 423, DateTimeKind.Local).AddTicks(7650), "AQAAAAIAAYagAAAAEG1AFv8HdZPLzYVRwo7fEzBrDHmu+KsNSyzASnZF8aWt8beJpgaZX6liarnEp09y8A==", "c8ee4ad0-a90e-4037-9a24-e1643ce668f0" });

            migrationBuilder.UpdateData(
                table: "Designations",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2025, 9, 25, 16, 59, 1, 423, DateTimeKind.Local).AddTicks(7492), new DateTime(2025, 9, 25, 16, 59, 1, 423, DateTimeKind.Local).AddTicks(7492) });

            migrationBuilder.UpdateData(
                table: "EmploymentTypes",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2025, 9, 25, 16, 59, 1, 423, DateTimeKind.Local).AddTicks(7511), new DateTime(2025, 9, 25, 16, 59, 1, 423, DateTimeKind.Local).AddTicks(7511) });

            migrationBuilder.UpdateData(
                table: "Genders",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2025, 9, 25, 16, 59, 1, 423, DateTimeKind.Local).AddTicks(7562), new DateTime(2025, 9, 25, 16, 59, 1, 423, DateTimeKind.Local).AddTicks(7563) });

            migrationBuilder.UpdateData(
                table: "Nationalities",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2025, 9, 25, 16, 59, 1, 423, DateTimeKind.Local).AddTicks(7529), new DateTime(2025, 9, 25, 16, 59, 1, 423, DateTimeKind.Local).AddTicks(7529) });

            migrationBuilder.UpdateData(
                table: "Person",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "DateOfBirth", "EmploymentDate", "EndofEmploymentDate", "Modified" },
                values: new object[] { new DateTime(2025, 9, 25, 16, 59, 1, 423, DateTimeKind.Local).AddTicks(7588), new DateTime(2025, 9, 25, 16, 59, 1, 423, DateTimeKind.Local).AddTicks(7586), new DateTime(2025, 9, 25, 16, 59, 1, 423, DateTimeKind.Local).AddTicks(7583), new DateTime(2025, 9, 25, 16, 59, 1, 423, DateTimeKind.Local).AddTicks(7583), new DateTime(2025, 9, 25, 16, 59, 1, 423, DateTimeKind.Local).AddTicks(7589) });

            migrationBuilder.UpdateData(
                table: "Religions",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2025, 9, 25, 16, 59, 1, 423, DateTimeKind.Local).AddTicks(7548), new DateTime(2025, 9, 25, 16, 59, 1, 423, DateTimeKind.Local).AddTicks(7549) });

            migrationBuilder.UpdateData(
                table: "StaffCategories",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2025, 9, 25, 16, 59, 1, 423, DateTimeKind.Local).AddTicks(7474), new DateTime(2025, 9, 25, 16, 59, 1, 423, DateTimeKind.Local).AddTicks(7475) });

            migrationBuilder.AddForeignKey(
                name: "FK_Assessments_AssessmentTypes_AssessmentTypeId",
                table: "Assessments",
                column: "AssessmentTypeId",
                principalTable: "AssessmentTypes",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Assessments_Grades_GradeId",
                table: "Assessments",
                column: "GradeId",
                principalTable: "Grades",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Assessments_Person_StudentId",
                table: "Assessments",
                column: "StudentId",
                principalTable: "Person",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Assessments_SchoolClasses_SchoolClassId",
                table: "Assessments",
                column: "SchoolClassId",
                principalTable: "SchoolClasses",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Assessments_Sessions_SessionId",
                table: "Assessments",
                column: "SessionId",
                principalTable: "Sessions",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Assessments_SpecificOutcomes_SpecificOutcomeId",
                table: "Assessments",
                column: "SpecificOutcomeId",
                principalTable: "SpecificOutcomes",
                principalColumn: "Id");
        }
    }
}
