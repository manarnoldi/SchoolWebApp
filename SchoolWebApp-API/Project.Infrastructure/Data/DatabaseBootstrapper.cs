using System.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Project.Infrastructure.Data
{
    /// <summary>
    /// Applies idempotent schema adjustments and seed data at startup for changes that don't
    /// warrant a dedicated migration (e.g. role seeding, FK OnDelete tweaks, enum backfills).
    /// </summary>
    public static class DatabaseBootstrapper
    {
        public static async Task ApplyAsync(ApplicationDbContext db, ILogger logger)
        {
            try { await EnsureLogsTableAsync(db, logger); } catch (Exception ex) { logger.LogError(ex, "EnsureLogsTableAsync failed."); }
            try { await SeedSuperAdministratorRoleAsync(db, logger); } catch (Exception ex) { logger.LogError(ex, "SeedSuperAdministratorRoleAsync failed."); }
            try { await RestrictApprovalCascadeDeletesAsync(db, logger); } catch (Exception ex) { logger.LogError(ex, "RestrictApprovalCascadeDeletesAsync failed."); }
        }

        /// <summary>
        /// Creates the Logs table for fresh deploys, or migrates an existing
        /// legacy Serilog-sink table in place. The legacy schema had:
        ///   Id, Timestamp, Level, Template, Message, Exception, Properties, _ts
        /// (Template and Properties were NOT NULL — which would block NLog's
        /// INSERT that doesn't supply those columns.) We rename Timestamp →
        /// Logged, relax the NOT-NULL constraints on the leftover columns,
        /// and add the new NLog/audit fields. All idempotent so it's safe to
        /// run on every startup.
        /// </summary>
        private static async Task EnsureLogsTableAsync(ApplicationDbContext db, ILogger logger)
        {
            // Base table — used only when there's no existing Logs at all.
            await db.Database.ExecuteSqlRawAsync(@"
                CREATE TABLE IF NOT EXISTS Logs (
                    Id              INT NOT NULL AUTO_INCREMENT,
                    Logged          DATETIME(6) NOT NULL,
                    Level           VARCHAR(50)   NULL,
                    Message         TEXT          NULL,
                    Logger          VARCHAR(255)  NULL,
                    Exception       TEXT          NULL,
                    Url             VARCHAR(2048) NULL,
                    CallSite        VARCHAR(512)  NULL,
                    MachineName     VARCHAR(255)  NULL,
                    UserName        VARCHAR(255)  NULL,
                    Resolved        TINYINT(1) NOT NULL DEFAULT 0,
                    ResolvedBy      VARCHAR(255)  NULL,
                    ResolvedAt      DATETIME(6)   NULL,
                    ResolutionNote  TEXT          NULL,
                    PRIMARY KEY (Id)
                ) CHARACTER SET=utf8mb4;
            ");

            // Legacy Serilog schema had `Timestamp`; the new entity expects
            // `Logged`. Rename in place to preserve all existing rows.
            await RenameColumnIfPresentAsync(db, "Logs", "Timestamp", "Logged", "DATETIME(6) NOT NULL");

            // Legacy Serilog schema had Template/Properties as NOT NULL. NLog
            // doesn't supply values for those, so the inserts would fail.
            // Relax them; existing rows keep their data.
            await MakeColumnNullableIfPresentAsync(db, "Logs", "Template");
            await MakeColumnNullableIfPresentAsync(db, "Logs", "Properties");

            // Add the columns NLog needs (and the resolution-audit fields the
            // controller writes). Each call is independent + idempotent.
            await AddColumnIfMissingAsync(db, "Logs", "Logger",         "VARCHAR(255) NULL");
            await AddColumnIfMissingAsync(db, "Logs", "Url",            "VARCHAR(2048) NULL");
            await AddColumnIfMissingAsync(db, "Logs", "CallSite",       "VARCHAR(512) NULL");
            await AddColumnIfMissingAsync(db, "Logs", "MachineName",    "VARCHAR(255) NULL");
            await AddColumnIfMissingAsync(db, "Logs", "UserName",       "VARCHAR(255) NULL");
            await AddColumnIfMissingAsync(db, "Logs", "Resolved",       "TINYINT(1) NOT NULL DEFAULT 0");
            await AddColumnIfMissingAsync(db, "Logs", "ResolvedBy",     "VARCHAR(255) NULL");
            await AddColumnIfMissingAsync(db, "Logs", "ResolvedAt",     "DATETIME(6) NULL");
            await AddColumnIfMissingAsync(db, "Logs", "ResolutionNote", "TEXT NULL");

            logger.LogInformation("Logs table verified.");
        }

        /// <summary>
        /// Renames a column when the old name exists and the new name doesn't.
        /// CHANGE COLUMN also retypes, which is intentional — moving from
        /// TIMESTAMP to DATETIME(6) preserves all values within the TIMESTAMP
        /// range (1970-2038) without truncation.
        /// </summary>
        private static async Task RenameColumnIfPresentAsync(
            ApplicationDbContext db, string table, string oldName, string newName, string newColumnDef)
        {
            var conn = db.Database.GetDbConnection();
            var openedHere = conn.State != ConnectionState.Open;
            if (openedHere) await conn.OpenAsync();
            try
            {
                using var checkCmd = conn.CreateCommand();
                checkCmd.CommandText = $@"
                    SELECT COLUMN_NAME FROM information_schema.COLUMNS
                    WHERE TABLE_SCHEMA = DATABASE()
                      AND TABLE_NAME = '{table}'
                      AND COLUMN_NAME IN ('{oldName}', '{newName}')";
                bool oldPresent = false, newPresent = false;
                using (var reader = await ((System.Data.Common.DbCommand)checkCmd).ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        var name = reader.GetString(0);
                        if (string.Equals(name, oldName, StringComparison.OrdinalIgnoreCase)) oldPresent = true;
                        if (string.Equals(name, newName, StringComparison.OrdinalIgnoreCase)) newPresent = true;
                    }
                }
                if (!oldPresent || newPresent) return;

                using var alterCmd = conn.CreateCommand();
                alterCmd.CommandText = $"ALTER TABLE {table} CHANGE COLUMN {oldName} {newName} {newColumnDef}";
                await ((System.Data.Common.DbCommand)alterCmd).ExecuteNonQueryAsync();
            }
            finally
            {
                if (openedHere) await conn.CloseAsync();
            }
        }

        /// <summary>
        /// Drops the NOT NULL constraint on a column when present. Keeps the
        /// column's existing data type intact — we read it from
        /// information_schema and pass it back in the MODIFY clause so we
        /// don't accidentally widen / narrow the type.
        /// </summary>
        private static async Task MakeColumnNullableIfPresentAsync(
            ApplicationDbContext db, string table, string column)
        {
            var conn = db.Database.GetDbConnection();
            var openedHere = conn.State != ConnectionState.Open;
            if (openedHere) await conn.OpenAsync();
            try
            {
                using var checkCmd = conn.CreateCommand();
                checkCmd.CommandText = $@"
                    SELECT IS_NULLABLE, COLUMN_TYPE FROM information_schema.COLUMNS
                    WHERE TABLE_SCHEMA = DATABASE()
                      AND TABLE_NAME = '{table}'
                      AND COLUMN_NAME = '{column}'
                    LIMIT 1";
                string? isNullable = null;
                string? columnType = null;
                using (var reader = await ((System.Data.Common.DbCommand)checkCmd).ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        isNullable = reader.IsDBNull(0) ? null : reader.GetString(0);
                        columnType = reader.IsDBNull(1) ? null : reader.GetString(1);
                    }
                }
                if (string.IsNullOrEmpty(columnType)) return; // column doesn't exist
                if (string.Equals(isNullable, "YES", StringComparison.OrdinalIgnoreCase)) return; // already nullable

                using var alterCmd = conn.CreateCommand();
                alterCmd.CommandText = $"ALTER TABLE {table} MODIFY COLUMN {column} {columnType} NULL";
                await ((System.Data.Common.DbCommand)alterCmd).ExecuteNonQueryAsync();
            }
            finally
            {
                if (openedHere) await conn.CloseAsync();
            }
        }

        private static async Task AddColumnIfMissingAsync(
            ApplicationDbContext db, string table, string column, string columnDef)
        {
            var conn = db.Database.GetDbConnection();
            var openedHere = conn.State != ConnectionState.Open;
            if (openedHere) await conn.OpenAsync();
            try
            {
                using var checkCmd = conn.CreateCommand();
                checkCmd.CommandText = $@"
                    SELECT COUNT(*) FROM information_schema.COLUMNS
                    WHERE TABLE_SCHEMA = DATABASE()
                      AND TABLE_NAME = '{table}'
                      AND COLUMN_NAME = '{column}'";
                var count = Convert.ToInt32(await ((System.Data.Common.DbCommand)checkCmd).ExecuteScalarAsync());
                if (count > 0) return;

                using var alterCmd = conn.CreateCommand();
                alterCmd.CommandText = $"ALTER TABLE {table} ADD COLUMN {column} {columnDef}";
                await ((System.Data.Common.DbCommand)alterCmd).ExecuteNonQueryAsync();
            }
            finally
            {
                if (openedHere) await conn.CloseAsync();
            }
        }

        private static async Task SeedSuperAdministratorRoleAsync(ApplicationDbContext db, ILogger logger)
        {
            // Insert the SuperAdministrator role at Id=8 if missing, and attach the default admin user (Id=1).
            await db.Database.ExecuteSqlRawAsync(@"
                INSERT INTO AspNetRoles (Id, Name, NormalizedName, ConcurrencyStamp, Created, CreatedBy, Modified, ModifiedBy)
                SELECT 8, 'SuperAdministrator', 'SUPERADMINISTRATOR', UUID(), NOW(), 'admin', NOW(), 'admin'
                WHERE NOT EXISTS (SELECT 1 FROM AspNetRoles WHERE Id = 8);

                INSERT INTO AspNetUserRoles (UserId, RoleId)
                SELECT 1, 8
                WHERE NOT EXISTS (SELECT 1 FROM AspNetUserRoles WHERE UserId = 1 AND RoleId = 8)
                  AND EXISTS (SELECT 1 FROM AspNetUsers WHERE Id = 1);
            ");
            logger.LogInformation("SuperAdministrator role seed verified.");
        }

        private static async Task RestrictApprovalCascadeDeletesAsync(ApplicationDbContext db, ILogger logger)
        {
            // Force RESTRICT on the two approval FKs. We can't use MySQL user variables
            // (@sql) across separate ExecuteSqlRawAsync calls because each call pulls a
            // new pooled connection and user variables are per-connection. So we open
            // one connection explicitly and run discrete commands on it.
            var conn = db.Database.GetDbConnection();
            var openedHere = conn.State != ConnectionState.Open;
            if (openedHere) await conn.OpenAsync();
            try
            {
                await EnsureRestrictAsync(conn,
                    table: "ApprovalWorkflowSteps",
                    referencedTable: "ApprovalWorkflows",
                    column: "ApprovalWorkflowId",
                    newConstraintName: "FK_ApprovalWorkflowSteps_ApprovalWorkflows_ApprovalWorkflowId",
                    logger: logger);

                await EnsureRestrictAsync(conn,
                    table: "ApprovalStepActions",
                    referencedTable: "ApprovalRequests",
                    column: "ApprovalRequestId",
                    newConstraintName: "FK_ApprovalStepActions_ApprovalRequests_ApprovalRequestId",
                    logger: logger);
            }
            finally
            {
                if (openedHere) await conn.CloseAsync();
            }
        }

        private static async Task EnsureRestrictAsync(
            IDbConnection conn, string table, string referencedTable, string column,
            string newConstraintName, ILogger logger)
        {
            // 1. Look up the current DELETE_RULE. If already RESTRICT/NO ACTION, do nothing.
            var currentRule = await ScalarAsync(conn,
                $@"SELECT rc.DELETE_RULE
                   FROM information_schema.REFERENTIAL_CONSTRAINTS rc
                   JOIN information_schema.KEY_COLUMN_USAGE k
                     ON k.CONSTRAINT_SCHEMA = rc.CONSTRAINT_SCHEMA
                    AND k.CONSTRAINT_NAME = rc.CONSTRAINT_NAME
                   WHERE rc.CONSTRAINT_SCHEMA = DATABASE()
                     AND rc.TABLE_NAME = '{table}'
                     AND rc.REFERENCED_TABLE_NAME = '{referencedTable}'
                     AND k.COLUMN_NAME = '{column}'
                   LIMIT 1");

            if (string.Equals(currentRule, "RESTRICT", StringComparison.OrdinalIgnoreCase)
                || string.Equals(currentRule, "NO ACTION", StringComparison.OrdinalIgnoreCase))
            {
                return; // already correct, no-op
            }

            // 2. Find the existing constraint name (if any) so we can drop it.
            var existingFk = await ScalarAsync(conn,
                $@"SELECT CONSTRAINT_NAME
                   FROM information_schema.KEY_COLUMN_USAGE
                   WHERE TABLE_SCHEMA = DATABASE()
                     AND TABLE_NAME = '{table}'
                     AND REFERENCED_TABLE_NAME = '{referencedTable}'
                     AND COLUMN_NAME = '{column}'
                   LIMIT 1");

            if (!string.IsNullOrEmpty(existingFk))
            {
                await NonQueryAsync(conn, $"ALTER TABLE {table} DROP FOREIGN KEY {existingFk}");
            }

            // 3. Add the new constraint with ON DELETE RESTRICT.
            await NonQueryAsync(conn,
                $@"ALTER TABLE {table}
                   ADD CONSTRAINT {newConstraintName}
                   FOREIGN KEY ({column}) REFERENCES {referencedTable}(Id) ON DELETE RESTRICT");

            logger.LogInformation("FK {Table}.{Column} -> {Ref} set to RESTRICT.", table, column, referencedTable);
        }

        private static async Task<string> ScalarAsync(IDbConnection conn, string sql)
        {
            using var cmd = conn.CreateCommand();
            cmd.CommandText = sql;
            var result = await ((System.Data.Common.DbCommand)cmd).ExecuteScalarAsync();
            return result?.ToString();
        }

        private static async Task NonQueryAsync(IDbConnection conn, string sql)
        {
            using var cmd = conn.CreateCommand();
            cmd.CommandText = sql;
            await ((System.Data.Common.DbCommand)cmd).ExecuteNonQueryAsync();
        }
    }
}
