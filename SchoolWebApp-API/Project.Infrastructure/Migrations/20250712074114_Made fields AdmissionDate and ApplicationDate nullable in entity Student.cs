using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SchoolWebApp.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class MadefieldsAdmissionDateandApplicationDatenullableinentityStudent : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "269f0cf3-405e-4163-83f3-1b63ebebd62e",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2025, 7, 12, 10, 41, 13, 350, DateTimeKind.Local).AddTicks(394), new DateTime(2025, 7, 12, 10, 41, 13, 350, DateTimeKind.Local).AddTicks(395) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "448df289-142c-4959-a912-60733515e1b4",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2025, 7, 12, 10, 41, 13, 350, DateTimeKind.Local).AddTicks(367), new DateTime(2025, 7, 12, 10, 41, 13, 350, DateTimeKind.Local).AddTicks(368) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "48c50c3a-9958-453b-b649-4e21af131322",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2025, 7, 12, 10, 41, 13, 350, DateTimeKind.Local).AddTicks(334), new DateTime(2025, 7, 12, 10, 41, 13, 350, DateTimeKind.Local).AddTicks(335) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "717d9b15-a428-440c-b26b-08d3bbb68b02",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2025, 7, 12, 10, 41, 13, 350, DateTimeKind.Local).AddTicks(220), new DateTime(2025, 7, 12, 10, 41, 13, 350, DateTimeKind.Local).AddTicks(249) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "95ed2407-3e58-4af2-88a4-1c4e96473f68",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2025, 7, 12, 10, 41, 13, 350, DateTimeKind.Local).AddTicks(304), new DateTime(2025, 7, 12, 10, 41, 13, 350, DateTimeKind.Local).AddTicks(306) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "97942bee-ef12-4425-8225-4f293d0f36dd",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2025, 7, 12, 10, 41, 13, 350, DateTimeKind.Local).AddTicks(461), new DateTime(2025, 7, 12, 10, 41, 13, 350, DateTimeKind.Local).AddTicks(462) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "cd12b44b-103b-48df-8887-a2bf42e0651e",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2025, 7, 12, 10, 41, 13, 350, DateTimeKind.Local).AddTicks(435), new DateTime(2025, 7, 12, 10, 41, 13, 350, DateTimeKind.Local).AddTicks(436) });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "7e67d486-af3e-49f1-a109-a2b864b8e0ec",
                columns: new[] { "ConcurrencyStamp", "Created", "Modified", "PasswordHash", "SecurityStamp" },
                values: new object[] { "5abbdc76-cefd-441b-8b50-5b6b8597322e", new DateTime(2025, 7, 12, 10, 41, 13, 350, DateTimeKind.Local).AddTicks(977), new DateTime(2025, 7, 12, 10, 41, 13, 350, DateTimeKind.Local).AddTicks(980), "AQAAAAIAAYagAAAAEK88U7MEwBXIdl7XiqFVJ6jGEjCKvoKOsppFDzAJuEPmnl2C5qPWjs6wFqicX77J3g==", "d0b27cc7-cde2-499a-a7ec-bbc0b692cbf1" });

            migrationBuilder.UpdateData(
                table: "Designations",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2025, 7, 12, 10, 41, 13, 350, DateTimeKind.Local).AddTicks(548), new DateTime(2025, 7, 12, 10, 41, 13, 350, DateTimeKind.Local).AddTicks(550) });

            migrationBuilder.UpdateData(
                table: "EmploymentTypes",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2025, 7, 12, 10, 41, 13, 350, DateTimeKind.Local).AddTicks(585), new DateTime(2025, 7, 12, 10, 41, 13, 350, DateTimeKind.Local).AddTicks(587) });

            migrationBuilder.UpdateData(
                table: "Genders",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2025, 7, 12, 10, 41, 13, 350, DateTimeKind.Local).AddTicks(686), new DateTime(2025, 7, 12, 10, 41, 13, 350, DateTimeKind.Local).AddTicks(687) });

            migrationBuilder.UpdateData(
                table: "Nationalities",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2025, 7, 12, 10, 41, 13, 350, DateTimeKind.Local).AddTicks(629), new DateTime(2025, 7, 12, 10, 41, 13, 350, DateTimeKind.Local).AddTicks(629) });

            migrationBuilder.UpdateData(
                table: "Person",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "DateOfBirth", "EmploymentDate", "EndofEmploymentDate", "Modified" },
                values: new object[] { new DateTime(2025, 7, 12, 10, 41, 13, 350, DateTimeKind.Local).AddTicks(747), new DateTime(2025, 7, 12, 10, 41, 13, 350, DateTimeKind.Local).AddTicks(740), new DateTime(2025, 7, 12, 10, 41, 13, 350, DateTimeKind.Local).AddTicks(733), new DateTime(2025, 7, 12, 10, 41, 13, 350, DateTimeKind.Local).AddTicks(735), new DateTime(2025, 7, 12, 10, 41, 13, 350, DateTimeKind.Local).AddTicks(757) });

            migrationBuilder.UpdateData(
                table: "Religions",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2025, 7, 12, 10, 41, 13, 350, DateTimeKind.Local).AddTicks(658), new DateTime(2025, 7, 12, 10, 41, 13, 350, DateTimeKind.Local).AddTicks(659) });

            migrationBuilder.UpdateData(
                table: "StaffCategories",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2025, 7, 12, 10, 41, 13, 350, DateTimeKind.Local).AddTicks(509), new DateTime(2025, 7, 12, 10, 41, 13, 350, DateTimeKind.Local).AddTicks(511) });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
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
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2025, 7, 12, 7, 24, 24, 976, DateTimeKind.Local).AddTicks(3905), new DateTime(2025, 7, 12, 7, 24, 24, 976, DateTimeKind.Local).AddTicks(3913) });
        }
    }
}
