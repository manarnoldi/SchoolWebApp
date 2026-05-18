using Microsoft.EntityFrameworkCore;
using Project.Infrastructure.Data;

namespace Project.DataMigration.Phases;

public static class PhaseHelpers
{
    // Truncate one or more MySQL tables. Disables FK checks for the duration so callers
    // don't need to track FK ordering, then re-enables them. AUTO_INCREMENT resets to 1.
    public static async Task TruncateAsync(ApplicationDbContext db, params string[] tableNames)
    {
        await db.Database.ExecuteSqlRawAsync("SET FOREIGN_KEY_CHECKS = 0;");
        try
        {
            foreach (var t in tableNames)
            {
                // Table names are hard-coded literals from this codebase (not user input),
                // so the EF1002 interpolation warning is suppressed for this call.
#pragma warning disable EF1002
                await db.Database.ExecuteSqlRawAsync($"TRUNCATE TABLE `{t}`;");
#pragma warning restore EF1002
            }
        }
        finally
        {
            await db.Database.ExecuteSqlRawAsync("SET FOREIGN_KEY_CHECKS = 1;");
        }
    }
}
