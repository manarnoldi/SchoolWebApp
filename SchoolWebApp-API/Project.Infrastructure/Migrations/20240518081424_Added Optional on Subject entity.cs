using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SchoolWebApp.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddedOptionalonSubjectentity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Optional",
                table: "Subjects",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Optional",
                table: "Subjects");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "269f0cf3-405e-4163-83f3-1b63ebebd62e",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2024, 5, 18, 11, 0, 46, 251, DateTimeKind.Local).AddTicks(3821), new DateTime(2024, 5, 18, 11, 0, 46, 251, DateTimeKind.Local).AddTicks(3838) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "448df289-142c-4959-a912-60733515e1b4",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2024, 5, 18, 11, 0, 46, 251, DateTimeKind.Local).AddTicks(3796), new DateTime(2024, 5, 18, 11, 0, 46, 251, DateTimeKind.Local).AddTicks(3797) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "48c50c3a-9958-453b-b649-4e21af131322",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2024, 5, 18, 11, 0, 46, 251, DateTimeKind.Local).AddTicks(3746), new DateTime(2024, 5, 18, 11, 0, 46, 251, DateTimeKind.Local).AddTicks(3747) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "717d9b15-a428-440c-b26b-08d3bbb68b02",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2024, 5, 18, 11, 0, 46, 251, DateTimeKind.Local).AddTicks(3633), new DateTime(2024, 5, 18, 11, 0, 46, 251, DateTimeKind.Local).AddTicks(3667) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "95ed2407-3e58-4af2-88a4-1c4e96473f68",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2024, 5, 18, 11, 0, 46, 251, DateTimeKind.Local).AddTicks(3720), new DateTime(2024, 5, 18, 11, 0, 46, 251, DateTimeKind.Local).AddTicks(3721) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "97942bee-ef12-4425-8225-4f293d0f36dd",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2024, 5, 18, 11, 0, 46, 251, DateTimeKind.Local).AddTicks(3898), new DateTime(2024, 5, 18, 11, 0, 46, 251, DateTimeKind.Local).AddTicks(3899) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "cd12b44b-103b-48df-8887-a2bf42e0651e",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2024, 5, 18, 11, 0, 46, 251, DateTimeKind.Local).AddTicks(3873), new DateTime(2024, 5, 18, 11, 0, 46, 251, DateTimeKind.Local).AddTicks(3875) });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "7e67d486-af3e-49f1-a109-a2b864b8e0ec",
                columns: new[] { "ConcurrencyStamp", "Created", "Modified", "PasswordHash", "SecurityStamp" },
                values: new object[] { "e9c1aa1a-bfea-408b-ba72-b99c20aa5fab", new DateTime(2024, 5, 18, 11, 0, 46, 251, DateTimeKind.Local).AddTicks(4044), new DateTime(2024, 5, 18, 11, 0, 46, 251, DateTimeKind.Local).AddTicks(4045), "AQAAAAIAAYagAAAAEL7RFaeYp2aUeM1xI0glimf86T375cosoOkh0FL8VaVaIKdLlVyA6H3cvmrU8OakEw==", "adce4e29-c4ce-4d14-a8a0-9cdd7897796d" });
        }
    }
}
