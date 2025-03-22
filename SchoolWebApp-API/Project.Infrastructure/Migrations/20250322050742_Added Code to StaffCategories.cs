using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SchoolWebApp.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddedCodetoStaffCategories : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Code",
                table: "StaffCategories",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "269f0cf3-405e-4163-83f3-1b63ebebd62e",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2025, 3, 22, 8, 7, 40, 616, DateTimeKind.Local).AddTicks(8909), new DateTime(2025, 3, 22, 8, 7, 40, 616, DateTimeKind.Local).AddTicks(8930) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "448df289-142c-4959-a912-60733515e1b4",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2025, 3, 22, 8, 7, 40, 616, DateTimeKind.Local).AddTicks(8854), new DateTime(2025, 3, 22, 8, 7, 40, 616, DateTimeKind.Local).AddTicks(8856) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "48c50c3a-9958-453b-b649-4e21af131322",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2025, 3, 22, 8, 7, 40, 616, DateTimeKind.Local).AddTicks(8800), new DateTime(2025, 3, 22, 8, 7, 40, 616, DateTimeKind.Local).AddTicks(8802) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "717d9b15-a428-440c-b26b-08d3bbb68b02",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2025, 3, 22, 8, 7, 40, 616, DateTimeKind.Local).AddTicks(8564), new DateTime(2025, 3, 22, 8, 7, 40, 616, DateTimeKind.Local).AddTicks(8616) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "95ed2407-3e58-4af2-88a4-1c4e96473f68",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2025, 3, 22, 8, 7, 40, 616, DateTimeKind.Local).AddTicks(8731), new DateTime(2025, 3, 22, 8, 7, 40, 616, DateTimeKind.Local).AddTicks(8733) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "97942bee-ef12-4425-8225-4f293d0f36dd",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2025, 3, 22, 8, 7, 40, 616, DateTimeKind.Local).AddTicks(9089), new DateTime(2025, 3, 22, 8, 7, 40, 616, DateTimeKind.Local).AddTicks(9091) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "cd12b44b-103b-48df-8887-a2bf42e0651e",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2025, 3, 22, 8, 7, 40, 616, DateTimeKind.Local).AddTicks(9016), new DateTime(2025, 3, 22, 8, 7, 40, 616, DateTimeKind.Local).AddTicks(9019) });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "7e67d486-af3e-49f1-a109-a2b864b8e0ec",
                columns: new[] { "ConcurrencyStamp", "Created", "Modified", "PasswordHash", "SecurityStamp" },
                values: new object[] { "b6bcfecf-b93f-4866-9caf-e97ddcc724e7", new DateTime(2025, 3, 22, 8, 7, 40, 616, DateTimeKind.Local).AddTicks(9258), new DateTime(2025, 3, 22, 8, 7, 40, 616, DateTimeKind.Local).AddTicks(9261), "AQAAAAIAAYagAAAAEEAqGP1DEpqjrvnsbl2z2UpE1q0A60Q3adBzqw5qYhvKPpfDk5QmkUCeUwqPy9pS+g==", "034fc3f2-1902-4821-8fd9-6aa749895c5c" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Code",
                table: "StaffCategories");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "269f0cf3-405e-4163-83f3-1b63ebebd62e",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2025, 3, 21, 15, 57, 56, 936, DateTimeKind.Local).AddTicks(6269), new DateTime(2025, 3, 21, 15, 57, 56, 936, DateTimeKind.Local).AddTicks(6290) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "448df289-142c-4959-a912-60733515e1b4",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2025, 3, 21, 15, 57, 56, 936, DateTimeKind.Local).AddTicks(6187), new DateTime(2025, 3, 21, 15, 57, 56, 936, DateTimeKind.Local).AddTicks(6190) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "48c50c3a-9958-453b-b649-4e21af131322",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2025, 3, 21, 15, 57, 56, 936, DateTimeKind.Local).AddTicks(6123), new DateTime(2025, 3, 21, 15, 57, 56, 936, DateTimeKind.Local).AddTicks(6126) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "717d9b15-a428-440c-b26b-08d3bbb68b02",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2025, 3, 21, 15, 57, 56, 936, DateTimeKind.Local).AddTicks(5766), new DateTime(2025, 3, 21, 15, 57, 56, 936, DateTimeKind.Local).AddTicks(5811) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "95ed2407-3e58-4af2-88a4-1c4e96473f68",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2025, 3, 21, 15, 57, 56, 936, DateTimeKind.Local).AddTicks(6045), new DateTime(2025, 3, 21, 15, 57, 56, 936, DateTimeKind.Local).AddTicks(6049) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "97942bee-ef12-4425-8225-4f293d0f36dd",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2025, 3, 21, 15, 57, 56, 936, DateTimeKind.Local).AddTicks(6436), new DateTime(2025, 3, 21, 15, 57, 56, 936, DateTimeKind.Local).AddTicks(6438) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "cd12b44b-103b-48df-8887-a2bf42e0651e",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2025, 3, 21, 15, 57, 56, 936, DateTimeKind.Local).AddTicks(6374), new DateTime(2025, 3, 21, 15, 57, 56, 936, DateTimeKind.Local).AddTicks(6378) });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "7e67d486-af3e-49f1-a109-a2b864b8e0ec",
                columns: new[] { "ConcurrencyStamp", "Created", "Modified", "PasswordHash", "SecurityStamp" },
                values: new object[] { "d3e91798-652a-4108-8705-9a46fc256a34", new DateTime(2025, 3, 21, 15, 57, 56, 936, DateTimeKind.Local).AddTicks(6581), new DateTime(2025, 3, 21, 15, 57, 56, 936, DateTimeKind.Local).AddTicks(6585), "AQAAAAIAAYagAAAAEPIcY2KCASBtjhnNzO94l6Zyzk/WgbUp+9pq4fs7AUEkBJACrCmz7kmI8uq3bq0HjQ==", "3be37365-a3bb-41b7-925d-976c3dd3f018" });
        }
    }
}
