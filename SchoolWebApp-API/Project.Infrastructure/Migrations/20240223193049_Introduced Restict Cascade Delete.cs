using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SchoolWebApp.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class IntroducedRestictCascadeDelete : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetRoleClaims_AspNetRoles_RoleId",
                table: "AspNetRoleClaims");

            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUserClaims_AspNetUsers_UserId",
                table: "AspNetUserClaims");

            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUserLogins_AspNetUsers_UserId",
                table: "AspNetUserLogins");

            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUserRoles_AspNetRoles_RoleId",
                table: "AspNetUserRoles");

            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUserRoles_AspNetUsers_UserId",
                table: "AspNetUserRoles");

            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUserTokens_AspNetUsers_UserId",
                table: "AspNetUserTokens");

            migrationBuilder.DropForeignKey(
                name: "FK_Departments_Person_StaffDetailsId",
                table: "Departments");

            migrationBuilder.DropForeignKey(
                name: "FK_Discipline_OccurenceTypes_OccurenceTypeId",
                table: "Discipline");

            migrationBuilder.DropForeignKey(
                name: "FK_Discipline_Outcomes_OutcomeId",
                table: "Discipline");

            migrationBuilder.DropForeignKey(
                name: "FK_Discipline_Person_StaffDetailsId",
                table: "Discipline");

            migrationBuilder.DropForeignKey(
                name: "FK_Discipline_Person_StudentId",
                table: "Discipline");

            migrationBuilder.DropForeignKey(
                name: "FK_Events_Sessions_SessionId",
                table: "Events");

            migrationBuilder.DropForeignKey(
                name: "FK_ExamResults_Exams_ExamId",
                table: "ExamResults");

            migrationBuilder.DropForeignKey(
                name: "FK_ExamResults_Person_StudentId",
                table: "ExamResults");

            migrationBuilder.DropForeignKey(
                name: "FK_Exams_ExamTypes_ExamTypeId",
                table: "Exams");

            migrationBuilder.DropForeignKey(
                name: "FK_Exams_SchoolClasses_SchoolClassId",
                table: "Exams");

            migrationBuilder.DropForeignKey(
                name: "FK_Exams_Sessions_SessionId",
                table: "Exams");

            migrationBuilder.DropForeignKey(
                name: "FK_Exams_Subjects_SubjectId",
                table: "Exams");

            migrationBuilder.DropForeignKey(
                name: "FK_FormerSchools_Person_StudentId",
                table: "FormerSchools");

            migrationBuilder.DropForeignKey(
                name: "FK_FormerSchools_SchoolLevels_SchoolLevelId",
                table: "FormerSchools");

            migrationBuilder.DropForeignKey(
                name: "FK_Grades_Curricula_CurriculumId",
                table: "Grades");

            migrationBuilder.DropForeignKey(
                name: "FK_Person_Designations_DesignationId",
                table: "Person");

            migrationBuilder.DropForeignKey(
                name: "FK_Person_EmploymentTypes_EmploymentTypeId",
                table: "Person");

            migrationBuilder.DropForeignKey(
                name: "FK_Person_Genders_GenderId",
                table: "Person");

            migrationBuilder.DropForeignKey(
                name: "FK_Person_LearningModes_LearningModeId",
                table: "Person");

            migrationBuilder.DropForeignKey(
                name: "FK_Person_Nationalities_NationalityId",
                table: "Person");

            migrationBuilder.DropForeignKey(
                name: "FK_Person_Occupations_OccupationId",
                table: "Person");

            migrationBuilder.DropForeignKey(
                name: "FK_Person_Religions_ReligionId",
                table: "Person");

            migrationBuilder.DropForeignKey(
                name: "FK_Person_StaffCategories_StaffCategoryId",
                table: "Person");

            migrationBuilder.DropForeignKey(
                name: "FK_SchoolClasses_AcademicYears_AcademicYearId",
                table: "SchoolClasses");

            migrationBuilder.DropForeignKey(
                name: "FK_SchoolClasses_Curricula_CurriculumId",
                table: "SchoolClasses");

            migrationBuilder.DropForeignKey(
                name: "FK_SchoolClasses_Person_StudentId",
                table: "SchoolClasses");

            migrationBuilder.DropForeignKey(
                name: "FK_SchoolDetails_SchoolLevels_SchoolLevelId",
                table: "SchoolDetails");

            migrationBuilder.DropForeignKey(
                name: "FK_Sessions_AcademicYears_AcademicYearId",
                table: "Sessions");

            migrationBuilder.DropForeignKey(
                name: "FK_Sessions_Curricula_CurriculumId",
                table: "Sessions");

            migrationBuilder.DropForeignKey(
                name: "FK_Sessions_SessionTypes_SessionTypeId",
                table: "Sessions");

            migrationBuilder.DropForeignKey(
                name: "FK_StaffAttendances_Person_StaffDetailsId",
                table: "StaffAttendances");

            migrationBuilder.DropForeignKey(
                name: "FK_StaffClasses_Person_StaffDetailsId",
                table: "StaffClasses");

            migrationBuilder.DropForeignKey(
                name: "FK_StaffClasses_SchoolClasses_SchoolClassId",
                table: "StaffClasses");

            migrationBuilder.DropForeignKey(
                name: "FK_StaffSubjects_AcademicYears_AcademicYearId",
                table: "StaffSubjects");

            migrationBuilder.DropForeignKey(
                name: "FK_StaffSubjects_Person_StaffDetailsId",
                table: "StaffSubjects");

            migrationBuilder.DropForeignKey(
                name: "FK_StaffSubjects_Subjects_SubjectId",
                table: "StaffSubjects");

            migrationBuilder.DropForeignKey(
                name: "FK_StudentAttendances_StudentClasses_StudentClassId",
                table: "StudentAttendances");

            migrationBuilder.DropForeignKey(
                name: "FK_StudentClasses_Person_StudentId",
                table: "StudentClasses");

            migrationBuilder.DropForeignKey(
                name: "FK_StudentClasses_SchoolClasses_SchoolClassId",
                table: "StudentClasses");

            migrationBuilder.DropForeignKey(
                name: "FK_StudentParents_Person_ParentId",
                table: "StudentParents");

            migrationBuilder.DropForeignKey(
                name: "FK_StudentParents_Person_StudentId",
                table: "StudentParents");

            migrationBuilder.DropForeignKey(
                name: "FK_StudentParents_RelationShips_RelationShipId",
                table: "StudentParents");

            migrationBuilder.DropForeignKey(
                name: "FK_StudentSubjects_AcademicYears_AcademicYearId",
                table: "StudentSubjects");

            migrationBuilder.DropForeignKey(
                name: "FK_StudentSubjects_Person_StudentId",
                table: "StudentSubjects");

            migrationBuilder.DropForeignKey(
                name: "FK_StudentSubjects_Subjects_SubjectId",
                table: "StudentSubjects");

            migrationBuilder.DropForeignKey(
                name: "FK_SubjectGroups_Departments_DepartmentId",
                table: "SubjectGroups");

            migrationBuilder.DropForeignKey(
                name: "FK_Subjects_Curricula_CurriculumId",
                table: "Subjects");

            migrationBuilder.DropForeignKey(
                name: "FK_Subjects_Person_StaffDetailsId",
                table: "Subjects");

            migrationBuilder.DropForeignKey(
                name: "FK_Subjects_SubjectGroups_SubjectGroupId",
                table: "Subjects");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "269f0cf3-405e-4163-83f3-1b63ebebd62e",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2024, 2, 23, 22, 30, 48, 580, DateTimeKind.Local).AddTicks(2886), new DateTime(2024, 2, 23, 22, 30, 48, 580, DateTimeKind.Local).AddTicks(2888) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "448df289-142c-4959-a912-60733515e1b4",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2024, 2, 23, 22, 30, 48, 580, DateTimeKind.Local).AddTicks(2824), new DateTime(2024, 2, 23, 22, 30, 48, 580, DateTimeKind.Local).AddTicks(2827) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "48c50c3a-9958-453b-b649-4e21af131322",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2024, 2, 23, 22, 30, 48, 580, DateTimeKind.Local).AddTicks(2734), new DateTime(2024, 2, 23, 22, 30, 48, 580, DateTimeKind.Local).AddTicks(2752) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "717d9b15-a428-440c-b26b-08d3bbb68b02",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2024, 2, 23, 22, 30, 48, 580, DateTimeKind.Local).AddTicks(2491), new DateTime(2024, 2, 23, 22, 30, 48, 580, DateTimeKind.Local).AddTicks(2530) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "95ed2407-3e58-4af2-88a4-1c4e96473f68",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2024, 2, 23, 22, 30, 48, 580, DateTimeKind.Local).AddTicks(2673), new DateTime(2024, 2, 23, 22, 30, 48, 580, DateTimeKind.Local).AddTicks(2676) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "97942bee-ef12-4425-8225-4f293d0f36dd",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2024, 2, 23, 22, 30, 48, 580, DateTimeKind.Local).AddTicks(3009), new DateTime(2024, 2, 23, 22, 30, 48, 580, DateTimeKind.Local).AddTicks(3012) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "cd12b44b-103b-48df-8887-a2bf42e0651e",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2024, 2, 23, 22, 30, 48, 580, DateTimeKind.Local).AddTicks(2952), new DateTime(2024, 2, 23, 22, 30, 48, 580, DateTimeKind.Local).AddTicks(2955) });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "7e67d486-af3e-49f1-a109-a2b864b8e0ec",
                columns: new[] { "ConcurrencyStamp", "Created", "Modified", "PasswordHash", "SecurityStamp" },
                values: new object[] { "736139c4-4c0a-4b56-ad3a-3f80058de608", new DateTime(2024, 2, 23, 22, 30, 48, 580, DateTimeKind.Local).AddTicks(3158), new DateTime(2024, 2, 23, 22, 30, 48, 580, DateTimeKind.Local).AddTicks(3161), "AQAAAAIAAYagAAAAEA9E0eXZIp9DcYywHQNyKs6UEnplktU7/i1mtfW3xB2Hga3clAYHK5pi396ZaJFRQQ==", "cc2a00ec-f186-4e2d-97e2-7cddb5fa73fc" });

            migrationBuilder.UpdateData(
                table: "SchoolLevels",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2024, 2, 23, 22, 30, 48, 685, DateTimeKind.Local).AddTicks(573), new DateTime(2024, 2, 23, 22, 30, 48, 685, DateTimeKind.Local).AddTicks(599) });

            migrationBuilder.UpdateData(
                table: "SchoolLevels",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2024, 2, 23, 22, 30, 48, 685, DateTimeKind.Local).AddTicks(602), new DateTime(2024, 2, 23, 22, 30, 48, 685, DateTimeKind.Local).AddTicks(603) });

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetRoleClaims_AspNetRoles_RoleId",
                table: "AspNetRoleClaims",
                column: "RoleId",
                principalTable: "AspNetRoles",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUserClaims_AspNetUsers_UserId",
                table: "AspNetUserClaims",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUserLogins_AspNetUsers_UserId",
                table: "AspNetUserLogins",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUserRoles_AspNetRoles_RoleId",
                table: "AspNetUserRoles",
                column: "RoleId",
                principalTable: "AspNetRoles",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUserRoles_AspNetUsers_UserId",
                table: "AspNetUserRoles",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUserTokens_AspNetUsers_UserId",
                table: "AspNetUserTokens",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Departments_Person_StaffDetailsId",
                table: "Departments",
                column: "StaffDetailsId",
                principalTable: "Person",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Discipline_OccurenceTypes_OccurenceTypeId",
                table: "Discipline",
                column: "OccurenceTypeId",
                principalTable: "OccurenceTypes",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Discipline_Outcomes_OutcomeId",
                table: "Discipline",
                column: "OutcomeId",
                principalTable: "Outcomes",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Discipline_Person_StaffDetailsId",
                table: "Discipline",
                column: "StaffDetailsId",
                principalTable: "Person",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Discipline_Person_StudentId",
                table: "Discipline",
                column: "StudentId",
                principalTable: "Person",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Events_Sessions_SessionId",
                table: "Events",
                column: "SessionId",
                principalTable: "Sessions",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ExamResults_Exams_ExamId",
                table: "ExamResults",
                column: "ExamId",
                principalTable: "Exams",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ExamResults_Person_StudentId",
                table: "ExamResults",
                column: "StudentId",
                principalTable: "Person",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Exams_ExamTypes_ExamTypeId",
                table: "Exams",
                column: "ExamTypeId",
                principalTable: "ExamTypes",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Exams_SchoolClasses_SchoolClassId",
                table: "Exams",
                column: "SchoolClassId",
                principalTable: "SchoolClasses",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Exams_Sessions_SessionId",
                table: "Exams",
                column: "SessionId",
                principalTable: "Sessions",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Exams_Subjects_SubjectId",
                table: "Exams",
                column: "SubjectId",
                principalTable: "Subjects",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_FormerSchools_Person_StudentId",
                table: "FormerSchools",
                column: "StudentId",
                principalTable: "Person",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_FormerSchools_SchoolLevels_SchoolLevelId",
                table: "FormerSchools",
                column: "SchoolLevelId",
                principalTable: "SchoolLevels",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Grades_Curricula_CurriculumId",
                table: "Grades",
                column: "CurriculumId",
                principalTable: "Curricula",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Person_Designations_DesignationId",
                table: "Person",
                column: "DesignationId",
                principalTable: "Designations",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Person_EmploymentTypes_EmploymentTypeId",
                table: "Person",
                column: "EmploymentTypeId",
                principalTable: "EmploymentTypes",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Person_Genders_GenderId",
                table: "Person",
                column: "GenderId",
                principalTable: "Genders",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Person_LearningModes_LearningModeId",
                table: "Person",
                column: "LearningModeId",
                principalTable: "LearningModes",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Person_Nationalities_NationalityId",
                table: "Person",
                column: "NationalityId",
                principalTable: "Nationalities",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Person_Occupations_OccupationId",
                table: "Person",
                column: "OccupationId",
                principalTable: "Occupations",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Person_Religions_ReligionId",
                table: "Person",
                column: "ReligionId",
                principalTable: "Religions",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Person_StaffCategories_StaffCategoryId",
                table: "Person",
                column: "StaffCategoryId",
                principalTable: "StaffCategories",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_SchoolClasses_AcademicYears_AcademicYearId",
                table: "SchoolClasses",
                column: "AcademicYearId",
                principalTable: "AcademicYears",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_SchoolClasses_Curricula_CurriculumId",
                table: "SchoolClasses",
                column: "CurriculumId",
                principalTable: "Curricula",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_SchoolClasses_Person_StudentId",
                table: "SchoolClasses",
                column: "StudentId",
                principalTable: "Person",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_SchoolDetails_SchoolLevels_SchoolLevelId",
                table: "SchoolDetails",
                column: "SchoolLevelId",
                principalTable: "SchoolLevels",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Sessions_AcademicYears_AcademicYearId",
                table: "Sessions",
                column: "AcademicYearId",
                principalTable: "AcademicYears",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Sessions_Curricula_CurriculumId",
                table: "Sessions",
                column: "CurriculumId",
                principalTable: "Curricula",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Sessions_SessionTypes_SessionTypeId",
                table: "Sessions",
                column: "SessionTypeId",
                principalTable: "SessionTypes",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_StaffAttendances_Person_StaffDetailsId",
                table: "StaffAttendances",
                column: "StaffDetailsId",
                principalTable: "Person",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_StaffClasses_Person_StaffDetailsId",
                table: "StaffClasses",
                column: "StaffDetailsId",
                principalTable: "Person",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_StaffClasses_SchoolClasses_SchoolClassId",
                table: "StaffClasses",
                column: "SchoolClassId",
                principalTable: "SchoolClasses",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_StaffSubjects_AcademicYears_AcademicYearId",
                table: "StaffSubjects",
                column: "AcademicYearId",
                principalTable: "AcademicYears",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_StaffSubjects_Person_StaffDetailsId",
                table: "StaffSubjects",
                column: "StaffDetailsId",
                principalTable: "Person",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_StaffSubjects_Subjects_SubjectId",
                table: "StaffSubjects",
                column: "SubjectId",
                principalTable: "Subjects",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_StudentAttendances_StudentClasses_StudentClassId",
                table: "StudentAttendances",
                column: "StudentClassId",
                principalTable: "StudentClasses",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_StudentClasses_Person_StudentId",
                table: "StudentClasses",
                column: "StudentId",
                principalTable: "Person",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_StudentClasses_SchoolClasses_SchoolClassId",
                table: "StudentClasses",
                column: "SchoolClassId",
                principalTable: "SchoolClasses",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_StudentParents_Person_ParentId",
                table: "StudentParents",
                column: "ParentId",
                principalTable: "Person",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_StudentParents_Person_StudentId",
                table: "StudentParents",
                column: "StudentId",
                principalTable: "Person",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_StudentParents_RelationShips_RelationShipId",
                table: "StudentParents",
                column: "RelationShipId",
                principalTable: "RelationShips",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_StudentSubjects_AcademicYears_AcademicYearId",
                table: "StudentSubjects",
                column: "AcademicYearId",
                principalTable: "AcademicYears",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_StudentSubjects_Person_StudentId",
                table: "StudentSubjects",
                column: "StudentId",
                principalTable: "Person",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_StudentSubjects_Subjects_SubjectId",
                table: "StudentSubjects",
                column: "SubjectId",
                principalTable: "Subjects",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_SubjectGroups_Departments_DepartmentId",
                table: "SubjectGroups",
                column: "DepartmentId",
                principalTable: "Departments",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Subjects_Curricula_CurriculumId",
                table: "Subjects",
                column: "CurriculumId",
                principalTable: "Curricula",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Subjects_Person_StaffDetailsId",
                table: "Subjects",
                column: "StaffDetailsId",
                principalTable: "Person",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Subjects_SubjectGroups_SubjectGroupId",
                table: "Subjects",
                column: "SubjectGroupId",
                principalTable: "SubjectGroups",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetRoleClaims_AspNetRoles_RoleId",
                table: "AspNetRoleClaims");

            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUserClaims_AspNetUsers_UserId",
                table: "AspNetUserClaims");

            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUserLogins_AspNetUsers_UserId",
                table: "AspNetUserLogins");

            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUserRoles_AspNetRoles_RoleId",
                table: "AspNetUserRoles");

            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUserRoles_AspNetUsers_UserId",
                table: "AspNetUserRoles");

            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUserTokens_AspNetUsers_UserId",
                table: "AspNetUserTokens");

            migrationBuilder.DropForeignKey(
                name: "FK_Departments_Person_StaffDetailsId",
                table: "Departments");

            migrationBuilder.DropForeignKey(
                name: "FK_Discipline_OccurenceTypes_OccurenceTypeId",
                table: "Discipline");

            migrationBuilder.DropForeignKey(
                name: "FK_Discipline_Outcomes_OutcomeId",
                table: "Discipline");

            migrationBuilder.DropForeignKey(
                name: "FK_Discipline_Person_StaffDetailsId",
                table: "Discipline");

            migrationBuilder.DropForeignKey(
                name: "FK_Discipline_Person_StudentId",
                table: "Discipline");

            migrationBuilder.DropForeignKey(
                name: "FK_Events_Sessions_SessionId",
                table: "Events");

            migrationBuilder.DropForeignKey(
                name: "FK_ExamResults_Exams_ExamId",
                table: "ExamResults");

            migrationBuilder.DropForeignKey(
                name: "FK_ExamResults_Person_StudentId",
                table: "ExamResults");

            migrationBuilder.DropForeignKey(
                name: "FK_Exams_ExamTypes_ExamTypeId",
                table: "Exams");

            migrationBuilder.DropForeignKey(
                name: "FK_Exams_SchoolClasses_SchoolClassId",
                table: "Exams");

            migrationBuilder.DropForeignKey(
                name: "FK_Exams_Sessions_SessionId",
                table: "Exams");

            migrationBuilder.DropForeignKey(
                name: "FK_Exams_Subjects_SubjectId",
                table: "Exams");

            migrationBuilder.DropForeignKey(
                name: "FK_FormerSchools_Person_StudentId",
                table: "FormerSchools");

            migrationBuilder.DropForeignKey(
                name: "FK_FormerSchools_SchoolLevels_SchoolLevelId",
                table: "FormerSchools");

            migrationBuilder.DropForeignKey(
                name: "FK_Grades_Curricula_CurriculumId",
                table: "Grades");

            migrationBuilder.DropForeignKey(
                name: "FK_Person_Designations_DesignationId",
                table: "Person");

            migrationBuilder.DropForeignKey(
                name: "FK_Person_EmploymentTypes_EmploymentTypeId",
                table: "Person");

            migrationBuilder.DropForeignKey(
                name: "FK_Person_Genders_GenderId",
                table: "Person");

            migrationBuilder.DropForeignKey(
                name: "FK_Person_LearningModes_LearningModeId",
                table: "Person");

            migrationBuilder.DropForeignKey(
                name: "FK_Person_Nationalities_NationalityId",
                table: "Person");

            migrationBuilder.DropForeignKey(
                name: "FK_Person_Occupations_OccupationId",
                table: "Person");

            migrationBuilder.DropForeignKey(
                name: "FK_Person_Religions_ReligionId",
                table: "Person");

            migrationBuilder.DropForeignKey(
                name: "FK_Person_StaffCategories_StaffCategoryId",
                table: "Person");

            migrationBuilder.DropForeignKey(
                name: "FK_SchoolClasses_AcademicYears_AcademicYearId",
                table: "SchoolClasses");

            migrationBuilder.DropForeignKey(
                name: "FK_SchoolClasses_Curricula_CurriculumId",
                table: "SchoolClasses");

            migrationBuilder.DropForeignKey(
                name: "FK_SchoolClasses_Person_StudentId",
                table: "SchoolClasses");

            migrationBuilder.DropForeignKey(
                name: "FK_SchoolDetails_SchoolLevels_SchoolLevelId",
                table: "SchoolDetails");

            migrationBuilder.DropForeignKey(
                name: "FK_Sessions_AcademicYears_AcademicYearId",
                table: "Sessions");

            migrationBuilder.DropForeignKey(
                name: "FK_Sessions_Curricula_CurriculumId",
                table: "Sessions");

            migrationBuilder.DropForeignKey(
                name: "FK_Sessions_SessionTypes_SessionTypeId",
                table: "Sessions");

            migrationBuilder.DropForeignKey(
                name: "FK_StaffAttendances_Person_StaffDetailsId",
                table: "StaffAttendances");

            migrationBuilder.DropForeignKey(
                name: "FK_StaffClasses_Person_StaffDetailsId",
                table: "StaffClasses");

            migrationBuilder.DropForeignKey(
                name: "FK_StaffClasses_SchoolClasses_SchoolClassId",
                table: "StaffClasses");

            migrationBuilder.DropForeignKey(
                name: "FK_StaffSubjects_AcademicYears_AcademicYearId",
                table: "StaffSubjects");

            migrationBuilder.DropForeignKey(
                name: "FK_StaffSubjects_Person_StaffDetailsId",
                table: "StaffSubjects");

            migrationBuilder.DropForeignKey(
                name: "FK_StaffSubjects_Subjects_SubjectId",
                table: "StaffSubjects");

            migrationBuilder.DropForeignKey(
                name: "FK_StudentAttendances_StudentClasses_StudentClassId",
                table: "StudentAttendances");

            migrationBuilder.DropForeignKey(
                name: "FK_StudentClasses_Person_StudentId",
                table: "StudentClasses");

            migrationBuilder.DropForeignKey(
                name: "FK_StudentClasses_SchoolClasses_SchoolClassId",
                table: "StudentClasses");

            migrationBuilder.DropForeignKey(
                name: "FK_StudentParents_Person_ParentId",
                table: "StudentParents");

            migrationBuilder.DropForeignKey(
                name: "FK_StudentParents_Person_StudentId",
                table: "StudentParents");

            migrationBuilder.DropForeignKey(
                name: "FK_StudentParents_RelationShips_RelationShipId",
                table: "StudentParents");

            migrationBuilder.DropForeignKey(
                name: "FK_StudentSubjects_AcademicYears_AcademicYearId",
                table: "StudentSubjects");

            migrationBuilder.DropForeignKey(
                name: "FK_StudentSubjects_Person_StudentId",
                table: "StudentSubjects");

            migrationBuilder.DropForeignKey(
                name: "FK_StudentSubjects_Subjects_SubjectId",
                table: "StudentSubjects");

            migrationBuilder.DropForeignKey(
                name: "FK_SubjectGroups_Departments_DepartmentId",
                table: "SubjectGroups");

            migrationBuilder.DropForeignKey(
                name: "FK_Subjects_Curricula_CurriculumId",
                table: "Subjects");

            migrationBuilder.DropForeignKey(
                name: "FK_Subjects_Person_StaffDetailsId",
                table: "Subjects");

            migrationBuilder.DropForeignKey(
                name: "FK_Subjects_SubjectGroups_SubjectGroupId",
                table: "Subjects");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "269f0cf3-405e-4163-83f3-1b63ebebd62e",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2024, 2, 23, 22, 9, 18, 789, DateTimeKind.Local).AddTicks(9213), new DateTime(2024, 2, 23, 22, 9, 18, 789, DateTimeKind.Local).AddTicks(9214) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "448df289-142c-4959-a912-60733515e1b4",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2024, 2, 23, 22, 9, 18, 789, DateTimeKind.Local).AddTicks(9188), new DateTime(2024, 2, 23, 22, 9, 18, 789, DateTimeKind.Local).AddTicks(9189) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "48c50c3a-9958-453b-b649-4e21af131322",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2024, 2, 23, 22, 9, 18, 789, DateTimeKind.Local).AddTicks(9145), new DateTime(2024, 2, 23, 22, 9, 18, 789, DateTimeKind.Local).AddTicks(9155) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "717d9b15-a428-440c-b26b-08d3bbb68b02",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2024, 2, 23, 22, 9, 18, 789, DateTimeKind.Local).AddTicks(9030), new DateTime(2024, 2, 23, 22, 9, 18, 789, DateTimeKind.Local).AddTicks(9059) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "95ed2407-3e58-4af2-88a4-1c4e96473f68",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2024, 2, 23, 22, 9, 18, 789, DateTimeKind.Local).AddTicks(9115), new DateTime(2024, 2, 23, 22, 9, 18, 789, DateTimeKind.Local).AddTicks(9117) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "97942bee-ef12-4425-8225-4f293d0f36dd",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2024, 2, 23, 22, 9, 18, 789, DateTimeKind.Local).AddTicks(9266), new DateTime(2024, 2, 23, 22, 9, 18, 789, DateTimeKind.Local).AddTicks(9267) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "cd12b44b-103b-48df-8887-a2bf42e0651e",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2024, 2, 23, 22, 9, 18, 789, DateTimeKind.Local).AddTicks(9241), new DateTime(2024, 2, 23, 22, 9, 18, 789, DateTimeKind.Local).AddTicks(9242) });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "7e67d486-af3e-49f1-a109-a2b864b8e0ec",
                columns: new[] { "ConcurrencyStamp", "Created", "Modified", "PasswordHash", "SecurityStamp" },
                values: new object[] { "334e69f8-25a2-495c-8e77-74fce61a2f76", new DateTime(2024, 2, 23, 22, 9, 18, 789, DateTimeKind.Local).AddTicks(9368), new DateTime(2024, 2, 23, 22, 9, 18, 789, DateTimeKind.Local).AddTicks(9369), "AQAAAAIAAYagAAAAEMXea0b5cmuc2EHvQ0U5RRsJMUU9p5kJ/fhZyt2juR5hGzBWLNI1qmdqOaBbnot/Xg==", "eeb22d51-3a30-4a06-9c8f-5ec0b10ed9f7" });

            migrationBuilder.UpdateData(
                table: "SchoolLevels",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2024, 2, 23, 22, 9, 18, 881, DateTimeKind.Local).AddTicks(8787), new DateTime(2024, 2, 23, 22, 9, 18, 881, DateTimeKind.Local).AddTicks(8812) });

            migrationBuilder.UpdateData(
                table: "SchoolLevels",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2024, 2, 23, 22, 9, 18, 881, DateTimeKind.Local).AddTicks(8815), new DateTime(2024, 2, 23, 22, 9, 18, 881, DateTimeKind.Local).AddTicks(8816) });

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetRoleClaims_AspNetRoles_RoleId",
                table: "AspNetRoleClaims",
                column: "RoleId",
                principalTable: "AspNetRoles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUserClaims_AspNetUsers_UserId",
                table: "AspNetUserClaims",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUserLogins_AspNetUsers_UserId",
                table: "AspNetUserLogins",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUserRoles_AspNetRoles_RoleId",
                table: "AspNetUserRoles",
                column: "RoleId",
                principalTable: "AspNetRoles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUserRoles_AspNetUsers_UserId",
                table: "AspNetUserRoles",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUserTokens_AspNetUsers_UserId",
                table: "AspNetUserTokens",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Departments_Person_StaffDetailsId",
                table: "Departments",
                column: "StaffDetailsId",
                principalTable: "Person",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Discipline_OccurenceTypes_OccurenceTypeId",
                table: "Discipline",
                column: "OccurenceTypeId",
                principalTable: "OccurenceTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Discipline_Outcomes_OutcomeId",
                table: "Discipline",
                column: "OutcomeId",
                principalTable: "Outcomes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Discipline_Person_StaffDetailsId",
                table: "Discipline",
                column: "StaffDetailsId",
                principalTable: "Person",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Discipline_Person_StudentId",
                table: "Discipline",
                column: "StudentId",
                principalTable: "Person",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Events_Sessions_SessionId",
                table: "Events",
                column: "SessionId",
                principalTable: "Sessions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ExamResults_Exams_ExamId",
                table: "ExamResults",
                column: "ExamId",
                principalTable: "Exams",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ExamResults_Person_StudentId",
                table: "ExamResults",
                column: "StudentId",
                principalTable: "Person",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Exams_ExamTypes_ExamTypeId",
                table: "Exams",
                column: "ExamTypeId",
                principalTable: "ExamTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Exams_SchoolClasses_SchoolClassId",
                table: "Exams",
                column: "SchoolClassId",
                principalTable: "SchoolClasses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Exams_Sessions_SessionId",
                table: "Exams",
                column: "SessionId",
                principalTable: "Sessions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Exams_Subjects_SubjectId",
                table: "Exams",
                column: "SubjectId",
                principalTable: "Subjects",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_FormerSchools_Person_StudentId",
                table: "FormerSchools",
                column: "StudentId",
                principalTable: "Person",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_FormerSchools_SchoolLevels_SchoolLevelId",
                table: "FormerSchools",
                column: "SchoolLevelId",
                principalTable: "SchoolLevels",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Grades_Curricula_CurriculumId",
                table: "Grades",
                column: "CurriculumId",
                principalTable: "Curricula",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Person_Designations_DesignationId",
                table: "Person",
                column: "DesignationId",
                principalTable: "Designations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Person_EmploymentTypes_EmploymentTypeId",
                table: "Person",
                column: "EmploymentTypeId",
                principalTable: "EmploymentTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Person_Genders_GenderId",
                table: "Person",
                column: "GenderId",
                principalTable: "Genders",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Person_LearningModes_LearningModeId",
                table: "Person",
                column: "LearningModeId",
                principalTable: "LearningModes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Person_Nationalities_NationalityId",
                table: "Person",
                column: "NationalityId",
                principalTable: "Nationalities",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Person_Occupations_OccupationId",
                table: "Person",
                column: "OccupationId",
                principalTable: "Occupations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Person_Religions_ReligionId",
                table: "Person",
                column: "ReligionId",
                principalTable: "Religions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Person_StaffCategories_StaffCategoryId",
                table: "Person",
                column: "StaffCategoryId",
                principalTable: "StaffCategories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_SchoolClasses_AcademicYears_AcademicYearId",
                table: "SchoolClasses",
                column: "AcademicYearId",
                principalTable: "AcademicYears",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_SchoolClasses_Curricula_CurriculumId",
                table: "SchoolClasses",
                column: "CurriculumId",
                principalTable: "Curricula",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_SchoolClasses_Person_StudentId",
                table: "SchoolClasses",
                column: "StudentId",
                principalTable: "Person",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_SchoolDetails_SchoolLevels_SchoolLevelId",
                table: "SchoolDetails",
                column: "SchoolLevelId",
                principalTable: "SchoolLevels",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Sessions_AcademicYears_AcademicYearId",
                table: "Sessions",
                column: "AcademicYearId",
                principalTable: "AcademicYears",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Sessions_Curricula_CurriculumId",
                table: "Sessions",
                column: "CurriculumId",
                principalTable: "Curricula",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Sessions_SessionTypes_SessionTypeId",
                table: "Sessions",
                column: "SessionTypeId",
                principalTable: "SessionTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_StaffAttendances_Person_StaffDetailsId",
                table: "StaffAttendances",
                column: "StaffDetailsId",
                principalTable: "Person",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_StaffClasses_Person_StaffDetailsId",
                table: "StaffClasses",
                column: "StaffDetailsId",
                principalTable: "Person",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_StaffClasses_SchoolClasses_SchoolClassId",
                table: "StaffClasses",
                column: "SchoolClassId",
                principalTable: "SchoolClasses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_StaffSubjects_AcademicYears_AcademicYearId",
                table: "StaffSubjects",
                column: "AcademicYearId",
                principalTable: "AcademicYears",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_StaffSubjects_Person_StaffDetailsId",
                table: "StaffSubjects",
                column: "StaffDetailsId",
                principalTable: "Person",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_StaffSubjects_Subjects_SubjectId",
                table: "StaffSubjects",
                column: "SubjectId",
                principalTable: "Subjects",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_StudentAttendances_StudentClasses_StudentClassId",
                table: "StudentAttendances",
                column: "StudentClassId",
                principalTable: "StudentClasses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_StudentClasses_Person_StudentId",
                table: "StudentClasses",
                column: "StudentId",
                principalTable: "Person",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_StudentClasses_SchoolClasses_SchoolClassId",
                table: "StudentClasses",
                column: "SchoolClassId",
                principalTable: "SchoolClasses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_StudentParents_Person_ParentId",
                table: "StudentParents",
                column: "ParentId",
                principalTable: "Person",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_StudentParents_Person_StudentId",
                table: "StudentParents",
                column: "StudentId",
                principalTable: "Person",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_StudentParents_RelationShips_RelationShipId",
                table: "StudentParents",
                column: "RelationShipId",
                principalTable: "RelationShips",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_StudentSubjects_AcademicYears_AcademicYearId",
                table: "StudentSubjects",
                column: "AcademicYearId",
                principalTable: "AcademicYears",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_StudentSubjects_Person_StudentId",
                table: "StudentSubjects",
                column: "StudentId",
                principalTable: "Person",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_StudentSubjects_Subjects_SubjectId",
                table: "StudentSubjects",
                column: "SubjectId",
                principalTable: "Subjects",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_SubjectGroups_Departments_DepartmentId",
                table: "SubjectGroups",
                column: "DepartmentId",
                principalTable: "Departments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Subjects_Curricula_CurriculumId",
                table: "Subjects",
                column: "CurriculumId",
                principalTable: "Curricula",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Subjects_Person_StaffDetailsId",
                table: "Subjects",
                column: "StaffDetailsId",
                principalTable: "Person",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Subjects_SubjectGroups_SubjectGroupId",
                table: "Subjects",
                column: "SubjectGroupId",
                principalTable: "SubjectGroups",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
