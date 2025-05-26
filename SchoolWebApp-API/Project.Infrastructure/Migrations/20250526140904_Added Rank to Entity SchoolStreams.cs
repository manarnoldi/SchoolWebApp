using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SchoolWebApp.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddedRanktoEntitySchoolStreams : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Rank",
                table: "SchoolStreams",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "269f0cf3-405e-4163-83f3-1b63ebebd62e",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2025, 5, 26, 17, 9, 1, 750, DateTimeKind.Local).AddTicks(5754), new DateTime(2025, 5, 26, 17, 9, 1, 750, DateTimeKind.Local).AddTicks(5777) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "448df289-142c-4959-a912-60733515e1b4",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2025, 5, 26, 17, 9, 1, 750, DateTimeKind.Local).AddTicks(5618), new DateTime(2025, 5, 26, 17, 9, 1, 750, DateTimeKind.Local).AddTicks(5622) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "48c50c3a-9958-453b-b649-4e21af131322",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2025, 5, 26, 17, 9, 1, 750, DateTimeKind.Local).AddTicks(5518), new DateTime(2025, 5, 26, 17, 9, 1, 750, DateTimeKind.Local).AddTicks(5522) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "717d9b15-a428-440c-b26b-08d3bbb68b02",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2025, 5, 26, 17, 9, 1, 750, DateTimeKind.Local).AddTicks(5026), new DateTime(2025, 5, 26, 17, 9, 1, 750, DateTimeKind.Local).AddTicks(5101) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "95ed2407-3e58-4af2-88a4-1c4e96473f68",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2025, 5, 26, 17, 9, 1, 750, DateTimeKind.Local).AddTicks(5375), new DateTime(2025, 5, 26, 17, 9, 1, 750, DateTimeKind.Local).AddTicks(5379) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "97942bee-ef12-4425-8225-4f293d0f36dd",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2025, 5, 26, 17, 9, 1, 750, DateTimeKind.Local).AddTicks(5969), new DateTime(2025, 5, 26, 17, 9, 1, 750, DateTimeKind.Local).AddTicks(5973) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "cd12b44b-103b-48df-8887-a2bf42e0651e",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2025, 5, 26, 17, 9, 1, 750, DateTimeKind.Local).AddTicks(5902), new DateTime(2025, 5, 26, 17, 9, 1, 750, DateTimeKind.Local).AddTicks(5907) });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "7e67d486-af3e-49f1-a109-a2b864b8e0ec",
                columns: new[] { "ConcurrencyStamp", "Created", "Modified", "PasswordHash", "SecurityStamp" },
                values: new object[] { "b458ed57-e525-4e5c-9384-e37a75699f3f", new DateTime(2025, 5, 26, 17, 9, 1, 750, DateTimeKind.Local).AddTicks(6178), new DateTime(2025, 5, 26, 17, 9, 1, 750, DateTimeKind.Local).AddTicks(6182), "AQAAAAIAAYagAAAAEHjtj+Wxm9MwtFRpeGpjN4nX2erqdSOwf3TpYF+1ceeOFkGUZGrixwWg4o0mqkZW3w==", "6cb141f5-24e7-4dae-9c56-b0fdee3592ec" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Rank",
                table: "SchoolStreams");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "269f0cf3-405e-4163-83f3-1b63ebebd62e",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2025, 5, 10, 7, 32, 26, 864, DateTimeKind.Local).AddTicks(1794), new DateTime(2025, 5, 10, 7, 32, 26, 864, DateTimeKind.Local).AddTicks(1812) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "448df289-142c-4959-a912-60733515e1b4",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2025, 5, 10, 7, 32, 26, 864, DateTimeKind.Local).AddTicks(1754), new DateTime(2025, 5, 10, 7, 32, 26, 864, DateTimeKind.Local).AddTicks(1756) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "48c50c3a-9958-453b-b649-4e21af131322",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2025, 5, 10, 7, 32, 26, 864, DateTimeKind.Local).AddTicks(1714), new DateTime(2025, 5, 10, 7, 32, 26, 864, DateTimeKind.Local).AddTicks(1716) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "717d9b15-a428-440c-b26b-08d3bbb68b02",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2025, 5, 10, 7, 32, 26, 864, DateTimeKind.Local).AddTicks(1560), new DateTime(2025, 5, 10, 7, 32, 26, 864, DateTimeKind.Local).AddTicks(1598) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "95ed2407-3e58-4af2-88a4-1c4e96473f68",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2025, 5, 10, 7, 32, 26, 864, DateTimeKind.Local).AddTicks(1673), new DateTime(2025, 5, 10, 7, 32, 26, 864, DateTimeKind.Local).AddTicks(1675) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "97942bee-ef12-4425-8225-4f293d0f36dd",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2025, 5, 10, 7, 32, 26, 864, DateTimeKind.Local).AddTicks(1904), new DateTime(2025, 5, 10, 7, 32, 26, 864, DateTimeKind.Local).AddTicks(1906) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "cd12b44b-103b-48df-8887-a2bf42e0651e",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2025, 5, 10, 7, 32, 26, 864, DateTimeKind.Local).AddTicks(1864), new DateTime(2025, 5, 10, 7, 32, 26, 864, DateTimeKind.Local).AddTicks(1868) });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "7e67d486-af3e-49f1-a109-a2b864b8e0ec",
                columns: new[] { "ConcurrencyStamp", "Created", "Modified", "PasswordHash", "SecurityStamp" },
                values: new object[] { "5ce54d61-669f-4290-9735-45ce5ac22729", new DateTime(2025, 5, 10, 7, 32, 26, 864, DateTimeKind.Local).AddTicks(2212), new DateTime(2025, 5, 10, 7, 32, 26, 864, DateTimeKind.Local).AddTicks(2215), "AQAAAAIAAYagAAAAELMUZ/rCv/BSX/7yEkvbrD4LjVPQj7n+B0PXtFZ7YCxS21h45FGYk2N/lYSM6L8Qtg==", "5787c3a8-2423-4763-95eb-377b2ec0ee0c" });
        }
    }
}
