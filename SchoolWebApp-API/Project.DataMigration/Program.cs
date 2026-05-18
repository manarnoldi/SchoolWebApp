using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Project.DataMigration;
using Project.DataMigration.Mssql;
using Project.DataMigration.Phases;
using Project.Infrastructure.Data;

// ------------- Configuration -------------------------------------------------
var config = new ConfigurationBuilder()
    .SetBasePath(AppContext.BaseDirectory)
    .AddJsonFile("appsettings.json", optional: false)
    .AddEnvironmentVariables()
    .AddCommandLine(args)
    .Build();

var mssqlConn  = config.GetConnectionString("SourceMssqlConnection")
    ?? throw new InvalidOperationException("ConnectionStrings:SourceMssqlConnection is missing.");
var mysqlConn  = config.GetConnectionString("TargetMySqlConnection")
    ?? throw new InvalidOperationException("ConnectionStrings:TargetMySqlConnection is missing.");
var createdBy  = config["Migration:CreatedBy"] ?? "legacy-migration";
var targetYear = int.TryParse(config["Migration:TargetYear"], out var y) ? y : 2026;

// CLI parsing: --phase=<name> | --phase=all (default all)
var requested = config["phase"] ?? "all";

// ------------- DI ------------------------------------------------------------
var services = new ServiceCollection();
services.AddLogging(b => b.AddSimpleConsole(o =>
{
    o.SingleLine        = true;
    o.TimestampFormat   = "HH:mm:ss ";
    o.IncludeScopes     = false;
}).SetMinimumLevel(LogLevel.Information));

services.AddSingleton<IHttpContextAccessor>(new MigrationHttpContextAccessor(createdBy));
services.AddDbContext<ApplicationDbContext>(opts =>
    opts.UseMySql(mysqlConn, ServerVersion.AutoDetect(mysqlConn)));

var provider = services.BuildServiceProvider();
var logger   = provider.GetRequiredService<ILogger<Program>>();

// ------------- Phase registry ------------------------------------------------
var allPhases = new IMigrationPhase[]
{
    new LearningModeSeedPhase(),
    new AcademicYearPhase(),
    new SchoolStreamPhase(),
    new DepartmentPhase(),
    new SubjectGroupPhase(),
    new SchoolClassPhase(),
    new SubjectPhase(),
    new PersonReferenceSeedPhase(),
    new StaffDetailsPhase(),
    new StudentPhase(),
    new StudentClassPhase(),
    new EmployeeSalaryPhase(),
    new EmployeeSalaryItemPhase(),
};

var toRun = string.Equals(requested, "all", StringComparison.OrdinalIgnoreCase)
    ? allPhases
    : allPhases.Where(p => string.Equals(p.Name, requested, StringComparison.OrdinalIgnoreCase)).ToArray();

if (toRun.Length == 0)
{
    logger.LogError("Unknown phase '{Phase}'. Available: {Phases}",
        requested, string.Join(", ", allPhases.Select(p => p.Name).Append("all")));
    return 1;
}

// ------------- Run -----------------------------------------------------------
logger.LogInformation("Source MSSQL : {Conn}", Redact(mssqlConn));
logger.LogInformation("Target MySQL : {Conn}", Redact(mysqlConn));
logger.LogInformation("Phases to run: {Phases}", string.Join(", ", toRun.Select(p => p.Name)));

var ctx = new MigrationContext { TargetYear = targetYear };
logger.LogInformation("Target year for year-scoped phases: {Year}", targetYear);
await using var src = new MssqlSource(mssqlConn);

try
{
    await src.OpenAsync();
}
catch (Exception ex)
{
    logger.LogError(ex, "Failed to open MSSQL source connection.");
    return 2;
}

using (var scope = provider.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    // Sanity: confirm we can talk to MySQL.
    try { await db.Database.OpenConnectionAsync(); }
    catch (Exception ex)
    {
        logger.LogError(ex, "Failed to open MySQL target connection.");
        return 3;
    }

    foreach (var phase in toRun)
    {
        logger.LogInformation("=== Phase: {Name} ===", phase.Name);
        try
        {
            await phase.RunAsync(db, src, ctx, logger, CancellationToken.None);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Phase {Name} failed.", phase.Name);
            return 4;
        }
    }
}

logger.LogInformation("Migration completed successfully.");
return 0;

static string Redact(string conn)
{
    // Mask passwords in log output.
    var parts = conn.Split(';');
    for (int i = 0; i < parts.Length; i++)
    {
        var kv = parts[i].Split('=', 2);
        if (kv.Length == 2 && kv[0].Trim().Equals("Password", StringComparison.OrdinalIgnoreCase))
            parts[i] = $"{kv[0]}=***";
    }
    return string.Join(';', parts);
}
