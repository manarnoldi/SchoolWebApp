using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SchoolWebApp.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddedranktoentitiesLearningLevelandSchoolClass : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Rank",
                table: "SchoolClasses",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Rank",
                table: "LearningLevels",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "269f0cf3-405e-4163-83f3-1b63ebebd62e",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2025, 3, 18, 7, 1, 26, 214, DateTimeKind.Local).AddTicks(2751), new DateTime(2025, 3, 18, 7, 1, 26, 214, DateTimeKind.Local).AddTicks(2761) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "448df289-142c-4959-a912-60733515e1b4",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2025, 3, 18, 7, 1, 26, 214, DateTimeKind.Local).AddTicks(2694), new DateTime(2025, 3, 18, 7, 1, 26, 214, DateTimeKind.Local).AddTicks(2695) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "48c50c3a-9958-453b-b649-4e21af131322",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2025, 3, 18, 7, 1, 26, 214, DateTimeKind.Local).AddTicks(2667), new DateTime(2025, 3, 18, 7, 1, 26, 214, DateTimeKind.Local).AddTicks(2669) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "717d9b15-a428-440c-b26b-08d3bbb68b02",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2025, 3, 18, 7, 1, 26, 214, DateTimeKind.Local).AddTicks(2537), new DateTime(2025, 3, 18, 7, 1, 26, 214, DateTimeKind.Local).AddTicks(2573) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "95ed2407-3e58-4af2-88a4-1c4e96473f68",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2025, 3, 18, 7, 1, 26, 214, DateTimeKind.Local).AddTicks(2639), new DateTime(2025, 3, 18, 7, 1, 26, 214, DateTimeKind.Local).AddTicks(2641) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "97942bee-ef12-4425-8225-4f293d0f36dd",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2025, 3, 18, 7, 1, 26, 214, DateTimeKind.Local).AddTicks(2828), new DateTime(2025, 3, 18, 7, 1, 26, 214, DateTimeKind.Local).AddTicks(2828) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "cd12b44b-103b-48df-8887-a2bf42e0651e",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2025, 3, 18, 7, 1, 26, 214, DateTimeKind.Local).AddTicks(2801), new DateTime(2025, 3, 18, 7, 1, 26, 214, DateTimeKind.Local).AddTicks(2802) });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "7e67d486-af3e-49f1-a109-a2b864b8e0ec",
                columns: new[] { "ConcurrencyStamp", "Created", "Modified", "PasswordHash", "SecurityStamp" },
                values: new object[] { "753c9b52-20b5-46da-9a39-6f0932ae6048", new DateTime(2025, 3, 18, 7, 1, 26, 214, DateTimeKind.Local).AddTicks(2927), new DateTime(2025, 3, 18, 7, 1, 26, 214, DateTimeKind.Local).AddTicks(2929), "AQAAAAIAAYagAAAAEDlbl8QRz+0dlNrAhdOyMyTy8CmCKXZliSpnpDdx/a1+BvUD6OyIJnrtUly9MSYdbg==", "333917eb-914a-42c4-a76b-8abbbaab5466" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Rank",
                table: "SchoolClasses");

            migrationBuilder.DropColumn(
                name: "Rank",
                table: "LearningLevels");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "269f0cf3-405e-4163-83f3-1b63ebebd62e",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2025, 3, 16, 15, 43, 3, 497, DateTimeKind.Local).AddTicks(207), new DateTime(2025, 3, 16, 15, 43, 3, 497, DateTimeKind.Local).AddTicks(222) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "448df289-142c-4959-a912-60733515e1b4",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2025, 3, 16, 15, 43, 3, 497, DateTimeKind.Local).AddTicks(138), new DateTime(2025, 3, 16, 15, 43, 3, 497, DateTimeKind.Local).AddTicks(141) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "48c50c3a-9958-453b-b649-4e21af131322",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2025, 3, 16, 15, 43, 3, 497, DateTimeKind.Local).AddTicks(83), new DateTime(2025, 3, 16, 15, 43, 3, 497, DateTimeKind.Local).AddTicks(86) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "717d9b15-a428-440c-b26b-08d3bbb68b02",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2025, 3, 16, 15, 43, 3, 496, DateTimeKind.Local).AddTicks(9800), new DateTime(2025, 3, 16, 15, 43, 3, 496, DateTimeKind.Local).AddTicks(9853) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "95ed2407-3e58-4af2-88a4-1c4e96473f68",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2025, 3, 16, 15, 43, 3, 497, DateTimeKind.Local).AddTicks(21), new DateTime(2025, 3, 16, 15, 43, 3, 497, DateTimeKind.Local).AddTicks(24) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "97942bee-ef12-4425-8225-4f293d0f36dd",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2025, 3, 16, 15, 43, 3, 497, DateTimeKind.Local).AddTicks(356), new DateTime(2025, 3, 16, 15, 43, 3, 497, DateTimeKind.Local).AddTicks(359) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "cd12b44b-103b-48df-8887-a2bf42e0651e",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2025, 3, 16, 15, 43, 3, 497, DateTimeKind.Local).AddTicks(301), new DateTime(2025, 3, 16, 15, 43, 3, 497, DateTimeKind.Local).AddTicks(304) });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "7e67d486-af3e-49f1-a109-a2b864b8e0ec",
                columns: new[] { "ConcurrencyStamp", "Created", "Modified", "PasswordHash", "SecurityStamp" },
                values: new object[] { "30e78ae6-e8ac-459e-911e-c525bc48cea8", new DateTime(2025, 3, 16, 15, 43, 3, 497, DateTimeKind.Local).AddTicks(500), new DateTime(2025, 3, 16, 15, 43, 3, 497, DateTimeKind.Local).AddTicks(504), "AQAAAAIAAYagAAAAEPjc7nGDSxL3l6ysU7SB6qRMljN4n8KVGucKuIn4Ocbuk1gPh2PVHzEpXLf5BZ+FKA==", "6ffd17b8-09ca-4adf-9acd-a577dd4f68c4" });
        }
    }
}
