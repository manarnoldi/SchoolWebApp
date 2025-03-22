using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SchoolWebApp.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdatedExamResulttocontainStudentSubject : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            //migrationBuilder.DropForeignKey(
            //    name: "FK_ExamResults_Person_StudentId",
            //    table: "ExamResults");

            //migrationBuilder.RenameColumn(
            //    name: "StudentId",
            //    table: "ExamResults",
            //    newName: "StudentSubjectId");

            //migrationBuilder.RenameIndex(
            //    name: "IX_ExamResults_StudentId",
            //    table: "ExamResults",
            //    newName: "IX_ExamResults_StudentSubjectId");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "269f0cf3-405e-4163-83f3-1b63ebebd62e",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2025, 3, 21, 15, 57, 56, 936, DateTimeKind.Local).AddTicks(6269), new DateTime(2025, 3, 21, 15, 57, 56, 936, DateTimeKind.Local).AddTicks(6290) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "448df289-142c-4959-a912-60733515e1b4",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2025, 3, 21, 15, 57, 56, 936, DateTimeKind.Local).AddTicks(6187), new DateTime(2025, 3, 21, 15, 57, 56, 936, DateTimeKind.Local).AddTicks(6190) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "48c50c3a-9958-453b-b649-4e21af131322",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2025, 3, 21, 15, 57, 56, 936, DateTimeKind.Local).AddTicks(6123), new DateTime(2025, 3, 21, 15, 57, 56, 936, DateTimeKind.Local).AddTicks(6126) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "717d9b15-a428-440c-b26b-08d3bbb68b02",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2025, 3, 21, 15, 57, 56, 936, DateTimeKind.Local).AddTicks(5766), new DateTime(2025, 3, 21, 15, 57, 56, 936, DateTimeKind.Local).AddTicks(5811) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "95ed2407-3e58-4af2-88a4-1c4e96473f68",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2025, 3, 21, 15, 57, 56, 936, DateTimeKind.Local).AddTicks(6045), new DateTime(2025, 3, 21, 15, 57, 56, 936, DateTimeKind.Local).AddTicks(6049) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "97942bee-ef12-4425-8225-4f293d0f36dd",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2025, 3, 21, 15, 57, 56, 936, DateTimeKind.Local).AddTicks(6436), new DateTime(2025, 3, 21, 15, 57, 56, 936, DateTimeKind.Local).AddTicks(6438) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "cd12b44b-103b-48df-8887-a2bf42e0651e",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2025, 3, 21, 15, 57, 56, 936, DateTimeKind.Local).AddTicks(6374), new DateTime(2025, 3, 21, 15, 57, 56, 936, DateTimeKind.Local).AddTicks(6378) });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "7e67d486-af3e-49f1-a109-a2b864b8e0ec",
                columns: new[] { "ConcurrencyStamp", "Created", "Modified", "PasswordHash", "SecurityStamp" },
                values: new object[] { "d3e91798-652a-4108-8705-9a46fc256a34", new DateTime(2025, 3, 21, 15, 57, 56, 936, DateTimeKind.Local).AddTicks(6581), new DateTime(2025, 3, 21, 15, 57, 56, 936, DateTimeKind.Local).AddTicks(6585), "AQAAAAIAAYagAAAAEPIcY2KCASBtjhnNzO94l6Zyzk/WgbUp+9pq4fs7AUEkBJACrCmz7kmI8uq3bq0HjQ==", "3be37365-a3bb-41b7-925d-976c3dd3f018" });

            migrationBuilder.AddForeignKey(
                name: "FK_ExamResults_StudentSubjects_StudentSubjectId",
                table: "ExamResults",
                column: "StudentSubjectId",
                principalTable: "StudentSubjects",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            //migrationBuilder.DropForeignKey(
            //    name: "FK_ExamResults_StudentSubjects_StudentSubjectId",
            //    table: "ExamResults");

            //migrationBuilder.RenameColumn(
            //    name: "StudentSubjectId",
            //    table: "ExamResults",
            //    newName: "StudentId");

            //migrationBuilder.RenameIndex(
            //    name: "IX_ExamResults_StudentSubjectId",
            //    table: "ExamResults",
            //    newName: "IX_ExamResults_StudentId");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "269f0cf3-405e-4163-83f3-1b63ebebd62e",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2025, 3, 18, 12, 15, 52, 925, DateTimeKind.Local).AddTicks(7166), new DateTime(2025, 3, 18, 12, 15, 52, 925, DateTimeKind.Local).AddTicks(7192) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "448df289-142c-4959-a912-60733515e1b4",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2025, 3, 18, 12, 15, 52, 925, DateTimeKind.Local).AddTicks(7050), new DateTime(2025, 3, 18, 12, 15, 52, 925, DateTimeKind.Local).AddTicks(7055) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "48c50c3a-9958-453b-b649-4e21af131322",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2025, 3, 18, 12, 15, 52, 925, DateTimeKind.Local).AddTicks(6964), new DateTime(2025, 3, 18, 12, 15, 52, 925, DateTimeKind.Local).AddTicks(6967) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "717d9b15-a428-440c-b26b-08d3bbb68b02",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2025, 3, 18, 12, 15, 52, 925, DateTimeKind.Local).AddTicks(6547), new DateTime(2025, 3, 18, 12, 15, 52, 925, DateTimeKind.Local).AddTicks(6629) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "95ed2407-3e58-4af2-88a4-1c4e96473f68",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2025, 3, 18, 12, 15, 52, 925, DateTimeKind.Local).AddTicks(6868), new DateTime(2025, 3, 18, 12, 15, 52, 925, DateTimeKind.Local).AddTicks(6872) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "97942bee-ef12-4425-8225-4f293d0f36dd",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2025, 3, 18, 12, 15, 52, 925, DateTimeKind.Local).AddTicks(7421), new DateTime(2025, 3, 18, 12, 15, 52, 925, DateTimeKind.Local).AddTicks(7425) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "cd12b44b-103b-48df-8887-a2bf42e0651e",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2025, 3, 18, 12, 15, 52, 925, DateTimeKind.Local).AddTicks(7333), new DateTime(2025, 3, 18, 12, 15, 52, 925, DateTimeKind.Local).AddTicks(7337) });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "7e67d486-af3e-49f1-a109-a2b864b8e0ec",
                columns: new[] { "ConcurrencyStamp", "Created", "Modified", "PasswordHash", "SecurityStamp" },
                values: new object[] { "e6b93a89-c49a-421a-9af6-0a4c66b6f69b", new DateTime(2025, 3, 18, 12, 15, 52, 925, DateTimeKind.Local).AddTicks(7651), new DateTime(2025, 3, 18, 12, 15, 52, 925, DateTimeKind.Local).AddTicks(7658), "AQAAAAIAAYagAAAAEDvXK87UojbPgfXIBfgbzlBW7rBqJqS5bi42Ri569jE9OQRJJCgB1rMJ55kt/g9Dyg==", "99b06e3b-77b0-4a4c-a516-771b95613786" });

            migrationBuilder.AddForeignKey(
                name: "FK_ExamResults_Person_StudentId",
                table: "ExamResults",
                column: "StudentId",
                principalTable: "Person",
                principalColumn: "Id");
        }
    }
}
