using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SchoolWebApp.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddSessionIdToSpecificOutcome : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Drop existing FK if present (from shadow property)
            migrationBuilder.Sql(@"
                SET @fk_exists = (SELECT COUNT(*) FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS
                    WHERE TABLE_NAME = 'SpecificOutcomes' AND CONSTRAINT_NAME = 'FK_SpecificOutcomes_Sessions_SessionId' AND TABLE_SCHEMA = DATABASE());
                SET @sql = IF(@fk_exists > 0, 'ALTER TABLE SpecificOutcomes DROP FOREIGN KEY FK_SpecificOutcomes_Sessions_SessionId', 'SELECT 1');
                PREPARE stmt FROM @sql;
                EXECUTE stmt;
                DEALLOCATE PREPARE stmt;
            ");

            // Add column if not exists
            migrationBuilder.Sql(@"
                SET @col_exists = (SELECT COUNT(*) FROM INFORMATION_SCHEMA.COLUMNS
                    WHERE TABLE_NAME = 'SpecificOutcomes' AND COLUMN_NAME = 'SessionId' AND TABLE_SCHEMA = DATABASE());
                SET @sql = IF(@col_exists = 0, 'ALTER TABLE SpecificOutcomes ADD COLUMN SessionId int NULL', 'SELECT 1');
                PREPARE stmt FROM @sql;
                EXECUTE stmt;
                DEALLOCATE PREPARE stmt;
            ");

            // Ensure column is nullable
            migrationBuilder.Sql(@"ALTER TABLE SpecificOutcomes MODIFY COLUMN SessionId int NULL;");

            // Nullify invalid SessionId values
            migrationBuilder.Sql(@"
                UPDATE SpecificOutcomes SET SessionId = NULL
                WHERE SessionId IS NOT NULL AND SessionId NOT IN (SELECT Id FROM Sessions);
            ");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "269f0cf3-405e-4163-83f3-1b63ebebd62e",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 8, 21, 13, 47, 425, DateTimeKind.Local).AddTicks(3938), new DateTime(2026, 4, 8, 21, 13, 47, 425, DateTimeKind.Local).AddTicks(3940) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "448df289-142c-4959-a912-60733515e1b4",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 8, 21, 13, 47, 425, DateTimeKind.Local).AddTicks(3892), new DateTime(2026, 4, 8, 21, 13, 47, 425, DateTimeKind.Local).AddTicks(3894) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "48c50c3a-9958-453b-b649-4e21af131322",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 8, 21, 13, 47, 425, DateTimeKind.Local).AddTicks(3834), new DateTime(2026, 4, 8, 21, 13, 47, 425, DateTimeKind.Local).AddTicks(3836) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "717d9b15-a428-440c-b26b-08d3bbb68b02",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 8, 21, 13, 47, 425, DateTimeKind.Local).AddTicks(3630), new DateTime(2026, 4, 8, 21, 13, 47, 425, DateTimeKind.Local).AddTicks(3651) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "95ed2407-3e58-4af2-88a4-1c4e96473f68",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 8, 21, 13, 47, 425, DateTimeKind.Local).AddTicks(3782), new DateTime(2026, 4, 8, 21, 13, 47, 425, DateTimeKind.Local).AddTicks(3784) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "97942bee-ef12-4425-8225-4f293d0f36dd",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 8, 21, 13, 47, 425, DateTimeKind.Local).AddTicks(4047), new DateTime(2026, 4, 8, 21, 13, 47, 425, DateTimeKind.Local).AddTicks(4049) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "cd12b44b-103b-48df-8887-a2bf42e0651e",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 8, 21, 13, 47, 425, DateTimeKind.Local).AddTicks(3986), new DateTime(2026, 4, 8, 21, 13, 47, 425, DateTimeKind.Local).AddTicks(3988) });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "7e67d486-af3e-49f1-a109-a2b864b8e0ec",
                columns: new[] { "ConcurrencyStamp", "Created", "Modified", "PasswordHash", "SecurityStamp" },
                values: new object[] { "dc061620-deed-4cc2-b6b6-9cbec9c3380d", new DateTime(2026, 4, 8, 21, 13, 47, 425, DateTimeKind.Local).AddTicks(4577), new DateTime(2026, 4, 8, 21, 13, 47, 425, DateTimeKind.Local).AddTicks(4580), "AQAAAAIAAYagAAAAEAdSiihVbSqyOiZ0q1Z4VjxGf8wawewohHHPmYmsJSmGXgmF6AMnym7CjGfD+U4eLA==", "d5c3ac24-2506-4374-b90d-50ed32fd08b8" });

            migrationBuilder.UpdateData(
                table: "Designations",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 8, 21, 13, 47, 425, DateTimeKind.Local).AddTicks(4144), new DateTime(2026, 4, 8, 21, 13, 47, 425, DateTimeKind.Local).AddTicks(4146) });

            migrationBuilder.UpdateData(
                table: "EmploymentTypes",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 8, 21, 13, 47, 425, DateTimeKind.Local).AddTicks(4200), new DateTime(2026, 4, 8, 21, 13, 47, 425, DateTimeKind.Local).AddTicks(4202) });

            migrationBuilder.UpdateData(
                table: "Genders",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 8, 21, 13, 47, 425, DateTimeKind.Local).AddTicks(4308), new DateTime(2026, 4, 8, 21, 13, 47, 425, DateTimeKind.Local).AddTicks(4310) });

            migrationBuilder.UpdateData(
                table: "Nationalities",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 8, 21, 13, 47, 425, DateTimeKind.Local).AddTicks(4238), new DateTime(2026, 4, 8, 21, 13, 47, 425, DateTimeKind.Local).AddTicks(4240) });

            migrationBuilder.UpdateData(
                table: "Person",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "DateOfBirth", "EmploymentDate", "EndofEmploymentDate", "Modified" },
                values: new object[] { new DateTime(2026, 4, 8, 21, 13, 47, 425, DateTimeKind.Local).AddTicks(4371), new DateTime(2026, 4, 8, 21, 13, 47, 425, DateTimeKind.Local).AddTicks(4368), new DateTime(2026, 4, 8, 21, 13, 47, 425, DateTimeKind.Local).AddTicks(4364), new DateTime(2026, 4, 8, 21, 13, 47, 425, DateTimeKind.Local).AddTicks(4366), new DateTime(2026, 4, 8, 21, 13, 47, 425, DateTimeKind.Local).AddTicks(4378) });

            migrationBuilder.UpdateData(
                table: "Religions",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 8, 21, 13, 47, 425, DateTimeKind.Local).AddTicks(4274), new DateTime(2026, 4, 8, 21, 13, 47, 425, DateTimeKind.Local).AddTicks(4275) });

            migrationBuilder.UpdateData(
                table: "StaffCategories",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 8, 21, 13, 47, 425, DateTimeKind.Local).AddTicks(4102), new DateTime(2026, 4, 8, 21, 13, 47, 425, DateTimeKind.Local).AddTicks(4105) });

            migrationBuilder.Sql(@"
                SET @idx_exists = (SELECT COUNT(*) FROM INFORMATION_SCHEMA.STATISTICS
                    WHERE TABLE_NAME = 'SpecificOutcomes' AND INDEX_NAME = 'IX_SpecificOutcomes_SessionId' AND TABLE_SCHEMA = DATABASE());
                SET @sql = IF(@idx_exists = 0, 'CREATE INDEX IX_SpecificOutcomes_SessionId ON SpecificOutcomes (SessionId)', 'SELECT 1');
                PREPARE stmt FROM @sql;
                EXECUTE stmt;
                DEALLOCATE PREPARE stmt;
            ");

            migrationBuilder.Sql(@"
                SET @fk_exists = (SELECT COUNT(*) FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS
                    WHERE TABLE_NAME = 'SpecificOutcomes' AND CONSTRAINT_NAME = 'FK_SpecificOutcomes_Sessions_SessionId' AND TABLE_SCHEMA = DATABASE());
                SET @sql = IF(@fk_exists = 0, 'ALTER TABLE SpecificOutcomes ADD CONSTRAINT FK_SpecificOutcomes_Sessions_SessionId FOREIGN KEY (SessionId) REFERENCES Sessions(Id)', 'SELECT 1');
                PREPARE stmt FROM @sql;
                EXECUTE stmt;
                DEALLOCATE PREPARE stmt;
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SpecificOutcomes_Sessions_SessionId",
                table: "SpecificOutcomes");

            migrationBuilder.DropIndex(
                name: "IX_SpecificOutcomes_SessionId",
                table: "SpecificOutcomes");

            migrationBuilder.DropColumn(
                name: "SessionId",
                table: "SpecificOutcomes");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "269f0cf3-405e-4163-83f3-1b63ebebd62e",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 8, 19, 43, 30, 705, DateTimeKind.Local).AddTicks(4376), new DateTime(2026, 4, 8, 19, 43, 30, 705, DateTimeKind.Local).AddTicks(4377) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "448df289-142c-4959-a912-60733515e1b4",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 8, 19, 43, 30, 705, DateTimeKind.Local).AddTicks(4331), new DateTime(2026, 4, 8, 19, 43, 30, 705, DateTimeKind.Local).AddTicks(4333) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "48c50c3a-9958-453b-b649-4e21af131322",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 8, 19, 43, 30, 705, DateTimeKind.Local).AddTicks(4277), new DateTime(2026, 4, 8, 19, 43, 30, 705, DateTimeKind.Local).AddTicks(4278) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "717d9b15-a428-440c-b26b-08d3bbb68b02",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 8, 19, 43, 30, 705, DateTimeKind.Local).AddTicks(4110), new DateTime(2026, 4, 8, 19, 43, 30, 705, DateTimeKind.Local).AddTicks(4127) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "95ed2407-3e58-4af2-88a4-1c4e96473f68",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 8, 19, 43, 30, 705, DateTimeKind.Local).AddTicks(4228), new DateTime(2026, 4, 8, 19, 43, 30, 705, DateTimeKind.Local).AddTicks(4230) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "97942bee-ef12-4425-8225-4f293d0f36dd",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 8, 19, 43, 30, 705, DateTimeKind.Local).AddTicks(4464), new DateTime(2026, 4, 8, 19, 43, 30, 705, DateTimeKind.Local).AddTicks(4465) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "cd12b44b-103b-48df-8887-a2bf42e0651e",
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 8, 19, 43, 30, 705, DateTimeKind.Local).AddTicks(4420), new DateTime(2026, 4, 8, 19, 43, 30, 705, DateTimeKind.Local).AddTicks(4422) });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "7e67d486-af3e-49f1-a109-a2b864b8e0ec",
                columns: new[] { "ConcurrencyStamp", "Created", "Modified", "PasswordHash", "SecurityStamp" },
                values: new object[] { "805a85ae-34d8-468a-9df9-4e7073a0bf36", new DateTime(2026, 4, 8, 19, 43, 30, 705, DateTimeKind.Local).AddTicks(4896), new DateTime(2026, 4, 8, 19, 43, 30, 705, DateTimeKind.Local).AddTicks(4899), "AQAAAAIAAYagAAAAEOfg9jo+IoNrRXeVZetotgswuPi/cQiwWA9b9+SNvuCcfxLkzoIxZQqW0yXozhvmiA==", "dd61c7d4-bd8e-4735-80a5-67aa44f828b1" });

            migrationBuilder.UpdateData(
                table: "Designations",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 8, 19, 43, 30, 705, DateTimeKind.Local).AddTicks(4558), new DateTime(2026, 4, 8, 19, 43, 30, 705, DateTimeKind.Local).AddTicks(4559) });

            migrationBuilder.UpdateData(
                table: "EmploymentTypes",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 8, 19, 43, 30, 705, DateTimeKind.Local).AddTicks(4598), new DateTime(2026, 4, 8, 19, 43, 30, 705, DateTimeKind.Local).AddTicks(4600) });

            migrationBuilder.UpdateData(
                table: "Genders",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 8, 19, 43, 30, 705, DateTimeKind.Local).AddTicks(4716), new DateTime(2026, 4, 8, 19, 43, 30, 705, DateTimeKind.Local).AddTicks(4718) });

            migrationBuilder.UpdateData(
                table: "Nationalities",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 8, 19, 43, 30, 705, DateTimeKind.Local).AddTicks(4641), new DateTime(2026, 4, 8, 19, 43, 30, 705, DateTimeKind.Local).AddTicks(4643) });

            migrationBuilder.UpdateData(
                table: "Person",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "DateOfBirth", "EmploymentDate", "EndofEmploymentDate", "Modified" },
                values: new object[] { new DateTime(2026, 4, 8, 19, 43, 30, 705, DateTimeKind.Local).AddTicks(4795), new DateTime(2026, 4, 8, 19, 43, 30, 705, DateTimeKind.Local).AddTicks(4793), new DateTime(2026, 4, 8, 19, 43, 30, 705, DateTimeKind.Local).AddTicks(4788), new DateTime(2026, 4, 8, 19, 43, 30, 705, DateTimeKind.Local).AddTicks(4790), new DateTime(2026, 4, 8, 19, 43, 30, 705, DateTimeKind.Local).AddTicks(4802) });

            migrationBuilder.UpdateData(
                table: "Religions",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 8, 19, 43, 30, 705, DateTimeKind.Local).AddTicks(4681), new DateTime(2026, 4, 8, 19, 43, 30, 705, DateTimeKind.Local).AddTicks(4682) });

            migrationBuilder.UpdateData(
                table: "StaffCategories",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 8, 19, 43, 30, 705, DateTimeKind.Local).AddTicks(4523), new DateTime(2026, 4, 8, 19, 43, 30, 705, DateTimeKind.Local).AddTicks(4525) });
        }
    }
}
