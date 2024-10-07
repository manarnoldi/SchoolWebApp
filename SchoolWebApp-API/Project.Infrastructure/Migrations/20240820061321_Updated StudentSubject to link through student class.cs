using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SchoolWebApp.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdatedStudentSubjecttolinkthroughstudentclass : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_StudentSubjects_AcademicYears_AcademicYearId",
                table: "StudentSubjects");

            migrationBuilder.DropForeignKey(
                name: "FK_StudentSubjects_Person_StudentId",
                table: "StudentSubjects");

            migrationBuilder.DropIndex(
                name: "IX_StudentSubjects_AcademicYearId",
                table: "StudentSubjects");

            migrationBuilder.DropColumn(
                name: "AcademicYearId",
                table: "StudentSubjects");

            migrationBuilder.RenameColumn(
                name: "StudentId",
                table: "StudentSubjects",
                newName: "StudentClassId");

            migrationBuilder.RenameIndex(
                name: "IX_StudentSubjects_StudentId",
                table: "StudentSubjects",
                newName: "IX_StudentSubjects_StudentClassId");

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "StudentSubjects",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "269f0cf3-405e-4163-83f3-1b63ebebd62e",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2024, 8, 20, 9, 13, 19, 849, DateTimeKind.Local).AddTicks(9443), new DateTime(2024, 8, 20, 9, 13, 19, 849, DateTimeKind.Local).AddTicks(9454) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "448df289-142c-4959-a912-60733515e1b4",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2024, 8, 20, 9, 13, 19, 849, DateTimeKind.Local).AddTicks(9403), new DateTime(2024, 8, 20, 9, 13, 19, 849, DateTimeKind.Local).AddTicks(9405) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "48c50c3a-9958-453b-b649-4e21af131322",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2024, 8, 20, 9, 13, 19, 849, DateTimeKind.Local).AddTicks(9357), new DateTime(2024, 8, 20, 9, 13, 19, 849, DateTimeKind.Local).AddTicks(9358) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "717d9b15-a428-440c-b26b-08d3bbb68b02",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2024, 8, 20, 9, 13, 19, 849, DateTimeKind.Local).AddTicks(9234), new DateTime(2024, 8, 20, 9, 13, 19, 849, DateTimeKind.Local).AddTicks(9264) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "95ed2407-3e58-4af2-88a4-1c4e96473f68",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2024, 8, 20, 9, 13, 19, 849, DateTimeKind.Local).AddTicks(9330), new DateTime(2024, 8, 20, 9, 13, 19, 849, DateTimeKind.Local).AddTicks(9331) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "97942bee-ef12-4425-8225-4f293d0f36dd",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2024, 8, 20, 9, 13, 19, 849, DateTimeKind.Local).AddTicks(9529), new DateTime(2024, 8, 20, 9, 13, 19, 849, DateTimeKind.Local).AddTicks(9530) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "cd12b44b-103b-48df-8887-a2bf42e0651e",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2024, 8, 20, 9, 13, 19, 849, DateTimeKind.Local).AddTicks(9502), new DateTime(2024, 8, 20, 9, 13, 19, 849, DateTimeKind.Local).AddTicks(9503) });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "7e67d486-af3e-49f1-a109-a2b864b8e0ec",
                columns: new[] { "ConcurrencyStamp", "Created", "Modified", "PasswordHash", "SecurityStamp" },
                values: new object[] { "af406b5a-6147-4bcb-b811-236e5dd34de9", new DateTime(2024, 8, 20, 9, 13, 19, 849, DateTimeKind.Local).AddTicks(9656), new DateTime(2024, 8, 20, 9, 13, 19, 849, DateTimeKind.Local).AddTicks(9658), "AQAAAAIAAYagAAAAEKalvcSUvWgxbUzng3aIFrDAbiY+rsJ0mLGmapRYV2i95i77H3n65av3/SURDnn3mg==", "0b73f637-aa70-4c07-a481-97f2b1c0a987" });

            migrationBuilder.AddForeignKey(
                name: "FK_StudentSubjects_StudentClasses_StudentClassId",
                table: "StudentSubjects",
                column: "StudentClassId",
                principalTable: "StudentClasses",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_StudentSubjects_StudentClasses_StudentClassId",
                table: "StudentSubjects");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "StudentSubjects");

            migrationBuilder.RenameColumn(
                name: "StudentClassId",
                table: "StudentSubjects",
                newName: "StudentId");

            migrationBuilder.RenameIndex(
                name: "IX_StudentSubjects_StudentClassId",
                table: "StudentSubjects",
                newName: "IX_StudentSubjects_StudentId");

            migrationBuilder.AddColumn<int>(
                name: "AcademicYearId",
                table: "StudentSubjects",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "269f0cf3-405e-4163-83f3-1b63ebebd62e",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2024, 8, 5, 9, 21, 47, 781, DateTimeKind.Local).AddTicks(470), new DateTime(2024, 8, 5, 9, 21, 47, 781, DateTimeKind.Local).AddTicks(478) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "448df289-142c-4959-a912-60733515e1b4",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2024, 8, 5, 9, 21, 47, 781, DateTimeKind.Local).AddTicks(442), new DateTime(2024, 8, 5, 9, 21, 47, 781, DateTimeKind.Local).AddTicks(443) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "48c50c3a-9958-453b-b649-4e21af131322",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2024, 8, 5, 9, 21, 47, 781, DateTimeKind.Local).AddTicks(410), new DateTime(2024, 8, 5, 9, 21, 47, 781, DateTimeKind.Local).AddTicks(412) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "717d9b15-a428-440c-b26b-08d3bbb68b02",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2024, 8, 5, 9, 21, 47, 781, DateTimeKind.Local).AddTicks(291), new DateTime(2024, 8, 5, 9, 21, 47, 781, DateTimeKind.Local).AddTicks(316) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "95ed2407-3e58-4af2-88a4-1c4e96473f68",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2024, 8, 5, 9, 21, 47, 781, DateTimeKind.Local).AddTicks(379), new DateTime(2024, 8, 5, 9, 21, 47, 781, DateTimeKind.Local).AddTicks(381) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "97942bee-ef12-4425-8225-4f293d0f36dd",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2024, 8, 5, 9, 21, 47, 781, DateTimeKind.Local).AddTicks(554), new DateTime(2024, 8, 5, 9, 21, 47, 781, DateTimeKind.Local).AddTicks(556) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "cd12b44b-103b-48df-8887-a2bf42e0651e",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2024, 8, 5, 9, 21, 47, 781, DateTimeKind.Local).AddTicks(525), new DateTime(2024, 8, 5, 9, 21, 47, 781, DateTimeKind.Local).AddTicks(527) });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "7e67d486-af3e-49f1-a109-a2b864b8e0ec",
                columns: new[] { "ConcurrencyStamp", "Created", "Modified", "PasswordHash", "SecurityStamp" },
                values: new object[] { "db7976cb-219a-4644-969b-c88facf70083", new DateTime(2024, 8, 5, 9, 21, 47, 781, DateTimeKind.Local).AddTicks(668), new DateTime(2024, 8, 5, 9, 21, 47, 781, DateTimeKind.Local).AddTicks(669), "AQAAAAIAAYagAAAAEJHQjcxrXe+n1vWa1xWuFyPqP7/dEAGUQcHslUbgNs0X3GUkRWNP9aLahon6a2j/dA==", "5779fd39-e808-4c2c-b516-8d125aa6396d" });

            migrationBuilder.CreateIndex(
                name: "IX_StudentSubjects_AcademicYearId",
                table: "StudentSubjects",
                column: "AcademicYearId");

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
        }
    }
}
