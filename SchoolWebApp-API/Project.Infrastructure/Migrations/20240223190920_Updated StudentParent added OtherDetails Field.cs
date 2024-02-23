using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SchoolWebApp.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdatedStudentParentaddedOtherDetailsField : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "OtherDetails",
                table: "StudentParents",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "269f0cf3-405e-4163-83f3-1b63ebebd62e",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2024, 2, 23, 22, 9, 18, 789, DateTimeKind.Local).AddTicks(9213), new DateTime(2024, 2, 23, 22, 9, 18, 789, DateTimeKind.Local).AddTicks(9214) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "448df289-142c-4959-a912-60733515e1b4",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2024, 2, 23, 22, 9, 18, 789, DateTimeKind.Local).AddTicks(9188), new DateTime(2024, 2, 23, 22, 9, 18, 789, DateTimeKind.Local).AddTicks(9189) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "48c50c3a-9958-453b-b649-4e21af131322",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2024, 2, 23, 22, 9, 18, 789, DateTimeKind.Local).AddTicks(9145), new DateTime(2024, 2, 23, 22, 9, 18, 789, DateTimeKind.Local).AddTicks(9155) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "717d9b15-a428-440c-b26b-08d3bbb68b02",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2024, 2, 23, 22, 9, 18, 789, DateTimeKind.Local).AddTicks(9030), new DateTime(2024, 2, 23, 22, 9, 18, 789, DateTimeKind.Local).AddTicks(9059) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "95ed2407-3e58-4af2-88a4-1c4e96473f68",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2024, 2, 23, 22, 9, 18, 789, DateTimeKind.Local).AddTicks(9115), new DateTime(2024, 2, 23, 22, 9, 18, 789, DateTimeKind.Local).AddTicks(9117) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "97942bee-ef12-4425-8225-4f293d0f36dd",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2024, 2, 23, 22, 9, 18, 789, DateTimeKind.Local).AddTicks(9266), new DateTime(2024, 2, 23, 22, 9, 18, 789, DateTimeKind.Local).AddTicks(9267) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "cd12b44b-103b-48df-8887-a2bf42e0651e",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2024, 2, 23, 22, 9, 18, 789, DateTimeKind.Local).AddTicks(9241), new DateTime(2024, 2, 23, 22, 9, 18, 789, DateTimeKind.Local).AddTicks(9242) });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "7e67d486-af3e-49f1-a109-a2b864b8e0ec",
                columns: new[] { "ConcurrencyStamp", "Created", "Modified", "PasswordHash", "SecurityStamp" },
                values: new object[] { "334e69f8-25a2-495c-8e77-74fce61a2f76", new DateTime(2024, 2, 23, 22, 9, 18, 789, DateTimeKind.Local).AddTicks(9368), new DateTime(2024, 2, 23, 22, 9, 18, 789, DateTimeKind.Local).AddTicks(9369), "AQAAAAIAAYagAAAAEMXea0b5cmuc2EHvQ0U5RRsJMUU9p5kJ/fhZyt2juR5hGzBWLNI1qmdqOaBbnot/Xg==", "eeb22d51-3a30-4a06-9c8f-5ec0b10ed9f7" });

            migrationBuilder.UpdateData(
                table: "SchoolLevels",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2024, 2, 23, 22, 9, 18, 881, DateTimeKind.Local).AddTicks(8787), new DateTime(2024, 2, 23, 22, 9, 18, 881, DateTimeKind.Local).AddTicks(8812) });

            migrationBuilder.UpdateData(
                table: "SchoolLevels",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2024, 2, 23, 22, 9, 18, 881, DateTimeKind.Local).AddTicks(8815), new DateTime(2024, 2, 23, 22, 9, 18, 881, DateTimeKind.Local).AddTicks(8816) });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OtherDetails",
                table: "StudentParents");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "269f0cf3-405e-4163-83f3-1b63ebebd62e",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2024, 2, 23, 16, 3, 19, 180, DateTimeKind.Local).AddTicks(399), new DateTime(2024, 2, 23, 16, 3, 19, 180, DateTimeKind.Local).AddTicks(400) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "448df289-142c-4959-a912-60733515e1b4",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2024, 2, 23, 16, 3, 19, 180, DateTimeKind.Local).AddTicks(373), new DateTime(2024, 2, 23, 16, 3, 19, 180, DateTimeKind.Local).AddTicks(374) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "48c50c3a-9958-453b-b649-4e21af131322",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2024, 2, 23, 16, 3, 19, 180, DateTimeKind.Local).AddTicks(311), new DateTime(2024, 2, 23, 16, 3, 19, 180, DateTimeKind.Local).AddTicks(335) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "717d9b15-a428-440c-b26b-08d3bbb68b02",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2024, 2, 23, 16, 3, 19, 180, DateTimeKind.Local).AddTicks(187), new DateTime(2024, 2, 23, 16, 3, 19, 180, DateTimeKind.Local).AddTicks(217) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "95ed2407-3e58-4af2-88a4-1c4e96473f68",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2024, 2, 23, 16, 3, 19, 180, DateTimeKind.Local).AddTicks(278), new DateTime(2024, 2, 23, 16, 3, 19, 180, DateTimeKind.Local).AddTicks(279) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "97942bee-ef12-4425-8225-4f293d0f36dd",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2024, 2, 23, 16, 3, 19, 180, DateTimeKind.Local).AddTicks(453), new DateTime(2024, 2, 23, 16, 3, 19, 180, DateTimeKind.Local).AddTicks(454) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "cd12b44b-103b-48df-8887-a2bf42e0651e",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2024, 2, 23, 16, 3, 19, 180, DateTimeKind.Local).AddTicks(427), new DateTime(2024, 2, 23, 16, 3, 19, 180, DateTimeKind.Local).AddTicks(428) });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "7e67d486-af3e-49f1-a109-a2b864b8e0ec",
                columns: new[] { "ConcurrencyStamp", "Created", "Modified", "PasswordHash", "SecurityStamp" },
                values: new object[] { "b74dcec5-5346-41a7-88b2-2804cc639b09", new DateTime(2024, 2, 23, 16, 3, 19, 180, DateTimeKind.Local).AddTicks(540), new DateTime(2024, 2, 23, 16, 3, 19, 180, DateTimeKind.Local).AddTicks(541), "AQAAAAIAAYagAAAAEEcj/1sBFKuF/pHcVUVuF0eMkns9hm86JJGsKznNAssq+ERV3uw+e0MCsy+skcr2Lg==", "4cb94c53-ed15-4341-ba1f-1e9b2de12dde" });

            migrationBuilder.UpdateData(
                table: "SchoolLevels",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2024, 2, 23, 16, 3, 19, 260, DateTimeKind.Local).AddTicks(950), new DateTime(2024, 2, 23, 16, 3, 19, 260, DateTimeKind.Local).AddTicks(978) });

            migrationBuilder.UpdateData(
                table: "SchoolLevels",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2024, 2, 23, 16, 3, 19, 260, DateTimeKind.Local).AddTicks(981), new DateTime(2024, 2, 23, 16, 3, 19, 260, DateTimeKind.Local).AddTicks(982) });
        }
    }
}
