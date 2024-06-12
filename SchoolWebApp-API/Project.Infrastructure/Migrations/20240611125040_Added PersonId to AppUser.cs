using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SchoolWebApp.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddedPersonIdtoAppUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "PersonId",
                table: "AspNetUsers",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "269f0cf3-405e-4163-83f3-1b63ebebd62e",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2024, 6, 11, 15, 50, 39, 39, DateTimeKind.Local).AddTicks(1752), new DateTime(2024, 6, 11, 15, 50, 39, 39, DateTimeKind.Local).AddTicks(1783) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "448df289-142c-4959-a912-60733515e1b4",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2024, 6, 11, 15, 50, 39, 39, DateTimeKind.Local).AddTicks(1689), new DateTime(2024, 6, 11, 15, 50, 39, 39, DateTimeKind.Local).AddTicks(1691) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "48c50c3a-9958-453b-b649-4e21af131322",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2024, 6, 11, 15, 50, 39, 39, DateTimeKind.Local).AddTicks(1636), new DateTime(2024, 6, 11, 15, 50, 39, 39, DateTimeKind.Local).AddTicks(1638) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "717d9b15-a428-440c-b26b-08d3bbb68b02",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2024, 6, 11, 15, 50, 39, 39, DateTimeKind.Local).AddTicks(1414), new DateTime(2024, 6, 11, 15, 50, 39, 39, DateTimeKind.Local).AddTicks(1463) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "95ed2407-3e58-4af2-88a4-1c4e96473f68",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2024, 6, 11, 15, 50, 39, 39, DateTimeKind.Local).AddTicks(1577), new DateTime(2024, 6, 11, 15, 50, 39, 39, DateTimeKind.Local).AddTicks(1579) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "97942bee-ef12-4425-8225-4f293d0f36dd",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2024, 6, 11, 15, 50, 39, 39, DateTimeKind.Local).AddTicks(1903), new DateTime(2024, 6, 11, 15, 50, 39, 39, DateTimeKind.Local).AddTicks(1905) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "cd12b44b-103b-48df-8887-a2bf42e0651e",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2024, 6, 11, 15, 50, 39, 39, DateTimeKind.Local).AddTicks(1853), new DateTime(2024, 6, 11, 15, 50, 39, 39, DateTimeKind.Local).AddTicks(1855) });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "7e67d486-af3e-49f1-a109-a2b864b8e0ec",
                columns: new[] { "ConcurrencyStamp", "Created", "Modified", "PasswordHash", "PersonId", "SecurityStamp" },
                values: new object[] { "91242774-5ae3-4beb-8c25-e2c28c6d0427", new DateTime(2024, 6, 11, 15, 50, 39, 39, DateTimeKind.Local).AddTicks(2052), new DateTime(2024, 6, 11, 15, 50, 39, 39, DateTimeKind.Local).AddTicks(2054), "AQAAAAIAAYagAAAAEDZAw6fQzXBSHU70oy6RSjiVjCmTItqlSPmb+qPHSLshoUMthhGETyjiuasc8eUoCw==", 5, "bcb74c6f-2852-46d2-a0fa-6a401f07c248" });

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_PersonId",
                table: "AspNetUsers",
                column: "PersonId");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_Person_PersonId",
                table: "AspNetUsers",
                column: "PersonId",
                principalTable: "Person",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_Person_PersonId",
                table: "AspNetUsers");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_PersonId",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "PersonId",
                table: "AspNetUsers");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "269f0cf3-405e-4163-83f3-1b63ebebd62e",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2024, 5, 20, 9, 8, 32, 775, DateTimeKind.Local).AddTicks(1294), new DateTime(2024, 5, 20, 9, 8, 32, 775, DateTimeKind.Local).AddTicks(1306) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "448df289-142c-4959-a912-60733515e1b4",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2024, 5, 20, 9, 8, 32, 775, DateTimeKind.Local).AddTicks(1267), new DateTime(2024, 5, 20, 9, 8, 32, 775, DateTimeKind.Local).AddTicks(1268) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "48c50c3a-9958-453b-b649-4e21af131322",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2024, 5, 20, 9, 8, 32, 775, DateTimeKind.Local).AddTicks(1222), new DateTime(2024, 5, 20, 9, 8, 32, 775, DateTimeKind.Local).AddTicks(1223) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "717d9b15-a428-440c-b26b-08d3bbb68b02",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2024, 5, 20, 9, 8, 32, 775, DateTimeKind.Local).AddTicks(1100), new DateTime(2024, 5, 20, 9, 8, 32, 775, DateTimeKind.Local).AddTicks(1137) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "95ed2407-3e58-4af2-88a4-1c4e96473f68",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2024, 5, 20, 9, 8, 32, 775, DateTimeKind.Local).AddTicks(1194), new DateTime(2024, 5, 20, 9, 8, 32, 775, DateTimeKind.Local).AddTicks(1196) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "97942bee-ef12-4425-8225-4f293d0f36dd",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2024, 5, 20, 9, 8, 32, 775, DateTimeKind.Local).AddTicks(1432), new DateTime(2024, 5, 20, 9, 8, 32, 775, DateTimeKind.Local).AddTicks(1433) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "cd12b44b-103b-48df-8887-a2bf42e0651e",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2024, 5, 20, 9, 8, 32, 775, DateTimeKind.Local).AddTicks(1406), new DateTime(2024, 5, 20, 9, 8, 32, 775, DateTimeKind.Local).AddTicks(1408) });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "7e67d486-af3e-49f1-a109-a2b864b8e0ec",
                columns: new[] { "ConcurrencyStamp", "Created", "Modified", "PasswordHash", "SecurityStamp" },
                values: new object[] { "27b0a525-205f-4999-b675-c577bb162518", new DateTime(2024, 5, 20, 9, 8, 32, 775, DateTimeKind.Local).AddTicks(1538), new DateTime(2024, 5, 20, 9, 8, 32, 775, DateTimeKind.Local).AddTicks(1539), "AQAAAAIAAYagAAAAEPJVOgYV3MO/diQzIb3C1t07IrJTzaL3dvAzyD9fojxm5/jEI+hUj0dPK2Q6CE8ruw==", "cd2506e5-d244-4a6b-94f3-4e540ed40cb3" });
        }
    }
}
