using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SchoolWebApp.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class resolvinglearninglevelidFKonstrands : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)

        {
            //migrationBuilder.DropForeignKey(
            //    name: "FK_SpecificOutcomes_LearningLevels_LearningLevelId",
            //    table: "SpecificOutcomes");

            //migrationBuilder.DropIndex(
            //    name: "IX_SpecificOutcomes_LearningLevelId",
            //    table: "SpecificOutcomes");

            //migrationBuilder.DropColumn(
            //    name: "LearningLevelId",
            //    table: "SpecificOutcomes");

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

            migrationBuilder.AddForeignKey(
               name: "FK_Strands_LearningLevels_LearningLevelId",
               table: "Strands",
               column: "LearningLevelId",
               principalTable: "LearningLevels",
               principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Strands_LearningLevels_LearningLevelId",
                table: "Strands");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "269f0cf3-405e-4163-83f3-1b63ebebd62e",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2025, 9, 27, 20, 10, 27, 109, DateTimeKind.Local).AddTicks(7018), new DateTime(2025, 9, 27, 20, 10, 27, 109, DateTimeKind.Local).AddTicks(7020) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "448df289-142c-4959-a912-60733515e1b4",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2025, 9, 27, 20, 10, 27, 109, DateTimeKind.Local).AddTicks(6993), new DateTime(2025, 9, 27, 20, 10, 27, 109, DateTimeKind.Local).AddTicks(6995) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "48c50c3a-9958-453b-b649-4e21af131322",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2025, 9, 27, 20, 10, 27, 109, DateTimeKind.Local).AddTicks(6971), new DateTime(2025, 9, 27, 20, 10, 27, 109, DateTimeKind.Local).AddTicks(6973) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "717d9b15-a428-440c-b26b-08d3bbb68b02",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2025, 9, 27, 20, 10, 27, 109, DateTimeKind.Local).AddTicks(6882), new DateTime(2025, 9, 27, 20, 10, 27, 109, DateTimeKind.Local).AddTicks(6898) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "95ed2407-3e58-4af2-88a4-1c4e96473f68",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2025, 9, 27, 20, 10, 27, 109, DateTimeKind.Local).AddTicks(6944), new DateTime(2025, 9, 27, 20, 10, 27, 109, DateTimeKind.Local).AddTicks(6946) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "97942bee-ef12-4425-8225-4f293d0f36dd",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2025, 9, 27, 20, 10, 27, 109, DateTimeKind.Local).AddTicks(7074), new DateTime(2025, 9, 27, 20, 10, 27, 109, DateTimeKind.Local).AddTicks(7075) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "cd12b44b-103b-48df-8887-a2bf42e0651e",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2025, 9, 27, 20, 10, 27, 109, DateTimeKind.Local).AddTicks(7042), new DateTime(2025, 9, 27, 20, 10, 27, 109, DateTimeKind.Local).AddTicks(7045) });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "7e67d486-af3e-49f1-a109-a2b864b8e0ec",
                columns: new[] { "ConcurrencyStamp", "Created", "Modified", "PasswordHash", "SecurityStamp" },
                values: new object[] { "8a6cded2-1616-453a-9233-38e36a7b3cde", new DateTime(2025, 9, 27, 20, 10, 27, 109, DateTimeKind.Local).AddTicks(7359), new DateTime(2025, 9, 27, 20, 10, 27, 109, DateTimeKind.Local).AddTicks(7360), "AQAAAAIAAYagAAAAEJykU+ATBrw5nWhhdXF6LtT6nWPrViR5eZu4UUXHLzrg8mYt1IIyn8EzL33OYtIyOw==", "b00504b6-f641-4c8e-a409-86b0a4350a52" });

            migrationBuilder.UpdateData(
                table: "Designations",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2025, 9, 27, 20, 10, 27, 109, DateTimeKind.Local).AddTicks(7136), new DateTime(2025, 9, 27, 20, 10, 27, 109, DateTimeKind.Local).AddTicks(7139) });

            migrationBuilder.UpdateData(
                table: "EmploymentTypes",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2025, 9, 27, 20, 10, 27, 109, DateTimeKind.Local).AddTicks(7177), new DateTime(2025, 9, 27, 20, 10, 27, 109, DateTimeKind.Local).AddTicks(7178) });

            migrationBuilder.UpdateData(
                table: "Genders",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2025, 9, 27, 20, 10, 27, 109, DateTimeKind.Local).AddTicks(7236), new DateTime(2025, 9, 27, 20, 10, 27, 109, DateTimeKind.Local).AddTicks(7237) });

            migrationBuilder.UpdateData(
                table: "Nationalities",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2025, 9, 27, 20, 10, 27, 109, DateTimeKind.Local).AddTicks(7197), new DateTime(2025, 9, 27, 20, 10, 27, 109, DateTimeKind.Local).AddTicks(7198) });

            migrationBuilder.UpdateData(
                table: "Person",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "DateOfBirth", "EmploymentDate", "EndofEmploymentDate", "Modified" },
                values: new object[] { new DateTime(2025, 9, 27, 20, 10, 27, 109, DateTimeKind.Local).AddTicks(7277), new DateTime(2025, 9, 27, 20, 10, 27, 109, DateTimeKind.Local).AddTicks(7274), new DateTime(2025, 9, 27, 20, 10, 27, 109, DateTimeKind.Local).AddTicks(7267), new DateTime(2025, 9, 27, 20, 10, 27, 109, DateTimeKind.Local).AddTicks(7268), new DateTime(2025, 9, 27, 20, 10, 27, 109, DateTimeKind.Local).AddTicks(7284) });

            migrationBuilder.UpdateData(
                table: "Religions",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2025, 9, 27, 20, 10, 27, 109, DateTimeKind.Local).AddTicks(7216), new DateTime(2025, 9, 27, 20, 10, 27, 109, DateTimeKind.Local).AddTicks(7217) });

            migrationBuilder.UpdateData(
                table: "StaffCategories",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2025, 9, 27, 20, 10, 27, 109, DateTimeKind.Local).AddTicks(7112), new DateTime(2025, 9, 27, 20, 10, 27, 109, DateTimeKind.Local).AddTicks(7114) });
        }
    }
}
