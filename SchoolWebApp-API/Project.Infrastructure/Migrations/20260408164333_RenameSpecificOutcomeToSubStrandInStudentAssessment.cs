using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SchoolWebApp.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RenameSpecificOutcomeToSubStrandInStudentAssessment : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_StudentAssessments_SpecificOutcomes_SpecificOutcomeId",
                table: "StudentAssessments");

            migrationBuilder.RenameColumn(
                name: "SpecificOutcomeId",
                table: "StudentAssessments",
                newName: "SubStrandId");

            migrationBuilder.RenameIndex(
                name: "IX_StudentAssessments_SpecificOutcomeId",
                table: "StudentAssessments",
                newName: "IX_StudentAssessments_SubStrandId");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "269f0cf3-405e-4163-83f3-1b63ebebd62e",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 8, 19, 43, 30, 705, DateTimeKind.Local).AddTicks(4376), new DateTime(2026, 4, 8, 19, 43, 30, 705, DateTimeKind.Local).AddTicks(4377) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "448df289-142c-4959-a912-60733515e1b4",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 8, 19, 43, 30, 705, DateTimeKind.Local).AddTicks(4331), new DateTime(2026, 4, 8, 19, 43, 30, 705, DateTimeKind.Local).AddTicks(4333) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "48c50c3a-9958-453b-b649-4e21af131322",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 8, 19, 43, 30, 705, DateTimeKind.Local).AddTicks(4277), new DateTime(2026, 4, 8, 19, 43, 30, 705, DateTimeKind.Local).AddTicks(4278) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "717d9b15-a428-440c-b26b-08d3bbb68b02",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 8, 19, 43, 30, 705, DateTimeKind.Local).AddTicks(4110), new DateTime(2026, 4, 8, 19, 43, 30, 705, DateTimeKind.Local).AddTicks(4127) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "95ed2407-3e58-4af2-88a4-1c4e96473f68",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 8, 19, 43, 30, 705, DateTimeKind.Local).AddTicks(4228), new DateTime(2026, 4, 8, 19, 43, 30, 705, DateTimeKind.Local).AddTicks(4230) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "97942bee-ef12-4425-8225-4f293d0f36dd",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 8, 19, 43, 30, 705, DateTimeKind.Local).AddTicks(4464), new DateTime(2026, 4, 8, 19, 43, 30, 705, DateTimeKind.Local).AddTicks(4465) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "cd12b44b-103b-48df-8887-a2bf42e0651e",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 8, 19, 43, 30, 705, DateTimeKind.Local).AddTicks(4420), new DateTime(2026, 4, 8, 19, 43, 30, 705, DateTimeKind.Local).AddTicks(4422) });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "7e67d486-af3e-49f1-a109-a2b864b8e0ec",
                columns: new[] { "ConcurrencyStamp", "Created", "Modified", "PasswordHash", "SecurityStamp" },
                values: new object[] { "805a85ae-34d8-468a-9df9-4e7073a0bf36", new DateTime(2026, 4, 8, 19, 43, 30, 705, DateTimeKind.Local).AddTicks(4896), new DateTime(2026, 4, 8, 19, 43, 30, 705, DateTimeKind.Local).AddTicks(4899), "AQAAAAIAAYagAAAAEOfg9jo+IoNrRXeVZetotgswuPi/cQiwWA9b9+SNvuCcfxLkzoIxZQqW0yXozhvmiA==", "dd61c7d4-bd8e-4735-80a5-67aa44f828b1" });

            migrationBuilder.UpdateData(
                table: "Designations",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 8, 19, 43, 30, 705, DateTimeKind.Local).AddTicks(4558), new DateTime(2026, 4, 8, 19, 43, 30, 705, DateTimeKind.Local).AddTicks(4559) });

            migrationBuilder.UpdateData(
                table: "EmploymentTypes",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 8, 19, 43, 30, 705, DateTimeKind.Local).AddTicks(4598), new DateTime(2026, 4, 8, 19, 43, 30, 705, DateTimeKind.Local).AddTicks(4600) });

            migrationBuilder.UpdateData(
                table: "Genders",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 8, 19, 43, 30, 705, DateTimeKind.Local).AddTicks(4716), new DateTime(2026, 4, 8, 19, 43, 30, 705, DateTimeKind.Local).AddTicks(4718) });

            migrationBuilder.UpdateData(
                table: "Nationalities",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 8, 19, 43, 30, 705, DateTimeKind.Local).AddTicks(4641), new DateTime(2026, 4, 8, 19, 43, 30, 705, DateTimeKind.Local).AddTicks(4643) });

            migrationBuilder.UpdateData(
                table: "Person",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "DateOfBirth", "EmploymentDate", "EndofEmploymentDate", "Modified" },
                values: new object[] { new DateTime(2026, 4, 8, 19, 43, 30, 705, DateTimeKind.Local).AddTicks(4795), new DateTime(2026, 4, 8, 19, 43, 30, 705, DateTimeKind.Local).AddTicks(4793), new DateTime(2026, 4, 8, 19, 43, 30, 705, DateTimeKind.Local).AddTicks(4788), new DateTime(2026, 4, 8, 19, 43, 30, 705, DateTimeKind.Local).AddTicks(4790), new DateTime(2026, 4, 8, 19, 43, 30, 705, DateTimeKind.Local).AddTicks(4802) });

            migrationBuilder.UpdateData(
                table: "Religions",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 8, 19, 43, 30, 705, DateTimeKind.Local).AddTicks(4681), new DateTime(2026, 4, 8, 19, 43, 30, 705, DateTimeKind.Local).AddTicks(4682) });

            migrationBuilder.UpdateData(
                table: "StaffCategories",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 8, 19, 43, 30, 705, DateTimeKind.Local).AddTicks(4523), new DateTime(2026, 4, 8, 19, 43, 30, 705, DateTimeKind.Local).AddTicks(4525) });

            migrationBuilder.AddForeignKey(
                name: "FK_StudentAssessments_SubStrands_SubStrandId",
                table: "StudentAssessments",
                column: "SubStrandId",
                principalTable: "SubStrands",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_StudentAssessments_SubStrands_SubStrandId",
                table: "StudentAssessments");

            migrationBuilder.RenameColumn(
                name: "SubStrandId",
                table: "StudentAssessments",
                newName: "SpecificOutcomeId");

            migrationBuilder.RenameIndex(
                name: "IX_StudentAssessments_SubStrandId",
                table: "StudentAssessments",
                newName: "IX_StudentAssessments_SpecificOutcomeId");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "269f0cf3-405e-4163-83f3-1b63ebebd62e",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 8, 16, 4, 5, 401, DateTimeKind.Local).AddTicks(4897), new DateTime(2026, 4, 8, 16, 4, 5, 401, DateTimeKind.Local).AddTicks(4898) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "448df289-142c-4959-a912-60733515e1b4",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 8, 16, 4, 5, 401, DateTimeKind.Local).AddTicks(4880), new DateTime(2026, 4, 8, 16, 4, 5, 401, DateTimeKind.Local).AddTicks(4881) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "48c50c3a-9958-453b-b649-4e21af131322",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 8, 16, 4, 5, 401, DateTimeKind.Local).AddTicks(4864), new DateTime(2026, 4, 8, 16, 4, 5, 401, DateTimeKind.Local).AddTicks(4865) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "717d9b15-a428-440c-b26b-08d3bbb68b02",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 8, 16, 4, 5, 401, DateTimeKind.Local).AddTicks(4786), new DateTime(2026, 4, 8, 16, 4, 5, 401, DateTimeKind.Local).AddTicks(4807) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "95ed2407-3e58-4af2-88a4-1c4e96473f68",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 8, 16, 4, 5, 401, DateTimeKind.Local).AddTicks(4845), new DateTime(2026, 4, 8, 16, 4, 5, 401, DateTimeKind.Local).AddTicks(4847) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "97942bee-ef12-4425-8225-4f293d0f36dd",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 8, 16, 4, 5, 401, DateTimeKind.Local).AddTicks(4930), new DateTime(2026, 4, 8, 16, 4, 5, 401, DateTimeKind.Local).AddTicks(4931) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "cd12b44b-103b-48df-8887-a2bf42e0651e",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 8, 16, 4, 5, 401, DateTimeKind.Local).AddTicks(4913), new DateTime(2026, 4, 8, 16, 4, 5, 401, DateTimeKind.Local).AddTicks(4914) });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "7e67d486-af3e-49f1-a109-a2b864b8e0ec",
                columns: new[] { "ConcurrencyStamp", "Created", "Modified", "PasswordHash", "SecurityStamp" },
                values: new object[] { "44deb6ed-d25a-436f-ab68-e2d319380020", new DateTime(2026, 4, 8, 16, 4, 5, 401, DateTimeKind.Local).AddTicks(5175), new DateTime(2026, 4, 8, 16, 4, 5, 401, DateTimeKind.Local).AddTicks(5177), "AQAAAAIAAYagAAAAEA6/PTOOX1NhDST3l3O7Vwfkjmgezsz+s70alll6IgSPmZUgi0a7/LoWcfLoyR2PYQ==", "60755224-c13f-48fe-9870-d8c4961fa909" });

            migrationBuilder.UpdateData(
                table: "Designations",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 8, 16, 4, 5, 401, DateTimeKind.Local).AddTicks(4982), new DateTime(2026, 4, 8, 16, 4, 5, 401, DateTimeKind.Local).AddTicks(4983) });

            migrationBuilder.UpdateData(
                table: "EmploymentTypes",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 8, 16, 4, 5, 401, DateTimeKind.Local).AddTicks(5010), new DateTime(2026, 4, 8, 16, 4, 5, 401, DateTimeKind.Local).AddTicks(5011) });

            migrationBuilder.UpdateData(
                table: "Genders",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 8, 16, 4, 5, 401, DateTimeKind.Local).AddTicks(5077), new DateTime(2026, 4, 8, 16, 4, 5, 401, DateTimeKind.Local).AddTicks(5078) });

            migrationBuilder.UpdateData(
                table: "Nationalities",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 8, 16, 4, 5, 401, DateTimeKind.Local).AddTicks(5039), new DateTime(2026, 4, 8, 16, 4, 5, 401, DateTimeKind.Local).AddTicks(5040) });

            migrationBuilder.UpdateData(
                table: "Person",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "DateOfBirth", "EmploymentDate", "EndofEmploymentDate", "Modified" },
                values: new object[] { new DateTime(2026, 4, 8, 16, 4, 5, 401, DateTimeKind.Local).AddTicks(5109), new DateTime(2026, 4, 8, 16, 4, 5, 401, DateTimeKind.Local).AddTicks(5107), new DateTime(2026, 4, 8, 16, 4, 5, 401, DateTimeKind.Local).AddTicks(5103), new DateTime(2026, 4, 8, 16, 4, 5, 401, DateTimeKind.Local).AddTicks(5103), new DateTime(2026, 4, 8, 16, 4, 5, 401, DateTimeKind.Local).AddTicks(5110) });

            migrationBuilder.UpdateData(
                table: "Religions",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 8, 16, 4, 5, 401, DateTimeKind.Local).AddTicks(5061), new DateTime(2026, 4, 8, 16, 4, 5, 401, DateTimeKind.Local).AddTicks(5061) });

            migrationBuilder.UpdateData(
                table: "StaffCategories",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 8, 16, 4, 5, 401, DateTimeKind.Local).AddTicks(4961), new DateTime(2026, 4, 8, 16, 4, 5, 401, DateTimeKind.Local).AddTicks(4963) });

            migrationBuilder.AddForeignKey(
                name: "FK_StudentAssessments_SpecificOutcomes_SpecificOutcomeId",
                table: "StudentAssessments",
                column: "SpecificOutcomeId",
                principalTable: "SpecificOutcomes",
                principalColumn: "Id");
        }
    }
}
