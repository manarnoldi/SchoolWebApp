using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SchoolWebApp.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddedPersonTypetoClassLeadershipRoles : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "PersonType",
                table: "ClassLeadershipRoles",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "269f0cf3-405e-4163-83f3-1b63ebebd62e",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2024, 7, 12, 22, 16, 14, 878, DateTimeKind.Local).AddTicks(9398), new DateTime(2024, 7, 12, 22, 16, 14, 878, DateTimeKind.Local).AddTicks(9408) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "448df289-142c-4959-a912-60733515e1b4",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2024, 7, 12, 22, 16, 14, 878, DateTimeKind.Local).AddTicks(9353), new DateTime(2024, 7, 12, 22, 16, 14, 878, DateTimeKind.Local).AddTicks(9355) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "48c50c3a-9958-453b-b649-4e21af131322",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2024, 7, 12, 22, 16, 14, 878, DateTimeKind.Local).AddTicks(9324), new DateTime(2024, 7, 12, 22, 16, 14, 878, DateTimeKind.Local).AddTicks(9326) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "717d9b15-a428-440c-b26b-08d3bbb68b02",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2024, 7, 12, 22, 16, 14, 878, DateTimeKind.Local).AddTicks(9186), new DateTime(2024, 7, 12, 22, 16, 14, 878, DateTimeKind.Local).AddTicks(9221) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "95ed2407-3e58-4af2-88a4-1c4e96473f68",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2024, 7, 12, 22, 16, 14, 878, DateTimeKind.Local).AddTicks(9282), new DateTime(2024, 7, 12, 22, 16, 14, 878, DateTimeKind.Local).AddTicks(9283) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "97942bee-ef12-4425-8225-4f293d0f36dd",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2024, 7, 12, 22, 16, 14, 878, DateTimeKind.Local).AddTicks(9485), new DateTime(2024, 7, 12, 22, 16, 14, 878, DateTimeKind.Local).AddTicks(9487) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "cd12b44b-103b-48df-8887-a2bf42e0651e",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2024, 7, 12, 22, 16, 14, 878, DateTimeKind.Local).AddTicks(9457), new DateTime(2024, 7, 12, 22, 16, 14, 878, DateTimeKind.Local).AddTicks(9459) });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "7e67d486-af3e-49f1-a109-a2b864b8e0ec",
                columns: new[] { "ConcurrencyStamp", "Created", "Modified", "PasswordHash", "SecurityStamp" },
                values: new object[] { "aefdd437-1b4b-4f98-a753-5f3368092de6", new DateTime(2024, 7, 12, 22, 16, 14, 878, DateTimeKind.Local).AddTicks(9608), new DateTime(2024, 7, 12, 22, 16, 14, 878, DateTimeKind.Local).AddTicks(9610), "AQAAAAIAAYagAAAAENz2q0ntTKnCPhL/LD1a6fIfCI/zgOYoepVYSNtmhIzEGh1R+BuS8v+/Sg239tA2ow==", "c831359a-3d0d-49f4-a2ae-850992644742" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PersonType",
                table: "ClassLeadershipRoles");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "269f0cf3-405e-4163-83f3-1b63ebebd62e",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2024, 7, 11, 8, 10, 46, 637, DateTimeKind.Local).AddTicks(358), new DateTime(2024, 7, 11, 8, 10, 46, 637, DateTimeKind.Local).AddTicks(375) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "448df289-142c-4959-a912-60733515e1b4",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2024, 7, 11, 8, 10, 46, 637, DateTimeKind.Local).AddTicks(309), new DateTime(2024, 7, 11, 8, 10, 46, 637, DateTimeKind.Local).AddTicks(310) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "48c50c3a-9958-453b-b649-4e21af131322",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2024, 7, 11, 8, 10, 46, 637, DateTimeKind.Local).AddTicks(277), new DateTime(2024, 7, 11, 8, 10, 46, 637, DateTimeKind.Local).AddTicks(278) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "717d9b15-a428-440c-b26b-08d3bbb68b02",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2024, 7, 11, 8, 10, 46, 637, DateTimeKind.Local).AddTicks(133), new DateTime(2024, 7, 11, 8, 10, 46, 637, DateTimeKind.Local).AddTicks(177) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "95ed2407-3e58-4af2-88a4-1c4e96473f68",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2024, 7, 11, 8, 10, 46, 637, DateTimeKind.Local).AddTicks(244), new DateTime(2024, 7, 11, 8, 10, 46, 637, DateTimeKind.Local).AddTicks(245) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "97942bee-ef12-4425-8225-4f293d0f36dd",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2024, 7, 11, 8, 10, 46, 637, DateTimeKind.Local).AddTicks(450), new DateTime(2024, 7, 11, 8, 10, 46, 637, DateTimeKind.Local).AddTicks(452) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "cd12b44b-103b-48df-8887-a2bf42e0651e",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2024, 7, 11, 8, 10, 46, 637, DateTimeKind.Local).AddTicks(421), new DateTime(2024, 7, 11, 8, 10, 46, 637, DateTimeKind.Local).AddTicks(423) });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "7e67d486-af3e-49f1-a109-a2b864b8e0ec",
                columns: new[] { "ConcurrencyStamp", "Created", "Modified", "PasswordHash", "SecurityStamp" },
                values: new object[] { "7fad1984-841e-4bf6-8803-87db252c1985", new DateTime(2024, 7, 11, 8, 10, 46, 637, DateTimeKind.Local).AddTicks(563), new DateTime(2024, 7, 11, 8, 10, 46, 637, DateTimeKind.Local).AddTicks(565), "AQAAAAIAAYagAAAAEGK4dgh5PVE92+3xMOMs9BLeZwIkQF7LeAQS03JD96lmx/HTUqmcy5S203st0cGXOg==", "bf7d6e1c-556d-4a1a-ba52-329c681819dd" });
        }
    }
}
