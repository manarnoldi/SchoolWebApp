using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SchoolWebApp.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddAuditLogsTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // AuditLogs - append-only "who did what" table used by
            // the SaveChanges interceptor and IAuditService.
            migrationBuilder.CreateTable(
                name: "AuditLogs",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Timestamp = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    UserId = table.Column<string>(type: "varchar(450)", maxLength: 450, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    UserName = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Action = table.Column<string>(type: "varchar(40)", maxLength: 40, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    EntityType = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    EntityId = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    OldValues = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    NewValues = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IpAddress = table.Column<string>(type: "varchar(64)", maxLength: 64, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    UserAgent = table.Column<string>(type: "varchar(512)", maxLength: 512, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    RequestPath = table.Column<string>(type: "varchar(512)", maxLength: 512, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Notes = table.Column<string>(type: "varchar(2048)", maxLength: 2048, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AuditLogs", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            // Logs - already exists in production because NLog created
            // it at first write. The Log entity got added to the DbSet
            // after the previous migrations were generated, so EF only
            // picks it up now. Use CREATE TABLE IF NOT EXISTS so this
            // migration is safe to run against both fresh and existing
            // databases.
            migrationBuilder.Sql(@"
                CREATE TABLE IF NOT EXISTS `Logs` (
                    `Id` int NOT NULL AUTO_INCREMENT,
                    `Logged` datetime(6) NOT NULL,
                    `Level` varchar(50) CHARACTER SET utf8mb4 NULL,
                    `Message` text CHARACTER SET utf8mb4 NULL,
                    `Logger` varchar(255) CHARACTER SET utf8mb4 NULL,
                    `Exception` text CHARACTER SET utf8mb4 NULL,
                    `Url` varchar(2048) CHARACTER SET utf8mb4 NULL,
                    `CallSite` varchar(512) CHARACTER SET utf8mb4 NULL,
                    `MachineName` varchar(255) CHARACTER SET utf8mb4 NULL,
                    `UserName` varchar(255) CHARACTER SET utf8mb4 NULL,
                    `Resolved` tinyint(1) NOT NULL DEFAULT 0,
                    `ResolvedBy` varchar(255) CHARACTER SET utf8mb4 NULL,
                    `ResolvedAt` datetime(6) NULL,
                    `ResolutionNote` text CHARACTER SET utf8mb4 NULL,
                    CONSTRAINT `PK_Logs` PRIMARY KEY (`Id`)
                ) CHARACTER SET=utf8mb4;
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AuditLogs");

            // Intentionally do NOT drop Logs on rollback - it pre-existed
            // this migration via NLog and removing it would silently
            // wipe historical error logs.
        }
    }
}
