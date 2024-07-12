using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SchoolWebApp.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Routineupdates202407110758 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            //migrationBuilder.DropForeignKey(
            //    name: "FK_SchoolClasses_Person_StaffDetailsId",
            //    table: "SchoolClasses");

            //migrationBuilder.DropForeignKey(
            //    name: "FK_SchoolClasses_Person_StudentId",
            //    table: "SchoolClasses");

            //migrationBuilder.DropIndex(
            //    name: "IX_SchoolClasses_StaffDetailsId",
            //    table: "SchoolClasses");

            //migrationBuilder.DropIndex(
            //    name: "IX_SchoolClasses_StudentId",
            //    table: "SchoolClasses");

            //migrationBuilder.DropColumn(
            //    name: "StaffDetailsId",
            //    table: "SchoolClasses");

            //migrationBuilder.DropColumn(
            //    name: "StudentId",
            //    table: "SchoolClasses");

            //migrationBuilder.DropColumn(
            //    name: "Status",
            //    table: "Events");

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "SchoolClasses",
                type: "longtext",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "longtext")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "ClassLeadershipRoles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Created = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    CreatedBy = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Modified = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    ModifiedBy = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Name = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Description = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClassLeadershipRoles", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "SchoolClassLeaders",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    SchoolClassId = table.Column<int>(type: "int", nullable: false),
                    PersonId = table.Column<int>(type: "int", nullable: false),
                    ClassLeadershipRoleId = table.Column<int>(type: "int", nullable: false),
                    Description = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    StaffDetailsId = table.Column<int>(type: "int", nullable: true),
                    StudentId = table.Column<int>(type: "int", nullable: true),
                    Created = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    CreatedBy = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Modified = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    ModifiedBy = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SchoolClassLeaders", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SchoolClassLeaders_ClassLeadershipRoles_ClassLeadershipRoleId",
                        column: x => x.ClassLeadershipRoleId,
                        principalTable: "ClassLeadershipRoles",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_SchoolClassLeaders_Person_PersonId",
                        column: x => x.PersonId,
                        principalTable: "Person",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_SchoolClassLeaders_Person_StaffDetailsId",
                        column: x => x.StaffDetailsId,
                        principalTable: "Person",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_SchoolClassLeaders_Person_StudentId",
                        column: x => x.StudentId,
                        principalTable: "Person",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_SchoolClassLeaders_SchoolClasses_SchoolClassId",
                        column: x => x.SchoolClassId,
                        principalTable: "SchoolClasses",
                        principalColumn: "Id");
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "269f0cf3-405e-4163-83f3-1b63ebebd62e",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2024, 7, 11, 8, 4, 20, 188, DateTimeKind.Local).AddTicks(9960), new DateTime(2024, 7, 11, 8, 4, 20, 188, DateTimeKind.Local).AddTicks(9962) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "448df289-142c-4959-a912-60733515e1b4",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2024, 7, 11, 8, 4, 20, 188, DateTimeKind.Local).AddTicks(9922), new DateTime(2024, 7, 11, 8, 4, 20, 188, DateTimeKind.Local).AddTicks(9924) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "48c50c3a-9958-453b-b649-4e21af131322",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2024, 7, 11, 8, 4, 20, 188, DateTimeKind.Local).AddTicks(9880), new DateTime(2024, 7, 11, 8, 4, 20, 188, DateTimeKind.Local).AddTicks(9882) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "717d9b15-a428-440c-b26b-08d3bbb68b02",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2024, 7, 11, 8, 4, 20, 188, DateTimeKind.Local).AddTicks(9616), new DateTime(2024, 7, 11, 8, 4, 20, 188, DateTimeKind.Local).AddTicks(9650) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "95ed2407-3e58-4af2-88a4-1c4e96473f68",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2024, 7, 11, 8, 4, 20, 188, DateTimeKind.Local).AddTicks(9759), new DateTime(2024, 7, 11, 8, 4, 20, 188, DateTimeKind.Local).AddTicks(9768) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "97942bee-ef12-4425-8225-4f293d0f36dd",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2024, 7, 11, 8, 4, 20, 189, DateTimeKind.Local).AddTicks(88), new DateTime(2024, 7, 11, 8, 4, 20, 189, DateTimeKind.Local).AddTicks(91) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "cd12b44b-103b-48df-8887-a2bf42e0651e",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2024, 7, 11, 8, 4, 20, 189, DateTimeKind.Local), new DateTime(2024, 7, 11, 8, 4, 20, 189, DateTimeKind.Local).AddTicks(13) });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "7e67d486-af3e-49f1-a109-a2b864b8e0ec",
                columns: new[] { "ConcurrencyStamp", "Created", "Modified", "PasswordHash", "SecurityStamp" },
                values: new object[] { "eba0cdd4-852b-4bf3-bbdf-0052d2bb2e92", new DateTime(2024, 7, 11, 8, 4, 20, 189, DateTimeKind.Local).AddTicks(222), new DateTime(2024, 7, 11, 8, 4, 20, 189, DateTimeKind.Local).AddTicks(225), "AQAAAAIAAYagAAAAEHjYyPHxzZdFplM09VAfBiyQ1DysUkkDyUTloBUMvbH0qCcwkqpHpn1I7lFKrGGT8Q==", "8514fed1-31b9-4416-8218-385c8537c813" });

            migrationBuilder.CreateIndex(
                name: "IX_SchoolClassLeaders_ClassLeadershipRoleId",
                table: "SchoolClassLeaders",
                column: "ClassLeadershipRoleId");

            migrationBuilder.CreateIndex(
                name: "IX_SchoolClassLeaders_PersonId",
                table: "SchoolClassLeaders",
                column: "PersonId");

            migrationBuilder.CreateIndex(
                name: "IX_SchoolClassLeaders_SchoolClassId",
                table: "SchoolClassLeaders",
                column: "SchoolClassId");

            migrationBuilder.CreateIndex(
                name: "IX_SchoolClassLeaders_StaffDetailsId",
                table: "SchoolClassLeaders",
                column: "StaffDetailsId");

            migrationBuilder.CreateIndex(
                name: "IX_SchoolClassLeaders_StudentId",
                table: "SchoolClassLeaders",
                column: "StudentId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SchoolClassLeaders");

            migrationBuilder.DropTable(
                name: "ClassLeadershipRoles");

            migrationBuilder.UpdateData(
                table: "SchoolClasses",
                keyColumn: "Description",
                keyValue: null,
                column: "Description",
                value: "");

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "SchoolClasses",
                type: "longtext",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "longtext",
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<int>(
                name: "StaffDetailsId",
                table: "SchoolClasses",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "StudentId",
                table: "SchoolClasses",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "Status",
                table: "Events",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "269f0cf3-405e-4163-83f3-1b63ebebd62e",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2024, 7, 9, 9, 35, 20, 193, DateTimeKind.Local).AddTicks(3255), new DateTime(2024, 7, 9, 9, 35, 20, 193, DateTimeKind.Local).AddTicks(3257) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "448df289-142c-4959-a912-60733515e1b4",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2024, 7, 9, 9, 35, 20, 193, DateTimeKind.Local).AddTicks(3224), new DateTime(2024, 7, 9, 9, 35, 20, 193, DateTimeKind.Local).AddTicks(3226) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "48c50c3a-9958-453b-b649-4e21af131322",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2024, 7, 9, 9, 35, 20, 193, DateTimeKind.Local).AddTicks(3192), new DateTime(2024, 7, 9, 9, 35, 20, 193, DateTimeKind.Local).AddTicks(3194) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "717d9b15-a428-440c-b26b-08d3bbb68b02",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2024, 7, 9, 9, 35, 20, 193, DateTimeKind.Local).AddTicks(3048), new DateTime(2024, 7, 9, 9, 35, 20, 193, DateTimeKind.Local).AddTicks(3079) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "95ed2407-3e58-4af2-88a4-1c4e96473f68",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2024, 7, 9, 9, 35, 20, 193, DateTimeKind.Local).AddTicks(3146), new DateTime(2024, 7, 9, 9, 35, 20, 193, DateTimeKind.Local).AddTicks(3158) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "97942bee-ef12-4425-8225-4f293d0f36dd",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2024, 7, 9, 9, 35, 20, 193, DateTimeKind.Local).AddTicks(3367), new DateTime(2024, 7, 9, 9, 35, 20, 193, DateTimeKind.Local).AddTicks(3369) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "cd12b44b-103b-48df-8887-a2bf42e0651e",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2024, 7, 9, 9, 35, 20, 193, DateTimeKind.Local).AddTicks(3310), new DateTime(2024, 7, 9, 9, 35, 20, 193, DateTimeKind.Local).AddTicks(3321) });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "7e67d486-af3e-49f1-a109-a2b864b8e0ec",
                columns: new[] { "ConcurrencyStamp", "Created", "Modified", "PasswordHash", "SecurityStamp" },
                values: new object[] { "a3319857-b7dc-43da-a2d3-76ef714d19ab", new DateTime(2024, 7, 9, 9, 35, 20, 193, DateTimeKind.Local).AddTicks(3478), new DateTime(2024, 7, 9, 9, 35, 20, 193, DateTimeKind.Local).AddTicks(3480), "AQAAAAIAAYagAAAAEALhl+8ZJ0FWomZ5gq8f2A84lT61G2B8KzV1N4ztl85ZoSZbanHghbplrQhDwPCLeQ==", "637134d1-f401-4987-a6a9-f51146a39941" });

            migrationBuilder.CreateIndex(
                name: "IX_SchoolClasses_StaffDetailsId",
                table: "SchoolClasses",
                column: "StaffDetailsId");

            migrationBuilder.CreateIndex(
                name: "IX_SchoolClasses_StudentId",
                table: "SchoolClasses",
                column: "StudentId");

            migrationBuilder.AddForeignKey(
                name: "FK_SchoolClasses_Person_StaffDetailsId",
                table: "SchoolClasses",
                column: "StaffDetailsId",
                principalTable: "Person",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_SchoolClasses_Person_StudentId",
                table: "SchoolClasses",
                column: "StudentId",
                principalTable: "Person",
                principalColumn: "Id");
        }
    }
}
