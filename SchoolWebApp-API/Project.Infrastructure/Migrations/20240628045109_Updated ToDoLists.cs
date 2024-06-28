using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SchoolWebApp.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdatedToDoLists : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "269f0cf3-405e-4163-83f3-1b63ebebd62e",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2024, 6, 28, 7, 51, 8, 550, DateTimeKind.Local).AddTicks(1230), new DateTime(2024, 6, 28, 7, 51, 8, 550, DateTimeKind.Local).AddTicks(1231) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "448df289-142c-4959-a912-60733515e1b4",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2024, 6, 28, 7, 51, 8, 550, DateTimeKind.Local).AddTicks(1205), new DateTime(2024, 6, 28, 7, 51, 8, 550, DateTimeKind.Local).AddTicks(1206) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "48c50c3a-9958-453b-b649-4e21af131322",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2024, 6, 28, 7, 51, 8, 550, DateTimeKind.Local).AddTicks(1178), new DateTime(2024, 6, 28, 7, 51, 8, 550, DateTimeKind.Local).AddTicks(1181) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "717d9b15-a428-440c-b26b-08d3bbb68b02",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2024, 6, 28, 7, 51, 8, 550, DateTimeKind.Local).AddTicks(1060), new DateTime(2024, 6, 28, 7, 51, 8, 550, DateTimeKind.Local).AddTicks(1084) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "95ed2407-3e58-4af2-88a4-1c4e96473f68",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2024, 6, 28, 7, 51, 8, 550, DateTimeKind.Local).AddTicks(1145), new DateTime(2024, 6, 28, 7, 51, 8, 550, DateTimeKind.Local).AddTicks(1152) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "97942bee-ef12-4425-8225-4f293d0f36dd",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2024, 6, 28, 7, 51, 8, 550, DateTimeKind.Local).AddTicks(1341), new DateTime(2024, 6, 28, 7, 51, 8, 550, DateTimeKind.Local).AddTicks(1343) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "cd12b44b-103b-48df-8887-a2bf42e0651e",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2024, 6, 28, 7, 51, 8, 550, DateTimeKind.Local).AddTicks(1259), new DateTime(2024, 6, 28, 7, 51, 8, 550, DateTimeKind.Local).AddTicks(1271) });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "7e67d486-af3e-49f1-a109-a2b864b8e0ec",
                columns: new[] { "ConcurrencyStamp", "Created", "Modified", "PasswordHash", "SecurityStamp" },
                values: new object[] { "a357ff00-87c1-4375-8ed6-3e2727721589", new DateTime(2024, 6, 28, 7, 51, 8, 550, DateTimeKind.Local).AddTicks(1541), new DateTime(2024, 6, 28, 7, 51, 8, 550, DateTimeKind.Local).AddTicks(1542), "AQAAAAIAAYagAAAAEOu/MKv+vl7UAdVIxdRTyfg3SkDUT9t9YUQRXWDHwTLYBTyDpB4HE4x1d03Ms6P0JA==", "3467cf75-f3e7-4d6a-9470-a1d8db06d260" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "269f0cf3-405e-4163-83f3-1b63ebebd62e",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2024, 6, 28, 7, 42, 9, 163, DateTimeKind.Local).AddTicks(3899), new DateTime(2024, 6, 28, 7, 42, 9, 163, DateTimeKind.Local).AddTicks(3902) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "448df289-142c-4959-a912-60733515e1b4",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2024, 6, 28, 7, 42, 9, 163, DateTimeKind.Local).AddTicks(3827), new DateTime(2024, 6, 28, 7, 42, 9, 163, DateTimeKind.Local).AddTicks(3830) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "48c50c3a-9958-453b-b649-4e21af131322",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2024, 6, 28, 7, 42, 9, 163, DateTimeKind.Local).AddTicks(3755), new DateTime(2024, 6, 28, 7, 42, 9, 163, DateTimeKind.Local).AddTicks(3758) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "717d9b15-a428-440c-b26b-08d3bbb68b02",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2024, 6, 28, 7, 42, 9, 163, DateTimeKind.Local).AddTicks(3468), new DateTime(2024, 6, 28, 7, 42, 9, 163, DateTimeKind.Local).AddTicks(3510) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "95ed2407-3e58-4af2-88a4-1c4e96473f68",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2024, 6, 28, 7, 42, 9, 163, DateTimeKind.Local).AddTicks(3676), new DateTime(2024, 6, 28, 7, 42, 9, 163, DateTimeKind.Local).AddTicks(3680) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "97942bee-ef12-4425-8225-4f293d0f36dd",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2024, 6, 28, 7, 42, 9, 163, DateTimeKind.Local).AddTicks(4108), new DateTime(2024, 6, 28, 7, 42, 9, 163, DateTimeKind.Local).AddTicks(4112) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "cd12b44b-103b-48df-8887-a2bf42e0651e",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2024, 6, 28, 7, 42, 9, 163, DateTimeKind.Local).AddTicks(4002), new DateTime(2024, 6, 28, 7, 42, 9, 163, DateTimeKind.Local).AddTicks(4022) });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "7e67d486-af3e-49f1-a109-a2b864b8e0ec",
                columns: new[] { "ConcurrencyStamp", "Created", "Modified", "PasswordHash", "SecurityStamp" },
                values: new object[] { "3b8ebc86-cc5b-4c57-aa15-8c0ffd89ac81", new DateTime(2024, 6, 28, 7, 42, 9, 163, DateTimeKind.Local).AddTicks(4320), new DateTime(2024, 6, 28, 7, 42, 9, 163, DateTimeKind.Local).AddTicks(4325), "AQAAAAIAAYagAAAAEBbFqcc7BrShWC1gdF0bCOLvoraISRBJVXzNzvGPXyPmpt817evQ82AowPpY6FPSsw==", "fa3f1cda-41c9-446f-87a7-cadf3208e699" });
        }
    }
}
