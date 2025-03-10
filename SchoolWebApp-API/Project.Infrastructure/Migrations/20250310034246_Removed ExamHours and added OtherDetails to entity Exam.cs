using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SchoolWebApp.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RemovedExamHoursandaddedOtherDetailstoentityExam : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ExamHours",
                table: "Exams");

            migrationBuilder.AddColumn<string>(
                name: "OtherDetails",
                table: "Exams",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "269f0cf3-405e-4163-83f3-1b63ebebd62e",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2025, 3, 10, 6, 42, 38, 299, DateTimeKind.Local).AddTicks(9260), new DateTime(2025, 3, 10, 6, 42, 38, 299, DateTimeKind.Local).AddTicks(9291) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "448df289-142c-4959-a912-60733515e1b4",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2025, 3, 10, 6, 42, 38, 299, DateTimeKind.Local).AddTicks(8978), new DateTime(2025, 3, 10, 6, 42, 38, 299, DateTimeKind.Local).AddTicks(8983) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "48c50c3a-9958-453b-b649-4e21af131322",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2025, 3, 10, 6, 42, 38, 299, DateTimeKind.Local).AddTicks(8855), new DateTime(2025, 3, 10, 6, 42, 38, 299, DateTimeKind.Local).AddTicks(8860) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "717d9b15-a428-440c-b26b-08d3bbb68b02",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2025, 3, 10, 6, 42, 38, 299, DateTimeKind.Local).AddTicks(8013), new DateTime(2025, 3, 10, 6, 42, 38, 299, DateTimeKind.Local).AddTicks(8174) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "95ed2407-3e58-4af2-88a4-1c4e96473f68",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2025, 3, 10, 6, 42, 38, 299, DateTimeKind.Local).AddTicks(8711), new DateTime(2025, 3, 10, 6, 42, 38, 299, DateTimeKind.Local).AddTicks(8717) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "97942bee-ef12-4425-8225-4f293d0f36dd",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2025, 3, 10, 6, 42, 38, 299, DateTimeKind.Local).AddTicks(9586), new DateTime(2025, 3, 10, 6, 42, 38, 299, DateTimeKind.Local).AddTicks(9592) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "cd12b44b-103b-48df-8887-a2bf42e0651e",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2025, 3, 10, 6, 42, 38, 299, DateTimeKind.Local).AddTicks(9462), new DateTime(2025, 3, 10, 6, 42, 38, 299, DateTimeKind.Local).AddTicks(9468) });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "7e67d486-af3e-49f1-a109-a2b864b8e0ec",
                columns: new[] { "ConcurrencyStamp", "Created", "Modified", "PasswordHash", "SecurityStamp" },
                values: new object[] { "47f034f2-1a43-4e88-937b-c03041d90342", new DateTime(2025, 3, 10, 6, 42, 38, 300, DateTimeKind.Local).AddTicks(65), new DateTime(2025, 3, 10, 6, 42, 38, 300, DateTimeKind.Local).AddTicks(74), "AQAAAAIAAYagAAAAEPtJk+kHiOonSf8544e4W26QPSU24uDgbgGcCHVequYfQz9MCZ5SBns/MHiVRuYdjQ==", "f9241d48-626c-4b19-846d-e67869f398a1" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OtherDetails",
                table: "Exams");

            migrationBuilder.AddColumn<float>(
                name: "ExamHours",
                table: "Exams",
                type: "float",
                nullable: false,
                defaultValue: 0f);

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
        }
    }
}
