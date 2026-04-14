using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SchoolWebApp.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddExaminableToSubject : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Examinable",
                table: "Subjects",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "269f0cf3-405e-4163-83f3-1b63ebebd62e",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 9, 13, 35, 53, 270, DateTimeKind.Local).AddTicks(30), new DateTime(2026, 4, 9, 13, 35, 53, 270, DateTimeKind.Local).AddTicks(31) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "448df289-142c-4959-a912-60733515e1b4",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 9, 13, 35, 53, 270, DateTimeKind.Local).AddTicks(10), new DateTime(2026, 4, 9, 13, 35, 53, 270, DateTimeKind.Local).AddTicks(11) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "48c50c3a-9958-453b-b649-4e21af131322",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 9, 13, 35, 53, 269, DateTimeKind.Local).AddTicks(9979), new DateTime(2026, 4, 9, 13, 35, 53, 269, DateTimeKind.Local).AddTicks(9980) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "717d9b15-a428-440c-b26b-08d3bbb68b02",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 9, 13, 35, 53, 269, DateTimeKind.Local).AddTicks(9865), new DateTime(2026, 4, 9, 13, 35, 53, 269, DateTimeKind.Local).AddTicks(9883) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "95ed2407-3e58-4af2-88a4-1c4e96473f68",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 9, 13, 35, 53, 269, DateTimeKind.Local).AddTicks(9952), new DateTime(2026, 4, 9, 13, 35, 53, 269, DateTimeKind.Local).AddTicks(9953) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "97942bee-ef12-4425-8225-4f293d0f36dd",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 9, 13, 35, 53, 270, DateTimeKind.Local).AddTicks(69), new DateTime(2026, 4, 9, 13, 35, 53, 270, DateTimeKind.Local).AddTicks(69) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "cd12b44b-103b-48df-8887-a2bf42e0651e",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 9, 13, 35, 53, 270, DateTimeKind.Local).AddTicks(50), new DateTime(2026, 4, 9, 13, 35, 53, 270, DateTimeKind.Local).AddTicks(51) });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "7e67d486-af3e-49f1-a109-a2b864b8e0ec",
                columns: new[] { "ConcurrencyStamp", "Created", "Modified", "PasswordHash", "SecurityStamp" },
                values: new object[] { "afd5edb0-5890-4243-9b38-15e87da77df6", new DateTime(2026, 4, 9, 13, 35, 53, 270, DateTimeKind.Local).AddTicks(437), new DateTime(2026, 4, 9, 13, 35, 53, 270, DateTimeKind.Local).AddTicks(438), "AQAAAAIAAYagAAAAENNy1FCiJ5MQoWKDvdoFSVecoXMUi7uZlheh7viAWfPuHZN35FpO0i9D9TXAutNf5g==", "830ab2a4-cef1-4d83-b08a-86ace8149468" });

            migrationBuilder.UpdateData(
                table: "Designations",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 9, 13, 35, 53, 270, DateTimeKind.Local).AddTicks(122), new DateTime(2026, 4, 9, 13, 35, 53, 270, DateTimeKind.Local).AddTicks(122) });

            migrationBuilder.UpdateData(
                table: "EmploymentTypes",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 9, 13, 35, 53, 270, DateTimeKind.Local).AddTicks(158), new DateTime(2026, 4, 9, 13, 35, 53, 270, DateTimeKind.Local).AddTicks(159) });

            migrationBuilder.UpdateData(
                table: "Genders",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 9, 13, 35, 53, 270, DateTimeKind.Local).AddTicks(228), new DateTime(2026, 4, 9, 13, 35, 53, 270, DateTimeKind.Local).AddTicks(228) });

            migrationBuilder.UpdateData(
                table: "Nationalities",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 9, 13, 35, 53, 270, DateTimeKind.Local).AddTicks(181), new DateTime(2026, 4, 9, 13, 35, 53, 270, DateTimeKind.Local).AddTicks(181) });

            migrationBuilder.UpdateData(
                table: "Person",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "DateOfBirth", "EmploymentDate", "EndofEmploymentDate", "Modified" },
                values: new object[] { new DateTime(2026, 4, 9, 13, 35, 53, 270, DateTimeKind.Local).AddTicks(268), new DateTime(2026, 4, 9, 13, 35, 53, 270, DateTimeKind.Local).AddTicks(264), new DateTime(2026, 4, 9, 13, 35, 53, 270, DateTimeKind.Local).AddTicks(258), new DateTime(2026, 4, 9, 13, 35, 53, 270, DateTimeKind.Local).AddTicks(259), new DateTime(2026, 4, 9, 13, 35, 53, 270, DateTimeKind.Local).AddTicks(275) });

            migrationBuilder.UpdateData(
                table: "Religions",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 9, 13, 35, 53, 270, DateTimeKind.Local).AddTicks(201), new DateTime(2026, 4, 9, 13, 35, 53, 270, DateTimeKind.Local).AddTicks(202) });

            migrationBuilder.UpdateData(
                table: "StaffCategories",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 9, 13, 35, 53, 270, DateTimeKind.Local).AddTicks(99), new DateTime(2026, 4, 9, 13, 35, 53, 270, DateTimeKind.Local).AddTicks(100) });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Examinable",
                table: "Subjects");

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
        }
    }
}
