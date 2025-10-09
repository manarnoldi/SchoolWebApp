using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SchoolWebApp.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class MovedLearningLevelfromSpecificOutcomestoStrands2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            //migrationBuilder.DropForeignKey(
            //    name: "FK_SpecificOutcomes_LearningLevels_LearningLevelId",
            //    table: "SpecificOutcomes");

            //migrationBuilder.DropIndex(
            //    name: "IX_SpecificOutcomes_LearningLevelId",
            //    table: "SpecificOutcomes");

            //migrationBuilder.DropColumn(
            //    name: "LearningLevelId",
            //    table: "SpecificOutcomes");

            //migrationBuilder.AddColumn<int>(
            //    name: "LearningLevelId",
            //    table: "Strands",
            //    type: "int",
            //    nullable: false,
            //    defaultValue: 0);

            //migrationBuilder.AlterColumn<int>(
            //    name: "LearningLevelId",
            //    table: "SpecificOutcomes",
            //    type: "int",
            //    nullable: true,
            //    oldClrType: typeof(int),
            //    oldType: "int");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "269f0cf3-405e-4163-83f3-1b63ebebd62e",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2025, 9, 27, 20, 10, 27, 109, DateTimeKind.Local).AddTicks(7018), new DateTime(2025, 9, 27, 20, 10, 27, 109, DateTimeKind.Local).AddTicks(7020) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "448df289-142c-4959-a912-60733515e1b4",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2025, 9, 27, 20, 10, 27, 109, DateTimeKind.Local).AddTicks(6993), new DateTime(2025, 9, 27, 20, 10, 27, 109, DateTimeKind.Local).AddTicks(6995) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "48c50c3a-9958-453b-b649-4e21af131322",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2025, 9, 27, 20, 10, 27, 109, DateTimeKind.Local).AddTicks(6971), new DateTime(2025, 9, 27, 20, 10, 27, 109, DateTimeKind.Local).AddTicks(6973) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "717d9b15-a428-440c-b26b-08d3bbb68b02",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2025, 9, 27, 20, 10, 27, 109, DateTimeKind.Local).AddTicks(6882), new DateTime(2025, 9, 27, 20, 10, 27, 109, DateTimeKind.Local).AddTicks(6898) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "95ed2407-3e58-4af2-88a4-1c4e96473f68",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2025, 9, 27, 20, 10, 27, 109, DateTimeKind.Local).AddTicks(6944), new DateTime(2025, 9, 27, 20, 10, 27, 109, DateTimeKind.Local).AddTicks(6946) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "97942bee-ef12-4425-8225-4f293d0f36dd",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2025, 9, 27, 20, 10, 27, 109, DateTimeKind.Local).AddTicks(7074), new DateTime(2025, 9, 27, 20, 10, 27, 109, DateTimeKind.Local).AddTicks(7075) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "cd12b44b-103b-48df-8887-a2bf42e0651e",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2025, 9, 27, 20, 10, 27, 109, DateTimeKind.Local).AddTicks(7042), new DateTime(2025, 9, 27, 20, 10, 27, 109, DateTimeKind.Local).AddTicks(7045) });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "7e67d486-af3e-49f1-a109-a2b864b8e0ec",
                columns: new[] { "ConcurrencyStamp", "Created", "Modified", "PasswordHash", "SecurityStamp" },
                values: new object[] { "8a6cded2-1616-453a-9233-38e36a7b3cde", new DateTime(2025, 9, 27, 20, 10, 27, 109, DateTimeKind.Local).AddTicks(7359), new DateTime(2025, 9, 27, 20, 10, 27, 109, DateTimeKind.Local).AddTicks(7360), "AQAAAAIAAYagAAAAEJykU+ATBrw5nWhhdXF6LtT6nWPrViR5eZu4UUXHLzrg8mYt1IIyn8EzL33OYtIyOw==", "b00504b6-f641-4c8e-a409-86b0a4350a52" });

            migrationBuilder.UpdateData(
                table: "Designations",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2025, 9, 27, 20, 10, 27, 109, DateTimeKind.Local).AddTicks(7136), new DateTime(2025, 9, 27, 20, 10, 27, 109, DateTimeKind.Local).AddTicks(7139) });

            migrationBuilder.UpdateData(
                table: "EmploymentTypes",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2025, 9, 27, 20, 10, 27, 109, DateTimeKind.Local).AddTicks(7177), new DateTime(2025, 9, 27, 20, 10, 27, 109, DateTimeKind.Local).AddTicks(7178) });

            migrationBuilder.UpdateData(
                table: "Genders",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2025, 9, 27, 20, 10, 27, 109, DateTimeKind.Local).AddTicks(7236), new DateTime(2025, 9, 27, 20, 10, 27, 109, DateTimeKind.Local).AddTicks(7237) });

            migrationBuilder.UpdateData(
                table: "Nationalities",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2025, 9, 27, 20, 10, 27, 109, DateTimeKind.Local).AddTicks(7197), new DateTime(2025, 9, 27, 20, 10, 27, 109, DateTimeKind.Local).AddTicks(7198) });

            migrationBuilder.UpdateData(
                table: "Person",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "DateOfBirth", "EmploymentDate", "EndofEmploymentDate", "Modified" },
                values: new object[] { new DateTime(2025, 9, 27, 20, 10, 27, 109, DateTimeKind.Local).AddTicks(7277), new DateTime(2025, 9, 27, 20, 10, 27, 109, DateTimeKind.Local).AddTicks(7274), new DateTime(2025, 9, 27, 20, 10, 27, 109, DateTimeKind.Local).AddTicks(7267), new DateTime(2025, 9, 27, 20, 10, 27, 109, DateTimeKind.Local).AddTicks(7268), new DateTime(2025, 9, 27, 20, 10, 27, 109, DateTimeKind.Local).AddTicks(7284) });

            migrationBuilder.UpdateData(
                table: "Religions",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2025, 9, 27, 20, 10, 27, 109, DateTimeKind.Local).AddTicks(7216), new DateTime(2025, 9, 27, 20, 10, 27, 109, DateTimeKind.Local).AddTicks(7217) });

            migrationBuilder.UpdateData(
                table: "StaffCategories",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2025, 9, 27, 20, 10, 27, 109, DateTimeKind.Local).AddTicks(7112), new DateTime(2025, 9, 27, 20, 10, 27, 109, DateTimeKind.Local).AddTicks(7114) });

            //migrationBuilder.CreateIndex(
            //    name: "IX_Strands_LearningLevelId",
            //    table: "Strands",
            //    column: "LearningLevelId");

            //migrationBuilder.AddForeignKey(
            //    name: "FK_Strands_LearningLevels_LearningLevelId",
            //    table: "Strands",
            //    column: "LearningLevelId",
            //    principalTable: "LearningLevels",
            //    principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            //migrationBuilder.DropForeignKey(
            //    name: "FK_Strands_LearningLevels_LearningLevelId",
            //    table: "Strands");

            //migrationBuilder.DropIndex(
            //    name: "IX_Strands_LearningLevelId",
            //    table: "Strands");

            //migrationBuilder.DropColumn(
            //    name: "LearningLevelId",
            //    table: "Strands");

            //migrationBuilder.AlterColumn<int>(
            //    name: "LearningLevelId",
            //    table: "SpecificOutcomes",
            //    type: "int",
            //    nullable: false,
            //    defaultValue: 0,
            //    oldClrType: typeof(int),
            //    oldType: "int",
            //    oldNullable: true);

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "269f0cf3-405e-4163-83f3-1b63ebebd62e",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2025, 9, 26, 8, 32, 7, 13, DateTimeKind.Local).AddTicks(3934), new DateTime(2025, 9, 26, 8, 32, 7, 13, DateTimeKind.Local).AddTicks(3937) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "448df289-142c-4959-a912-60733515e1b4",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2025, 9, 26, 8, 32, 7, 13, DateTimeKind.Local).AddTicks(3876), new DateTime(2025, 9, 26, 8, 32, 7, 13, DateTimeKind.Local).AddTicks(3879) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "48c50c3a-9958-453b-b649-4e21af131322",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2025, 9, 26, 8, 32, 7, 13, DateTimeKind.Local).AddTicks(3843), new DateTime(2025, 9, 26, 8, 32, 7, 13, DateTimeKind.Local).AddTicks(3845) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "717d9b15-a428-440c-b26b-08d3bbb68b02",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2025, 9, 26, 8, 32, 7, 13, DateTimeKind.Local).AddTicks(3712), new DateTime(2025, 9, 26, 8, 32, 7, 13, DateTimeKind.Local).AddTicks(3741) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "95ed2407-3e58-4af2-88a4-1c4e96473f68",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2025, 9, 26, 8, 32, 7, 13, DateTimeKind.Local).AddTicks(3808), new DateTime(2025, 9, 26, 8, 32, 7, 13, DateTimeKind.Local).AddTicks(3810) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "97942bee-ef12-4425-8225-4f293d0f36dd",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2025, 9, 26, 8, 32, 7, 13, DateTimeKind.Local).AddTicks(4011), new DateTime(2025, 9, 26, 8, 32, 7, 13, DateTimeKind.Local).AddTicks(4013) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "cd12b44b-103b-48df-8887-a2bf42e0651e",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2025, 9, 26, 8, 32, 7, 13, DateTimeKind.Local).AddTicks(3977), new DateTime(2025, 9, 26, 8, 32, 7, 13, DateTimeKind.Local).AddTicks(3979) });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "7e67d486-af3e-49f1-a109-a2b864b8e0ec",
                columns: new[] { "ConcurrencyStamp", "Created", "Modified", "PasswordHash", "SecurityStamp" },
                values: new object[] { "e65e6df3-4d34-49cc-ba41-539ee240b915", new DateTime(2025, 9, 26, 8, 32, 7, 13, DateTimeKind.Local).AddTicks(4420), new DateTime(2025, 9, 26, 8, 32, 7, 13, DateTimeKind.Local).AddTicks(4422), "AQAAAAIAAYagAAAAEBsoP7MucQUkPcb0R/I1ngFdXa2mVHLkhLBlEvgF6PQeAwQAtkYmxAQFI93bFKFXrw==", "924a0c0e-c590-48b1-aa62-23be37a5a689" });

            migrationBuilder.UpdateData(
                table: "Designations",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2025, 9, 26, 8, 32, 7, 13, DateTimeKind.Local).AddTicks(4089), new DateTime(2025, 9, 26, 8, 32, 7, 13, DateTimeKind.Local).AddTicks(4090) });

            migrationBuilder.UpdateData(
                table: "EmploymentTypes",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2025, 9, 26, 8, 32, 7, 13, DateTimeKind.Local).AddTicks(4168), new DateTime(2025, 9, 26, 8, 32, 7, 13, DateTimeKind.Local).AddTicks(4169) });

            migrationBuilder.UpdateData(
                table: "Genders",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2025, 9, 26, 8, 32, 7, 13, DateTimeKind.Local).AddTicks(4257), new DateTime(2025, 9, 26, 8, 32, 7, 13, DateTimeKind.Local).AddTicks(4258) });

            migrationBuilder.UpdateData(
                table: "Nationalities",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2025, 9, 26, 8, 32, 7, 13, DateTimeKind.Local).AddTicks(4198), new DateTime(2025, 9, 26, 8, 32, 7, 13, DateTimeKind.Local).AddTicks(4199) });

            migrationBuilder.UpdateData(
                table: "Person",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "DateOfBirth", "EmploymentDate", "EndofEmploymentDate", "Modified" },
                values: new object[] { new DateTime(2025, 9, 26, 8, 32, 7, 13, DateTimeKind.Local).AddTicks(4311), new DateTime(2025, 9, 26, 8, 32, 7, 13, DateTimeKind.Local).AddTicks(4307), new DateTime(2025, 9, 26, 8, 32, 7, 13, DateTimeKind.Local).AddTicks(4301), new DateTime(2025, 9, 26, 8, 32, 7, 13, DateTimeKind.Local).AddTicks(4303), new DateTime(2025, 9, 26, 8, 32, 7, 13, DateTimeKind.Local).AddTicks(4315) });

            migrationBuilder.UpdateData(
                table: "Religions",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2025, 9, 26, 8, 32, 7, 13, DateTimeKind.Local).AddTicks(4228), new DateTime(2025, 9, 26, 8, 32, 7, 13, DateTimeKind.Local).AddTicks(4230) });

            migrationBuilder.UpdateData(
                table: "StaffCategories",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2025, 9, 26, 8, 32, 7, 13, DateTimeKind.Local).AddTicks(4057), new DateTime(2025, 9, 26, 8, 32, 7, 13, DateTimeKind.Local).AddTicks(4059) });
        }
    }
}
