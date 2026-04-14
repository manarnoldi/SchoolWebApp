using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SchoolWebApp.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddSubStrandAndStrandToStudentAssessment : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "SpecificOutcomeId",
                table: "StudentAssessments",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<int>(
                name: "StrandId",
                table: "StudentAssessments",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "SubStrandId",
                table: "StudentAssessments",
                type: "int",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "269f0cf3-405e-4163-83f3-1b63ebebd62e",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 8, 22, 39, 45, 422, DateTimeKind.Local).AddTicks(8716), new DateTime(2026, 4, 8, 22, 39, 45, 422, DateTimeKind.Local).AddTicks(8717) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "448df289-142c-4959-a912-60733515e1b4",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 8, 22, 39, 45, 422, DateTimeKind.Local).AddTicks(8699), new DateTime(2026, 4, 8, 22, 39, 45, 422, DateTimeKind.Local).AddTicks(8700) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "48c50c3a-9958-453b-b649-4e21af131322",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 8, 22, 39, 45, 422, DateTimeKind.Local).AddTicks(8682), new DateTime(2026, 4, 8, 22, 39, 45, 422, DateTimeKind.Local).AddTicks(8683) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "717d9b15-a428-440c-b26b-08d3bbb68b02",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 8, 22, 39, 45, 422, DateTimeKind.Local).AddTicks(8611), new DateTime(2026, 4, 8, 22, 39, 45, 422, DateTimeKind.Local).AddTicks(8626) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "95ed2407-3e58-4af2-88a4-1c4e96473f68",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 8, 22, 39, 45, 422, DateTimeKind.Local).AddTicks(8663), new DateTime(2026, 4, 8, 22, 39, 45, 422, DateTimeKind.Local).AddTicks(8665) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "97942bee-ef12-4425-8225-4f293d0f36dd",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 8, 22, 39, 45, 422, DateTimeKind.Local).AddTicks(8756), new DateTime(2026, 4, 8, 22, 39, 45, 422, DateTimeKind.Local).AddTicks(8757) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "cd12b44b-103b-48df-8887-a2bf42e0651e",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 8, 22, 39, 45, 422, DateTimeKind.Local).AddTicks(8739), new DateTime(2026, 4, 8, 22, 39, 45, 422, DateTimeKind.Local).AddTicks(8741) });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "7e67d486-af3e-49f1-a109-a2b864b8e0ec",
                columns: new[] { "ConcurrencyStamp", "Created", "Modified", "PasswordHash", "SecurityStamp" },
                values: new object[] { "dbe819d2-5621-45b5-9bee-05dc1683c1c2", new DateTime(2026, 4, 8, 22, 39, 45, 422, DateTimeKind.Local).AddTicks(9004), new DateTime(2026, 4, 8, 22, 39, 45, 422, DateTimeKind.Local).AddTicks(9006), "AQAAAAIAAYagAAAAEMc8pyl+m+To38leHwb2shO9CKv/Hi6KZLXDvXOt3IubMZJvTrvYbUaKY9XZTKdbXQ==", "856642e6-711d-4e87-9419-494ff7c4ff7f" });

            migrationBuilder.UpdateData(
                table: "Designations",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 8, 22, 39, 45, 422, DateTimeKind.Local).AddTicks(8804), new DateTime(2026, 4, 8, 22, 39, 45, 422, DateTimeKind.Local).AddTicks(8805) });

            migrationBuilder.UpdateData(
                table: "EmploymentTypes",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 8, 22, 39, 45, 422, DateTimeKind.Local).AddTicks(8843), new DateTime(2026, 4, 8, 22, 39, 45, 422, DateTimeKind.Local).AddTicks(8844) });

            migrationBuilder.UpdateData(
                table: "Genders",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 8, 22, 39, 45, 422, DateTimeKind.Local).AddTicks(8897), new DateTime(2026, 4, 8, 22, 39, 45, 422, DateTimeKind.Local).AddTicks(8898) });

            migrationBuilder.UpdateData(
                table: "Nationalities",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 8, 22, 39, 45, 422, DateTimeKind.Local).AddTicks(8862), new DateTime(2026, 4, 8, 22, 39, 45, 422, DateTimeKind.Local).AddTicks(8863) });

            migrationBuilder.UpdateData(
                table: "Person",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "DateOfBirth", "EmploymentDate", "EndofEmploymentDate", "Modified" },
                values: new object[] { new DateTime(2026, 4, 8, 22, 39, 45, 422, DateTimeKind.Local).AddTicks(8933), new DateTime(2026, 4, 8, 22, 39, 45, 422, DateTimeKind.Local).AddTicks(8930), new DateTime(2026, 4, 8, 22, 39, 45, 422, DateTimeKind.Local).AddTicks(8926), new DateTime(2026, 4, 8, 22, 39, 45, 422, DateTimeKind.Local).AddTicks(8927), new DateTime(2026, 4, 8, 22, 39, 45, 422, DateTimeKind.Local).AddTicks(8938) });

            migrationBuilder.UpdateData(
                table: "Religions",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 8, 22, 39, 45, 422, DateTimeKind.Local).AddTicks(8881), new DateTime(2026, 4, 8, 22, 39, 45, 422, DateTimeKind.Local).AddTicks(8881) });

            migrationBuilder.UpdateData(
                table: "StaffCategories",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 8, 22, 39, 45, 422, DateTimeKind.Local).AddTicks(8784), new DateTime(2026, 4, 8, 22, 39, 45, 422, DateTimeKind.Local).AddTicks(8786) });

            migrationBuilder.CreateIndex(
                name: "IX_StudentAssessments_StrandId",
                table: "StudentAssessments",
                column: "StrandId");

            migrationBuilder.CreateIndex(
                name: "IX_StudentAssessments_SubStrandId",
                table: "StudentAssessments",
                column: "SubStrandId");

            migrationBuilder.AddForeignKey(
                name: "FK_StudentAssessments_Strands_StrandId",
                table: "StudentAssessments",
                column: "StrandId",
                principalTable: "Strands",
                principalColumn: "Id");

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
                name: "FK_StudentAssessments_Strands_StrandId",
                table: "StudentAssessments");

            migrationBuilder.DropForeignKey(
                name: "FK_StudentAssessments_SubStrands_SubStrandId",
                table: "StudentAssessments");

            migrationBuilder.DropIndex(
                name: "IX_StudentAssessments_StrandId",
                table: "StudentAssessments");

            migrationBuilder.DropIndex(
                name: "IX_StudentAssessments_SubStrandId",
                table: "StudentAssessments");

            migrationBuilder.DropColumn(
                name: "StrandId",
                table: "StudentAssessments");

            migrationBuilder.DropColumn(
                name: "SubStrandId",
                table: "StudentAssessments");

            migrationBuilder.AlterColumn<int>(
                name: "SpecificOutcomeId",
                table: "StudentAssessments",
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
        }
    }
}
