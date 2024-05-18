using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SchoolWebApp.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddedDescriptionandNumOfLessonsonSubjectentity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Subjects",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<int>(
                name: "NumOfLessons",
                table: "Subjects",
                type: "int",
                nullable: false,
                defaultValue: 0);

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Description",
                table: "Subjects");

            migrationBuilder.DropColumn(
                name: "NumOfLessons",
                table: "Subjects");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "269f0cf3-405e-4163-83f3-1b63ebebd62e",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2024, 5, 18, 9, 46, 10, 569, DateTimeKind.Local).AddTicks(5825), new DateTime(2024, 5, 18, 9, 46, 10, 569, DateTimeKind.Local).AddTicks(5845) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "448df289-142c-4959-a912-60733515e1b4",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2024, 5, 18, 9, 46, 10, 569, DateTimeKind.Local).AddTicks(5797), new DateTime(2024, 5, 18, 9, 46, 10, 569, DateTimeKind.Local).AddTicks(5799) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "48c50c3a-9958-453b-b649-4e21af131322",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2024, 5, 18, 9, 46, 10, 569, DateTimeKind.Local).AddTicks(5767), new DateTime(2024, 5, 18, 9, 46, 10, 569, DateTimeKind.Local).AddTicks(5768) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "717d9b15-a428-440c-b26b-08d3bbb68b02",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2024, 5, 18, 9, 46, 10, 569, DateTimeKind.Local).AddTicks(5574), new DateTime(2024, 5, 18, 9, 46, 10, 569, DateTimeKind.Local).AddTicks(5646) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "95ed2407-3e58-4af2-88a4-1c4e96473f68",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2024, 5, 18, 9, 46, 10, 569, DateTimeKind.Local).AddTicks(5719), new DateTime(2024, 5, 18, 9, 46, 10, 569, DateTimeKind.Local).AddTicks(5721) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "97942bee-ef12-4425-8225-4f293d0f36dd",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2024, 5, 18, 9, 46, 10, 569, DateTimeKind.Local).AddTicks(5911), new DateTime(2024, 5, 18, 9, 46, 10, 569, DateTimeKind.Local).AddTicks(5912) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "cd12b44b-103b-48df-8887-a2bf42e0651e",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2024, 5, 18, 9, 46, 10, 569, DateTimeKind.Local).AddTicks(5883), new DateTime(2024, 5, 18, 9, 46, 10, 569, DateTimeKind.Local).AddTicks(5885) });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "7e67d486-af3e-49f1-a109-a2b864b8e0ec",
                columns: new[] { "ConcurrencyStamp", "Created", "Modified", "PasswordHash", "SecurityStamp" },
                values: new object[] { "f419b830-5077-49cd-b567-ac0dad7d68a3", new DateTime(2024, 5, 18, 9, 46, 10, 569, DateTimeKind.Local).AddTicks(6005), new DateTime(2024, 5, 18, 9, 46, 10, 569, DateTimeKind.Local).AddTicks(6007), "AQAAAAIAAYagAAAAEFF0MT4r9lBSMbhKzus9S/aNJVmyytkHv5dbUiv+0B6vtZAlVyIyc569VLyPy+IKOA==", "c4e70fd0-43e2-416d-8392-415f158063b0" });
        }
    }
}
