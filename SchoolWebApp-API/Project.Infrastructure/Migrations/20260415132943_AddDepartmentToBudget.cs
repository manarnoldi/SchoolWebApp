using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SchoolWebApp.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddDepartmentToBudget : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "AcademicYearId",
                table: "Budgets",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            // Column may already exist from a previously-failed attempt; only add if missing
            migrationBuilder.Sql(@"
                SET @col_exists := (SELECT COUNT(*) FROM information_schema.columns
                    WHERE table_schema = DATABASE() AND table_name = 'Budgets' AND column_name = 'DepartmentId');
                SET @sql := IF(@col_exists = 0,
                    'ALTER TABLE `Budgets` ADD COLUMN `DepartmentId` int NOT NULL DEFAULT 0',
                    'SELECT 1');
                PREPARE stmt FROM @sql; EXECUTE stmt; DEALLOCATE PREPARE stmt;
            ");

            // Assign an existing Department to any pre-existing Budgets so the FK can be enforced
            migrationBuilder.Sql(@"
                UPDATE `Budgets`
                SET `DepartmentId` = COALESCE((SELECT MIN(`Id`) FROM `Departments`), 0)
                WHERE `DepartmentId` = 0;
            ");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 15, 16, 29, 42, 792, DateTimeKind.Local).AddTicks(3573), new DateTime(2026, 4, 15, 16, 29, 42, 792, DateTimeKind.Local).AddTicks(3587) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 15, 16, 29, 42, 792, DateTimeKind.Local).AddTicks(3618), new DateTime(2026, 4, 15, 16, 29, 42, 792, DateTimeKind.Local).AddTicks(3619) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 15, 16, 29, 42, 792, DateTimeKind.Local).AddTicks(3634), new DateTime(2026, 4, 15, 16, 29, 42, 792, DateTimeKind.Local).AddTicks(3635) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 15, 16, 29, 42, 792, DateTimeKind.Local).AddTicks(3649), new DateTime(2026, 4, 15, 16, 29, 42, 792, DateTimeKind.Local).AddTicks(3650) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 15, 16, 29, 42, 792, DateTimeKind.Local).AddTicks(3664), new DateTime(2026, 4, 15, 16, 29, 42, 792, DateTimeKind.Local).AddTicks(3665) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 6,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 15, 16, 29, 42, 792, DateTimeKind.Local).AddTicks(3684), new DateTime(2026, 4, 15, 16, 29, 42, 792, DateTimeKind.Local).AddTicks(3685) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 7,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 15, 16, 29, 42, 792, DateTimeKind.Local).AddTicks(3702), new DateTime(2026, 4, 15, 16, 29, 42, 792, DateTimeKind.Local).AddTicks(3702) });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "ConcurrencyStamp", "Created", "Modified", "PasswordHash" },
                values: new object[] { "b335c255-f35e-40c3-bc7e-736f4671136b", new DateTime(2026, 4, 15, 16, 29, 42, 792, DateTimeKind.Local).AddTicks(4047), new DateTime(2026, 4, 15, 16, 29, 42, 792, DateTimeKind.Local).AddTicks(4048), "AQAAAAIAAYagAAAAEAG4Dwb5m8BUHFOOyNI8JFujJPQjkWzWh2xmM1hh/3DSAn8MwWfFw6+MmyAnC2kOHw==" });

            migrationBuilder.UpdateData(
                table: "Designations",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 15, 16, 29, 42, 792, DateTimeKind.Local).AddTicks(3787), new DateTime(2026, 4, 15, 16, 29, 42, 792, DateTimeKind.Local).AddTicks(3788) });

            migrationBuilder.UpdateData(
                table: "EmploymentTypes",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 15, 16, 29, 42, 792, DateTimeKind.Local).AddTicks(3808), new DateTime(2026, 4, 15, 16, 29, 42, 792, DateTimeKind.Local).AddTicks(3809) });

            migrationBuilder.UpdateData(
                table: "Genders",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 15, 16, 29, 42, 792, DateTimeKind.Local).AddTicks(3871), new DateTime(2026, 4, 15, 16, 29, 42, 792, DateTimeKind.Local).AddTicks(3871) });

            migrationBuilder.UpdateData(
                table: "Nationalities",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 15, 16, 29, 42, 792, DateTimeKind.Local).AddTicks(3828), new DateTime(2026, 4, 15, 16, 29, 42, 792, DateTimeKind.Local).AddTicks(3829) });

            migrationBuilder.UpdateData(
                table: "Person",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "DateOfBirth", "EmploymentDate", "EndofEmploymentDate", "Modified" },
                values: new object[] { new DateTime(2026, 4, 15, 16, 29, 42, 792, DateTimeKind.Local).AddTicks(3924), new DateTime(2026, 4, 15, 16, 29, 42, 792, DateTimeKind.Local).AddTicks(3921), new DateTime(2026, 4, 15, 16, 29, 42, 792, DateTimeKind.Local).AddTicks(3917), new DateTime(2026, 4, 15, 16, 29, 42, 792, DateTimeKind.Local).AddTicks(3918), new DateTime(2026, 4, 15, 16, 29, 42, 792, DateTimeKind.Local).AddTicks(3929) });

            migrationBuilder.UpdateData(
                table: "Religions",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 15, 16, 29, 42, 792, DateTimeKind.Local).AddTicks(3854), new DateTime(2026, 4, 15, 16, 29, 42, 792, DateTimeKind.Local).AddTicks(3855) });

            migrationBuilder.UpdateData(
                table: "StaffCategories",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 15, 16, 29, 42, 792, DateTimeKind.Local).AddTicks(3741), new DateTime(2026, 4, 15, 16, 29, 42, 792, DateTimeKind.Local).AddTicks(3743) });

            migrationBuilder.Sql(@"
                SET @idx_exists := (SELECT COUNT(*) FROM information_schema.statistics
                    WHERE table_schema = DATABASE() AND table_name = 'Budgets' AND index_name = 'IX_Budgets_DepartmentId');
                SET @sql := IF(@idx_exists = 0,
                    'CREATE INDEX `IX_Budgets_DepartmentId` ON `Budgets`(`DepartmentId`)',
                    'SELECT 1');
                PREPARE stmt FROM @sql; EXECUTE stmt; DEALLOCATE PREPARE stmt;
            ");

            migrationBuilder.Sql(@"
                SET @fk_exists := (SELECT COUNT(*) FROM information_schema.table_constraints
                    WHERE table_schema = DATABASE() AND table_name = 'Budgets' AND constraint_name = 'FK_Budgets_Departments_DepartmentId');
                SET @sql := IF(@fk_exists = 0,
                    'ALTER TABLE `Budgets` ADD CONSTRAINT `FK_Budgets_Departments_DepartmentId` FOREIGN KEY (`DepartmentId`) REFERENCES `Departments`(`Id`)',
                    'SELECT 1');
                PREPARE stmt FROM @sql; EXECUTE stmt; DEALLOCATE PREPARE stmt;
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Budgets_Departments_DepartmentId",
                table: "Budgets");

            migrationBuilder.DropIndex(
                name: "IX_Budgets_DepartmentId",
                table: "Budgets");

            migrationBuilder.DropColumn(
                name: "DepartmentId",
                table: "Budgets");

            migrationBuilder.AlterColumn<int>(
                name: "AcademicYearId",
                table: "Budgets",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 15, 16, 16, 26, 566, DateTimeKind.Local).AddTicks(6694), new DateTime(2026, 4, 15, 16, 16, 26, 566, DateTimeKind.Local).AddTicks(6715) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 15, 16, 16, 26, 566, DateTimeKind.Local).AddTicks(6834), new DateTime(2026, 4, 15, 16, 16, 26, 566, DateTimeKind.Local).AddTicks(6836) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 15, 16, 16, 26, 566, DateTimeKind.Local).AddTicks(6885), new DateTime(2026, 4, 15, 16, 16, 26, 566, DateTimeKind.Local).AddTicks(6887) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 15, 16, 16, 26, 566, DateTimeKind.Local).AddTicks(6945), new DateTime(2026, 4, 15, 16, 16, 26, 566, DateTimeKind.Local).AddTicks(6948) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 15, 16, 16, 26, 566, DateTimeKind.Local).AddTicks(6991), new DateTime(2026, 4, 15, 16, 16, 26, 566, DateTimeKind.Local).AddTicks(6994) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 6,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 15, 16, 16, 26, 566, DateTimeKind.Local).AddTicks(7045), new DateTime(2026, 4, 15, 16, 16, 26, 566, DateTimeKind.Local).AddTicks(7047) });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 7,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 15, 16, 16, 26, 566, DateTimeKind.Local).AddTicks(7098), new DateTime(2026, 4, 15, 16, 16, 26, 566, DateTimeKind.Local).AddTicks(7100) });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "ConcurrencyStamp", "Created", "Modified", "PasswordHash" },
                values: new object[] { "638285de-01a5-4f00-a616-373b8b7989b3", new DateTime(2026, 4, 15, 16, 16, 26, 566, DateTimeKind.Local).AddTicks(7674), new DateTime(2026, 4, 15, 16, 16, 26, 566, DateTimeKind.Local).AddTicks(7676), "AQAAAAIAAYagAAAAELgCiYCJkr3K740oxFRyI4IK14YURMfh2xKPXr5dcF1+JhDg6XnEk4pWzp2uly68dA==" });

            migrationBuilder.UpdateData(
                table: "Designations",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 15, 16, 16, 26, 566, DateTimeKind.Local).AddTicks(7235), new DateTime(2026, 4, 15, 16, 16, 26, 566, DateTimeKind.Local).AddTicks(7237) });

            migrationBuilder.UpdateData(
                table: "EmploymentTypes",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 15, 16, 16, 26, 566, DateTimeKind.Local).AddTicks(7284), new DateTime(2026, 4, 15, 16, 16, 26, 566, DateTimeKind.Local).AddTicks(7286) });

            migrationBuilder.UpdateData(
                table: "Genders",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 15, 16, 16, 26, 566, DateTimeKind.Local).AddTicks(7420), new DateTime(2026, 4, 15, 16, 16, 26, 566, DateTimeKind.Local).AddTicks(7422) });

            migrationBuilder.UpdateData(
                table: "Nationalities",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 15, 16, 16, 26, 566, DateTimeKind.Local).AddTicks(7329), new DateTime(2026, 4, 15, 16, 16, 26, 566, DateTimeKind.Local).AddTicks(7331) });

            migrationBuilder.UpdateData(
                table: "Person",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "DateOfBirth", "EmploymentDate", "EndofEmploymentDate", "Modified" },
                values: new object[] { new DateTime(2026, 4, 15, 16, 16, 26, 566, DateTimeKind.Local).AddTicks(7496), new DateTime(2026, 4, 15, 16, 16, 26, 566, DateTimeKind.Local).AddTicks(7491), new DateTime(2026, 4, 15, 16, 16, 26, 566, DateTimeKind.Local).AddTicks(7486), new DateTime(2026, 4, 15, 16, 16, 26, 566, DateTimeKind.Local).AddTicks(7488), new DateTime(2026, 4, 15, 16, 16, 26, 566, DateTimeKind.Local).AddTicks(7506) });

            migrationBuilder.UpdateData(
                table: "Religions",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 15, 16, 16, 26, 566, DateTimeKind.Local).AddTicks(7381), new DateTime(2026, 4, 15, 16, 16, 26, 566, DateTimeKind.Local).AddTicks(7383) });

            migrationBuilder.UpdateData(
                table: "StaffCategories",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 15, 16, 16, 26, 566, DateTimeKind.Local).AddTicks(7161), new DateTime(2026, 4, 15, 16, 16, 26, 566, DateTimeKind.Local).AddTicks(7163) });
        }
    }
}
