using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SchoolWebApp.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Routineupdates202407110810 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SchoolClassLeaders_Person_StaffDetailsId",
                table: "SchoolClassLeaders");

            migrationBuilder.DropForeignKey(
                name: "FK_SchoolClassLeaders_Person_StudentId",
                table: "SchoolClassLeaders");

            migrationBuilder.DropIndex(
                name: "IX_SchoolClassLeaders_StaffDetailsId",
                table: "SchoolClassLeaders");

            migrationBuilder.DropIndex(
                name: "IX_SchoolClassLeaders_StudentId",
                table: "SchoolClassLeaders");

            migrationBuilder.DropColumn(
                name: "StaffDetailsId",
                table: "SchoolClassLeaders");

            migrationBuilder.DropColumn(
                name: "StudentId",
                table: "SchoolClassLeaders");

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "StaffDetailsId",
                table: "SchoolClassLeaders",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "StudentId",
                table: "SchoolClassLeaders",
                type: "int",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "269f0cf3-405e-4163-83f3-1b63ebebd62e",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2024, 7, 11, 8, 4, 20, 188, DateTimeKind.Local).AddTicks(9960), new DateTime(2024, 7, 11, 8, 4, 20, 188, DateTimeKind.Local).AddTicks(9962) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "448df289-142c-4959-a912-60733515e1b4",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2024, 7, 11, 8, 4, 20, 188, DateTimeKind.Local).AddTicks(9922), new DateTime(2024, 7, 11, 8, 4, 20, 188, DateTimeKind.Local).AddTicks(9924) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "48c50c3a-9958-453b-b649-4e21af131322",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2024, 7, 11, 8, 4, 20, 188, DateTimeKind.Local).AddTicks(9880), new DateTime(2024, 7, 11, 8, 4, 20, 188, DateTimeKind.Local).AddTicks(9882) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "717d9b15-a428-440c-b26b-08d3bbb68b02",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2024, 7, 11, 8, 4, 20, 188, DateTimeKind.Local).AddTicks(9616), new DateTime(2024, 7, 11, 8, 4, 20, 188, DateTimeKind.Local).AddTicks(9650) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "95ed2407-3e58-4af2-88a4-1c4e96473f68",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2024, 7, 11, 8, 4, 20, 188, DateTimeKind.Local).AddTicks(9759), new DateTime(2024, 7, 11, 8, 4, 20, 188, DateTimeKind.Local).AddTicks(9768) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "97942bee-ef12-4425-8225-4f293d0f36dd",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2024, 7, 11, 8, 4, 20, 189, DateTimeKind.Local).AddTicks(88), new DateTime(2024, 7, 11, 8, 4, 20, 189, DateTimeKind.Local).AddTicks(91) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "cd12b44b-103b-48df-8887-a2bf42e0651e",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2024, 7, 11, 8, 4, 20, 189, DateTimeKind.Local), new DateTime(2024, 7, 11, 8, 4, 20, 189, DateTimeKind.Local).AddTicks(13) });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "7e67d486-af3e-49f1-a109-a2b864b8e0ec",
                columns: new[] { "ConcurrencyStamp", "Created", "Modified", "PasswordHash", "SecurityStamp" },
                values: new object[] { "eba0cdd4-852b-4bf3-bbdf-0052d2bb2e92", new DateTime(2024, 7, 11, 8, 4, 20, 189, DateTimeKind.Local).AddTicks(222), new DateTime(2024, 7, 11, 8, 4, 20, 189, DateTimeKind.Local).AddTicks(225), "AQAAAAIAAYagAAAAEHjYyPHxzZdFplM09VAfBiyQ1DysUkkDyUTloBUMvbH0qCcwkqpHpn1I7lFKrGGT8Q==", "8514fed1-31b9-4416-8218-385c8537c813" });

            migrationBuilder.CreateIndex(
                name: "IX_SchoolClassLeaders_StaffDetailsId",
                table: "SchoolClassLeaders",
                column: "StaffDetailsId");

            migrationBuilder.CreateIndex(
                name: "IX_SchoolClassLeaders_StudentId",
                table: "SchoolClassLeaders",
                column: "StudentId");

            migrationBuilder.AddForeignKey(
                name: "FK_SchoolClassLeaders_Person_StaffDetailsId",
                table: "SchoolClassLeaders",
                column: "StaffDetailsId",
                principalTable: "Person",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_SchoolClassLeaders_Person_StudentId",
                table: "SchoolClassLeaders",
                column: "StudentId",
                principalTable: "Person",
                principalColumn: "Id");
        }
    }
}
