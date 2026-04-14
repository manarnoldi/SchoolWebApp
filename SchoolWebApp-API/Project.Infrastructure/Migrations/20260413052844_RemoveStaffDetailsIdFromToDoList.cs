using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SchoolWebApp.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RemoveStaffDetailsIdFromToDoList : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "269f0cf3-405e-4163-83f3-1b63ebebd62e",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 13, 8, 28, 42, 835, DateTimeKind.Local).AddTicks(3659), new DateTime(2026, 4, 13, 8, 28, 42, 835, DateTimeKind.Local).AddTicks(3661) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "448df289-142c-4959-a912-60733515e1b4",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 13, 8, 28, 42, 835, DateTimeKind.Local).AddTicks(3625), new DateTime(2026, 4, 13, 8, 28, 42, 835, DateTimeKind.Local).AddTicks(3626) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "48c50c3a-9958-453b-b649-4e21af131322",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 13, 8, 28, 42, 835, DateTimeKind.Local).AddTicks(3590), new DateTime(2026, 4, 13, 8, 28, 42, 835, DateTimeKind.Local).AddTicks(3592) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "717d9b15-a428-440c-b26b-08d3bbb68b02",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 13, 8, 28, 42, 835, DateTimeKind.Local).AddTicks(3438), new DateTime(2026, 4, 13, 8, 28, 42, 835, DateTimeKind.Local).AddTicks(3463) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "95ed2407-3e58-4af2-88a4-1c4e96473f68",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 13, 8, 28, 42, 835, DateTimeKind.Local).AddTicks(3552), new DateTime(2026, 4, 13, 8, 28, 42, 835, DateTimeKind.Local).AddTicks(3554) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "97942bee-ef12-4425-8225-4f293d0f36dd",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 13, 8, 28, 42, 835, DateTimeKind.Local).AddTicks(3739), new DateTime(2026, 4, 13, 8, 28, 42, 835, DateTimeKind.Local).AddTicks(3740) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "cd12b44b-103b-48df-8887-a2bf42e0651e",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 13, 8, 28, 42, 835, DateTimeKind.Local).AddTicks(3694), new DateTime(2026, 4, 13, 8, 28, 42, 835, DateTimeKind.Local).AddTicks(3696) });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "7e67d486-af3e-49f1-a109-a2b864b8e0ec",
                columns: new[] { "ConcurrencyStamp", "Created", "Modified", "PasswordHash", "SecurityStamp" },
                values: new object[] { "abb2f32b-fdd7-4d72-93ef-e4322112c0bf", new DateTime(2026, 4, 13, 8, 28, 42, 835, DateTimeKind.Local).AddTicks(4227), new DateTime(2026, 4, 13, 8, 28, 42, 835, DateTimeKind.Local).AddTicks(4228), "AQAAAAIAAYagAAAAELwilftTB9dlvFZ34yT2B3CB/ulKQfTJGxTLbSb0kbiaC3HvmldhHaaxdUcpVIwgEA==", "2e04b2c7-d313-4292-a1a4-d42fcfe090bb" });

            migrationBuilder.UpdateData(
                table: "Designations",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 13, 8, 28, 42, 835, DateTimeKind.Local).AddTicks(3836), new DateTime(2026, 4, 13, 8, 28, 42, 835, DateTimeKind.Local).AddTicks(3838) });

            migrationBuilder.UpdateData(
                table: "EmploymentTypes",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 13, 8, 28, 42, 835, DateTimeKind.Local).AddTicks(3876), new DateTime(2026, 4, 13, 8, 28, 42, 835, DateTimeKind.Local).AddTicks(3878) });

            migrationBuilder.UpdateData(
                table: "Genders",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 13, 8, 28, 42, 835, DateTimeKind.Local).AddTicks(4046), new DateTime(2026, 4, 13, 8, 28, 42, 835, DateTimeKind.Local).AddTicks(4048) });

            migrationBuilder.UpdateData(
                table: "Nationalities",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 13, 8, 28, 42, 835, DateTimeKind.Local).AddTicks(3931), new DateTime(2026, 4, 13, 8, 28, 42, 835, DateTimeKind.Local).AddTicks(3934) });

            migrationBuilder.UpdateData(
                table: "Person",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "DateOfBirth", "EmploymentDate", "EndofEmploymentDate", "Modified" },
                values: new object[] { new DateTime(2026, 4, 13, 8, 28, 42, 835, DateTimeKind.Local).AddTicks(4107), new DateTime(2026, 4, 13, 8, 28, 42, 835, DateTimeKind.Local).AddTicks(4103), new DateTime(2026, 4, 13, 8, 28, 42, 835, DateTimeKind.Local).AddTicks(4097), new DateTime(2026, 4, 13, 8, 28, 42, 835, DateTimeKind.Local).AddTicks(4098), new DateTime(2026, 4, 13, 8, 28, 42, 835, DateTimeKind.Local).AddTicks(4112) });

            migrationBuilder.UpdateData(
                table: "Religions",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 13, 8, 28, 42, 835, DateTimeKind.Local).AddTicks(4014), new DateTime(2026, 4, 13, 8, 28, 42, 835, DateTimeKind.Local).AddTicks(4016) });

            migrationBuilder.UpdateData(
                table: "StaffCategories",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 13, 8, 28, 42, 835, DateTimeKind.Local).AddTicks(3794), new DateTime(2026, 4, 13, 8, 28, 42, 835, DateTimeKind.Local).AddTicks(3797) });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
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
    }
}
