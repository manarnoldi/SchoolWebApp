using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SchoolWebApp.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddedDescriptionFieldtoStudentClassEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "StudentClasses",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "StaffSubjects",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<int>(
                name: "SchoolClassId",
                table: "StaffSubjects",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "269f0cf3-405e-4163-83f3-1b63ebebd62e",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2024, 5, 20, 9, 8, 32, 775, DateTimeKind.Local).AddTicks(1294), new DateTime(2024, 5, 20, 9, 8, 32, 775, DateTimeKind.Local).AddTicks(1306) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "448df289-142c-4959-a912-60733515e1b4",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2024, 5, 20, 9, 8, 32, 775, DateTimeKind.Local).AddTicks(1267), new DateTime(2024, 5, 20, 9, 8, 32, 775, DateTimeKind.Local).AddTicks(1268) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "48c50c3a-9958-453b-b649-4e21af131322",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2024, 5, 20, 9, 8, 32, 775, DateTimeKind.Local).AddTicks(1222), new DateTime(2024, 5, 20, 9, 8, 32, 775, DateTimeKind.Local).AddTicks(1223) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "717d9b15-a428-440c-b26b-08d3bbb68b02",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2024, 5, 20, 9, 8, 32, 775, DateTimeKind.Local).AddTicks(1100), new DateTime(2024, 5, 20, 9, 8, 32, 775, DateTimeKind.Local).AddTicks(1137) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "95ed2407-3e58-4af2-88a4-1c4e96473f68",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2024, 5, 20, 9, 8, 32, 775, DateTimeKind.Local).AddTicks(1194), new DateTime(2024, 5, 20, 9, 8, 32, 775, DateTimeKind.Local).AddTicks(1196) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "97942bee-ef12-4425-8225-4f293d0f36dd",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2024, 5, 20, 9, 8, 32, 775, DateTimeKind.Local).AddTicks(1432), new DateTime(2024, 5, 20, 9, 8, 32, 775, DateTimeKind.Local).AddTicks(1433) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "cd12b44b-103b-48df-8887-a2bf42e0651e",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2024, 5, 20, 9, 8, 32, 775, DateTimeKind.Local).AddTicks(1406), new DateTime(2024, 5, 20, 9, 8, 32, 775, DateTimeKind.Local).AddTicks(1408) });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "7e67d486-af3e-49f1-a109-a2b864b8e0ec",
                columns: new[] { "ConcurrencyStamp", "Created", "Modified", "PasswordHash", "SecurityStamp" },
                values: new object[] { "27b0a525-205f-4999-b675-c577bb162518", new DateTime(2024, 5, 20, 9, 8, 32, 775, DateTimeKind.Local).AddTicks(1538), new DateTime(2024, 5, 20, 9, 8, 32, 775, DateTimeKind.Local).AddTicks(1539), "AQAAAAIAAYagAAAAEPJVOgYV3MO/diQzIb3C1t07IrJTzaL3dvAzyD9fojxm5/jEI+hUj0dPK2Q6CE8ruw==", "cd2506e5-d244-4a6b-94f3-4e540ed40cb3" });

            migrationBuilder.CreateIndex(
                name: "IX_StaffSubjects_SchoolClassId",
                table: "StaffSubjects",
                column: "SchoolClassId");

            migrationBuilder.AddForeignKey(
                name: "FK_StaffSubjects_SchoolClasses_SchoolClassId",
                table: "StaffSubjects",
                column: "SchoolClassId",
                principalTable: "SchoolClasses",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_StaffSubjects_SchoolClasses_SchoolClassId",
                table: "StaffSubjects");

            migrationBuilder.DropIndex(
                name: "IX_StaffSubjects_SchoolClassId",
                table: "StaffSubjects");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "StudentClasses");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "StaffSubjects");

            migrationBuilder.DropColumn(
                name: "SchoolClassId",
                table: "StaffSubjects");

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
    }
}
