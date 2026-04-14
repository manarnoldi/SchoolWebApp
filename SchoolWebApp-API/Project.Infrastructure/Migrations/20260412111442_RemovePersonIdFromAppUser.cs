using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SchoolWebApp.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RemovePersonIdFromAppUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_Person_PersonId",
                table: "AspNetUsers");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_PersonId",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "PersonId",
                table: "AspNetUsers");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "269f0cf3-405e-4163-83f3-1b63ebebd62e",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 12, 14, 14, 40, 213, DateTimeKind.Local).AddTicks(6826), new DateTime(2026, 4, 12, 14, 14, 40, 213, DateTimeKind.Local).AddTicks(6829) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "448df289-142c-4959-a912-60733515e1b4",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 12, 14, 14, 40, 213, DateTimeKind.Local).AddTicks(6780), new DateTime(2026, 4, 12, 14, 14, 40, 213, DateTimeKind.Local).AddTicks(6783) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "48c50c3a-9958-453b-b649-4e21af131322",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 12, 14, 14, 40, 213, DateTimeKind.Local).AddTicks(6740), new DateTime(2026, 4, 12, 14, 14, 40, 213, DateTimeKind.Local).AddTicks(6742) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "717d9b15-a428-440c-b26b-08d3bbb68b02",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 12, 14, 14, 40, 213, DateTimeKind.Local).AddTicks(6602), new DateTime(2026, 4, 12, 14, 14, 40, 213, DateTimeKind.Local).AddTicks(6623) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "95ed2407-3e58-4af2-88a4-1c4e96473f68",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 12, 14, 14, 40, 213, DateTimeKind.Local).AddTicks(6701), new DateTime(2026, 4, 12, 14, 14, 40, 213, DateTimeKind.Local).AddTicks(6704) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "97942bee-ef12-4425-8225-4f293d0f36dd",
                columns: new[] { "Created", "Modified", "Name", "NormalizedName" },
                values: new object[] { new DateTime(2026, 4, 12, 14, 14, 40, 213, DateTimeKind.Local).AddTicks(6903), new DateTime(2026, 4, 12, 14, 14, 40, 213, DateTimeKind.Local).AddTicks(6906), "Others", "OTHERS" });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "cd12b44b-103b-48df-8887-a2bf42e0651e",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 12, 14, 14, 40, 213, DateTimeKind.Local).AddTicks(6866), new DateTime(2026, 4, 12, 14, 14, 40, 213, DateTimeKind.Local).AddTicks(6868) });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "7e67d486-af3e-49f1-a109-a2b864b8e0ec",
                columns: new[] { "ConcurrencyStamp", "Created", "Modified", "PasswordHash", "SecurityStamp" },
                values: new object[] { "744ea72c-66ea-421f-bd70-dce18f3ee783", new DateTime(2026, 4, 12, 14, 14, 40, 213, DateTimeKind.Local).AddTicks(7364), new DateTime(2026, 4, 12, 14, 14, 40, 213, DateTimeKind.Local).AddTicks(7367), "AQAAAAIAAYagAAAAEJXltM6bKkfdcca4m46V4H35onPAmxGNvfVy5BC+LcIbPWoPp/O6zr3VxLcLj5eZXg==", "36f2146c-3d3e-479d-8746-03735a002751" });

            migrationBuilder.UpdateData(
                table: "Designations",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 12, 14, 14, 40, 213, DateTimeKind.Local).AddTicks(6998), new DateTime(2026, 4, 12, 14, 14, 40, 213, DateTimeKind.Local).AddTicks(6999) });

            migrationBuilder.UpdateData(
                table: "EmploymentTypes",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 12, 14, 14, 40, 213, DateTimeKind.Local).AddTicks(7046), new DateTime(2026, 4, 12, 14, 14, 40, 213, DateTimeKind.Local).AddTicks(7047) });

            migrationBuilder.UpdateData(
                table: "Genders",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 12, 14, 14, 40, 213, DateTimeKind.Local).AddTicks(7168), new DateTime(2026, 4, 12, 14, 14, 40, 213, DateTimeKind.Local).AddTicks(7170) });

            migrationBuilder.UpdateData(
                table: "Nationalities",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 12, 14, 14, 40, 213, DateTimeKind.Local).AddTicks(7084), new DateTime(2026, 4, 12, 14, 14, 40, 213, DateTimeKind.Local).AddTicks(7085) });

            migrationBuilder.UpdateData(
                table: "Person",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "DateOfBirth", "EmploymentDate", "EndofEmploymentDate", "Modified" },
                values: new object[] { new DateTime(2026, 4, 12, 14, 14, 40, 213, DateTimeKind.Local).AddTicks(7246), new DateTime(2026, 4, 12, 14, 14, 40, 213, DateTimeKind.Local).AddTicks(7243), new DateTime(2026, 4, 12, 14, 14, 40, 213, DateTimeKind.Local).AddTicks(7233), new DateTime(2026, 4, 12, 14, 14, 40, 213, DateTimeKind.Local).AddTicks(7234), new DateTime(2026, 4, 12, 14, 14, 40, 213, DateTimeKind.Local).AddTicks(7255) });

            migrationBuilder.UpdateData(
                table: "Religions",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 12, 14, 14, 40, 213, DateTimeKind.Local).AddTicks(7133), new DateTime(2026, 4, 12, 14, 14, 40, 213, DateTimeKind.Local).AddTicks(7134) });

            migrationBuilder.UpdateData(
                table: "StaffCategories",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 12, 14, 14, 40, 213, DateTimeKind.Local).AddTicks(6959), new DateTime(2026, 4, 12, 14, 14, 40, 213, DateTimeKind.Local).AddTicks(6961) });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "PersonId",
                table: "AspNetUsers",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "269f0cf3-405e-4163-83f3-1b63ebebd62e",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 11, 20, 37, 19, 146, DateTimeKind.Local).AddTicks(8930), new DateTime(2026, 4, 11, 20, 37, 19, 146, DateTimeKind.Local).AddTicks(8931) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "448df289-142c-4959-a912-60733515e1b4",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 11, 20, 37, 19, 146, DateTimeKind.Local).AddTicks(8907), new DateTime(2026, 4, 11, 20, 37, 19, 146, DateTimeKind.Local).AddTicks(8908) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "48c50c3a-9958-453b-b649-4e21af131322",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 11, 20, 37, 19, 146, DateTimeKind.Local).AddTicks(8883), new DateTime(2026, 4, 11, 20, 37, 19, 146, DateTimeKind.Local).AddTicks(8884) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "717d9b15-a428-440c-b26b-08d3bbb68b02",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 11, 20, 37, 19, 146, DateTimeKind.Local).AddTicks(8778), new DateTime(2026, 4, 11, 20, 37, 19, 146, DateTimeKind.Local).AddTicks(8792) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "95ed2407-3e58-4af2-88a4-1c4e96473f68",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 11, 20, 37, 19, 146, DateTimeKind.Local).AddTicks(8857), new DateTime(2026, 4, 11, 20, 37, 19, 146, DateTimeKind.Local).AddTicks(8858) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "97942bee-ef12-4425-8225-4f293d0f36dd",
                columns: new[] { "Created", "Modified", "Name", "NormalizedName" },
                values: new object[] { new DateTime(2026, 4, 11, 20, 37, 19, 146, DateTimeKind.Local).AddTicks(8977), new DateTime(2026, 4, 11, 20, 37, 19, 146, DateTimeKind.Local).AddTicks(8978), "Visitor", "VISITOR" });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "cd12b44b-103b-48df-8887-a2bf42e0651e",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 11, 20, 37, 19, 146, DateTimeKind.Local).AddTicks(8954), new DateTime(2026, 4, 11, 20, 37, 19, 146, DateTimeKind.Local).AddTicks(8955) });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "7e67d486-af3e-49f1-a109-a2b864b8e0ec",
                columns: new[] { "ConcurrencyStamp", "Created", "Modified", "PasswordHash", "PersonId", "SecurityStamp" },
                values: new object[] { "a85145b9-4cca-4da8-b693-7e0a71f47dfc", new DateTime(2026, 4, 11, 20, 37, 19, 146, DateTimeKind.Local).AddTicks(9265), new DateTime(2026, 4, 11, 20, 37, 19, 146, DateTimeKind.Local).AddTicks(9267), "AQAAAAIAAYagAAAAEAPGjSP8ke/efQpILYxLS1RLajiJS0CtJ4gPwqaNtsWSnEobKfwZQIeJjFxr3s4uUw==", 1, "8aa7020e-2e8f-4dfc-9dbf-9074ce6ae640" });

            migrationBuilder.UpdateData(
                table: "Designations",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 11, 20, 37, 19, 146, DateTimeKind.Local).AddTicks(9037), new DateTime(2026, 4, 11, 20, 37, 19, 146, DateTimeKind.Local).AddTicks(9038) });

            migrationBuilder.UpdateData(
                table: "EmploymentTypes",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 11, 20, 37, 19, 146, DateTimeKind.Local).AddTicks(9064), new DateTime(2026, 4, 11, 20, 37, 19, 146, DateTimeKind.Local).AddTicks(9065) });

            migrationBuilder.UpdateData(
                table: "Genders",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 11, 20, 37, 19, 146, DateTimeKind.Local).AddTicks(9129), new DateTime(2026, 4, 11, 20, 37, 19, 146, DateTimeKind.Local).AddTicks(9130) });

            migrationBuilder.UpdateData(
                table: "Nationalities",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 11, 20, 37, 19, 146, DateTimeKind.Local).AddTicks(9084), new DateTime(2026, 4, 11, 20, 37, 19, 146, DateTimeKind.Local).AddTicks(9085) });

            migrationBuilder.UpdateData(
                table: "Person",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "DateOfBirth", "EmploymentDate", "EndofEmploymentDate", "Modified" },
                values: new object[] { new DateTime(2026, 4, 11, 20, 37, 19, 146, DateTimeKind.Local).AddTicks(9171), new DateTime(2026, 4, 11, 20, 37, 19, 146, DateTimeKind.Local).AddTicks(9168), new DateTime(2026, 4, 11, 20, 37, 19, 146, DateTimeKind.Local).AddTicks(9164), new DateTime(2026, 4, 11, 20, 37, 19, 146, DateTimeKind.Local).AddTicks(9165), new DateTime(2026, 4, 11, 20, 37, 19, 146, DateTimeKind.Local).AddTicks(9184) });

            migrationBuilder.UpdateData(
                table: "Religions",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 11, 20, 37, 19, 146, DateTimeKind.Local).AddTicks(9109), new DateTime(2026, 4, 11, 20, 37, 19, 146, DateTimeKind.Local).AddTicks(9110) });

            migrationBuilder.UpdateData(
                table: "StaffCategories",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 11, 20, 37, 19, 146, DateTimeKind.Local).AddTicks(9014), new DateTime(2026, 4, 11, 20, 37, 19, 146, DateTimeKind.Local).AddTicks(9016) });

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_PersonId",
                table: "AspNetUsers",
                column: "PersonId");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_Person_PersonId",
                table: "AspNetUsers",
                column: "PersonId",
                principalTable: "Person",
                principalColumn: "Id");
        }
    }
}
