﻿using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SchoolWebApp.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class MadeDescriptionnullableonSchoolClassLeaders : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "SchoolClassLeaders",
                type: "longtext",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "longtext")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "269f0cf3-405e-4163-83f3-1b63ebebd62e",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2024, 8, 1, 10, 59, 20, 645, DateTimeKind.Local).AddTicks(2533), new DateTime(2024, 8, 1, 10, 59, 20, 645, DateTimeKind.Local).AddTicks(2546) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "448df289-142c-4959-a912-60733515e1b4",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2024, 8, 1, 10, 59, 20, 645, DateTimeKind.Local).AddTicks(2502), new DateTime(2024, 8, 1, 10, 59, 20, 645, DateTimeKind.Local).AddTicks(2503) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "48c50c3a-9958-453b-b649-4e21af131322",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2024, 8, 1, 10, 59, 20, 645, DateTimeKind.Local).AddTicks(2461), new DateTime(2024, 8, 1, 10, 59, 20, 645, DateTimeKind.Local).AddTicks(2462) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "717d9b15-a428-440c-b26b-08d3bbb68b02",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2024, 8, 1, 10, 59, 20, 645, DateTimeKind.Local).AddTicks(2328), new DateTime(2024, 8, 1, 10, 59, 20, 645, DateTimeKind.Local).AddTicks(2364) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "95ed2407-3e58-4af2-88a4-1c4e96473f68",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2024, 8, 1, 10, 59, 20, 645, DateTimeKind.Local).AddTicks(2429), new DateTime(2024, 8, 1, 10, 59, 20, 645, DateTimeKind.Local).AddTicks(2430) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "97942bee-ef12-4425-8225-4f293d0f36dd",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2024, 8, 1, 10, 59, 20, 645, DateTimeKind.Local).AddTicks(2620), new DateTime(2024, 8, 1, 10, 59, 20, 645, DateTimeKind.Local).AddTicks(2621) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "cd12b44b-103b-48df-8887-a2bf42e0651e",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2024, 8, 1, 10, 59, 20, 645, DateTimeKind.Local).AddTicks(2591), new DateTime(2024, 8, 1, 10, 59, 20, 645, DateTimeKind.Local).AddTicks(2593) });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "7e67d486-af3e-49f1-a109-a2b864b8e0ec",
                columns: new[] { "ConcurrencyStamp", "Created", "Modified", "PasswordHash", "SecurityStamp" },
                values: new object[] { "3c8fe575-bb64-4a32-a54e-25ad61e37730", new DateTime(2024, 8, 1, 10, 59, 20, 645, DateTimeKind.Local).AddTicks(2720), new DateTime(2024, 8, 1, 10, 59, 20, 645, DateTimeKind.Local).AddTicks(2722), "AQAAAAIAAYagAAAAECEybmq7xvMnJKNloWmpl2d6HSTZAeCB7WLygZGOcaR5EtmhcT/RfDUzsOVS4GbN+A==", "56e957d7-7f84-45e3-a9f3-d1c3a46db8ff" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "SchoolClassLeaders",
                keyColumn: "Description",
                keyValue: null,
                column: "Description",
                value: "");

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "SchoolClassLeaders",
                type: "longtext",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "longtext",
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "269f0cf3-405e-4163-83f3-1b63ebebd62e",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2024, 7, 16, 10, 19, 16, 871, DateTimeKind.Local).AddTicks(7841), new DateTime(2024, 7, 16, 10, 19, 16, 871, DateTimeKind.Local).AddTicks(7857) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "448df289-142c-4959-a912-60733515e1b4",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2024, 7, 16, 10, 19, 16, 871, DateTimeKind.Local).AddTicks(7813), new DateTime(2024, 7, 16, 10, 19, 16, 871, DateTimeKind.Local).AddTicks(7814) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "48c50c3a-9958-453b-b649-4e21af131322",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2024, 7, 16, 10, 19, 16, 871, DateTimeKind.Local).AddTicks(7785), new DateTime(2024, 7, 16, 10, 19, 16, 871, DateTimeKind.Local).AddTicks(7786) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "717d9b15-a428-440c-b26b-08d3bbb68b02",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2024, 7, 16, 10, 19, 16, 871, DateTimeKind.Local).AddTicks(7652), new DateTime(2024, 7, 16, 10, 19, 16, 871, DateTimeKind.Local).AddTicks(7689) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "95ed2407-3e58-4af2-88a4-1c4e96473f68",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2024, 7, 16, 10, 19, 16, 871, DateTimeKind.Local).AddTicks(7751), new DateTime(2024, 7, 16, 10, 19, 16, 871, DateTimeKind.Local).AddTicks(7754) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "97942bee-ef12-4425-8225-4f293d0f36dd",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2024, 7, 16, 10, 19, 16, 871, DateTimeKind.Local).AddTicks(7937), new DateTime(2024, 7, 16, 10, 19, 16, 871, DateTimeKind.Local).AddTicks(7938) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "cd12b44b-103b-48df-8887-a2bf42e0651e",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2024, 7, 16, 10, 19, 16, 871, DateTimeKind.Local).AddTicks(7896), new DateTime(2024, 7, 16, 10, 19, 16, 871, DateTimeKind.Local).AddTicks(7897) });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "7e67d486-af3e-49f1-a109-a2b864b8e0ec",
                columns: new[] { "ConcurrencyStamp", "Created", "Modified", "PasswordHash", "SecurityStamp" },
                values: new object[] { "cb4b1066-8892-4232-a3cb-1f7550228c53", new DateTime(2024, 7, 16, 10, 19, 16, 871, DateTimeKind.Local).AddTicks(8045), new DateTime(2024, 7, 16, 10, 19, 16, 871, DateTimeKind.Local).AddTicks(8046), "AQAAAAIAAYagAAAAENbqJu4sMQWFlzR+oCYzglfs82dmqMH89mgTPnWoy2nrNFej/SzteOef6rMjWg35xA==", "a5357f14-9ef4-493a-b609-cbf6d13c61fc" });
        }
    }
}
