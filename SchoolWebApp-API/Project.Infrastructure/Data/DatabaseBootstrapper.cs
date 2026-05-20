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
            try { await SeedSuperAdministratorRoleAsync(db, logger); } catch (Exception ex) { logger.LogError(ex, "SeedSuperAdministratorRoleAsync failed."); }
            try { await RestrictApprovalCascadeDeletesAsync(db, logger); } catch (Exception ex) { logger.LogError(ex, "RestrictApprovalCascadeDeletesAsync failed."); }
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
