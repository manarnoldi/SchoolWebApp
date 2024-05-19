using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SchoolWebApp.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddedNamefieldtoEntityExam : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "Exams",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "269f0cf3-405e-4163-83f3-1b63ebebd62e",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2024, 5, 19, 11, 2, 13, 288, DateTimeKind.Local).AddTicks(2973), new DateTime(2024, 5, 19, 11, 2, 13, 288, DateTimeKind.Local).AddTicks(2983) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "448df289-142c-4959-a912-60733515e1b4",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2024, 5, 19, 11, 2, 13, 288, DateTimeKind.Local).AddTicks(2943), new DateTime(2024, 5, 19, 11, 2, 13, 288, DateTimeKind.Local).AddTicks(2944) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "48c50c3a-9958-453b-b649-4e21af131322",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2024, 5, 19, 11, 2, 13, 288, DateTimeKind.Local).AddTicks(2914), new DateTime(2024, 5, 19, 11, 2, 13, 288, DateTimeKind.Local).AddTicks(2915) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "717d9b15-a428-440c-b26b-08d3bbb68b02",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2024, 5, 19, 11, 2, 13, 288, DateTimeKind.Local).AddTicks(2737), new DateTime(2024, 5, 19, 11, 2, 13, 288, DateTimeKind.Local).AddTicks(2824) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "95ed2407-3e58-4af2-88a4-1c4e96473f68",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2024, 5, 19, 11, 2, 13, 288, DateTimeKind.Local).AddTicks(2883), new DateTime(2024, 5, 19, 11, 2, 13, 288, DateTimeKind.Local).AddTicks(2885) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "97942bee-ef12-4425-8225-4f293d0f36dd",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2024, 5, 19, 11, 2, 13, 288, DateTimeKind.Local).AddTicks(3058), new DateTime(2024, 5, 19, 11, 2, 13, 288, DateTimeKind.Local).AddTicks(3060) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "cd12b44b-103b-48df-8887-a2bf42e0651e",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2024, 5, 19, 11, 2, 13, 288, DateTimeKind.Local).AddTicks(3030), new DateTime(2024, 5, 19, 11, 2, 13, 288, DateTimeKind.Local).AddTicks(3032) });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "7e67d486-af3e-49f1-a109-a2b864b8e0ec",
                columns: new[] { "ConcurrencyStamp", "Created", "Modified", "PasswordHash", "SecurityStamp" },
                values: new object[] { "cb2652ce-de12-49f7-980b-f463cbd40df9", new DateTime(2024, 5, 19, 11, 2, 13, 288, DateTimeKind.Local).AddTicks(3201), new DateTime(2024, 5, 19, 11, 2, 13, 288, DateTimeKind.Local).AddTicks(3203), "AQAAAAIAAYagAAAAEMj090HYM2nbfrPNCtAeKlsSK6eVuYXaTuqg806/kVqpi7w9HEKcy67ykh7sH0X1Aw==", "8502935a-c92d-4e06-a49b-3c19b142b113" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Name",
                table: "Exams");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "269f0cf3-405e-4163-83f3-1b63ebebd62e",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2024, 5, 18, 11, 14, 22, 949, DateTimeKind.Local).AddTicks(9518), new DateTime(2024, 5, 18, 11, 14, 22, 949, DateTimeKind.Local).AddTicks(9520) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "448df289-142c-4959-a912-60733515e1b4",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2024, 5, 18, 11, 14, 22, 949, DateTimeKind.Local).AddTicks(9472), new DateTime(2024, 5, 18, 11, 14, 22, 949, DateTimeKind.Local).AddTicks(9473) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "48c50c3a-9958-453b-b649-4e21af131322",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2024, 5, 18, 11, 14, 22, 949, DateTimeKind.Local).AddTicks(9443), new DateTime(2024, 5, 18, 11, 14, 22, 949, DateTimeKind.Local).AddTicks(9444) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "717d9b15-a428-440c-b26b-08d3bbb68b02",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2024, 5, 18, 11, 14, 22, 949, DateTimeKind.Local).AddTicks(9317), new DateTime(2024, 5, 18, 11, 14, 22, 949, DateTimeKind.Local).AddTicks(9343) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "95ed2407-3e58-4af2-88a4-1c4e96473f68",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2024, 5, 18, 11, 14, 22, 949, DateTimeKind.Local).AddTicks(9408), new DateTime(2024, 5, 18, 11, 14, 22, 949, DateTimeKind.Local).AddTicks(9415) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "97942bee-ef12-4425-8225-4f293d0f36dd",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2024, 5, 18, 11, 14, 22, 949, DateTimeKind.Local).AddTicks(9599), new DateTime(2024, 5, 18, 11, 14, 22, 949, DateTimeKind.Local).AddTicks(9600) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "cd12b44b-103b-48df-8887-a2bf42e0651e",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2024, 5, 18, 11, 14, 22, 949, DateTimeKind.Local).AddTicks(9551), new DateTime(2024, 5, 18, 11, 14, 22, 949, DateTimeKind.Local).AddTicks(9566) });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "7e67d486-af3e-49f1-a109-a2b864b8e0ec",
                columns: new[] { "ConcurrencyStamp", "Created", "Modified", "PasswordHash", "SecurityStamp" },
                values: new object[] { "1bdd77dc-f601-4c1e-9079-a9b6f515dfff", new DateTime(2024, 5, 18, 11, 14, 22, 949, DateTimeKind.Local).AddTicks(9688), new DateTime(2024, 5, 18, 11, 14, 22, 949, DateTimeKind.Local).AddTicks(9691), "AQAAAAIAAYagAAAAEMFtRLhwL6G2G7eoOCbYXnVQJdug/mnn6iXoFeDOBj0VyGZJgv5GuL0jlMZIEnCJ8Q==", "d750ade6-45e1-46f2-a801-25cc542083aa" });
        }
    }
}
