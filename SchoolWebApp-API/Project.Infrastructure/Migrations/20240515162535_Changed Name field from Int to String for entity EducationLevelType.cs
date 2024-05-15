using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SchoolWebApp.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ChangedNamefieldfromInttoStringforentityEducationLevelType : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "EducationLevelTypes",
                type: "varchar(255)",
                maxLength: 255,
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldMaxLength: 255)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "269f0cf3-405e-4163-83f3-1b63ebebd62e",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2024, 5, 15, 19, 25, 34, 131, DateTimeKind.Local).AddTicks(8644), new DateTime(2024, 5, 15, 19, 25, 34, 131, DateTimeKind.Local).AddTicks(8660) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "448df289-142c-4959-a912-60733515e1b4",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2024, 5, 15, 19, 25, 34, 131, DateTimeKind.Local).AddTicks(8587), new DateTime(2024, 5, 15, 19, 25, 34, 131, DateTimeKind.Local).AddTicks(8589) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "48c50c3a-9958-453b-b649-4e21af131322",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2024, 5, 15, 19, 25, 34, 131, DateTimeKind.Local).AddTicks(8529), new DateTime(2024, 5, 15, 19, 25, 34, 131, DateTimeKind.Local).AddTicks(8531) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "717d9b15-a428-440c-b26b-08d3bbb68b02",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2024, 5, 15, 19, 25, 34, 131, DateTimeKind.Local).AddTicks(8298), new DateTime(2024, 5, 15, 19, 25, 34, 131, DateTimeKind.Local).AddTicks(8339) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "95ed2407-3e58-4af2-88a4-1c4e96473f68",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2024, 5, 15, 19, 25, 34, 131, DateTimeKind.Local).AddTicks(8464), new DateTime(2024, 5, 15, 19, 25, 34, 131, DateTimeKind.Local).AddTicks(8466) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "97942bee-ef12-4425-8225-4f293d0f36dd",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2024, 5, 15, 19, 25, 34, 131, DateTimeKind.Local).AddTicks(8796), new DateTime(2024, 5, 15, 19, 25, 34, 131, DateTimeKind.Local).AddTicks(8797) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "cd12b44b-103b-48df-8887-a2bf42e0651e",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2024, 5, 15, 19, 25, 34, 131, DateTimeKind.Local).AddTicks(8738), new DateTime(2024, 5, 15, 19, 25, 34, 131, DateTimeKind.Local).AddTicks(8740) });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "7e67d486-af3e-49f1-a109-a2b864b8e0ec",
                columns: new[] { "ConcurrencyStamp", "Created", "Modified", "PasswordHash", "SecurityStamp" },
                values: new object[] { "2a88f291-f26e-443f-a5e0-efaedc491001", new DateTime(2024, 5, 15, 19, 25, 34, 131, DateTimeKind.Local).AddTicks(8964), new DateTime(2024, 5, 15, 19, 25, 34, 131, DateTimeKind.Local).AddTicks(8967), "AQAAAAIAAYagAAAAEPMPhW+s1gu/11Uxe3oKLIZeXQEpHYpgK9OUxu5IEoRUMixm3wv8vNYWvMDiH6kJcg==", "6b82ef74-3f82-4773-8f03-a707690df16f" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "Name",
                table: "EducationLevelTypes",
                type: "int",
                maxLength: 255,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(255)",
                oldMaxLength: 255)
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "269f0cf3-405e-4163-83f3-1b63ebebd62e",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2024, 5, 15, 18, 36, 1, 916, DateTimeKind.Local).AddTicks(7420), new DateTime(2024, 5, 15, 18, 36, 1, 916, DateTimeKind.Local).AddTicks(7432) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "448df289-142c-4959-a912-60733515e1b4",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2024, 5, 15, 18, 36, 1, 916, DateTimeKind.Local).AddTicks(7386), new DateTime(2024, 5, 15, 18, 36, 1, 916, DateTimeKind.Local).AddTicks(7387) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "48c50c3a-9958-453b-b649-4e21af131322",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2024, 5, 15, 18, 36, 1, 916, DateTimeKind.Local).AddTicks(7351), new DateTime(2024, 5, 15, 18, 36, 1, 916, DateTimeKind.Local).AddTicks(7353) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "717d9b15-a428-440c-b26b-08d3bbb68b02",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2024, 5, 15, 18, 36, 1, 916, DateTimeKind.Local).AddTicks(7158), new DateTime(2024, 5, 15, 18, 36, 1, 916, DateTimeKind.Local).AddTicks(7209) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "95ed2407-3e58-4af2-88a4-1c4e96473f68",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2024, 5, 15, 18, 36, 1, 916, DateTimeKind.Local).AddTicks(7312), new DateTime(2024, 5, 15, 18, 36, 1, 916, DateTimeKind.Local).AddTicks(7314) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "97942bee-ef12-4425-8225-4f293d0f36dd",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2024, 5, 15, 18, 36, 1, 916, DateTimeKind.Local).AddTicks(7536), new DateTime(2024, 5, 15, 18, 36, 1, 916, DateTimeKind.Local).AddTicks(7538) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "cd12b44b-103b-48df-8887-a2bf42e0651e",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2024, 5, 15, 18, 36, 1, 916, DateTimeKind.Local).AddTicks(7497), new DateTime(2024, 5, 15, 18, 36, 1, 916, DateTimeKind.Local).AddTicks(7499) });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "7e67d486-af3e-49f1-a109-a2b864b8e0ec",
                columns: new[] { "ConcurrencyStamp", "Created", "Modified", "PasswordHash", "SecurityStamp" },
                values: new object[] { "6cb6c1da-9744-49fe-8273-98b5d10a8ef3", new DateTime(2024, 5, 15, 18, 36, 1, 916, DateTimeKind.Local).AddTicks(7693), new DateTime(2024, 5, 15, 18, 36, 1, 916, DateTimeKind.Local).AddTicks(7696), "AQAAAAIAAYagAAAAEP7+vCrmf4tREaQhDeolfmUUT+OlKtPlGIS/1+1dI2wjg3JI1NcMaWVivdZaNkXTeg==", "e2b4574c-4dbe-449b-a174-a6d647b3f6d0" });
        }
    }
}
