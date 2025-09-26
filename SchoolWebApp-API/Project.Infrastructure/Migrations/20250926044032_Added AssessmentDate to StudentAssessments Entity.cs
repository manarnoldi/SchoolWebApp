using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SchoolWebApp.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddedAssessmentDatetoStudentAssessmentsEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "AssessmentDate",
                table: "StudentAssessments",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "StaffDetailsId",
                table: "StudentAssessments",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "269f0cf3-405e-4163-83f3-1b63ebebd62e",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2025, 9, 26, 7, 40, 30, 475, DateTimeKind.Local).AddTicks(2104), new DateTime(2025, 9, 26, 7, 40, 30, 475, DateTimeKind.Local).AddTicks(2105) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "448df289-142c-4959-a912-60733515e1b4",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2025, 9, 26, 7, 40, 30, 475, DateTimeKind.Local).AddTicks(2074), new DateTime(2025, 9, 26, 7, 40, 30, 475, DateTimeKind.Local).AddTicks(2075) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "48c50c3a-9958-453b-b649-4e21af131322",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2025, 9, 26, 7, 40, 30, 475, DateTimeKind.Local).AddTicks(2055), new DateTime(2025, 9, 26, 7, 40, 30, 475, DateTimeKind.Local).AddTicks(2056) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "717d9b15-a428-440c-b26b-08d3bbb68b02",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2025, 9, 26, 7, 40, 30, 475, DateTimeKind.Local).AddTicks(1987), new DateTime(2025, 9, 26, 7, 40, 30, 475, DateTimeKind.Local).AddTicks(2002) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "95ed2407-3e58-4af2-88a4-1c4e96473f68",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2025, 9, 26, 7, 40, 30, 475, DateTimeKind.Local).AddTicks(2036), new DateTime(2025, 9, 26, 7, 40, 30, 475, DateTimeKind.Local).AddTicks(2037) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "97942bee-ef12-4425-8225-4f293d0f36dd",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2025, 9, 26, 7, 40, 30, 475, DateTimeKind.Local).AddTicks(2141), new DateTime(2025, 9, 26, 7, 40, 30, 475, DateTimeKind.Local).AddTicks(2142) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "cd12b44b-103b-48df-8887-a2bf42e0651e",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2025, 9, 26, 7, 40, 30, 475, DateTimeKind.Local).AddTicks(2123), new DateTime(2025, 9, 26, 7, 40, 30, 475, DateTimeKind.Local).AddTicks(2125) });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "7e67d486-af3e-49f1-a109-a2b864b8e0ec",
                columns: new[] { "ConcurrencyStamp", "Created", "Modified", "PasswordHash", "SecurityStamp" },
                values: new object[] { "92dfc7ea-8656-4193-bf14-b720b5527424", new DateTime(2025, 9, 26, 7, 40, 30, 475, DateTimeKind.Local).AddTicks(2351), new DateTime(2025, 9, 26, 7, 40, 30, 475, DateTimeKind.Local).AddTicks(2353), "AQAAAAIAAYagAAAAENJ/cUj9li32HENRbk6gI8jgRzMdA7i1A0Xbfm6po+x4Z8Uhj3gkkbSzrOPAqBgG5w==", "0b9f5df0-5913-4067-b7bb-9730d61d98f4" });

            migrationBuilder.UpdateData(
                table: "Designations",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2025, 9, 26, 7, 40, 30, 475, DateTimeKind.Local).AddTicks(2189), new DateTime(2025, 9, 26, 7, 40, 30, 475, DateTimeKind.Local).AddTicks(2190) });

            migrationBuilder.UpdateData(
                table: "EmploymentTypes",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2025, 9, 26, 7, 40, 30, 475, DateTimeKind.Local).AddTicks(2209), new DateTime(2025, 9, 26, 7, 40, 30, 475, DateTimeKind.Local).AddTicks(2210) });

            migrationBuilder.UpdateData(
                table: "Genders",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2025, 9, 26, 7, 40, 30, 475, DateTimeKind.Local).AddTicks(2262), new DateTime(2025, 9, 26, 7, 40, 30, 475, DateTimeKind.Local).AddTicks(2262) });

            migrationBuilder.UpdateData(
                table: "Nationalities",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2025, 9, 26, 7, 40, 30, 475, DateTimeKind.Local).AddTicks(2228), new DateTime(2025, 9, 26, 7, 40, 30, 475, DateTimeKind.Local).AddTicks(2228) });

            migrationBuilder.UpdateData(
                table: "Person",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "DateOfBirth", "EmploymentDate", "EndofEmploymentDate", "Modified" },
                values: new object[] { new DateTime(2025, 9, 26, 7, 40, 30, 475, DateTimeKind.Local).AddTicks(2297), new DateTime(2025, 9, 26, 7, 40, 30, 475, DateTimeKind.Local).AddTicks(2294), new DateTime(2025, 9, 26, 7, 40, 30, 475, DateTimeKind.Local).AddTicks(2290), new DateTime(2025, 9, 26, 7, 40, 30, 475, DateTimeKind.Local).AddTicks(2291), new DateTime(2025, 9, 26, 7, 40, 30, 475, DateTimeKind.Local).AddTicks(2297) });

            migrationBuilder.UpdateData(
                table: "Religions",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2025, 9, 26, 7, 40, 30, 475, DateTimeKind.Local).AddTicks(2245), new DateTime(2025, 9, 26, 7, 40, 30, 475, DateTimeKind.Local).AddTicks(2246) });

            migrationBuilder.UpdateData(
                table: "StaffCategories",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2025, 9, 26, 7, 40, 30, 475, DateTimeKind.Local).AddTicks(2171), new DateTime(2025, 9, 26, 7, 40, 30, 475, DateTimeKind.Local).AddTicks(2173) });

            migrationBuilder.CreateIndex(
                name: "IX_StudentAssessments_StaffDetailsId",
                table: "StudentAssessments",
                column: "StaffDetailsId");

            migrationBuilder.AddForeignKey(
                name: "FK_StudentAssessments_Person_StaffDetailsId",
                table: "StudentAssessments",
                column: "StaffDetailsId",
                principalTable: "Person",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_StudentAssessments_Person_StaffDetailsId",
                table: "StudentAssessments");

            migrationBuilder.DropIndex(
                name: "IX_StudentAssessments_StaffDetailsId",
                table: "StudentAssessments");

            migrationBuilder.DropColumn(
                name: "AssessmentDate",
                table: "StudentAssessments");

            migrationBuilder.DropColumn(
                name: "StaffDetailsId",
                table: "StudentAssessments");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "269f0cf3-405e-4163-83f3-1b63ebebd62e",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2025, 9, 26, 5, 15, 1, 99, DateTimeKind.Local).AddTicks(6954), new DateTime(2025, 9, 26, 5, 15, 1, 99, DateTimeKind.Local).AddTicks(6956) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "448df289-142c-4959-a912-60733515e1b4",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2025, 9, 26, 5, 15, 1, 99, DateTimeKind.Local).AddTicks(6935), new DateTime(2025, 9, 26, 5, 15, 1, 99, DateTimeKind.Local).AddTicks(6936) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "48c50c3a-9958-453b-b649-4e21af131322",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2025, 9, 26, 5, 15, 1, 99, DateTimeKind.Local).AddTicks(6914), new DateTime(2025, 9, 26, 5, 15, 1, 99, DateTimeKind.Local).AddTicks(6916) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "717d9b15-a428-440c-b26b-08d3bbb68b02",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2025, 9, 26, 5, 15, 1, 99, DateTimeKind.Local).AddTicks(6838), new DateTime(2025, 9, 26, 5, 15, 1, 99, DateTimeKind.Local).AddTicks(6852) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "95ed2407-3e58-4af2-88a4-1c4e96473f68",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2025, 9, 26, 5, 15, 1, 99, DateTimeKind.Local).AddTicks(6887), new DateTime(2025, 9, 26, 5, 15, 1, 99, DateTimeKind.Local).AddTicks(6889) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "97942bee-ef12-4425-8225-4f293d0f36dd",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2025, 9, 26, 5, 15, 1, 99, DateTimeKind.Local).AddTicks(7001), new DateTime(2025, 9, 26, 5, 15, 1, 99, DateTimeKind.Local).AddTicks(7002) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "cd12b44b-103b-48df-8887-a2bf42e0651e",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2025, 9, 26, 5, 15, 1, 99, DateTimeKind.Local).AddTicks(6982), new DateTime(2025, 9, 26, 5, 15, 1, 99, DateTimeKind.Local).AddTicks(6984) });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "7e67d486-af3e-49f1-a109-a2b864b8e0ec",
                columns: new[] { "ConcurrencyStamp", "Created", "Modified", "PasswordHash", "SecurityStamp" },
                values: new object[] { "9c3faf87-9e08-4399-9410-8a90f49e45fd", new DateTime(2025, 9, 26, 5, 15, 1, 99, DateTimeKind.Local).AddTicks(7208), new DateTime(2025, 9, 26, 5, 15, 1, 99, DateTimeKind.Local).AddTicks(7209), "AQAAAAIAAYagAAAAEKDO1s9DS5cB+RdBW6NZseiXvzymylYvoB+eZUPAksJwQHfoyvymZyexTpyzj7Yb7Q==", "d61bba51-5ff7-453c-ace0-d362feb5dee8" });

            migrationBuilder.UpdateData(
                table: "Designations",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2025, 9, 26, 5, 15, 1, 99, DateTimeKind.Local).AddTicks(7051), new DateTime(2025, 9, 26, 5, 15, 1, 99, DateTimeKind.Local).AddTicks(7052) });

            migrationBuilder.UpdateData(
                table: "EmploymentTypes",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2025, 9, 26, 5, 15, 1, 99, DateTimeKind.Local).AddTicks(7071), new DateTime(2025, 9, 26, 5, 15, 1, 99, DateTimeKind.Local).AddTicks(7072) });

            migrationBuilder.UpdateData(
                table: "Genders",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2025, 9, 26, 5, 15, 1, 99, DateTimeKind.Local).AddTicks(7124), new DateTime(2025, 9, 26, 5, 15, 1, 99, DateTimeKind.Local).AddTicks(7124) });

            migrationBuilder.UpdateData(
                table: "Nationalities",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2025, 9, 26, 5, 15, 1, 99, DateTimeKind.Local).AddTicks(7090), new DateTime(2025, 9, 26, 5, 15, 1, 99, DateTimeKind.Local).AddTicks(7091) });

            migrationBuilder.UpdateData(
                table: "Person",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "DateOfBirth", "EmploymentDate", "EndofEmploymentDate", "Modified" },
                values: new object[] { new DateTime(2025, 9, 26, 5, 15, 1, 99, DateTimeKind.Local).AddTicks(7150), new DateTime(2025, 9, 26, 5, 15, 1, 99, DateTimeKind.Local).AddTicks(7148), new DateTime(2025, 9, 26, 5, 15, 1, 99, DateTimeKind.Local).AddTicks(7145), new DateTime(2025, 9, 26, 5, 15, 1, 99, DateTimeKind.Local).AddTicks(7145), new DateTime(2025, 9, 26, 5, 15, 1, 99, DateTimeKind.Local).AddTicks(7151) });

            migrationBuilder.UpdateData(
                table: "Religions",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2025, 9, 26, 5, 15, 1, 99, DateTimeKind.Local).AddTicks(7107), new DateTime(2025, 9, 26, 5, 15, 1, 99, DateTimeKind.Local).AddTicks(7108) });

            migrationBuilder.UpdateData(
                table: "StaffCategories",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2025, 9, 26, 5, 15, 1, 99, DateTimeKind.Local).AddTicks(7030), new DateTime(2025, 9, 26, 5, 15, 1, 99, DateTimeKind.Local).AddTicks(7032) });
        }
    }
}
