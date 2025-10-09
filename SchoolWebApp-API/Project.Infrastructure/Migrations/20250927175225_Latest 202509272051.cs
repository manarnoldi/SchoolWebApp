using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SchoolWebApp.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Latest202509272051 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SpecificOutcomes_LearningLevels_LearningLevelId",
                table: "SpecificOutcomes");

            migrationBuilder.DropIndex(
                name: "IX_SpecificOutcomes_LearningLevelId",
                table: "SpecificOutcomes");

            migrationBuilder.DropColumn(
                name: "LearningLevelId",
                table: "SpecificOutcomes");

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "269f0cf3-405e-4163-83f3-1b63ebebd62e",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2025, 9, 27, 20, 26, 4, 441, DateTimeKind.Local).AddTicks(8584), new DateTime(2025, 9, 27, 20, 26, 4, 441, DateTimeKind.Local).AddTicks(8586) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "448df289-142c-4959-a912-60733515e1b4",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2025, 9, 27, 20, 26, 4, 441, DateTimeKind.Local).AddTicks(8558), new DateTime(2025, 9, 27, 20, 26, 4, 441, DateTimeKind.Local).AddTicks(8559) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "48c50c3a-9958-453b-b649-4e21af131322",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2025, 9, 27, 20, 26, 4, 441, DateTimeKind.Local).AddTicks(8528), new DateTime(2025, 9, 27, 20, 26, 4, 441, DateTimeKind.Local).AddTicks(8529) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "717d9b15-a428-440c-b26b-08d3bbb68b02",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2025, 9, 27, 20, 26, 4, 441, DateTimeKind.Local).AddTicks(8416), new DateTime(2025, 9, 27, 20, 26, 4, 441, DateTimeKind.Local).AddTicks(8440) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "95ed2407-3e58-4af2-88a4-1c4e96473f68",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2025, 9, 27, 20, 26, 4, 441, DateTimeKind.Local).AddTicks(8496), new DateTime(2025, 9, 27, 20, 26, 4, 441, DateTimeKind.Local).AddTicks(8498) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "97942bee-ef12-4425-8225-4f293d0f36dd",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2025, 9, 27, 20, 26, 4, 441, DateTimeKind.Local).AddTicks(8646), new DateTime(2025, 9, 27, 20, 26, 4, 441, DateTimeKind.Local).AddTicks(8647) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "cd12b44b-103b-48df-8887-a2bf42e0651e",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2025, 9, 27, 20, 26, 4, 441, DateTimeKind.Local).AddTicks(8611), new DateTime(2025, 9, 27, 20, 26, 4, 441, DateTimeKind.Local).AddTicks(8613) });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "7e67d486-af3e-49f1-a109-a2b864b8e0ec",
                columns: new[] { "ConcurrencyStamp", "Created", "Modified", "PasswordHash", "SecurityStamp" },
                values: new object[] { "e1eb543f-ea5f-4382-9c1c-3efe63b09159", new DateTime(2025, 9, 27, 20, 26, 4, 441, DateTimeKind.Local).AddTicks(9007), new DateTime(2025, 9, 27, 20, 26, 4, 441, DateTimeKind.Local).AddTicks(9009), "AQAAAAIAAYagAAAAEPGsFoLdCh4a6gH6yIW5Zgqzm+2Hv1Q/L1iL21Fgk/sOZI4YQU9RQuUyhwRXC2MYxg==", "fec5203c-43b3-4819-8319-7ab8f015ba77" });

            migrationBuilder.UpdateData(
                table: "Designations",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2025, 9, 27, 20, 26, 4, 441, DateTimeKind.Local).AddTicks(8733), new DateTime(2025, 9, 27, 20, 26, 4, 441, DateTimeKind.Local).AddTicks(8734) });

            migrationBuilder.UpdateData(
                table: "EmploymentTypes",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2025, 9, 27, 20, 26, 4, 441, DateTimeKind.Local).AddTicks(8761), new DateTime(2025, 9, 27, 20, 26, 4, 441, DateTimeKind.Local).AddTicks(8762) });

            migrationBuilder.UpdateData(
                table: "Genders",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2025, 9, 27, 20, 26, 4, 441, DateTimeKind.Local).AddTicks(8846), new DateTime(2025, 9, 27, 20, 26, 4, 441, DateTimeKind.Local).AddTicks(8847) });

            migrationBuilder.UpdateData(
                table: "Nationalities",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2025, 9, 27, 20, 26, 4, 441, DateTimeKind.Local).AddTicks(8788), new DateTime(2025, 9, 27, 20, 26, 4, 441, DateTimeKind.Local).AddTicks(8789) });

            migrationBuilder.UpdateData(
                table: "Person",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "DateOfBirth", "EmploymentDate", "EndofEmploymentDate", "Modified" },
                values: new object[] { new DateTime(2025, 9, 27, 20, 26, 4, 441, DateTimeKind.Local).AddTicks(8894), new DateTime(2025, 9, 27, 20, 26, 4, 441, DateTimeKind.Local).AddTicks(8891), new DateTime(2025, 9, 27, 20, 26, 4, 441, DateTimeKind.Local).AddTicks(8887), new DateTime(2025, 9, 27, 20, 26, 4, 441, DateTimeKind.Local).AddTicks(8888), new DateTime(2025, 9, 27, 20, 26, 4, 441, DateTimeKind.Local).AddTicks(8894) });

            migrationBuilder.UpdateData(
                table: "Religions",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2025, 9, 27, 20, 26, 4, 441, DateTimeKind.Local).AddTicks(8819), new DateTime(2025, 9, 27, 20, 26, 4, 441, DateTimeKind.Local).AddTicks(8820) });

            migrationBuilder.UpdateData(
                table: "StaffCategories",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2025, 9, 27, 20, 26, 4, 441, DateTimeKind.Local).AddTicks(8700), new DateTime(2025, 9, 27, 20, 26, 4, 441, DateTimeKind.Local).AddTicks(8702) });
        }
    }
}
