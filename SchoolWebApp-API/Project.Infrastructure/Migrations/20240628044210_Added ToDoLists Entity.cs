using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SchoolWebApp.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddedToDoListsEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ToDoLists",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    ItemName = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CompleteBy = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    Completed = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    StaffDetailsId = table.Column<int>(type: "int", nullable: false),
                    Created = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    CreatedBy = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Modified = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    ModifiedBy = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ToDoLists", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ToDoLists_Person_StaffDetailsId",
                        column: x => x.StaffDetailsId,
                        principalTable: "Person",
                        principalColumn: "Id");
                })
                .Annotation("MySql:CharSet", "utf8mb4");

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

            migrationBuilder.CreateIndex(
                name: "IX_ToDoLists_StaffDetailsId",
                table: "ToDoLists",
                column: "StaffDetailsId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ToDoLists");

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
    }
}
