using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SchoolWebApp.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddedGeneralOutcometoSpecificOutcomeEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "GeneralOutcomeId",
                table: "SpecificOutcomes",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "269f0cf3-405e-4163-83f3-1b63ebebd62e",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2025, 9, 26, 5, 15, 1, 99, DateTimeKind.Local).AddTicks(6954), new DateTime(2025, 9, 26, 5, 15, 1, 99, DateTimeKind.Local).AddTicks(6956) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "448df289-142c-4959-a912-60733515e1b4",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2025, 9, 26, 5, 15, 1, 99, DateTimeKind.Local).AddTicks(6935), new DateTime(2025, 9, 26, 5, 15, 1, 99, DateTimeKind.Local).AddTicks(6936) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "48c50c3a-9958-453b-b649-4e21af131322",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2025, 9, 26, 5, 15, 1, 99, DateTimeKind.Local).AddTicks(6914), new DateTime(2025, 9, 26, 5, 15, 1, 99, DateTimeKind.Local).AddTicks(6916) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "717d9b15-a428-440c-b26b-08d3bbb68b02",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2025, 9, 26, 5, 15, 1, 99, DateTimeKind.Local).AddTicks(6838), new DateTime(2025, 9, 26, 5, 15, 1, 99, DateTimeKind.Local).AddTicks(6852) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "95ed2407-3e58-4af2-88a4-1c4e96473f68",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2025, 9, 26, 5, 15, 1, 99, DateTimeKind.Local).AddTicks(6887), new DateTime(2025, 9, 26, 5, 15, 1, 99, DateTimeKind.Local).AddTicks(6889) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "97942bee-ef12-4425-8225-4f293d0f36dd",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2025, 9, 26, 5, 15, 1, 99, DateTimeKind.Local).AddTicks(7001), new DateTime(2025, 9, 26, 5, 15, 1, 99, DateTimeKind.Local).AddTicks(7002) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "cd12b44b-103b-48df-8887-a2bf42e0651e",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2025, 9, 26, 5, 15, 1, 99, DateTimeKind.Local).AddTicks(6982), new DateTime(2025, 9, 26, 5, 15, 1, 99, DateTimeKind.Local).AddTicks(6984) });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "7e67d486-af3e-49f1-a109-a2b864b8e0ec",
                columns: new[] { "ConcurrencyStamp", "Created", "Modified", "PasswordHash", "SecurityStamp" },
                values: new object[] { "9c3faf87-9e08-4399-9410-8a90f49e45fd", new DateTime(2025, 9, 26, 5, 15, 1, 99, DateTimeKind.Local).AddTicks(7208), new DateTime(2025, 9, 26, 5, 15, 1, 99, DateTimeKind.Local).AddTicks(7209), "AQAAAAIAAYagAAAAEKDO1s9DS5cB+RdBW6NZseiXvzymylYvoB+eZUPAksJwQHfoyvymZyexTpyzj7Yb7Q==", "d61bba51-5ff7-453c-ace0-d362feb5dee8" });

            migrationBuilder.UpdateData(
                table: "Designations",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2025, 9, 26, 5, 15, 1, 99, DateTimeKind.Local).AddTicks(7051), new DateTime(2025, 9, 26, 5, 15, 1, 99, DateTimeKind.Local).AddTicks(7052) });

            migrationBuilder.UpdateData(
                table: "EmploymentTypes",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2025, 9, 26, 5, 15, 1, 99, DateTimeKind.Local).AddTicks(7071), new DateTime(2025, 9, 26, 5, 15, 1, 99, DateTimeKind.Local).AddTicks(7072) });

            migrationBuilder.UpdateData(
                table: "Genders",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2025, 9, 26, 5, 15, 1, 99, DateTimeKind.Local).AddTicks(7124), new DateTime(2025, 9, 26, 5, 15, 1, 99, DateTimeKind.Local).AddTicks(7124) });

            migrationBuilder.UpdateData(
                table: "Nationalities",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2025, 9, 26, 5, 15, 1, 99, DateTimeKind.Local).AddTicks(7090), new DateTime(2025, 9, 26, 5, 15, 1, 99, DateTimeKind.Local).AddTicks(7091) });

            migrationBuilder.UpdateData(
                table: "Person",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "DateOfBirth", "EmploymentDate", "EndofEmploymentDate", "Modified" },
                values: new object[] { new DateTime(2025, 9, 26, 5, 15, 1, 99, DateTimeKind.Local).AddTicks(7150), new DateTime(2025, 9, 26, 5, 15, 1, 99, DateTimeKind.Local).AddTicks(7148), new DateTime(2025, 9, 26, 5, 15, 1, 99, DateTimeKind.Local).AddTicks(7145), new DateTime(2025, 9, 26, 5, 15, 1, 99, DateTimeKind.Local).AddTicks(7145), new DateTime(2025, 9, 26, 5, 15, 1, 99, DateTimeKind.Local).AddTicks(7151) });

            migrationBuilder.UpdateData(
                table: "Religions",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2025, 9, 26, 5, 15, 1, 99, DateTimeKind.Local).AddTicks(7107), new DateTime(2025, 9, 26, 5, 15, 1, 99, DateTimeKind.Local).AddTicks(7108) });

            migrationBuilder.UpdateData(
                table: "StaffCategories",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2025, 9, 26, 5, 15, 1, 99, DateTimeKind.Local).AddTicks(7030), new DateTime(2025, 9, 26, 5, 15, 1, 99, DateTimeKind.Local).AddTicks(7032) });

            migrationBuilder.CreateIndex(
                name: "IX_SpecificOutcomes_GeneralOutcomeId",
                table: "SpecificOutcomes",
                column: "GeneralOutcomeId");

            migrationBuilder.AddForeignKey(
                name: "FK_SpecificOutcomes_GeneralOutcomes_GeneralOutcomeId",
                table: "SpecificOutcomes",
                column: "GeneralOutcomeId",
                principalTable: "GeneralOutcomes",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SpecificOutcomes_GeneralOutcomes_GeneralOutcomeId",
                table: "SpecificOutcomes");

            migrationBuilder.DropIndex(
                name: "IX_SpecificOutcomes_GeneralOutcomeId",
                table: "SpecificOutcomes");

            migrationBuilder.DropColumn(
                name: "GeneralOutcomeId",
                table: "SpecificOutcomes");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "269f0cf3-405e-4163-83f3-1b63ebebd62e",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2025, 9, 25, 17, 27, 0, 864, DateTimeKind.Local).AddTicks(1787), new DateTime(2025, 9, 25, 17, 27, 0, 864, DateTimeKind.Local).AddTicks(1788) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "448df289-142c-4959-a912-60733515e1b4",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2025, 9, 25, 17, 27, 0, 864, DateTimeKind.Local).AddTicks(1750), new DateTime(2025, 9, 25, 17, 27, 0, 864, DateTimeKind.Local).AddTicks(1752) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "48c50c3a-9958-453b-b649-4e21af131322",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2025, 9, 25, 17, 27, 0, 864, DateTimeKind.Local).AddTicks(1731), new DateTime(2025, 9, 25, 17, 27, 0, 864, DateTimeKind.Local).AddTicks(1733) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "717d9b15-a428-440c-b26b-08d3bbb68b02",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2025, 9, 25, 17, 27, 0, 864, DateTimeKind.Local).AddTicks(1659), new DateTime(2025, 9, 25, 17, 27, 0, 864, DateTimeKind.Local).AddTicks(1672) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "95ed2407-3e58-4af2-88a4-1c4e96473f68",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2025, 9, 25, 17, 27, 0, 864, DateTimeKind.Local).AddTicks(1711), new DateTime(2025, 9, 25, 17, 27, 0, 864, DateTimeKind.Local).AddTicks(1712) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "97942bee-ef12-4425-8225-4f293d0f36dd",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2025, 9, 25, 17, 27, 0, 864, DateTimeKind.Local).AddTicks(1824), new DateTime(2025, 9, 25, 17, 27, 0, 864, DateTimeKind.Local).AddTicks(1826) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "cd12b44b-103b-48df-8887-a2bf42e0651e",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2025, 9, 25, 17, 27, 0, 864, DateTimeKind.Local).AddTicks(1807), new DateTime(2025, 9, 25, 17, 27, 0, 864, DateTimeKind.Local).AddTicks(1808) });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "7e67d486-af3e-49f1-a109-a2b864b8e0ec",
                columns: new[] { "ConcurrencyStamp", "Created", "Modified", "PasswordHash", "SecurityStamp" },
                values: new object[] { "4f3a0b61-d15a-4f11-8d73-1ebc886a2537", new DateTime(2025, 9, 25, 17, 27, 0, 864, DateTimeKind.Local).AddTicks(2016), new DateTime(2025, 9, 25, 17, 27, 0, 864, DateTimeKind.Local).AddTicks(2018), "AQAAAAIAAYagAAAAEBaSGwPuo+YAp45z8P/NGSwOgaj8tgpMsezv7pDUK8SyFZdbyaOHVveQRQBGsLnwCQ==", "fa010c75-d5b7-40c7-a8a1-52218c4e12f6" });

            migrationBuilder.UpdateData(
                table: "Designations",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2025, 9, 25, 17, 27, 0, 864, DateTimeKind.Local).AddTicks(1869), new DateTime(2025, 9, 25, 17, 27, 0, 864, DateTimeKind.Local).AddTicks(1869) });

            migrationBuilder.UpdateData(
                table: "EmploymentTypes",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2025, 9, 25, 17, 27, 0, 864, DateTimeKind.Local).AddTicks(1889), new DateTime(2025, 9, 25, 17, 27, 0, 864, DateTimeKind.Local).AddTicks(1889) });

            migrationBuilder.UpdateData(
                table: "Genders",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2025, 9, 25, 17, 27, 0, 864, DateTimeKind.Local).AddTicks(1936), new DateTime(2025, 9, 25, 17, 27, 0, 864, DateTimeKind.Local).AddTicks(1937) });

            migrationBuilder.UpdateData(
                table: "Nationalities",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2025, 9, 25, 17, 27, 0, 864, DateTimeKind.Local).AddTicks(1907), new DateTime(2025, 9, 25, 17, 27, 0, 864, DateTimeKind.Local).AddTicks(1907) });

            migrationBuilder.UpdateData(
                table: "Person",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "DateOfBirth", "EmploymentDate", "EndofEmploymentDate", "Modified" },
                values: new object[] { new DateTime(2025, 9, 25, 17, 27, 0, 864, DateTimeKind.Local).AddTicks(1963), new DateTime(2025, 9, 25, 17, 27, 0, 864, DateTimeKind.Local).AddTicks(1960), new DateTime(2025, 9, 25, 17, 27, 0, 864, DateTimeKind.Local).AddTicks(1957), new DateTime(2025, 9, 25, 17, 27, 0, 864, DateTimeKind.Local).AddTicks(1958), new DateTime(2025, 9, 25, 17, 27, 0, 864, DateTimeKind.Local).AddTicks(1963) });

            migrationBuilder.UpdateData(
                table: "Religions",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2025, 9, 25, 17, 27, 0, 864, DateTimeKind.Local).AddTicks(1922), new DateTime(2025, 9, 25, 17, 27, 0, 864, DateTimeKind.Local).AddTicks(1923) });

            migrationBuilder.UpdateData(
                table: "StaffCategories",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2025, 9, 25, 17, 27, 0, 864, DateTimeKind.Local).AddTicks(1852), new DateTime(2025, 9, 25, 17, 27, 0, 864, DateTimeKind.Local).AddTicks(1853) });
        }
    }
}
