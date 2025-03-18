using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SchoolWebApp.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddedDatestoExamEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateOnly>(
                name: "ExamEndDate",
                table: "Exams",
                type: "date",
                nullable: false,
                defaultValue: new DateOnly(1, 1, 1));

            migrationBuilder.AddColumn<DateOnly>(
                name: "ExamMarkEntryEndDate",
                table: "Exams",
                type: "date",
                nullable: false,
                defaultValue: new DateOnly(1, 1, 1));

            migrationBuilder.AddColumn<DateOnly>(
                name: "ExamStartDate",
                table: "Exams",
                type: "date",
                nullable: false,
                defaultValue: new DateOnly(1, 1, 1));

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ExamEndDate",
                table: "Exams");

            migrationBuilder.DropColumn(
                name: "ExamMarkEntryEndDate",
                table: "Exams");

            migrationBuilder.DropColumn(
                name: "ExamStartDate",
                table: "Exams");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "269f0cf3-405e-4163-83f3-1b63ebebd62e",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2025, 3, 10, 7, 18, 59, 389, DateTimeKind.Local).AddTicks(461), new DateTime(2025, 3, 10, 7, 18, 59, 389, DateTimeKind.Local).AddTicks(488) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "448df289-142c-4959-a912-60733515e1b4",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2025, 3, 10, 7, 18, 59, 389, DateTimeKind.Local).AddTicks(389), new DateTime(2025, 3, 10, 7, 18, 59, 389, DateTimeKind.Local).AddTicks(392) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "48c50c3a-9958-453b-b649-4e21af131322",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2025, 3, 10, 7, 18, 59, 389, DateTimeKind.Local).AddTicks(332), new DateTime(2025, 3, 10, 7, 18, 59, 389, DateTimeKind.Local).AddTicks(335) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "717d9b15-a428-440c-b26b-08d3bbb68b02",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2025, 3, 10, 7, 18, 59, 389, DateTimeKind.Local).AddTicks(53), new DateTime(2025, 3, 10, 7, 18, 59, 389, DateTimeKind.Local).AddTicks(147) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "95ed2407-3e58-4af2-88a4-1c4e96473f68",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2025, 3, 10, 7, 18, 59, 389, DateTimeKind.Local).AddTicks(272), new DateTime(2025, 3, 10, 7, 18, 59, 389, DateTimeKind.Local).AddTicks(275) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "97942bee-ef12-4425-8225-4f293d0f36dd",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2025, 3, 10, 7, 18, 59, 389, DateTimeKind.Local).AddTicks(663), new DateTime(2025, 3, 10, 7, 18, 59, 389, DateTimeKind.Local).AddTicks(666) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "cd12b44b-103b-48df-8887-a2bf42e0651e",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2025, 3, 10, 7, 18, 59, 389, DateTimeKind.Local).AddTicks(584), new DateTime(2025, 3, 10, 7, 18, 59, 389, DateTimeKind.Local).AddTicks(587) });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "7e67d486-af3e-49f1-a109-a2b864b8e0ec",
                columns: new[] { "ConcurrencyStamp", "Created", "Modified", "PasswordHash", "SecurityStamp" },
                values: new object[] { "dbd50058-ccb8-46ff-843b-72c87a723b27", new DateTime(2025, 3, 10, 7, 18, 59, 389, DateTimeKind.Local).AddTicks(863), new DateTime(2025, 3, 10, 7, 18, 59, 389, DateTimeKind.Local).AddTicks(869), "AQAAAAIAAYagAAAAEEIaGADVwaQ4+tk8xw5g2ivkQVA/QfVjun8spLOmye6EzL2FRgwHQwUed+WM7XVWBA==", "8ce02d60-1970-47d4-88ca-3985a66363dd" });
        }
    }
}
