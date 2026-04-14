using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SchoolWebApp.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddUserIdToToDoList : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "StaffDetailsId",
                table: "ToDoLists",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "ToDoLists",
                type: "varchar(450)",
                maxLength: 450,
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "269f0cf3-405e-4163-83f3-1b63ebebd62e",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 13, 8, 18, 47, 245, DateTimeKind.Local).AddTicks(4075), new DateTime(2026, 4, 13, 8, 18, 47, 245, DateTimeKind.Local).AddTicks(4076) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "448df289-142c-4959-a912-60733515e1b4",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 13, 8, 18, 47, 245, DateTimeKind.Local).AddTicks(4059), new DateTime(2026, 4, 13, 8, 18, 47, 245, DateTimeKind.Local).AddTicks(4059) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "48c50c3a-9958-453b-b649-4e21af131322",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 13, 8, 18, 47, 245, DateTimeKind.Local).AddTicks(4043), new DateTime(2026, 4, 13, 8, 18, 47, 245, DateTimeKind.Local).AddTicks(4043) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "717d9b15-a428-440c-b26b-08d3bbb68b02",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 13, 8, 18, 47, 245, DateTimeKind.Local).AddTicks(3960), new DateTime(2026, 4, 13, 8, 18, 47, 245, DateTimeKind.Local).AddTicks(3980) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "95ed2407-3e58-4af2-88a4-1c4e96473f68",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 13, 8, 18, 47, 245, DateTimeKind.Local).AddTicks(4024), new DateTime(2026, 4, 13, 8, 18, 47, 245, DateTimeKind.Local).AddTicks(4024) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "97942bee-ef12-4425-8225-4f293d0f36dd",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 13, 8, 18, 47, 245, DateTimeKind.Local).AddTicks(4109), new DateTime(2026, 4, 13, 8, 18, 47, 245, DateTimeKind.Local).AddTicks(4110) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "cd12b44b-103b-48df-8887-a2bf42e0651e",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 13, 8, 18, 47, 245, DateTimeKind.Local).AddTicks(4093), new DateTime(2026, 4, 13, 8, 18, 47, 245, DateTimeKind.Local).AddTicks(4093) });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "7e67d486-af3e-49f1-a109-a2b864b8e0ec",
                columns: new[] { "ConcurrencyStamp", "Created", "Modified", "PasswordHash", "SecurityStamp" },
                values: new object[] { "39e756ca-7601-4e4d-abb0-9d0fb1a6bf32", new DateTime(2026, 4, 13, 8, 18, 47, 245, DateTimeKind.Local).AddTicks(4368), new DateTime(2026, 4, 13, 8, 18, 47, 245, DateTimeKind.Local).AddTicks(4369), "AQAAAAIAAYagAAAAEDfwVbsqLeiA8CfYEGaMxp/J7DPoUKymoJA7TbidNXUbV8yS45Y36oPnz86W0GPGJA==", "512dd474-363a-4f17-a73b-7911ad307035" });

            migrationBuilder.UpdateData(
                table: "Designations",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 13, 8, 18, 47, 245, DateTimeKind.Local).AddTicks(4163), new DateTime(2026, 4, 13, 8, 18, 47, 245, DateTimeKind.Local).AddTicks(4164) });

            migrationBuilder.UpdateData(
                table: "EmploymentTypes",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 13, 8, 18, 47, 245, DateTimeKind.Local).AddTicks(4189), new DateTime(2026, 4, 13, 8, 18, 47, 245, DateTimeKind.Local).AddTicks(4190) });

            migrationBuilder.UpdateData(
                table: "Genders",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 13, 8, 18, 47, 245, DateTimeKind.Local).AddTicks(4261), new DateTime(2026, 4, 13, 8, 18, 47, 245, DateTimeKind.Local).AddTicks(4261) });

            migrationBuilder.UpdateData(
                table: "Nationalities",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 13, 8, 18, 47, 245, DateTimeKind.Local).AddTicks(4219), new DateTime(2026, 4, 13, 8, 18, 47, 245, DateTimeKind.Local).AddTicks(4220) });

            migrationBuilder.UpdateData(
                table: "Person",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "DateOfBirth", "EmploymentDate", "EndofEmploymentDate", "Modified" },
                values: new object[] { new DateTime(2026, 4, 13, 8, 18, 47, 245, DateTimeKind.Local).AddTicks(4300), new DateTime(2026, 4, 13, 8, 18, 47, 245, DateTimeKind.Local).AddTicks(4297), new DateTime(2026, 4, 13, 8, 18, 47, 245, DateTimeKind.Local).AddTicks(4294), new DateTime(2026, 4, 13, 8, 18, 47, 245, DateTimeKind.Local).AddTicks(4295), new DateTime(2026, 4, 13, 8, 18, 47, 245, DateTimeKind.Local).AddTicks(4307) });

            migrationBuilder.UpdateData(
                table: "Religions",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 13, 8, 18, 47, 245, DateTimeKind.Local).AddTicks(4243), new DateTime(2026, 4, 13, 8, 18, 47, 245, DateTimeKind.Local).AddTicks(4244) });

            migrationBuilder.UpdateData(
                table: "StaffCategories",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 13, 8, 18, 47, 245, DateTimeKind.Local).AddTicks(4146), new DateTime(2026, 4, 13, 8, 18, 47, 245, DateTimeKind.Local).AddTicks(4148) });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UserId",
                table: "ToDoLists");

            migrationBuilder.AlterColumn<int>(
                name: "StaffDetailsId",
                table: "ToDoLists",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "269f0cf3-405e-4163-83f3-1b63ebebd62e",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 12, 14, 14, 40, 213, DateTimeKind.Local).AddTicks(6826), new DateTime(2026, 4, 12, 14, 14, 40, 213, DateTimeKind.Local).AddTicks(6829) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "448df289-142c-4959-a912-60733515e1b4",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 12, 14, 14, 40, 213, DateTimeKind.Local).AddTicks(6780), new DateTime(2026, 4, 12, 14, 14, 40, 213, DateTimeKind.Local).AddTicks(6783) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "48c50c3a-9958-453b-b649-4e21af131322",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 12, 14, 14, 40, 213, DateTimeKind.Local).AddTicks(6740), new DateTime(2026, 4, 12, 14, 14, 40, 213, DateTimeKind.Local).AddTicks(6742) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "717d9b15-a428-440c-b26b-08d3bbb68b02",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 12, 14, 14, 40, 213, DateTimeKind.Local).AddTicks(6602), new DateTime(2026, 4, 12, 14, 14, 40, 213, DateTimeKind.Local).AddTicks(6623) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "95ed2407-3e58-4af2-88a4-1c4e96473f68",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 12, 14, 14, 40, 213, DateTimeKind.Local).AddTicks(6701), new DateTime(2026, 4, 12, 14, 14, 40, 213, DateTimeKind.Local).AddTicks(6704) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "97942bee-ef12-4425-8225-4f293d0f36dd",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 12, 14, 14, 40, 213, DateTimeKind.Local).AddTicks(6903), new DateTime(2026, 4, 12, 14, 14, 40, 213, DateTimeKind.Local).AddTicks(6906) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "cd12b44b-103b-48df-8887-a2bf42e0651e",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 12, 14, 14, 40, 213, DateTimeKind.Local).AddTicks(6866), new DateTime(2026, 4, 12, 14, 14, 40, 213, DateTimeKind.Local).AddTicks(6868) });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "7e67d486-af3e-49f1-a109-a2b864b8e0ec",
                columns: new[] { "ConcurrencyStamp", "Created", "Modified", "PasswordHash", "SecurityStamp" },
                values: new object[] { "744ea72c-66ea-421f-bd70-dce18f3ee783", new DateTime(2026, 4, 12, 14, 14, 40, 213, DateTimeKind.Local).AddTicks(7364), new DateTime(2026, 4, 12, 14, 14, 40, 213, DateTimeKind.Local).AddTicks(7367), "AQAAAAIAAYagAAAAEJXltM6bKkfdcca4m46V4H35onPAmxGNvfVy5BC+LcIbPWoPp/O6zr3VxLcLj5eZXg==", "36f2146c-3d3e-479d-8746-03735a002751" });

            migrationBuilder.UpdateData(
                table: "Designations",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 12, 14, 14, 40, 213, DateTimeKind.Local).AddTicks(6998), new DateTime(2026, 4, 12, 14, 14, 40, 213, DateTimeKind.Local).AddTicks(6999) });

            migrationBuilder.UpdateData(
                table: "EmploymentTypes",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 12, 14, 14, 40, 213, DateTimeKind.Local).AddTicks(7046), new DateTime(2026, 4, 12, 14, 14, 40, 213, DateTimeKind.Local).AddTicks(7047) });

            migrationBuilder.UpdateData(
                table: "Genders",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 12, 14, 14, 40, 213, DateTimeKind.Local).AddTicks(7168), new DateTime(2026, 4, 12, 14, 14, 40, 213, DateTimeKind.Local).AddTicks(7170) });

            migrationBuilder.UpdateData(
                table: "Nationalities",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 12, 14, 14, 40, 213, DateTimeKind.Local).AddTicks(7084), new DateTime(2026, 4, 12, 14, 14, 40, 213, DateTimeKind.Local).AddTicks(7085) });

            migrationBuilder.UpdateData(
                table: "Person",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "DateOfBirth", "EmploymentDate", "EndofEmploymentDate", "Modified" },
                values: new object[] { new DateTime(2026, 4, 12, 14, 14, 40, 213, DateTimeKind.Local).AddTicks(7246), new DateTime(2026, 4, 12, 14, 14, 40, 213, DateTimeKind.Local).AddTicks(7243), new DateTime(2026, 4, 12, 14, 14, 40, 213, DateTimeKind.Local).AddTicks(7233), new DateTime(2026, 4, 12, 14, 14, 40, 213, DateTimeKind.Local).AddTicks(7234), new DateTime(2026, 4, 12, 14, 14, 40, 213, DateTimeKind.Local).AddTicks(7255) });

            migrationBuilder.UpdateData(
                table: "Religions",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 12, 14, 14, 40, 213, DateTimeKind.Local).AddTicks(7133), new DateTime(2026, 4, 12, 14, 14, 40, 213, DateTimeKind.Local).AddTicks(7134) });

            migrationBuilder.UpdateData(
                table: "StaffCategories",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 12, 14, 14, 40, 213, DateTimeKind.Local).AddTicks(6959), new DateTime(2026, 4, 12, 14, 14, 40, 213, DateTimeKind.Local).AddTicks(6961) });
        }
    }
}
