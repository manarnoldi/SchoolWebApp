using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SchoolWebApp.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddedExamNames : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            //migrationBuilder.DropForeignKey(
            //    name: "FK_Exams_ExamTypes_ExamTypeId",
            //    table: "Exams");

            //migrationBuilder.RenameColumn(
            //    name: "ExamTypeId",
            //    table: "Exams",
            //    newName: "ExamNameId");

            //migrationBuilder.RenameIndex(
            //    name: "IX_Exams_ExamTypeId",
            //    table: "Exams",
            //    newName: "IX_Exams_ExamNameId");

            //migrationBuilder.CreateTable(
            //    name: "ExamNames",
            //    columns: table => new
            //    {
            //        Id = table.Column<int>(type: "int", nullable: false)
            //            .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
            //        ExamtypeId = table.Column<int>(type: "int", nullable: false),
            //        Created = table.Column<DateTime>(type: "datetime(6)", nullable: true),
            //        CreatedBy = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true)
            //            .Annotation("MySql:CharSet", "utf8mb4"),
            //        Modified = table.Column<DateTime>(type: "datetime(6)", nullable: true),
            //        ModifiedBy = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true)
            //            .Annotation("MySql:CharSet", "utf8mb4"),
            //        Name = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: false)
            //            .Annotation("MySql:CharSet", "utf8mb4"),
            //        Description = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true)
            //            .Annotation("MySql:CharSet", "utf8mb4"),
            //        Rank = table.Column<int>(type: "int", nullable: false)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_ExamNames", x => x.Id);
            //        table.ForeignKey(
            //            name: "FK_ExamNames_ExamTypes_ExamtypeId",
            //            column: x => x.ExamtypeId,
            //            principalTable: "ExamTypes",
            //            principalColumn: "Id");
            //    })
            //    .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "269f0cf3-405e-4163-83f3-1b63ebebd62e",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2025, 7, 6, 7, 49, 2, 801, DateTimeKind.Local).AddTicks(7241), new DateTime(2025, 7, 6, 7, 49, 2, 801, DateTimeKind.Local).AddTicks(7242) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "448df289-142c-4959-a912-60733515e1b4",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2025, 7, 6, 7, 49, 2, 801, DateTimeKind.Local).AddTicks(7191), new DateTime(2025, 7, 6, 7, 49, 2, 801, DateTimeKind.Local).AddTicks(7192) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "48c50c3a-9958-453b-b649-4e21af131322",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2025, 7, 6, 7, 49, 2, 801, DateTimeKind.Local).AddTicks(7154), new DateTime(2025, 7, 6, 7, 49, 2, 801, DateTimeKind.Local).AddTicks(7155) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "717d9b15-a428-440c-b26b-08d3bbb68b02",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2025, 7, 6, 7, 49, 2, 801, DateTimeKind.Local).AddTicks(6980), new DateTime(2025, 7, 6, 7, 49, 2, 801, DateTimeKind.Local).AddTicks(7005) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "95ed2407-3e58-4af2-88a4-1c4e96473f68",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2025, 7, 6, 7, 49, 2, 801, DateTimeKind.Local).AddTicks(7106), new DateTime(2025, 7, 6, 7, 49, 2, 801, DateTimeKind.Local).AddTicks(7113) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "97942bee-ef12-4425-8225-4f293d0f36dd",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2025, 7, 6, 7, 49, 2, 801, DateTimeKind.Local).AddTicks(7340), new DateTime(2025, 7, 6, 7, 49, 2, 801, DateTimeKind.Local).AddTicks(7342) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "cd12b44b-103b-48df-8887-a2bf42e0651e",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2025, 7, 6, 7, 49, 2, 801, DateTimeKind.Local).AddTicks(7280), new DateTime(2025, 7, 6, 7, 49, 2, 801, DateTimeKind.Local).AddTicks(7294) });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "7e67d486-af3e-49f1-a109-a2b864b8e0ec",
                columns: new[] { "ConcurrencyStamp", "Created", "Modified", "PasswordHash", "SecurityStamp" },
                values: new object[] { "4820e21a-2c2b-4ea1-8c1c-d5be57a75bee", new DateTime(2025, 7, 6, 7, 49, 2, 801, DateTimeKind.Local).AddTicks(7453), new DateTime(2025, 7, 6, 7, 49, 2, 801, DateTimeKind.Local).AddTicks(7456), "AQAAAAIAAYagAAAAEAPIkZpIWefgZf7+WDBLQ+pKLTgM+DdCTXqFJEND84ifCGON6c8ohAAFAiIgVsoIdQ==", "221061b1-7e68-4072-9a1a-5c0540174d16" });

            //migrationBuilder.CreateIndex(
            //    name: "IX_ExamNames_ExamtypeId",
            //    table: "ExamNames",
            //    column: "ExamtypeId");

            //migrationBuilder.AddForeignKey(
            //    name: "FK_Exams_ExamNames_ExamNameId",
            //    table: "Exams",
            //    column: "ExamNameId",
            //    principalTable: "ExamNames",
            //    principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            //migrationBuilder.DropForeignKey(
            //    name: "FK_Exams_ExamNames_ExamNameId",
            //    table: "Exams");

            //migrationBuilder.DropTable(
            //    name: "ExamNames");

            //migrationBuilder.RenameColumn(
            //    name: "ExamNameId",
            //    table: "Exams",
            //    newName: "ExamTypeId");

            //migrationBuilder.RenameIndex(
            //    name: "IX_Exams_ExamNameId",
            //    table: "Exams",
            //    newName: "IX_Exams_ExamTypeId");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "269f0cf3-405e-4163-83f3-1b63ebebd62e",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2025, 6, 22, 7, 14, 37, 394, DateTimeKind.Local).AddTicks(3140), new DateTime(2025, 6, 22, 7, 14, 37, 394, DateTimeKind.Local).AddTicks(3162) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "448df289-142c-4959-a912-60733515e1b4",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2025, 6, 22, 7, 14, 37, 394, DateTimeKind.Local).AddTicks(3099), new DateTime(2025, 6, 22, 7, 14, 37, 394, DateTimeKind.Local).AddTicks(3100) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "48c50c3a-9958-453b-b649-4e21af131322",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2025, 6, 22, 7, 14, 37, 394, DateTimeKind.Local).AddTicks(3070), new DateTime(2025, 6, 22, 7, 14, 37, 394, DateTimeKind.Local).AddTicks(3071) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "717d9b15-a428-440c-b26b-08d3bbb68b02",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2025, 6, 22, 7, 14, 37, 394, DateTimeKind.Local).AddTicks(2943), new DateTime(2025, 6, 22, 7, 14, 37, 394, DateTimeKind.Local).AddTicks(2975) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "95ed2407-3e58-4af2-88a4-1c4e96473f68",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2025, 6, 22, 7, 14, 37, 394, DateTimeKind.Local).AddTicks(3034), new DateTime(2025, 6, 22, 7, 14, 37, 394, DateTimeKind.Local).AddTicks(3035) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "97942bee-ef12-4425-8225-4f293d0f36dd",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2025, 6, 22, 7, 14, 37, 394, DateTimeKind.Local).AddTicks(3238), new DateTime(2025, 6, 22, 7, 14, 37, 394, DateTimeKind.Local).AddTicks(3240) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "cd12b44b-103b-48df-8887-a2bf42e0651e",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2025, 6, 22, 7, 14, 37, 394, DateTimeKind.Local).AddTicks(3200), new DateTime(2025, 6, 22, 7, 14, 37, 394, DateTimeKind.Local).AddTicks(3202) });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "7e67d486-af3e-49f1-a109-a2b864b8e0ec",
                columns: new[] { "ConcurrencyStamp", "Created", "Modified", "PasswordHash", "SecurityStamp" },
                values: new object[] { "fbac1684-8dd8-4261-bbee-dc528ebd6797", new DateTime(2025, 6, 22, 7, 14, 37, 394, DateTimeKind.Local).AddTicks(3328), new DateTime(2025, 6, 22, 7, 14, 37, 394, DateTimeKind.Local).AddTicks(3330), "AQAAAAIAAYagAAAAEKbQxjZy369ZsnuYxB6VP4F0pZZ6cwEoSRfqHsoY0oD80E4BliEokwjkyWqleoc4ow==", "76544aed-781d-4334-bdb1-0b1ca7f0c6c1" });

            //migrationBuilder.AddForeignKey(
            //    name: "FK_Exams_ExamTypes_ExamTypeId",
            //    table: "Exams",
            //    column: "ExamTypeId",
            //    principalTable: "ExamTypes",
            //    principalColumn: "Id");
        }
    }
}
