using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SchoolWebApp.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddPaymentTypeAndCreditDebitNotes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "OriginalPaymentId",
                table: "Payments",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PaymentType",
                table: "Payments",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Reason",
                table: "Payments",
                type: "varchar(500)",
                maxLength: 500,
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 16, 15, 22, 42, 704, DateTimeKind.Local).AddTicks(6179), new DateTime(2026, 4, 16, 15, 22, 42, 704, DateTimeKind.Local).AddTicks(6206) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 16, 15, 22, 42, 704, DateTimeKind.Local).AddTicks(6367), new DateTime(2026, 4, 16, 15, 22, 42, 704, DateTimeKind.Local).AddTicks(6371) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 16, 15, 22, 42, 704, DateTimeKind.Local).AddTicks(6453), new DateTime(2026, 4, 16, 15, 22, 42, 704, DateTimeKind.Local).AddTicks(6456) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 16, 15, 22, 42, 704, DateTimeKind.Local).AddTicks(6529), new DateTime(2026, 4, 16, 15, 22, 42, 704, DateTimeKind.Local).AddTicks(6532) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 16, 15, 22, 42, 704, DateTimeKind.Local).AddTicks(6603), new DateTime(2026, 4, 16, 15, 22, 42, 704, DateTimeKind.Local).AddTicks(6606) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 6,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 16, 15, 22, 42, 704, DateTimeKind.Local).AddTicks(6690), new DateTime(2026, 4, 16, 15, 22, 42, 704, DateTimeKind.Local).AddTicks(6694) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 7,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 16, 15, 22, 42, 704, DateTimeKind.Local).AddTicks(6777), new DateTime(2026, 4, 16, 15, 22, 42, 704, DateTimeKind.Local).AddTicks(6780) });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "ConcurrencyStamp", "Created", "Modified", "PasswordHash" },
                values: new object[] { "636eabdc-7981-4ea6-a104-653741d6560e", new DateTime(2026, 4, 16, 15, 22, 42, 704, DateTimeKind.Local).AddTicks(7573), new DateTime(2026, 4, 16, 15, 22, 42, 704, DateTimeKind.Local).AddTicks(7577), "AQAAAAIAAYagAAAAEBcnE8SdK7uJ/vJa0ew0C5hPfnfSFRKnYwkumdA6q+ZwwgXPGUkKOISmyf4Nooxzfw==" });

            migrationBuilder.UpdateData(
                table: "Designations",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 16, 15, 22, 42, 704, DateTimeKind.Local).AddTicks(6947), new DateTime(2026, 4, 16, 15, 22, 42, 704, DateTimeKind.Local).AddTicks(6950) });

            migrationBuilder.UpdateData(
                table: "EmploymentTypes",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 16, 15, 22, 42, 704, DateTimeKind.Local).AddTicks(7023), new DateTime(2026, 4, 16, 15, 22, 42, 704, DateTimeKind.Local).AddTicks(7026) });

            migrationBuilder.UpdateData(
                table: "Genders",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 16, 15, 22, 42, 704, DateTimeKind.Local).AddTicks(7229), new DateTime(2026, 4, 16, 15, 22, 42, 704, DateTimeKind.Local).AddTicks(7232) });

            migrationBuilder.UpdateData(
                table: "Nationalities",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 16, 15, 22, 42, 704, DateTimeKind.Local).AddTicks(7088), new DateTime(2026, 4, 16, 15, 22, 42, 704, DateTimeKind.Local).AddTicks(7091) });

            migrationBuilder.UpdateData(
                table: "Person",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "DateOfBirth", "EmploymentDate", "EndofEmploymentDate", "Modified" },
                values: new object[] { new DateTime(2026, 4, 16, 15, 22, 42, 704, DateTimeKind.Local).AddTicks(7335), new DateTime(2026, 4, 16, 15, 22, 42, 704, DateTimeKind.Local).AddTicks(7330), new DateTime(2026, 4, 16, 15, 22, 42, 704, DateTimeKind.Local).AddTicks(7322), new DateTime(2026, 4, 16, 15, 22, 42, 704, DateTimeKind.Local).AddTicks(7325), new DateTime(2026, 4, 16, 15, 22, 42, 704, DateTimeKind.Local).AddTicks(7354) });

            migrationBuilder.UpdateData(
                table: "Religions",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 16, 15, 22, 42, 704, DateTimeKind.Local).AddTicks(7159), new DateTime(2026, 4, 16, 15, 22, 42, 704, DateTimeKind.Local).AddTicks(7163) });

            migrationBuilder.UpdateData(
                table: "StaffCategories",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 16, 15, 22, 42, 704, DateTimeKind.Local).AddTicks(6865), new DateTime(2026, 4, 16, 15, 22, 42, 704, DateTimeKind.Local).AddTicks(6869) });

            migrationBuilder.CreateIndex(
                name: "IX_Payments_OriginalPaymentId",
                table: "Payments",
                column: "OriginalPaymentId");

            migrationBuilder.AddForeignKey(
                name: "FK_Payments_Payments_OriginalPaymentId",
                table: "Payments",
                column: "OriginalPaymentId",
                principalTable: "Payments",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Payments_Payments_OriginalPaymentId",
                table: "Payments");

            migrationBuilder.DropIndex(
                name: "IX_Payments_OriginalPaymentId",
                table: "Payments");

            migrationBuilder.DropColumn(
                name: "OriginalPaymentId",
                table: "Payments");

            migrationBuilder.DropColumn(
                name: "PaymentType",
                table: "Payments");

            migrationBuilder.DropColumn(
                name: "Reason",
                table: "Payments");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 16, 14, 56, 36, 180, DateTimeKind.Local).AddTicks(633), new DateTime(2026, 4, 16, 14, 56, 36, 180, DateTimeKind.Local).AddTicks(652) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 16, 14, 56, 36, 180, DateTimeKind.Local).AddTicks(692), new DateTime(2026, 4, 16, 14, 56, 36, 180, DateTimeKind.Local).AddTicks(693) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 16, 14, 56, 36, 180, DateTimeKind.Local).AddTicks(709), new DateTime(2026, 4, 16, 14, 56, 36, 180, DateTimeKind.Local).AddTicks(709) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 16, 14, 56, 36, 180, DateTimeKind.Local).AddTicks(725), new DateTime(2026, 4, 16, 14, 56, 36, 180, DateTimeKind.Local).AddTicks(726) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 16, 14, 56, 36, 180, DateTimeKind.Local).AddTicks(741), new DateTime(2026, 4, 16, 14, 56, 36, 180, DateTimeKind.Local).AddTicks(742) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 6,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 16, 14, 56, 36, 180, DateTimeKind.Local).AddTicks(841), new DateTime(2026, 4, 16, 14, 56, 36, 180, DateTimeKind.Local).AddTicks(841) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 7,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 16, 14, 56, 36, 180, DateTimeKind.Local).AddTicks(869), new DateTime(2026, 4, 16, 14, 56, 36, 180, DateTimeKind.Local).AddTicks(870) });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "ConcurrencyStamp", "Created", "Modified", "PasswordHash" },
                values: new object[] { "524b50a9-1557-4f38-83ca-b994471dff03", new DateTime(2026, 4, 16, 14, 56, 36, 180, DateTimeKind.Local).AddTicks(1192), new DateTime(2026, 4, 16, 14, 56, 36, 180, DateTimeKind.Local).AddTicks(1194), "AQAAAAIAAYagAAAAEGWfuAjnxucFZXqmvku3bdfZgNctUQWPVNzV3PCi6LTADp4ftO1gaXmEglJHZgMhrA==" });

            migrationBuilder.UpdateData(
                table: "Designations",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 16, 14, 56, 36, 180, DateTimeKind.Local).AddTicks(946), new DateTime(2026, 4, 16, 14, 56, 36, 180, DateTimeKind.Local).AddTicks(947) });

            migrationBuilder.UpdateData(
                table: "EmploymentTypes",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 16, 14, 56, 36, 180, DateTimeKind.Local).AddTicks(972), new DateTime(2026, 4, 16, 14, 56, 36, 180, DateTimeKind.Local).AddTicks(973) });

            migrationBuilder.UpdateData(
                table: "Genders",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 16, 14, 56, 36, 180, DateTimeKind.Local).AddTicks(1029), new DateTime(2026, 4, 16, 14, 56, 36, 180, DateTimeKind.Local).AddTicks(1030) });

            migrationBuilder.UpdateData(
                table: "Nationalities",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 16, 14, 56, 36, 180, DateTimeKind.Local).AddTicks(992), new DateTime(2026, 4, 16, 14, 56, 36, 180, DateTimeKind.Local).AddTicks(993) });

            migrationBuilder.UpdateData(
                table: "Person",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "DateOfBirth", "EmploymentDate", "EndofEmploymentDate", "Modified" },
                values: new object[] { new DateTime(2026, 4, 16, 14, 56, 36, 180, DateTimeKind.Local).AddTicks(1068), new DateTime(2026, 4, 16, 14, 56, 36, 180, DateTimeKind.Local).AddTicks(1064), new DateTime(2026, 4, 16, 14, 56, 36, 180, DateTimeKind.Local).AddTicks(1061), new DateTime(2026, 4, 16, 14, 56, 36, 180, DateTimeKind.Local).AddTicks(1062), new DateTime(2026, 4, 16, 14, 56, 36, 180, DateTimeKind.Local).AddTicks(1076) });

            migrationBuilder.UpdateData(
                table: "Religions",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 16, 14, 56, 36, 180, DateTimeKind.Local).AddTicks(1012), new DateTime(2026, 4, 16, 14, 56, 36, 180, DateTimeKind.Local).AddTicks(1013) });

            migrationBuilder.UpdateData(
                table: "StaffCategories",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 16, 14, 56, 36, 180, DateTimeKind.Local).AddTicks(906), new DateTime(2026, 4, 16, 14, 56, 36, 180, DateTimeKind.Local).AddTicks(908) });
        }
    }
}
