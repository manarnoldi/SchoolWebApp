using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SchoolWebApp.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddedLearningExperiencesnavtoSubStrand : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "LearningExperience",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    SubStrandId = table.Column<int>(type: "int", nullable: false),
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
                    table.PrimaryKey("PK_LearningExperience", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LearningExperience_SubStrands_SubStrandId",
                        column: x => x.SubStrandId,
                        principalTable: "SubStrands",
                        principalColumn: "Id");
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "269f0cf3-405e-4163-83f3-1b63ebebd62e",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 8, 10, 37, 26, 233, DateTimeKind.Local).AddTicks(4767), new DateTime(2026, 4, 8, 10, 37, 26, 233, DateTimeKind.Local).AddTicks(4769) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "448df289-142c-4959-a912-60733515e1b4",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 8, 10, 37, 26, 233, DateTimeKind.Local).AddTicks(4722), new DateTime(2026, 4, 8, 10, 37, 26, 233, DateTimeKind.Local).AddTicks(4724) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "48c50c3a-9958-453b-b649-4e21af131322",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 8, 10, 37, 26, 233, DateTimeKind.Local).AddTicks(4636), new DateTime(2026, 4, 8, 10, 37, 26, 233, DateTimeKind.Local).AddTicks(4638) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "717d9b15-a428-440c-b26b-08d3bbb68b02",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 8, 10, 37, 26, 233, DateTimeKind.Local).AddTicks(4454), new DateTime(2026, 4, 8, 10, 37, 26, 233, DateTimeKind.Local).AddTicks(4471) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "95ed2407-3e58-4af2-88a4-1c4e96473f68",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 8, 10, 37, 26, 233, DateTimeKind.Local).AddTicks(4573), new DateTime(2026, 4, 8, 10, 37, 26, 233, DateTimeKind.Local).AddTicks(4576) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "97942bee-ef12-4425-8225-4f293d0f36dd",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 8, 10, 37, 26, 233, DateTimeKind.Local).AddTicks(4858), new DateTime(2026, 4, 8, 10, 37, 26, 233, DateTimeKind.Local).AddTicks(4859) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "cd12b44b-103b-48df-8887-a2bf42e0651e",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 8, 10, 37, 26, 233, DateTimeKind.Local).AddTicks(4814), new DateTime(2026, 4, 8, 10, 37, 26, 233, DateTimeKind.Local).AddTicks(4816) });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "7e67d486-af3e-49f1-a109-a2b864b8e0ec",
                columns: new[] { "ConcurrencyStamp", "Created", "Modified", "PasswordHash", "SecurityStamp" },
                values: new object[] { "9df7d960-f901-4d83-8543-21d0001b15aa", new DateTime(2026, 4, 8, 10, 37, 26, 233, DateTimeKind.Local).AddTicks(5326), new DateTime(2026, 4, 8, 10, 37, 26, 233, DateTimeKind.Local).AddTicks(5328), "AQAAAAIAAYagAAAAEEy8o+jpd+fXLm+ivKPu3n3TzU07vLHCvSuOZM4+YOJp6Zcv73RkhwHjRWWnAEk/Eg==", "242aaa3b-1972-47c9-85d6-82dd0608a682" });

            migrationBuilder.UpdateData(
                table: "Designations",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 8, 10, 37, 26, 233, DateTimeKind.Local).AddTicks(4964), new DateTime(2026, 4, 8, 10, 37, 26, 233, DateTimeKind.Local).AddTicks(4965) });

            migrationBuilder.UpdateData(
                table: "EmploymentTypes",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 8, 10, 37, 26, 233, DateTimeKind.Local).AddTicks(5014), new DateTime(2026, 4, 8, 10, 37, 26, 233, DateTimeKind.Local).AddTicks(5015) });

            migrationBuilder.UpdateData(
                table: "Genders",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 8, 10, 37, 26, 233, DateTimeKind.Local).AddTicks(5138), new DateTime(2026, 4, 8, 10, 37, 26, 233, DateTimeKind.Local).AddTicks(5140) });

            migrationBuilder.UpdateData(
                table: "Nationalities",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 8, 10, 37, 26, 233, DateTimeKind.Local).AddTicks(5057), new DateTime(2026, 4, 8, 10, 37, 26, 233, DateTimeKind.Local).AddTicks(5058) });

            migrationBuilder.UpdateData(
                table: "Person",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "DateOfBirth", "EmploymentDate", "EndofEmploymentDate", "Modified" },
                values: new object[] { new DateTime(2026, 4, 8, 10, 37, 26, 233, DateTimeKind.Local).AddTicks(5209), new DateTime(2026, 4, 8, 10, 37, 26, 233, DateTimeKind.Local).AddTicks(5205), new DateTime(2026, 4, 8, 10, 37, 26, 233, DateTimeKind.Local).AddTicks(5200), new DateTime(2026, 4, 8, 10, 37, 26, 233, DateTimeKind.Local).AddTicks(5202), new DateTime(2026, 4, 8, 10, 37, 26, 233, DateTimeKind.Local).AddTicks(5219) });

            migrationBuilder.UpdateData(
                table: "Religions",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 8, 10, 37, 26, 233, DateTimeKind.Local).AddTicks(5099), new DateTime(2026, 4, 8, 10, 37, 26, 233, DateTimeKind.Local).AddTicks(5101) });

            migrationBuilder.UpdateData(
                table: "StaffCategories",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 8, 10, 37, 26, 233, DateTimeKind.Local).AddTicks(4921), new DateTime(2026, 4, 8, 10, 37, 26, 233, DateTimeKind.Local).AddTicks(4924) });

            migrationBuilder.CreateIndex(
                name: "IX_LearningExperience_SubStrandId",
                table: "LearningExperience",
                column: "SubStrandId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LearningExperience");

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
        }
    }
}
