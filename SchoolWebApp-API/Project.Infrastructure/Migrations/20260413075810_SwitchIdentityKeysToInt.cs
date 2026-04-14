using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace SchoolWebApp.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class SwitchIdentityKeysToInt : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Drop all Identity tables and recreate with int keys
            migrationBuilder.Sql("SET FOREIGN_KEY_CHECKS = 0;");
            migrationBuilder.Sql("DROP TABLE IF EXISTS `AspNetUserTokens`;");
            migrationBuilder.Sql("DROP TABLE IF EXISTS `AspNetUserLogins`;");
            migrationBuilder.Sql("DROP TABLE IF EXISTS `AspNetUserClaims`;");
            migrationBuilder.Sql("DROP TABLE IF EXISTS `AspNetUserRoles`;");
            migrationBuilder.Sql("DROP TABLE IF EXISTS `AspNetRoleClaims`;");
            migrationBuilder.Sql("DROP TABLE IF EXISTS `AspNetUsers`;");
            migrationBuilder.Sql("DROP TABLE IF EXISTS `AspNetRoles`;");

            // Change ToDoLists.UserId from varchar to int
            migrationBuilder.Sql("UPDATE `ToDoLists` SET `UserId` = NULL;");
            migrationBuilder.Sql("ALTER TABLE `ToDoLists` MODIFY COLUMN `UserId` int NULL;");

            // Recreate AspNetRoles with int PK
            migrationBuilder.Sql(@"
                CREATE TABLE `AspNetRoles` (
                    `Id` int NOT NULL AUTO_INCREMENT,
                    `Name` varchar(256) NULL,
                    `NormalizedName` varchar(256) NULL,
                    `ConcurrencyStamp` longtext NULL,
                    `Created` datetime(6) NULL,
                    `CreatedBy` varchar(255) NULL,
                    `Modified` datetime(6) NULL,
                    `ModifiedBy` varchar(255) NULL,
                    PRIMARY KEY (`Id`)
                ) CHARACTER SET=utf8mb4;");

            // Recreate AspNetUsers with int PK
            migrationBuilder.Sql(@"
                CREATE TABLE `AspNetUsers` (
                    `Id` int NOT NULL AUTO_INCREMENT,
                    `FirstName` varchar(255) NOT NULL,
                    `LastName` varchar(255) NOT NULL,
                    `Created` datetime(6) NULL,
                    `CreatedBy` varchar(255) NULL,
                    `Modified` datetime(6) NULL,
                    `ModifiedBy` varchar(255) NULL,
                    `UserName` varchar(256) NULL,
                    `NormalizedUserName` varchar(256) NULL,
                    `Email` varchar(256) NULL,
                    `NormalizedEmail` varchar(256) NULL,
                    `EmailConfirmed` tinyint(1) NOT NULL,
                    `PasswordHash` longtext NULL,
                    `SecurityStamp` longtext NULL,
                    `ConcurrencyStamp` longtext NULL,
                    `PhoneNumber` longtext NULL,
                    `PhoneNumberConfirmed` tinyint(1) NOT NULL,
                    `TwoFactorEnabled` tinyint(1) NOT NULL,
                    `LockoutEnd` datetime(6) NULL,
                    `LockoutEnabled` tinyint(1) NOT NULL,
                    `AccessFailedCount` int NOT NULL,
                    PRIMARY KEY (`Id`)
                ) CHARACTER SET=utf8mb4;");

            // Recreate AspNetUserRoles
            migrationBuilder.Sql(@"
                CREATE TABLE `AspNetUserRoles` (
                    `UserId` int NOT NULL,
                    `RoleId` int NOT NULL,
                    PRIMARY KEY (`UserId`, `RoleId`),
                    CONSTRAINT `FK_AspNetUserRoles_AspNetUsers_UserId` FOREIGN KEY (`UserId`) REFERENCES `AspNetUsers` (`Id`) ON DELETE CASCADE,
                    CONSTRAINT `FK_AspNetUserRoles_AspNetRoles_RoleId` FOREIGN KEY (`RoleId`) REFERENCES `AspNetRoles` (`Id`) ON DELETE CASCADE
                ) CHARACTER SET=utf8mb4;");

            // Recreate AspNetUserClaims
            migrationBuilder.Sql(@"
                CREATE TABLE `AspNetUserClaims` (
                    `Id` int NOT NULL AUTO_INCREMENT,
                    `UserId` int NOT NULL,
                    `ClaimType` longtext NULL,
                    `ClaimValue` longtext NULL,
                    PRIMARY KEY (`Id`),
                    CONSTRAINT `FK_AspNetUserClaims_AspNetUsers_UserId` FOREIGN KEY (`UserId`) REFERENCES `AspNetUsers` (`Id`) ON DELETE CASCADE
                ) CHARACTER SET=utf8mb4;");

            // Recreate AspNetUserLogins
            migrationBuilder.Sql(@"
                CREATE TABLE `AspNetUserLogins` (
                    `LoginProvider` varchar(255) NOT NULL,
                    `ProviderKey` varchar(255) NOT NULL,
                    `ProviderDisplayName` longtext NULL,
                    `UserId` int NOT NULL,
                    PRIMARY KEY (`LoginProvider`, `ProviderKey`),
                    CONSTRAINT `FK_AspNetUserLogins_AspNetUsers_UserId` FOREIGN KEY (`UserId`) REFERENCES `AspNetUsers` (`Id`) ON DELETE CASCADE
                ) CHARACTER SET=utf8mb4;");

            // Recreate AspNetUserTokens
            migrationBuilder.Sql(@"
                CREATE TABLE `AspNetUserTokens` (
                    `UserId` int NOT NULL,
                    `LoginProvider` varchar(255) NOT NULL,
                    `Name` varchar(255) NOT NULL,
                    `Value` longtext NULL,
                    PRIMARY KEY (`UserId`, `LoginProvider`, `Name`),
                    CONSTRAINT `FK_AspNetUserTokens_AspNetUsers_UserId` FOREIGN KEY (`UserId`) REFERENCES `AspNetUsers` (`Id`) ON DELETE CASCADE
                ) CHARACTER SET=utf8mb4;");

            // Recreate AspNetRoleClaims
            migrationBuilder.Sql(@"
                CREATE TABLE `AspNetRoleClaims` (
                    `Id` int NOT NULL AUTO_INCREMENT,
                    `RoleId` int NOT NULL,
                    `ClaimType` longtext NULL,
                    `ClaimValue` longtext NULL,
                    PRIMARY KEY (`Id`),
                    CONSTRAINT `FK_AspNetRoleClaims_AspNetRoles_RoleId` FOREIGN KEY (`RoleId`) REFERENCES `AspNetRoles` (`Id`) ON DELETE CASCADE
                ) CHARACTER SET=utf8mb4;");

            migrationBuilder.Sql("SET FOREIGN_KEY_CHECKS = 1;");

            // Seed data via MigrationBuilder
            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Created", "CreatedBy", "Modified", "ModifiedBy", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { 1, null, new DateTime(2026, 4, 13, 10, 58, 8, 312, DateTimeKind.Local).AddTicks(9077), "admin", new DateTime(2026, 4, 13, 10, 58, 8, 312, DateTimeKind.Local).AddTicks(9094), "admin", "Administrator", "ADMINISTRATOR" },
                    { 2, null, new DateTime(2026, 4, 13, 10, 58, 8, 312, DateTimeKind.Local).AddTicks(9138), "admin", new DateTime(2026, 4, 13, 10, 58, 8, 312, DateTimeKind.Local).AddTicks(9139), "admin", "HeadTeacher", "HEADTEACHER" },
                    { 3, null, new DateTime(2026, 4, 13, 10, 58, 8, 312, DateTimeKind.Local).AddTicks(9160), "admin", new DateTime(2026, 4, 13, 10, 58, 8, 312, DateTimeKind.Local).AddTicks(9160), "admin", "Teacher", "TEACHER" },
                    { 4, null, new DateTime(2026, 4, 13, 10, 58, 8, 312, DateTimeKind.Local).AddTicks(9179), "admin", new DateTime(2026, 4, 13, 10, 58, 8, 312, DateTimeKind.Local).AddTicks(9180), "admin", "Student", "STUDENT" },
                    { 5, null, new DateTime(2026, 4, 13, 10, 58, 8, 312, DateTimeKind.Local).AddTicks(9198), "admin", new DateTime(2026, 4, 13, 10, 58, 8, 312, DateTimeKind.Local).AddTicks(9199), "admin", "Parent", "PARENT" },
                    { 6, null, new DateTime(2026, 4, 13, 10, 58, 8, 312, DateTimeKind.Local).AddTicks(9217), "admin", new DateTime(2026, 4, 13, 10, 58, 8, 312, DateTimeKind.Local).AddTicks(9218), "admin", "Accounts", "ACCOUNTS" },
                    { 7, null, new DateTime(2026, 4, 13, 10, 58, 8, 312, DateTimeKind.Local).AddTicks(9237), "admin", new DateTime(2026, 4, 13, 10, 58, 8, 312, DateTimeKind.Local).AddTicks(9237), "admin", "Others", "OTHERS" }
                });

            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] { "Id", "AccessFailedCount", "ConcurrencyStamp", "Created", "CreatedBy", "Email", "EmailConfirmed", "FirstName", "LastName", "LockoutEnabled", "LockoutEnd", "Modified", "ModifiedBy", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "SecurityStamp", "TwoFactorEnabled", "UserName" },
                values: new object[] { 1, 0, "000e9ebb-cdf4-4a10-b2b3-ee4b660e0fe3", new DateTime(2026, 4, 13, 10, 58, 8, 312, DateTimeKind.Local).AddTicks(9575), "admin", "admin@kodetek.co.ke", true, "SchoolSoft", "Administrator", false, null, new DateTime(2026, 4, 13, 10, 58, 8, 312, DateTimeKind.Local).AddTicks(9577), "admin", "ADMIN@KODETEK.CO.KE", "ADMIN", "AQAAAAIAAYagAAAAEN/xn+ca9bs2aJlGeoX8DhZZ7+6JwKY6YVpSOXRWvArG4kXtf80l704WF+YnqTKRwA==", "+254724920000", true, null, false, "admin" });

            migrationBuilder.UpdateData(
                table: "Designations",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 13, 10, 58, 8, 312, DateTimeKind.Local).AddTicks(9295), new DateTime(2026, 4, 13, 10, 58, 8, 312, DateTimeKind.Local).AddTicks(9296) });

            migrationBuilder.UpdateData(
                table: "EmploymentTypes",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 13, 10, 58, 8, 312, DateTimeKind.Local).AddTicks(9325), new DateTime(2026, 4, 13, 10, 58, 8, 312, DateTimeKind.Local).AddTicks(9326) });

            migrationBuilder.UpdateData(
                table: "Genders",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 13, 10, 58, 8, 312, DateTimeKind.Local).AddTicks(9404), new DateTime(2026, 4, 13, 10, 58, 8, 312, DateTimeKind.Local).AddTicks(9405) });

            migrationBuilder.UpdateData(
                table: "Nationalities",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 13, 10, 58, 8, 312, DateTimeKind.Local).AddTicks(9346), new DateTime(2026, 4, 13, 10, 58, 8, 312, DateTimeKind.Local).AddTicks(9346) });

            migrationBuilder.UpdateData(
                table: "Person",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "DateOfBirth", "EmploymentDate", "EndofEmploymentDate", "Modified" },
                values: new object[] { new DateTime(2026, 4, 13, 10, 58, 8, 312, DateTimeKind.Local).AddTicks(9451), new DateTime(2026, 4, 13, 10, 58, 8, 312, DateTimeKind.Local).AddTicks(9448), new DateTime(2026, 4, 13, 10, 58, 8, 312, DateTimeKind.Local).AddTicks(9444), new DateTime(2026, 4, 13, 10, 58, 8, 312, DateTimeKind.Local).AddTicks(9446), new DateTime(2026, 4, 13, 10, 58, 8, 312, DateTimeKind.Local).AddTicks(9457) });

            migrationBuilder.UpdateData(
                table: "Religions",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 13, 10, 58, 8, 312, DateTimeKind.Local).AddTicks(9380), new DateTime(2026, 4, 13, 10, 58, 8, 312, DateTimeKind.Local).AddTicks(9380) });

            migrationBuilder.UpdateData(
                table: "StaffCategories",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 13, 10, 58, 8, 312, DateTimeKind.Local).AddTicks(9274), new DateTime(2026, 4, 13, 10, 58, 8, 312, DateTimeKind.Local).AddTicks(9276) });

            migrationBuilder.InsertData(
                table: "AspNetUserRoles",
                columns: new[] { "RoleId", "UserId" },
                values: new object[] { 1, 1 });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { 1, 1 });

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "ToDoLists",
                type: "varchar(450)",
                maxLength: 450,
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "AspNetUserTokens",
                type: "varchar(255)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "Id",
                table: "AspNetUsers",
                type: "varchar(255)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AlterColumn<string>(
                name: "RoleId",
                table: "AspNetUserRoles",
                type: "varchar(255)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "AspNetUserRoles",
                type: "varchar(255)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "AspNetUserLogins",
                type: "varchar(255)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "AspNetUserClaims",
                type: "varchar(255)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "Id",
                table: "AspNetRoles",
                type: "varchar(255)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AlterColumn<string>(
                name: "RoleId",
                table: "AspNetRoleClaims",
                type: "varchar(255)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Created", "CreatedBy", "Modified", "ModifiedBy", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "269f0cf3-405e-4163-83f3-1b63ebebd62e", null, new DateTime(2026, 4, 13, 8, 40, 6, 333, DateTimeKind.Local).AddTicks(6081), "admin", new DateTime(2026, 4, 13, 8, 40, 6, 333, DateTimeKind.Local).AddTicks(6083), "admin", "Parent", "PARENT" },
                    { "448df289-142c-4959-a912-60733515e1b4", null, new DateTime(2026, 4, 13, 8, 40, 6, 333, DateTimeKind.Local).AddTicks(6058), "admin", new DateTime(2026, 4, 13, 8, 40, 6, 333, DateTimeKind.Local).AddTicks(6060), "admin", "Student", "STUDENT" },
                    { "48c50c3a-9958-453b-b649-4e21af131322", null, new DateTime(2026, 4, 13, 8, 40, 6, 333, DateTimeKind.Local).AddTicks(6023), "admin", new DateTime(2026, 4, 13, 8, 40, 6, 333, DateTimeKind.Local).AddTicks(6025), "admin", "Teacher", "TEACHER" },
                    { "717d9b15-a428-440c-b26b-08d3bbb68b02", null, new DateTime(2026, 4, 13, 8, 40, 6, 333, DateTimeKind.Local).AddTicks(5916), "admin", new DateTime(2026, 4, 13, 8, 40, 6, 333, DateTimeKind.Local).AddTicks(5937), "admin", "Administrator", "ADMINISTRATOR" },
                    { "95ed2407-3e58-4af2-88a4-1c4e96473f68", null, new DateTime(2026, 4, 13, 8, 40, 6, 333, DateTimeKind.Local).AddTicks(5987), "admin", new DateTime(2026, 4, 13, 8, 40, 6, 333, DateTimeKind.Local).AddTicks(5988), "admin", "HeadTeacher", "HEADTEACHER" },
                    { "97942bee-ef12-4425-8225-4f293d0f36dd", null, new DateTime(2026, 4, 13, 8, 40, 6, 333, DateTimeKind.Local).AddTicks(6131), "admin", new DateTime(2026, 4, 13, 8, 40, 6, 333, DateTimeKind.Local).AddTicks(6132), "admin", "Others", "OTHERS" },
                    { "cd12b44b-103b-48df-8887-a2bf42e0651e", null, new DateTime(2026, 4, 13, 8, 40, 6, 333, DateTimeKind.Local).AddTicks(6108), "admin", new DateTime(2026, 4, 13, 8, 40, 6, 333, DateTimeKind.Local).AddTicks(6110), "admin", "Accounts", "ACCOUNTS" }
                });

            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] { "Id", "AccessFailedCount", "ConcurrencyStamp", "Created", "CreatedBy", "Email", "EmailConfirmed", "FirstName", "LastName", "LockoutEnabled", "LockoutEnd", "Modified", "ModifiedBy", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "SecurityStamp", "TwoFactorEnabled", "UserName" },
                values: new object[] { "7e67d486-af3e-49f1-a109-a2b864b8e0ec", 0, "c8217ac2-8538-43de-914f-7dde0f2edcdb", new DateTime(2026, 4, 13, 8, 40, 6, 333, DateTimeKind.Local).AddTicks(6425), "admin", "admin@kodetek.co.ke", true, "SchoolSoft", "Administrator", false, null, new DateTime(2026, 4, 13, 8, 40, 6, 333, DateTimeKind.Local).AddTicks(6427), "admin", "ADMIN@KODETEK.CO.KE", "ADMIN", "AQAAAAIAAYagAAAAEOA2nKs+3xXFg3EwDh1mR8WbeyGvDdGBQwrB2GuZUJSdtGrpdycUYb6tc/GOyvRgMg==", "+254724920000", true, "86ac6180-3d87-4ef6-9dc2-1eed9b8e6fe7", false, "admin" });

            migrationBuilder.UpdateData(
                table: "Designations",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 13, 8, 40, 6, 333, DateTimeKind.Local).AddTicks(6194), new DateTime(2026, 4, 13, 8, 40, 6, 333, DateTimeKind.Local).AddTicks(6195) });

            migrationBuilder.UpdateData(
                table: "EmploymentTypes",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 13, 8, 40, 6, 333, DateTimeKind.Local).AddTicks(6225), new DateTime(2026, 4, 13, 8, 40, 6, 333, DateTimeKind.Local).AddTicks(6226) });

            migrationBuilder.UpdateData(
                table: "Genders",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 13, 8, 40, 6, 333, DateTimeKind.Local).AddTicks(6295), new DateTime(2026, 4, 13, 8, 40, 6, 333, DateTimeKind.Local).AddTicks(6295) });

            migrationBuilder.UpdateData(
                table: "Nationalities",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 13, 8, 40, 6, 333, DateTimeKind.Local).AddTicks(6250), new DateTime(2026, 4, 13, 8, 40, 6, 333, DateTimeKind.Local).AddTicks(6251) });

            migrationBuilder.UpdateData(
                table: "Person",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "DateOfBirth", "EmploymentDate", "EndofEmploymentDate", "Modified" },
                values: new object[] { new DateTime(2026, 4, 13, 8, 40, 6, 333, DateTimeKind.Local).AddTicks(6342), new DateTime(2026, 4, 13, 8, 40, 6, 333, DateTimeKind.Local).AddTicks(6338), new DateTime(2026, 4, 13, 8, 40, 6, 333, DateTimeKind.Local).AddTicks(6334), new DateTime(2026, 4, 13, 8, 40, 6, 333, DateTimeKind.Local).AddTicks(6335), new DateTime(2026, 4, 13, 8, 40, 6, 333, DateTimeKind.Local).AddTicks(6343) });

            migrationBuilder.UpdateData(
                table: "Religions",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 13, 8, 40, 6, 333, DateTimeKind.Local).AddTicks(6274), new DateTime(2026, 4, 13, 8, 40, 6, 333, DateTimeKind.Local).AddTicks(6274) });

            migrationBuilder.UpdateData(
                table: "StaffCategories",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Created", "Modified" },
                values: new object[] { new DateTime(2026, 4, 13, 8, 40, 6, 333, DateTimeKind.Local).AddTicks(6171), new DateTime(2026, 4, 13, 8, 40, 6, 333, DateTimeKind.Local).AddTicks(6173) });

            migrationBuilder.InsertData(
                table: "AspNetUserRoles",
                columns: new[] { "RoleId", "UserId" },
                values: new object[] { "717d9b15-a428-440c-b26b-08d3bbb68b02", "7e67d486-af3e-49f1-a109-a2b864b8e0ec" });
        }
    }
}
