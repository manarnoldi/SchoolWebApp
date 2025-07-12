using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SchoolWebApp.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddedRanktoEducationLevelType : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Rank",
                table: "EducationLevelTypes",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "269f0cf3-405e-4163-83f3-1b63ebebd62e",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2025, 7, 11, 7, 27, 11, 771, DateTimeKind.Local).AddTicks(3344), new DateTime(2025, 7, 11, 7, 27, 11, 771, DateTimeKind.Local).AddTicks(3345) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "448df289-142c-4959-a912-60733515e1b4",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2025, 7, 11, 7, 27, 11, 771, DateTimeKind.Local).AddTicks(3312), new DateTime(2025, 7, 11, 7, 27, 11, 771, DateTimeKind.Local).AddTicks(3318) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "48c50c3a-9958-453b-b649-4e21af131322",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2025, 7, 11, 7, 27, 11, 771, DateTimeKind.Local).AddTicks(3286), new DateTime(2025, 7, 11, 7, 27, 11, 771, DateTimeKind.Local).AddTicks(3287) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "717d9b15-a428-440c-b26b-08d3bbb68b02",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2025, 7, 11, 7, 27, 11, 771, DateTimeKind.Local).AddTicks(3127), new DateTime(2025, 7, 11, 7, 27, 11, 771, DateTimeKind.Local).AddTicks(3161) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "95ed2407-3e58-4af2-88a4-1c4e96473f68",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2025, 7, 11, 7, 27, 11, 771, DateTimeKind.Local).AddTicks(3247), new DateTime(2025, 7, 11, 7, 27, 11, 771, DateTimeKind.Local).AddTicks(3248) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "97942bee-ef12-4425-8225-4f293d0f36dd",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2025, 7, 11, 7, 27, 11, 771, DateTimeKind.Local).AddTicks(3401), new DateTime(2025, 7, 11, 7, 27, 11, 771, DateTimeKind.Local).AddTicks(3402) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "cd12b44b-103b-48df-8887-a2bf42e0651e",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2025, 7, 11, 7, 27, 11, 771, DateTimeKind.Local).AddTicks(3375), new DateTime(2025, 7, 11, 7, 27, 11, 771, DateTimeKind.Local).AddTicks(3376) });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "7e67d486-af3e-49f1-a109-a2b864b8e0ec",
                columns: new[] { "ConcurrencyStamp", "Created", "Modified", "PasswordHash", "SecurityStamp" },
                values: new object[] { "09f959e6-7cc4-4c5a-b7e2-db56b6b66cb3", new DateTime(2025, 7, 11, 7, 27, 11, 771, DateTimeKind.Local).AddTicks(3953), new DateTime(2025, 7, 11, 7, 27, 11, 771, DateTimeKind.Local).AddTicks(3956), "AQAAAAIAAYagAAAAEJA+L0pM+mIWF+5zFg4oqY9S4JgTnkPLSPOVkl3Fjldf1NcBCs482Pl60h8rS1ax3Q==", "4d34c80c-3290-43b7-a6da-ad1f6cba6ae7" });

            migrationBuilder.UpdateData(
                table: "Designations",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2025, 7, 11, 7, 27, 11, 771, DateTimeKind.Local).AddTicks(3518), new DateTime(2025, 7, 11, 7, 27, 11, 771, DateTimeKind.Local).AddTicks(3519) });

            migrationBuilder.UpdateData(
                table: "EmploymentTypes",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2025, 7, 11, 7, 27, 11, 771, DateTimeKind.Local).AddTicks(3545), new DateTime(2025, 7, 11, 7, 27, 11, 771, DateTimeKind.Local).AddTicks(3546) });

            migrationBuilder.UpdateData(
                table: "Genders",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2025, 7, 11, 7, 27, 11, 771, DateTimeKind.Local).AddTicks(3669), new DateTime(2025, 7, 11, 7, 27, 11, 771, DateTimeKind.Local).AddTicks(3671) });

            migrationBuilder.UpdateData(
                table: "Nationalities",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2025, 7, 11, 7, 27, 11, 771, DateTimeKind.Local).AddTicks(3587), new DateTime(2025, 7, 11, 7, 27, 11, 771, DateTimeKind.Local).AddTicks(3588) });

            migrationBuilder.UpdateData(
                table: "Person",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "DateOfBirth", "EmploymentDate", "EndofEmploymentDate", "Modified" },
                values: new object[] { new DateTime(2025, 7, 11, 7, 27, 11, 771, DateTimeKind.Local).AddTicks(3774), new DateTime(2025, 7, 11, 7, 27, 11, 771, DateTimeKind.Local).AddTicks(3766), new DateTime(2025, 7, 11, 7, 27, 11, 771, DateTimeKind.Local).AddTicks(3759), new DateTime(2025, 7, 11, 7, 27, 11, 771, DateTimeKind.Local).AddTicks(3761), new DateTime(2025, 7, 11, 7, 27, 11, 771, DateTimeKind.Local).AddTicks(3789) });

            migrationBuilder.UpdateData(
                table: "Religions",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2025, 7, 11, 7, 27, 11, 771, DateTimeKind.Local).AddTicks(3626), new DateTime(2025, 7, 11, 7, 27, 11, 771, DateTimeKind.Local).AddTicks(3627) });

            migrationBuilder.UpdateData(
                table: "StaffCategories",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2025, 7, 11, 7, 27, 11, 771, DateTimeKind.Local).AddTicks(3478), new DateTime(2025, 7, 11, 7, 27, 11, 771, DateTimeKind.Local).AddTicks(3480) });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Rank",
                table: "EducationLevelTypes");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "269f0cf3-405e-4163-83f3-1b63ebebd62e",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2025, 7, 11, 6, 59, 6, 427, DateTimeKind.Local).AddTicks(2277), new DateTime(2025, 7, 11, 6, 59, 6, 427, DateTimeKind.Local).AddTicks(2280) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "448df289-142c-4959-a912-60733515e1b4",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2025, 7, 11, 6, 59, 6, 427, DateTimeKind.Local).AddTicks(2212), new DateTime(2025, 7, 11, 6, 59, 6, 427, DateTimeKind.Local).AddTicks(2214) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "48c50c3a-9958-453b-b649-4e21af131322",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2025, 7, 11, 6, 59, 6, 427, DateTimeKind.Local).AddTicks(2147), new DateTime(2025, 7, 11, 6, 59, 6, 427, DateTimeKind.Local).AddTicks(2149) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "717d9b15-a428-440c-b26b-08d3bbb68b02",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2025, 7, 11, 6, 59, 6, 427, DateTimeKind.Local).AddTicks(1884), new DateTime(2025, 7, 11, 6, 59, 6, 427, DateTimeKind.Local).AddTicks(1924) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "95ed2407-3e58-4af2-88a4-1c4e96473f68",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2025, 7, 11, 6, 59, 6, 427, DateTimeKind.Local).AddTicks(2050), new DateTime(2025, 7, 11, 6, 59, 6, 427, DateTimeKind.Local).AddTicks(2054) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "97942bee-ef12-4425-8225-4f293d0f36dd",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2025, 7, 11, 6, 59, 6, 427, DateTimeKind.Local).AddTicks(2413), new DateTime(2025, 7, 11, 6, 59, 6, 427, DateTimeKind.Local).AddTicks(2415) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "cd12b44b-103b-48df-8887-a2bf42e0651e",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2025, 7, 11, 6, 59, 6, 427, DateTimeKind.Local).AddTicks(2349), new DateTime(2025, 7, 11, 6, 59, 6, 427, DateTimeKind.Local).AddTicks(2352) });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "7e67d486-af3e-49f1-a109-a2b864b8e0ec",
                columns: new[] { "ConcurrencyStamp", "Created", "Modified", "PasswordHash", "SecurityStamp" },
                values: new object[] { "5063d639-4e92-448b-9a7b-000785dbe4f8", new DateTime(2025, 7, 11, 6, 59, 6, 427, DateTimeKind.Local).AddTicks(3161), new DateTime(2025, 7, 11, 6, 59, 6, 427, DateTimeKind.Local).AddTicks(3165), "AQAAAAIAAYagAAAAEI+F4M/qI1kRRucf1GjEJJzY7/YUAuvVDiJUpRTOygBVz95f7k2OKCB1h5ZC8odcDg==", "8e916ebf-f00a-41fa-9939-82be98b6e67a" });

            migrationBuilder.UpdateData(
                table: "Designations",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2025, 7, 11, 6, 59, 6, 427, DateTimeKind.Local).AddTicks(2585), new DateTime(2025, 7, 11, 6, 59, 6, 427, DateTimeKind.Local).AddTicks(2588) });

            migrationBuilder.UpdateData(
                table: "EmploymentTypes",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2025, 7, 11, 6, 59, 6, 427, DateTimeKind.Local).AddTicks(2651), new DateTime(2025, 7, 11, 6, 59, 6, 427, DateTimeKind.Local).AddTicks(2655) });

            migrationBuilder.UpdateData(
                table: "Genders",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2025, 7, 11, 6, 59, 6, 427, DateTimeKind.Local).AddTicks(2881), new DateTime(2025, 7, 11, 6, 59, 6, 427, DateTimeKind.Local).AddTicks(2884) });

            migrationBuilder.UpdateData(
                table: "Nationalities",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2025, 7, 11, 6, 59, 6, 427, DateTimeKind.Local).AddTicks(2722), new DateTime(2025, 7, 11, 6, 59, 6, 427, DateTimeKind.Local).AddTicks(2724) });

            migrationBuilder.UpdateData(
                table: "Person",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "DateOfBirth", "EmploymentDate", "EndofEmploymentDate", "Modified" },
                values: new object[] { new DateTime(2025, 7, 11, 6, 59, 6, 427, DateTimeKind.Local).AddTicks(2991), new DateTime(2025, 7, 11, 6, 59, 6, 427, DateTimeKind.Local).AddTicks(2980), new DateTime(2025, 7, 11, 6, 59, 6, 427, DateTimeKind.Local).AddTicks(2969), new DateTime(2025, 7, 11, 6, 59, 6, 427, DateTimeKind.Local).AddTicks(2972), new DateTime(2025, 7, 11, 6, 59, 6, 427, DateTimeKind.Local).AddTicks(3004) });

            migrationBuilder.UpdateData(
                table: "Religions",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2025, 7, 11, 6, 59, 6, 427, DateTimeKind.Local).AddTicks(2806), new DateTime(2025, 7, 11, 6, 59, 6, 427, DateTimeKind.Local).AddTicks(2808) });

            migrationBuilder.UpdateData(
                table: "StaffCategories",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2025, 7, 11, 6, 59, 6, 427, DateTimeKind.Local).AddTicks(2512), new DateTime(2025, 7, 11, 6, 59, 6, 427, DateTimeKind.Local).AddTicks(2519) });
        }
    }
}
