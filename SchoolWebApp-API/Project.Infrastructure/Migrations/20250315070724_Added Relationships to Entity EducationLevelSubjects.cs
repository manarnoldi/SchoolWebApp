using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SchoolWebApp.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddedRelationshipstoEntityEducationLevelSubjects : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "269f0cf3-405e-4163-83f3-1b63ebebd62e",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2025, 3, 15, 10, 7, 22, 76, DateTimeKind.Local).AddTicks(6848), new DateTime(2025, 3, 15, 10, 7, 22, 76, DateTimeKind.Local).AddTicks(6866) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "448df289-142c-4959-a912-60733515e1b4",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2025, 3, 15, 10, 7, 22, 76, DateTimeKind.Local).AddTicks(6767), new DateTime(2025, 3, 15, 10, 7, 22, 76, DateTimeKind.Local).AddTicks(6769) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "48c50c3a-9958-453b-b649-4e21af131322",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2025, 3, 15, 10, 7, 22, 76, DateTimeKind.Local).AddTicks(6705), new DateTime(2025, 3, 15, 10, 7, 22, 76, DateTimeKind.Local).AddTicks(6707) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "717d9b15-a428-440c-b26b-08d3bbb68b02",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2025, 3, 15, 10, 7, 22, 76, DateTimeKind.Local).AddTicks(6413), new DateTime(2025, 3, 15, 10, 7, 22, 76, DateTimeKind.Local).AddTicks(6464) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "95ed2407-3e58-4af2-88a4-1c4e96473f68",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2025, 3, 15, 10, 7, 22, 76, DateTimeKind.Local).AddTicks(6635), new DateTime(2025, 3, 15, 10, 7, 22, 76, DateTimeKind.Local).AddTicks(6641) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "97942bee-ef12-4425-8225-4f293d0f36dd",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2025, 3, 15, 10, 7, 22, 76, DateTimeKind.Local).AddTicks(7019), new DateTime(2025, 3, 15, 10, 7, 22, 76, DateTimeKind.Local).AddTicks(7021) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "cd12b44b-103b-48df-8887-a2bf42e0651e",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2025, 3, 15, 10, 7, 22, 76, DateTimeKind.Local).AddTicks(6961), new DateTime(2025, 3, 15, 10, 7, 22, 76, DateTimeKind.Local).AddTicks(6964) });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "7e67d486-af3e-49f1-a109-a2b864b8e0ec",
                columns: new[] { "ConcurrencyStamp", "Created", "Modified", "PasswordHash", "SecurityStamp" },
                values: new object[] { "8cc8e532-fd2b-481b-b772-626cb10c15ee", new DateTime(2025, 3, 15, 10, 7, 22, 76, DateTimeKind.Local).AddTicks(7167), new DateTime(2025, 3, 15, 10, 7, 22, 76, DateTimeKind.Local).AddTicks(7171), "AQAAAAIAAYagAAAAEHxJCxghrpIda9OXBfPmBifWrwlJXOms2FRS+tNsndqBph+cWrIFzG6eqtec9UyGbA==", "b32bfa35-81c4-4a6a-aa06-5d51befe138c" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "269f0cf3-405e-4163-83f3-1b63ebebd62e",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2025, 3, 15, 10, 3, 8, 215, DateTimeKind.Local).AddTicks(6504), new DateTime(2025, 3, 15, 10, 3, 8, 215, DateTimeKind.Local).AddTicks(6505) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "448df289-142c-4959-a912-60733515e1b4",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2025, 3, 15, 10, 3, 8, 215, DateTimeKind.Local).AddTicks(6477), new DateTime(2025, 3, 15, 10, 3, 8, 215, DateTimeKind.Local).AddTicks(6479) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "48c50c3a-9958-453b-b649-4e21af131322",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2025, 3, 15, 10, 3, 8, 215, DateTimeKind.Local).AddTicks(6451), new DateTime(2025, 3, 15, 10, 3, 8, 215, DateTimeKind.Local).AddTicks(6452) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "717d9b15-a428-440c-b26b-08d3bbb68b02",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2025, 3, 15, 10, 3, 8, 215, DateTimeKind.Local).AddTicks(6299), new DateTime(2025, 3, 15, 10, 3, 8, 215, DateTimeKind.Local).AddTicks(6334) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "95ed2407-3e58-4af2-88a4-1c4e96473f68",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2025, 3, 15, 10, 3, 8, 215, DateTimeKind.Local).AddTicks(6412), new DateTime(2025, 3, 15, 10, 3, 8, 215, DateTimeKind.Local).AddTicks(6423) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "97942bee-ef12-4425-8225-4f293d0f36dd",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2025, 3, 15, 10, 3, 8, 215, DateTimeKind.Local).AddTicks(6594), new DateTime(2025, 3, 15, 10, 3, 8, 215, DateTimeKind.Local).AddTicks(6596) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "cd12b44b-103b-48df-8887-a2bf42e0651e",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2025, 3, 15, 10, 3, 8, 215, DateTimeKind.Local).AddTicks(6536), new DateTime(2025, 3, 15, 10, 3, 8, 215, DateTimeKind.Local).AddTicks(6547) });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "7e67d486-af3e-49f1-a109-a2b864b8e0ec",
                columns: new[] { "ConcurrencyStamp", "Created", "Modified", "PasswordHash", "SecurityStamp" },
                values: new object[] { "412c0f9a-92bd-4f9f-8f3a-71643f3b1564", new DateTime(2025, 3, 15, 10, 3, 8, 215, DateTimeKind.Local).AddTicks(6770), new DateTime(2025, 3, 15, 10, 3, 8, 215, DateTimeKind.Local).AddTicks(6772), "AQAAAAIAAYagAAAAECK9c78oA0vWPIukIojad1GNkmdW300fOPc0CgPjWre8HFNjgWhTDhI07WakhlUqzg==", "c896fa23-3919-46f5-95e8-5c549a7b23ad" });
        }
    }
}
