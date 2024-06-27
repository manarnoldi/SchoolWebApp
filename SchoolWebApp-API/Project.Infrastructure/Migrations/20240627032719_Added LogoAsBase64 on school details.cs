using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SchoolWebApp.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddedLogoAsBase64onschooldetails : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "LogoAsBase64",
                table: "SchoolDetails",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "269f0cf3-405e-4163-83f3-1b63ebebd62e",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2024, 6, 27, 6, 27, 17, 898, DateTimeKind.Local).AddTicks(6787), new DateTime(2024, 6, 27, 6, 27, 17, 898, DateTimeKind.Local).AddTicks(6799) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "448df289-142c-4959-a912-60733515e1b4",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2024, 6, 27, 6, 27, 17, 898, DateTimeKind.Local).AddTicks(6757), new DateTime(2024, 6, 27, 6, 27, 17, 898, DateTimeKind.Local).AddTicks(6759) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "48c50c3a-9958-453b-b649-4e21af131322",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2024, 6, 27, 6, 27, 17, 898, DateTimeKind.Local).AddTicks(6714), new DateTime(2024, 6, 27, 6, 27, 17, 898, DateTimeKind.Local).AddTicks(6715) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "717d9b15-a428-440c-b26b-08d3bbb68b02",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2024, 6, 27, 6, 27, 17, 898, DateTimeKind.Local).AddTicks(6586), new DateTime(2024, 6, 27, 6, 27, 17, 898, DateTimeKind.Local).AddTicks(6625) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "95ed2407-3e58-4af2-88a4-1c4e96473f68",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2024, 6, 27, 6, 27, 17, 898, DateTimeKind.Local).AddTicks(6683), new DateTime(2024, 6, 27, 6, 27, 17, 898, DateTimeKind.Local).AddTicks(6685) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "97942bee-ef12-4425-8225-4f293d0f36dd",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2024, 6, 27, 6, 27, 17, 898, DateTimeKind.Local).AddTicks(6928), new DateTime(2024, 6, 27, 6, 27, 17, 898, DateTimeKind.Local).AddTicks(6930) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "cd12b44b-103b-48df-8887-a2bf42e0651e",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2024, 6, 27, 6, 27, 17, 898, DateTimeKind.Local).AddTicks(6877), new DateTime(2024, 6, 27, 6, 27, 17, 898, DateTimeKind.Local).AddTicks(6880) });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "7e67d486-af3e-49f1-a109-a2b864b8e0ec",
                columns: new[] { "ConcurrencyStamp", "Created", "Modified", "PasswordHash", "SecurityStamp" },
                values: new object[] { "b8c13f48-5db0-4bc0-bf41-220bee4ddd26", new DateTime(2024, 6, 27, 6, 27, 17, 898, DateTimeKind.Local).AddTicks(7121), new DateTime(2024, 6, 27, 6, 27, 17, 898, DateTimeKind.Local).AddTicks(7124), "AQAAAAIAAYagAAAAEDRLsLujqrvL+uP1dXvfq6L8FA83ZNaSeV8Hp/4enaT3F2wHgkEIjPO37S41zbdrNQ==", "40b55061-a6b6-4044-8981-b41ca727d40b" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LogoAsBase64",
                table: "SchoolDetails");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "269f0cf3-405e-4163-83f3-1b63ebebd62e",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2024, 6, 26, 10, 39, 23, 541, DateTimeKind.Local).AddTicks(2860), new DateTime(2024, 6, 26, 10, 39, 23, 541, DateTimeKind.Local).AddTicks(2871) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "448df289-142c-4959-a912-60733515e1b4",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2024, 6, 26, 10, 39, 23, 541, DateTimeKind.Local).AddTicks(2822), new DateTime(2024, 6, 26, 10, 39, 23, 541, DateTimeKind.Local).AddTicks(2823) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "48c50c3a-9958-453b-b649-4e21af131322",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2024, 6, 26, 10, 39, 23, 541, DateTimeKind.Local).AddTicks(2784), new DateTime(2024, 6, 26, 10, 39, 23, 541, DateTimeKind.Local).AddTicks(2785) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "717d9b15-a428-440c-b26b-08d3bbb68b02",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2024, 6, 26, 10, 39, 23, 541, DateTimeKind.Local).AddTicks(2621), new DateTime(2024, 6, 26, 10, 39, 23, 541, DateTimeKind.Local).AddTicks(2657) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "95ed2407-3e58-4af2-88a4-1c4e96473f68",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2024, 6, 26, 10, 39, 23, 541, DateTimeKind.Local).AddTicks(2733), new DateTime(2024, 6, 26, 10, 39, 23, 541, DateTimeKind.Local).AddTicks(2734) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "97942bee-ef12-4425-8225-4f293d0f36dd",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2024, 6, 26, 10, 39, 23, 541, DateTimeKind.Local).AddTicks(2973), new DateTime(2024, 6, 26, 10, 39, 23, 541, DateTimeKind.Local).AddTicks(2974) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "cd12b44b-103b-48df-8887-a2bf42e0651e",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2024, 6, 26, 10, 39, 23, 541, DateTimeKind.Local).AddTicks(2938), new DateTime(2024, 6, 26, 10, 39, 23, 541, DateTimeKind.Local).AddTicks(2940) });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "7e67d486-af3e-49f1-a109-a2b864b8e0ec",
                columns: new[] { "ConcurrencyStamp", "Created", "Modified", "PasswordHash", "SecurityStamp" },
                values: new object[] { "a72a9a70-6586-4fb3-9516-5b59ffb7fd26", new DateTime(2024, 6, 26, 10, 39, 23, 541, DateTimeKind.Local).AddTicks(3084), new DateTime(2024, 6, 26, 10, 39, 23, 541, DateTimeKind.Local).AddTicks(3085), "AQAAAAIAAYagAAAAEHktlI/04nqHfRRmKSN4BJb4E/q2jC84Q6mpNUxxpz3etvCe1h9IgV2sytV7NtvCJQ==", "0540e317-4bb2-4a41-8332-26e8fba6d3e4" });
        }
    }
}
