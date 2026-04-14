using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SchoolWebApp.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RevertStudentAssessmentToSpecificOutcome : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
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
                values: new object[] { new DateTime(2026, 4, 8, 21, 34, 39, 947, DateTimeKind.Local).AddTicks(1591), new DateTime(2026, 4, 8, 21, 34, 39, 947, DateTimeKind.Local).AddTicks(1592) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "448df289-142c-4959-a912-60733515e1b4",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 8, 21, 34, 39, 947, DateTimeKind.Local).AddTicks(1572), new DateTime(2026, 4, 8, 21, 34, 39, 947, DateTimeKind.Local).AddTicks(1574) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "48c50c3a-9958-453b-b649-4e21af131322",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 8, 21, 34, 39, 947, DateTimeKind.Local).AddTicks(1552), new DateTime(2026, 4, 8, 21, 34, 39, 947, DateTimeKind.Local).AddTicks(1553) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "717d9b15-a428-440c-b26b-08d3bbb68b02",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 8, 21, 34, 39, 947, DateTimeKind.Local).AddTicks(1473), new DateTime(2026, 4, 8, 21, 34, 39, 947, DateTimeKind.Local).AddTicks(1485) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "95ed2407-3e58-4af2-88a4-1c4e96473f68",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 8, 21, 34, 39, 947, DateTimeKind.Local).AddTicks(1523), new DateTime(2026, 4, 8, 21, 34, 39, 947, DateTimeKind.Local).AddTicks(1524) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "97942bee-ef12-4425-8225-4f293d0f36dd",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 8, 21, 34, 39, 947, DateTimeKind.Local).AddTicks(1633), new DateTime(2026, 4, 8, 21, 34, 39, 947, DateTimeKind.Local).AddTicks(1634) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "cd12b44b-103b-48df-8887-a2bf42e0651e",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 8, 21, 34, 39, 947, DateTimeKind.Local).AddTicks(1609), new DateTime(2026, 4, 8, 21, 34, 39, 947, DateTimeKind.Local).AddTicks(1611) });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "7e67d486-af3e-49f1-a109-a2b864b8e0ec",
                columns: new[] { "ConcurrencyStamp", "Created", "Modified", "PasswordHash", "SecurityStamp" },
                values: new object[] { "d46b3acf-b878-4e95-9883-41c3be4541c1", new DateTime(2026, 4, 8, 21, 34, 39, 947, DateTimeKind.Local).AddTicks(1851), new DateTime(2026, 4, 8, 21, 34, 39, 947, DateTimeKind.Local).AddTicks(1852), "AQAAAAIAAYagAAAAEPHMv1HmXt5ZgKBiuO6qpHmRbibND8Cngm9v0mgu0ny8FkA720qS9+SX8a5ULZidng==", "d85b4505-86e9-4681-a1f6-eeda2c7fe348" });

            migrationBuilder.UpdateData(
                table: "Designations",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 8, 21, 34, 39, 947, DateTimeKind.Local).AddTicks(1673), new DateTime(2026, 4, 8, 21, 34, 39, 947, DateTimeKind.Local).AddTicks(1674) });

            migrationBuilder.UpdateData(
                table: "EmploymentTypes",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 8, 21, 34, 39, 947, DateTimeKind.Local).AddTicks(1700), new DateTime(2026, 4, 8, 21, 34, 39, 947, DateTimeKind.Local).AddTicks(1701) });

            migrationBuilder.UpdateData(
                table: "Genders",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 8, 21, 34, 39, 947, DateTimeKind.Local).AddTicks(1755), new DateTime(2026, 4, 8, 21, 34, 39, 947, DateTimeKind.Local).AddTicks(1756) });

            migrationBuilder.UpdateData(
                table: "Nationalities",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 8, 21, 34, 39, 947, DateTimeKind.Local).AddTicks(1722), new DateTime(2026, 4, 8, 21, 34, 39, 947, DateTimeKind.Local).AddTicks(1723) });

            migrationBuilder.UpdateData(
                table: "Person",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "DateOfBirth", "EmploymentDate", "EndofEmploymentDate", "Modified" },
                values: new object[] { new DateTime(2026, 4, 8, 21, 34, 39, 947, DateTimeKind.Local).AddTicks(1786), new DateTime(2026, 4, 8, 21, 34, 39, 947, DateTimeKind.Local).AddTicks(1783), new DateTime(2026, 4, 8, 21, 34, 39, 947, DateTimeKind.Local).AddTicks(1780), new DateTime(2026, 4, 8, 21, 34, 39, 947, DateTimeKind.Local).AddTicks(1781), new DateTime(2026, 4, 8, 21, 34, 39, 947, DateTimeKind.Local).AddTicks(1789) });

            migrationBuilder.UpdateData(
                table: "Religions",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 8, 21, 34, 39, 947, DateTimeKind.Local).AddTicks(1738), new DateTime(2026, 4, 8, 21, 34, 39, 947, DateTimeKind.Local).AddTicks(1739) });

            migrationBuilder.UpdateData(
                table: "StaffCategories",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 8, 21, 34, 39, 947, DateTimeKind.Local).AddTicks(1656), new DateTime(2026, 4, 8, 21, 34, 39, 947, DateTimeKind.Local).AddTicks(1658) });

            migrationBuilder.AddForeignKey(
                name: "FK_StudentAssessments_SpecificOutcomes_SpecificOutcomeId",
                table: "StudentAssessments",
                column: "SpecificOutcomeId",
                principalTable: "SpecificOutcomes",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
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
                values: new object[] { new DateTime(2026, 4, 8, 21, 13, 47, 425, DateTimeKind.Local).AddTicks(3938), new DateTime(2026, 4, 8, 21, 13, 47, 425, DateTimeKind.Local).AddTicks(3940) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "448df289-142c-4959-a912-60733515e1b4",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 8, 21, 13, 47, 425, DateTimeKind.Local).AddTicks(3892), new DateTime(2026, 4, 8, 21, 13, 47, 425, DateTimeKind.Local).AddTicks(3894) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "48c50c3a-9958-453b-b649-4e21af131322",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 8, 21, 13, 47, 425, DateTimeKind.Local).AddTicks(3834), new DateTime(2026, 4, 8, 21, 13, 47, 425, DateTimeKind.Local).AddTicks(3836) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "717d9b15-a428-440c-b26b-08d3bbb68b02",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 8, 21, 13, 47, 425, DateTimeKind.Local).AddTicks(3630), new DateTime(2026, 4, 8, 21, 13, 47, 425, DateTimeKind.Local).AddTicks(3651) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "95ed2407-3e58-4af2-88a4-1c4e96473f68",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 8, 21, 13, 47, 425, DateTimeKind.Local).AddTicks(3782), new DateTime(2026, 4, 8, 21, 13, 47, 425, DateTimeKind.Local).AddTicks(3784) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "97942bee-ef12-4425-8225-4f293d0f36dd",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 8, 21, 13, 47, 425, DateTimeKind.Local).AddTicks(4047), new DateTime(2026, 4, 8, 21, 13, 47, 425, DateTimeKind.Local).AddTicks(4049) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "cd12b44b-103b-48df-8887-a2bf42e0651e",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 8, 21, 13, 47, 425, DateTimeKind.Local).AddTicks(3986), new DateTime(2026, 4, 8, 21, 13, 47, 425, DateTimeKind.Local).AddTicks(3988) });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "7e67d486-af3e-49f1-a109-a2b864b8e0ec",
                columns: new[] { "ConcurrencyStamp", "Created", "Modified", "PasswordHash", "SecurityStamp" },
                values: new object[] { "dc061620-deed-4cc2-b6b6-9cbec9c3380d", new DateTime(2026, 4, 8, 21, 13, 47, 425, DateTimeKind.Local).AddTicks(4577), new DateTime(2026, 4, 8, 21, 13, 47, 425, DateTimeKind.Local).AddTicks(4580), "AQAAAAIAAYagAAAAEAdSiihVbSqyOiZ0q1Z4VjxGf8wawewohHHPmYmsJSmGXgmF6AMnym7CjGfD+U4eLA==", "d5c3ac24-2506-4374-b90d-50ed32fd08b8" });

            migrationBuilder.UpdateData(
                table: "Designations",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 8, 21, 13, 47, 425, DateTimeKind.Local).AddTicks(4144), new DateTime(2026, 4, 8, 21, 13, 47, 425, DateTimeKind.Local).AddTicks(4146) });

            migrationBuilder.UpdateData(
                table: "EmploymentTypes",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 8, 21, 13, 47, 425, DateTimeKind.Local).AddTicks(4200), new DateTime(2026, 4, 8, 21, 13, 47, 425, DateTimeKind.Local).AddTicks(4202) });

            migrationBuilder.UpdateData(
                table: "Genders",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 8, 21, 13, 47, 425, DateTimeKind.Local).AddTicks(4308), new DateTime(2026, 4, 8, 21, 13, 47, 425, DateTimeKind.Local).AddTicks(4310) });

            migrationBuilder.UpdateData(
                table: "Nationalities",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 8, 21, 13, 47, 425, DateTimeKind.Local).AddTicks(4238), new DateTime(2026, 4, 8, 21, 13, 47, 425, DateTimeKind.Local).AddTicks(4240) });

            migrationBuilder.UpdateData(
                table: "Person",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "DateOfBirth", "EmploymentDate", "EndofEmploymentDate", "Modified" },
                values: new object[] { new DateTime(2026, 4, 8, 21, 13, 47, 425, DateTimeKind.Local).AddTicks(4371), new DateTime(2026, 4, 8, 21, 13, 47, 425, DateTimeKind.Local).AddTicks(4368), new DateTime(2026, 4, 8, 21, 13, 47, 425, DateTimeKind.Local).AddTicks(4364), new DateTime(2026, 4, 8, 21, 13, 47, 425, DateTimeKind.Local).AddTicks(4366), new DateTime(2026, 4, 8, 21, 13, 47, 425, DateTimeKind.Local).AddTicks(4378) });

            migrationBuilder.UpdateData(
                table: "Religions",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 8, 21, 13, 47, 425, DateTimeKind.Local).AddTicks(4274), new DateTime(2026, 4, 8, 21, 13, 47, 425, DateTimeKind.Local).AddTicks(4275) });

            migrationBuilder.UpdateData(
                table: "StaffCategories",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 8, 21, 13, 47, 425, DateTimeKind.Local).AddTicks(4102), new DateTime(2026, 4, 8, 21, 13, 47, 425, DateTimeKind.Local).AddTicks(4105) });

            migrationBuilder.AddForeignKey(
                name: "FK_StudentAssessments_SubStrands_SubStrandId",
                table: "StudentAssessments",
                column: "SubStrandId",
                principalTable: "SubStrands",
                principalColumn: "Id");
        }
    }
}
