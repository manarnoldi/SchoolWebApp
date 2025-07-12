using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SchoolWebApp.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddedForTeachingtoStaffCategory : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "ForTeaching",
                table: "StaffCategories",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "269f0cf3-405e-4163-83f3-1b63ebebd62e",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2025, 7, 12, 7, 24, 24, 976, DateTimeKind.Local).AddTicks(3342), new DateTime(2025, 7, 12, 7, 24, 24, 976, DateTimeKind.Local).AddTicks(3347) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "448df289-142c-4959-a912-60733515e1b4",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2025, 7, 12, 7, 24, 24, 976, DateTimeKind.Local).AddTicks(3203), new DateTime(2025, 7, 12, 7, 24, 24, 976, DateTimeKind.Local).AddTicks(3208) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "48c50c3a-9958-453b-b649-4e21af131322",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2025, 7, 12, 7, 24, 24, 976, DateTimeKind.Local).AddTicks(3077), new DateTime(2025, 7, 12, 7, 24, 24, 976, DateTimeKind.Local).AddTicks(3082) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "717d9b15-a428-440c-b26b-08d3bbb68b02",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2025, 7, 12, 7, 24, 24, 976, DateTimeKind.Local).AddTicks(2033), new DateTime(2025, 7, 12, 7, 24, 24, 976, DateTimeKind.Local).AddTicks(2467) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "95ed2407-3e58-4af2-88a4-1c4e96473f68",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2025, 7, 12, 7, 24, 24, 976, DateTimeKind.Local).AddTicks(2930), new DateTime(2025, 7, 12, 7, 24, 24, 976, DateTimeKind.Local).AddTicks(2936) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "97942bee-ef12-4425-8225-4f293d0f36dd",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2025, 7, 12, 7, 24, 24, 976, DateTimeKind.Local).AddTicks(3727), new DateTime(2025, 7, 12, 7, 24, 24, 976, DateTimeKind.Local).AddTicks(3732) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "cd12b44b-103b-48df-8887-a2bf42e0651e",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2025, 7, 12, 7, 24, 24, 976, DateTimeKind.Local).AddTicks(3551), new DateTime(2025, 7, 12, 7, 24, 24, 976, DateTimeKind.Local).AddTicks(3555) });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "7e67d486-af3e-49f1-a109-a2b864b8e0ec",
                columns: new[] { "ConcurrencyStamp", "Created", "Modified", "PasswordHash", "SecurityStamp" },
                values: new object[] { "eef426ad-1e3d-4f44-abd9-756e3d4bd22d", new DateTime(2025, 7, 12, 7, 24, 24, 976, DateTimeKind.Local).AddTicks(5525), new DateTime(2025, 7, 12, 7, 24, 24, 976, DateTimeKind.Local).AddTicks(5532), "AQAAAAIAAYagAAAAEN7QIvhvrk8vySKygVjbv+Uh1b1SXE5lLEeB8vux16oVgv/ey4eDXMW8anVaRHsznQ==", "33f90929-f05d-49a3-a9f5-8be98646ec36" });

            migrationBuilder.UpdateData(
                table: "Designations",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2025, 7, 12, 7, 24, 24, 976, DateTimeKind.Local).AddTicks(4053), new DateTime(2025, 7, 12, 7, 24, 24, 976, DateTimeKind.Local).AddTicks(4059) });

            migrationBuilder.UpdateData(
                table: "EmploymentTypes",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2025, 7, 12, 7, 24, 24, 976, DateTimeKind.Local).AddTicks(4230), new DateTime(2025, 7, 12, 7, 24, 24, 976, DateTimeKind.Local).AddTicks(4236) });

            migrationBuilder.UpdateData(
                table: "Genders",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2025, 7, 12, 7, 24, 24, 976, DateTimeKind.Local).AddTicks(4875), new DateTime(2025, 7, 12, 7, 24, 24, 976, DateTimeKind.Local).AddTicks(4880) });

            migrationBuilder.UpdateData(
                table: "Nationalities",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2025, 7, 12, 7, 24, 24, 976, DateTimeKind.Local).AddTicks(4386), new DateTime(2025, 7, 12, 7, 24, 24, 976, DateTimeKind.Local).AddTicks(4390) });

            migrationBuilder.UpdateData(
                table: "Person",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "DateOfBirth", "EmploymentDate", "EndofEmploymentDate", "Modified" },
                values: new object[] { new DateTime(2025, 7, 12, 7, 24, 24, 976, DateTimeKind.Local).AddTicks(5127), new DateTime(2025, 7, 12, 7, 24, 24, 976, DateTimeKind.Local).AddTicks(5112), new DateTime(2025, 7, 12, 7, 24, 24, 976, DateTimeKind.Local).AddTicks(5091), new DateTime(2025, 7, 12, 7, 24, 24, 976, DateTimeKind.Local).AddTicks(5101), new DateTime(2025, 7, 12, 7, 24, 24, 976, DateTimeKind.Local).AddTicks(5163) });

            migrationBuilder.UpdateData(
                table: "Religions",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2025, 7, 12, 7, 24, 24, 976, DateTimeKind.Local).AddTicks(4758), new DateTime(2025, 7, 12, 7, 24, 24, 976, DateTimeKind.Local).AddTicks(4764) });

            migrationBuilder.UpdateData(
                table: "StaffCategories",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "ForTeaching", "Modified" },
                values: new object[] { new DateTime(2025, 7, 12, 7, 24, 24, 976, DateTimeKind.Local).AddTicks(3905), false, new DateTime(2025, 7, 12, 7, 24, 24, 976, DateTimeKind.Local).AddTicks(3913) });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ForTeaching",
                table: "StaffCategories");

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
    }
}
