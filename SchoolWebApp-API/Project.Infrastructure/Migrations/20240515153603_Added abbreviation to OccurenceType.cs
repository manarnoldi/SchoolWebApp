using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SchoolWebApp.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddedabbreviationtoOccurenceType : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SubjectGroups_Departments_DepartmentId",
                table: "SubjectGroups");

            migrationBuilder.DropIndex(
                name: "IX_SubjectGroups_DepartmentId",
                table: "SubjectGroups");

            migrationBuilder.DropColumn(
                name: "DepartmentId",
                table: "SubjectGroups");

            migrationBuilder.AddColumn<int>(
                name: "DepartmentId",
                table: "Subjects",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Abbreviation",
                table: "OccurenceTypes",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

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

            migrationBuilder.CreateIndex(
                name: "IX_Subjects_DepartmentId",
                table: "Subjects",
                column: "DepartmentId");

            migrationBuilder.AddForeignKey(
                name: "FK_Subjects_Departments_DepartmentId",
                table: "Subjects",
                column: "DepartmentId",
                principalTable: "Departments",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Subjects_Departments_DepartmentId",
                table: "Subjects");

            migrationBuilder.DropIndex(
                name: "IX_Subjects_DepartmentId",
                table: "Subjects");

            migrationBuilder.DropColumn(
                name: "DepartmentId",
                table: "Subjects");

            migrationBuilder.DropColumn(
                name: "Abbreviation",
                table: "OccurenceTypes");

            migrationBuilder.AddColumn<int>(
                name: "DepartmentId",
                table: "SubjectGroups",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "269f0cf3-405e-4163-83f3-1b63ebebd62e",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2024, 5, 14, 17, 29, 52, 550, DateTimeKind.Local).AddTicks(5164), new DateTime(2024, 5, 14, 17, 29, 52, 550, DateTimeKind.Local).AddTicks(5165) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "448df289-142c-4959-a912-60733515e1b4",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2024, 5, 14, 17, 29, 52, 550, DateTimeKind.Local).AddTicks(5139), new DateTime(2024, 5, 14, 17, 29, 52, 550, DateTimeKind.Local).AddTicks(5140) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "48c50c3a-9958-453b-b649-4e21af131322",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2024, 5, 14, 17, 29, 52, 550, DateTimeKind.Local).AddTicks(5111), new DateTime(2024, 5, 14, 17, 29, 52, 550, DateTimeKind.Local).AddTicks(5112) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "717d9b15-a428-440c-b26b-08d3bbb68b02",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2024, 5, 14, 17, 29, 52, 550, DateTimeKind.Local).AddTicks(4984), new DateTime(2024, 5, 14, 17, 29, 52, 550, DateTimeKind.Local).AddTicks(5014) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "95ed2407-3e58-4af2-88a4-1c4e96473f68",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2024, 5, 14, 17, 29, 52, 550, DateTimeKind.Local).AddTicks(5073), new DateTime(2024, 5, 14, 17, 29, 52, 550, DateTimeKind.Local).AddTicks(5084) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "97942bee-ef12-4425-8225-4f293d0f36dd",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2024, 5, 14, 17, 29, 52, 550, DateTimeKind.Local).AddTicks(5253), new DateTime(2024, 5, 14, 17, 29, 52, 550, DateTimeKind.Local).AddTicks(5255) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "cd12b44b-103b-48df-8887-a2bf42e0651e",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2024, 5, 14, 17, 29, 52, 550, DateTimeKind.Local).AddTicks(5194), new DateTime(2024, 5, 14, 17, 29, 52, 550, DateTimeKind.Local).AddTicks(5204) });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "7e67d486-af3e-49f1-a109-a2b864b8e0ec",
                columns: new[] { "ConcurrencyStamp", "Created", "Modified", "PasswordHash", "SecurityStamp" },
                values: new object[] { "ac40a9f3-274f-4ff5-9202-779d46cd640c", new DateTime(2024, 5, 14, 17, 29, 52, 550, DateTimeKind.Local).AddTicks(5352), new DateTime(2024, 5, 14, 17, 29, 52, 550, DateTimeKind.Local).AddTicks(5353), "AQAAAAIAAYagAAAAEJB+R3CRrTOkMpAZQLYdZMGozdizrxCy0wCLYO9ejzvbvziGd7GsNjZKqWqY0lUknA==", "d62dbd4a-f799-4645-bf71-a992386f43c1" });

            migrationBuilder.CreateIndex(
                name: "IX_SubjectGroups_DepartmentId",
                table: "SubjectGroups",
                column: "DepartmentId");

            migrationBuilder.AddForeignKey(
                name: "FK_SubjectGroups_Departments_DepartmentId",
                table: "SubjectGroups",
                column: "DepartmentId",
                principalTable: "Departments",
                principalColumn: "Id");
        }
    }
}
