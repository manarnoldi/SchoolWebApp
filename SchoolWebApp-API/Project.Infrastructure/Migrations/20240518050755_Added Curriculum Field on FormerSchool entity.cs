using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SchoolWebApp.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddedCurriculumFieldonFormerSchoolentity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CurriculumId",
                table: "FormerSchools",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "269f0cf3-405e-4163-83f3-1b63ebebd62e",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2024, 5, 18, 8, 7, 54, 641, DateTimeKind.Local).AddTicks(5488), new DateTime(2024, 5, 18, 8, 7, 54, 641, DateTimeKind.Local).AddTicks(5490) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "448df289-142c-4959-a912-60733515e1b4",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2024, 5, 18, 8, 7, 54, 641, DateTimeKind.Local).AddTicks(5417), new DateTime(2024, 5, 18, 8, 7, 54, 641, DateTimeKind.Local).AddTicks(5419) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "48c50c3a-9958-453b-b649-4e21af131322",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2024, 5, 18, 8, 7, 54, 641, DateTimeKind.Local).AddTicks(5359), new DateTime(2024, 5, 18, 8, 7, 54, 641, DateTimeKind.Local).AddTicks(5360) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "717d9b15-a428-440c-b26b-08d3bbb68b02",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2024, 5, 18, 8, 7, 54, 641, DateTimeKind.Local).AddTicks(4886), new DateTime(2024, 5, 18, 8, 7, 54, 641, DateTimeKind.Local).AddTicks(4932) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "95ed2407-3e58-4af2-88a4-1c4e96473f68",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2024, 5, 18, 8, 7, 54, 641, DateTimeKind.Local).AddTicks(5295), new DateTime(2024, 5, 18, 8, 7, 54, 641, DateTimeKind.Local).AddTicks(5316) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "97942bee-ef12-4425-8225-4f293d0f36dd",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2024, 5, 18, 8, 7, 54, 641, DateTimeKind.Local).AddTicks(5602), new DateTime(2024, 5, 18, 8, 7, 54, 641, DateTimeKind.Local).AddTicks(5604) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "cd12b44b-103b-48df-8887-a2bf42e0651e",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2024, 5, 18, 8, 7, 54, 641, DateTimeKind.Local).AddTicks(5543), new DateTime(2024, 5, 18, 8, 7, 54, 641, DateTimeKind.Local).AddTicks(5553) });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "7e67d486-af3e-49f1-a109-a2b864b8e0ec",
                columns: new[] { "ConcurrencyStamp", "Created", "Modified", "PasswordHash", "SecurityStamp" },
                values: new object[] { "5064db1f-f4f9-467f-b517-8c3cad8d7331", new DateTime(2024, 5, 18, 8, 7, 54, 641, DateTimeKind.Local).AddTicks(5781), new DateTime(2024, 5, 18, 8, 7, 54, 641, DateTimeKind.Local).AddTicks(5784), "AQAAAAIAAYagAAAAED9rxYkzQIxm1+3DAhTY8jt51pT5U5FHbyGwEQVb0ESNpwc7udonG/wh5JcgAkdufg==", "abcb10ee-5bb1-42be-8877-d5c9ec3f4ae0" });

            migrationBuilder.CreateIndex(
                name: "IX_FormerSchools_CurriculumId",
                table: "FormerSchools",
                column: "CurriculumId");

            migrationBuilder.AddForeignKey(
                name: "FK_FormerSchools_Curricula_CurriculumId",
                table: "FormerSchools",
                column: "CurriculumId",
                principalTable: "Curricula",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FormerSchools_Curricula_CurriculumId",
                table: "FormerSchools");

            migrationBuilder.DropIndex(
                name: "IX_FormerSchools_CurriculumId",
                table: "FormerSchools");

            migrationBuilder.DropColumn(
                name: "CurriculumId",
                table: "FormerSchools");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "269f0cf3-405e-4163-83f3-1b63ebebd62e",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2024, 5, 17, 21, 5, 42, 910, DateTimeKind.Local).AddTicks(3830), new DateTime(2024, 5, 17, 21, 5, 42, 910, DateTimeKind.Local).AddTicks(3846) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "448df289-142c-4959-a912-60733515e1b4",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2024, 5, 17, 21, 5, 42, 910, DateTimeKind.Local).AddTicks(3775), new DateTime(2024, 5, 17, 21, 5, 42, 910, DateTimeKind.Local).AddTicks(3777) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "48c50c3a-9958-453b-b649-4e21af131322",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2024, 5, 17, 21, 5, 42, 910, DateTimeKind.Local).AddTicks(3736), new DateTime(2024, 5, 17, 21, 5, 42, 910, DateTimeKind.Local).AddTicks(3738) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "717d9b15-a428-440c-b26b-08d3bbb68b02",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2024, 5, 17, 21, 5, 42, 910, DateTimeKind.Local).AddTicks(3540), new DateTime(2024, 5, 17, 21, 5, 42, 910, DateTimeKind.Local).AddTicks(3586) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "95ed2407-3e58-4af2-88a4-1c4e96473f68",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2024, 5, 17, 21, 5, 42, 910, DateTimeKind.Local).AddTicks(3695), new DateTime(2024, 5, 17, 21, 5, 42, 910, DateTimeKind.Local).AddTicks(3697) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "97942bee-ef12-4425-8225-4f293d0f36dd",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2024, 5, 17, 21, 5, 42, 910, DateTimeKind.Local).AddTicks(3937), new DateTime(2024, 5, 17, 21, 5, 42, 910, DateTimeKind.Local).AddTicks(3938) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "cd12b44b-103b-48df-8887-a2bf42e0651e",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2024, 5, 17, 21, 5, 42, 910, DateTimeKind.Local).AddTicks(3898), new DateTime(2024, 5, 17, 21, 5, 42, 910, DateTimeKind.Local).AddTicks(3900) });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "7e67d486-af3e-49f1-a109-a2b864b8e0ec",
                columns: new[] { "ConcurrencyStamp", "Created", "Modified", "PasswordHash", "SecurityStamp" },
                values: new object[] { "b18976e7-000f-40b6-ab8e-a3c1b4e9c63d", new DateTime(2024, 5, 17, 21, 5, 42, 910, DateTimeKind.Local).AddTicks(4075), new DateTime(2024, 5, 17, 21, 5, 42, 910, DateTimeKind.Local).AddTicks(4078), "AQAAAAIAAYagAAAAEFfK1siaccVhQLbUfQKhdeZfWFW7+3Jrvb916Qy6m0JBtMjKOU6Oxeosh+hr3fZZgw==", "8589a589-5a2f-49ee-a385-fb52a72dd178" });
        }
    }
}
