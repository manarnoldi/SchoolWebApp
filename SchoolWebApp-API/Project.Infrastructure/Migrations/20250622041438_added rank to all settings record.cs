using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SchoolWebApp.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class addedranktoallsettingsrecord : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<bool>(
                name: "Present",
                table: "StudentAttendances",
                type: "tinyint(1)",
                nullable: true,
                oldClrType: typeof(bool),
                oldType: "tinyint(1)");

            migrationBuilder.AddColumn<int>(
                name: "Rank",
                table: "StaffCategories",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<bool>(
                name: "Present",
                table: "StaffAttendances",
                type: "tinyint(1)",
                nullable: true,
                oldClrType: typeof(bool),
                oldType: "tinyint(1)");

            migrationBuilder.AddColumn<int>(
                name: "Rank",
                table: "SessionTypes",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Rank",
                table: "Religions",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Rank",
                table: "RelationShips",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Rank",
                table: "Outcomes",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Rank",
                table: "OccurenceTypes",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Rank",
                table: "Occupations",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Rank",
                table: "Nationalities",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Rank",
                table: "Genders",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Rank",
                table: "ExamTypes",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Rank",
                table: "EmploymentTypes",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Rank",
                table: "Designations",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Rank",
                table: "ClassLeadershipRoles",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "269f0cf3-405e-4163-83f3-1b63ebebd62e",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2025, 6, 22, 7, 14, 37, 394, DateTimeKind.Local).AddTicks(3140), new DateTime(2025, 6, 22, 7, 14, 37, 394, DateTimeKind.Local).AddTicks(3162) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "448df289-142c-4959-a912-60733515e1b4",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2025, 6, 22, 7, 14, 37, 394, DateTimeKind.Local).AddTicks(3099), new DateTime(2025, 6, 22, 7, 14, 37, 394, DateTimeKind.Local).AddTicks(3100) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "48c50c3a-9958-453b-b649-4e21af131322",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2025, 6, 22, 7, 14, 37, 394, DateTimeKind.Local).AddTicks(3070), new DateTime(2025, 6, 22, 7, 14, 37, 394, DateTimeKind.Local).AddTicks(3071) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "717d9b15-a428-440c-b26b-08d3bbb68b02",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2025, 6, 22, 7, 14, 37, 394, DateTimeKind.Local).AddTicks(2943), new DateTime(2025, 6, 22, 7, 14, 37, 394, DateTimeKind.Local).AddTicks(2975) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "95ed2407-3e58-4af2-88a4-1c4e96473f68",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2025, 6, 22, 7, 14, 37, 394, DateTimeKind.Local).AddTicks(3034), new DateTime(2025, 6, 22, 7, 14, 37, 394, DateTimeKind.Local).AddTicks(3035) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "97942bee-ef12-4425-8225-4f293d0f36dd",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2025, 6, 22, 7, 14, 37, 394, DateTimeKind.Local).AddTicks(3238), new DateTime(2025, 6, 22, 7, 14, 37, 394, DateTimeKind.Local).AddTicks(3240) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "cd12b44b-103b-48df-8887-a2bf42e0651e",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2025, 6, 22, 7, 14, 37, 394, DateTimeKind.Local).AddTicks(3200), new DateTime(2025, 6, 22, 7, 14, 37, 394, DateTimeKind.Local).AddTicks(3202) });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "7e67d486-af3e-49f1-a109-a2b864b8e0ec",
                columns: new[] { "ConcurrencyStamp", "Created", "Modified", "PasswordHash", "SecurityStamp" },
                values: new object[] { "fbac1684-8dd8-4261-bbee-dc528ebd6797", new DateTime(2025, 6, 22, 7, 14, 37, 394, DateTimeKind.Local).AddTicks(3328), new DateTime(2025, 6, 22, 7, 14, 37, 394, DateTimeKind.Local).AddTicks(3330), "AQAAAAIAAYagAAAAEKbQxjZy369ZsnuYxB6VP4F0pZZ6cwEoSRfqHsoY0oD80E4BliEokwjkyWqleoc4ow==", "76544aed-781d-4334-bdb1-0b1ca7f0c6c1" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Rank",
                table: "StaffCategories");

            migrationBuilder.DropColumn(
                name: "Rank",
                table: "SessionTypes");

            migrationBuilder.DropColumn(
                name: "Rank",
                table: "Religions");

            migrationBuilder.DropColumn(
                name: "Rank",
                table: "RelationShips");

            migrationBuilder.DropColumn(
                name: "Rank",
                table: "Outcomes");

            migrationBuilder.DropColumn(
                name: "Rank",
                table: "OccurenceTypes");

            migrationBuilder.DropColumn(
                name: "Rank",
                table: "Occupations");

            migrationBuilder.DropColumn(
                name: "Rank",
                table: "Nationalities");

            migrationBuilder.DropColumn(
                name: "Rank",
                table: "Genders");

            migrationBuilder.DropColumn(
                name: "Rank",
                table: "ExamTypes");

            migrationBuilder.DropColumn(
                name: "Rank",
                table: "EmploymentTypes");

            migrationBuilder.DropColumn(
                name: "Rank",
                table: "Designations");

            migrationBuilder.DropColumn(
                name: "Rank",
                table: "ClassLeadershipRoles");

            migrationBuilder.AlterColumn<bool>(
                name: "Present",
                table: "StudentAttendances",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(bool),
                oldType: "tinyint(1)",
                oldNullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "Present",
                table: "StaffAttendances",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(bool),
                oldType: "tinyint(1)",
                oldNullable: true);

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "269f0cf3-405e-4163-83f3-1b63ebebd62e",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2025, 5, 29, 7, 38, 53, 568, DateTimeKind.Local).AddTicks(4944), new DateTime(2025, 5, 29, 7, 38, 53, 568, DateTimeKind.Local).AddTicks(4964) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "448df289-142c-4959-a912-60733515e1b4",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2025, 5, 29, 7, 38, 53, 568, DateTimeKind.Local).AddTicks(4843), new DateTime(2025, 5, 29, 7, 38, 53, 568, DateTimeKind.Local).AddTicks(4847) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "48c50c3a-9958-453b-b649-4e21af131322",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2025, 5, 29, 7, 38, 53, 568, DateTimeKind.Local).AddTicks(4736), new DateTime(2025, 5, 29, 7, 38, 53, 568, DateTimeKind.Local).AddTicks(4740) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "717d9b15-a428-440c-b26b-08d3bbb68b02",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2025, 5, 29, 7, 38, 53, 568, DateTimeKind.Local).AddTicks(4297), new DateTime(2025, 5, 29, 7, 38, 53, 568, DateTimeKind.Local).AddTicks(4392) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "95ed2407-3e58-4af2-88a4-1c4e96473f68",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2025, 5, 29, 7, 38, 53, 568, DateTimeKind.Local).AddTicks(4661), new DateTime(2025, 5, 29, 7, 38, 53, 568, DateTimeKind.Local).AddTicks(4665) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "97942bee-ef12-4425-8225-4f293d0f36dd",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2025, 5, 29, 7, 38, 53, 568, DateTimeKind.Local).AddTicks(5197), new DateTime(2025, 5, 29, 7, 38, 53, 568, DateTimeKind.Local).AddTicks(5200) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "cd12b44b-103b-48df-8887-a2bf42e0651e",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2025, 5, 29, 7, 38, 53, 568, DateTimeKind.Local).AddTicks(5127), new DateTime(2025, 5, 29, 7, 38, 53, 568, DateTimeKind.Local).AddTicks(5131) });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "7e67d486-af3e-49f1-a109-a2b864b8e0ec",
                columns: new[] { "ConcurrencyStamp", "Created", "Modified", "PasswordHash", "SecurityStamp" },
                values: new object[] { "dbec0904-24fd-4488-98dc-733fc5c78219", new DateTime(2025, 5, 29, 7, 38, 53, 568, DateTimeKind.Local).AddTicks(5416), new DateTime(2025, 5, 29, 7, 38, 53, 568, DateTimeKind.Local).AddTicks(5423), "AQAAAAIAAYagAAAAEMtD5uVSzIUf5ab2g9wVM9wPFYzZMmjwZ/iSm6AcWt83R4VTF5kXR29JbeMP+opxHw==", "9f5dae26-8bdd-4d6b-a0d4-6f95940db692" });
        }
    }
}
