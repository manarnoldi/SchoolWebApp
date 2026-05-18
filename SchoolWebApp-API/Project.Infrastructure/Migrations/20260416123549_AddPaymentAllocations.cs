using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SchoolWebApp.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddPaymentAllocations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PaymentAllocations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    PaymentId = table.Column<int>(type: "int", nullable: false),
                    StudentInvoiceItemId = table.Column<int>(type: "int", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(65,30)", nullable: false),
                    Created = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    CreatedBy = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Modified = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    ModifiedBy = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PaymentAllocations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PaymentAllocations_Payments_PaymentId",
                        column: x => x.PaymentId,
                        principalTable: "Payments",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_PaymentAllocations_StudentInvoiceItems_StudentInvoiceItemId",
                        column: x => x.StudentInvoiceItemId,
                        principalTable: "StudentInvoiceItems",
                        principalColumn: "Id");
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 16, 15, 35, 45, 776, DateTimeKind.Local).AddTicks(7763), new DateTime(2026, 4, 16, 15, 35, 45, 776, DateTimeKind.Local).AddTicks(7785) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 16, 15, 35, 45, 776, DateTimeKind.Local).AddTicks(7884), new DateTime(2026, 4, 16, 15, 35, 45, 776, DateTimeKind.Local).AddTicks(7886) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 16, 15, 35, 45, 776, DateTimeKind.Local).AddTicks(7931), new DateTime(2026, 4, 16, 15, 35, 45, 776, DateTimeKind.Local).AddTicks(7933) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 16, 15, 35, 45, 776, DateTimeKind.Local).AddTicks(8149), new DateTime(2026, 4, 16, 15, 35, 45, 776, DateTimeKind.Local).AddTicks(8152) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 16, 15, 35, 45, 776, DateTimeKind.Local).AddTicks(8213), new DateTime(2026, 4, 16, 15, 35, 45, 776, DateTimeKind.Local).AddTicks(8215) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 6,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 16, 15, 35, 45, 776, DateTimeKind.Local).AddTicks(8268), new DateTime(2026, 4, 16, 15, 35, 45, 776, DateTimeKind.Local).AddTicks(8269) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 7,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 16, 15, 35, 45, 776, DateTimeKind.Local).AddTicks(8316), new DateTime(2026, 4, 16, 15, 35, 45, 776, DateTimeKind.Local).AddTicks(8318) });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "ConcurrencyStamp", "Created", "Modified", "PasswordHash" },
                values: new object[] { "31532897-e92f-4a85-9e06-4b78171afa5a", new DateTime(2026, 4, 16, 15, 35, 45, 776, DateTimeKind.Local).AddTicks(8871), new DateTime(2026, 4, 16, 15, 35, 45, 776, DateTimeKind.Local).AddTicks(8874), "AQAAAAIAAYagAAAAEOQSASv42cL0npw440/lLTMKrA8rdaLzk5yP+VZu1jeHE9+xHLGcKLchduGwzmRoEA==" });

            migrationBuilder.UpdateData(
                table: "Designations",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 16, 15, 35, 45, 776, DateTimeKind.Local).AddTicks(8417), new DateTime(2026, 4, 16, 15, 35, 45, 776, DateTimeKind.Local).AddTicks(8419) });

            migrationBuilder.UpdateData(
                table: "EmploymentTypes",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 16, 15, 35, 45, 776, DateTimeKind.Local).AddTicks(8462), new DateTime(2026, 4, 16, 15, 35, 45, 776, DateTimeKind.Local).AddTicks(8464) });

            migrationBuilder.UpdateData(
                table: "Genders",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 16, 15, 35, 45, 776, DateTimeKind.Local).AddTicks(8586), new DateTime(2026, 4, 16, 15, 35, 45, 776, DateTimeKind.Local).AddTicks(8587) });

            migrationBuilder.UpdateData(
                table: "Nationalities",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 16, 15, 35, 45, 776, DateTimeKind.Local).AddTicks(8502), new DateTime(2026, 4, 16, 15, 35, 45, 776, DateTimeKind.Local).AddTicks(8504) });

            migrationBuilder.UpdateData(
                table: "Person",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "DateOfBirth", "EmploymentDate", "EndofEmploymentDate", "Modified" },
                values: new object[] { new DateTime(2026, 4, 16, 15, 35, 45, 776, DateTimeKind.Local).AddTicks(8667), new DateTime(2026, 4, 16, 15, 35, 45, 776, DateTimeKind.Local).AddTicks(8664), new DateTime(2026, 4, 16, 15, 35, 45, 776, DateTimeKind.Local).AddTicks(8659), new DateTime(2026, 4, 16, 15, 35, 45, 776, DateTimeKind.Local).AddTicks(8661), new DateTime(2026, 4, 16, 15, 35, 45, 776, DateTimeKind.Local).AddTicks(8676) });

            migrationBuilder.UpdateData(
                table: "Religions",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 16, 15, 35, 45, 776, DateTimeKind.Local).AddTicks(8544), new DateTime(2026, 4, 16, 15, 35, 45, 776, DateTimeKind.Local).AddTicks(8546) });

            migrationBuilder.UpdateData(
                table: "StaffCategories",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 16, 15, 35, 45, 776, DateTimeKind.Local).AddTicks(8377), new DateTime(2026, 4, 16, 15, 35, 45, 776, DateTimeKind.Local).AddTicks(8380) });

            migrationBuilder.CreateIndex(
                name: "IX_PaymentAllocations_PaymentId",
                table: "PaymentAllocations",
                column: "PaymentId");

            migrationBuilder.CreateIndex(
                name: "IX_PaymentAllocations_StudentInvoiceItemId",
                table: "PaymentAllocations",
                column: "StudentInvoiceItemId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PaymentAllocations");

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
        }
    }
}
