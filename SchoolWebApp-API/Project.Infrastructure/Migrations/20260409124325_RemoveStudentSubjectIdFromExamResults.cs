using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SchoolWebApp.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RemoveStudentSubjectIdFromExamResults : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ExamResults_StudentSubjects_StudentSubjectId",
                table: "ExamResults");

            migrationBuilder.DropIndex(
                name: "IX_ExamResults_StudentSubjectId",
                table: "ExamResults");

            migrationBuilder.DropColumn(
                name: "StudentSubjectId",
                table: "ExamResults");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "269f0cf3-405e-4163-83f3-1b63ebebd62e",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 9, 15, 43, 24, 378, DateTimeKind.Local).AddTicks(9881), new DateTime(2026, 4, 9, 15, 43, 24, 378, DateTimeKind.Local).AddTicks(9882) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "448df289-142c-4959-a912-60733515e1b4",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 9, 15, 43, 24, 378, DateTimeKind.Local).AddTicks(9853), new DateTime(2026, 4, 9, 15, 43, 24, 378, DateTimeKind.Local).AddTicks(9854) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "48c50c3a-9958-453b-b649-4e21af131322",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 9, 15, 43, 24, 378, DateTimeKind.Local).AddTicks(9836), new DateTime(2026, 4, 9, 15, 43, 24, 378, DateTimeKind.Local).AddTicks(9838) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "717d9b15-a428-440c-b26b-08d3bbb68b02",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 9, 15, 43, 24, 378, DateTimeKind.Local).AddTicks(9769), new DateTime(2026, 4, 9, 15, 43, 24, 378, DateTimeKind.Local).AddTicks(9784) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "95ed2407-3e58-4af2-88a4-1c4e96473f68",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 9, 15, 43, 24, 378, DateTimeKind.Local).AddTicks(9817), new DateTime(2026, 4, 9, 15, 43, 24, 378, DateTimeKind.Local).AddTicks(9819) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "97942bee-ef12-4425-8225-4f293d0f36dd",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 9, 15, 43, 24, 378, DateTimeKind.Local).AddTicks(9916), new DateTime(2026, 4, 9, 15, 43, 24, 378, DateTimeKind.Local).AddTicks(9917) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "cd12b44b-103b-48df-8887-a2bf42e0651e",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 9, 15, 43, 24, 378, DateTimeKind.Local).AddTicks(9900), new DateTime(2026, 4, 9, 15, 43, 24, 378, DateTimeKind.Local).AddTicks(9901) });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "7e67d486-af3e-49f1-a109-a2b864b8e0ec",
                columns: new[] { "ConcurrencyStamp", "Created", "Modified", "PasswordHash", "SecurityStamp" },
                values: new object[] { "5ee9a0ff-4a61-43a0-a4e1-7477b94cbdd9", new DateTime(2026, 4, 9, 15, 43, 24, 379, DateTimeKind.Local).AddTicks(139), new DateTime(2026, 4, 9, 15, 43, 24, 379, DateTimeKind.Local).AddTicks(140), "AQAAAAIAAYagAAAAECEXcXmq+oq5Erc+aduae70UVU1+YZejcXC/V0gdXrz3fouRoXnTgngLjd3eaUwFOg==", "2d9b7f15-dd8a-45c8-9a4e-301da84f4267" });

            migrationBuilder.UpdateData(
                table: "Designations",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 9, 15, 43, 24, 378, DateTimeKind.Local).AddTicks(9971), new DateTime(2026, 4, 9, 15, 43, 24, 378, DateTimeKind.Local).AddTicks(9972) });

            migrationBuilder.UpdateData(
                table: "EmploymentTypes",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 9, 15, 43, 24, 378, DateTimeKind.Local).AddTicks(9995), new DateTime(2026, 4, 9, 15, 43, 24, 378, DateTimeKind.Local).AddTicks(9995) });

            migrationBuilder.UpdateData(
                table: "Genders",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 9, 15, 43, 24, 379, DateTimeKind.Local).AddTicks(49), new DateTime(2026, 4, 9, 15, 43, 24, 379, DateTimeKind.Local).AddTicks(50) });

            migrationBuilder.UpdateData(
                table: "Nationalities",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 9, 15, 43, 24, 379, DateTimeKind.Local).AddTicks(13), new DateTime(2026, 4, 9, 15, 43, 24, 379, DateTimeKind.Local).AddTicks(14) });

            migrationBuilder.UpdateData(
                table: "Person",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "DateOfBirth", "EmploymentDate", "EndofEmploymentDate", "Modified" },
                values: new object[] { new DateTime(2026, 4, 9, 15, 43, 24, 379, DateTimeKind.Local).AddTicks(78), new DateTime(2026, 4, 9, 15, 43, 24, 379, DateTimeKind.Local).AddTicks(75), new DateTime(2026, 4, 9, 15, 43, 24, 379, DateTimeKind.Local).AddTicks(72), new DateTime(2026, 4, 9, 15, 43, 24, 379, DateTimeKind.Local).AddTicks(73), new DateTime(2026, 4, 9, 15, 43, 24, 379, DateTimeKind.Local).AddTicks(79) });

            migrationBuilder.UpdateData(
                table: "Religions",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 9, 15, 43, 24, 379, DateTimeKind.Local).AddTicks(31), new DateTime(2026, 4, 9, 15, 43, 24, 379, DateTimeKind.Local).AddTicks(31) });

            migrationBuilder.UpdateData(
                table: "StaffCategories",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 9, 15, 43, 24, 378, DateTimeKind.Local).AddTicks(9943), new DateTime(2026, 4, 9, 15, 43, 24, 378, DateTimeKind.Local).AddTicks(9944) });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "StudentSubjectId",
                table: "ExamResults",
                type: "int",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "269f0cf3-405e-4163-83f3-1b63ebebd62e",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 9, 13, 35, 53, 270, DateTimeKind.Local).AddTicks(30), new DateTime(2026, 4, 9, 13, 35, 53, 270, DateTimeKind.Local).AddTicks(31) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "448df289-142c-4959-a912-60733515e1b4",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 9, 13, 35, 53, 270, DateTimeKind.Local).AddTicks(10), new DateTime(2026, 4, 9, 13, 35, 53, 270, DateTimeKind.Local).AddTicks(11) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "48c50c3a-9958-453b-b649-4e21af131322",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 9, 13, 35, 53, 269, DateTimeKind.Local).AddTicks(9979), new DateTime(2026, 4, 9, 13, 35, 53, 269, DateTimeKind.Local).AddTicks(9980) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "717d9b15-a428-440c-b26b-08d3bbb68b02",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 9, 13, 35, 53, 269, DateTimeKind.Local).AddTicks(9865), new DateTime(2026, 4, 9, 13, 35, 53, 269, DateTimeKind.Local).AddTicks(9883) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "95ed2407-3e58-4af2-88a4-1c4e96473f68",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 9, 13, 35, 53, 269, DateTimeKind.Local).AddTicks(9952), new DateTime(2026, 4, 9, 13, 35, 53, 269, DateTimeKind.Local).AddTicks(9953) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "97942bee-ef12-4425-8225-4f293d0f36dd",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 9, 13, 35, 53, 270, DateTimeKind.Local).AddTicks(69), new DateTime(2026, 4, 9, 13, 35, 53, 270, DateTimeKind.Local).AddTicks(69) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "cd12b44b-103b-48df-8887-a2bf42e0651e",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 9, 13, 35, 53, 270, DateTimeKind.Local).AddTicks(50), new DateTime(2026, 4, 9, 13, 35, 53, 270, DateTimeKind.Local).AddTicks(51) });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "7e67d486-af3e-49f1-a109-a2b864b8e0ec",
                columns: new[] { "ConcurrencyStamp", "Created", "Modified", "PasswordHash", "SecurityStamp" },
                values: new object[] { "afd5edb0-5890-4243-9b38-15e87da77df6", new DateTime(2026, 4, 9, 13, 35, 53, 270, DateTimeKind.Local).AddTicks(437), new DateTime(2026, 4, 9, 13, 35, 53, 270, DateTimeKind.Local).AddTicks(438), "AQAAAAIAAYagAAAAENNy1FCiJ5MQoWKDvdoFSVecoXMUi7uZlheh7viAWfPuHZN35FpO0i9D9TXAutNf5g==", "830ab2a4-cef1-4d83-b08a-86ace8149468" });

            migrationBuilder.UpdateData(
                table: "Designations",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 9, 13, 35, 53, 270, DateTimeKind.Local).AddTicks(122), new DateTime(2026, 4, 9, 13, 35, 53, 270, DateTimeKind.Local).AddTicks(122) });

            migrationBuilder.UpdateData(
                table: "EmploymentTypes",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 9, 13, 35, 53, 270, DateTimeKind.Local).AddTicks(158), new DateTime(2026, 4, 9, 13, 35, 53, 270, DateTimeKind.Local).AddTicks(159) });

            migrationBuilder.UpdateData(
                table: "Genders",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 9, 13, 35, 53, 270, DateTimeKind.Local).AddTicks(228), new DateTime(2026, 4, 9, 13, 35, 53, 270, DateTimeKind.Local).AddTicks(228) });

            migrationBuilder.UpdateData(
                table: "Nationalities",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 9, 13, 35, 53, 270, DateTimeKind.Local).AddTicks(181), new DateTime(2026, 4, 9, 13, 35, 53, 270, DateTimeKind.Local).AddTicks(181) });

            migrationBuilder.UpdateData(
                table: "Person",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "DateOfBirth", "EmploymentDate", "EndofEmploymentDate", "Modified" },
                values: new object[] { new DateTime(2026, 4, 9, 13, 35, 53, 270, DateTimeKind.Local).AddTicks(268), new DateTime(2026, 4, 9, 13, 35, 53, 270, DateTimeKind.Local).AddTicks(264), new DateTime(2026, 4, 9, 13, 35, 53, 270, DateTimeKind.Local).AddTicks(258), new DateTime(2026, 4, 9, 13, 35, 53, 270, DateTimeKind.Local).AddTicks(259), new DateTime(2026, 4, 9, 13, 35, 53, 270, DateTimeKind.Local).AddTicks(275) });

            migrationBuilder.UpdateData(
                table: "Religions",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 9, 13, 35, 53, 270, DateTimeKind.Local).AddTicks(201), new DateTime(2026, 4, 9, 13, 35, 53, 270, DateTimeKind.Local).AddTicks(202) });

            migrationBuilder.UpdateData(
                table: "StaffCategories",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 9, 13, 35, 53, 270, DateTimeKind.Local).AddTicks(99), new DateTime(2026, 4, 9, 13, 35, 53, 270, DateTimeKind.Local).AddTicks(100) });

            migrationBuilder.CreateIndex(
                name: "IX_ExamResults_StudentSubjectId",
                table: "ExamResults",
                column: "StudentSubjectId");

            migrationBuilder.AddForeignKey(
                name: "FK_ExamResults_StudentSubjects_StudentSubjectId",
                table: "ExamResults",
                column: "StudentSubjectId",
                principalTable: "StudentSubjects",
                principalColumn: "Id");
        }
    }
}
