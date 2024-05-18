using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SchoolWebApp.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddedDescriptiononFormerSchoolentity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "FormerSchools",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "269f0cf3-405e-4163-83f3-1b63ebebd62e",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2024, 5, 18, 9, 46, 10, 569, DateTimeKind.Local).AddTicks(5825), new DateTime(2024, 5, 18, 9, 46, 10, 569, DateTimeKind.Local).AddTicks(5845) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "448df289-142c-4959-a912-60733515e1b4",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2024, 5, 18, 9, 46, 10, 569, DateTimeKind.Local).AddTicks(5797), new DateTime(2024, 5, 18, 9, 46, 10, 569, DateTimeKind.Local).AddTicks(5799) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "48c50c3a-9958-453b-b649-4e21af131322",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2024, 5, 18, 9, 46, 10, 569, DateTimeKind.Local).AddTicks(5767), new DateTime(2024, 5, 18, 9, 46, 10, 569, DateTimeKind.Local).AddTicks(5768) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "717d9b15-a428-440c-b26b-08d3bbb68b02",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2024, 5, 18, 9, 46, 10, 569, DateTimeKind.Local).AddTicks(5574), new DateTime(2024, 5, 18, 9, 46, 10, 569, DateTimeKind.Local).AddTicks(5646) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "95ed2407-3e58-4af2-88a4-1c4e96473f68",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2024, 5, 18, 9, 46, 10, 569, DateTimeKind.Local).AddTicks(5719), new DateTime(2024, 5, 18, 9, 46, 10, 569, DateTimeKind.Local).AddTicks(5721) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "97942bee-ef12-4425-8225-4f293d0f36dd",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2024, 5, 18, 9, 46, 10, 569, DateTimeKind.Local).AddTicks(5911), new DateTime(2024, 5, 18, 9, 46, 10, 569, DateTimeKind.Local).AddTicks(5912) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "cd12b44b-103b-48df-8887-a2bf42e0651e",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2024, 5, 18, 9, 46, 10, 569, DateTimeKind.Local).AddTicks(5883), new DateTime(2024, 5, 18, 9, 46, 10, 569, DateTimeKind.Local).AddTicks(5885) });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "7e67d486-af3e-49f1-a109-a2b864b8e0ec",
                columns: new[] { "ConcurrencyStamp", "Created", "Modified", "PasswordHash", "SecurityStamp" },
                values: new object[] { "f419b830-5077-49cd-b567-ac0dad7d68a3", new DateTime(2024, 5, 18, 9, 46, 10, 569, DateTimeKind.Local).AddTicks(6005), new DateTime(2024, 5, 18, 9, 46, 10, 569, DateTimeKind.Local).AddTicks(6007), "AQAAAAIAAYagAAAAEFF0MT4r9lBSMbhKzus9S/aNJVmyytkHv5dbUiv+0B6vtZAlVyIyc569VLyPy+IKOA==", "c4e70fd0-43e2-416d-8392-415f158063b0" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Description",
                table: "FormerSchools");

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
        }
    }
}
