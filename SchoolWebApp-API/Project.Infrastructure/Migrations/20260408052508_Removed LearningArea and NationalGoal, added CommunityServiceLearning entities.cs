using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SchoolWebApp.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RemovedLearningAreaandNationalGoaladdedCommunityServiceLearningentities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CommunityServiceActivities",
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
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Rank = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CommunityServiceActivities", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "StudentCommunityServiceActivities",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    StudentId = table.Column<int>(type: "int", nullable: false),
                    CommunityServiceActivityId = table.Column<int>(type: "int", nullable: false),
                    SessionId = table.Column<int>(type: "int", nullable: false),
                    AcademicYearId = table.Column<int>(type: "int", nullable: false),
                    Description = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Created = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    CreatedBy = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Modified = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    ModifiedBy = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StudentCommunityServiceActivities", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StudentCommunityServiceActivities_AcademicYears_AcademicYear~",
                        column: x => x.AcademicYearId,
                        principalTable: "AcademicYears",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_StudentCommunityServiceActivities_CommunityServiceActivities~",
                        column: x => x.CommunityServiceActivityId,
                        principalTable: "CommunityServiceActivities",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_StudentCommunityServiceActivities_Person_StudentId",
                        column: x => x.StudentId,
                        principalTable: "Person",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_StudentCommunityServiceActivities_Sessions_SessionId",
                        column: x => x.SessionId,
                        principalTable: "Sessions",
                        principalColumn: "Id");
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "269f0cf3-405e-4163-83f3-1b63ebebd62e",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 8, 8, 25, 5, 933, DateTimeKind.Local).AddTicks(2932), new DateTime(2026, 4, 8, 8, 25, 5, 933, DateTimeKind.Local).AddTicks(2933) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "448df289-142c-4959-a912-60733515e1b4",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 8, 8, 25, 5, 933, DateTimeKind.Local).AddTicks(2915), new DateTime(2026, 4, 8, 8, 25, 5, 933, DateTimeKind.Local).AddTicks(2916) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "48c50c3a-9958-453b-b649-4e21af131322",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 8, 8, 25, 5, 933, DateTimeKind.Local).AddTicks(2898), new DateTime(2026, 4, 8, 8, 25, 5, 933, DateTimeKind.Local).AddTicks(2898) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "717d9b15-a428-440c-b26b-08d3bbb68b02",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 8, 8, 25, 5, 933, DateTimeKind.Local).AddTicks(2812), new DateTime(2026, 4, 8, 8, 25, 5, 933, DateTimeKind.Local).AddTicks(2830) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "95ed2407-3e58-4af2-88a4-1c4e96473f68",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 8, 8, 25, 5, 933, DateTimeKind.Local).AddTicks(2878), new DateTime(2026, 4, 8, 8, 25, 5, 933, DateTimeKind.Local).AddTicks(2879) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "97942bee-ef12-4425-8225-4f293d0f36dd",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 8, 8, 25, 5, 933, DateTimeKind.Local).AddTicks(2972), new DateTime(2026, 4, 8, 8, 25, 5, 933, DateTimeKind.Local).AddTicks(2972) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "cd12b44b-103b-48df-8887-a2bf42e0651e",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 8, 8, 25, 5, 933, DateTimeKind.Local).AddTicks(2955), new DateTime(2026, 4, 8, 8, 25, 5, 933, DateTimeKind.Local).AddTicks(2956) });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "7e67d486-af3e-49f1-a109-a2b864b8e0ec",
                columns: new[] { "ConcurrencyStamp", "Created", "Modified", "PasswordHash", "SecurityStamp" },
                values: new object[] { "2e8f164c-fea5-4a41-9715-6025cf1078d8", new DateTime(2026, 4, 8, 8, 25, 5, 933, DateTimeKind.Local).AddTicks(3290), new DateTime(2026, 4, 8, 8, 25, 5, 933, DateTimeKind.Local).AddTicks(3292), "AQAAAAIAAYagAAAAEKMVUAlvh7SVoHkEp/ahlrR4ZPDLKRknpX7Prj+vUnmDD7YLipE5m3JoHydzMiysww==", "69b07e20-9ed8-4d23-8b69-a2a363e2a098" });

            migrationBuilder.UpdateData(
                table: "Designations",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 8, 8, 25, 5, 933, DateTimeKind.Local).AddTicks(3029), new DateTime(2026, 4, 8, 8, 25, 5, 933, DateTimeKind.Local).AddTicks(3030) });

            migrationBuilder.UpdateData(
                table: "EmploymentTypes",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 8, 8, 25, 5, 933, DateTimeKind.Local).AddTicks(3049), new DateTime(2026, 4, 8, 8, 25, 5, 933, DateTimeKind.Local).AddTicks(3049) });

            migrationBuilder.UpdateData(
                table: "Genders",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 8, 8, 25, 5, 933, DateTimeKind.Local).AddTicks(3112), new DateTime(2026, 4, 8, 8, 25, 5, 933, DateTimeKind.Local).AddTicks(3112) });

            migrationBuilder.UpdateData(
                table: "Nationalities",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 8, 8, 25, 5, 933, DateTimeKind.Local).AddTicks(3074), new DateTime(2026, 4, 8, 8, 25, 5, 933, DateTimeKind.Local).AddTicks(3075) });

            migrationBuilder.UpdateData(
                table: "Person",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "DateOfBirth", "EmploymentDate", "EndofEmploymentDate", "Modified" },
                values: new object[] { new DateTime(2026, 4, 8, 8, 25, 5, 933, DateTimeKind.Local).AddTicks(3144), new DateTime(2026, 4, 8, 8, 25, 5, 933, DateTimeKind.Local).AddTicks(3142), new DateTime(2026, 4, 8, 8, 25, 5, 933, DateTimeKind.Local).AddTicks(3139), new DateTime(2026, 4, 8, 8, 25, 5, 933, DateTimeKind.Local).AddTicks(3139), new DateTime(2026, 4, 8, 8, 25, 5, 933, DateTimeKind.Local).AddTicks(3148) });

            migrationBuilder.UpdateData(
                table: "Religions",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 8, 8, 25, 5, 933, DateTimeKind.Local).AddTicks(3093), new DateTime(2026, 4, 8, 8, 25, 5, 933, DateTimeKind.Local).AddTicks(3094) });

            migrationBuilder.UpdateData(
                table: "StaffCategories",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 8, 8, 25, 5, 933, DateTimeKind.Local).AddTicks(3008), new DateTime(2026, 4, 8, 8, 25, 5, 933, DateTimeKind.Local).AddTicks(3011) });

            migrationBuilder.CreateIndex(
                name: "IX_StudentCommunityServiceActivities_AcademicYearId",
                table: "StudentCommunityServiceActivities",
                column: "AcademicYearId");

            migrationBuilder.CreateIndex(
                name: "IX_StudentCommunityServiceActivities_CommunityServiceActivityId",
                table: "StudentCommunityServiceActivities",
                column: "CommunityServiceActivityId");

            migrationBuilder.CreateIndex(
                name: "IX_StudentCommunityServiceActivities_SessionId",
                table: "StudentCommunityServiceActivities",
                column: "SessionId");

            migrationBuilder.CreateIndex(
                name: "IX_StudentCommunityServiceActivities_StudentId",
                table: "StudentCommunityServiceActivities",
                column: "StudentId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "StudentCommunityServiceActivities");

            migrationBuilder.DropTable(
                name: "CommunityServiceActivities");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "269f0cf3-405e-4163-83f3-1b63ebebd62e",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 7, 18, 17, 9, 248, DateTimeKind.Local).AddTicks(2933), new DateTime(2026, 4, 7, 18, 17, 9, 248, DateTimeKind.Local).AddTicks(2937) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "448df289-142c-4959-a912-60733515e1b4",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 7, 18, 17, 9, 248, DateTimeKind.Local).AddTicks(2820), new DateTime(2026, 4, 7, 18, 17, 9, 248, DateTimeKind.Local).AddTicks(2824) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "48c50c3a-9958-453b-b649-4e21af131322",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 7, 18, 17, 9, 248, DateTimeKind.Local).AddTicks(2725), new DateTime(2026, 4, 7, 18, 17, 9, 248, DateTimeKind.Local).AddTicks(2729) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "717d9b15-a428-440c-b26b-08d3bbb68b02",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 7, 18, 17, 9, 248, DateTimeKind.Local).AddTicks(2337), new DateTime(2026, 4, 7, 18, 17, 9, 248, DateTimeKind.Local).AddTicks(2369) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "95ed2407-3e58-4af2-88a4-1c4e96473f68",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 7, 18, 17, 9, 248, DateTimeKind.Local).AddTicks(2614), new DateTime(2026, 4, 7, 18, 17, 9, 248, DateTimeKind.Local).AddTicks(2619) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "97942bee-ef12-4425-8225-4f293d0f36dd",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 7, 18, 17, 9, 248, DateTimeKind.Local).AddTicks(3128), new DateTime(2026, 4, 7, 18, 17, 9, 248, DateTimeKind.Local).AddTicks(3132) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "cd12b44b-103b-48df-8887-a2bf42e0651e",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 7, 18, 17, 9, 248, DateTimeKind.Local).AddTicks(3033), new DateTime(2026, 4, 7, 18, 17, 9, 248, DateTimeKind.Local).AddTicks(3037) });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "7e67d486-af3e-49f1-a109-a2b864b8e0ec",
                columns: new[] { "ConcurrencyStamp", "Created", "Modified", "PasswordHash", "SecurityStamp" },
                values: new object[] { "4566e9eb-5bbd-4751-9cd2-84dad3c2522f", new DateTime(2026, 4, 7, 18, 17, 9, 248, DateTimeKind.Local).AddTicks(4035), new DateTime(2026, 4, 7, 18, 17, 9, 248, DateTimeKind.Local).AddTicks(4041), "AQAAAAIAAYagAAAAEJLjJbVvm/I2lmNgFXLkPAEsxNfAfTjOuus2UdS+V6pVvMP6jLjB7rAUJFqZM3Eo7w==", "80408627-f9c2-4fce-b9ff-9fbd62f6cbcc" });

            migrationBuilder.UpdateData(
                table: "Designations",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 7, 18, 17, 9, 248, DateTimeKind.Local).AddTicks(3297), new DateTime(2026, 4, 7, 18, 17, 9, 248, DateTimeKind.Local).AddTicks(3301) });

            migrationBuilder.UpdateData(
                table: "EmploymentTypes",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 7, 18, 17, 9, 248, DateTimeKind.Local).AddTicks(3392), new DateTime(2026, 4, 7, 18, 17, 9, 248, DateTimeKind.Local).AddTicks(3396) });

            migrationBuilder.UpdateData(
                table: "Genders",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 7, 18, 17, 9, 248, DateTimeKind.Local).AddTicks(3604), new DateTime(2026, 4, 7, 18, 17, 9, 248, DateTimeKind.Local).AddTicks(3607) });

            migrationBuilder.UpdateData(
                table: "Nationalities",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 7, 18, 17, 9, 248, DateTimeKind.Local).AddTicks(3473), new DateTime(2026, 4, 7, 18, 17, 9, 248, DateTimeKind.Local).AddTicks(3477) });

            migrationBuilder.UpdateData(
                table: "Person",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "DateOfBirth", "EmploymentDate", "EndofEmploymentDate", "Modified" },
                values: new object[] { new DateTime(2026, 4, 7, 18, 17, 9, 248, DateTimeKind.Local).AddTicks(3705), new DateTime(2026, 4, 7, 18, 17, 9, 248, DateTimeKind.Local).AddTicks(3700), new DateTime(2026, 4, 7, 18, 17, 9, 248, DateTimeKind.Local).AddTicks(3692), new DateTime(2026, 4, 7, 18, 17, 9, 248, DateTimeKind.Local).AddTicks(3695), new DateTime(2026, 4, 7, 18, 17, 9, 248, DateTimeKind.Local).AddTicks(3714) });

            migrationBuilder.UpdateData(
                table: "Religions",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 7, 18, 17, 9, 248, DateTimeKind.Local).AddTicks(3536), new DateTime(2026, 4, 7, 18, 17, 9, 248, DateTimeKind.Local).AddTicks(3540) });

            migrationBuilder.UpdateData(
                table: "StaffCategories",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 7, 18, 17, 9, 248, DateTimeKind.Local).AddTicks(3221), new DateTime(2026, 4, 7, 18, 17, 9, 248, DateTimeKind.Local).AddTicks(3226) });
        }
    }
}
