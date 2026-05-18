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
            // Drop and recreate the two approval FK constraints with RESTRICT instead of CASCADE.
            // Uses MySQL information_schema to find the actual constraint names.
            var alterSteps = @"
                SET @sql = (SELECT CONCAT('ALTER TABLE ApprovalWorkflowSteps DROP FOREIGN KEY ', CONSTRAINT_NAME)
                            FROM information_schema.KEY_COLUMN_USAGE
                            WHERE TABLE_SCHEMA = DATABASE() AND TABLE_NAME = 'ApprovalWorkflowSteps'
                              AND REFERENCED_TABLE_NAME = 'ApprovalWorkflows' AND COLUMN_NAME = 'ApprovalWorkflowId' LIMIT 1);
                SET @sql = IFNULL(@sql, 'SELECT 1');
                PREPARE stmt FROM @sql; EXECUTE stmt; DEALLOCATE PREPARE stmt;
                ALTER TABLE ApprovalWorkflowSteps
                    ADD CONSTRAINT FK_ApprovalWorkflowSteps_ApprovalWorkflows_ApprovalWorkflowId
                    FOREIGN KEY (ApprovalWorkflowId) REFERENCES ApprovalWorkflows(Id) ON DELETE RESTRICT;
            ";
            var alterActions = @"
                SET @sql = (SELECT CONCAT('ALTER TABLE ApprovalStepActions DROP FOREIGN KEY ', CONSTRAINT_NAME)
                            FROM information_schema.KEY_COLUMN_USAGE
                            WHERE TABLE_SCHEMA = DATABASE() AND TABLE_NAME = 'ApprovalStepActions'
                              AND REFERENCED_TABLE_NAME = 'ApprovalRequests' AND COLUMN_NAME = 'ApprovalRequestId' LIMIT 1);
                SET @sql = IFNULL(@sql, 'SELECT 1');
                PREPARE stmt FROM @sql; EXECUTE stmt; DEALLOCATE PREPARE stmt;
                ALTER TABLE ApprovalStepActions
                    ADD CONSTRAINT FK_ApprovalStepActions_ApprovalRequests_ApprovalRequestId
                    FOREIGN KEY (ApprovalRequestId) REFERENCES ApprovalRequests(Id) ON DELETE RESTRICT;
            ";
            // MySQL doesn't allow multi-statement prepared blocks via ExecuteSqlRaw unless AllowUserVariables is enabled.
            // Split into individual statements.
            foreach (var stmt in SplitStatements(alterSteps).Concat(SplitStatements(alterActions)))
            {
                if (string.IsNullOrWhiteSpace(stmt)) continue;
                await db.Database.ExecuteSqlRawAsync(stmt);
            }
            logger.LogInformation("Approval FK cascade constraints set to RESTRICT.");
        }

        private static IEnumerable<string> SplitStatements(string block)
        {
            return block.Split(';', StringSplitOptions.RemoveEmptyEntries)
                .Select(s => s.Trim())
                .Where(s => s.Length > 0);
        }
    }
}
