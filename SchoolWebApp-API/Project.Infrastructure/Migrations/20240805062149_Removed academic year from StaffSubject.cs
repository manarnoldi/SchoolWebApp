using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SchoolWebApp.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RemovedacademicyearfromStaffSubject : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_StaffSubjects_AcademicYears_AcademicYearId",
                table: "StaffSubjects");

            migrationBuilder.DropIndex(
                name: "IX_StaffSubjects_AcademicYearId",
                table: "StaffSubjects");

            migrationBuilder.DropColumn(
                name: "AcademicYearId",
                table: "StaffSubjects");

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
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "AcademicYearId",
                table: "StaffSubjects",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "269f0cf3-405e-4163-83f3-1b63ebebd62e",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2024, 8, 1, 10, 59, 20, 645, DateTimeKind.Local).AddTicks(2533), new DateTime(2024, 8, 1, 10, 59, 20, 645, DateTimeKind.Local).AddTicks(2546) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "448df289-142c-4959-a912-60733515e1b4",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2024, 8, 1, 10, 59, 20, 645, DateTimeKind.Local).AddTicks(2502), new DateTime(2024, 8, 1, 10, 59, 20, 645, DateTimeKind.Local).AddTicks(2503) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "48c50c3a-9958-453b-b649-4e21af131322",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2024, 8, 1, 10, 59, 20, 645, DateTimeKind.Local).AddTicks(2461), new DateTime(2024, 8, 1, 10, 59, 20, 645, DateTimeKind.Local).AddTicks(2462) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "717d9b15-a428-440c-b26b-08d3bbb68b02",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2024, 8, 1, 10, 59, 20, 645, DateTimeKind.Local).AddTicks(2328), new DateTime(2024, 8, 1, 10, 59, 20, 645, DateTimeKind.Local).AddTicks(2364) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "95ed2407-3e58-4af2-88a4-1c4e96473f68",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2024, 8, 1, 10, 59, 20, 645, DateTimeKind.Local).AddTicks(2429), new DateTime(2024, 8, 1, 10, 59, 20, 645, DateTimeKind.Local).AddTicks(2430) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "97942bee-ef12-4425-8225-4f293d0f36dd",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2024, 8, 1, 10, 59, 20, 645, DateTimeKind.Local).AddTicks(2620), new DateTime(2024, 8, 1, 10, 59, 20, 645, DateTimeKind.Local).AddTicks(2621) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "cd12b44b-103b-48df-8887-a2bf42e0651e",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2024, 8, 1, 10, 59, 20, 645, DateTimeKind.Local).AddTicks(2591), new DateTime(2024, 8, 1, 10, 59, 20, 645, DateTimeKind.Local).AddTicks(2593) });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "7e67d486-af3e-49f1-a109-a2b864b8e0ec",
                columns: new[] { "ConcurrencyStamp", "Created", "Modified", "PasswordHash", "SecurityStamp" },
                values: new object[] { "3c8fe575-bb64-4a32-a54e-25ad61e37730", new DateTime(2024, 8, 1, 10, 59, 20, 645, DateTimeKind.Local).AddTicks(2720), new DateTime(2024, 8, 1, 10, 59, 20, 645, DateTimeKind.Local).AddTicks(2722), "AQAAAAIAAYagAAAAECEybmq7xvMnJKNloWmpl2d6HSTZAeCB7WLygZGOcaR5EtmhcT/RfDUzsOVS4GbN+A==", "56e957d7-7f84-45e3-a9f3-d1c3a46db8ff" });

            migrationBuilder.CreateIndex(
                name: "IX_StaffSubjects_AcademicYearId",
                table: "StaffSubjects",
                column: "AcademicYearId");

            migrationBuilder.AddForeignKey(
                name: "FK_StaffSubjects_AcademicYears_AcademicYearId",
                table: "StaffSubjects",
                column: "AcademicYearId",
                principalTable: "AcademicYears",
                principalColumn: "Id");
        }
    }
}
