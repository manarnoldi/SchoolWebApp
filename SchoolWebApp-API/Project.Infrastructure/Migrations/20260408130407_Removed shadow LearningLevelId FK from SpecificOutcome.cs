using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SchoolWebApp.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RemovedshadowLearningLevelIdFKfromSpecificOutcome : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Schema changes not needed - shadow FK/column never existed in DB.
            // This migration only updates the EF model snapshot.

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
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "LearningLevelId",
                table: "SpecificOutcomes",
                type: "int",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "269f0cf3-405e-4163-83f3-1b63ebebd62e",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 8, 10, 37, 26, 233, DateTimeKind.Local).AddTicks(4767), new DateTime(2026, 4, 8, 10, 37, 26, 233, DateTimeKind.Local).AddTicks(4769) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "448df289-142c-4959-a912-60733515e1b4",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 8, 10, 37, 26, 233, DateTimeKind.Local).AddTicks(4722), new DateTime(2026, 4, 8, 10, 37, 26, 233, DateTimeKind.Local).AddTicks(4724) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "48c50c3a-9958-453b-b649-4e21af131322",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 8, 10, 37, 26, 233, DateTimeKind.Local).AddTicks(4636), new DateTime(2026, 4, 8, 10, 37, 26, 233, DateTimeKind.Local).AddTicks(4638) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "717d9b15-a428-440c-b26b-08d3bbb68b02",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 8, 10, 37, 26, 233, DateTimeKind.Local).AddTicks(4454), new DateTime(2026, 4, 8, 10, 37, 26, 233, DateTimeKind.Local).AddTicks(4471) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "95ed2407-3e58-4af2-88a4-1c4e96473f68",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 8, 10, 37, 26, 233, DateTimeKind.Local).AddTicks(4573), new DateTime(2026, 4, 8, 10, 37, 26, 233, DateTimeKind.Local).AddTicks(4576) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "97942bee-ef12-4425-8225-4f293d0f36dd",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 8, 10, 37, 26, 233, DateTimeKind.Local).AddTicks(4858), new DateTime(2026, 4, 8, 10, 37, 26, 233, DateTimeKind.Local).AddTicks(4859) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "cd12b44b-103b-48df-8887-a2bf42e0651e",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 8, 10, 37, 26, 233, DateTimeKind.Local).AddTicks(4814), new DateTime(2026, 4, 8, 10, 37, 26, 233, DateTimeKind.Local).AddTicks(4816) });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "7e67d486-af3e-49f1-a109-a2b864b8e0ec",
                columns: new[] { "ConcurrencyStamp", "Created", "Modified", "PasswordHash", "SecurityStamp" },
                values: new object[] { "9df7d960-f901-4d83-8543-21d0001b15aa", new DateTime(2026, 4, 8, 10, 37, 26, 233, DateTimeKind.Local).AddTicks(5326), new DateTime(2026, 4, 8, 10, 37, 26, 233, DateTimeKind.Local).AddTicks(5328), "AQAAAAIAAYagAAAAEEy8o+jpd+fXLm+ivKPu3n3TzU07vLHCvSuOZM4+YOJp6Zcv73RkhwHjRWWnAEk/Eg==", "242aaa3b-1972-47c9-85d6-82dd0608a682" });

            migrationBuilder.UpdateData(
                table: "Designations",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 8, 10, 37, 26, 233, DateTimeKind.Local).AddTicks(4964), new DateTime(2026, 4, 8, 10, 37, 26, 233, DateTimeKind.Local).AddTicks(4965) });

            migrationBuilder.UpdateData(
                table: "EmploymentTypes",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 8, 10, 37, 26, 233, DateTimeKind.Local).AddTicks(5014), new DateTime(2026, 4, 8, 10, 37, 26, 233, DateTimeKind.Local).AddTicks(5015) });

            migrationBuilder.UpdateData(
                table: "Genders",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 8, 10, 37, 26, 233, DateTimeKind.Local).AddTicks(5138), new DateTime(2026, 4, 8, 10, 37, 26, 233, DateTimeKind.Local).AddTicks(5140) });

            migrationBuilder.UpdateData(
                table: "Nationalities",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 8, 10, 37, 26, 233, DateTimeKind.Local).AddTicks(5057), new DateTime(2026, 4, 8, 10, 37, 26, 233, DateTimeKind.Local).AddTicks(5058) });

            migrationBuilder.UpdateData(
                table: "Person",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "DateOfBirth", "EmploymentDate", "EndofEmploymentDate", "Modified" },
                values: new object[] { new DateTime(2026, 4, 8, 10, 37, 26, 233, DateTimeKind.Local).AddTicks(5209), new DateTime(2026, 4, 8, 10, 37, 26, 233, DateTimeKind.Local).AddTicks(5205), new DateTime(2026, 4, 8, 10, 37, 26, 233, DateTimeKind.Local).AddTicks(5200), new DateTime(2026, 4, 8, 10, 37, 26, 233, DateTimeKind.Local).AddTicks(5202), new DateTime(2026, 4, 8, 10, 37, 26, 233, DateTimeKind.Local).AddTicks(5219) });

            migrationBuilder.UpdateData(
                table: "Religions",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 8, 10, 37, 26, 233, DateTimeKind.Local).AddTicks(5099), new DateTime(2026, 4, 8, 10, 37, 26, 233, DateTimeKind.Local).AddTicks(5101) });

            migrationBuilder.UpdateData(
                table: "StaffCategories",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 8, 10, 37, 26, 233, DateTimeKind.Local).AddTicks(4921), new DateTime(2026, 4, 8, 10, 37, 26, 233, DateTimeKind.Local).AddTicks(4924) });

            migrationBuilder.CreateIndex(
                name: "IX_SpecificOutcomes_LearningLevelId",
                table: "SpecificOutcomes",
                column: "LearningLevelId");

            migrationBuilder.AddForeignKey(
                name: "FK_SpecificOutcomes_LearningLevels_LearningLevelId",
                table: "SpecificOutcomes",
                column: "LearningLevelId",
                principalTable: "LearningLevels",
                principalColumn: "Id");
        }
    }
}
