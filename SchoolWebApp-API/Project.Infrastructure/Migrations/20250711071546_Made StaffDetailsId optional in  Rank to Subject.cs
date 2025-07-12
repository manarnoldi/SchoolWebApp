using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SchoolWebApp.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class MadeStaffDetailsIdoptionalinRanktoSubject : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "StaffDetailsId",
                table: "Subjects",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "269f0cf3-405e-4163-83f3-1b63ebebd62e",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2025, 7, 11, 10, 15, 44, 358, DateTimeKind.Local).AddTicks(8911), new DateTime(2025, 7, 11, 10, 15, 44, 358, DateTimeKind.Local).AddTicks(8913) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "448df289-142c-4959-a912-60733515e1b4",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2025, 7, 11, 10, 15, 44, 358, DateTimeKind.Local).AddTicks(8863), new DateTime(2025, 7, 11, 10, 15, 44, 358, DateTimeKind.Local).AddTicks(8864) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "48c50c3a-9958-453b-b649-4e21af131322",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2025, 7, 11, 10, 15, 44, 358, DateTimeKind.Local).AddTicks(8826), new DateTime(2025, 7, 11, 10, 15, 44, 358, DateTimeKind.Local).AddTicks(8828) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "717d9b15-a428-440c-b26b-08d3bbb68b02",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2025, 7, 11, 10, 15, 44, 358, DateTimeKind.Local).AddTicks(8626), new DateTime(2025, 7, 11, 10, 15, 44, 358, DateTimeKind.Local).AddTicks(8660) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "95ed2407-3e58-4af2-88a4-1c4e96473f68",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2025, 7, 11, 10, 15, 44, 358, DateTimeKind.Local).AddTicks(8771), new DateTime(2025, 7, 11, 10, 15, 44, 358, DateTimeKind.Local).AddTicks(8773) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "97942bee-ef12-4425-8225-4f293d0f36dd",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2025, 7, 11, 10, 15, 44, 358, DateTimeKind.Local).AddTicks(8987), new DateTime(2025, 7, 11, 10, 15, 44, 358, DateTimeKind.Local).AddTicks(8988) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "cd12b44b-103b-48df-8887-a2bf42e0651e",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2025, 7, 11, 10, 15, 44, 358, DateTimeKind.Local).AddTicks(8951), new DateTime(2025, 7, 11, 10, 15, 44, 358, DateTimeKind.Local).AddTicks(8953) });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "7e67d486-af3e-49f1-a109-a2b864b8e0ec",
                columns: new[] { "ConcurrencyStamp", "Created", "Modified", "PasswordHash", "SecurityStamp" },
                values: new object[] { "9afcdc84-469f-4d27-be4d-ace123d5fece", new DateTime(2025, 7, 11, 10, 15, 44, 358, DateTimeKind.Local).AddTicks(9479), new DateTime(2025, 7, 11, 10, 15, 44, 358, DateTimeKind.Local).AddTicks(9482), "AQAAAAIAAYagAAAAEFYV0UFfByGYJn0tq7D8d1YC8fQZ4WaYfX0lLiNosFPf9NINGhrxlvzkICKp6bxl/w==", "559ef81b-2a80-4457-907a-a846acd2fd0f" });

            migrationBuilder.UpdateData(
                table: "Designations",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2025, 7, 11, 10, 15, 44, 358, DateTimeKind.Local).AddTicks(9095), new DateTime(2025, 7, 11, 10, 15, 44, 358, DateTimeKind.Local).AddTicks(9096) });

            migrationBuilder.UpdateData(
                table: "EmploymentTypes",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2025, 7, 11, 10, 15, 44, 358, DateTimeKind.Local).AddTicks(9137), new DateTime(2025, 7, 11, 10, 15, 44, 358, DateTimeKind.Local).AddTicks(9138) });

            migrationBuilder.UpdateData(
                table: "Genders",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2025, 7, 11, 10, 15, 44, 358, DateTimeKind.Local).AddTicks(9266), new DateTime(2025, 7, 11, 10, 15, 44, 358, DateTimeKind.Local).AddTicks(9268) });

            migrationBuilder.UpdateData(
                table: "Nationalities",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2025, 7, 11, 10, 15, 44, 358, DateTimeKind.Local).AddTicks(9184), new DateTime(2025, 7, 11, 10, 15, 44, 358, DateTimeKind.Local).AddTicks(9185) });

            migrationBuilder.UpdateData(
                table: "Person",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "DateOfBirth", "EmploymentDate", "EndofEmploymentDate", "Modified" },
                values: new object[] { new DateTime(2025, 7, 11, 10, 15, 44, 358, DateTimeKind.Local).AddTicks(9339), new DateTime(2025, 7, 11, 10, 15, 44, 358, DateTimeKind.Local).AddTicks(9334), new DateTime(2025, 7, 11, 10, 15, 44, 358, DateTimeKind.Local).AddTicks(9326), new DateTime(2025, 7, 11, 10, 15, 44, 358, DateTimeKind.Local).AddTicks(9328), new DateTime(2025, 7, 11, 10, 15, 44, 358, DateTimeKind.Local).AddTicks(9351) });

            migrationBuilder.UpdateData(
                table: "Religions",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2025, 7, 11, 10, 15, 44, 358, DateTimeKind.Local).AddTicks(9227), new DateTime(2025, 7, 11, 10, 15, 44, 358, DateTimeKind.Local).AddTicks(9229) });

            migrationBuilder.UpdateData(
                table: "StaffCategories",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2025, 7, 11, 10, 15, 44, 358, DateTimeKind.Local).AddTicks(9051), new DateTime(2025, 7, 11, 10, 15, 44, 358, DateTimeKind.Local).AddTicks(9054) });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "StaffDetailsId",
                table: "Subjects",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "269f0cf3-405e-4163-83f3-1b63ebebd62e",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2025, 7, 11, 8, 44, 54, 410, DateTimeKind.Local).AddTicks(8599), new DateTime(2025, 7, 11, 8, 44, 54, 410, DateTimeKind.Local).AddTicks(8602) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "448df289-142c-4959-a912-60733515e1b4",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2025, 7, 11, 8, 44, 54, 410, DateTimeKind.Local).AddTicks(8451), new DateTime(2025, 7, 11, 8, 44, 54, 410, DateTimeKind.Local).AddTicks(8454) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "48c50c3a-9958-453b-b649-4e21af131322",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2025, 7, 11, 8, 44, 54, 410, DateTimeKind.Local).AddTicks(8372), new DateTime(2025, 7, 11, 8, 44, 54, 410, DateTimeKind.Local).AddTicks(8375) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "717d9b15-a428-440c-b26b-08d3bbb68b02",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2025, 7, 11, 8, 44, 54, 410, DateTimeKind.Local).AddTicks(7982), new DateTime(2025, 7, 11, 8, 44, 54, 410, DateTimeKind.Local).AddTicks(8049) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "95ed2407-3e58-4af2-88a4-1c4e96473f68",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2025, 7, 11, 8, 44, 54, 410, DateTimeKind.Local).AddTicks(8281), new DateTime(2025, 7, 11, 8, 44, 54, 410, DateTimeKind.Local).AddTicks(8284) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "97942bee-ef12-4425-8225-4f293d0f36dd",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2025, 7, 11, 8, 44, 54, 410, DateTimeKind.Local).AddTicks(8775), new DateTime(2025, 7, 11, 8, 44, 54, 410, DateTimeKind.Local).AddTicks(8777) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "cd12b44b-103b-48df-8887-a2bf42e0651e",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2025, 7, 11, 8, 44, 54, 410, DateTimeKind.Local).AddTicks(8697), new DateTime(2025, 7, 11, 8, 44, 54, 410, DateTimeKind.Local).AddTicks(8700) });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "7e67d486-af3e-49f1-a109-a2b864b8e0ec",
                columns: new[] { "ConcurrencyStamp", "Created", "Modified", "PasswordHash", "SecurityStamp" },
                values: new object[] { "67cb2780-1203-42ab-8b65-f397845d717f", new DateTime(2025, 7, 11, 8, 44, 54, 410, DateTimeKind.Local).AddTicks(9660), new DateTime(2025, 7, 11, 8, 44, 54, 410, DateTimeKind.Local).AddTicks(9666), "AQAAAAIAAYagAAAAEEH2/h80U234UNa+U5XI0Bd+5YtIavBZKQhEnpG/2RHN1ibh8y8osq/HmNSkwMrELw==", "875aef35-05b3-4074-bbd2-d716a52f39d7" });

            migrationBuilder.UpdateData(
                table: "Designations",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2025, 7, 11, 8, 44, 54, 410, DateTimeKind.Local).AddTicks(8992), new DateTime(2025, 7, 11, 8, 44, 54, 410, DateTimeKind.Local).AddTicks(8994) });

            migrationBuilder.UpdateData(
                table: "EmploymentTypes",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2025, 7, 11, 8, 44, 54, 410, DateTimeKind.Local).AddTicks(9082), new DateTime(2025, 7, 11, 8, 44, 54, 410, DateTimeKind.Local).AddTicks(9086) });

            migrationBuilder.UpdateData(
                table: "Genders",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2025, 7, 11, 8, 44, 54, 410, DateTimeKind.Local).AddTicks(9303), new DateTime(2025, 7, 11, 8, 44, 54, 410, DateTimeKind.Local).AddTicks(9307) });

            migrationBuilder.UpdateData(
                table: "Nationalities",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2025, 7, 11, 8, 44, 54, 410, DateTimeKind.Local).AddTicks(9157), new DateTime(2025, 7, 11, 8, 44, 54, 410, DateTimeKind.Local).AddTicks(9161) });

            migrationBuilder.UpdateData(
                table: "Person",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "DateOfBirth", "EmploymentDate", "EndofEmploymentDate", "Modified" },
                values: new object[] { new DateTime(2025, 7, 11, 8, 44, 54, 410, DateTimeKind.Local).AddTicks(9454), new DateTime(2025, 7, 11, 8, 44, 54, 410, DateTimeKind.Local).AddTicks(9433), new DateTime(2025, 7, 11, 8, 44, 54, 410, DateTimeKind.Local).AddTicks(9422), new DateTime(2025, 7, 11, 8, 44, 54, 410, DateTimeKind.Local).AddTicks(9425), new DateTime(2025, 7, 11, 8, 44, 54, 410, DateTimeKind.Local).AddTicks(9466) });

            migrationBuilder.UpdateData(
                table: "Religions",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2025, 7, 11, 8, 44, 54, 410, DateTimeKind.Local).AddTicks(9235), new DateTime(2025, 7, 11, 8, 44, 54, 410, DateTimeKind.Local).AddTicks(9239) });

            migrationBuilder.UpdateData(
                table: "StaffCategories",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2025, 7, 11, 8, 44, 54, 410, DateTimeKind.Local).AddTicks(8898), new DateTime(2025, 7, 11, 8, 44, 54, 410, DateTimeKind.Local).AddTicks(8903) });
        }
    }
}
