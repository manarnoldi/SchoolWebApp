using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SchoolWebApp.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddedAcademicYeartoStrand : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            //migrationBuilder.AddColumn<int>(
            //    name: "AcademicYearId",
            //    table: "Strands",
            //    type: "int",
            //    nullable: false,
            //    defaultValue: 0);

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "269f0cf3-405e-4163-83f3-1b63ebebd62e",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2025, 9, 27, 22, 0, 4, 515, DateTimeKind.Local).AddTicks(3421), new DateTime(2025, 9, 27, 22, 0, 4, 515, DateTimeKind.Local).AddTicks(3422) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "448df289-142c-4959-a912-60733515e1b4",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2025, 9, 27, 22, 0, 4, 515, DateTimeKind.Local).AddTicks(3402), new DateTime(2025, 9, 27, 22, 0, 4, 515, DateTimeKind.Local).AddTicks(3404) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "48c50c3a-9958-453b-b649-4e21af131322",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2025, 9, 27, 22, 0, 4, 515, DateTimeKind.Local).AddTicks(3384), new DateTime(2025, 9, 27, 22, 0, 4, 515, DateTimeKind.Local).AddTicks(3385) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "717d9b15-a428-440c-b26b-08d3bbb68b02",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2025, 9, 27, 22, 0, 4, 515, DateTimeKind.Local).AddTicks(3243), new DateTime(2025, 9, 27, 22, 0, 4, 515, DateTimeKind.Local).AddTicks(3318) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "95ed2407-3e58-4af2-88a4-1c4e96473f68",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2025, 9, 27, 22, 0, 4, 515, DateTimeKind.Local).AddTicks(3363), new DateTime(2025, 9, 27, 22, 0, 4, 515, DateTimeKind.Local).AddTicks(3365) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "97942bee-ef12-4425-8225-4f293d0f36dd",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2025, 9, 27, 22, 0, 4, 515, DateTimeKind.Local).AddTicks(3469), new DateTime(2025, 9, 27, 22, 0, 4, 515, DateTimeKind.Local).AddTicks(3470) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "cd12b44b-103b-48df-8887-a2bf42e0651e",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2025, 9, 27, 22, 0, 4, 515, DateTimeKind.Local).AddTicks(3441), new DateTime(2025, 9, 27, 22, 0, 4, 515, DateTimeKind.Local).AddTicks(3443) });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "7e67d486-af3e-49f1-a109-a2b864b8e0ec",
                columns: new[] { "ConcurrencyStamp", "Created", "Modified", "PasswordHash", "SecurityStamp" },
                values: new object[] { "1fea64c7-5a76-4a35-a64b-7b1b47951d83", new DateTime(2025, 9, 27, 22, 0, 4, 515, DateTimeKind.Local).AddTicks(3755), new DateTime(2025, 9, 27, 22, 0, 4, 515, DateTimeKind.Local).AddTicks(3756), "AQAAAAIAAYagAAAAEGSYUFU9rnAVLzJPze6FcIT+CMOIUAHJpj/9swvubJvcwYHEYg1U0ycL+sI7iqbnJA==", "2ea82e68-1f65-4b65-b8ec-f7bae2e90471" });

            migrationBuilder.UpdateData(
                table: "Designations",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2025, 9, 27, 22, 0, 4, 515, DateTimeKind.Local).AddTicks(3525), new DateTime(2025, 9, 27, 22, 0, 4, 515, DateTimeKind.Local).AddTicks(3526) });

            migrationBuilder.UpdateData(
                table: "EmploymentTypes",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2025, 9, 27, 22, 0, 4, 515, DateTimeKind.Local).AddTicks(3568), new DateTime(2025, 9, 27, 22, 0, 4, 515, DateTimeKind.Local).AddTicks(3569) });

            migrationBuilder.UpdateData(
                table: "Genders",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2025, 9, 27, 22, 0, 4, 515, DateTimeKind.Local).AddTicks(3629), new DateTime(2025, 9, 27, 22, 0, 4, 515, DateTimeKind.Local).AddTicks(3630) });

            migrationBuilder.UpdateData(
                table: "Nationalities",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2025, 9, 27, 22, 0, 4, 515, DateTimeKind.Local).AddTicks(3593), new DateTime(2025, 9, 27, 22, 0, 4, 515, DateTimeKind.Local).AddTicks(3593) });

            migrationBuilder.UpdateData(
                table: "Person",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "DateOfBirth", "EmploymentDate", "EndofEmploymentDate", "Modified" },
                values: new object[] { new DateTime(2025, 9, 27, 22, 0, 4, 515, DateTimeKind.Local).AddTicks(3675), new DateTime(2025, 9, 27, 22, 0, 4, 515, DateTimeKind.Local).AddTicks(3665), new DateTime(2025, 9, 27, 22, 0, 4, 515, DateTimeKind.Local).AddTicks(3661), new DateTime(2025, 9, 27, 22, 0, 4, 515, DateTimeKind.Local).AddTicks(3662), new DateTime(2025, 9, 27, 22, 0, 4, 515, DateTimeKind.Local).AddTicks(3695) });

            migrationBuilder.UpdateData(
                table: "Religions",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2025, 9, 27, 22, 0, 4, 515, DateTimeKind.Local).AddTicks(3610), new DateTime(2025, 9, 27, 22, 0, 4, 515, DateTimeKind.Local).AddTicks(3611) });

            migrationBuilder.UpdateData(
                table: "StaffCategories",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2025, 9, 27, 22, 0, 4, 515, DateTimeKind.Local).AddTicks(3500), new DateTime(2025, 9, 27, 22, 0, 4, 515, DateTimeKind.Local).AddTicks(3503) });

            //migrationBuilder.CreateIndex(
            //    name: "IX_Strands_AcademicYearId",
            //    table: "Strands",
            //    column: "AcademicYearId");

            migrationBuilder.AddForeignKey(
                name: "FK_Strands_AcademicYears_AcademicYearId",
                table: "Strands",
                column: "AcademicYearId",
                principalTable: "AcademicYears",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Strands_AcademicYears_AcademicYearId",
                table: "Strands");

            migrationBuilder.DropIndex(
                name: "IX_Strands_AcademicYearId",
                table: "Strands");

            migrationBuilder.DropColumn(
                name: "AcademicYearId",
                table: "Strands");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "269f0cf3-405e-4163-83f3-1b63ebebd62e",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2025, 9, 27, 20, 52, 25, 284, DateTimeKind.Local).AddTicks(799), new DateTime(2025, 9, 27, 20, 52, 25, 284, DateTimeKind.Local).AddTicks(800) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "448df289-142c-4959-a912-60733515e1b4",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2025, 9, 27, 20, 52, 25, 284, DateTimeKind.Local).AddTicks(777), new DateTime(2025, 9, 27, 20, 52, 25, 284, DateTimeKind.Local).AddTicks(779) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "48c50c3a-9958-453b-b649-4e21af131322",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2025, 9, 27, 20, 52, 25, 284, DateTimeKind.Local).AddTicks(756), new DateTime(2025, 9, 27, 20, 52, 25, 284, DateTimeKind.Local).AddTicks(757) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "717d9b15-a428-440c-b26b-08d3bbb68b02",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2025, 9, 27, 20, 52, 25, 284, DateTimeKind.Local).AddTicks(665), new DateTime(2025, 9, 27, 20, 52, 25, 284, DateTimeKind.Local).AddTicks(682) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "95ed2407-3e58-4af2-88a4-1c4e96473f68",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2025, 9, 27, 20, 52, 25, 284, DateTimeKind.Local).AddTicks(722), new DateTime(2025, 9, 27, 20, 52, 25, 284, DateTimeKind.Local).AddTicks(724) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "97942bee-ef12-4425-8225-4f293d0f36dd",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2025, 9, 27, 20, 52, 25, 284, DateTimeKind.Local).AddTicks(845), new DateTime(2025, 9, 27, 20, 52, 25, 284, DateTimeKind.Local).AddTicks(846) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "cd12b44b-103b-48df-8887-a2bf42e0651e",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2025, 9, 27, 20, 52, 25, 284, DateTimeKind.Local).AddTicks(824), new DateTime(2025, 9, 27, 20, 52, 25, 284, DateTimeKind.Local).AddTicks(825) });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "7e67d486-af3e-49f1-a109-a2b864b8e0ec",
                columns: new[] { "ConcurrencyStamp", "Created", "Modified", "PasswordHash", "SecurityStamp" },
                values: new object[] { "f0184ffe-6524-4681-9af5-6f46c5751f52", new DateTime(2025, 9, 27, 20, 52, 25, 284, DateTimeKind.Local).AddTicks(1153), new DateTime(2025, 9, 27, 20, 52, 25, 284, DateTimeKind.Local).AddTicks(1155), "AQAAAAIAAYagAAAAEPNVrG6vJy+9qFubUe4M6kpRv3f0xK3uV1bBz1s6d3dxTd9TDoOGGVKCzyiG8HEKjQ==", "93c98b5b-ff57-4b64-b981-2f1589a793e4" });

            migrationBuilder.UpdateData(
                table: "Designations",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2025, 9, 27, 20, 52, 25, 284, DateTimeKind.Local).AddTicks(899), new DateTime(2025, 9, 27, 20, 52, 25, 284, DateTimeKind.Local).AddTicks(900) });

            migrationBuilder.UpdateData(
                table: "EmploymentTypes",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2025, 9, 27, 20, 52, 25, 284, DateTimeKind.Local).AddTicks(936), new DateTime(2025, 9, 27, 20, 52, 25, 284, DateTimeKind.Local).AddTicks(937) });

            migrationBuilder.UpdateData(
                table: "Genders",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2025, 9, 27, 20, 52, 25, 284, DateTimeKind.Local).AddTicks(996), new DateTime(2025, 9, 27, 20, 52, 25, 284, DateTimeKind.Local).AddTicks(997) });

            migrationBuilder.UpdateData(
                table: "Nationalities",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2025, 9, 27, 20, 52, 25, 284, DateTimeKind.Local).AddTicks(961), new DateTime(2025, 9, 27, 20, 52, 25, 284, DateTimeKind.Local).AddTicks(962) });

            migrationBuilder.UpdateData(
                table: "Person",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "DateOfBirth", "EmploymentDate", "EndofEmploymentDate", "Modified" },
                values: new object[] { new DateTime(2025, 9, 27, 20, 52, 25, 284, DateTimeKind.Local).AddTicks(1034), new DateTime(2025, 9, 27, 20, 52, 25, 284, DateTimeKind.Local).AddTicks(1032), new DateTime(2025, 9, 27, 20, 52, 25, 284, DateTimeKind.Local).AddTicks(1028), new DateTime(2025, 9, 27, 20, 52, 25, 284, DateTimeKind.Local).AddTicks(1029), new DateTime(2025, 9, 27, 20, 52, 25, 284, DateTimeKind.Local).AddTicks(1038) });

            migrationBuilder.UpdateData(
                table: "Religions",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2025, 9, 27, 20, 52, 25, 284, DateTimeKind.Local).AddTicks(979), new DateTime(2025, 9, 27, 20, 52, 25, 284, DateTimeKind.Local).AddTicks(980) });

            migrationBuilder.UpdateData(
                table: "StaffCategories",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2025, 9, 27, 20, 52, 25, 284, DateTimeKind.Local).AddTicks(878), new DateTime(2025, 9, 27, 20, 52, 25, 284, DateTimeKind.Local).AddTicks(880) });
        }
    }
}
